namespace ReservasAPI.Services
{
    public interface IServicio
    {
        Task<string> ConsultarDisponible(string tipo, DateTime fecha_inicio, DateTime fecha_fin);
        Task<bool> Reservar(int room_id, DateTime fecha_inicio, DateTime fecha_fin, string accion);
        Task<int> ConsultarRoomId(int room_id);
        Task<int> ConsultarRoomNum(int room_num);
    }
}
