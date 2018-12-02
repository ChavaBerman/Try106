using Client_WinForm.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace Client_WinForm.Requests
{
    public class PresentDayRequests
    {
        
        static int idPresentDay = 0;
        /// <summary>
        /// Register new instace of presence when worker signs in to his work time.
        /// </summary>
        /// <param name="NewpresentDay"></param>
        /// <returns>true if succeeded false if failed</returns>
        public static bool AddPresent(PresentDay NewpresentDay)
        {
            //Post Request for Register
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://localhost:61309/api/PresentDay/AddPresent");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string presentDay = JsonConvert.SerializeObject(NewpresentDay, Formatting.None);

                streamWriter.Write(presentDay);
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
                    //If AddPresent succeeded
                    if (httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        if (!int.TryParse(result, out idPresentDay))
                            return false;
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

                    //Printing the matchung errors:
                    MessageBox.Show(reader.ReadToEnd());
                }
                return false;
            }
        }
        /// <summary>
        /// Update presence when worker signs out of his work time.
        /// </summary>
        /// <param name="NewpresentDay"></param>
        /// <returns>true if succeeded false if failed</returns>
        public static bool UpdatePresentDay(PresentDay presentDay)
        {

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($@"http://localhost:61309/api/PresentDay/UpdatePresentDay");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                dynamic currentPresentDay = presentDay;
                string currentPresentNameString = Newtonsoft.Json.JsonConvert.SerializeObject(currentPresentDay, Formatting.None);
                streamWriter.Write(currentPresentNameString);
                streamWriter.Flush();
                streamWriter.Close();
            }
            //Get response
            try
            {
                //Gettting response
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //Reading response
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), ASCIIEncoding.ASCII))
                {
                    string result = streamReader.ReadToEnd();
                    //If AddPresent succeeded
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
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

                    //Printing the matchung errors:
                    MessageBox.Show(reader.ReadToEnd());
                }
                return false;
            }

        }
        /// <summary>
        /// if user logout via the button of logout, save his information.
        /// </summary>
        /// <returns></returns>
        public static bool UpdateDetailsByLogout()
        {
            if (!UpdatePresentDay(new PresentDay { IdPresentDay = idPresentDay, TimeEnd = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) }))
            {
                System.Windows.Forms.MessageBox.Show("failed to update");
                return false;
            }
            return true;

        }
    }
}
