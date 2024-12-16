using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasAPI.Data;
using ReservasAPI.Models;
using ReservasAPI.Services;

namespace ReservasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class reservationsController : ControllerBase
    {
        private readonly ReservasAPIContext _context;
        protected RespuestaApi _resultadoApi;
        private readonly IServicio _servicioApi;

        public reservationsController(ReservasAPIContext context, IServicio servicioapi)
        {
            _context = context;
            _resultadoApi = new();
            _servicioApi = servicioapi;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<reservations>> Getreservations(int id)
        {
            reservations usu = await _context.reservations.FirstOrDefaultAsync(x => x.Id == id);
            if (usu != null)
            {
                _resultadoApi.Tarea = usu;
                _resultadoApi.httpResponseCode = HttpStatusCode.OK.ToString();
                return Ok(_resultadoApi);
            }
            else
            {
                _resultadoApi.httpResponseCode = HttpStatusCode.BadRequest.ToString();
                return BadRequest(_resultadoApi);
            }
        }


        [HttpPost]
        [Route("Post/{tipo}")]
        public async Task<ActionResult<reservations>> Postreservations(string tipo,[FromBody]reservations reservations)
        {
            reservations us = await _context.reservations.FirstOrDefaultAsync(x => x.Id == reservations.Id);
            if (us == null)
            {
                string habitacion=await _servicioApi.ConsultarDisponible(tipo,reservations.start_date,reservations.end_date);
                int idRoom = int.Parse(habitacion);
                if (idRoom != 0)
                {
                    int roomnum = await _servicioApi.ConsultarRoomId(idRoom);
                    reservations.room_number=roomnum;
                    bool resp=await _servicioApi.Reservar(idRoom, reservations.start_date, reservations.end_date, "occupied");
                    if (resp)
                    {
                        await _context.reservations.AddAsync(reservations);
                        await _context.SaveChangesAsync();
                        _resultadoApi.httpResponseCode = HttpStatusCode.OK.ToString().ToUpper();
                        return Ok(_resultadoApi);
                    }
                    else
                    {
                        _resultadoApi.httpResponseCode = HttpStatusCode.BadRequest.ToString().ToUpper();
                        return BadRequest(_resultadoApi);
                    }
                }
                else
                {
                    _resultadoApi.httpResponseCode = HttpStatusCode.Conflict.ToString().ToUpper();
                    return Ok(_resultadoApi);
                }
            }
            else
            {
                _resultadoApi.httpResponseCode = HttpStatusCode.BadRequest.ToString().ToUpper();
                return BadRequest(_resultadoApi);
            }
        }

        // DELETE: api/reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletereservations(int id)
        {
            reservations tec = await _context.reservations.FirstOrDefaultAsync(x => x.Id == id);
            if (tec != null)
            {
                int roomId = await _servicioApi.ConsultarRoomNum(tec.room_number);
                bool resp = await _servicioApi.Reservar(roomId, tec.start_date, tec.end_date, "available");
                if (resp)
                {
                    _context.reservations.Remove(tec);
                    await _context.SaveChangesAsync();
                    _resultadoApi.httpResponseCode = HttpStatusCode.OK.ToString();
                    return Ok(_resultadoApi);
                }
                else
                {
                    _resultadoApi.httpResponseCode = HttpStatusCode.BadRequest.ToString().ToUpper();
                    return BadRequest(_resultadoApi);
                }

            }
            else
            {
                _resultadoApi.httpResponseCode = HttpStatusCode.BadRequest.ToString();
                return BadRequest(_resultadoApi);
            }
        }
    }
}
