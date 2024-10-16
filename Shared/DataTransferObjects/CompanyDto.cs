namespace Shared.DataTransferObjects
{
    public record CompanyDto
    {
        public Guid Id { get; init; } //kérdőjel
        public string? Name { get; init; }
        public string? FullAddress { get; init; }

        public override string ToString()
        {
            return $"{Id},{Name},{FullAddress}";
        }
    }
}
