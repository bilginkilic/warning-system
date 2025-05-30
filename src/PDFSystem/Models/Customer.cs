using System;
using System.Collections.Generic;

namespace PDFSystem.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public List<Contract> Contracts { get; set; }

        public Customer()
        {
            Contracts = new List<Contract>();
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
    }
} 