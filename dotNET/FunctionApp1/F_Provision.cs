using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HVClientSample;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp1
{
    public static class F_Provision
    {
        [FunctionName("Provision")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Try to provsion application to HealtVaul...");
            try
            {
                HVClient clientSample = new HVClient();
                clientSample.ProvisionApplication();
                log.Info("Done");
                return req.CreateResponse(HttpStatusCode.OK, "Done");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return req.CreateResponse(HttpStatusCode.BadRequest, "Error during provisioning");
            }
        }
    }
}
