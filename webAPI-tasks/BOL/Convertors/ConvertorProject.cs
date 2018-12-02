using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL.Convertors
{
  public  class ConvertorProject
    {
        public static Project convertToProject(MySqlDataReader readerRow)
        {
            return new Project
            {
                ProjectId = readerRow.GetInt32(0),
                ProjectName = readerRow.GetString(1),
                CustomerName = readerRow.GetString(2),
                QAHours = readerRow.GetInt32(3),
                UIUXHours = readerRow.GetInt32(4),
                DevHours = readerRow.GetInt32(5),
                DateBegin = readerRow.GetMySqlDateTime(6).GetDateTime(),
                DateEnd = readerRow.GetMySqlDateTime(7).GetDateTime(),
                IdTeamHead = readerRow.GetInt32(8),
                IsFinish = readerRow.GetBoolean(9)
            };
        }
    }
}
