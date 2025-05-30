using System;
using System.Collections.Generic;

namespace PDFSystem.Models
{
    public class SignatureCircular
    {
        public int SignatureCircularId { get; set; }
        public int ContractId { get; set; }
        public string CircularTitle { get; set; }
        public string Description { get; set; }
        public string PDFFilePath { get; set; }
        public string PDFFileName { get; set; }
        public byte[] PDFContent { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } // Pending, InProgress, Completed, Cancelled
        public bool IsActive { get; set; }

        // Navigation Properties
        public Contract Contract { get; set; }
        public List<SignatureAssignment> SignatureAssignments { get; set; }

        public SignatureCircular()
        {
            SignatureAssignments = new List<SignatureAssignment>();
            CreatedDate = DateTime.Now;
            Status = "Pending";
            IsActive = true;
        }
    }
} 