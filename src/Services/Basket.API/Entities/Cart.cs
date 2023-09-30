namespace Basket.API.Entities
{
    public class Cart
    {
        public string UserName { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalPrice 
        { 
            get { return Items.Sum(x => x.ItemPrice * x.Quantity); } 
        }


        public Cart() { }
        public Cart(string username)
        {
            UserName = username;
        }
    }
}
