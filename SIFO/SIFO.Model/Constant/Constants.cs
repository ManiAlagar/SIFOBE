using Microsoft.AspNetCore.Http.HttpResults;
using SIFO.Model.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SIFO.Model.Constant
{
    public static class Constants
    {
        #region Roles
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
        public const string ROLE_PATIENT = "Patient";  
        #endregion

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
        public static string INVALID_OTP = "invalid OTP";
        public static string MINISTERIAL_ID_EXISTS = "ministerial id already exists";
        public static string PHARMACY_ID_NOT_EXISTS = "pharmacy id does not exists.";
        public static string ALLERGY_ALREADY_EXISTS = "allergy already exists";
        public static string ALLERGY_NOT_FOUND = "allergy not found";
        public static string INTOLERANCE_MANAGEMENT_ALREADY_EXISTS = "intolerance management already exists";
        public static string INTOLERANCE_MANAGEMENT_NOT_FOUND = "intolerance management not found";
        public static string UPDATED_SUCCESSFULLY = "updated successfully";
        public static string MAIL_OR_PASSWORD_CANNOT_BE_EMPTY = "Mail or password cannot be empty";
        public static string INVALID_EMAIL_OR_PASSWORD = "Invalid email and/or password";
        public static string SOMETHING_WENT_WRONG_WHILE_SENDING_MAIL = "Something went wrong while sending the mail";
        public static string USER_NOT_FOUND = "User not found";
        public static string INVALID_OLD_PASSWORD = "Invalid old password";
        public static string PHARMACY_ID_DOES_NOT_EXIST = "Pharmacy ID does not exist.";
        public static string ONLY_HOSPITAL_PHARMACIES_CAN_BE_CREATED = "Only hospital pharmacies can be created. Please provide a valid pharmacy ID.";
        public static string HOSPITAL_FACILITY_CONTACT_PHARMACY_CREATED_SUCCESSFULLY = "Hospital facility, contact, and pharmacy created successfully!";
        public static string PHARMACY_AND_CONTACT_CREATED_SUCCESSFULLY = "Pharmacy and contact created successfully!";
        public static string ACCESS_DENIED = "Access denied.";
        public static string APPROVED = "Approved";
        public static string OTP_EXPIRED = "OTP expired";
        public static string PROFILE_IMG_IS_INVALID = "Profile image is invalid";
        public static string ACTIVATED = "Activated";
        public static string DEACTIVATED = "Deactivated";
        public static string ACCOUNT_ACTIVATION = "Account Activation";
        public static string ACCOUNT_DEACTIVATION = "Account Deactivation";
        public static string ADDRESS_NAME_REQUIRED = "address name is required";
        public static string NAME_REQUIRED = "name is required";
        public static string IS_ACTIVE_REQUIRED = "is active is required";
        public static string PHARMACY_ID_REQUIRED = "pharmacy id is required";
        public static string CALENDAR_DATE_GREATER_THAN_TODAY = "calendar date must be greater than today";
        public static string COUNTRY_NAME_REQUIRED = "Country name is required";
        public static string NAME_TOO_LONG = "Name cannot exceed 100 characters";
        public static string COUNTRY_CODE_TOO_LONG = "Country code cannot exceed 2 characters";
        public static string STATE_CODE_TOO_LONG = "State code cannot exceed 2 characters";
        public static string ISO3_CODE_TOO_LONG = "ISO-3 code cannot exceed 3 characters";
        public static string ISO2_CODE_TOO_LONG = "ISO-2 code cannot exceed 2 characters";
        public static string PHONE_CODE_TOO_LONG = "Phone code cannot exceed 255 characters";
        public static string HOSPITAL_FACILITY_NAME_REQUIRED = "Hospital facility name is required";
        public static string PROVINCE_REQUIRED = "Province is required";
        public static string PROVINCE_TOO_LONG = "Province cannot exceed more than 2 characters";
        public static string ADDRESS_REQUIRED = "Address is required";
        public static string CITY_REQUIRED = "City is required";
        public static string CAP_CODE_REQUIRED = "CAP code is required";
        public static string PHONE_NUMBER_REQUIRED = "Phone number is required";
        public static string FIRST_NAME_REQUIRED = "First name is required";
        public static string LAST_NAME_REQUIRED = "Last name is required";
        public static string ROLE_REQUIRED = "Role is required";
        public static string PHONE_NUMBER_TOO_LONG = "Phone number cannot exceed more than 15 characters";
        public static string HOSPITAL_NAME_REQUIRED = "hospital name is required";
        public static string PHARMACY_NAME_REQUIRED = "pharmacy name is required";
        public static string CAP_CODE_TOO_LONG = "cap code cannot exceed more than 6 characters";
        public static string PHARMACY_TYPE_REQUIRED = "pharmacy type is required";
        public static string ASL_REQUIRED = "asl is required";
        public static string EMAIL_REQUIRED = "email is required";
        public static string USER_PHONE_NUMBER_TOO_LONG = "phone number should not exceed 10 characters";
        public static string AUTHENTICATION_TYPE_REQUIRED = "authentication type is required";
        public static string COUNTRY_CODE_REQUIRED = "country code is required";
        public static string FISCAL_CODE_REQUIRED = "fiscal code should not be empty";
        public static string FISCAL_CODE_TOO_LONG = "fiscal code should not exceed 16 characters";
        public static string PHARMACY_ID_LIST_REQUIRED = "pharmacy ID list is required";
        public static string EACH_PHARMACY_ID_REQUIRED = "each pharmacy ID is required";
        public static string HOSPITAL_ID_LIST_REQUIRED = "hospital ID list is required";
        public static string EACH_HOSPITAL_ID_REQUIRED = "each hospital ID is required";
        public static string PASSWORD_REQUIRED = "password is required";
        public static string INVALID_PAGE_NUMBER = "Page number must be greater than 0";
        public static string INVALID_PAGE_SIZE = "Page size must be greater than 0";
        public static string INVALID_FILTER_LENGTH = "Filter length cannot exceed 255 characters";
        public static string INVALID_SORT_COLUMN = "Invalid sort column";
        public static string INVALID_SORT_DIRECTION = "Invalid sort direction. It should be 'asc' or 'desc'";
        public static string PATIENT_NOT_EXISTS = "Patient does not exists";
        public static string USER_ALREADY_VERIFIED = "user is already verified";
        public static string FORBIDDEN = "access denied";
        public static string EMAIL_PASSWORD_NOT_EMPTY = "Email or password cannot be empty";
        public static string ERROR_SAVING_PHARMACY_DETAILS = "Error while saving Pharmacy Details";
        public static string ERROR_SAVING_HOSPITAL_DETAILS = "Error while saving Hospital Details";

        public enum PharmacyTypes
        {
            retail,
            hospital
        }

    }
}
