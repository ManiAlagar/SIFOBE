﻿using SIFO.Model.Request;
using SIFO.Model.Response;

namespace SIFO.APIService.Master.Service.Contracts
{
    public interface IMasterService
    {
        public Task<ApiResponse<string>> SendOtpRequestAsync(SendOtpRequest request);

    }
}