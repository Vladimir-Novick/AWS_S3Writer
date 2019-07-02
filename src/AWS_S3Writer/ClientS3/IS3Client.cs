using System.Collections.Generic;
using System.IO;

namespace AWS_S3Writer.ClientS3
{
    public interface IS3Client
    {
        bool Copy(string SourceFile, string DistanationFolder);
        bool Copy(string SourceFile, string DistanationFolder, string strDirName);
        bool CopyStream(Stream InputStrem, string storeFolderOrS3Key, string fileName);
        void CreateDirectory(string newDirecoryFullName);
        void CreateDirectory(string baseDirectory, string newDirecoryName);
        void DeleteDirectory(string directory_name);
        void DeleteDirectory(string backet_name, string dir_name);
        void DeleteFiles(string backet_name, List<string> list);
        bool Exists(string filePath);
        bool Exists(string bucket, string key);
        List<string> GetDirectories(string storageKey, string filter = null);
        List<string> GetFiles(string storageKey, string filter = null);
        string ReadAllText(string filePath);
        void WriteAllText(string path, string contents);
    }
}