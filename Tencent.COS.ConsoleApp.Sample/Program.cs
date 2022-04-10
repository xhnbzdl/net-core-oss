using System;

namespace Tencent.COS.ConsoleApp.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // 获取系统环境变量
            string secretId = Environment.GetEnvironmentVariable("SECRET_ID");
            Console.WriteLine(secretId);
        }
    }
}
