using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Utility.Implementations;

namespace SIFO.APIService.Hospital.Service.Implementations
{
    public class DrugService
    {
        private readonly IDrugRepository _drugRepository;
        private readonly IConfiguration _configuration;
        public DrugService(IDrugRepository drugRepository, IConfiguration configuration)
        {
            _drugRepository = drugRepository;
        }
        public async Task<ApiResponse<string>> CreateDrugAsync(DrugRequest drugRequest)
        {
            try
            {
                bool IsAICExists = await _drugRepository.IsAICExists(drugRequest.AIC);
                if (IsAICExists)
                {
                    return ApiResponse<string>.BadRequest("AIC value already exists. Please use a unique AIC value.!");
                }

                if (drugRequest.DD && !drugRequest.DrugRegionRequests.Any(dr => dr.DrugType.ToLower() == "dd"))
                {
                    return ApiResponse<string>.BadRequest("At least one DD region is required when DD is true.");
                }
                if (drugRequest.DPC && !drugRequest.DrugRegionRequests.Any(dr => dr.DrugType.ToLower() == "dpc"))
                {
                    return ApiResponse<string>.BadRequest("At least one DPC region is required when DPC is true.");
                }
                bool isDrugSaved = await _drugRepository.SaveDrugAsync(drugRequest);
                if (isDrugSaved)
                {
                    return ApiResponse<string>.Success("Drug Created Successfully!");
                }

                return ApiResponse<string>.InternalServerError("Something went wrong while creating the drug");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError(ex.Message);
            }
        }
        public async Task<ApiResponse<string>> UpdateDrugAsync(DrugRequest drugRequest, long drugId)
        {
            try
            {
                var drugResponse = await _drugRepository.GetDrugById(drugId);
                if (drugResponse == null)
                {
                    return ApiResponse<string>.NotFound("Drug Id not exists");
                }
                bool IsAICExists = await _drugRepository.IsAICExists(drugRequest.AIC, drugId);
                if (IsAICExists)
                {
                    return ApiResponse<string>.BadRequest("AIC value already exists. Please use a unique AIC value.!");
                }
                foreach (var drug in drugRequest.DrugRegionRequests)
                {
                    if (drug.IsNew == false)
                    {
                        bool drugRegionExists = await _drugRepository.IsDrugRegionExists(drug.DrugsRegionsId);
                        if (!drugRegionExists)
                            return ApiResponse<string>.NotFound("Drug Region not exists");

                    }
                    bool regionExists = await _drugRepository.IsRegionExists(drug.RegionId);
                    if (regionExists)
                        return ApiResponse<string>.NotFound("Region already exists");
                }

                if (drugRequest.DD && !drugRequest.DrugRegionRequests.Any(dr => dr.DrugType == "DD"))
                {
                    return ApiResponse<string>.BadRequest("At least one DD region is required when DD is true.");
                }
                if (drugRequest.DPC && !drugRequest.DrugRegionRequests.Any(dr => dr.DrugType == "DPC"))
                {
                    return ApiResponse<string>.BadRequest("At least one DPC region is required when DPC is true.");
                }
                bool isDrugUpdated = await _drugRepository.UpdateDrugAsync(drugRequest, drugResponse.Id);
                if (isDrugUpdated)
                    return ApiResponse<string>.Success("Drug Updated Successfully!");
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError($"{ex.Message}");
            }
        }
        public async Task<ApiResponse<DrugResponse>> GetDrugByIdWithRegionsAsync(long id)
        {
            if (id <= 0)
                return ApiResponse<DrugResponse>.BadRequest();
            var response = await _drugRepository.GetDrugByIdWithRegionsAsync(id);
            if (response != null)
                return ApiResponse<DrugResponse>.Success(Constants.SUCCESS, response);
            return ApiResponse<DrugResponse>.NotFound("Id not found");
        }
        //public async Task<ApiResponse<PagedResponse<DrugResponse>>> GetAllDrugs(int pageNo = 1, int pageSize = 10, string filter = "", string sortColumn = "Id", string sortDirection = "DESC", bool isAll = false)
        //{
        //    var isValid = await HelperService.ValidateGet(pageNo, pageSize, filter, sortColumn, sortDirection);
        //    if (isValid.Any())
        //        return ApiResponse<PagedResponse<DrugResponse>>.BadRequest(isValid[0]);
        //    var response = await _drugRepository.GetAllDrugs(pageNo, pageSize, filter, sortColumn, sortDirection, isAll);
        //    return ApiResponse<PagedResponse<DrugResponse>>.Success(Constants.SUCCESS, response);
        //}
        public async Task<ApiResponse<string>> DeleteHospitalAsync(long id)
        {
            var drug = await _drugRepository.GetDrugById(id);
            if (drug == null)
            {
                return ApiResponse<string>.InternalServerError("Drug id does not exist");
            }
            bool isDrugDeleted = await _drugRepository.DeleteHospitalAsync(drug);
            if (!isDrugDeleted)
            {
                return ApiResponse<string>.InternalServerError(Constants.INTERNAL_SERVER_ERROR);
            }
            return ApiResponse<string>.Success("Drug deleted Successfully!!");
        }

    }
}
