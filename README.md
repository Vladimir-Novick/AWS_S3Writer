# AWS_S3Writer
Upload files to AWS S3 Repository

## Upload file:

             using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                Boolean ret = client.UploadFile(@"C:/development/test.txt", backet_name,
                    "tst/t12");

                Boolean ret2 = client.UploadFile(@"C:/development/test2.txt", backet_name,
                   "tst/t11");
            }
			
## 	Multithreading upload :

            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                String[] FileList = { @"C:/development/test.txt", @"C:\development\NuGet_Library\AWS_S3Writer.1.0.0.nupkg" };

                Parallel.ForEach(FileList, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (st) =>
                {
                    Boolean ret = client.UploadFile(st, backet_name,
                            "tst/t121");
                });

            }
			
##  Upload Stream:
     
            using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                String filePath = @"C:\development\NuGet_Library\AWS_S3Writer.1.0.0.nupkg";
                using (var fileToUploadStream =
                        new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    client.UploadStream(fileToUploadStream, backet_name, "t44/tt.nupkg");
                }
            }
			
## Get File List:

			List<String> list ;
			using (AWS_S3_Client client = new AWS_S3_Client(AmazonS3ServiceURL, AmazonS3AccessKey, AmazonS3SecretKey))
            {
                list = client.GetFileList(backet_name);
            }			
   		

		

Copyright (C) 2018 by Vladimir Novick http://www.linkedin.com/in/vladimirnovick , 

vlad.novick@gmail.com , http://www.sgcombo.com , https://github.com/Vladimir-Novick	

## License

		Copyright (C) 2018 by Vladimir Novick http://www.linkedin.com/in/vladimirnovick , 

		Permission is hereby granted, free of charge, to any person obtaining a copy
		of this software and associated documentation files (the "Software"), to deal
		in the Software without restriction, including without limitation the rights
		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
		copies of the Software, and to permit persons to whom the Software is
		furnished to do so, subject to the following conditions:

		The above copyright notice and this permission notice shall be included in
		all copies or substantial portions of the Software.

		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
		THE SOFTWARE. 
