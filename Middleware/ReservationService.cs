using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoombookingApp.Data;
using RoombookingApp.Models;

namespace RoombookingApp.Middleware
{
    public class ReservationService
    {
        private readonly AppDbContext _context;

        public ReservationService(AppDbContext context)
        { _context = context; }

        public List<Room> GetAvailableRooms(DateTime from, DateTime to)
        {
            return _context.Rooms
                .Include(r => r.Reservations)
                .Where(r => !r.Reservations.Any(res =>
                    (res.ReservationFrom < to && res.ReservationTo > from)))
                .ToList();
        }
        public IQueryable<Room> Rooms => _context.Rooms;
        public async Task<bool> ReserveRoom(DateTime from, DateTime to, string name, int userId)
        {

            var room = await _context.Rooms
                .Include(r => r.Reservations)
                .FirstOrDefaultAsync(r => r.Name == name);
            var reservation = new Reservation
            {
                Room = room,
                UserId = userId,
                ReservationFrom = from,
                ReservationTo = to,
                RoomId = room.Id
            };

            room.Reservations.Add(reservation);

            _context.Rooms.Update(room);
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteReservation(int id, int userId)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null || reservation.UserId != userId)
            {
                return false;
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
        }


        public List<Reservation> GetMyReservation(int id)
        {
            return _context.Reservations
                .Include(r => r.Room)
                .Where(r => r.UserId == id && !r.Archived)
                .ToList();
        }
        public List<Reservation> GetMyArchivedReservation(int id)
        {
            return _context.Reservations
                .Include(r => r.Room)
                .Where(r => r.UserId == id && r.Archived)
                .ToList();
        }
    }
}
