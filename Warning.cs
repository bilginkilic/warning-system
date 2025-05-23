using System;
using System.Drawing;

public class Warning
{
    public int WarningNo { get; set; }
    public string WarningText { get; set; }
    public string WarningColor { get; set; }

    public Color GetSystemColor()
    {
        switch (WarningColor.ToLower())
        {
            case "black":
                return Color.Black;
            case "darkgray":
                return Color.DarkGray;
            case "red":
                return Color.Red;
            default:
                return Color.Black;
        }
    }
} 