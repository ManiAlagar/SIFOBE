using Grpc.Net.Client;
using SIFO.APIService.Hospital; // Correct namespace for the generated classes

namespace SIFO.APIService.User
{
    public class GrpcClient
    {
        private IConfiguration configuration;

        public GrpcClient(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<long> GetRetailPharmacy()
        {
            // Connect to the gRPC server
            var channel = GrpcChannel.ForAddress("https://localhost:7044");

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