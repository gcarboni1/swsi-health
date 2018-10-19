using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HVClientSample;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Health;
using Newtonsoft.Json.Linq;

namespace FunctionApp1
{
    public static class F_GetAnagraphic
    {
        [FunctionName("GetAnagraphic")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {            
            try
            {
                log.Info("Anagraphic request...calling Health Vault server");
                HVClient clientSample = new HVClient();
                HealthRecordItemCollection items = clientSample.GetWeightFromHealthVault();

                // Get list of authorized people
                ApplicationConnection connection = clientSample.HealthClientApplication.ApplicationConnection;
                List<PersonInfo> authorizedPeople = new List<PersonInfo>(connection.GetAuthorizedPeople());

                if (authorizedPeople.Count == 0)
                {
                    log.Info("No records were authorized.  Application setup process did not complete.");
                    return req.CreateResponse(HttpStatusCode.BadRequest, "");
                }
                PersonInfo personInfo = authorizedPeople[0];
                JObject jsonResponse=CreateJsonResponse(personInfo);
                    
                return req.CreateResponse(HttpStatusCode.OK, jsonResponse);
            }
            catch (global::System.Exception)
            {
//                JObject jsonResponse = CreateUnauthorizedResponse();
                
                return req.CreateResponse(HttpStatusCode.Unauthorized, "");
            }
        }

        private static JObject CreateJsonResponse(PersonInfo personInfo)
        {
            JObject jsonObject = new JObject();
            jsonObject["id"] = personInfo.PersonId;
            jsonObject["version"] = "0";
            jsonObject["firstNames"] = personInfo.Name;
            jsonObject["lastNames"] = "";
            jsonObject["gender"] = "MALE";
            jsonObject["dateOfBirth"] = "1986-10-07";


            JObject result = new JObject();
            result["meta"] = null;
            result["action"] = null;
            result["party"] = jsonObject;
            return result;
        }

    }
    
}
