
using BOL;
using BLL;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BOL.HelpModel;
using System.Web.Http.Cors;

namespace webAPI_tasks.Controllers
{


    //--------------------------------
    [EnableCors("*", "*", "*")]
    public class WorkersController : ApiController
    {

     
        //get asll workers (manager-team heads- workers):
        [HttpGet]
        [Route("api/Workers/GetAllWorkers")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicWorker.GetAllWorkers());
        }

        //get all team heads:
        [HttpGet]
        [Route("api/Workers/GetAllTeamHeads")]
        public HttpResponseMessage GetAllTeamHeads()
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicWorker.GetAllTeamHeads());
        }

        //get workers(DEV-QA-UIUX):
        [HttpGet]
        [Route("api/Workers/GetWorkers")]
        public HttpResponseMessage GetAllWorkers()
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicWorker.GetWorkers());
        }

        //get all workers that are not belong to this team head (gets team head id):
        [HttpGet]
        [Route("api/Workers/GetAllowedWorkers/{teamHeadId}")]
        public HttpResponseMessage GetAllowedWorkers(int teamHeadId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicWorker.GetAllowedWorkers(teamHeadId));
        }

        //get all workers that are belong to this team head (gets team head id):
        [HttpGet]
        [Route("api/Workers/GetWorkersByTeamhead/{teamHeadId}")]
        public HttpResponseMessage GetWorkersByTeamhead(int teamHeadId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicWorker.GetWorkersByTeamhead(teamHeadId));
        }

        //get worker by id:
        [HttpGet]
        [Route("api/Workers/GetWorkerDetails/{id}")]
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicWorker.GetWorkerDetails(id));
        }

        //send message to manager (gets EmailParams object that contains worker id, subject and message):
        [HttpPost]                                                       
        [Route("api/Workers/SendMessageToManager")]                        
        public HttpResponseMessage sendMessageToManager([FromBody]EmailParams emailParams)
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicWorker.sendEmailToManager(emailParams.idWorker, emailParams.message, emailParams.subject));
        }

        //add worker to DB (gets worker object):
        [HttpPost]
        [Route("api/Workers/AddWorker")]
        public HttpResponseMessage Post([FromBody]Worker value)
        {
            if (ModelState.IsValid)
            {
                return (LogicWorker.AddWorker(value)) ?
                    Request.CreateResponse(HttpStatusCode.Created) :
                    Request.CreateResponse(HttpStatusCode.BadRequest, "Can not add to DB");
            };

            List<string> ErrorList = new List<string>();

            //if the code reached this part - the worker is not valid
            foreach (var item in ModelState.Values)
                foreach (var err in item.Errors)
                    ErrorList.Add(err.ErrorMessage);
            return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorList);
        }

        //login to system with worker name and password (gets LoginWorker object that contains name and password):
        [HttpPost]
        [Route("api/Workers/LoginByPassword")]
        public HttpResponseMessage LoginByPassword([FromBody]LoginWorker value)
        {
            if (ModelState.IsValid)
            {
                Worker worker = LogicWorker.GetWorkerDetailsByPassword(value.Password, value.WorkerName);
                //TODO:TOKEN
                return worker != null ?
                    Request.CreateResponse(HttpStatusCode.Created, worker) :
                    Request.CreateResponse(HttpStatusCode.BadRequest, "can not login with those details");

            };

            List<string> ErrorList = new List<string>();

            //if the code reached this part - the worker is not valid
            foreach (var item in ModelState.Values)
                foreach (var err in item.Errors)
                    ErrorList.Add(err.ErrorMessage);

            return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorList);

        }

        //login by computer (gets ComputerLogin object taht contains ComputerIp):
        [HttpPost]
        [Route("api/Workers/LoginByComputerWorker")]
        public HttpResponseMessage LoginByComputerWorker([FromBody]ComputerLogin computerLogin)
        {
            Worker worker = LogicWorker.GetWorkerDetailsComputerWorker(computerLogin.ComputerIp);
            return worker != null ?
                 Request.CreateResponse(HttpStatusCode.Created, worker) :
                    Request.CreateResponse(HttpStatusCode.BadRequest, "Can not add to DB");

        }

        [HttpGet]
        [Route("api/Workers/ForgotPassword/{email}/{workerName}")]
        public HttpResponseMessage ForgotPassword([FromUri]string email,[FromUri] string workerName)
        {
            string result = LogicWorker.ForgotPassword(workerName, email);
            return  result==null?
                 Request.CreateResponse(HttpStatusCode.Created,"Email was sent successfully") :
                    Request.CreateResponse(HttpStatusCode.BadRequest, result);

        }

        //update worker details (gets Worker object for updating);
        [HttpPut]
        [Route("api/Workers/UpdateWorker")]
        public HttpResponseMessage UpdateWorker([FromBody]Worker value)
        {

            if (ModelState.IsValid)
            {
                return LogicWorker.UpdateWorker(value) ?
                     Request.CreateResponse(HttpStatusCode.Created) :
                    Request.CreateResponse(HttpStatusCode.BadRequest, "did not succeed to update");
            };

            List<string> ErrorList = new List<string>();
            
            foreach (var item in ModelState.Values)
                foreach (var err in item.Errors)
                    ErrorList.Add(err.ErrorMessage);

            return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorList);
        }

        //delete worker from DB (gets the workerId):
        [HttpDelete]
        [Route("api/Workers/RemoveWorker/{workerId}")]
        public HttpResponseMessage Delete(int workerId)
        {
            return LogicWorker.RemoveWorker(workerId) ?
                 Request.CreateResponse(HttpStatusCode.OK) :
                    Request.CreateResponse(HttpStatusCode.BadRequest, "Can not remove from DB");
        }

        //set new password to worker
        [HttpPost]
        [Route("api/Workers/SetNewPassword")]
        public HttpResponseMessage SetNewPassword([FromBody] ForgotPassword forgotPassword)
        {
            string result = LogicForgetPassword.ChangePassword(forgotPassword);
            return result == null ?
                Request.CreateResponse(HttpStatusCode.Created, "Saved your new password") :
                   Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }

    }
}
