using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Master.Repository.Contracts;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Repository.Implementations
{
    public class MasterRepository : IMasterRepository
    {
        private readonly SIFOContext _context;

        public MasterRepository(SIFOContext context)
        {
            _context = context;
        }

        public async Task<Users> IsUserExists(long userId)
        {
            try
            {
                var userData = await _context.Users.Where(a => a.Id == userId).SingleOrDefaultAsync();
                return userData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> ImportLableAsync(List<Labels> labels)
        {
            using (var context = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var label in labels)
                    {
                        if (!string.IsNullOrEmpty(label.Label))
                        {
                            await _context.Label.AddAsync(label);
                            await _context.SaveChangesAsync();
                        }
                    }
                    await _context.Database.CommitTransactionAsync();
                    return Constants.SUCCESS;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<LabelResponse> GetLabelsAsync()
        {
            try
            {
                var englishLabels = await _context.Label.Where(l => l.Language.ToLower() == "en").ToDictionaryAsync(l => l.FkVar, l => l.Label);
                var italianLabels = await _context.Label.Where(l => l.Language.ToLower() == "it").ToDictionaryAsync(l => l.FkVar, l => l.Label);

                return new LabelResponse
                {
                    en = new LanguageData { labels = englishLabels },
                    it = new LanguageData { labels = italianLabels }
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PharmacyTypeResponse>> GetAllPharmacyTypesAsync()
        {
            try
            {
                var response = await _context.PharmacyTypes.Select(pt => new PharmacyTypeResponse
                {
                    Id = pt.Id,
                    Name = pt.Name,
                    Description = pt.Description,
                }).ToListAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<AuthenticationTypeResponse>> GetAllAuthenticationTypesAsync()
        {
            try
            {
                var response = await _context.AuthenticationType.Select(pt => new AuthenticationTypeResponse
                {
                    Id = pt.Id,
                    AuthenticationType = pt.AuthType
                }).ToListAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
