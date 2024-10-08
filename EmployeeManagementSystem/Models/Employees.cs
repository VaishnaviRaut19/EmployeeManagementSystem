using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Employees
    {
        public int Id { get; set; }
        public string? EmployeeName { get; set; }
        
        public string? Designetion { get; set; }

        public string? Address {  get; set; }

        public DateTime? Joindate { get; set; }
        public string? Email { get; set; }
        
        public string? Password { get; set; }

        public string? Confirmpassword { get; set; }

    }
}
