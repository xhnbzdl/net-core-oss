using System.IO;
using System.Threading.Tasks;
using COSXML;

namespace Tencent.COS.WebApi.Sample.Services
{
    public interface ICosService
    {
        public void PutBucket(string bucket);
        public void DeleteBucket(string bucket);
        public string PutObject(string fileName, FileStream stream);
        public string PutObject(string fileName, byte[] data);
        public byte[] GetObjectToByte(string cosKey);
        public string DeleteObject(string cosKey);
    }
}