using Microsoft.EntityFrameworkCore;
using Vent.DataAccess;
using Vent.Repositories.Helpers;
using Vent.Repositories.Interfaces;
using Vent.Shared.Entities;
using Vent.Shared.Pagination;
using Vent.Shared.Responses;

namespace Vent.Repositories.Implementation
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Response> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Countries.Include(x => x.States).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double counting = await queryable.CountAsync();
            Response result = new()
            {
                IsSuccess = true,
                Result = await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync(),
                CountItem = counting
            };

            return result;
        }
    }
}