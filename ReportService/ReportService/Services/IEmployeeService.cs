namespace ReportService.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;

    public interface IEmployeeService
    {
        /// <summary>
        /// Получение списка сотрудников.
        /// </summary>
        /// <returns>Список сотрудников.</returns>
        Task<IEnumerable<Employee>> GetActiveEmployees();
    }
}