namespace IWantApp.Endpoints.Orders
{
public record OrderRequest (List<Guid> Products, string DeliveryAddress);
}