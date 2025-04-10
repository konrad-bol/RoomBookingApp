using Microsoft.Extensions.Logging;
using RoombookingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace RoombookingApp.Middleware
{
    //archiwizacja starych rezerwacji 
    //podczas pracy aplikacji rezerwacje ktore sa juz starsze niz X czasu 
    // beda mialy ustawiona flage archived
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
                        //pobranie starych rezerwacji ktore nie maja flagi
                        var oldReservations = await dbContext.Reservations
                            .Where(r => !r.Archived && r.ReservationTo < DateTime.Now.AddMinutes(-5)) //starsze niz 5 min w tym przypadku
                            .ToListAsync(stoppingToken);
                        if (oldReservations.Any())
                        {
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
                // okreslenie cyklu dzialania aplikacji 
                // w tym przypadku co minute bedzie sie zapetlac
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
