using System.Threading.Tasks;

namespace EmployeeDataManipulation
{
    public interface IEmployeeRepo
    {
        Task<bool> CreateEmployeeAsync(Employee emp);

        Task<bool> UpdateEmployeeAsync(Employee emp);

        Task<bool> DeleteEmployeeAsync(Employee emp);
    }
}