using System;

namespace PDFSystem.Models
{
    public class SignatureAssignment
    {
        public int SignatureAssignmentId { get; set; }
        public int SignatureCircularId { get; set; }
        public string AssignedPersonName { get; set; }
        public string AssignedPersonEmail { get; set; }
        public string AssignedPersonTitle { get; set; }
        public string AssignedPersonDepartment { get; set; }
        
        // PDF'deki imza konumu bilgileri
        public int PDFPageNumber { get; set; }
        public float SignatureX { get; set; }
        public float SignatureY { get; set; }
        public float SignatureWidth { get; set; }
        public float SignatureHeight { get; set; }
        
        // İmza imajı bilgileri
        public string SignatureImagePath { get; set; }
        public byte[] SignatureImageData { get; set; }
        public string SignatureImageFormat { get; set; } // PNG, JPG, etc.
        
        public DateTime AssignedDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public string Status { get; set; } // Pending, Signed, Rejected
        public string Notes { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public SignatureCircular SignatureCircular { get; set; }

        public SignatureAssignment()
        {
            AssignedDate = DateTime.Now;
            Status = "Pending";
            IsActive = true;
        }
    }
} 