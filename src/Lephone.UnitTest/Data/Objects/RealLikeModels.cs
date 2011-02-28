using System.Collections.Generic;
using Lephone.Data.Definition;

namespace Lephone.UnitTest.Data.Objects
{
    public class Company : DbObjectModel<Company>
    {
        public string Name { get; set; }

        [HasMany]
        public IList<User> Users { get; set; }

        [HasMany]
        public IList<Department> Departments { get; set; }

        [HasMany]
        public IList<Employee> Employees { get; set; }

        [HasMany]
        public IList<Resource> Resources { get; set; }

        [HasMany]
        public IList<Trade> Trades { get; set; }

        [HasMany]
        public IList<Customer> Customers { get; set; }

        [HasMany]
        public IList<Product> Products { get; set; }
    }

    public enum UserType
    {
        Administrator,
        Manager,
        Editor,
        Viewer,
    }

    public class User : DbObjectModel<User>
    {
        public string Name { get; set; }

        public UserType Type { get; set; }

        public bool Valid { get; set; }

        [BelongsTo]
        public Company Company { get; set; }
    }

    public class Department : DbObjectModel<Department>
    {
        public string Name { get; set; }

        [BelongsTo]
        public Company Company { get; set; }

        [HasMany]
        public IList<Employee> Employees { get; set; }

        [HasMany]
        public IList<Resource> Resources { get; set; }
    }

    public class Employee : DbObjectModel<Employee>
    {
        public string Name { get; set; }

        public int Age { get; set; }

        [BelongsTo]
        public Company Company { get; set; }

        [BelongsTo]
        public Department Department { get; set; }
    }

    public class Resource : DbObjectModel<Resource>
    {
        public string Name { get; set; }

        public int Count { get; set; }

        [BelongsTo]
        public Company Company { get; set; }

        [BelongsTo]
        public Department Department { get; set; }
    }

    public class Trade : DbObjectModel<Trade>
    {
        public string Name { get; set; }

        public decimal TotalPrice { get; set; }

        [BelongsTo]
        public Company Company { get; set; }

        [HasMany]
        public IList<Order> Orders { get; set; }
    }

    public class Order : DbObjectModel<Order>
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }

        [LazyLoad]
        public string Description { get; set; }

        [BelongsTo]
        public Trade Trade { get; set; }

        [BelongsTo]
        public Product Product { get; set; }
    }

    public class Product : DbObjectModel<Product>
    {
        public string Name { get; set; }

        [HasMany]
        public IList<Order> Orders { get; set; }

        [BelongsTo]
        public Company Company { get; set; }
    }

    public class Customer : DbObjectModel<Customer>
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Mobile { get; set; }

        [BelongsTo]
        public Company Company { get; set; }
    }
}
