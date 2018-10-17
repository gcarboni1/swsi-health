using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using HVClientSample;
using Microsoft.Health;
using ItemTypes = Microsoft.Health.ItemTypes;
using Newtonsoft.Json.Linq;
using System;

namespace FunctionApp1
{
    public static class F_GetWeight
    {
        [FunctionName("GetWeight")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Weight request...calling Health Vault server");
            DataObject dataObj = new DataObject();
            dataObj.Id = "Id";
            dataObj.Name = "Name";
            dataObj.Size = "Size";

            //await outputQueueItem.AddAsync(dataObj);



            HVClient clientSample = new HVClient();
            //clientSample.ProvisionApplication();
            HealthRecordItemCollection items = clientSample.GetWeightFromHealthVault();

            JArray itemArr = new JArray();
            if (items != null)
            {
                foreach (HealthRecordItem item in items)
                {
                    ItemTypes.Weight weight = (ItemTypes.Weight)item;

                    JObject itemW = new JObject();
                    itemW["date"] = weight.When.ToString();
                    itemW["value"] = weight.Value.Kilograms.ToString();
                    itemW["misure"] = "Kilograms";
                    itemArr.Add(itemW);
                }
            }
            JObject result = new JObject();
            if (itemArr.Count > 0)
            {
                result["result"] = itemArr;
                return req.CreateResponse(HttpStatusCode.OK, result);
            }          
                
            else
            {
                result["result"] = null;
                return req.CreateResponse(HttpStatusCode.NoContent, result);
            }
                
                      
            


        }
    }
}
