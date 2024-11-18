using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vent.DataAccess;
using Vent.Repositories.Interfaces;
using Vent.Shared.Entities;

namespace Vent.Repositories.Implementation
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            var listCountry = await _context.Countries
                .Include(x => x.States)
                .ToListAsync();

            return listCountry;
        }

        public async Task<IEnumerable<Country>> GetAsync()
        {
            var listCountry = await _context.Countries
                .ToListAsync();

            return listCountry;
        }
    }
}