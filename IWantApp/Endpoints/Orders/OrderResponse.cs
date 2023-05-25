namespace IWantApp.Endpoints.Orders
{
    public record OrderResponse(Guid Id, string Email, string DeliveryAddress, IEnumerable<OrderProduct> OrderProducts);

    public record OrderProduct(Guid Id, string Name);
}