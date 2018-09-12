using System;
using System.Runtime.Serialization;

namespace  AWS_S3Writer.ClientS3
{
    [Serializable]
    public class ClientS3ConfigDataKeys
    {

        [DataMember]
        public string LocalFileName { get; set; }

        [DataMember]
        public string AmazonS3Key { get; set; }

    }
}
