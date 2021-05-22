using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PgpCore;

namespace PGPEncryption.AzureFunction
{
    public static class DecryptFile
    {
        [FunctionName("DecryptFile")]
        public static async Task Run(
            [BlobTrigger("files-to-decrypt/{name}.{ext}")] Stream input,
            [Blob("decrypted-files/{name}", FileAccess.Write)] Stream output,
            string name,
            string ext,
            ILogger log)
        {
            string privateKey = Environment.GetEnvironmentVariable("PrivateKey", EnvironmentVariableTarget.Process);

            EncryptionKeys encryptionKeys = new EncryptionKeys(privateKey, "");
            PGP pgp = new PGP(encryptionKeys);

            await pgp.DecryptStreamAsync(input, output);
        }
    }
}