# Warning System

Modern ve kullanıcı dostu bir Windows Forms uyarı sistemi.

## Özellikler

- Farklı uyarı modları (Simulation, Confirmation, vb.)
- Renkli ve gruplandırılmış uyarılar
- Modern ve şık arayüz
- Asenkron çalışma desteği
- Özelleştirilebilir tasarım

## Kullanım

```csharp
// Uyarı listesi oluştur
var warnings = new List<Warning>
{
    new Warning 
    { 
        WarningNo = 1, 
        WarningText = "Kritik uyarı mesajı", 
        WarningColor = "Red",
        WarningMode = WarningMode.Critical
    },
    new Warning 
    { 
        WarningNo = 2, 
        WarningText = "Bilgilendirme mesajı", 
        WarningColor = "DarkGray",
        WarningMode = WarningMode.Information
    }
};

// WarningManager oluştur
var warningManager = new WarningManager();

// Uyarıları göster
var results = await warningManager.ShowWarningsAsync(warnings, WarningMode.Simulation);
```

## Uyarı Modları

- **Simulation**: Sadece görüntüleme modu
- **Confirmation**: Onay gerektiren mod
- **Information**: Bilgilendirme amaçlı
- **Critical**: Kritik uyarılar
- **System**: Sistem uyarıları
- Ve daha fazlası...

## Gereksinimler

- .NET Framework 4.5.2 veya üzeri
- Windows Forms uygulaması

## Kurulum

1. Projeyi klonlayın
2. WarningSystem klasörünü projenize ekleyin
3. Gerekli referansları ekleyin
4. Kullanmaya başlayın

## Lisans

MIT 