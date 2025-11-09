namespace GenericRepository.Base_Entity
{
    // Example entity implementations
    public class User : BaseEntity<Guid>
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public static Guid GenerateNewKey() => Guid.NewGuid();
    }
}
