using AWS_S3Writer.ClientS3;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Xunit.Abstractions;
////////////////////////////////////////////////////////////////////////////
//	Copyright 2018 : Vladimir Novick    https://www.linkedin.com/in/vladimirnovick/  
//
//    NO WARRANTIES ARE EXTENDED. USE AT YOUR OWN RISK. 
//
// To contact the author with suggestions or comments, use  :vlad.novick@gmail.com
//
////////////////////////////////////////////////////////////////////////////

namespace AWS_S3Writer_UnitTest
{
    public class UnitTest1
    {

        String AmazonS3AccessKey = "xxxxxxxxxxxxxxxxxxxxxxx";
        String AmazonS3SecretKey = "yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy";
        String AmazonS3ServiceURL = "http://zzzzzzzzzzzzzzzzzzzzzzzz";

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }


        private readonly ITestOutputHelper output;

        [Fact]
        public void CreateClient()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {

            }
        }

        [Fact]
        public void GetDirList()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                List<String> list = client.GetDirList("sgcombo.data");
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
                List<String> list = client.GetDirList("sgcombo.data", "ean");
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
                List<String> list = client.GetFileList("sgcombo.data");
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
                List<String> list = client.GetFileList("sgcombo.data", "ean");
                foreach (var s in list)
                {
                    output.WriteLine(s);
                }
            }
        }

        [Fact]
        public void mkDir()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                Boolean ret = client.mkDir("sgcombo.data", "ean2/t1");
            }
        }

        [Fact]
        public void UploadFile()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                Boolean ret = client.UploadFile("C:\\Users\\User\\Desktop\\dotnet-svcutil tools.txt", "sgcombo.data",
                    "ean2/t12");

                Boolean ret2 = client.UploadFile("C:\\Users\\User\\Desktop\\dotnet-svcutil tools.txt", "sgcombo.data",
                   "ean2/t11");
            }
        }

        [Fact]
        public void UploadStream()
        {
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                String filePath = "C:\\Users\\User\\Desktop\\dotnet-svcutil tools.txt";
                using (var fileToUploadStream =
                        new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    client.UploadStream(fileToUploadStream, "sgcombo.data", "t44/tt.txt");
                }
            }
        }
       

            

    }
}
