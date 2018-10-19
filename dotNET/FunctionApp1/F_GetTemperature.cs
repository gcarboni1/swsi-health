using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HVClientSample;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Build.Framework.XamlTypes;
using Microsoft.Health;
using Microsoft.Health.ItemTypes;
using Newtonsoft.Json.Linq;

namespace FunctionApp1
{
    public static class F_GetTemperature
    {
        [FunctionName("GetTemperature")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("Temperature request...calling Health Vault server");
                HVClient clientSample = new HVClient();
                HealthRecordItemCollection items = clientSample.GetTemperatureFromHealthVault();
                JArray jsonResponse = CreateJsonResponse(items);
                if (jsonResponse.Count > 0)
                    return req.CreateResponse(HttpStatusCode.OK, jsonResponse);
                else
                    return req.CreateResponse(HttpStatusCode.OK, jsonResponse);
            }
            catch (global::System.Exception)
            {
                JObject jsonResponse = CreateUnauthorizedResponse();
                return req.CreateResponse(HttpStatusCode.Unauthorized, jsonResponse);
            }
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
                    VitalSigns vitalSign = (VitalSigns)item;
                    JObject itemV = new JObject();
                    itemV["unit"] ="°C";
                    itemV["temperature"] = vitalSign.VitalSignsResults[0].Value;
                    itemV["time"] = vitalSign.EffectiveDate.ToUniversalTime();
                    itemArr.Add(itemV);
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

        private static JObject CreateUnauthorizedResponse()
        {
            JObject result = new JObject();
            JObject resultObj = new JObject();
            result["status"] = "Access to HealthVault is denied";
            resultObj["getWeightResponse"] = result;
            return resultObj;
        }
    }


}
