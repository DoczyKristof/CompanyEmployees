namespace Entities.Exceptions
{
    public sealed class CompanyCollectionBadRequest : BadRequestException
    {
        public CompanyCollectionBadRequest()
            : base("Company collection sent by client is null. :(")
        {
            
        }
    }
}
