namespace GenericRepository.Base_Entity
{
    public class Product : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public static Guid GenerateNewKey() => 0; // Let database generate
    }
}
