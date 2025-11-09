namespace GenericRepository.Base_Entity
{
    // Base entity interface with static abstract members
    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }

        // Static abstract member for key generation
        static abstract TKey GenerateNewKey();
    }
    // Base entity implementation
    public abstract class BaseEntity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public required TKey Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public static TKey GenerateNewKey() => default(TKey)!;
    }
}
