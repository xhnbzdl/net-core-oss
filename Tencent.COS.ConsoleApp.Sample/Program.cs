using System;
using System.IO;
using System.Threading.Tasks;

namespace Tencent.COS.ConsoleApp.Sample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var appId = "1303196211";
            var bucket = $"aspnetcoredemo-{appId}";
            var cosXmlServer = CosManager.InitCosXmlServer();
            try
            {
                // 创建存储痛
                Console.WriteLine(" ======= Put Bucket ======");
                //CosManager.PutBucket(cosXmlServer, bucket);

                #region 上传对象
                Console.WriteLine(" ======= Put Object ======");
                string txtFilePath = @"C:\Users\hyx\Desktop\工作学习.txt";
                string txtFileName = Path.GetFileName(txtFilePath);
                string imgFilePath = @"C:\Users\hyx\Pictures\微信图片_20220130182305.jpg";
                string imgFileName = Path.GetFileName(imgFilePath);
                string pdfFIlePath = @"E:\学习文件\毕设\简历\黄宇翔.NET开发工程师19918493966.pdf";
                string pdfFileName = Path.GetFileName(pdfFIlePath);
                // 本地路径上传 task
                //await CosManager.PutObject(cosXmlServer, bucket,txtFileName, txtFilePath);
                // 本地路径上传 request
                //CosManager.PutObject(cosXmlServer, bucket, imgFilePath);

                #region 二进制上传
                //FileStream stream = File.OpenRead(txtFilePath);
                //var bytes = new byte[stream.Length];
                //await stream.ReadAsync(bytes, 0, bytes.Length);
                //CosManager.PutObject(cosXmlServer, bucket, txtFileName, bytes);
                #endregion

                //// 文件流上传
                //CosManager.PutObject(cosXmlServer, bucket, txtFileName, File.OpenRead(txtFilePath));
                //CosManager.PutObject(cosXmlServer, bucket, imgFileName, File.OpenRead(imgFilePath));
                //CosManager.PutObject(cosXmlServer, bucket, pdfFileName, File.OpenRead(pdfFIlePath));

                // 创建目录
                //CosManager.PutObject(cosXmlServer, bucket, "demoDir/",Array.Empty<byte>());
                // 上传到指定目录
                //CosManager.PutObject(cosXmlServer, bucket, $"demoDir/{imgFileName}", File.OpenRead(imgFilePath));
                #endregion

                #region 下载对象
                Console.WriteLine(" ======= Get Object ======");
                //await CosManager.GetObject(cosXmlServer, bucket, txtFileName);
                //CosManager.GetObjectToFile(cosXmlServer, bucket, imgFileName);
                #endregion

                #region 删除对象
                Console.WriteLine(" ======= Delete Object ======");
                //CosManager.DeleteObject(cosXmlServer,bucket, "streamObject-pdf");
                #endregion

                Console.WriteLine(" ======= Put Directory ======");
                //CosManager.UploadDirectory(cosXmlServer,bucket, @"C:\Users\hyx\Pictures");
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                Console.WriteLine("CosClientException: " + clientEx.Message);
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                Console.WriteLine("CosServerException: " + serverEx.GetInfo());
            }

            finally
            {
                // 删除存储桶
                Console.WriteLine(" ======= Delete Bucket ======");
            }
        }
    }
}
