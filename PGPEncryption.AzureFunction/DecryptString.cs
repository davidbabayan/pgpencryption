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
    public static class DecryptString
    {
        [FunctionName("DecryptString")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string privateKey = Environment.GetEnvironmentVariable("PrivateKey", EnvironmentVariableTarget.Process);

            EncryptionKeys keys = new EncryptionKeys(privateKey, "");

            PGP pgp = new PGP(keys);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string encryptedContent = data?.content;

            string decodedContent = pgp.DecryptArmoredString(encryptedContent);

            string responseMessage = string.IsNullOrEmpty(decodedContent)
                ? "The encrypted content is invalid!"
                : decodedContent;

            return new OkObjectResult(responseMessage);
        }
    }
}