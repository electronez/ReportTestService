namespace ReportService.Domain
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Serilog;

    public static class EmployeeCommonMethods
    {
        public static async Task<int> Salary(this Employee employee)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://salary.local/api/empcode/"+employee.Inn);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(new { employee.BuhCode });
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                var reader = new System.IO.StreamReader(httpResponse.GetResponseStream(), true);
                string responseText = reader.ReadToEnd();
                return int.Parse(responseText);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Error on EmployeeCommonMethods.Salary");
                throw e;
            }
        }

    }
}
