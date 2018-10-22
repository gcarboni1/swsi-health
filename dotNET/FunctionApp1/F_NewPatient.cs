using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace FunctionApp1
{
    public static class F_NewPatient
    {
        [FunctionName("NewPatient")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("New Patient creation request...");
                log.Info("Try to parse request parameter...");               
                JObject jsonObj = createJsonRequest(req.Content.ReadAsFormDataAsync().Result.ToString(), log);
                if (jsonObj == null) {
                    jsonObj = new JObject();
                    jsonObj["error"] = "error parsing request";
                    return req.CreateResponse(HttpStatusCode.BadRequest, jsonObj);
                }
                log.Info("Done...Try to save patient on db...");
                var client = new MongoClient("mongodb://swsi-health:swsi-health2018@ds151169.mlab.com:51169/swsi");
                var db = client.GetDatabase("swsi");
                var collection = db.GetCollection<BsonDocument>("patients");
                BsonDocument doc = BsonDocument.Parse(jsonObj.ToString());
                collection.InsertOne(doc);
                log.Info("Done.");
                return req.CreateResponse(HttpStatusCode.OK, jsonObj);
            }
            catch (Exception ex)
            {
                JObject jsonObj = new JObject();
                jsonObj["error"] = ex.Message;
                return req.CreateResponse(HttpStatusCode.InternalServerError, jsonObj);
            }
        }

        static private JObject createJsonRequest(string requestContent, TraceWriter log) {
            JObject jsonObject = new JObject();
            requestContent= requestContent.Replace("%5b", "_").Replace("%5d", "").Replace("%40","@").Replace("%2b","+");
            string[] stringSeparators = new string[] {"&"};
            string[] result= requestContent.Split(stringSeparators, StringSplitOptions.None);
            foreach (string s in result)
            {
                try
                {
                    string key = s.Split('=')[0];
                    string value = s.Split('=').Count().Equals(1) ? "null" : s.Split('=')[1];
                    jsonObject[key.IndexOf("_").Equals(-1) ? key : key.Substring(key.IndexOf("_")+1)] = value;
                    log.Info(key + " -> " + value);
                }
                catch (Exception ex)
                {
                    log.Error( ex.Message);
                    return null;
                }
            }
            return jsonObject;
        }
    }
}
