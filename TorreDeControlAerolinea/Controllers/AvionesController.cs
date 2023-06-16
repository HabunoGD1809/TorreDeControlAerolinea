using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TorreDeControlAerolinea.Models;
using System.Linq;

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
            // Validar restricciones de horarios de salida y llegada
            foreach (var avion in aviones)
            {
                if (_context.Aviones.Any(a =>
                    a.AeropuertoSalidaId == avion.AeropuertoSalidaId &&
                    a.HoraSalida == avion.HoraSalida))
                {
                    return BadRequest($"Ya existe un avión con salida del aeropuerto {avion.AeropuertoSalidaId} a la misma hora.");
                }

                if (_context.Aviones.Any(a =>
                    a.AeropuertoLlegadaId == avion.AeropuertoLlegadaId &&
                    a.HoraAterrizaje == avion.HoraAterrizaje))
                {
                    return BadRequest($"Ya existe un avión con llegada al aeropuerto {avion.AeropuertoLlegadaId} a la misma hora.");
                }
            }

            _context.Aviones.AddRange(aviones);
            _context.SaveChanges();

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

        // GET api/pasajeros/avion/{avionId}
        [HttpGet("avion/{avionId}")]
        public IActionResult GetPasajerosPorAvion(int avionId)
        {
            // Obtén el avión con los pasajeros correspondientes
            var avion = _context.Aviones
                .Include(a => a.AeropuertoSalida)
                .Include(a => a.AeropuertoLlegada)
                .Include(a => a.Pasajeros)
                .FirstOrDefault(a => a.AvionId == avionId);

            if (avion == null)
            {
                return NotFound(); // El avión no fue encontrado
            }

            // Obtén los pasajeros del avión
            var pasajeros = avion.Pasajeros.ToList();

            return Ok(pasajeros);
        }

        //POST PASAJEROS
        [HttpPost]
        [Route("{avionId}/pasajeros")]
        public IActionResult AgregarPasajero(int avionId, Pasajero pasajero)
        {
            var avion = _context.Aviones.FirstOrDefault(a => a.AvionId == avionId);
            if (avion == null)
            {
                return NotFound($"No se encontró el avión con ID {avionId}.");
            }

            // Verificar si el avión ha aterrizado o ya está en vuelo
            if (avion.EstatusVuelo != "En espera")
            {
                return BadRequest($"No se puede agregar un pasajero al avión con ID {avionId} porque ya ha aterrizado o está en vuelo.");
            }

            // Obtener la lista actual de pasajeros del avión
            var pasajerosActuales = _context.Pasajeros.Where(p => p.AvionId == avionId).ToList();

            if (pasajerosActuales.Count >= avion.LimitePasajeros)
            {
                return BadRequest($"El avión con ID {avionId} ha alcanzado su límite de pasajeros.");
            }

            decimal pesoEquipajeActual = pasajerosActuales.Sum(p => p.PesoEquipaje);

            // Verificar si el peso del nuevo pasajero excede el límite de peso del avión
            if (pesoEquipajeActual + pasajero.PesoEquipaje > avion.PesoLimite)
            {
                return BadRequest($"El peso del pasajero excede el límite de peso del avión con ID {avionId}.");
            }

            // Asignar el ID del avión al pasajero y guardarlo en la base de datos
            pasajero.AvionId = avionId;
            _context.Pasajeros.Add(pasajero);
            _context.SaveChanges();

            return Ok($"Se ha agregado el pasajero con ID {pasajero.PasajeroId} al avión con ID {avionId}.");
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

        // POST: api/Aeropuertos
        [HttpPost]
        public IActionResult PostAeropuerto(List<Aeropuerto> aeropuertos)
        {
            var aeropuertosExistentes = _context.Aeropuertos.ToList();

            foreach (var aeropuerto in aeropuertos)
            {
                // Verificar si el aeropuerto ya existe
                if (aeropuertosExistentes.Any(a => a.Nombre == aeropuerto.Nombre))
                {
                    return Conflict($"Ya existe un aeropuerto con el nombre {aeropuerto.Nombre}.");
                }

                // Verificar si se ha alcanzado el límite de aviones por aeropuerto
                var avionesEnAeropuerto = _context.Aviones
                    .Where(a => a.AeropuertoSalidaId == aeropuerto.AeropuertoId || a.AeropuertoLlegadaId == aeropuerto.AeropuertoId)
                    .ToList();

                int limiteAviones = aeropuerto.LimiteAviones;
                if (avionesEnAeropuerto.Count >= limiteAviones)
                {
                    return BadRequest($"El aeropuerto con ID {aeropuerto.AeropuertoId} ha alcanzado su límite de aviones.");
                }

                _context.Aeropuertos.Add(aeropuerto);
            }

            _context.SaveChanges();

            return CreatedAtAction("GetAeropuerto", new { id = aeropuertos[0].AeropuertoId }, aeropuertos);
        }


        // GET: api/Aeropuertos/{id}
        [HttpGet("{id}")]
        public ActionResult<Aeropuerto> GetAeropuerto(int id)
        {
            var aeropuerto = _context.Aeropuertos.Find(id);

            if (aeropuerto == null)
            {
                return NotFound();
            }

            return aeropuerto;
        }

        // GET: api/Aeropuertos/GetAeropuertoPorId/{id}
        [HttpGet("GetAeropuertoPorId/{id}")]
        public ActionResult<Aeropuerto> GetAeropuertoPorId(int id)
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
            _context.Aeropuertos.RemoveRange(aeropuertos);
            _context.SaveChanges();

            return NoContent();
        }

    }
}
