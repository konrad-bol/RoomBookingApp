using Microsoft.Extensions.Logging;
using RoombookingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace RoombookingApp.Middleware
{
    public class ReservationArchiver :BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReservationArchiver> _logger;
        public ReservationArchiver(IServiceScopeFactory scopeFactory, ILogger<ReservationArchiver> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var oldReservations = await dbContext.Reservations
                            .Where(r => !r.Archived && r.ReservationTo < DateTime.Now.AddMinutes(-5))
                            .ToListAsync(stoppingToken);
                        Console.WriteLine(DateTime.Now);
                        Console.WriteLine("dzialam");
                        if (oldReservations.Any())
                        {
                            Console.WriteLine("jeste stara rezerwacja");
                            foreach (var reservation in oldReservations)
                            {
                                reservation.Archived = true;
                            }
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("Zarchiwizowano {Count} rezerwacji.", oldReservations.Count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Błąd podczas archiwizacji rezerwacji.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
