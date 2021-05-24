# PGP Encryption Project։ Azure Function

This project is allowing to encrypt/decrypt any type of data with PGP encryption.
<br/><br/>

## Projects

<br/>

This project is the main project containing the code of the funciton app. It contains four functions to encrypt/decrypt strings and files.

<br/><br/>

## **Instructions to create the projects**

<br/>

## a) Adding code to AzureFunction project

<br/>

### 1. Open EncryptString.cs in AzureFunction

### 2. Past this code in EncryptString.cs

```C#
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
```

### 3. Open DecryptString.cs in AzureFunction

### 4. Past this code in DecryptString.cs

```C#
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
```

### 5. Open EncryptFile.cs in AzureFunction

### 6. Past this code in EncryptFile.cs

```C#
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

            EncryptionKeys encryptionKeys = new EncryptionKeys(publicKey);

            EncryptionKeys(privateKey, "");
            PGP pgp = new PGP(encryptionKeys);

            await pgp.EncryptStreamAsync(input, output);
        }
    }
}
```

### 7. Open DecryptFile.cs in AzureFunction

### 8. Past this code in DecryptFile.cs

```C#
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
```
