using Client_WinForm.HelpModel;
using Client_WinForm.Models;
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
    public class TaskRequests
    {
        /// <summary>
        /// Add new instance to task table in DB
        /// </summary>
        /// <param name="newTask">the task to be added</param>
        /// <returns>true if succeeded,false if failed</returns>
        public static bool AddTask(Task newTask)
        {
                //Post Request for Add Task
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://localhost:61309/api/Tasks/AddTask");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string task = JsonConvert.SerializeObject(newTask, Formatting.None);

                    streamWriter.Write(task);
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
                        //If Adding Task succeeded
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
        /// Get all tasks which realted to this project
        /// </summary>
        /// <param name="projectId">the project to search tasks for</param>
        /// <returns>true if succeded, false if failed</returns>
        public static List<Task> GetAllTasksByProjectId(int projectId)
        {
            List<Task> allTasks = new List<Task>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"http://localhost:61309/api/Tasks/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"GetTasksWithWorkerAndProjectByProjectId/{projectId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var tasksJson = response.Content.ReadAsStringAsync().Result;
                allTasks = JsonConvert.DeserializeObject<List<Task>>(tasksJson);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            return allTasks;
        }
        /// <summary>
        /// Get all tasks which realted to this worker
        /// </summary>
        /// <param name="workerId"> the worker to search the tasks for </param>
        /// <returns>list of tasks that matches the condition</returns>
        public static List<Task> GetAllTasksByWorkerId(int workerId)
        {
            List<Task> allTasks = new List<Task>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"http://localhost:61309/api/Tasks/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"GetTasksWithWorkerAndProjectByWorkerId/{workerId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var tasksJson = response.Content.ReadAsStringAsync().Result;
                allTasks = JsonConvert.DeserializeObject<List<Task>>(tasksJson);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            return allTasks;
        }
        /// <summary>
        /// Enable option of change amount of given or reserving hours for task
        /// </summary>
        /// <param name="task">the task to be changed</param>
        /// <returns>true if succeded, false if failed</returns>
        public static bool UpdateTask(Task task)
        {
            
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($@"http://localhost:61309/api/Tasks/UpdateTask");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    dynamic currentTask = task;
                    string currentWorkerNameString = Newtonsoft.Json.JsonConvert.SerializeObject(currentTask, Formatting.None);
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
                    //If Update Task succeeded
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
        /// Get the information from DB in order to view hours  charts
        /// </summary>
        /// <param name="workerId">the worker to get information for </param>
        /// <returns>dictionary of KEY: string-workerName, VALUE:hours-reserving and given hours</returns>
        public static Dictionary<string, Hours> GetWorkerTasksDictionary(int workerId)
        {
            Dictionary<string, Hours> allTasks = new Dictionary<string, Hours>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"http://localhost:61309/api/Tasks/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"GetWorkerTasksDictionary/{workerId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var infoJson = response.Content.ReadAsStringAsync().Result;
                allTasks = JsonConvert.DeserializeObject<Dictionary<string, Hours>>(infoJson);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            return allTasks;
        }
    }
}
