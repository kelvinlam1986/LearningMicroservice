namespace Shared.SeedWork
{
    public class PagedList<T> : List<T>
    {
        private MetaData _metaData;

        public PagedList(IEnumerable<T> items, long totalItems, int pageNumber, int pageSize)
        {
            _metaData = new MetaData();
            _metaData.TotalItems = totalItems;
            _metaData.PageSize = pageSize;
            _metaData.CurrentPage = pageNumber;
            _metaData.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            AddRange(items);
        }

        public MetaData GetMetaData()
        {
            return _metaData;
        }
    }
}
