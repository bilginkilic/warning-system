# İmza Sirküleri Yönetim Sistemi (PDFSystem2)

Bu proje, Gizmox WebGUI framework kullanılarak .NET Framework 4.5.2 ile geliştirilmiş bir imza sirküleri yönetim sistemidir.

## Proje Yapısı

```
PDFSystem2/
├── Database/
│   ├── CreateTables.sql          # Veritabanı tabloları
│   └── StoredProcedures.sql      # Stored procedure'lar
├── DataLayer/
│   ├── MockDataService.cs        # Test verisi sağlayıcısı
│   └── DatabaseService.cs        # Veritabanı erişim katmanı
├── Models/
│   ├── SgnCircular.cs           # Ana sirküler modeli
│   ├── SgnCircularDetail.cs     # Yetkili detayları modeli
│   ├── SgnOperation.cs          # İşlem türleri modeli
│   └── SgnRoleType.cs           # Yetki türleri modeli
├── Properties/
│   └── AssemblyInfo.cs          # Assembly bilgileri
├── MainPage.cs                  # Ana form
├── MainPage.Designer.cs         # Form tasarımı
├── PDFSystem2.csproj           # Proje dosyası
└── README.md                    # Bu dosya
```

## Veritabanı Yapısı

### Ana Tablolar

1. **SGN_CIRCULAR**: İmza sirkülerinin genel bilgilerini saklayan ana tablo
   - Firma bilgileri
   - Düzenlenme ve geçerlilik tarihleri
   - PDF dosya yolu
   - Özel durumlar ve açıklamalar

2. **SGN_CIRCULARDETAIL**: İmza yetkilerinin detaylarını saklayan tablo
   - Yetkililer ve yetki şekilleri
   - İmza koordinatları
   - Yetki grupları ve sınırlamalar

3. **SGN_OPERATION**: İşlem türlerini saklayan tablo
   - Kredi sözleşmeleri
   - Transfer işlemleri
   - Türev işlemler vb.

4. **SGN_ROLETYPE**: Yetki türlerini ve gruplarını saklayan tablo
   - A, B, C grupları
   - Derece bilgileri
   - İmza yetki sınırları

## Kurulum

### Gereksinimler

- .NET Framework 4.5.2
- Visual Studio 2015 veya üzeri
- Gizmox WebGUI Framework
- SQL Server 2012 veya üzeri

### Adımlar

1. **Veritabanı Kurulumu**:
   ```sql
   -- CreateTables.sql dosyasını çalıştırın
   -- StoredProcedures.sql dosyasını çalıştırın
   ```

2. **Proje Yapılandırması**:
   - Gizmox WebGUI DLL'lerinin yollarını güncelleyin
   - Connection string'i DatabaseService constructor'ında ayarlayın

3. **Bağımlılıklar**:
   - Gizmox.WebGUI.Common.dll
   - Gizmox.WebGUI.Forms.dll
   - Gizmox.WebGUI.Server.dll

## Kullanım

### Ana Özellikler

1. **Tab Yapısı**:
   - **İmza Sirküleri Genel Bilgiler**: Ana firma bilgileri ve PDF yükleme
   - **İşlem Türleri**: Mevcut işlem tiplerinin görüntülenmesi
   - **Yetki Türleri**: Yetki grupları ve seviyelerinin yönetimi

2. **PDF Yönetimi**:
   - PDF dosyası yükleme
   - İmza alanlarını seçme (koordinat bazlı)
   - PDF önizleme (gelecekte implement edilecek)

3. **Veri Yönetimi**:
   - CRUD işlemleri
   - Mock data ile test
   - Stored procedure entegrasyonu

### Mock Data ile Test

Proje başlangıçta `MockDataService` kullanarak test verisi sağlar:

```csharp
var mockService = new MockDataService();
var circulars = mockService.GetAllCirculars();
```

### Veritabanı Entegrasyonu

Production ortamında `DatabaseService` kullanın:

```csharp
var dbService = new DatabaseService("your_connection_string");
int newId = dbService.InsertCircular(circular);
```

## Stored Procedure İsimlendirme

Tüm stored procedure'lar şu şekilde isimlendirilmiştir:

- `SGN_CIRCULAR_INS_SP` - Insert
- `SGN_CIRCULAR_UPD_SP` - Update  
- `SGN_CIRCULAR_SEL_SP` - Select
- `SGN_CIRCULAR_DEL_SP` - Delete (Soft Delete)

Diğer tablolar için de aynı pattern kullanılmıştır.

## Form Tasarımı

Ana form üç tabdan oluşur:

1. **İmza Sirküleri Genel Bilgiler**:
   - Sol panel: Firma bilgileri formu
   - Sağ panel: PDF yükleme ve önizleme
   - Alt panel: Yetkili bilgileri grid'i

2. **İşlem Türleri**:
   - Tam sayfa DataGridView

3. **Yetki Türleri**:
   - Tam sayfa DataGridView

## Gelecek Geliştirmeler

- [ ] PDF önizleme ve görüntüleme
- [ ] İmza alanı seçimi için mouse handling
- [ ] PDF'den koordinat yakalama
- [ ] İmza görüntülerini kaydetme
- [ ] Gelişmiş raporlama
- [ ] Kullanıcı yetki yönetimi

## Destek

Sorularınız için proje geliştiricisine ulaşabilirsiniz.

## Lisans

Bu proje özel kullanım içindir. 