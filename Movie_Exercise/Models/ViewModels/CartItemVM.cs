namespace Movie_Exercise.Models.ViewModels
{
    public class CartItemVM
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string MovieTitle { get; set; }
        public int MovieId { get; set; }
        public decimal Price { get; set; }
        public string ImgFile { get; set; }

        public decimal CartItemTotal()
        {
            return this.Price * this.Quantity;
        }
        public CartItemVM()
        {
        }

        public CartItemVM(int quantity, string movieTitle, int movieId, decimal price)
        {
            Quantity = quantity;
            MovieId = movieId;
            MovieTitle = movieTitle;
            Price = price;
        }
    }
}
