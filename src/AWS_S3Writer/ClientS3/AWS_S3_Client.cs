using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Amazon.S3.Transfer;
using System.Threading.Tasks;
/*


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

*/
namespace AWS_S3Writer.ClientS3
{
    public class AWS_S3_Client : IDisposable
    {

        public void Dispose()
        {

            if (fileTransferUtility != null)
            {
                fileTransferUtility.Dispose();
                fileTransferUtility = null;
            }

            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
            Dispose(true);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (fileTransferUtility != null)
                {
                    fileTransferUtility.Dispose();
                    fileTransferUtility = null;
                }

                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
        }

        private AmazonS3Client _client { get; set; } = null;
		
	    private TransferUtility fileTransferUtility = null;

		
        /// <summary>
        ///   Create AWS S3 Client by config file 
        /// </summary>
        public AWS_S3_Client()
        {
            _client = GetAmazonS3Client();

        }

        /// <summary>
        ///   Create AWS S3 Client
        /// </summary>
        /// <param name="AmazonS3ServiceURL"></param>
        /// <param name="AmazonS3AccessKey"></param>
        /// <param name="AmazonS3SecretKey"></param>
        public AWS_S3_Client(String AmazonS3ServiceURL, String AmazonS3AccessKey, String AmazonS3SecretKey)
        {
            _client = new AmazonS3Client(
                    AmazonS3AccessKey,
                    AmazonS3SecretKey,
                    new Amazon.S3.AmazonS3Config { ServiceURL = AmazonS3ServiceURL });
            if (_client == null)
            {
                throw new ArgumentException("Invalid AWS S3 access key");
            }
            makeFileTransport(_client);
        }

      
        AmazonS3Client GetAmazonS3Client()
        {
            AmazonS3Client _client = new AmazonS3Client(
                    ClientS3Config.GetConfigData.AmazonS3AccessKey,
                    ClientS3Config.GetConfigData.AmazonS3SecretKey,
                    new Amazon.S3.AmazonS3Config { ServiceURL = ClientS3Config.GetConfigData.AmazonS3ServiceURL }

            );

            if (_client == null)
            {
                throw new InvalidDataException("S3 Configureation is failed");
            }

            makeFileTransport(_client);

            return _client;
        }

        private void makeFileTransport(AmazonS3Client _client)
        {
            if (fileTransferUtility == null)
            {
                TransferUtilityConfig config = new TransferUtilityConfig();
                config.ConcurrentServiceRequests = 20;
                fileTransferUtility = new TransferUtility(_client, config);
            }
        }

        /// <summary>
        ///   Make storade directory
        ///      please use like :
        ///          myFolder
        ///          myFolder/mySubfolder
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="strDirName"></param>
        /// <returns></returns>
        public Boolean mkDir(String storageKey, String strDirName)
        {
            if (String.IsNullOrWhiteSpace(strDirName))
            {
                new ArgumentNullException("strDirName");
            }
            String bucket = $"{storageKey}/{strDirName}/";
            var result = _client.PutBucketAsync(bucket).GetAwaiter().GetResult();

            return true;
        }

        public static Boolean UploadFile(List<String> filesList, String storageKey, String strDirName)
        {

            List<String> listFiles = new List<string>();

            foreach (var itemFile in filesList)
            {
                listFiles.Add(itemFile);
                if (listFiles.Count > 19)
                {
                    UploadFiles(listFiles, storageKey, strDirName);

                    listFiles.Clear();
                }

            }

            UploadFiles(listFiles, storageKey, strDirName);
            return true;
        }
        /// <summary>
        ///   Parallel upload files to storage directory.
        /// </summary>
        /// <param name="listFiles"></param>
        /// <param name="storageKey"></param>
        /// <param name="strDirName"></param>
        private static void UploadFiles(List<string> listFiles, String storageKey, String strDirName)
        {
            using (AWS_S3_Client client = new AWS_S3_Client())
            {

                Parallel.ForEach(listFiles, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (st) =>
                {
                    client.UploadFile(st, storageKey, strDirName);
                });

            }
        }

        /// <summary>
        ///    Upload file to S3 repository
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="storageKey"></param>
        /// <param name="strDirName">
        /// 
        /// Like:
        ///   myFolder
        ///   myFolder/mySubFolder
        /// 
        /// </param>
        /// <returns></returns>
        public Boolean UploadFile(String filePath, String storageKey, String strDirName)
        {

            String bucket = $"{storageKey}/{strDirName}";
            fileTransferUtility.UploadAsync(filePath, bucket).GetAwaiter().GetResult();

            return true;
        }

        /// <summary>
        ///   Upload Stream to Repository
        /// </summary>
        /// <param name="dataStrem"></param>
        /// <param name="storageKey"></param>
        /// <param name="keyName">
        ///      Like :
        ///      "t44/tt.txt"
        /// </param>
        /// <returns></returns>
        public Boolean UploadStream(Stream dataStrem, String storageKey, String keyName)
        {

            fileTransferUtility.UploadAsync(dataStrem, storageKey, keyName).GetAwaiter().GetResult();

            return true;
        }

        /// <summary>
        ///   RGet list of files 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="filter">
        /// 
        ///    Like:
        ///           "ean"
        ///           "ean/ttt"
        ///           or null
        /// 
        /// </param>
        /// <returns></returns>
        public List<String> GetFileList(String storageKey, String filter = null)
        {
            ListObjectsRequest request;

            List<String> ListKeys = new List<string>();

            request = makeRequest(storageKey, filter);

            do
            {
                ListObjectsResponse response = _client.ListObjectsAsync(request).Result;

                IEnumerable<S3Object> files = from tr in response.S3Objects
                                              where tr.Key.EndsWith(@"/") == false
                                              select tr;

                files.ToList().ForEach(x => ListKeys.Add(x.Key));

                if (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;
                }
                else
                {
                    request = null;
                }
            } while (request != null);

            return ListKeys;
        }

        /// <summary>
        ///  get File List of S3 Storage
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="filter">
        ///        Like:
        ///           "ean"
        ///           "ean/ttt"
        ///           or null
        ///    </param>
        /// <returns></returns>
        public List<String> GetDirList(String storageKey, String filter = null)
        {
            ListObjectsRequest request;

            List<String> ListKeys = new List<string>();

            request = makeRequest(storageKey, filter);

            do
            {
                ListObjectsResponse response = _client.ListObjectsAsync(request).Result;

                IEnumerable<S3Object> folders = response.S3Objects.Where(x =>
                    x.Key.EndsWith(@"/") && x.Size == 0);

                folders.ToList().ForEach(x => ListKeys.Add(x.Key));

                if (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;
                }
                else
                {
                    request = null;
                }
            } while (request != null);

            return ListKeys;
        }

        private static ListObjectsRequest makeRequest(string storageKey, string mainFolder)
        {
            ListObjectsRequest request;
            if (mainFolder == null)
            {
                request = new ListObjectsRequest
                {
                    BucketName = storageKey
                };
            }
            else
            {
                request = new ListObjectsRequest
                {
                    BucketName = storageKey,
                    Prefix = mainFolder + "/"
                };
            };
            return request;
        }

    }
}
