namespace SIFO.Model.Constant
{
    public static class Constants
    {
        public const string ROLE_SUPER_ADMIN = "Super Admin";
        public const string ROLE_QC_ADMINISTRATOR = "QC Administrator"; 
        public const string ROLE_HOSPITAL_PHARMACY_SUPERVISOR = "Hospital Pharmacy Supervisor"; 
        public const string ROLE_HOSPITAL_PHARMACY_OPERATOR = "Hospital Pharmacy Operator"; 
        //public const string ROLE_SUPER_ADMINISTRATOR = "Administrator / Hospital Referent"; 
        public const string ROLE_ADMINISTRATOR = "Administrator"; 
        public const string ROLE_HOSPITAL_REFERENT = "Administrator / Hospital Referent"; 
        public const string ROLE_DOCTOR = "Doctor"; 
        public const string ROLE_QC_OPERATOR = "QC Operator"; 
        public const string ROLE_RETAIL_PHARMACY_SUPERVISOR = "Retail Pharmacy Supervisor"; 
        public const string ROLE_RETAIL_PHARMACY_OPERATOR = "Retail Pharmacy Operator"; 


        public static string SUCCESS = "success";
        public static string CREATED = "successfully created";
        public static string NOT_FOUND = "not found";
        public static string BAD_REQUEST = "bad request";
        public static string INTERNAL_SERVER_ERROR = "something went wrong";
        public static string CONFLICT = "conflict"; 
        public static string DATA_DEPENDENCY_ERROR_MESSAGE = "cannot delete data due to data dependency"; 
        public static long DATA_DEPENDENCY_CODE = 1451; 

        public static string FIRST_LOGIN = "conflict"; 
        public static string USED_TEMP_PASSWORD = "conflict"; 
        public static string EMAIL = "email"; 
        public static string SMS = "sms"; 
        public static string TWILIO_AUTHY = "twilio_authy";
        public static string EMAIL_REGEX = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string INVALID_EMAIL_FORMAT = "Invalid email format.";
        public const string EMAIL_ALREADY_EXISTS = "Email already exists.";
        public const string PHONE_ALREADY_EXISTS = "Phone number already exists.";

        public static string CITY_ALREADY_EXISTS = "city already exists";
        public static string STATE_ALREADY_EXISTS = "state already exists";
        public static string COUNTRY_ALREADY_EXISTS = "country already exists";
        public static string CITY_NOT_FOUND = "city not found";
        public static string STATE_NOT_FOUND = "state not found";
        public static string COUNTRY_NOT_FOUND = "country not found";
        public static string HOSPITAL_NOT_FOUND = "hospital not found";
        public static string PHARMACY_NOT_FOUND = "pharmacy not found";
        public static string HOSPITAL_FACILITY_NOT_FOUND = "hospital facility not found";
        public static string ADDRESSDETAIL_NOT_FOUND = "address detail not found";
        public static string ADDRESSDETAIL_ALREADY_EXISTS = "address detail already exists"; 
        public static string USER_NOT_AUTHENTICATED = "user not authenticated"; 
        public static string INVALID_ROLE = "you are restricted to do any operations for this role"; 

    }
}
