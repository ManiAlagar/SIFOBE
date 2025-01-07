﻿using AutoMapper;
using SIFO.Model.Entity;
using SIFO.Model.Request;

namespace SIFO.Model.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CountryRequest, Country>();
            CreateMap<StateRequest, State>();
            CreateMap<CityRequest, City>();
            CreateMap<AddressDetailRequest, AddressDetail>();
            CreateMap<UserRequest, Users>();
            CreateMap<HospitalRequest, Hospital>();
            CreateMap<CalendarRequest, Calendar>();
            CreateMap<IntoleranceManagementRequest, IntoleranceManagement>();
            CreateMap<AllergyRequest, Allergy>();
            CreateMap<WeeklyMoodEntryRequest, WeeklyMoodEntry>();
            CreateMap<PatientAnalysisReportRequest, PatientAnalysisReport>();
            CreateMap<AdverseEventRequest, AdverseEvent>();
            CreateMap<PatientRequest, Patients>();
        }
    }
}
