using System;
using System.Drawing;

namespace WarningSystem
{
    public class Warning
    {
        public int Id { get; set; }
        public int WarningNo { get; set; }
        public string WarningText { get; set; }
        public string WarningColor { get; set; }
        public WarningMode WarningMode { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string ProcessedBy { get; set; }

        public Color GetSystemColor()
        {
            return WarningColor?.ToLower() switch
            {
                "black" => Color.Black,
                "darkgray" => Color.DarkGray,
                "red" => Color.Red,
                _ => Color.Black
            };
        }
    }
} 