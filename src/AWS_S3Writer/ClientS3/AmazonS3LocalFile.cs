using System;

namespace AWS_S3Writer.ClientS3
{
    public class AmazonS3LocalFile
    {
        public string Key { get; set; }
        public string File_Name { get; set; }
        public DateTime Date_modified { get; set; }
        public long Size { get; set; }
    }
}
