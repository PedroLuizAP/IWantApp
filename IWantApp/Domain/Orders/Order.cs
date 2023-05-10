using Flunt.Validations;
using IWantApp.Domain.Products;

namespace IWantApp.Domain.Orders
{
    public class Order : Entity
    {
        public string ClientId { get; private set; }
        public List<Product> Products { get; private set; }
        public decimal Total { get; private set; }
        public string DeliveryAddress { get; private set; }
        public Order() { }

        //create contructor
        public Order(string clientId, string clientName, List<Product> products, string deliveryAddress)
        {
            ClientId = clientId;
            Products = products;
            DeliveryAddress = deliveryAddress;
            CreatedBy = clientName;
            EditedBy = clientName;
            CreatedOn = DateTime.Now;
            EditedOn = DateTime.Now;

            Total = 0;
            foreach (var product in products)
            {
                Total += product.Price;
            }

            Validate();
        }

        private void Validate()
        {
            var contract = new Contract<Order>()
                .IsNotNull(ClientId, "Client")
                .IsNotNull(Products, "Products");

            AddNotifications(contract);
        }
    }
}
