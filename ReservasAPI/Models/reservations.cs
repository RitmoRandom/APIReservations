namespace ReservasAPI.Models
{
    public class reservations
    {
        public int Id { get; set; }
        public int room_number { get; set; }
        public string customer_name { get; set;}
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string status { get; set; }
    }
}
