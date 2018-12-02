using BOL;
using BOL.Convertors;
using BOL.HelpModel;
using DAL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
   public class LogicForgetPassword
    {
        /// <summary>
        /// Add new instance to fogotPassword table
        /// </summary>
        /// <param name="workerId">id worker to be added to DB</param>
        /// <returns>true if succeeded false if failed</returns>
        public static bool AddForgetPasswordRow(int workerId)
        {
            string query = $"INSERT INTO `task`.`forgetpassword`(`idUser`)VALUES({workerId}); ";
            return DBAccess.RunNonQuery(query) == 1;
        }

        /// <summary>
        /// Check if fogotPassword table contains suitable instatce
        /// </summary>
        /// <param name="forgotPassword">ForgotPassword instance</param>
        /// <returns>worker id if found, 0 if not found</returns>
        public static int CheckRequestId(ForgotPassword forgotPassword)
        {
            string query = $"select task.forgetpassword.idUser from task.forgetpassword join task.user on task.forgetpassword.idUser=task.user.idUser where task.forgetpassword.IdForgetPassword = {forgotPassword.RequestId} and task.user.userName = '{forgotPassword.WorkerName}' limit 1";
           object result = DBAccess.RunScalar(query);
            int outRes;
            return result!=null ? int.TryParse(result.ToString(),out outRes)? int.Parse(DBAccess.RunScalar(query).ToString()) : 0:0;
        }

        /// <summary>
        /// Change password to the worker whose workerName is sent in ForgotPassword instance to the sent password 
        /// </summary>
        /// <param name="forgotPassword">ForgotPassword instance</param>
        /// <returns>null if succeeded, matching error if failed</returns>
        public static string ChangePassword(ForgotPassword forgotPassword)
        {
            int workerId = CheckRequestId(forgotPassword);
            if (workerId != 0)
            {
                Worker worker = LogicWorker.GetWorkerDetails(workerId);
                string newPasswordHash = forgotPassword.Password.ToUpper();
                if (!CheckIfPasswordExists(newPasswordHash))
                {
                    worker.Password = newPasswordHash;
                    if (LogicWorker.UpdatePassword(worker))
                    {
                        RemoveForgotPassword(forgotPassword.RequestId);
                        return null;
                    }
                    else return "did not succeed to save your password, try again.";
                }
                else return "Enter another password";
            }
            return "Do not try to change password!";
        }

        /// <summary>
        /// Check if password already exists in DB
        /// </summary>
        /// <param name="password">password for checking</param>
        /// <returns>true if found, false if not</returns>
        public static bool CheckIfPasswordExists(string password)
        {
            string query = $"SELECT * FROM task.user JOIN task.status ON task.user.IdStatus=task.status.IdStatus WHERE password='{password}';";

            Func<MySqlDataReader, List<Worker>> func = (reader) =>
            {
                List<Worker> workers = new List<Worker>();
                while (reader.Read())
                {
                    workers.Add(ConvertorWorker.convertDBtoWorker(reader));
                }
                return workers;
            };

            return DBAccess.RunReader(query, func).Count > 0 ? true : false;
        }

        /// <summary>
        /// Delete forgotPassword row from DB
        /// </summary>
        /// <param name="idForgotPassword">id of row</param>
        /// <returns>true if succeeded, false if failed</returns>
        public static bool RemoveForgotPassword(int idForgotPassword)
        {
            string query = $"DELETE FROM task.forgetPassword WHERE IdForgetPassword={idForgotPassword};";
            return DBAccess.RunNonQuery(query) == 1;
        }

        /// <summary>
        /// Get id of forgotPassword row that belongs to the current worker
        /// </summary>
        /// <param name="workerId">worker id to serch his request id</param>
        /// <returns>request id if found, 0 if not</returns>
        public static int GetRequestId(int workerId)
        {
            string query = $"select task.forgetpassword.IdForgetPassword from task.forgetpassword  where task.forgetpassword.idUser = {workerId}  limit 1";
            int result = 0;
            return int.TryParse(DBAccess.RunScalar(query).ToString(), out result) ? int.Parse(DBAccess.RunScalar(query).ToString()) : 0;
        }
    }
}
