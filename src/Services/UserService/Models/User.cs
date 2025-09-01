namespace UserService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string ClinicianId { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Role { get; set; } = "Clinician";
    }
}
