using Client_WinForm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Client_WinForm.Requests
{
  public  class ReportsRequests
    {
        /// <summary>
        /// Create report for manager, includes all information about projects and workers
        /// </summary>
        /// <returns></returns>
        public static List<ReportData> CreateReport()
        {
            List<ReportData> ReportDataList = new List<ReportData>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"http://localhost:61309/api/Reports/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("GetReportData").Result;
            if (response.IsSuccessStatusCode)
            {
                var reportJson = response.Content.ReadAsStringAsync().Result;
                ReportDataList = JsonConvert.DeserializeObject<List<ReportData>>(reportJson);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            return ReportDataList;
        }
     
    }
}
