using DAL;
using BOL;
using BOL.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;


namespace BLL
{
   public class LogicPresentDay
    {
        /// <summary>
        /// Get present days list
        /// </summary>
        /// <returns>List of all present days</returns>
        public static List<PresentDay> GetAllPresents()
        {
            string query = $"SELECT * FROM task.PresentDayUser;";
            //TODO:לגמור לעדכן
            Func<MySqlDataReader, List<PresentDay>> func = (reader) =>
            {
                List<PresentDay> presentDays = new List<PresentDay>();
                while (reader.Read())
                {
                    presentDays.Add(new PresentDay
                    {
                        ProjectId=reader.GetInt32(0),
                        WorkerId = reader.GetInt32(1),
                        TimeBegin=reader.GetDateTime(2),
                        TimeEnd=reader.GetDateTime(3),
                        sumHoursDay=reader.GetInt32(4)
                    });
                }
                return presentDays;
            };

            return DBAccess.RunReader(query, func);
        }

        /// <summary>
        /// get present day by following params:
        /// </summary>
        /// <param name="idWorker">id of worker</param>
        /// <param name="idProject"> id of project</param>
        /// <param name="dateBegin">begin date</param>
        /// <returns>PresentDay that was found</returns>
        public static PresentDay GetPresent(int idWorker,int idProject,DateTime dateBegin)
        {
            string query = $"SELECT * FROM task.PresentDay WHERE idproject={idProject} and iduser={idWorker} and startHour='{dateBegin.ToString("yyyy-MM-dd HH:mm:ss")}'";
            Func<MySqlDataReader, List<PresentDay>> func = (reader) =>
            {
                List<PresentDay> presentDays = new List<PresentDay>();
                while (reader.Read())
                {
                    presentDays.Add(new PresentDay
                    {
                        ProjectId = reader.GetInt32(0),
                        WorkerId = reader.GetInt32(1),
                        TimeBegin = reader.GetMySqlDateTime(2).GetDateTime(),
                        TimeEnd = reader.GetMySqlDateTime(3).GetDateTime(),
                        sumHoursDay = reader.GetInt32(4),
                        IdPresentDay=reader.GetInt32(5)
                    });
                }
                return presentDays;
            };

            return DBAccess.RunReader(query, func)[0];
        }

        /// <summary>
        /// Get present day by id
        /// </summary>
        /// <param name="id">id of present day</param>
        /// <returns></returns>
        public static PresentDay GetPresentById(int id)
        {
            string query = $"SELECT * FROM task.PresentDay WHERE  IdpresentDay={id}";



            Func<MySqlDataReader, List<PresentDay>> func = (reader) =>
            {
                List<PresentDay> presentDays = new List<PresentDay>();
                while (reader.Read())
                {
                    presentDays.Add(new PresentDay
                    {
                        ProjectId = reader.GetInt32(0),
                        WorkerId = reader.GetInt32(1),
                        TimeBegin = reader.GetDateTime(2),
                        TimeEnd = reader.GetDateTime(3),
                        sumHoursDay = reader.GetInt32(4),
                        IdPresentDay=reader.GetInt32(5)
                    });
                }
                return presentDays;
            };

            return DBAccess.RunReader(query, func)[0];
        }

        /// <summary>
        /// Delete present day instance from DB
        /// </summary>
        /// <param name="id">id of instance to be deleted</param>
        /// <returns></returns>
        public static bool RemovePresent(int id)
        {
            string query = $"DELETE FROM task.PresentDay WHERE presentid={id}";
            return DBAccess.RunNonQuery(query) == 1;
        }

        /// <summary>
        /// Update present day's details
        /// </summary>
        /// <param name="presentDay">presentDay instance with details for updating</param>
        /// <returns>true if succeeded, false if failed</returns>
        public static bool UpdatePresent(PresentDay presentDay)
        {
            PresentDay currentPresentDay = GetPresentById(presentDay.IdPresentDay);
            TimeSpan t = DateTime.Parse(presentDay.TimeEnd.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")) - DateTime.Parse(currentPresentDay.TimeBegin.ToString("yyyy-MM-dd HH:mm:ss"));
            
            double addedHours = t.TotalHours;
               
            string query = $"UPDATE task.PresentDay SET EndHour='{presentDay.TimeEnd.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}',TotalHours={addedHours} WHERE IdPresentDay={presentDay.IdPresentDay}";
            if (DBAccess.RunNonQuery(query) == 1)
            {
                BOL.Models.Task currentTask = LogicTask.GetTaskByIdProjectAndIdWorker(currentPresentDay.WorkerId, currentPresentDay.ProjectId);
                currentTask.GivenHours +=(decimal)addedHours;
                if (LogicTask.UpdateTask(currentTask))
                {
                   Worker CurrentWorker =LogicWorker.GetWorkerDetails(currentPresentDay.WorkerId);
                    CurrentWorker.NumHoursWork +=(decimal) addedHours;
                  if(  LogicWorker.UpdateWorker(CurrentWorker))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add present day instance to DB
        /// </summary>
        /// <param name="presentDay">presentDay instance</param>
        /// <returns>id of added row if succeeded, 0 if failed</returns>
        public static int AddPresent(PresentDay presentDay)
        {
            string query = $"INSERT INTO `task`.`PresentDay`(`IdProject`,`IdUser`,`startHour`,`EndHour`,`Totalhours`) VALUES({presentDay.ProjectId},{presentDay.WorkerId},'{presentDay.TimeBegin.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss tt")}','{presentDay.TimeEnd.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}',{presentDay.sumHoursDay}); ";
            if (DBAccess.RunNonQuery(query) == 1)
            {
                try {
                return  GetPresent(presentDay.WorkerId, presentDay.ProjectId, presentDay.TimeBegin.ToLocalTime()).IdPresentDay;
                }
                catch
                {
                    return 0;
                }
            }
            return 0;


        }
    }
}
