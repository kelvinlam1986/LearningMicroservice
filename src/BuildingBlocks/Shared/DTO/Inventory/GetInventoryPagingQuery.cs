using Shared.SeedWork;

namespace Shared.DTO.Inventory
{
    public class GetInventoryPagingQuery : PagingRequestParameters
    {
        private string _itemNo;

        public string ItemNo() => _itemNo;

        public void SetItemNo(string itemNo) => _itemNo = itemNo;

        public string? SearchTerm { get; set; }
    }
}
