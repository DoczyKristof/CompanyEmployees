namespace Entities.Exceptions
{
    public class RoleNotExistsException : Exception
    {
        public RoleNotExistsException(string role) : base($"The role {role} is not found in the database.") { }
    }
}
