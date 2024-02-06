namespace com.ss.genericrepository.models
{
    public class Pagination<T>
    {
        public uint PageNumber { get; set; }
        public uint PageSize { get; set; }
        public uint FirstRowIndexFromPage
        {
            get
            {
                return (PageNumber * PageSize) - (PageSize - 1);
            }
        }
        public uint LastRowIndexFromPage
        {
            get
            {
                return PageNumber * PageSize;
            }
        }
        public uint TotalRow { get; set; }
        public uint TotalPage
        {
            get
            {
                return (uint)Math.Ceiling((decimal)(TotalRow / PageSize));
            }
        }
        public bool HasPreviousPage
        {
            get
            {
                return PageNumber - 1 > 0;
            }
        }
        public bool HasNextPage
        {
            get
            {
                return PageNumber + 1 < TotalPage;
            }
        }

        public List<T> ItemList { get; set; } = null;

        public Pagination()
        {
        }

        public Pagination(uint pageNumber, uint pageSize, uint totalRow, List<T> itemList)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRow = totalRow;
            ItemList = itemList;
        }

        public Pagination<T> GetNextPage()
        {
            if (HasNextPage)
                PageNumber++;
            return this;
        }

        public Pagination<T> GetPreviousPage()
        {
            if (HasPreviousPage)
                PageNumber--;
            return this;
        }
    }
}

