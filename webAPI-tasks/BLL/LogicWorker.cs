
using DAL;
using BOL;
using BOL.Convertors;
using BOL.HelpModel;
using BOL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Security.Cryptography;

namespace BLL
{
    public class LogicWorker
    {
        /// <summary>
        /// Get all users which defined as workers such as :'developers','QA' etc.
        /// </summary>
        /// <returns>list of workers</returns>
        public static List<Worker> GetAllWorkers()
        {
            string query = $"SELECT * FROM task.user JOIN task.status ON task.user.IdStatus=task.status.IdStatus;";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return workers;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        /// Get all users which defined as workers such as :'developers','QA' etc. with their passwords
        /// </summary>
        /// <returns>list of workers</returns>
        public static List<Worker> GetAllWorkersWithPassword()
        {
            string query = $"SELECT * FROM task.user JOIN task.status ON task.user.IdStatus=task.status.IdStatus;";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorkerWithPassword(reader));
                }
                return workers;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        /// Get a worker by his teamHead
        /// </summary>
        /// <param name="id">teamHead id to search his workers</param>
        /// <returns>list of this teamHead's workers</returns>
        public static List<Worker> GetWorkersByTeamhead(int teamHeadId)
        {

            string query = $"SELECT * FROM task.user JOIN task.status ON task.user.IdStatus=task.status.IdStatus WHERE task.user.managerId={teamHeadId};";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return workers;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        /// Get workers by project id
        /// </summary>
        /// <param name="projectId">id of project</param>
        /// <returns>list of workers who works in this project</returns>
        public static List<Worker> GetWorkersByProjectId(int projectId)
        {
            string query = $"SELECT * FROM task.user JOIN task.TASK on task.user.idUser=task.task.idUser WHERE task.task.idProject={projectId};";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return workers;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        /// Get all workers which are not registered under this teamHead
        /// </summary>
        /// <param name="teamHeadId">the teamHead to get the worker for</param>
        /// <returns>list of workers which match the condition</returns>
        public static List<Worker> GetAllowedWorkers(int teamHeadId)
        {
            string query = $"SELECT * FROM task.user JOIN task.status ON task.user.IdStatus=task.status.IdStatus WHERE task.status.statusname!='TeamHead' and task.status.statusname!='Manager' and task.user.managerId!={teamHeadId};";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return workers;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        /// Get all users which defined as 'TeamHeaders'
        /// </summary>
        /// <returns>list of teamHeads</returns>
        public static List<Worker> GetAllTeamHeads()
        {
            string query = $"SELECT * FROM task.user JOIN task.status ON task.user.IdStatus=task.status.IdStatus WHERE task.status.statusname='TeamHead';";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return workers;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        /// Get workers only ('DEV','QA','UIUX')
        /// </summary>
        /// <returns>LIST OF MATCHING WORKERS</returns>
        public static List<Worker> GetWorkers()
        {
            string query = $"SELECT * FROM task.user JOIN task.status ON task.user.IdStatus=task.status.IdStatus WHERE task.status.statusname!='TeamHead' and task.status.statusname!='Manager';";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return workers;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        ///Get worker's details by id
        /// </summary>
        /// <param name="workerId">id of worker</param>
        /// <returns>worker instance of mathing worker</returns>
        public static Worker GetWorkerDetails(int workerId)
        {
            string query = $"SELECT * FROM task.user JOIN task.status ON user.IdStatus=status.IdStatus WHERE idUser={workerId}";
            List<Worker> workers = new List<Worker>();
            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> projectsWorker = new List<Worker>();
                while (reader.Read())
                {
                    projectsWorker.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return projectsWorker;
            };
            List<Worker> workersList = DBAccess.RunReader(query, func);
            return workersList.Count > 0 ? workersList[0] as Worker : null;
        }

        /// <summary>
        /// Get worker's details by name and email address
        /// </summary>
        /// <param name="workerName"> worker name</param>
        /// <param name="emailAddress">email address</param>
        /// <returns>worker instance of matching worker</returns>
        public static Worker GetWorkerDetailsByNameAndEmail(string workerName, string emailAddress)
        {
            string query = $"SELECT * FROM task.user JOIN task.status ON user.IdStatus=status.IdStatus WHERE userName='{workerName}' and email='{emailAddress}'";
            List<Worker> workers = new List<Worker>();
            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> projectsWorker = new List<Worker>();
                while (reader.Read())
                {
                    projectsWorker.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return projectsWorker;
            };
            List<Worker> workersList = DBAccess.RunReader(query, func);
            return workersList.Count > 0 ? workersList[0] as Worker : null;
        }

        /// <summary>
        /// Get the manager of app
        /// </summary>
        /// <returns>worker instance of manager</returns>
        public static Worker GetManager()
        {
            string query = "SELECT * FROM task.user JOIN task.status ON task.user.IdStatus=task.status.IdStatus WHERE task.status.statusname='Manager';";
            List<Worker> managers = new List<Worker>();
            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> manager = new List<Worker>();
                while (reader.Read())
                {
                    manager.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return manager;
            };
            managers = DBAccess.RunReader(query, func);
            return managers.Count > 0 ? managers[0] as Worker : null;
        }


        public static string ForgotPassword(string workerName, string emailAddress)
        {
            Worker worker = GetWorkerDetailsByNameAndEmail(workerName, emailAddress);
            if (worker != null)
            {
                LogicForgetPassword.AddForgetPasswordRow(worker.WorkerId);
                int requestId = LogicForgetPassword.GetRequestId(worker.WorkerId);
                string message = $"Enter to this link: http://localhost:4200/taskManagement/change-password/{requestId}";
                string subject = "password from managemet task:";
                if (sendMessage(worker, message, subject))
                    return null;
                return "did not succeed to send you email.";
            }
            return "These details do not match any information.";
        }

        /// <summary>
        /// Get worker's details by name and password
        /// </summary>
        /// <param name="password">password</param>
        /// <param name="workerName">name</param>
        /// <returns>worker instance</returns>
        public static Worker GetWorkerDetailsByPassword(string password, string workerName)
        {
            string query = $"SELECT * FROM task.user JOIN task.status ON user.IdStatus=status.IdStatus  WHERE password='{password}' and username='{workerName}'";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return workers;
            };

            List<Worker> workersLogin = DBAccess.RunReader(query, func);
            if (workersLogin != null && workersLogin.Count > 0)
            {

                return workersLogin[0];
            }
            return null;

        }

        /// <summary>
        /// Delete worker
        /// </summary>
        /// <param name="workerId">id of worker</param>
        /// <returns>true if succeeded, false if failed</returns>
        public static bool RemoveWorker(int workerId)
        {
            string query = $"DELETE FROM task.user WHERE idUser={workerId};";
            return DBAccess.RunNonQuery(query) == 1;
        }

        /// <summary>
        /// Update worker details
        /// </summary>
        /// <param name="worker">worker with details for updating</param>
        /// <returns>true if succeeded, false if failed</returns>
        public static bool UpdateWorker(Worker worker)
        {
            string query = $"UPDATE task.user SET IdStatus={worker.StatusId} ,managerId={worker.ManagerId},userComputer='{worker.WorkerComputer}'  WHERE idUser={worker.WorkerId} ;";
            return DBAccess.RunNonQuery(query) == 1;
        }

        /// <summary>
        /// Update password for worker
        /// </summary>
        /// <param name="worker">worker instance for updating</param>
        /// <returns>true if succeeded, false if failed</returns>
        public static bool UpdatePassword(Worker worker)
        {
            string query = $"UPDATE task.user SET password='{worker.Password}'  WHERE idUser={worker.WorkerId} ;";
            return DBAccess.RunNonQuery(query) == 1;
        }

        /// <summary>
        /// Add worker to DB
        /// </summary>
        /// <param name="worker">worker instance for adding</param>
        /// <returns>true if succeeded, false if failed</returns>
        public static bool AddWorker(Worker worker)
        {

            string query = $"INSERT INTO `task`.`user`(`userName`,`password`,`email`,`IdStatus`,`totalhours`,`managerId`,`userComputer`) VALUES('{worker.WorkerName}','{worker.Password}','{worker.Email}',{worker.StatusId},{worker.NumHoursWork},{worker.ManagerId},'{worker.WorkerComputer}'); ";
            return DBAccess.RunNonQuery(query) == 1;
        }

        /// <summary>
        /// Get worker details by computer ip
        /// </summary>
        /// <param name="computerWorker">computer ip</param>
        /// <returns>worker instance</returns>
        public static Worker GetWorkerDetailsComputerWorker(string computerWorker)
        {
            string query = $"USE task;SELECT * FROM task.user JOIN task.status ON user.IdStatus=status.IdStatus WHERE userComputer='{computerWorker}'";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorker(reader));

                }
                return workers;
            };

            List<Worker> workersLogin = DBAccess.RunReader(query, func);
            if (workersLogin != null && workersLogin.Count > 0)
            {

                return workersLogin[0];
            }
            return null;
        }

        /// <summary>
        /// send email to manager
        /// </summary>
        /// <param name="idWorker">id of worker who send the email</param>
        /// <param name="message">massage of email</param>
        /// <param name="subject">subject of email</param>
        /// <returns>true if succeeded, false if not</returns>
        public static bool sendEmailToManager(int idWorker, string message, string subject)
        {
            Worker manager = GetManager();
            Worker worker = GetWorkerDetails(idWorker);
            if (manager == null)
                return false;
            message += $"<br/><span style = 'color:red'> sincerly,{ worker.WorkerName}<span/>";
            return sendMessage(manager, message, subject);

        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="worker">worker who send the email</param>
        /// <param name="message">message</param>
        /// <param name="subject">subject</param>
        /// <returns>true if succeeded, false if not</returns>
        public static bool sendMessage(Worker worker, string message, string subject)
        {

            if (worker == null)
                return false;
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("task.FrishmanBerman@gmail.com", "task1234");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;
            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("task.FrishmanBerman@gmail.com");
            msg.To.Add(new MailAddress(worker.Email));
            msg.Subject = subject;
            msg.IsBodyHtml = true;
            msg.Body = string.Format($"<html><body><p>{message}</br></p></body>");
            try
            {
                client.Send(msg);
                return true;
            }
            catch
            {
                return false;

            }
        }
    }
}


