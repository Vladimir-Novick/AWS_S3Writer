using AWS_S3Writer.ClientS3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AWS_S3Writer_UnitTest
{
    public class UnitTest1
    {
        readonly string AmazonS3AccessKey = "xxxxxxxxxxxxxxxxxxxxxxxx";
        readonly string AmazonS3SecretKey = "yyyyyyyyyyyyyyyyyyyyyyyyy";
        readonly string AmazonS3ServiceURL = "http://vvvvvvvvvvv.amazonaws.com";
        readonly string backet_name = "test";

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        private readonly ITestOutputHelper output;

        [Fact]
        public void CreateClient()
        {
            try
            {
                DateTime start = DateTime.Now;
                output.WriteLine($"Start{start}");

                using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
                {

                }

                DateTime end = DateTime.Now;

                output.WriteLine($"End: {end} ");

                output.WriteLine("ok");
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.Message);
            }
        }

        [Fact]
        public void GetDirList()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                List<String> list = client.GetDirList(backet_name);
                foreach (var s in list)
                {
                    output.WriteLine(s);
                }
            }
        }

        [Fact]
        public void GetDirListFilter()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                List<String> list = client.GetDirList(backet_name, "tst");
                foreach (var s in list)
                {
                    output.WriteLine(s);
                }
            }
        }

        [Fact]
        public void GetFileList()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                List<String> list = client.GetFileList(backet_name);
                foreach (var s in list)
                {
                    output.WriteLine(s);
                }
            }
        }

        [Fact]
        public void GetFileListFilter()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                List<String> list = client.GetFileList(backet_name, "tst");
                foreach (var s in list)
                {
                    output.WriteLine(s);
                }
            }
        }

        [Fact]
        public void MkDir()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                Boolean ret = client.mkDir(backet_name, "tst/t1");
            }
        }

        [Fact]
        public void ParrallelUploadFile()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                String[] FileList = { @"C:/development/test.txt", @"C:\development\NuGet_Library\AWS_S3Writer.1.0.0.nupkg" };

                Parallel.ForEach(FileList, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (st) =>
                {
                    Boolean ret = client.UploadFile(st, backet_name,
                            "tst/t121");
                });

            }
        }


        [Fact]
        public void UploadFile()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                Boolean ret = client.UploadFile(@"C:/development/test.txt", backet_name,
                    "tst/t12");

                Boolean ret2 = client.UploadFile(@"C:/development/test.txt", backet_name,
                   "tst/t11");
            }
        }

        [Fact]
        public void UploadStream()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                String filePath = @"C:\development\NuGet_Library\AWS_S3Writer.1.0.0.nupkg";
                using (var fileToUploadStream =
                        new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    client.UploadStream(fileToUploadStream, backet_name, "t44/tt.nupkg");
                }
            }
        }

    }
}
