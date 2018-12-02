﻿
using BLL;
using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace webAPI_tasks.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ProjectsController : ApiController
    {
        //get all projects from DB
        [HttpGet]
        [Route("api/Projects/GetAllProjects")]
        public HttpResponseMessage GetAllProjects()
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicProjects.GetAllProjects());
        }

        //get all projects witch belong to team head by team head id:
        [HttpGet]
        [Route("api/Projects/GetAllProjectsByTeamHead/{TeamHeadId}")]
        public HttpResponseMessage GetAllProjectsByTeamHead(int teamHeadId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicProjects.GetAllProjectsByTeamHead(teamHeadId));
        }

        //get all projects witch belong to worker by worker id:
        [HttpGet]
        [Route("api/Projects/GetAllProjectsByWorker/{workerId}")]
        public HttpResponseMessage GetAllProjectsByWorker(int workerId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicProjects.GetAllProjectsByWorker(workerId));
        }

        //get project details by project name
        [HttpGet]
        [Route("api/Projects/GetProjectDetails")]
        public HttpResponseMessage GetProjectDetails(string projectName)
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicProjects.GetProjectDetails(projectName));
        }

        [HttpGet]
        [Route("api/Projects/GetProjectState/{idProject}")]
        public HttpResponseMessage GetProjectState(int idProject)
        {
            return Request.CreateResponse(HttpStatusCode.OK, LogicProjects.GetProjectState(idProject));
        }

        [HttpPost]
        [Route("api/Projects/AddProject")]
        public HttpResponseMessage AddProject([FromBody]Project value)
        {
        
            if (ModelState.IsValid)
            {
                return (LogicProjects.AddProject(value)) ?
                   Request.CreateResponse(HttpStatusCode.Created) :
                   Request.CreateResponse(HttpStatusCode.BadRequest, "Can not add to DB");
            };

            List<string> ErrorList = new List<string>();
            
            foreach (var item in ModelState.Values)
                foreach (var err in item.Errors)
                    ErrorList.Add(err.ErrorMessage);

            return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorList);
        }


        [HttpDelete]
        [Route("api/Projects/RemoveProject")]
        public HttpResponseMessage Delete(string projectName)
        {
            return LogicProjects.RemoveProject(projectName) ?
                   Request.CreateResponse(HttpStatusCode.OK) :
                   Request.CreateResponse(HttpStatusCode.BadRequest, "Can not remove from DB");
        }
    }
}
