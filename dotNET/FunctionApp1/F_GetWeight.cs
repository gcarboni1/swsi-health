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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("Weight request...calling Health Vault server");
                HVClient clientSample = new HVClient();
                HealthRecordItemCollection items = clientSample.GetWeightFromHealthVault();
                JArray jsonResponse = CreateJsonResponse(items);
                if (jsonResponse.Count > 0)                    
                    return req.CreateResponse(HttpStatusCode.OK, jsonResponse);
                else
                    return req.CreateResponse(HttpStatusCode.NoContent, jsonResponse);
            }
            catch (global::System.Exception)
            {
                JObject jsonResponse = CreateUnauthorizedResponse();
                return req.CreateResponse(HttpStatusCode.Unauthorized, jsonResponse);
            }
        }

        private static JObject CreateUnauthorizedResponse() {
            JObject result = new JObject();
            JObject resultObj = new JObject();
            result["status"] = "Access to HealthVault is denied";
            resultObj["getWeightResponse"] = result;
            return resultObj;
        }


        private static JArray CreateJsonResponse(HealthRecordItemCollection items)
        {
            JObject result = new JObject();
            JObject resultObj = new JObject();
            JArray itemArr = new JArray();
            if (items != null)
            {
                foreach (HealthRecordItem item in items)
                {
                    ItemTypes.Weight weight = (ItemTypes.Weight)item;
                    JObject itemW = new JObject();
                    itemW["time"] = weight.EffectiveDate.ToUniversalTime();
                    itemW["weight"] = weight.Value.Kilograms.ToString();
                    itemW["unit"] = "kilograms";
                    itemArr.Add(itemW);
                }
            }
            result["count"] = items.Count();
            result["result"] = itemArr;
            if (itemArr.Count > 0)
            {
                resultObj["status"] = "ok";
                resultObj["getWeightResponse"] = result;
                return itemArr;
            }
            else
            {
                resultObj["status"] = "no content";
                resultObj["getWeightResponse"] = result;
                return itemArr;
            }            
        }



    }
}
