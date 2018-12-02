using BLL;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace webAPI_tasks.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ReportsController : ApiController
    {
        //get report data of projects with workers:
        [HttpGet]
        [Route("api/Reports/GetReportData")]
        public HttpResponseMessage GetReportData()
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicReports.CreateReport());
        }
    
    }
}
