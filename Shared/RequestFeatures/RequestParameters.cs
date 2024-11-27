namespace Shared.RequestFeatures
{
    public abstract class RequestParameters
    {
        const int MAXPAGESIZE = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSzize = 10;

        public int PageSize
        {
            get { return _pageSzize; }
            set { _pageSzize = (value > MAXPAGESIZE) ? MAXPAGESIZE : value; }
        }

        public string? OrderBy { get; set; }
    }
}
