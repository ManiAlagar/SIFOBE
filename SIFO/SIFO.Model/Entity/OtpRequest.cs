using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIFO.Model.Entity
{
    public class OtpRequest
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("userid")]
        public long UserId { get; set; }

        [Column("otpcode")]
        public string? OtpCode { get; set; }

        [Column("expirationdate")]
        public DateTime? ExpirationDate { get; set; }
        [Column("verifieddate")]
        public DateTime? VerifiedDate { get; set; }
        [Column("createddate")]
        public DateTime CreatedDate { get; set; }
        [Column("createdby")]
        public long CreatedBy { get; set; }
        [Column("updateddate")]
        public DateTime? UpdatedDate { get; set; }
        [Column("updatedby")]
        public long? UpdatedBy { get; set; }
        [Column("authenticatedtype")]
        public long? AuthenticationType { get; set; }
        [Column("authenticatedfor")]
        public string AuthenticationFor { get; set; }
    }
}
