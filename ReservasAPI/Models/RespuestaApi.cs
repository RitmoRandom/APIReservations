namespace ReservasAPI.Models
{
    public class RespuestaApi
    {
        public string httpResponseCode { get; set; }
        public List<reservations> LTareas { get; set; }
        public reservations Tarea { get; set; }
    }
}
