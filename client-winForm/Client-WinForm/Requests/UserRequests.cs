using Client_WinForm.HelpModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;

namespace Client_WinForm.Requests
{
   public class WorkerRequests
    {
        /// <summary>
/// Calls an API which gets the ip address of this compmuter by json format
/// </summary>
/// <returns>string of ip address</returns>
        public static dynamic GetIp()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"https://api.ipify.org/?format=json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("").Result;
            dynamic result = null;
            if (response.IsSuccessStatusCode)
            {
                var workersJson = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<dynamic>(workersJson);
            }
            return result["ip"];
        }
        /// <summary>
/// Get all users which defined as workers such as :'developers','QA' etc.
/// </summary>
/// <returns>list of workers</returns>
        public static List<Models.Worker> GetWorkers()
        {
            List<Models.Worker> allWorkers = new List<Models.Worker>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"http://localhost:61309/api/Workers/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("GetWorkers").Result;
            if (response.IsSuccessStatusCode)
            {
                var workersJson = response.Content.ReadAsStringAsync().Result;
                allWorkers = JsonConvert.DeserializeObject<List<Models.Worker>>(workersJson);
            }
            return allWorkers;
        }
        /// <summary>
        /// Login worker by entering password
        /// </summary>
        /// <param name="loginWorker"> help model which transfer to the server, includes details that were entered by user.</param>
        /// <returns>the worker which logged in</returns>
        public static Models.Worker LoginByPassword(LoginWorker loginWorker)
        {
            Models.Worker worker = new Models.Worker();

            //Post Request for Login
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://localhost:61309/api/Workers/LoginByPassword");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string workerStr = JsonConvert.SerializeObject(loginWorker, Formatting.None);

                streamWriter.Write(workerStr);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                //Gettting response
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                //Reading response
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), ASCIIEncoding.ASCII))
                {
                    string result = streamReader.ReadToEnd();
                    //If Login succeeded
                    if (httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        dynamic obj = JsonConvert.DeserializeObject(result);
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Worker>(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
                    }

                    else return null;

                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    //Printing the matching error
                    MessageBox.Show(reader.ReadToEnd());
                }
                return null;

            }

        }
        /// <summary>
        /// Login by ip address poweres immedietally when app started,and try to check if this ip already registered in the DB
        /// </summary>
        /// <returns>the worker that logged in</returns>
        public static Models.Worker LoginByComputerWorker()
        {
            
                //Post Request for Login
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://localhost:61309/api/Workers/LoginByComputerWorker");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string ip = GetIp();
                    string computerWorkerStr = JsonConvert.SerializeObject(new ComputerLogin { ComputerIp = ip }, Formatting.None);

                    streamWriter.Write(computerWorkerStr);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
               
                try {
                    //Gettting response
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    //Reading response
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), ASCIIEncoding.ASCII))
                    {
                        string result = streamReader.ReadToEnd();
                        //If Login succeeded
                        if (httpResponse.StatusCode == HttpStatusCode.Created)
                        {
                            dynamic obj = JsonConvert.DeserializeObject(result);
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Worker>(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
                        }

                        else return null;

                    }
                }
                catch (WebException ex)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        //Printing the matching error
                        MessageBox.Show(reader.ReadToEnd());
                    }
                    return null;

                }  
        }
        /// <summary>
        /// Get a worker by his teamHead
        /// </summary>
        /// <param name="id">teamHead id to search his workers</param>
        /// <returns>list of this teamHead's workers</returns>
        public static List<Models.Worker> GetWorkersByTeamhead(int id)
        {
            List<Models.Worker> allWorkers = new List<Models.Worker>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"http://localhost:61309/api/Workers/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"GetWorkersByTeamhead/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var workersJson = response.Content.ReadAsStringAsync().Result;
                allWorkers = JsonConvert.DeserializeObject<List<Models.Worker>>(workersJson);
            }
            return allWorkers;
        }
        /// <summary>
        /// Get the information from DB in order to view hours  charts
        /// </summary>
        /// <param name="projectId">the project to view the info for</param>
        /// <returns>dictionary KEY:string-workerName VALUE:hours-reserving and given hours</returns>
        public static Dictionary<string, Hours> GetWorkersDictionary(int projectId)
        {
            Dictionary<string, Hours> allWorkers = new Dictionary<string, Hours>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"http://localhost:61309/api/Tasks/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"GetWorkersDictionary/{projectId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var workersJson = response.Content.ReadAsStringAsync().Result;
                allWorkers = JsonConvert.DeserializeObject<Dictionary<string, Hours>>(workersJson);
            }
            return allWorkers;
        }
        /// <summary>
        /// Get all users which defined as 'TeamHeaders'
        /// </summary>
        /// <returns>list of teamHeads</returns>
        public static List<Models.Worker> GetAllTeamHeads()
        {
            List<Models.Worker> teamHeads = new List<Models.Worker>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"http://localhost:61309/api/Workers/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("GetAllTeamHeads").Result;
            if (response.IsSuccessStatusCode)
            {
                var workersJson = response.Content.ReadAsStringAsync().Result;
                teamHeads = JsonConvert.DeserializeObject<List<Models.Worker>>(workersJson);
            }
            return teamHeads;
        }
        /// <summary>
        /// Get all workers which are not registered under this teamHead
        /// </summary>
        /// <param name="teamHeadId">the teamHead to get the worker for</param>
        /// <returns>list of workers which match the condition</returns>
        public static List<Models.Worker> GetAllowedWorkers(int teamHeadId)
        {
            List<Models.Worker> allowedWorkers = new List<Models.Worker>();
            //Get request for getting the alloed workers for project
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(@"http://localhost:61309/api/Workers/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"GetAllowedWorkers/{teamHeadId}").Result;
            //If got the information
            if (response.IsSuccessStatusCode)
            {
                var workersJson = response.Content.ReadAsStringAsync().Result;
                allowedWorkers = JsonConvert.DeserializeObject<List<Models.Worker>>(workersJson);
            }
            return allowedWorkers;
        }
        /// <summary>
        /// Update worker
        /// </summary>
        /// <param name="worker">the worker to be updated</param>
        /// <returns>true uf, false if failed</returns>
        public static bool UpdateWorker(Models.Worker worker)
        {
            
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($@"http://localhost:61309/api/Workers/UpdateWorker");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    dynamic currentWorker = worker;
                    string currentWorkerNameString = Newtonsoft.Json.JsonConvert.SerializeObject(currentWorker, Formatting.None);
                    streamWriter.Write(currentWorkerNameString);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            try
            {
                //Gettting response
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                //Reading response
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), ASCIIEncoding.ASCII))
                {
                    string result = streamReader.ReadToEnd();
                    //If Update succeeded
                    if (httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    //Printing the matching error
                    MessageBox.Show(reader.ReadToEnd());
                }
                return false;

            }
        }
        /// <summary>
        /// Send email to manager 
        /// </summary>
        /// <param name="idWorker">the worker that sends the email</param>
        /// <param name="message">the message content</param>
        /// <param name="subject">the subject of the message (can be null)</param>
        /// <returns>true if sending succeeded, false if failed</returns>
        public static bool sendMessageToManager(int idWorker, string message, string subject)
        {
            EmailParams emailParams = new EmailParams { idWorker = idWorker, message = message, subject = subject };
            try
            {
                //Post Request for Sending email to manager
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://localhost:61309/api/Workers/SendMessageToManager");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string emailInfo = JsonConvert.SerializeObject(emailParams, Formatting.None);

                    streamWriter.Write(emailInfo);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                //Gettting response
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                //Reading response
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), ASCIIEncoding.ASCII))
                {
                    string result = streamReader.ReadToEnd();
                    //If Sending succeeded
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    
                    else return false;
                }
            }
            catch 
            {
                return false;

            }


        }
        /// <summary>
        /// Delete worker
        /// </summary>
        /// <param name="id">the worker to be deleted</param>
        /// <returns>true if succeded, false if failed</returns>
        public static bool DeleteWorker(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:61309/api/Workers/");
                HttpResponseMessage response = client.DeleteAsync($"RemoveWorker/{id}").Result;
                //If Deleting Succeeded
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                
                    return false;
            }

        }
        /// <summary>
        /// Add new instance of worker
        /// </summary>
        /// <param name="newWorker">the new worker to be added</param>
        /// <returns>true if succeed, false if failed</returns>
        public static bool AddWorker(Models.Worker newWorker)
        {
            
                //Post Request for Register
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://localhost:61309/api/Workers/AddWorker");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string workerJson = JsonConvert.SerializeObject(newWorker, Formatting.None);

                    streamWriter.Write(workerJson);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                try
                {
                    //Gettting response
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    //Reading response
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), ASCIIEncoding.ASCII))
                    {
                        string result = streamReader.ReadToEnd();
                        //If Register succeeded
                        if (httpResponse.StatusCode == HttpStatusCode.Created)
                        {
                            return true;
                        }

                        

                    }
                }
                catch (WebException ex)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                    //Printing the matching error
                    MessageBox.Show(reader.ReadToEnd());
                    }
                    return false;

                }
                return false;


            





            }
        /// <summary>
        /// Send email to worker with link to get an ability to change his password(after validating hin in server side)
        /// </summary>
        /// <param name="email">email address where to send</param>
        /// <param name="workerName">name for validation needed</param>
        /// <returns>true if succeded, false if failed</returns>
        public static string SendForgotPassword(string email, string workerName)
        {
            
               
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(@"http://localhost:61309/api/Workers/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync($"ForgotPassword/{email}/{workerName}").Result;
                  dynamic result = null;
            if (response.IsSuccessStatusCode)
            {
                var msgJson = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<dynamic>(msgJson);
            }
            
               
                return result;
            
        }
}
    }
