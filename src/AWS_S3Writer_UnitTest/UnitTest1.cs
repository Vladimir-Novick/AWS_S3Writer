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
        readonly string AmazonS3AccessKey = "11111111111111";
        readonly string AmazonS3SecretKey = "2222222222222222222222222222";
        readonly string AmazonS3ServiceURL = "http://s3-eu-west-1.amazonaws.com";
        readonly string backet_name = "4444444444444444444";

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
        public void GetDirectories()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                List<String> list = client.GetDirectories(backet_name);
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
                List<String> list = client.GetDirectories(backet_name, "ean");
                foreach (var s in list)
                {
                    output.WriteLine(s);
                }
            }
        }

        [Fact]
        public void GetFiles()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                List<String> list = client.GetFiles(backet_name);
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
                List<String> list = client.GetFiles(backet_name, "ean");
                foreach (var s in list)
                {
                    output.WriteLine(s);
                }
            }
        }

        [Fact]
        public void DeleteFiles()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                List<String> list = client.GetFiles(backet_name, "gta");
                client.DeleteFiles(backet_name, list);
            }
        }



        [Fact]
        public void CreateDirectory()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                client.CreateDirectory(backet_name, "t44");
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
                    Boolean ret = client.Copy(st, backet_name,
                            "ean1/t121");
                });

            }
        }


        [Fact]
        public void Copy()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                Boolean ret = client.Copy(@"C:/development/test.txt", backet_name,
                    "ean/t12");

                Boolean ret2 = client.Copy(@"C:/development/test.txt", backet_name,
                   "ean/t11");
            }
        }

        [Fact]
        public void CopyStream()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                String filePath = @"C:\development\NuGet_Library\AWS_S3Writer.1.0.0.nupkg";
                using (var fileToUploadStream =
                        new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    client.CopyStream(fileToUploadStream, backet_name, "t44/tt.nupkg");
                }
            }
        }

        [Fact]
        public void WriteAllText()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
           
                    client.WriteAllText(backet_name +  "/writeText.txt","WriteText");

            }
        }

        [Fact]
        public void ReadAllText()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {

               String text = client.ReadAllText(backet_name + "/writeText.txt");
                output.WriteLine("Text >" + text);
            }
        }

        [Fact]
        public void Exists_OK()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {

                var ok = client.Exists(backet_name , "t44/");
                Assert.True(ok);
            }
        }

        [Fact]
        public void Exists_FALSE()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {

                var ok = client.Exists(backet_name, "t444");
                Assert.False(ok);
            }
        }


        [Fact]
        public void DeleteDirectory()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {

                client.DeleteDirectory(backet_name,"t44/");
              
            }
        }


        [Fact]
        public void DeleteDirectory2()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {

                client.DeleteDirectory(backet_name +  "/t44/");

            }
        }

        [Fact]
        public void Exists_OK2()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {

                var ok = client.Exists(backet_name + "/t44/");
                Assert.True(ok);
            }
        }

        [Fact]
        public void Exists_FALSE2()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {

                var ok = client.Exists(backet_name+"/t444");
                Assert.False(ok);
            }
        }

    }
}
