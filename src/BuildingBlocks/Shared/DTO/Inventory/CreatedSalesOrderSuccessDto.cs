namespace Shared.DTO.Inventory
{
    public class CreatedSalesOrderSuccessDto
    {
        public CreatedSalesOrderSuccessDto(string documentNo)
        {
            DocumentNo = documentNo;    
        }

        public string DocumentNo { get; }
    }
}
