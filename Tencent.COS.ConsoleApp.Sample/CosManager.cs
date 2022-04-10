using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using COSXML;
using COSXML.Auth;
using COSXML.Model.Bucket;
using COSXML.Model.Object;
using COSXML.Model.Tag;
using COSXML.Transfer;

namespace Tencent.COS.ConsoleApp.Sample
{
    public class CosManager
    {
        public static CosXmlServer InitCosXmlServer()
        {
            // 腾讯云 SecretId
            string secretId = "AKIDUnrgGbgW6RWMwIOFqCJAm8E4f05vVpuj";
            // 腾讯云 SecretKey
            string secretKey = "bw2siJflcFdmfIgohKhusb4HExQhjVWI";
            // 存储桶所在地域
            string region = "ap-nanjing";

            // 普通初始化方式
            CosXmlConfig config = new CosXmlConfig.Builder()
                .SetRegion(region)
                .SetDebugLog(true)
                .Build();

            long keyDurationSecond = 600;
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, keyDurationSecond);

            // service 初始化完成
            CosXmlServer cosXml = new CosXmlServer(config, qCloudCredentialProvider);
            return cosXml;

        }
        /// <summary>
        /// 创建存储桶
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="bucket"></param>
        public static void PutBucket(CosXmlServer cosXml,string bucket)
        {
            try
            {
                PutBucketRequest request = new PutBucketRequest(bucket);

                //执行请求
                PutBucketResult result = cosXml.PutBucket(request);

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
        /// <summary>
        /// 删除存储桶
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="bucket"></param>
        public static void DeleteBucket(CosXmlServer cosXml,string bucket)
        {
            DeleteBucketRequest request = new DeleteBucketRequest(bucket);

            DeleteBucketResult result = cosXml.DeleteBucket(request);

            Console.WriteLine(result.GetResultInfo());
        }

        /// <summary>
        /// 上传对象,指定文件的本地路径，Task方式
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="bucket"></param>
        /// <param name="filePath"></param>
        /// <param name="cosKey">对象在存储桶中的位置标识符，即称对象键</param>
        /// <returns></returns>
        public static async Task<string> PutObject(CosXmlServer cosXml,string bucket,string cosKey, string filePath)
        {
            //.cssg-snippet-body-start:[transfer-upload-file]
            // 初始化 TransferConfig
            TransferConfig transferConfig = new TransferConfig();

            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(cosXml, transferConfig);
            //本地文件绝对路径
            string srcPath = filePath;

            // 上传对象
            COSXMLUploadTask uploadTask = new COSXMLUploadTask(bucket, cosKey);
            uploadTask.SetSrcPath(srcPath);

            uploadTask.progressCallback = delegate (long completed, long total)
            {
                Console.WriteLine($"progress = {completed * 100.0 / total:##.##}%");
            };

            try
            {
                COSXML.Transfer.COSXMLUploadTask.UploadTaskResult result = await
                    transferManager.UploadAsync(uploadTask);
                Console.WriteLine(result.GetResultInfo());
                string eTag = result.eTag;
            }
            catch (Exception e)
            {
                Console.WriteLine("CosException: " + e);
            }

            return cosKey;
        }

        /// <summary>
        /// 上传对象，以二进制文件
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="bucket"></param>
        /// <param name="data"></param>
        /// <param name="cosKey"></param>
        /// <param name="fileName"></param>
        public static void PutObject(CosXmlServer cosXml, string bucket, string fileName, byte[] data)
        {
            try
            {
                var cosKey = fileName;
                PutObjectRequest putObjectRequest = new PutObjectRequest(bucket, cosKey, data);
                // 发起上传
                PutObjectResult result = cosXml.PutObject(putObjectRequest);

                Console.WriteLine(result.GetResultInfo());
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
        }

        /// <summary>
        /// 上传对象，以文件流
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="bucket"></param>
        /// <param name="stream"></param>
        /// <param name="cosKey"></param>
        /// <param name="fileName"></param>
        public static void PutObject(CosXmlServer cosXml,string bucket,string fileName, FileStream stream)
        {
            try
            {
                // 组装上传请求，其中 offset sendLength 为可选参数
                long offset = 0L;
                long sendLength = stream.Length;

                var cosKey = fileName;
                PutObjectRequest request = new PutObjectRequest(bucket, cosKey, stream, offset, sendLength);
                //request.SetRequestHeader("Content-Type", GetContentType(Path.GetExtension(fileName)));
                //设置进度回调
                request.SetCosProgressCallback(delegate (long completed, long total)
                {
                    Console.WriteLine($"progress = {completed * 100.0 / total:##.##}%");
                });
                //执行请求
                PutObjectResult result = cosXml.PutObject(request);
                //关闭文件流
                stream.Close();
                //打印请求结果
                Console.WriteLine(result.GetResultInfo());
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
        }
        /// <summary>
        /// 上传对象，指定文件本地路径，Request方式
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="bucket"></param>
        /// <param name="cosKey"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public static void PutObject(CosXmlServer cosXml, string bucket, string filePath)
        {
            try
            {
                var cosKey = Path.GetFileName(filePath);
                PutObjectRequest request = new PutObjectRequest(bucket, cosKey,filePath);
                //设置进度回调
                request.SetCosProgressCallback(delegate (long completed, long total)
                {
                    Console.WriteLine($"progress = {completed * 100.0 / total:##.##}%");
                });
                //执行请求
                PutObjectResult result = cosXml.PutObject(request);

                //打印请求结果
                Console.WriteLine(result.GetResultInfo());
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
        }

        /// <summary>
        /// 上传文件夹内的文件
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="bucket"></param>
        /// <param name="directoryPath"></param>
        public static void UploadDirectory(CosXmlServer cosXml, string bucket,string directoryPath)
        {
            //.cssg-snippet-body-start:[transfer-upload-file]
            // 初始化 TransferConfig
            TransferConfig transferConfig = new TransferConfig();

            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(cosXml, transferConfig);

            var files = System.IO.Directory.GetFiles(directoryPath);

            var tasks = new List<Task>();
            foreach (var file in files)
            {
                Console.WriteLine("Enqueue Upload: " + file);
                //对象在存储桶中的位置标识符，即称对象键
                string cosPath = new FileInfo(file).Name;

                // 上传对象
                COSXMLUploadTask uploadTask = new COSXMLUploadTask(bucket, cosPath);
                uploadTask.SetSrcPath(file);

                tasks.Add(transferManager.UploadAsync(uploadTask));
            }

            try
            {
                // Wait for all the tasks to finish.
                Task.WaitAll(tasks.ToArray());

                // We should never get to this point
                Console.WriteLine("Upload Directory Complete");
            }
            catch (AggregateException e)
            {
                Console.WriteLine("\nThe following exceptions have been thrown by WaitAll(): (THIS WAS EXPECTED)");
                for (int j = 0; j < e.InnerExceptions.Count; j++)
                {
                    Console.WriteLine("\n-------------------------------------------------\n{0}", e.InnerExceptions[j].ToString());
                }
            }

        }

        /// <summary>
        /// 获取下载对象 task
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="cosKey"></param>
        /// <param name="bucket"></param>
        /// <param name="saveFileName"></param>
        /// <returns></returns>
        public static async Task GetObject(CosXmlServer cosXml, string bucket,string cosKey)
        {
            TransferConfig transferConfig = new TransferConfig();

            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(cosXml, transferConfig);

            //本地文件夹：临时文件 C:\Users\UserName\AppData\Local\Temp\
            string localDir = System.IO.Path.GetTempPath();
            //指定本地保存的文件名
            string localFileName = cosKey;

            // 下载对象
            COSXMLDownloadTask downloadTask = new COSXMLDownloadTask(bucket, cosKey,
                localDir, localFileName);

            downloadTask.progressCallback = delegate (long completed, long total)
            {
                Console.WriteLine($"progress = {completed * 100.0 / total:##.##}%");
            };

            try
            {
                COSXML.Transfer.COSXMLDownloadTask.DownloadTaskResult result = await
                    transferManager.DownloadAsync(downloadTask);
                Console.WriteLine(result.GetResultInfo());
            }
            catch (Exception e)
            {
                Console.WriteLine("CosException: " + e);
            }
        }
        /// <summary>
        /// 获取下载对象 request
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="bucket"></param>
        /// <param name="cosKey"></param>
        /// <param name="saveFileName"></param>
        public static void GetObjectToFile(CosXmlServer cosXml, string bucket, string cosKey)
        {
            try
            {
                string localDir = System.IO.Path.GetTempPath();//本地文件夹
                string localFileName = cosKey; //指定本地保存的文件名
                GetObjectRequest request = new GetObjectRequest(bucket, cosKey, localDir, localFileName);
                //设置进度回调
                request.SetCosProgressCallback(delegate (long completed, long total)
                {
                    Console.WriteLine($"progress = {completed * 100.0 / total:##.##}%");
                });
                //执行请求
                GetObjectResult result = cosXml.GetObject(request);
                //请求成功
                Console.WriteLine(result.GetResultInfo());
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

        }
        /// <summary>
        /// 下载对象，返回bytes
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="bucket"></param>
        /// <param name="cosKey"></param>
        public void GetObjectToMemory(CosXmlServer cosXml, string bucket, string cosKey)
        {
            try
            {
                GetObjectBytesRequest request = new GetObjectBytesRequest(bucket, cosKey);
                //设置进度回调
                request.SetCosProgressCallback(delegate (long completed, long total)
                {
                    Console.WriteLine($"progress = {completed * 100.0 / total:##.##}%");
                });
                //执行请求
                GetObjectBytesResult result = cosXml.GetObject(request);
                //获取内容到 byte 数组中
                byte[] content = result.content;
                //请求成功
                Console.WriteLine(result.GetResultInfo());
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
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="cosXml"></param>
        /// <param name="cosKey"></param>
        /// <param name="bucket"></param>
        public static void DeleteObject(CosXmlServer cosXml,string bucket, string cosKey)
        {
            DeleteObjectRequest request = new DeleteObjectRequest(bucket, cosKey);

            DeleteObjectResult result = cosXml.DeleteObject(request);

            Console.WriteLine(result.GetResultInfo());
        }
        /// <summary>
        /// 获取不同文件对应的contentType
        /// </summary>
        /// <param name="fileExt"></param>
        /// <returns></returns>
        private static string GetContentType(string fileExt)
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