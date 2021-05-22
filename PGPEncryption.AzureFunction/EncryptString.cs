using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PgpCore;

namespace PGPEncryption.AzureFunction
{
    public static class EncryptString
    {
        [FunctionName("EncryptString")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string publicKey = Environment.GetEnvironmentVariable("PublicKey", EnvironmentVariableTarget.Process);

            EncryptionKeys keys = new EncryptionKeys(publicKey);

            PGP pgp = new PGP(keys);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string content = data?.content;

            string encryptedContent = pgp.EncryptArmoredString(content);

            string responseMessage = string.IsNullOrEmpty(encryptedContent)
                ? "Something went wrong!"
                : encryptedContent;

            return new OkObjectResult(responseMessage);
        }
    }
}