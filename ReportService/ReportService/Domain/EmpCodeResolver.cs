namespace ReportService.Domain
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Serilog;

    public class EmpCodeResolver
    {
        public static async Task<string> GetCode(string inn)
        {
            try
            {
                var client = new HttpClient();
                return await client.GetStringAsync("http://buh.local/api/inn/" + inn);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Error on EmpCodeResolver.GetCode");
                throw e;
            }
        }
    }
}