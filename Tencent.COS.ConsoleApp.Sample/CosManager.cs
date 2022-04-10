using COSXML;

namespace Tencent.COS.ConsoleApp.Sample
{
    public class CosManager
    {
        public static void InitTencentCos()
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
        }
    }
}