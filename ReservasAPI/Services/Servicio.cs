using Grpc.Net.Client;
using Myservice;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace ReservasAPI.Services
{
    public class Servicio:IServicio
    {
        private static string SOAPDisponibilidad = "http://localhost:5000/?wsdl";
        private static string Rooms = "http://localhost:50051";
        public Servicio()
        {

        }
        public async Task<string> ConsultarDisponible(string tipo, DateTime fecha_inicio, DateTime fecha_fin)
        {
            var soapEnvelope = $@"<?xml version='1.0' encoding='UTF-8'?>
        <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:spyne='spyne.examples.availability'>
            <soapenv:Body>
                <spyne:get_availability>
                    <spyne:tipo>{tipo}</spyne:tipo>
                    <spyne:fecha_inicio>{fecha_inicio:yyyy-MM-dd}</spyne:fecha_inicio>
                    <spyne:fecha_fin>{fecha_fin:yyyy-MM-dd}</spyne:fecha_fin>
                </spyne:get_availability>
            </soapenv:Body>
        </soapenv:Envelope>";

            
            HttpClient clienteHttp = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, SOAPDisponibilidad);
            request.Headers.Add("SOAPAction", "spyne.examples.availability.get_availability");
            request.Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
            var response = await clienteHttp.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var responseXml = await response.Content.ReadAsStringAsync();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseXml);

                // Crear un XmlNamespaceManager y agregar los namespaces del XML
                var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                namespaceManager.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
                namespaceManager.AddNamespace("tns", "spyne.examples.availability");

                // Realizar la consulta XPath usando el XmlNamespaceManager
                var roomIdNode = xmlDoc.SelectSingleNode("//tns:get_availabilityResult/tns:string", namespaceManager);

                var roomIdText = roomIdNode?.InnerText;

                // Usar una expresión regular para extraer el Room ID
                if (!string.IsNullOrEmpty(roomIdText))
                {
                    var regex = new Regex(@"Room ID:\s*(\d+)"); // Expresión regular para capturar el número después de "Room ID:"
                    var match = regex.Match(roomIdText);
                    if (match.Success)
                    {
                        return match.Groups[1].Value; // Retorna solo el Room ID (número)
                    }
                }

                return null; // En caso de no encontrar un Room ID válido
            }
            return null;

        }
        public async Task<bool> Reservar(int room_id, DateTime fecha_inicio, DateTime fecha_fin, string accion)
        {
            var soapEnvelope = $@"<?xml version='1.0' encoding='UTF-8'?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:spyne=""spyne.examples.availability"">
    <soapenv:Body>
        <spyne:update_status>
            <spyne:room_id>{room_id}</spyne:room_id>
            <spyne:fecha_inicio>{fecha_inicio:yyyy-MM-dd}</spyne:fecha_inicio>
            <spyne:fecha_fin>{fecha_fin:yyyy-MM-dd}</spyne:fecha_fin>
            <spyne:accion>{accion}</spyne:accion>
        </spyne:update_status>
    </soapenv:Body>
</soapenv:Envelope>";


            HttpClient clienteHttp = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, SOAPDisponibilidad);
            request.Headers.Add("SOAPAction", "spyne.examples.availability.get_availability");
            request.Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
            var response = await clienteHttp.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;

        }
        public async Task<int> ConsultarRoomId(int room_id)
        {
            var channel = GrpcChannel.ForAddress(Rooms);
            var client = new MyService.MyServiceClient(channel);
            var request = new GetRoomByIdRequest
            {
                RoomId = room_id,
            };
            var response = client.GetRoomById(request);
            if (response!=null)
            {
                return response.RoomNumber;
            }
            else
            {
                return 0;
            }

        }
        public async Task<int> ConsultarRoomNum(int room_num)
        {
            var channel = GrpcChannel.ForAddress(Rooms);
            var client = new MyService.MyServiceClient(channel);
            var request = new GetRoomByNumRequest
            {
                RoomNumber = room_num,
            };
            var response = client.GetRoomByNum(request);
            if (response != null)
            {
                return response.RoomId;
            }
            else
            {
                return 0;
            }

        }
    }
}
