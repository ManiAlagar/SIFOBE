using Grpc.Core;
using SIFO.APIService.Hospital;
using SIFO.APIService.Hospital.Repository.Contracts;

public class PharmacyServiceImpl : PharmacyGrpcService.PharmacyGrpcServiceBase
{
    private readonly IPharmacyRepository _pharmacyRepository;

    // Inject the repository into the gRPC service
    public PharmacyServiceImpl(IPharmacyRepository pharmacyRepository)
    {
        _pharmacyRepository = pharmacyRepository;
    }

    public override async Task<GetRetailPharmacyResponse> GetRetailPharmacy(GetRetailPharmacyRequest request, ServerCallContext context)
    {
        try
        {
            // Call the repository method to get the retail pharmacy ID
            var pharmacyId = await _pharmacyRepository.GetRetailPharmacyAsync();

            // Return the response with the pharmacy ID
            return new GetRetailPharmacyResponse
            {
                PharmacyId = pharmacyId
            };
        }
        catch (Exception ex)
        {
            // Log the error (if needed)
            // Optionally throw an RPC exception with a custom message or status code
            throw new RpcException(new Status(StatusCode.Internal, "An error occurred while fetching the retail pharmacy"));
        }
    }
}