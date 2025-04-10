using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoombookingApp.Middleware;
using RoombookingApp.Models;

namespace RoombookingApp.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ReservationService _reservationService;

        public ReservationsController(ReservationService reservationService)
        { _reservationService = reservationService; }
        public IActionResult Index()
        {
            return View();
        }

        // przejscie do formularza zeby ustalic dane widelki czasowe 
        // gdzie chcemy sprawdzic jakie pokoje sa wolne
        [HttpGet]
        [RequireLoggedIn]
        public IActionResult GetAvailableRooms()
        {
            return View();
        }

        //wyswietlenie przefiltrowanych pokoi z walidacja danych
        [HttpPost]
        [RequireLoggedIn]
        public IActionResult GetAvailableRooms(RoomAvailabilityRequest request)
        {
            if (request.From >= request.To)
            { return BadRequest("Data rozpoczęcia rezerwacji musi być wcześniejsza niż data zakończenia."); }
            
            var rooms = _reservationService.GetAvailableRooms(request.From, request.To);
            ViewData["From"] = request.From;
            ViewData["To"] = request.To;
            return View("~/Views/Rooms/Index.cshtml", rooms);
        }
        [HttpGet]
        [RequireLoggedIn]
        public IActionResult ReserveRoom(int id)
        {
            ViewData["RoomId"] = id;
            ViewData["userId"] = HttpContext.Session.GetInt32("user_id");
            return View();
        }
        [HttpPost]
        [RequireLoggedIn]
        public async Task<IActionResult> ReserveRoom(int id, RoomAvailabilityRequest request)
        {
            int userId = HttpContext.Session.GetInt32("user_id") ?? 0;

            if (request.From >= request.To)
            { return BadRequest("Data rozpoczęcia rezerwacji musi być wcześniejsza niż data zakończenia."); }
            
            if (request.From < DateTime.Now)
            { return BadRequest("Data rozpoczecia musi byc późniejsza niz aktualny czas"); }

            var existingRoom = await _reservationService.Rooms
                .FirstOrDefaultAsync(r => r.Id == id);
            var rooms = _reservationService.GetAvailableRooms(request.From, request.To);
            
            if (existingRoom == null)
            { return BadRequest("nie ma takiego pokoju"); }

            if (!rooms.Contains(existingRoom))
            { return BadRequest("ten pokoj jest zajety"); }

            await _reservationService.ReserveRoom(request.From, request.To, existingRoom.Name, userId);
            return Ok(new { message = "rezerwacja udana" });
        }
        [RequireLoggedIn]
        public IActionResult GetMyReservation()
        {
            int userId = HttpContext.Session.GetInt32("user_id") ?? 0;
            var reservations = _reservationService.GetMyReservation(userId);
            if (reservations.Count == 0)
            {
                return Ok(new { message = "nie masz rezerwacji." });
            }
            return View(reservations);
        }

        [RequireLoggedIn]
        public IActionResult GetMyArchivedReservation()
        {
            int userId = HttpContext.Session.GetInt32("user_id") ?? 0;
            var reservations = _reservationService.GetMyArchivedReservation(userId);
            if (reservations.Count == 0)
            {
                return Ok(new { message = "nie masz rezerwacji." });
            }
            return View(reservations);
        }

        [RequireLoggedIn]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            int userId = HttpContext.Session.GetInt32("user_id") ?? 0;
            bool deleted = await _reservationService.DeleteReservation(id, userId);
            if (!deleted)
            { return NotFound(new { message = "Nie znaleziono rezerwacji lub brak uprawnień." }); }

            return RedirectToAction("GetMyReservation");

        }
    }
}
