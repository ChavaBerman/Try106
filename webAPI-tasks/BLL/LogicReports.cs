using BOL;
using BOL.HelpModel;
using BOL.Models;
using DAL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    public class LogicReports
    {

        /// <summary>
        /// Get report data
        /// </summary>
        /// <returns>list of reportData</returns>
        public static List<ReportData> CreateReport()
        {
            int id = 1;
            List<ReportData> reportDataList = new List<ReportData>();
            List<Project> projects = new List<Project>();
            projects = LogicProjects.GetAllProjects();
            foreach (Project project in projects)
            {
                int parentId = id++;
                decimal givenHoursOfProject = 0;
                List<Worker> workersOfProject = new List<Worker>();
                workersOfProject = LogicWorker.GetWorkersByProjectId(project.ProjectId);
                foreach (Worker worker in workersOfProject)
                {
                    Task task = new Task();
                    task = LogicTask.GetTaskByIdProjectAndIdWorker(worker.WorkerId, project.ProjectId);
                    ReportData reportData = new ReportData
                    {
                        Id = id++,
                        ParentId = parentId,
                        Name = worker.WorkerName,
                        TeamHeader = LogicWorker.GetWorkerDetails(project.IdTeamHead).WorkerName,
                        ReservingHours = task.ReservingHours,
                        GivenHours = task.GivenHours,
                        DateBegin = null,
                        DateEnd = null
                    };
                    givenHoursOfProject += reportData.GivenHours;
                    reportDataList.Add(reportData);
                }
                reportDataList.Add(new ReportData
                {
                    Id = parentId,
                    ParentId = 0,
                    Name = project.ProjectName,
                    TeamHeader = LogicWorker.GetWorkerDetails(project.IdTeamHead).WorkerName,
                    ReservingHours = project.QAHours + project.UIUXHours + project.DevHours,
                    GivenHours = givenHoursOfProject,
                    Customer = project.CustomerName,
                    DateBegin = project.DateBegin,
                    DateEnd = project.DateEnd,
                    Days = (project.DateEnd - project.DateBegin).Days,
                    WorkedDays = (DateTime.Now > project.DateEnd) ? (project.DateEnd - project.DateBegin).Days : (DateTime.Now - project.DateBegin).Days
                });

            }
            return reportDataList;
        }
        
    }
}
