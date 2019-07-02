using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Amazon.S3.Transfer;
using System.Threading.Tasks;
using System.Text;

namespace AWS_S3Writer.ClientS3
{
    public class AWS_S3_Client : IDisposable, IS3Client
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




        /// <summary>
        ///    Check if file or folder exist : folder like: base_folder/folder/
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool Exists(string filePath)
        {

            string bucketName, keyName;
            GetBacketKeys(filePath, out bucketName, out keyName);
            return Exists(bucketName, keyName);

        }

        private static void GetBacketKeys(string filePath,  out string bucketName, out string keyName)
        {
            string _filePath = filePath;
            bool isFolderCheck = false;
            if (filePath[filePath.Length - 1] == '/')
            {
                isFolderCheck = true;
                _filePath = _filePath.Substring(0, _filePath.Length - 1);
            }
            int indexFolder = _filePath.LastIndexOf("/");
            bucketName = _filePath.Substring(0, indexFolder);
            keyName = _filePath.Substring(indexFolder + 1);
            if (isFolderCheck)
            {
                keyName += "/";
            }

            return ;
        }


        /// <summary>
        ///    Folder : please like tt/
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string bucket, string key)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest();
                request.BucketName = bucket;
                request.Key = key;
                var response = _client.GetObjectAsync(request).GetAwaiter().GetResult();
                if (response.ResponseStream != null)
                {
                    return true;
                }
                return false;

            }

            catch (Exception ex)
            {
                    return false;
            }
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


        public List<List<T>> Partition<T>(List<T> list, int maxSize)
        {
            if (list == null)
                throw new ArgumentNullException("list");



            List<List<T>> partitions = new List<List<T>>();

            int k = 0;

            bool exit = false;

            while ( true)
            {
                List<T> part  = new List<T>();
                partitions.Add(part);
                for (int j = k; j < k + maxSize; j++)
                {
                    if (j >= list.Count)
                    {
                        exit = true;
                        break;
                    }
                    part.Add(list[j]);
                }
                k += maxSize;
                if (exit) { break; }
            }

            return partitions;
        }


        public void DeleteFiles(string backet_name, List<string> list)
        {
           var listOfLists = Partition<String>(list, 1000);
            foreach (var keys in listOfLists)
            {
                DeletePartFiles(backet_name, keys);
            }
        }

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

        private TransferUtility fileTransferUtility = null;

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
        /// <param name="baseDirectory"></param>
        /// <param name="newDirecoryName"></param>
        /// <returns></returns>
        public void CreateDirectory(String baseDirectory, String newDirecoryName)
        {
            if (String.IsNullOrWhiteSpace(newDirecoryName))
            {
                new ArgumentNullException("strDirName");
            }
            String bucket = $"{baseDirectory}/{newDirecoryName}/";
            CreateDirectory(bucket);

            return ;
        }
        /// <summary>
        ///   Create new direcory : new directory like : $"{baseDirectory}/{newDirecoryName}/
        /// </summary>
        /// <param name="newDirecoryFullName"> like  : $"{baseDirectory}/{newDirecoryName}/</param>
        public void CreateDirectory(String newDirecoryFullName)
        {
            var result = _client.PutBucketAsync(newDirecoryFullName).GetAwaiter().GetResult();

        }

        /// <summary>
        ///    Delete directory 
        /// </summary>
        /// <param name="newDirecoryFullName"></param>
        public void DeleteDirectory(String backet_name, string dir_name)
        {

            List<String> keys = new List<string>();
            keys.Add(dir_name);

            DeletePartFiles(backet_name, keys);

        }

        private void DeletePartFiles(string backet_name, List<string> keys)
        {
            DeleteObjectsRequest deleteRequest = new DeleteObjectsRequest()
            {
                BucketName = backet_name

            };

            foreach (var item in keys)
            {
                KeyVersion key = new KeyVersion()
                {
                    Key = item
                };
                deleteRequest.Objects.Add(key);
            }

            var result = _client.DeleteObjectsAsync(deleteRequest).GetAwaiter().GetResult();
        }

        /// <summary>
        ///    Delete directory 
        /// </summary>
        /// <param name="newDirecoryFullName"></param>
        public void DeleteDirectory(String directory_name)
        {
            GetBacketKeys(directory_name,  out string backet_name, out string keyName);
            DeleteDirectory(backet_name, keyName);
        }



        /// <summary>
        ///   Parallel upload files to storage directory. -   String bucket = $"{storageKey}/{strDirName}/";
        /// </summary>
        /// <param name="listFiles"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="strDirName"></param>
        private static void Copy(List<string> listFiles, String targetDirectory)
        {
            using (AWS_S3_Client client = new AWS_S3_Client())
            {

                Parallel.ForEach(listFiles, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (st) =>
                {
                    client.Copy(st, targetDirectory);
                });

            }
        }

        /// <summary>
        ///    Upload file to S3 repository
        /// </summary>
        /// <param name="SourceFile"></param>
        /// <param name="DistanationFolder"></param>
        /// <param name="strDirName">
        /// 
        /// Like:
        ///   myFolder
        ///   myFolder/mySubFolder
        /// 
        /// </param>
        /// <returns></returns>
        public Boolean Copy(String SourceFile, String DistanationFolder, String strDirName)
        {

            String bucket = $"{DistanationFolder}/{strDirName}";
            Copy(SourceFile, bucket);

            return true;
        }

        public string ReadAllText(string filePath)
        {

            string responseBody = "";

            int indexFolder = filePath.LastIndexOf("/");
            String bucketName = filePath.Substring(0, indexFolder);
            String keyName = filePath.Substring(indexFolder + 1);
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };

          
            using (GetObjectResponse response = _client.GetObjectAsync(request).GetAwaiter().GetResult())
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                responseBody = reader.ReadToEnd(); 
            }

            return responseBody;
        }

        /// <summary>
        ///    Copy to Directory
        /// </summary>
        /// <param name="SourceFile"></param>
        /// <param name="DistanationFolder"></param>
        /// <returns></returns>
        public Boolean Copy(String SourceFile, String DistanationFolder)
        {

            fileTransferUtility.UploadAsync(SourceFile, DistanationFolder).GetAwaiter().GetResult();

            return true;
        }

        /// <summary>
        ///   Upload Stream to Repository
        /// </summary>
        /// <param name="InputStrem"></param>
        /// <param name="storeFolderOrS3Key"></param>
        /// <param name="fileName">
        ///      Like :
        ///      "t44/tt.txt"
        /// </param>
        /// <returns></returns>
        public Boolean CopyStream(Stream InputStrem, String storeFolderOrS3Key, String fileName)
        {

            fileTransferUtility.UploadAsync(InputStrem, storeFolderOrS3Key, fileName).GetAwaiter().GetResult();

            return true;
        }

        /// <summary>
        ///    Write all text to file : path -like : storeFolderOrS3Key/fileName
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        public void WriteAllText(string path, string contents)
        {
            int indexFolder = path.LastIndexOf("/");
            String storeFolderOrS3Key = path.Substring(0, indexFolder) ;
            String fileName = path.Substring(indexFolder + 1);
            var tbytes = Encoding.UTF8.GetBytes(contents);
            var tstream = new MemoryStream(tbytes);
            CopyStream(tstream, storeFolderOrS3Key, fileName);
        }




        /// <summary>
        ///   Get list of files from sub directory
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
        public List<String> GetFiles(String storageKey, String filter = null)
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
        public List<String> GetDirectories(String storageKey, String filter = null)
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
