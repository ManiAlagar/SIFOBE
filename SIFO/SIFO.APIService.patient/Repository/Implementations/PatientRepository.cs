using SIFO.APIService.Patient.Repository.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using SIFO.Model.Constant;
using SIFO.Model.Request;
using Twilio.Http;

namespace SIFO.APIService.Patient.Repository.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        private readonly SIFOContext _context;
        public PatientRepository(SIFOContext context)
        {
            _context = context;
        }
        public async Task<PagedResponse<PatientResponse>> GetPatientAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll, string roleName)
        {
            try
            {
                var query = from patient in _context.Patients
                            join addressDetails in _context.AddressDetails on patient.AddressId equals addressDetails.Id
                            join country in _context.Countries on addressDetails.CountryId equals country.Id
                            join state in _context.States on addressDetails.Region equals state.Id
                            join city in _context.Cities on addressDetails.CityId equals city.Id
                            select new PatientResponse
                            {
                                Id = patient.Id.Value,
                                Code = patient.Code,
                                LastName = patient.LastName,
                                FirstName = patient.FirstName,
                                FiscalCode = patient.FiscalCode,
                                Email = patient.Email,
                                Phone = patient.Phone,
                                DeliveryMethod = patient.DeliveryMethod,
                                DeliveryPharmacyId = patient.DeliveryPharmacyId,
                                DeliveryPharmacyName = "", //need to change this 
                                AddressId = patient.AddressId,
                                Address = addressDetails.Address,
                                CityId = addressDetails.CityId,
                                City = city.Name,
                                StateId = addressDetails.Region,
                                State = state.Name,
                                CountryId = addressDetails.CountryId,
                                Country = country.Name,
                                NotificationModeId = patient.NotificationModeId.Value,
                                SmsReminder = patient.SmsReminder.Value,
                                ReminderApp = patient.ReminderApp.Value,
                                ConsentPersonalData = patient.ConsentPersonalData.Value,
                                ConsentSensitiveData = patient.ConsentSensitiveData.Value,
                                ConsentDataProfiling = patient.ConsentDataProfiling,
                                ConsentThirdPartyMarketing = patient.ConsentThirdPartyMarketing,
                            };

                var count = _context.Patients.Count();
                PagedResponse<PatientResponse> pagedResponse = new PagedResponse<PatientResponse>();
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
                    query = query.Where(x => x.FirstName.ToLower().Contains(filter));
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
                throw new Exception(ex.Message);
            }
        }

        public async Task<PatientResponse> GetPatientByIdAsync(long patientId)
        {
            try
            {
                var response = from patient in _context.Patients
                               join addressDetails in _context.AddressDetails on patient.AddressId equals addressDetails.Id
                               join country in _context.Countries on addressDetails.CountryId equals country.Id
                               join state in _context.States on addressDetails.Region equals state.Id
                               join city in _context.Cities on addressDetails.CityId equals city.Id 
                               where patient.Id == patientId
                               select new PatientResponse
                               {
                                   Id = patient.Id.Value,
                                   Code = patient.Code,
                                   LastName = patient.LastName,
                                   FirstName = patient.FirstName,
                                   FiscalCode = patient.FiscalCode,
                                   Email = patient.Email,
                                   Phone = patient.Phone,
                                   DeliveryMethod = patient.DeliveryMethod,
                                   DeliveryPharmacyId = patient.DeliveryPharmacyId,
                                   DeliveryPharmacyName = "", //need to change this 
                                   AddressId = patient.AddressId,
                                   Address = addressDetails.Address,
                                   CityId = addressDetails.CityId,
                                   City = city.Name,
                                   StateId = addressDetails.Region,
                                   State = state.Name,
                                   CountryId = addressDetails.CountryId,
                                   Country = country.Name,
                                   NotificationModeId = patient.NotificationModeId.Value,
                                   SmsReminder = patient.SmsReminder.Value,
                                   ReminderApp = patient.ReminderApp.Value,
                                   ConsentPersonalData = patient.ConsentPersonalData.Value,
                                   ConsentSensitiveData = patient.ConsentSensitiveData.Value,
                                   ConsentDataProfiling = patient.ConsentDataProfiling,
                                   ConsentThirdPartyMarketing = patient.ConsentThirdPartyMarketing,
                               };
                return await response.SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> CreatePatientAsync(Patients entity)
        {
            try
            {
                var result = await _context.Patients.AddAsync(entity);
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> UpdatePatientAsync(Patients entity)
        {
            try
            { 
                var result = await _context.Patients.AsNoTracking().Where(a=>a.Id == entity.Id).SingleOrDefaultAsync();
                if (result != null)
                {
                    entity.CreatedBy = result.CreatedBy;
                    entity.CreatedDate = result.CreatedDate;
                }
                _context.Patients.Update(entity); 
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> DeletePatientByIdAsync(long patientId)
        {
            try
            {
                var patientResponse = await _context.Patients.Where(x => x.Id == patientId).SingleOrDefaultAsync();
                if (patientResponse is not null)
                {
                    _context.Patients.Remove(patientResponse);
                    await _context.SaveChangesAsync();
                    return Constants.SUCCESS;
                }
                return Constants.NOT_FOUND;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> PhoneNumberOrEmailExists(string phoneNumber, string email, long patientId)
        {
            try
            {
                var response = new object();
                if (patientId > 0)
                     response =  await _context.Patients.Where(c => (c.Phone.ToLower() == phoneNumber.Trim().ToLower()  || c.Email.ToLower() == email.Trim().ToLower()) && c.Id != patientId).SingleOrDefaultAsync();
                else
                     response = await _context.Patients.Where(c => c.Phone.ToLower() == phoneNumber.Trim().ToLower() || c.Email.ToLower() == email.Trim().ToLower()).SingleOrDefaultAsync();
                if (response is not null)
                    return Constants.SUCCESS;
                return Constants.NOT_FOUND;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 

        public async Task<string> GetPatientByPhoneNumber(string phoneNumber)
        {
            try
            {
                var response = await _context.Patients.Where(a => a.Phone == phoneNumber).SingleOrDefaultAsync(); 
                if(response is not null) 
                    return Constants.SUCCESS; 
                return Constants.NOT_FOUND;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> AssistedCodeExistsAsync(string assistedCode)
        {
            try
            {
                var response = await _context.Patients.Where(a => a.Code == assistedCode).SingleOrDefaultAsync();
                return response != null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Patients> RegisterPatientAsync(Patients entity)
        {
            try
            {
                var response = _context.Patients.Add(entity);
                await _context.SaveChangesAsync(); 
                return response.Entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<long> GetAuthIdByTypeAsync(string authType)
        {
            try
            {
                var response = await _context.AuthenticationType.Where(a => a.AuthType.ToLower() == authType.ToLower()).Select(a=>a.Id).SingleOrDefaultAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OtpRequest> VerifyPatientAsync(VerifyPatientRequest request)
        {
            try
            {
                var patientId = await _context.Patients.Where(a => a.Code == request.PatientCode).Select(a=>a.Id).SingleOrDefaultAsync();
                if (patientId is null)
                    return null;

                var otpData = await _context.OtpRequests.OrderByDescending(a => a.Id).Where(a => a.OtpCode == request.OtpCode
                && a.AuthenticationFor.ToLower() == request.AuthenticationFor.ToLower() && a.AuthenticationType == request.AuthenticationType
                && a.UserId == patientId
                && a.ExpirationDate > DateTime.UtcNow && a.isVerified == null).FirstOrDefaultAsync();

                return otpData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> UpdateOtpDataAsync(OtpRequest request)
        {
            try
            {
                request.isVerified = true;
                _context.OtpRequests.Update(request); 
                await _context.SaveChangesAsync(); 
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CreatePasswordRequest(CreatePasswordRequest request)
        {
            try
            {
                try
                {
                    var patient = await _context.Patients.Where(a => a.Code == request.Code && a.IsActive == true).FirstOrDefaultAsync();

                    
                    if (patient == null)
                        return false;

                    patient.Password = request.Password;
                    _context.Patients.Update(patient);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
            catch (Exception)
            {
                          await transaction.RollbackAsync();
                return false;
            }
        }
        }

        public async Task<Patients> GetPatientByCodeAsync(string patientCode)
        {
            try
            {
                var patient = await _context.Patients.Where(a => a.Code == patientCode).SingleOrDefaultAsync();
                return patient;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> UpdatePatientStatus(string patientCode)
        {
            try
            {
                var response = await _context.Patients.Where(a => a.Code == patientCode).SingleOrDefaultAsync();
                response.IsActive = true;
                await _context.SaveChangesAsync();
                return Constants.SUCCESS;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Patients> CheckPatientExists(string userId)
        {
            try
            {
                var patient = await _context.Patients.Where(a => a.Id == Convert.ToInt64(userId)).FirstOrDefaultAsync();
                 if(patient != null)
                {
                    return patient;
                }

                return patient;
            }
          catch
            {
                throw;
            }
           
        }
        public async Task<bool> UpdatePasswordAsync(long? userId, string hashedPassword)
        {
            using (await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userData = await _context.Patients.Where(a => a.Id == userId).SingleOrDefaultAsync();
                    if (userData != null)
                    {
                        userData.Password = hashedPassword;
                        userData.UpdatedDate = DateTime.UtcNow;
                        userData.UpdatedBy = userId;
                        userData.PswdUpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        await _context.Database.CommitTransactionAsync();
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
}
