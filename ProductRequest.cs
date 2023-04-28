public static partial class ProductRepository {
    //criar uma classe para representar o payload (ProductDto)
    public record ProductRequest(string Code, string Name, string Description, int CategoryId, List<string> Tags);
}
