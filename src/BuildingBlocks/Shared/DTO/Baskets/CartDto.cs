namespace Shared.DTO.Baskets
{
    public class CartDto
    {
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice
        {
            get { return Items.Sum(x => x.ItemPrice * x.Quantity); }
        }


        public CartDto() { }
        public CartDto(string username)
        {
            UserName = username;
        }
    }
}
