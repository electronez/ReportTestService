namespace ReportService.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class ReportBuilder
    {
        private const string WL = "--------------------------------------------";
        private const string WT = "         ";
        private const string DepartmentTotalSalary = "Всего по отделу";
        private const string TotalSalary = "Всего по предприятию";
        
        private StringBuilder sb;
        private int total;

        public ReportBuilder(int year, int month)
        {
            this.sb = new StringBuilder();
            this.sb.AppendLine(new DateTime(year, month, 01).ToString("MMMM yyyy", new CultureInfo("ru-Ru")));
            sb.AppendLine(WL);
        }

        public void FillDepartmentInfo(string departmentName, IEnumerable<Employee> employees)
        {
            sb.AppendLine(departmentName);

            foreach (var employee in employees)
            {
                this.FillSalaryInfo(employee.Name, employee.Salary);
            }

            var departmentTotal = employees.Sum(o => o.Salary);
            this.FillSalaryInfo(DepartmentTotalSalary, departmentTotal);
            total += departmentTotal;
        }

        public void FillTotalInfo()
        {
            sb.AppendLine(WL);
            this.FillSalaryInfo(TotalSalary, this.total);
        }

        public override string ToString()
        {
            return this.sb.ToString();
        }

        private void FillSalaryInfo(string objectName, int salary)
        {
            this.sb.AppendLine($"{objectName}{WT}{salary}р");
        }
    }
}