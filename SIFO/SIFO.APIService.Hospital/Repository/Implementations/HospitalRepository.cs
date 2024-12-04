using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.Common.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;

namespace SIFO.APIService.Hospital.Repository.Implementations
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly SIFOContext _context;
        private readonly ICommonService _commonService;

        public HospitalRepository(SIFOContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        public async Task<bool> SaveHospitalAsync(HospitalRequest request)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var hospital = new Hospitals()
                    {
                        Name = request.HospitalName,
                        AddressId = 1,
                        ASL = request.ASL,
                        PhoneNumber = request.PhoneNumber,
                        Province = request.Province,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 1,
                        IsActive = true,
                        CAB= request.CAB,
                    };

                    var addedData = await _context.Hospitals.AddAsync(hospital);
                    await _context.SaveChangesAsync();
                    var hospitalId = hospital.Id;


                    foreach (var contact in request.Contact)
                    {
                        var contactCreated = await SaveContact(contact, hospitalId);
                        if (!contactCreated)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            return false;
                        }
                    }


                    foreach (var pharmacy in request.Pharmacy)
                    {
                        var pharmacyCreated = await SavePharmacy(pharmacy, hospitalId);
                        if (!pharmacyCreated)
                        {
                            await _context.Database.RollbackTransactionAsync();
                            return false;
                        }
                    }

                    await _context.Database.CommitTransactionAsync();
                    return true;
                }
                catch (Exception e)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return false;
                }
            }
        }


        public async Task<bool> CheckIfEmailOrPhoneExists(string phoneNumber, long userId)
        {
            return await _context.Hospitals.AnyAsync(a => a.PhoneNumber == phoneNumber);

        }

        public async Task<bool> SaveContact(ContactRequest contact, long hospitalId)
        {
            try
            {
                //var data= await _commonService.GetDataFromToken();
                var newContact = new Contact()
                {
                    ContactName=contact.ContactName,
                    ContactSurname = contact.ContactSurname,
                    PhoneNumber = contact.PhoneNumber,
                    Role = contact.Role,
                    HospitalId = hospitalId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1,
                };
                await _context.Contacts.AddAsync(newContact);
                _context.SaveChanges();

                return true;

            }
            catch 
            {

                return false;
            }


        }
        public async Task<bool> SavePharmacy(PharmacyRequest pharmacy, long hospitalId)
        {

            try
            {
                var newPharmacy = new Pharmacy()
                {
                    PharmacyName = pharmacy.PharmacyName,
                    HospitalId = hospitalId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1
                };

                await _context.Pharmacies.AddAsync(newPharmacy);
                await _context.SaveChangesAsync();


                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

        public async Task<bool> UpdateHospitalAsync(HospitalRequest request, long id)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var hospitalData = await _context.Hospitals.Where(a => a.Id == id).FirstOrDefaultAsync();

                    if (hospitalData != null)
                    {

                        hospitalData.Name = request.HospitalName;
                        hospitalData.ASL = request.ASL;
                        hospitalData.PhoneNumber = request.PhoneNumber;
                        hospitalData.Province = request.Province;
                        hospitalData.UpdatedDate = DateTime.UtcNow;
                        hospitalData.UpdatedBy = 1;
                        hospitalData.IsActive = request.IsActive;

                        _context.Hospitals.Update(hospitalData);
                        await _context.SaveChangesAsync();
                        if (request.IsActive)
                        {
                            foreach (var contact in request.Contact)
                            {
                                if (contact.IsDeleted == true)
                                {
                                    var contactData = await _context.Contacts.Where(a => a.Id == contact.Id
                                      && a.HospitalId == id).FirstOrDefaultAsync();

                                    if (contactData != null)
                                        _context.Contacts.Remove(contactData);
                                }

                                if (contact.IsNew == true)
                                {
                                    var contactCreated = await SaveContact(contact, id);
                                }
                            }


                            foreach (var pharmacy in request.Pharmacy)
                            {
                                if (pharmacy.IsDeleted == true)
                                {
                                    var pharmacyData = await _context.Pharmacies.Where(a => a.Id == pharmacy.Id
                                    && a.HospitalId == id).FirstOrDefaultAsync();

                                    if (pharmacyData != null)
                                        _context.Pharmacies.Remove(pharmacyData);

                                }

                                if (pharmacy.IsNew == true)
                                {
                                    var pharmacyCreated = await SavePharmacy(pharmacy, id);
                                }
                            }
                            await _context.SaveChangesAsync();
                            await _context.Database.CommitTransactionAsync();
                        }
                        return true;


                    }
                    else
                    {
                        await _context.Database.RollbackTransactionAsync();
                        return false;
                    }
                }
                catch (Exception e)
                {
                    await _context.Database.RollbackTransactionAsync();
                    return false;
                }
            }
        }
       



    }
}
