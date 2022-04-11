using System;
using COSXML;
using COSXML.Auth;
using COSXML.Model.Bucket;
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
        public void InitCosXmlServer()
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
    }
}