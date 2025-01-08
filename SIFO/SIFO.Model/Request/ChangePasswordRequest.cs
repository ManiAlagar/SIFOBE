namespace SIFO.Model.Request
{
    public class ChangePasswordRequest
    {
        public long UserId { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
    }
}
