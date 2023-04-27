using Flunt.Validations;

namespace IWantApp.Domain.Products
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasStock { get; set; }
        public Category Category { get; set; }
        public long CategoryId { get; set; }
        public bool Active { get; set; } = true;
        public decimal Price { get; private set; }

        public Product() { }
        public Product(string name, Category category, string description, bool hasStock, string createdBy, decimal price)
        {
            Name = name;
            Category = category;
            Description = description;
            HasStock = hasStock;
            Price = price;
            CreatedBy = createdBy;
            EditedBy = createdBy;

            CreatedOn = DateTime.Now;
            EditedOn = DateTime.Now;

            Validate();
        }

        private void Validate()
        {
            var contract = new Contract<Product>()
                .IsNotNullOrEmpty(Name, "Name")
                .IsGreaterOrEqualsThan(Name, 3, "Name")
                .IsNotNullOrEmpty(Description, "Description")
                .IsNotNull(Category, "Category", "Category not Found")
                .IsGreaterOrEqualsThan(Description, 3, "Description")
                .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
                .IsNotNullOrEmpty(EditedBy, "EditedBy")
                .IsGreaterOrEqualsThan(Price, 1, "Price");

            AddNotifications(contract);
        }
    }
}
