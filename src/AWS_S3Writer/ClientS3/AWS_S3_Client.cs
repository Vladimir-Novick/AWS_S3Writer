using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Collections.Concurrent;
using Amazon.S3.Transfer;
////////////////////////////////////////////////////////////////////////////
//	Copyright 2018 : Vladimir Novick    https://www.linkedin.com/in/vladimirnovick/  
//
//    NO WARRANTIES ARE EXTENDED. USE AT YOUR OWN RISK. 
//
// To contact the author with suggestions or comments, use  :vlad.novick@gmail.com
//
////////////////////////////////////////////////////////////////////////////
namespace AWS_S3Writer.ClientS3
{
    public class AWS_S3_Client : IDisposable
    {
        public void Dispose()
        {
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
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
        }

        private AmazonS3Client _client { get; set; } = null;
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
        public AWS_S3_Client(String AmazonS3ServiceURL,String AmazonS3AccessKey, String AmazonS3SecretKey)
        {
            _client = new AmazonS3Client(
                    AmazonS3AccessKey,
                    AmazonS3SecretKey,
                    new Amazon.S3.AmazonS3Config { ServiceURL = AmazonS3ServiceURL });
            if (_client == null)
            {
                throw new ArgumentException("Invalid AWS S3 access key");
            }
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

            return _client;
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
            var result = _client.PutBucketAsync(bucket).Result;

            return true;
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
        public Boolean  UploadFile(String filePath,String storageKey, String strDirName)
        {
            using (var fileTransferUtility =
                    new TransferUtility(_client))
            {
                String bucket = $"{storageKey}/{strDirName}";
                var f = fileTransferUtility.UploadAsync(filePath, bucket);
                    f.Wait();
                
            }
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
            using (var fileTransferUtility =
                    new TransferUtility(_client))
            {
                var f = fileTransferUtility.UploadAsync(dataStrem, storageKey, keyName);
                f.Wait();

            }
            return true;
        }


        /// <summary>
        ///   RGet list of files 
        /// </summary>
        /// <param name="storageKey"></param>
        /// <param name="filter">
        /// 
        ///    Like:
        ///           "sgcombo"
        ///           "sgcombo/ttt"
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
        ///           "sgcombo"
        ///           "sgcombo/ttt"
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
