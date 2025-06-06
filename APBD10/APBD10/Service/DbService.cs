using APBD10.Data;
using APBD10.DTO;
using Microsoft.EntityFrameworkCore;

namespace APBD10.Service;

public class DbService
{
    public interface ITripService
    {
        Task<PagedTripsDto> GetTripsAsync(int page, int pageSize);
    }

    public class TripService : ITripService
    {
        private readonly Apbd10Context _context;

        public TripService(Apbd10Context context)
        {
            _context = context;
        }

        public async Task<PagedTripsDto> GetTripsAsync(int page, int pageSize)
        {
            var query = _context.Trips
                .Include(t => t.IdCountries)
                .Include(t => t.ClientTrips).ThenInclude(ct => ct.IdClientNavigation)
                .OrderByDescending(t => t.DateFrom);

            var totalTrips = await query.CountAsync();
            var trips = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TripDto
                {
                    Name = t.Name,
                    Description = t.Description,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo,
                    MaxPeople = t.MaxPeople,
                    Countries = t.IdCountries.Select(c => c.Name).ToList(),
                    Clients = t.ClientTrips.Select(ct => new ClientDto
                    {
                        FirstName = ct.IdClientNavigation.FirstName,
                        LastName = ct.IdClientNavigation.LastName
                    }).ToList()
                })
                .ToListAsync();

            var allPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

            return new PagedTripsDto
            {
                PageNum = page,
                PageSize = pageSize,
                AllPages = allPages,
                Trips = trips
            };
        }
    }
}