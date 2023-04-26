using IWantApp.Domain.Products;

namespace IWantApp.Endpoints.Products
{
    public record ProductResponse(string Name, Category Category, string Description, bool HasStock, bool Active);
    
}
