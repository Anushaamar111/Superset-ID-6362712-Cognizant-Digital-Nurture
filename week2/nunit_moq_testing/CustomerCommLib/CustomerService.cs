using System;
using System.Collections.Generic;

namespace CustomerCommLib
{
    /// <summary>
    /// Interface for database operations - allows mocking database interactions
    /// </summary>
    public interface ICustomerRepository
    {
        Customer GetCustomerById(int customerId);
        List<Customer> GetAllCustomers();
        bool SaveCustomer(Customer customer);
        bool DeleteCustomer(int customerId);
    }

    /// <summary>
    /// Customer entity class
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Real database repository implementation
    /// This would contain actual database connection logic
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        public Customer GetCustomerById(int customerId)
        {
            // In real implementation, this would query the database
            // For demonstration, we'll throw an exception to show why mocking is needed
            throw new NotImplementedException("Real database implementation would go here");
        }

        public List<Customer> GetAllCustomers()
        {
            throw new NotImplementedException("Real database implementation would go here");
        }

        public bool SaveCustomer(Customer customer)
        {
            throw new NotImplementedException("Real database implementation would go here");
        }

        public bool DeleteCustomer(int customerId)
        {
            throw new NotImplementedException("Real database implementation would go here");
        }
    }

    /// <summary>
    /// Service class that uses database repository
    /// This class demonstrates how to make database-dependent code testable
    /// </summary>
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMailSender _mailSender;

        public CustomerService(ICustomerRepository customerRepository, IMailSender mailSender)
        {
            _customerRepository = customerRepository;
            _mailSender = mailSender;
        }

        /// <summary>
        /// Business method that retrieves customer and sends welcome email
        /// </summary>
        public bool SendWelcomeEmail(int customerId)
        {
            // Get customer from database
            var customer = _customerRepository.GetCustomerById(customerId);
            
            if (customer == null)
                return false;

            // Send welcome email
            string message = $"Welcome {customer.Name}! Thank you for joining us.";
            return _mailSender.SendMail(customer.Email, message);
        }

        /// <summary>
        /// Business method that creates a new customer and sends welcome email
        /// </summary>
        public bool CreateCustomerAndSendWelcome(string name, string email)
        {
            var customer = new Customer
            {
                Name = name,
                Email = email,
                CreatedDate = DateTime.Now
            };

            // Save to database
            bool saved = _customerRepository.SaveCustomer(customer);
            if (!saved)
                return false;

            // Send welcome email
            string message = $"Welcome {customer.Name}! Your account has been created successfully.";
            return _mailSender.SendMail(customer.Email, message);
        }
    }
}
