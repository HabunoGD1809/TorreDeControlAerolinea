using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TorreDeControlAerolinea.Models;

namespace TorreDeControlAerolinea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvionesController : ControllerBase
    {
        private readonly TorreDeControlContext _context;

        public AvionesController(TorreDeControlContext context)
        {
            _context = context;
        }

        // GET: api/Aviones
        [HttpGet]
        public ActionResult<IEnumerable<Avion>> GetAviones()
        {
            return _context.Aviones.ToList();
        }

        // GET: api/Aviones/{id}
        [HttpGet("{id}")]
        public ActionResult<Avion> GetAvion(int id)
        {
            var avion = _context.Aviones.Find(id);

            if (avion == null)
            {
                return NotFound();
            }

            return avion;
        }

        // POST: api/Aviones
        [HttpPost]
        public ActionResult<IEnumerable<Avion>> PostAvion(List<Avion> aviones)
        {
            foreach (var avion in aviones)
            {
                _context.Aviones.Add(avion);
                _context.SaveChanges();
            }

            return CreatedAtAction("GetAviones", aviones);
        }


        // DELETE: api/Aviones/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteAvion(int id)
        {
            var avion = _context.Aviones.Find(id);

            if (avion == null)
            {
                return NotFound();
            }

            _context.Aviones.Remove(avion);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/Aviones/{id}/Pasajeros
        [HttpGet("{id}/Pasajeros")]
        public ActionResult<IEnumerable<Pasajero>> GetPasajerosByAvion(int id)
        {
            var pasajeros = _context.Pasajeros.Where(p => p.AvionId == id).ToList();
            return pasajeros;
        }

        // POST: api/Aviones/{id}/Pasajeros
        [HttpPost("{avionId}/Pasajeros")]
        public ActionResult<Pasajero> PostPasajero(int avionId, Pasajero pasajero)
        {
            var avion = _context.Aviones.FirstOrDefault(a => a.AvionId == avionId);

            if (avion == null)
            {
                return NotFound("El avión especificado no existe.");
            }

            decimal pesoTotalEquipaje = _context.Pasajeros
                .Where(p => p.AvionId == avionId)
                .Sum(p => p.PesoEquipaje) + pasajero.PesoEquipaje;

            if (pesoTotalEquipaje > avion.PesoLimite)
            {
                return BadRequest("El peso total del equipaje excede el límite del avión.");
            }

            pasajero.AvionId = avionId;
            _context.Pasajeros.Add(pasajero);
            _context.SaveChanges();

            return CreatedAtAction("GetPasajero", new { id = pasajero.PasajeroId }, pasajero);
        }

        // GET: api/Aviones/{id}/EstadoVuelo
        [HttpGet("{id}/EstadoVuelo")]
        public ActionResult<string> GetEstadoVuelo(int id)
        {
            var avion = _context.Aviones.Find(id);

            if (avion == null)
            {
                return NotFound();
            }

            DateTime now = DateTime.Now;

            if (now < avion.HoraSalida)
            {
                return $"El avión aún no ha salido del aeropuerto. Salida programada a las {avion.HoraSalida}.";
            }
            else if (now > avion.HoraAterrizaje)
            {
                avion.EstatusVuelo = "Aterrizado";
                _context.SaveChanges();
                return $"El avión ha aterrizado.";
            }
            else
            {
                TimeSpan tiempoRestante = avion.HoraAterrizaje - now;
                return $"El avión está en vuelo. Faltan {tiempoRestante.TotalHours} horas para aterrizar.";
            }
        }
    }
    //AeropuertosController
    [Route("api/[controller]")]
    [ApiController]
    public class AeropuertosController : ControllerBase
    {
        private readonly TorreDeControlContext _context;

        public AeropuertosController(TorreDeControlContext context)
        {
            _context = context;
        }

        // GET: api/Aeropuertos
        [HttpGet]
        public ActionResult<IEnumerable<Aeropuerto>> GetAeropuertos()
        {
            var aeropuertos = _context.Aeropuertos.ToList();

            if (aeropuertos == null)
            {
                return NotFound();
            }
            return aeropuertos;
        }

        //POST: api/Aeropuertos
        [HttpPost]
        public ActionResult<IEnumerable<Aeropuerto>> PostAeropuertos(List<Aeropuerto> aeropuertos)
        {
            _context.Aeropuertos.AddRange(aeropuertos);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAeropuertos), aeropuertos);
        }



        //GET: api/Aeropuertos/{id}
        [HttpGet("{id}")]
        public ActionResult<Aeropuerto> GetAeropuertoById(int id)
        {
            var aeropuerto = _context.Aeropuertos.Find(id);

            if (aeropuerto == null)
            {
                return NotFound();
            }

            return aeropuerto;
        }

        // GET: api/Aeropuertos/{id}/AvionesSalida
        [HttpGet("{id}/AvionesSalida")]
        public ActionResult<IEnumerable<Avion>> GetAvionesByAeropuertoSalida(int id)
        {
            var aviones = _context.Aviones
                .Where(a => a.AeropuertoSalidaId == id)
                .ToList();

            return aviones;
        }

        // GET: api/Aeropuertos/{id}/AvionesLlegada
        [HttpGet("{id}/AvionesLlegada")]
        public ActionResult<IEnumerable<Avion>> GetAvionesByAeropuertoLlegada(int id)
        {
            var aviones = _context.Aviones
                .Where(a => a.AeropuertoLlegadaId == id)
                .ToList();

            return aviones;
        }

        // DELETE: api/Aeropuertos
        [HttpDelete]
        public IActionResult DeleteAeropuertos()
        {
            var aeropuertos = _context.Aeropuertos.ToList();

            foreach (var aeropuerto in aeropuertos)
            {
                _context.Aeropuertos.Remove(aeropuerto);
            }

            _context.SaveChanges();

            return NoContent();
        }

    }
}
