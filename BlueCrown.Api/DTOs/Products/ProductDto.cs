namespace BlueCrown.Api.DTOs.Products
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? StockQuantity { get; set; }
        public bool? IsPrescriptionRequired { get; set; }
        public string? ActiveIngredient { get; set; }
        public string? TherapeuticGroup { get; set; }
        public string? DosageForm { get; set; }
        public string? Strength { get; set; }
    }
}
