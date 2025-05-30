using System;
using System.Collections.Generic;

namespace PDFSystem.Models
{
    public class Contract
    {
        public int ContractId { get; set; }
        public int CustomerId { get; set; }
        public string ContractNumber { get; set; }
        public string ContractTitle { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? ContractAmount { get; set; }
        public string Status { get; set; } // Draft, Active, Completed, Cancelled
        public bool IsActive { get; set; }

        // Navigation Properties
        public Customer Customer { get; set; }
        public List<SignatureCircular> SignatureCirculars { get; set; }

        public Contract()
        {
            SignatureCirculars = new List<SignatureCircular>();
            CreatedDate = DateTime.Now;
            Status = "Draft";
            IsActive = true;
        }
    }
} 