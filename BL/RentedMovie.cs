namespace matala5_server.BL
{
    public class RentedMovie
    {
        public int userId { get; set; }
        public int movieId { get; set; }
        public DateTime rentStart { get; set; }
        public DateTime rentEnd { get; set; }
        public double totalPrice { get; set; }
    }
}
