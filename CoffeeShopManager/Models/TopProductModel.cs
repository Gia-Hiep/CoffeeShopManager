namespace CoffeeShopManager.Models
{
    public class TopProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public int TotalQuantity { get; set; }
    }
}