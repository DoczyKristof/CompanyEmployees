namespace Entities.Exceptions
{
    public sealed class IdParamatersBadRequestException : BadRequestException
    {
        public IdParamatersBadRequestException()
            : base("Parameter ids is null.")
        {
            
        }
    }
}
