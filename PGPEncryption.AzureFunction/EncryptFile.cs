using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PgpCore;

namespace PGPEncryption.AzureFunction
{
    public static class EncryptFile
    {
        [FunctionName("EncryptFile")]
        public static async Task Run(
            [BlobTrigger("files-to-encrypt/{name}.{ext}")] Stream input,
            [Blob("encrypted-files/{name}", FileAccess.Write)] Stream output,
            string name,
            string ext,
            ILogger log)
        {
            string publicKey = Environment.GetEnvironmentVariable("PublicKey", EnvironmentVariableTarget.Process);

            log.LogInformation(publicKey);

            EncryptionKeys encryptionKeys = new EncryptionKeys(publicKey);

            log.LogInformation("keys created!");
            PGP pgp = new PGP(encryptionKeys);

            log.LogInformation("pgp object is created!");

            await pgp.EncryptStreamAsync(input, output);
        }
    }
}