# PDF İmza Sirküler Sistemi

Bu proje, müşterilerin kontraktları altında imza sirküler yönetimi için geliştirilmiş bir Gizmox Web Application Framework tabanlı sistemdir.

## Özellikler

### Ana Özellikler
- **Müşteri Yönetimi**: Müşteri bilgilerini kaydetme ve yönetme
- **Kontrakt Yönetimi**: Müşterilere ait kontraktları organize etme
- **İmza Sirküler Sistemi**: PDF dokümanlar için imza akış süreçleri
- **PDF Görüntüleme**: Web tabanlı PDF görüntüleme ve işaretleme
- **İmza Atama**: PDF üzerinde imza alanlarını işaretleme ve kişilere atama
- **İmza Takibi**: İmza süreçlerini takip etme ve durum yönetimi

### Teknik Özellikler
- **.NET Framework 4.5.2** uyumlu
- **Gizmox Web Application Framework** kullanımı
- **SQL Server** veritabanı desteği
- **Responsive** web arayüzü
- **Multi-user** kullanım desteği

## Sistem Gereksinimleri

- .NET Framework 4.5.2 veya üzeri
- Visual Studio 2013 veya üzeri
- SQL Server 2012 veya üzeri
- Gizmox Web Framework Runtime
- IIS 7.0 veya üzeri (deployment için)

## Kurulum

### 1. Veritabanı Kurulumu
```sql
-- SQL Server Management Studio'da aşağıdaki script'i çalıştırın:
-- Database/CreateTables.sql dosyasındaki script'i kullanın
```

### 2. Connection String Ayarları
`app.config` veya `web.config` dosyasında connection string'i güncelleyin:
```xml
<connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=YOUR_SERVER;Initial Catalog=PDFSignatureSystem;Integrated Security=True" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 3. Proje Derleme
- Visual Studio'da projeyi açın
- Solution'ı rebuild edin
- Gizmox referanslarının doğru olduğundan emin olun

## Kullanım

### Ana Ekranlar

#### 1. Müşteri Kontraktları Formu (`CustomerContractsForm`)
- Müşteri seçimi
- Müşteriye ait kontraktları listeleme
- Yeni kontrakt ekleme
- İmza sirküler yönetimine geçiş

#### 2. İmza Sirküler Formu (`SignatureCircularForm`)
- Kontrakt bazlı imza sirküler listesi
- Yeni imza sirküler oluşturma
- PDF görüntüleme ve imza yönetimi
- Sirküler durumu takibi

#### 3. PDF Görüntüleyici (`PDFViewerForm`)
- PDF dokümanını görüntüleme
- Zoom işlemleri (yakınlaştır, uzaklaştır, genişliğe sığdır)
- İmza alanlarını işaretleme (fare ile alan seçimi)
- İmza atamalarını görüntüleme
- Koordinat bazlı imza konumlandırma

### Temel İş Akışı

1. **Müşteri Seçimi**: Ana formda müşterinizi seçin
2. **Kontrakt Yönetimi**: Müşterinin kontraktlarını görüntüleyin/ekleyin
3. **İmza Sirküler Oluşturma**: Kontrakt için yeni imza sirküler başlatın
4. **PDF Yükleme**: İmzalanacak PDF dokümanını sisteme yükleyin
5. **İmza Alanı İşaretleme**: PDF üzerinde imza alanlarını fare ile işaretleyin
6. **Kişi Atama**: Her imza alanına sorumlu kişileri atayın
7. **İmza Takibi**: İmza süreçlerini takip edin

## Proje Yapısı

```
src/PDFSystem/
├── Models/
│   ├── Customer.cs              # Müşteri model
│   ├── Contract.cs              # Kontrakt model
│   ├── SignatureCircular.cs     # İmza sirküler model
│   └── SignatureAssignment.cs   # İmza atama model
├── Services/
│   ├── CustomerService.cs       # Müşteri işlemleri
│   ├── ContractService.cs       # Kontrakt işlemleri
│   └── SignatureCircularService.cs # İmza sirküler işlemleri
├── Forms/
│   ├── CustomerContractsForm.cs # Ana müşteri-kontrakt formu
│   ├── SignatureCircularForm.cs # İmza sirküler yönetim formu
│   └── PDFViewerForm.cs         # PDF görüntüleme ve işaretleme formu
└── Database/
    └── CreateTables.sql         # Veritabanı oluşturma script'i
```

## Veritabanı Şeması

### Tablolar
- **Customers**: Müşteri bilgileri
- **Contracts**: Kontrakt bilgileri
- **SignatureCirculars**: İmza sirküler bilgileri ve PDF içeriği
- **SignatureAssignments**: İmza atamaları ve konum bilgileri

### İlişkiler
```
Customers (1) -> (N) Contracts
Contracts (1) -> (N) SignatureCirculars
SignatureCirculars (1) -> (N) SignatureAssignments
```

## Önemli Notlar

### PDF İşleme
- PDF görüntüleme için gerçek uygulamada PDF.js veya benzeri kütüphane kullanılmalı
- Şu anda örnek bitmap görüntü kullanılmaktadır
- PDF koordinat sistemi ile UI koordinat sistemi arasında dönüşüm yapılmaktadır

### İmza İşaretleme
- Fare ile sürükle-bırak ile alan seçimi
- Zoom faktörü dikkate alınarak gerçek PDF koordinatları hesaplanır
- Minimum alan boyutu kontrolü (10x10 piksel)

### Güvenlik
- SQL Injection koruması parametreli sorgular ile sağlanmıştır
- Connection string'ler config dosyalarında saklanmalıdır
- Production ortamında ek güvenlik önlemleri alınmalıdır

## Geliştirme Notları

### Eksik/Geliştirilmesi Gereken Özellikler
1. **PDF Kütüphanesi**: Gerçek PDF rendering kütüphanesi entegrasyonu
2. **E-posta Bildirimleri**: İmza atamaları için otomatik e-posta gönderimi
3. **Digital İmza**: Elektronik imza entegrasyonu
4. **Raporlama**: İmza süreçleri için detaylı raporlar
5. **Audit Log**: Sistem işlemlerinin loglanması
6. **Mobile Support**: Mobil cihaz desteği

### Teknik İyileştirmeler
- Repository pattern implementasyonu
- Dependency injection
- Unit testing
- Error handling & logging
- Configuration management

## Lisans

Bu proje özel kullanım için geliştirilmiştir. Dağıtım ve kullanım hakları proje sahibine aittir.

## İletişim

Proje ile ilgili sorular için: [Developer Contact Information] 