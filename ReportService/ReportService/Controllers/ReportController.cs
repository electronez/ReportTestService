namespace ReportService.Controllers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;
    using Services;

    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly IEmployeeService employeeService;

        public ReportController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        }
        
        [HttpGet]
        [Route("{year}/{month}")]
        public async Task<IActionResult> Download(int year, int month)
        {
            Log.Logger.Debug("Init ReportBuilder: year: {@year}, month: {@month}", year, month);
            var report = new ReportBuilder(year, month);

            Log.Logger.Debug("GetActiveEmployees");
            var employees = await this.employeeService.GetActiveEmployees();

            Log.Logger.Debug("Fill report");
            foreach (var employeesByDepartment in employees.GroupBy(o => o.Department))
            {
                report.FillDepartmentInfo(employeesByDepartment.Key, employeesByDepartment);
            }
            
            report.FillTotalInfo();
            
            Log.Logger.Debug("Finish download");
            return File(Encoding.UTF8.GetBytes(report.ToString()), "text/plain", "report.txt");
        }
    }
}
