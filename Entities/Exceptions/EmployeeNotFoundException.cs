namespace Entities.Exceptions
{
    public sealed class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException(Guid employeeId) : base($"The employee (id: {employeeId}) doesn't exist in the database.") { }
    }
}
