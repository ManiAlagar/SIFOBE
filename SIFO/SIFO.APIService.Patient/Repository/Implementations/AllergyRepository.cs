﻿using SIFO.Model.Entity;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Patient.Repository.Contracts;

namespace SIFO.APIService.Patient.Repository.Implementations
{
    public class AllergyRepository : IAllergyRepository
    {
        private readonly SIFOContext _context;

        public AllergyRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<long> AllergyNameExistsAsync(string? name, long? allergyId)
        {
            if (allergyId > 0)
            {
                return await _context.Allergys
                    .Where(c => c.Name.ToLower() == name.Trim().ToLower() && c.Id != allergyId).Select(a => a.Id).FirstOrDefaultAsync();
            }
            else
            {
                return await _context.Allergys
                     .Where(c => c.Name.ToLower() == name.Trim().ToLower()).Select(a => a.Id).FirstOrDefaultAsync();
            }
        }

        public async Task<bool> CreateAllergyAsync(Allergy entity)
        {
            try
            {
                var result = await _context.Allergys.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteAllergyAsync(long allergyId)
        {
            try
            {
                var allergyResponse = await _context.Allergys.Where(x => x.Id == allergyId).SingleOrDefaultAsync();
                if (allergyResponse is not null)
                {
                    _context.Allergys.Remove(allergyResponse);
                    await _context.SaveChangesAsync();
                    return Constants.SUCCESS;
                }
                return Constants.NOT_FOUND;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResponse<AllergyResponse>> GetAllAllergyAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll)
        {
            try
            {
                var query = from allergy in _context.Allergys
                            select new AllergyResponse
                            {
                                Id = allergy.Id,
                                Name = allergy.Name,
                                Description = allergy.Description,
                                IsActive = allergy.IsActive
                            };

                var count = _context.Allergys.Count();

                PagedResponse<AllergyResponse> pagedResponse = new PagedResponse<AllergyResponse>();

                if (isAll)
                {
                    var result = await query.ToListAsync();
                    pagedResponse.Result = result;
                    pagedResponse.TotalCount = result.Count;
                    pagedResponse.TotalPages = 0;
                    pagedResponse.CurrentPage = 0;
                    return pagedResponse;
                }

                string orderByExpression = $"{sortColumn} {sortDirection}";
                if (filter != null && filter.Length > 0)
                {
                    filter = filter.ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(filter));
                    count = query.Count();
                }
                query = query.OrderBy(orderByExpression).Skip((pageNo - 1) * pageSize).Take(pageSize).AsQueryable();
                pagedResponse.Result = query;
                pagedResponse.TotalCount = count;
                pagedResponse.TotalPages = (int)Math.Ceiling((pagedResponse.TotalCount ?? 0) / (double)pageSize);
                pagedResponse.CurrentPage = pageNo;
                return pagedResponse;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<AllergyResponse> GetAllergyByIdAsync(long allergyId)
        {
            try
            {
                var response = await (from allergy in _context.Allergys
                                      where allergy.Id == allergyId
                                      select new AllergyResponse
                                      {
                                          Id = allergy.Id,
                                          Name = allergy.Name,
                                          Description = allergy.Description,
                                          IsActive = allergy.IsActive
                                      }).FirstOrDefaultAsync(); ;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAllergyAsync(Allergy entity, long allergyId)
        {
            try
            {
                var result = await _context.Allergys.Where(a => a.Id == allergyId).FirstOrDefaultAsync();
                if (result is not null)
                {
                    result.Name = entity.Name;
                    result.Description = entity.Description;
                    result.IsActive = entity.IsActive;
                    result.UpdatedDate = entity.UpdatedDate;
                    result.UpdatedBy = entity.UpdatedBy;
                    _context.Allergys.Update(result);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}