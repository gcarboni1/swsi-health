using System.IO;
using HVClientSample;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Health;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public class DataObject
    {
        public string Id { get; set; }
        public string Size { get; set; }
        public string Name { get; set; }        
    }
    

    [StorageAccount("AzureWebJobsStorage")]
    public static class F_BlobTrigger
    {
        [FunctionName("BlobTrigger")]       
        public static async System.Threading.Tasks.Task RunAsync(
            [BlobTrigger("doc-repository/{name}", Connection = "AzureWebJobsStorage")]
            Stream myBlob, string name,TraceWriter log)
        {            
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            HVClient clientSample = new HVClient();
            //clientSample.ProvisionApplication();
            clientSample.SetTemperatureOnHealthVault(110);
                   
            /*DataObject data = new DataObject();
            data.Id = "Id";
            data.Name = "Name";
            data.Size = "Size";
            */
            log.Info("Try to pop element into output-queue");        
            

            return;
        
        }
    }
}
