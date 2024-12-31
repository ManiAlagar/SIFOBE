using Grpc.Net.Client;
using SIFO.APIService.Hospital; // Correct namespace for the generated classes

namespace SIFO.APIService.User
{
    public class GrpcClient
    {
        private IConfiguration _configuration;

        public GrpcClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<long> GetRetailPharmacy()
        {
            var baseUrl = _configuration["hospitalCluster:Destinations:destination1:Address"];

            // Connect to the gRPC server
            var channel = GrpcChannel.ForAddress(baseUrl);

            // Create the gRPC client
            var client = new PharmacyGrpcService.PharmacyGrpcServiceClient(channel); // or PharmacyGrpcServiceClient

            // Create the request object (empty in your case)
            var request = new GetRetailPharmacyRequest();

            // Make the gRPC call
            var response = await client.GetRetailPharmacyAsync(request);

            // Print the response (Pharmacy ID)
            return response.PharmacyId;
        }
    }
}