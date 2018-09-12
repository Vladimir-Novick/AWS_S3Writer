using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace  AWS_S3Writer.ClientS3
{
    [Serializable]
    public class AmazonS3Storage
    {

        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public long Size { get; set; }

        [DataMember]
        public DateTime Last_Modify { get; set; }

    }
}
