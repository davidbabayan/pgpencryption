# PGP Encryption Project

This project is allowing to encrypt/decrypt any type of data with PGP encryption.
<br/><br/>

## Projects

<br/>

Here you can find the project information and the references:

**Add project structure table**
| Project | Path | Description| Project Type |
|---------|------|------------|--------------|
| Azure Function | [~\\PGPEncryption.AzureFunction\\](https://github.com/davidbabayan/pgpencryption/tree/master/PGPEncryption.AzureFunction) | This project is the main project containing the code of the funciton app. It contains four functions to encrypt/decrypt strings and files. | Azure Function App
| Key Generator | [~\\PGPEncryption.KeyGenerator\\](https://github.com/davidbabayan/pgpencryption/tree/master/PGPEncryption.KeyGenerator) | This project is generating the PGP private and public keys for your use. | Console App |

<br/><br/>

## **Instructions to create the projects**

<br/>

## a) Initial Creation using any commandline tool

<br/>

### 1. Checking the program availability

```powershell
dotnet --version

az --version

func --version

code --version
```

### 2. Creating root folder

```powershell
cd C:\Users\david\source\repos

mkdir PGPEncryption

cd PGPEncryption
```

### 3. Creating Solution file

```powershell
dotnet new sln -n PGPEncryption
```

### 4. Creating projects

```powershell
dotnet new console -n PGPEncryption.KeyGenerator

func init PGPEncryption.AzureFunction --dotnet
```

### 5. Add projects refernece to the Solution

```powershell
dotnet sln add PGPEncryption.KeyGenerator\PGPEncryption.KeyGenerator.csproj

dotnet sln add PGPEncryption.AzureFunction\PGPEncryption_AzureFunction.csproj
```

### 6. Install PGPCore package to both projects

```powershell
cd PGPEncryption.KeyGenerator

dotnet add package PgpCore

cd ..\PGPEncryption.AzureFunction

dotnet add package PgpCore

cd ..
```

### 7. Add functions to AzureFunction app

```powershell
cd PGPEncryption.AzureFunction

func new --name EncryptString --template "HTTP trigger" --authlevel "anonymous"

func new --name DecryptString --template "HTTP trigger" --authlevel "anonymous"

func new --name EncryptFile --template "Blob trigger"

func new --name DecryptFile --template "Blob trigger"

cd ..
```

### 8. Open VS Code

```powershell
code .
```

<br/>

## b) Adding Key Generation Functionality

<br/>

### 1. Open Program.cs in KeyGenerator app

### 2. Past this code in Program.cs

```C#
using (PgpCore.PGP pgp = new PgpCore.PGP())
{
    System.IO.Directory.CreateDirectory(@"C:\TEMP\Keys");
    pgp.GenerateKey(
        @"C:\TEMP\Keys\public.asc",
        @"C:\TEMP\Keys\private.asc",
        "email@email.com",
        "password");
}
System.Console.WriteLine("The keys are generated successfully!");
```

<br/>

## c) Generating the Keys

<br/>

### 1. Open any commandline tool

### 2. Run the KeyGenerator app

```powershell
cd PGPEncryption.KeyGenerator
dotnet run
cd ..
```

<br/>

## d) Adding code to AzureFunction project

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

## e) Publish the function

### 1. Login to Azure, create resource group and app

```powershell
az login
```

### 2. Create resource group and Azure Function

```powershell
az group create --name PgpEncryptionResources --location northeurope
```

### 3. Create storage account for using as Azure Function host and files container (NOTE: Add the date of the creation to get unique name!)

```powershell
az storage account create --name pgpencryptionstorage[ddmm] --location northeurope --resource-group PgpEncryptionResources --sku Standard_LRS
```

### 4. Get storage connection string

```powershell
$connectionstring = (az storage account show-connection-string -g PgpEncryptionResources -n pgpencryptionstorage[ddmm] | convertfrom-json).connectionString
```

### 5. Create Azure Key Vault for your keys

```powershell
az keyvault create --name PgpEncryptionKeys --resource-group PgpEncryptionResources --location northeurope
```

### 6. Upload your keys to Key Vault

```powershell
az keyvault secret set --name PublicKey --vault-name PgpEncryptionKeys --file C:\TEMP\Keys\public.asc --encoding ascii

az keyvault secret set --name PrivateKey --vault-name PgpEncryptionKeys --file C:\TEMP\Keys\private.asc --encoding ascii
```

### 7. Create the Azure Function App

```powershell
az functionapp create --resource-group PgpEncryptionResources --consumption-plan-location northeurope --runtime dotnet --functions-version 3 --name PgpEncryptionApp --storage-account pgpencryptionstorage[ddmm]
```

### 8. Enable system managed identity for the Azure Funciton

```powershell
az functionapp identity assign -g PgpEncryptionResources -n PgpEncryptionApp
$funcapp = (az functionapp identity show --name PgpEncryptionApp --resource-group PgpEncryptionResources | convertfrom-json).principalId
```

### 9. Set access policies for the Function app in Key Vault

```powershell
az keyvault set-policy -n PgpEncryptionKeys --secret-permissions get list --object-id $funcapp
```

### 10. Publish the function app

```powershell
func azure functionapp publish PgpEncryptionApp --nozip
```

### 11. Configure the Function App configuration

**There is a issue with app configuration command. Use [Azure Portal}(https://portal.azure.com) to update your function app configuration**

```powershell
az functionapp config appsettings set --name PgpEncryptionApp --resource-group PgpEncryptionResources --settings "AzureWebJobsStorage=$connectionstring"
az --% functionapp config appsettings set --name PgpEncryptionApp --resource-group PgpEncryptionResources --settings "PublicKey=@Microsoft.KeyVault(VaultName=pgpencryptionkeys;SecretName=PublicKey)"
az --% functionapp config appsettings set --name PgpEncryptionApp --resource-group PgpEncryptionResources --settings "PrivateKey=@Microsoft.KeyVault(VaultName=pgpencryptionkeys;SecretName=PrivateKey)"
```
