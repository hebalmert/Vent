using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
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

        public async Task<Response> GetIdAsync(int Id)
        {
            if (Id != 0)
            {
                var resultId = await _context.Countries.FindAsync(Id);
                return new Response { IsSuccess = true, Result = resultId };
            }
            return new Response { IsSuccess = false };
        }

        public async Task<Response> PostAsync(Country country)
        {
            if (country is not null)
            {
                _context.Countries.Add(country);
                await _context.SaveChangesAsync();
                return new Response { IsSuccess = true, Result = country };
            }

            return new Response { IsSuccess = false };
        }

        public async Task<Response> PutAsync(Country country)
        {
            if (country is not null)
            {
                _context.Countries.Update(country);
                await _context.SaveChangesAsync();
                return new Response { IsSuccess = true, Result = country };
            }
            return new Response { IsSuccess = false };
        }

        public async Task<Response> DeleteAsync(int Id)
        {
            try
            {
                var resulDelete = await _context.Countries.FindAsync(Id);
                _context.Countries.Remove(resulDelete!);
                await _context.SaveChangesAsync();
                return new Response { IsSuccess = true };
            }
            catch (DbUpdateException dbUpdateException)
            {

                if (dbUpdateException.InnerException!.Message.Contains("REFERENCE"))
                {
                    return new Response
                    { IsSuccess = false, Message = "REFERENCE" };
                }
                else
                {
                    return new Response
                    { IsSuccess = false, Message = dbUpdateException.InnerException.Message };
                }
            }
            catch (Exception e)
            {
                return new Response
                { IsSuccess = false, Message = e.Message };
            }

        }
    }
}