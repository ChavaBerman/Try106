using BOL;
using BOL.Convertors;
using BOL.HelpModel;
using BOL.Models;
using DAL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LogicTask
    {
        /// <summary>
        /// Add task to DB
        /// </summary>
        /// <param name="task">task instance to be added</param>
        /// <returns>true if succeeded, false if failed</returns>
        public static bool AddTask(BOL.Models.Task task)
        {
            string query = $"INSERT INTO `task`.`task`(`reservingHours`,`givenHours`,`idProject`,`idUser`)VALUES({task.ReservingHours},{task.GivenHours },{task.IdProject},{task.IdWorker});";
            return DBAccess.RunNonQuery(query) == 1;
        }

        /// <summary>
        /// Check if task already exists (with the same worker and project)
        /// </summary>
        /// <param name="task">task instance for checking</param>
        /// <returns>true if exists, false if not</returns>
        public static bool CheckIfExists(BOL.Models.Task task)
        {
            string query = $"SELECT * FROM task.task WHERE idProject={task.IdProject} and idUser={task.IdWorker};";
            List<BOL.Models.Task> projects = new List<BOL.Models.Task>();
            Func<MySqlDataReader, List<BOL.Models.Task>> func = (reader) =>
            {
                List<BOL.Models.Task> projectsWorker = new List<BOL.Models.Task>();
                while (reader.Read())
                {
                    projectsWorker.Add(ConvertorTask.convertToTask(reader));
                }
                return projectsWorker;
            };

            List<BOL.Models.Task> ProjectWorker = DBAccess.RunReader(query, func);
            if (ProjectWorker != null && ProjectWorker.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// Get all tasks by worker id
        /// </summary>
        /// <param name="workerId">id of worker for searching</param>
        /// <returns>list of tasks witch belong to worker</returns>
        public static List<BOL.Models.Task> GetTasksWithWorkerAndProjectByWorkerId(int workerId)
        {
            string query = $"SELECT task.task.*,task.project.name,task.user.userName FROM task.task join task.user on task.user.idUser=task.task.idUser join task.project on task.project.idProject=task.task.idProject where task.task.idUser={workerId};";
            Func<MySqlDataReader, List<BOL.Models.Task>> func = (reader) =>
            {
                List<BOL.Models.Task> tasks = new List<BOL.Models.Task>();
                while (reader.Read())
                {
                    tasks.Add(ConvertorTask.convertToProjectWithProjectAndWorker(reader));
                }
                return tasks;
            };

            List<BOL.Models.Task> tasksList = DBAccess.RunReader(query, func);

            return tasksList;

        }

        /// <summary>
        /// Get all tasks by worker and project
        /// </summary>
        /// <param name="workerId">id of worker</param>
        /// <param name="projectId">id of project</param>
        /// <returns>list of tasks witch belong to project and worker</returns>
        public static BOL.Models.Task GetTaskByIdProjectAndIdWorker(int workerId, int projectId)
        {
            string query = $"SELECT * FROM task.task  where task.task.idUser={workerId} and task.task.idProject={projectId};";
            Func<MySqlDataReader, List<BOL.Models.Task>> func = (reader) =>
            {
                List<BOL.Models.Task> tasks = new List<BOL.Models.Task>();
                while (reader.Read())
                {
                    tasks.Add(ConvertorTask.convertToTask(reader));
                }
                return tasks;
            };

            List<BOL.Models.Task> tasksList = DBAccess.RunReader(query, func);

            return tasksList[0];

        }

        /// <summary>
        /// Get all tasks by project id
        /// </summary>
        /// <param name="workerId">id of project for searching</param>
        /// <returns>list of tasks witch belong to project</returns>
        public static List<BOL.Models.Task> GetTasksWithWorkerAndProjectByProjectId(int idProject)
        {
            string query = $"SELECT task.task.*,task.project.name,task.user.userName FROM task.task join task.user on task.user.idUser=task.task.idUser join task.project on task.project.idProject=task.task.idProject where task.task.idProject={idProject};";
            Func<MySqlDataReader, List<BOL.Models.Task>> func = (reader) =>
            {
                List<BOL.Models.Task> tasks = new List<BOL.Models.Task>();
                while (reader.Read())
                {
                    tasks.Add(ConvertorTask.convertToProjectWithProjectAndWorker(reader));
                }
                return tasks;
            };

            List<BOL.Models.Task> tasksList = DBAccess.RunReader(query, func);

            return tasksList;

        }

        /// <summary>
        /// Get the information from DB in order to view hours  charts
        /// </summary>
        /// <param name="projectId">the project to view the info for</param>
        /// <returns>dictionary KEY:string-workerName VALUE:hours-reserving and given hours</returns>
        public static Dictionary<string,Hours> GetWorkersDictionary(int projectId)
        {
            List<BOL.Models.Task> tasks = new List<BOL.Models.Task>();
            Dictionary<string, Hours> dictionary = new Dictionary<string, Hours>();
            tasks = GetTasksWithWorkerAndProjectByProjectId(projectId);
            foreach (BOL.Models.Task item in tasks)
            {
                dictionary.Add(item.workerName, new Hours { GivenHours = item.GivenHours, ReservingHours = item.ReservingHours });
            }
            return dictionary;
        }

        /// <summary>
        /// Get the information from DB in order to view hours  charts
        /// </summary>
        /// <param name="workerId">the worker to get information for </param>
        /// <returns>dictionary of KEY: string-workerName, VALUE:hours-reserving and given hours</returns>
        public static Dictionary<string, Hours> GetWorkerTasksDictionary(int workerId)
        {
            List<BOL.Models.Task> tasks = new List<BOL.Models.Task>();
            Dictionary<string, Hours> dictionary = new Dictionary<string, Hours>();
            tasks = GetTasksWithWorkerAndProjectByWorkerId(workerId);
            foreach (BOL.Models.Task item in tasks)
            {
                dictionary.Add(item.projectName, new Hours { GivenHours = item.GivenHours, ReservingHours = item.ReservingHours });
            }
            return dictionary;
        }

        /// <summary>
        /// Enable option of change amount of given or reserving hours for task
        /// </summary>
        /// <param name="task">the task to be changed</param>
        /// <returns>true if succeded, false if failed</returns>
        public static bool UpdateTask(BOL.Models.Task task)
        {
            string query = $"UPDATE `task`.`task` SET `reservingHours` = {task.ReservingHours},`givenHours` = {task.GivenHours} WHERE `idTask` = {task.IdTask};";
            return DBAccess.RunNonQuery(query) == 1;
        }


    }
}
