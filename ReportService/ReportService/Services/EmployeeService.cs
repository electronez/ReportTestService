namespace ReportService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;
    using Npgsql;
    using Serilog;

    public class EmployeeService : IEmployeeService
    {
        private readonly NpgsqlConnection connection;

        private const string ActiveEmployeesQuery =
            "SELECT e.name, e.inn, d.name from emps e inner join deps d on e.departmentid = d.id where d.active = true";

        public EmployeeService(NpgsqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc cref="IEmployeeService"/> 
        public async Task<IEnumerable<Employee>> GetActiveEmployees()
        {
            var employees = await this.GetActiveEmployeesFromDatabase();
            var tasks = new List<Task>();
            
            Log.Logger.Debug("Fill salary data");
            foreach (var employee in employees)
            {
                var task = await Task.Factory.StartNew(async param =>
                {
                    var employeeInTask = (Employee)param;
                    employeeInTask.Salary = await employee.Salary();
                    employeeInTask.BuhCode = await EmpCodeResolver.GetCode(employee.Inn);
                }, employee);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            return employees;
        }
        
        /// <inheritdoc cref="IEmployeeRepository"/>
        private async Task<IEnumerable<Employee>> GetActiveEmployeesFromDatabase()
        {
            Log.Logger.Debug("Get active employees from DB");
            var result = new List<Employee>();
            await this.connection.OpenAsync();
            var cmd = new NpgsqlCommand(ActiveEmployeesQuery, this.connection);
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new Employee()
                {
                    Name = reader.GetString(0),
                    Inn = reader.GetString(1),
                    Department = reader.GetString(2)
                });
            }

            return result;
        }
    }
}