using Newtonsoft.Json;
using System;
using System.IO;

namespace AWS_S3Writer.ClientS3
{
    public class ClientS3Config
    {
        private static ClientS3ConfigData mConfigS3 = null;

        public static ClientS3ConfigData GetConfigData
        {
            get
            {
                if (mConfigS3 == null)
                {
                    GetConfiguration();
                }
                return mConfigS3;
            }
        }

        private static void GetConfiguration()
        {
            String configFile = "amazons3.json";

            String pathToTheFile = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + configFile;

            using (StreamReader file = File.OpenText(pathToTheFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                mConfigS3 = (ClientS3ConfigData)serializer.Deserialize(file, typeof(ClientS3ConfigData));
            }

        }

    }
}
