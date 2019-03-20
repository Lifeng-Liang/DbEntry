using System.Collections.Generic;
using Leafing.Data.Definition;

namespace Leafing.UnitTest.Data.Objects {
    public class Company : DbObjectModel<Company> {
        public string Name { get; set; }

        public HasMany<User> Users { get; private set; }

        public HasMany<Department> Departments { get; private set; }

        public HasMany<Employee> Employees { get; private set; }

        public HasMany<Resource> Resources { get; private set; }

        public HasMany<Trade> Trades { get; private set; }

        public HasMany<Customer> Customers { get; private set; }

        public HasMany<Product> Products { get; private set; }

        public Company() {
            Users = new HasMany<User>(this, "Id", "Company_Id");
            Departments = new HasMany<Department>(this, "Id", "Company_Id");
            Employees = new HasMany<Employee>(this, "Id", "Company_Id");
            Resources = new HasMany<Resource>(this, "Id", "Company_Id");
            Trades = new HasMany<Trade>(this, "Id", "Company_Id");
            Customers = new HasMany<Customer>(this, "Id", "Company_Id");
            Products = new HasMany<Product>(this, "Id", "Company_Id");
        }
    }

    public enum UserType {
        Administrator,
        Manager,
        Editor,
        Viewer,
    }

    public class User : DbObjectModel<User> {
        public string Name { get; set; }

        public UserType Type { get; set; }

        public bool Valid { get; set; }

        public BelongsTo<Company, long> Company { get; private set; }

        public User() {
            Company = new BelongsTo<Company, long>(this, "Company_Id");
        }
    }

    public class Department : DbObjectModel<Department> {
        public string Name { get; set; }

        public BelongsTo<Company, long> Company { get; private set; }

        public HasMany<Employee> Employees { get; private set; }

        public HasMany<Resource> Resources { get; private set; }

        public Department() {
            Company = new BelongsTo<Company, long>(this, "Company_Id");
            Employees = new HasMany<Employee>(this, "Id", "Department_Id");
            Resources = new HasMany<Resource>(this, "Id", "Department_Id");
        }
    }

    public class Employee : DbObjectModel<Employee> {
        public string Name { get; set; }

        public int Age { get; set; }

        public BelongsTo<Company, long> Company { get; private set; }

        public BelongsTo<Department, long> Department { get; private set; }

        public Employee() {
            Company = new BelongsTo<Company, long>(this, "Company_Id");
            Department = new BelongsTo<Department, long>(this, "Department_Id");
        }
    }

    public class Resource : DbObjectModel<Resource> {
        public string Name { get; set; }

        public int Count { get; set; }

        public BelongsTo<Company, long> Company { get; private set; }

        public BelongsTo<Department, long> Department { get; private set; }

        public Resource() {
            Company = new BelongsTo<Company, long>(this, "Company_Id");
            Department = new BelongsTo<Department, long>(this, "Department_Id");
        }
    }

    public class Trade : DbObjectModel<Trade> {
        public string Name { get; set; }

        public decimal TotalPrice { get; set; }

        public BelongsTo<Company, long> Company { get; private set; }

        public HasMany<Order> Orders { get; private set; }

        public Trade() {
            Company = new BelongsTo<Company, long>(this, "Company_Id");
            Orders = new HasMany<Order>(this, "Id", "Trade_Id");
        }
    }

    public class Order : DbObjectModel<Order> {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Count { get; set; }

        public LazyLoad<string> Description { get; private set; }

        public BelongsTo<Trade, long> Trade { get; private set; }

        public BelongsTo<Product, long> Product { get; private set; }
    }

    public class Product : DbObjectModel<Product> {
        public string Name { get; set; }

        public HasMany<Order> Orders { get; private set; }

        public BelongsTo<Company, long> Company { get; private set; }
    }

    public class Customer : DbObjectModel<Customer> {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Mobile { get; set; }

        public BelongsTo<Company, long> Company { get; set; }
    }
}