using System;
using System.Drawing;

namespace WarningSystem
{
    public enum WarningMode
    {
        // Temel modlar
        Simulation = 0,           // Sadece gösterim modu, OK butonu
        Confirmation = 1,         // Onay gerektiren mod, Continue/Cancel butonları
        
        // Bilgilendirme modları
        Information = 10,         // Bilgi verici uyarılar
        Notification = 11,        // Anlık bildirimler
        
        // Operasyonel modlar
        Process = 20,            // İşlem gerektiren uyarılar
        Action = 21,             // Aksiyon gerektiren uyarılar
        Task = 22,               // Görev oluşturan uyarılar
        
        // Kritik modlar
        Critical = 30,           // Kritik uyarılar
        Emergency = 31,          // Acil durum uyarıları
        Alert = 32,              // Alarm durumu
        
        // Sistem modları
        System = 40,             // Sistem uyarıları
        Maintenance = 41,        // Bakım uyarıları
        Update = 42,             // Güncelleme uyarıları
        
        // Özel modlar
        Custom = 90,             // Özelleştirilebilir mod
        Debug = 91,              // Hata ayıklama modu
        Test = 92               // Test modu
    }

    public static class WarningModeExtensions
    {
        public static bool RequiresConfirmation(this WarningMode mode)
        {
            return mode switch
            {
                WarningMode.Confirmation => true,
                WarningMode.Process => true,
                WarningMode.Action => true,
                WarningMode.Critical => true,
                WarningMode.Emergency => true,
                _ => false
            };
        }

        public static bool IsSystemLevel(this WarningMode mode)
        {
            return mode switch
            {
                WarningMode.System => true,
                WarningMode.Maintenance => true,
                WarningMode.Update => true,
                _ => false
            };
        }

        public static string GetDescription(this WarningMode mode)
        {
            return mode switch
            {
                WarningMode.Simulation => "Sadece görüntüleme modu",
                WarningMode.Confirmation => "Kullanıcı onayı gerektiren mod",
                WarningMode.Information => "Bilgilendirme amaçlı uyarılar",
                WarningMode.Notification => "Anlık bildirim uyarıları",
                WarningMode.Process => "İşlem gerektiren uyarılar",
                WarningMode.Action => "Aksiyon gerektiren uyarılar",
                WarningMode.Task => "Görev oluşturan uyarılar",
                WarningMode.Critical => "Kritik seviye uyarılar",
                WarningMode.Emergency => "Acil durum uyarıları",
                WarningMode.Alert => "Alarm durumu uyarıları",
                WarningMode.System => "Sistem seviyesi uyarılar",
                WarningMode.Maintenance => "Bakım ile ilgili uyarılar",
                WarningMode.Update => "Güncelleme bildirimleri",
                WarningMode.Custom => "Özelleştirilmiş uyarı modu",
                WarningMode.Debug => "Hata ayıklama modu",
                WarningMode.Test => "Test amaçlı uyarılar",
                _ => "Tanımlanmamış mod"
            };
        }

        public static Color GetModeColor(this WarningMode mode)
        {
            return mode switch
            {
                WarningMode.Simulation => Color.Gray,
                WarningMode.Confirmation => Color.Blue,
                WarningMode.Information => Color.DarkGray,
                WarningMode.Notification => Color.Green,
                WarningMode.Process => Color.Orange,
                WarningMode.Action => Color.DarkOrange,
                WarningMode.Task => Color.Purple,
                WarningMode.Critical => Color.Red,
                WarningMode.Emergency => Color.DarkRed,
                WarningMode.Alert => Color.OrangeRed,
                WarningMode.System => Color.Navy,
                WarningMode.Maintenance => Color.Teal,
                WarningMode.Update => Color.RoyalBlue,
                WarningMode.Custom => Color.Black,
                WarningMode.Debug => Color.DimGray,
                WarningMode.Test => Color.SlateGray,
                _ => Color.Black
            };
        }
    }
} 