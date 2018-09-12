using System.Collections.Generic;
using System.Runtime.Serialization;

namespace  AWS_S3Writer.ClientS3
{
    [DataContract(Namespace = "")]
    public class ClientS3ConfigData
    {
        [DataMember]
        public string AmazonS3AccessKey { get; set; }

        [DataMember]
        public string AmazonS3SecretKey { get; set; }

        [DataMember]

        public string AmazonS3BucketName { get; set; }

        [DataMember]

        public string AmazonS3ServiceURL { get; set; }

    }
}
