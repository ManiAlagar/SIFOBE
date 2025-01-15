using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using System.Linq;

namespace SIFO.APIService.Hospital.Repository.Implementations
{
    public class DrugRepository : IDrugRepository
    {

        private readonly SIFOContext _context;
        private readonly ICommonService _commonService;
        private readonly IConfiguration _configuration;
        public DrugRepository(SIFOContext context, ICommonService commonService, IConfiguration configuration)
        {
            _context = context;
            _commonService = commonService;
            _configuration = configuration;
        }
        public async Task<bool> IsRegionExists(List<DrugRegionRequest> regionRequests)
        {
            var regionIds = regionRequests.Select(r => r.RegionId).ToList();
            return await _context.States
                                 .Where(a => regionIds.Contains(a.Id))
                                 .AnyAsync();
        }


        public async Task<bool> SaveDrugAsync(DrugRequest drugRequest, long userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create a new Drug entity
                var drugData = new Drugs()
                {
                    DD = drugRequest.DD,
                    DPC = drugRequest.DPC,
                    InPharmacy = drugRequest.InPharmacy,
                    AIC = drugRequest.AIC.Trim(),
                    ExtendedDescription = drugRequest.ExtendedDescription.Trim(),
                    CompanyName = drugRequest.CompanyName.Trim(),
                    Price = drugRequest.Price,
                    ProductType = drugRequest.ProductType.Trim(),
                    Class = drugRequest.Class.Trim(),
                    PharmaceuticalForm = drugRequest.PharmaceuticalForm.Trim(),
                    UMR = drugRequest.UMR,
                    PrescriptionType = drugRequest.PrescriptionType.Trim(),
                    ProductImage = drugRequest.ProductImage.Trim(),
                    TherapeuticIndications = drugRequest.TherapeuticIndications.Trim(),
                    Temperature = drugRequest.Temperature.Trim(),
                    NumberGGAlert = drugRequest.NumberGGAlert,
                    AlertHours = drugRequest.AlertHours,
                    IsActive = true,
                    DrugDosage = drugRequest.DrugDosage,
                    CreatedBy = userId
                };

                if (!string.IsNullOrEmpty(drugRequest.ProductImage))
                {
                    var writtenPath = await _commonService.SaveFileAsync(drugRequest.ProductImage, null, Path.Join(_configuration["FileUploadPath:Path"], $"Users/{userId}"));
                    if (writtenPath is null)
                    {
                        return false;
                    }
                    else
                    {
                        drugData.ProductImage = writtenPath;
                    }
                }

                // Add the new drug to the context
                await _context.Drugs.AddAsync(drugData);
                await _context.SaveChangesAsync();

                // Save all drug regions
                foreach (var item in drugRequest.DrugRegionRequests)
                {
                    bool isDrugRegionSaved = await SaveDrugRegion(item, drugData.Id, userId);
                    if (!isDrugRegionSaved)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                // Commit the transaction after all operations
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> SaveDrugRegion(DrugRegionRequest drugRegionRequest, long id, long userId)
        {
            try
            {
                var drugRegionData = new DrugRegion()
                {
                    RegionId = drugRegionRequest.RegionId,
                    DrugType = drugRegionRequest.DrugType,
                    DrugId = id,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = userId
                };
                await _context.DrugRegions.AddAsync(drugRegionData);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<Drugs?> GetDrugById(long drugId)
        {
            var drug = await _context.Drugs.FindAsync(drugId);

            return drug;
        }
        public async Task<bool> UpdateDrugAsync(DrugRequest drugRequest, long drugId)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingDrug = await _context.Drugs.FindAsync(drugId);
                    existingDrug.DD = drugRequest.DD;
                    existingDrug.DPC = drugRequest.DPC;
                    existingDrug.InPharmacy = drugRequest.InPharmacy;
                    existingDrug.AIC = drugRequest.AIC.Trim();
                    existingDrug.ExtendedDescription = drugRequest.ExtendedDescription.Trim();
                    existingDrug.CompanyName = drugRequest.CompanyName.Trim();
                    existingDrug.Price = drugRequest.Price;
                    existingDrug.ProductType = drugRequest.ProductType.Trim();
                    existingDrug.Class = drugRequest.Class.Trim();
                    existingDrug.PharmaceuticalForm = drugRequest.PharmaceuticalForm.Trim();
                    existingDrug.UMR = drugRequest.UMR;
                    existingDrug.PrescriptionType = drugRequest.PrescriptionType.Trim();
                    existingDrug.ProductImage = drugRequest.ProductImage.Trim();
                    existingDrug.TherapeuticIndications = drugRequest.TherapeuticIndications.Trim();
                    existingDrug.Temperature = drugRequest.Temperature.Trim();
                    existingDrug.NumberGGAlert = drugRequest.NumberGGAlert;
                    existingDrug.AlertHours = drugRequest.AlertHours;
                    existingDrug.IsActive = drugRequest.IsActive;
                    existingDrug.UpdatedBy = 1;
                    existingDrug.UpdatedDate = DateTime.UtcNow;
                    _context.Drugs.Update(existingDrug);
                    await _context.SaveChangesAsync();
                    //foreach (var drugRegionRequest in drugRequest.DrugRegionRequests)
                    //{
                    //    if (drugRegionRequest.IsDeleted == true)
                    //    {
                    //        var existingDrugRegion = await _context.DrugRegions
                    //        .Where(a => a.DrugsRegionsId == drugRegionRequest.DrugsRegionsId
                    //                    && a.DrugId == drugId)
                    //        .FirstOrDefaultAsync();
                    //        if (existingDrugRegion != null)
                    //        {
                    //            _context.DrugRegions.Remove(existingDrugRegion);
                    //            await _context.SaveChangesAsync();
                    //        }
                    //    }
                    //    if ((bool)drugRegionRequest.IsNew)
                    //    {
                    //        await SaveDrugRegion(drugRegionRequest, drugId, 1);
                    //    }
                    //}
                    await _context.Database.CommitTransactionAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return false;
                }
            }
        }

        public async Task<DrugResponse> GetDrugByIdWithRegionsAsync(long drugId)
        {

            var drug = await _context.Drugs
                .Where(d => d.Id == drugId)
                .Select(d => new DrugResponse
                {
                    Id = d.Id,
                    DD = d.DD,
                    DPC = d.DPC,
                    InPharmacy = d.InPharmacy,
                    AIC = d.AIC,
                    ExtendedDescription = d.ExtendedDescription,
                    CompanyName = d.CompanyName,
                    Price = d.Price,
                    ProductType = d.ProductType,
                    Class = d.Class,
                    PharmaceuticalForm = d.PharmaceuticalForm,
                    UMR = d.UMR,
                    PrescriptionType = d.PrescriptionType,
                    ProductImage = d.ProductImage,
                    TherapeuticIndications = d.TherapeuticIndications,
                    Temperature = d.Temperature,
                    NumberGGAlert = d.NumberGGAlert,
                    AlertHours = d.AlertHours,
                    IsActive = d.IsActive,
                    DrugRegionResponse = _context.DrugRegions
                        .Where(dr => dr.DrugId == d.Id)
                        .Join(_context.States, dr => dr.RegionId, st => st.Id, (dr, st) => new DrugRegionResponse
                        {
                            DrugsRegionsId = dr.DrugsRegionsId,
                            RegionId = dr.RegionId,
                            RegionName = st.Name,
                            DrugType = dr.DrugType,
                            DrugId = dr.DrugId
                        }).ToList()
                })
                .FirstOrDefaultAsync();
            if (drug == null)
            {
                return null;
            }
            return drug;
        }


        //public async Task<PagedResponse<DrugResponse>> GetAllDrugs(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
        //{
        //    try
        //    {
        //        var drugRegions = await _context.DrugRegions.ToListAsync();
        //        var query = from drug in _context.Drugs
        //                    select new DrugResponse
        //                    {
        //                        Id = drug.Id,
        //                        DD = drug.DD,
        //                        DPC = drug.DPC,
        //                        InPharmacy = drug.InPharmacy,
        //                        AIC = drug.AIC,
        //                        ExtendedDescription = drug.ExtendedDescription,
        //                        CompanyName = drug.CompanyName,
        //                        Price = drug.Price,
        //                        ProductType = drug.ProductType,
        //                        Class = drug.Class,
        //                        PharmaceuticalForm = drug.PharmaceuticalForm,
        //                        UMR = drug.UMR,
        //                        PrescriptionType = drug.PrescriptionType,
        //                        ProductImage = drug.ProductImage,
        //                        TherapeuticIndications = drug.TherapeuticIndications,
        //                        Temperature = drug.Temperature,
        //                        NumberGGAlert = drug.NumberGGAlert,
        //                        AlertHours = drug.AlertHours,
        //                        IsActive = drug.IsActive,
        //                        DrugRegionResponse = (from dr in _context.DrugRegions
        //                                              join st in _context.States on dr.RegionId equals st.Id
        //                                              where dr.DrugId == drug.Id
        //                                              select new DrugRegionResponse
        //                                              {
        //                                                  DrugsRegionsId = dr.DrugsRegionsId,
        //                                                  RegionId = dr.RegionId,
        //                                                  RegionName = st.Name,
        //                                                  DrugType = dr.DrugType,
        //                                                  DrugId = dr.DrugId
        //                                              }).ToList()
        //                    };
        //        var count = (from drugs in _context.Drugs
        //                     select drugs).Count();
        //        PagedResponse<DrugResponse> pagedResponse = new PagedResponse<DrugResponse>();
        //        if (isAll)
        //        {
        //            var result = await query.Where(a => a.IsActive == true).ToListAsync();
        //            pagedResponse.Result = result;
        //            pagedResponse.TotalCount = result.Count;
        //            pagedResponse.TotalPages = 0;
        //            pagedResponse.CurrentPage = 0;
        //            return pagedResponse;
        //        }
        //        string orderByExpression = $"{sortColumn} {sortDirection}";
        //        if (filter != null && filter.Length > 0)
        //        {
        //            filter = filter.ToLower();
        //            query = query.Where(x => x.AIC.ToLower().Contains(filter) || x.ExtendedDescription.ToLower().Contains(filter));
        //            count = query.Count();
        //        }
        //        query = query.OrderBy(orderByExpression).Skip((pageNo - 1) * pageSize).Take(pageSize).AsQueryable();
        //        pagedResponse.Result = query;
        //        pagedResponse.TotalCount = count;
        //        pagedResponse.TotalPages = (int)Math.Ceiling((pagedResponse.TotalCount ?? 0) / (double)pageSize);
        //        pagedResponse.CurrentPage = pageNo;
        //        return pagedResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        public async Task<bool> DeleteHospitalAsync(Drugs drug)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    var drugRegions = await _context.DrugRegions.Where(a => a.DrugId == drug.Id).ToListAsync();
                    if (drugRegions.Any())
                    {
                        _context.DrugRegions.RemoveRange(drugRegions);
                        await _context.SaveChangesAsync();
                    }

                    _context.Drugs.Remove(drug);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public async Task<bool> IsDrugRegionExists(long? DrugsRegionsId)
        {
            return await _context.DrugRegions.AnyAsync(a => a.DrugsRegionsId == DrugsRegionsId);
        }

        public async Task<bool> IsAICExists(string aic, long? drugId = null)
        {
            return await _context.Drugs.AnyAsync(drug => drug.AIC == aic && (drugId == null || drug.Id != drugId));
        }
        public async Task<bool> IsRegionDuplicated(IEnumerable<DrugRegionRequest> drugRegionRequests)
        {
            // Extract region IDs to check
            var regionIds = drugRegionRequests.Select(a => a.RegionId).ToList();
            // Fetch DD regions from the database
            var ddRegions =  drugRegionRequests
                                          .Where(dr => dr.DrugType == "dd") 
                                          .Select(dr => dr.RegionId)
                                          .ToList();
            // Fetch DPC regions from the database
            var dpcRegions =  drugRegionRequests
                                           .Where(dr => dr.DrugType == "dpc").Select(dr => dr.RegionId)
                                           .ToList();
            // Check for intersections
            return ddRegions.Intersect(dpcRegions).Any();
        }

    }
}
