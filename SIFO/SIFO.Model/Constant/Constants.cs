namespace SIFO.Model.Constant
{
    public static class Constants
    { 
        public static string SUCCESS = "success";
        public static string CREATED = "successfully created";
        public static string NOT_FOUND = "not found";
        public static string BAD_REQUEST = "bad request";
        public static string INTERNAL_SERVER_ERROR = "something went wrong";
        public static string CONFLICT = "conflict"; 

        public static string FIRST_LOGIN = "conflict"; 
        public static string USED_TEMP_PASSWORD = "conflict"; 
        public static string EMAIL = "email"; 
        public static string SMS = "sms"; 
        public static string TWILIO_AUTHY = "twilio_authy";
        public static string EMAIL_REGEX = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string REQUIRED = "is required.";
        public const string INVALID_EMAIL_FORMAT = "Invalid email format.";
        public const string EMAIL_ALREADY_EXISTS = "Email already exists.";
        public const string PHONE_ALREADY_EXISTS = "Phone number already exists.";


    }
}
