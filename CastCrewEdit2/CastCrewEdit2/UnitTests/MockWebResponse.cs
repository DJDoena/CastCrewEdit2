using System.IO;
using System.Net;

namespace UnitTests
{
    internal class MockWebResponse : WebResponse
    {
        private string FileName { get; set; }

        private Stream FileStream { get; set; }

        public MockWebResponse(string fileName)
        {
            FileName = fileName;
        }

        public override void Close()
        {
            if (FileStream != null)
            {
                FileStream.Close();
                FileStream.Dispose();
                FileStream = null;
            }
        }

        public override Stream GetResponseStream()
        {
            if (FileStream != null)
            {
                Close();
            }

            FileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            return (FileStream);
        }
    }
}