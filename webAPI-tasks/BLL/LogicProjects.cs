using DAL;
using BOL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using BOL.Convertors;

namespace BLL
{
    public class LogicProjects
    {
        //timer for deadLine checking:
        private static Timer aTimer;

        /// <summary>
        /// Get all projects
        /// </summary>
        /// <returns>list of projects</returns>
        public static List<Project> GetAllProjects()
        {
            string query = $"SELECT * FROM task.project;";

            Func<MySqlDataReader, List<Project>> func = (reader) =>
            {
                List<Project> projects = new List<Project>();
                while (reader.Read())
                {
                    projects.Add(ConvertorProject.convertToProject(reader));
                }
                return projects;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        /// Get all projects witch their dead-line is tommorow
        /// </summary>
        /// <returns>list of projects that match the condition</returns>
        public static List<Project> GetAllProjectsByDeadLine()
        {
            string query = $"SELECT * FROM task.project where DATEDIFF(task.project.endDate,date(now()))=1;";

            Func<MySqlDataReader, List<Project>> func = (reader) =>
            {
                List<Project> projects = new List<Project>();
                while (reader.Read())
                {
                    projects.Add(ConvertorProject.convertToProject(reader));
                }
                return projects;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        /// Get all projects witch this worker included in
        /// </summary>
        /// <param name="workerId">id of worker</param>
        /// <returns>list of rrojects witch match the condition</returns>
        public static List<Project> GetAllProjectsByWorker(int workerId)
        {
            {
                string query = $"SELECT task.project.* FROM task.project join task.task on task.task.IdProject=task.project.idProject where task.task.idUser={workerId};";

                Func<MySqlDataReader, List<Project>> func = (reader) =>
                {
                    List<Project> projects = new List<Project>();
                    while (reader.Read())
                    {
                        projects.Add(ConvertorProject.convertToProject(reader));
                    }
                    return projects;
                };

                return DBAccess.RunReader(query, func);
            }
        }

        /// <summary>
        /// Get all projects witch belong to team head
        /// </summary>
        /// <param name="TeamHeadId">id of team head</param>
        /// <returns>list of projects witch match the condition</returns>
        public static List<Project> GetAllProjectsByTeamHead(int TeamHeadId)
        {
            {
                string query = $"SELECT * FROM task.project WHERE TeamHeadId={TeamHeadId};";

                Func<MySqlDataReader, List<Project>> func = (reader) =>
                {
                    List<Project> projects = new List<Project>();
                    while (reader.Read())
                    {
                        projects.Add(ConvertorProject.convertToProject(reader));
                    }
                    return projects;
                };

                return DBAccess.RunReader(query, func);
            }
        }

        /// <summary>
        /// Get project instance by project name
        /// </summary>
        /// <param name="projectName">name of project</param>
        /// <returns>project instance witch name was gotten</returns>
        public static Project GetProjectDetails(string projectName)
        {
            string query = $"SELECT * FROM task.project WHERE name='{projectName}'";
            Func<MySqlDataReader, List<Project>> func = (reader) =>
            {
                List<Project> projects = new List<Project>();
                while (reader.Read())
                {
                    projects.Add(ConvertorProject.convertToProject(reader));
                }
                return projects;
            };
            List<Project> proj = DBAccess.RunReader(query, func);
            if (proj != null && proj.Count > 0)
            {

                return proj[0];
            }
            return null;


        }

        /// <summary>
        /// Get project instance by project id
        /// </summary>
        /// <param name="projectName">id of project</param>
        /// <returns>project instance witch id was gotten</returns>
        public static Project GetProjectDetails(int projectId)
        {
            string query = $"SELECT * FROM task.project WHERE projectId={projectId}";
            Func<MySqlDataReader, List<Project>> func = (reader) =>
            {
                List<Project> projects = new List<Project>();
                while (reader.Read())
                {
                    projects.Add(ConvertorProject.convertToProject(reader));
                }
                return projects;
            };

            return (DBAccess.RunReader(query, func).Count() != 0 ? DBAccess.RunReader(query, func)[0] : null);

        }

        /// <summary>
        ///  Get precents of done part of current project
        /// </summary>
        /// <param name="projectId">id of project to search</param>
        /// <returns>num of precents</returns>
        public static decimal GetProjectState(int projectId)
        {
            string query = $"select sum(task.task.givenHours)*(task.project.QAHours+task.project.devHours+task.project.UIUXHours)/100 from task.task join task.project on task.project.idProject = task.task.idProject where task.task.idProject = {projectId}";
            return Convert.ToDecimal(DBAccess.RunScalar(query));
        }


        /// <summary>
        /// Delete project from DB
        /// </summary>
        /// <param name="projectName">project name for deleting</param>
        /// <returns>true if succeeded, false if failed</returns>
        public static bool RemoveProject(string projectName)
        {
            string query = $"DELETE FROM task.hourfordepartment WHERE Name={projectName}";
            return DBAccess.RunNonQuery(query) == 1;
        }

        /// <summary>
        /// Add project to DB
        /// </summary>
        /// <param name="project">project instance to be added</param>
        /// <returns>true if succeeded, false if failed</returns>
        public static bool AddProject(Project project)
        {
            string dateBegin = project.DateBegin.ToString("yyy-MM-dd");
            string dateEnd = project.DateEnd.ToString("yyy-MM-dd");
            string query = $"INSERT INTO `task`.`project`(`name`,`startdate`,`Enddate`,`isFinished`,`customerName`,`DevHours`,`QAHours`,`UIUXHours`,`teamheadId`) VALUES('{project.ProjectName}','{dateBegin}','{dateEnd}',{project.IsFinish},'{project.CustomerName}',{project.DevHours},{project.QAHours},{project.UIUXHours},{project.IdTeamHead}); ";
            if (DBAccess.RunNonQuery(query) == 1)
            {
                Project currentProject = GetProjectDetails(project.ProjectName);
                foreach (BOL.Models.Task task in project.tasks)
                {
                    query = $"INSERT INTO `task`.`task`(`reservingHours`,`givenHours`,`idProject`,`idUser`)VALUES({task.ReservingHours},0,{currentProject.ProjectId},{task.IdWorker });";
                    DBAccess.RunNonQuery(query);
                }
                return true;
            }
            return false;

        }

        /// <summary>
        /// Check witch projects' end date is tommorow
        /// </summary>
        public static void CheckDeadLine()
        {
            List<Project> deadProjects = new List<Project>();
            deadProjects = GetAllProjectsByDeadLine();
            foreach (Project proj in deadProjects)
            {
                Worker teamHead = new Worker();
                teamHead = LogicWorker.GetWorkerDetails(proj.IdTeamHead);
                List<Worker> workers = new List<Worker>();
                workers = LogicWorker.GetWorkersByTeamhead(proj.IdTeamHead);
                LogicWorker.sendMessage(teamHead, $"Hi {teamHead.WorkerName}, <br/>Project: {proj.ProjectName} is about to reach the deadline tomorrow. This project is under your responsibility, please hurry up!!!", "ATTENTION");
                foreach (Worker worker in workers)
                {
                    LogicWorker.sendMessage(worker, $"Hi {worker.WorkerName}, <br/>Project: {proj.ProjectName} is about to reach the deadline tomorrow. you are subscribed to this project, please hurry up!!!", "ATTENTION");
                }

            }
        }
   
        /// <summary>
        /// raise timer that checks each part of time the dead-Line of projects 
        /// </summary>
        public static void RaiseTimer()
        {
            SetTimer();

            System.Diagnostics.Debug.WriteLine("\nPress the Enter key to exit the application...\n");
            System.Diagnostics.Debug.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            Console.ReadLine();
            aTimer.Stop();
            aTimer.Dispose();

            System.Diagnostics.Debug.WriteLine("Terminating the application...");
        }

        /// <summary>
        /// Set timer that checks the dead-line
        /// </summary>
        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new Timer(1000 * 59);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }


        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ll");

            if (DateTime.Now.ToShortTimeString() == "21:00")
                LogicProjects.CheckDeadLine();


        }
    }
}
