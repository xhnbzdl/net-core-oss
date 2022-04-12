using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using COSXML;
using COSXML.Auth;
using COSXML.Model.Bucket;
using COSXML.Model.Object;
using COSXML.Transfer;
using Microsoft.Extensions.Options;

namespace Tencent.COS.WebApi.Sample.Services
{
    public class CosService:ICosService
    {
        private readonly TencentCosConfig _config;
        private static CosXmlServer _cosXmlServer = null;
        
        public CosService(IOptions<TencentCosConfig> config)
        {
            // 如果使用Nacos接入配置，请使用IOptionsSnapshot，因为IOptions不能同步更新
            _config = config.Value;
            if (_cosXmlServer == null)
            {
                InitCosXmlServer();
            }
        }
        private void InitCosXmlServer()
        {
            // 腾讯云 SecretId
            string secretId = _config.SecretId;
            // 腾讯云 SecretKey
            string secretKey = _config.SecretKey;
            // 存储桶所在地域
            string region = _config.Region;

            // 普通初始化方式
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetRegion(region)
                .SetDebugLog(true)
                .Build();

            long keyDurationSecond = 600;
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, keyDurationSecond);

            // service 初始化完成
            CosXmlServer cosXml = new CosXmlServer(config, qCloudCredentialProvider);
            _cosXmlServer = cosXml;

        }

        public void PutBucket(string bucket)
        {
            try
            {
                PutBucketRequest request = new PutBucketRequest($"{bucket}-{_config.AppId}");

                //执行请求
                PutBucketResult result = _cosXmlServer.PutBucket(request);

                Console.WriteLine(result.GetResultInfo());
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                if (serverEx.statusCode != 409)
                {
                    throw serverEx;
                }
                else
                {
                    Console.WriteLine("Bucket Already exists.");
                }
            }
        }

        public void DeleteBucket(string bucket)
        {
            DeleteBucketRequest request = new DeleteBucketRequest(bucket);

            DeleteBucketResult result = _cosXmlServer.DeleteBucket(request);

            Console.WriteLine(result.GetResultInfo());
        }

        public string PutObject(string fileName, FileStream stream)
        {
            string resultString = null;
            try
            {
                // 组装上传请求，其中 offset sendLength 为可选参数
                long offset = 0L;
                long sendLength = stream.Length;

                var cosKey = fileName;
                PutObjectRequest request = new PutObjectRequest(_config.Bucket, cosKey, stream, offset, sendLength);
                //执行请求
                PutObjectResult result = _cosXmlServer.PutObject(request);
                //关闭文件流
                stream.Close();
                //请求结果
                resultString = result.GetResultInfo();
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                //请求失败
                Console.WriteLine("CosClientException: " + clientEx);
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                //请求失败
                Console.WriteLine("CosServerException: " + serverEx.GetInfo());
            }

            return resultString;
        }

        public string PutObject(string fileName, byte[] data)
        {
            string resultString = null;
            try
            {
                var cosKey = fileName;
                PutObjectRequest putObjectRequest = new PutObjectRequest(_config.Bucket, cosKey, data);
                // 发起上传
                PutObjectResult result = _cosXmlServer.PutObject(putObjectRequest);

                resultString = result.GetResultInfo();
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                //请求失败
                Console.WriteLine("CosClientException: " + clientEx);
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                //请求失败
                Console.WriteLine("CosServerException: " + serverEx.GetInfo());
            }

            return resultString;
        }

        public byte[] GetObjectToByte(string cosKey)
        {
            byte [] data = null;
            try
            {
                GetObjectBytesRequest request = new GetObjectBytesRequest(_config.Bucket, cosKey);
                //执行请求
                GetObjectBytesResult result = _cosXmlServer.GetObject(request);
                //获取内容到 byte 数组中
                data = result.content;
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                //请求失败
                Console.WriteLine("CosClientException: " + clientEx);
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                //请求失败
                Console.WriteLine("CosServerException: " + serverEx.GetInfo());
            }

            return data;
        }

        public string DeleteObject(string cosKey)
        {
            DeleteObjectRequest request = new DeleteObjectRequest(_config.Bucket, cosKey);

            DeleteObjectResult result = _cosXmlServer.DeleteObject(request);

            return result.GetResultInfo();
        }

        public static string GetContentType(string fileExt)
        {
            Dictionary<string, string> map = new Dictionary<string, string>
            {
                {".png","image/png" },
                {".jpg","image/jpg" },
                {".pdf","application/pdf" },
                {".gif","image/gif" },
                {".jpeg","image/jpeg" },
            };
            if (map.ContainsKey(fileExt.ToLower()))
            {
                return map[fileExt];
            }
            return "application/octet-stream";
        }
    }
}