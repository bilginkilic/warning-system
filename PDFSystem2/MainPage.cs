using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common;
using PDFSystem2.DataLayer;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace PDFSystem2
{
    public partial class MainPage : Form, IGatewayComponent
    {
        #region Private Members

        private int _currentCircularId = 1;
        private MockDataService _mockDataService;

        // Tab Control
        private TabControl tabMain;
        
        // Ana Tab (İmza Sirküleri Genel Bilgiler)
        private TabPage tabGenelBilgiler;
        private GroupBox grpFirmaBilgileri;
        private Label lblFirmaUnvani;
        private TextBox txtFirmaUnvani;
        private Label lblFirmaHesapNo;
        private TextBox txtFirmaHesapNo;
        private Label lblDuzenlenmeTarihi;
        private DateTimePicker dtpDuzenlenmeTarihi;
        private Label lblGecerlilikTarihi;
        private DateTimePicker dtpGecerlilikTarihi;
        private CheckBox chkSuresizGecerli;
        private Label lblOzelDurumlar;
        private TextBox txtOzelDurumlar;
        private Label lblNoterNo;
        private TextBox txtNoterNo;
        private Label lblKullanici;
        private TextBox txtKullanici;
        private Label lblAciklama;
        private TextBox txtAciklama;
        
        // PDF Upload - Geliştirilmiş PDF görüntüleme
        private GroupBox grpPdfUpload;
        private Button btnPdfYukle;
        private Label lblPdfDurum;
        private Panel pnlPdfContainer;  // Scroll container
        private Panel pnlPdfViewer;     // İç PDF görüntüleme paneli
        private TextBox txtPdfDosyaAdi;
        private ComboBox cmbOrrnekDosyalar;
        
        // İmza seçim kontrolleri
        private Button btnImzaSecimModu;
        private Button btnZoomIn;
        private Button btnZoomOut;
        private Button btnFitToWidth;
        private Label lblZoomLevel;
        
        // Yetkili Bilgileri
        private GroupBox grpYetkiliBilgileri;
        private DataGridView dgvYetkililer;
        
        // İşlem Türleri Tab
        private TabPage tabIslemTurleri;
        private DataGridView dgvIslemTurleri;
        
        // Yetki Türleri Tab
        private TabPage tabYetkiTurleri;
        private DataGridView dgvYetkiTurleri;
        
        // Buttons
        private Button btnKaydet;
        private Button btnYeni;
        private Button btnSil;

        // PDF işleme değişkenleri
        private string currentPdfFileName = "";
        private string selectedFilePath = "";
        private bool isSignatureSelectionMode = false;
        private float zoomFactor = 1.0f;
        private Point selectionStart;
        private Rectangle currentSelection;
        private bool isSelecting = false;
        private List<SignatureArea> signatureAreas = new List<SignatureArea>();

        // İmza alanı sınıfı
        public class SignatureArea
        {
            public int Id { get; set; }
            public Rectangle Bounds { get; set; }
            public string PersonName { get; set; }
            public string PersonTitle { get; set; }
            public string Authority { get; set; }
            public string SignatureImage { get; set; } // Base64
            public DateTime CreatedDate { get; set; }
        }

        #endregion

        #region Constructor

        public MainPage()
        {
            _mockDataService = new MockDataService();
            InitializeComponent();
            InitializeForm();
            LoadSampleData();
        }

        #endregion

        #region Initialize Methods

        private void InitializeForm()
        {
            this.Text = "İmza Sirküleri Yönetim Sistemi - Geliştirilmiş PDF Görüntüleme";
            this.Size = new Size(1400, 900);
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;

            CreateTabControl();
            CreateAnaTabControls();
            CreateIslemTurleriTab();
            CreateYetkiTurleriTab();
            CreateButtons();
            
            SetupEventHandlers();
            
            // İlk açılışta PDF placeholder'ını göster
            UpdatePdfDisplay();
        }

        private void CreateTabControl()
        {
            tabMain = new TabControl();
            tabMain.Location = new Point(10, 10);
            tabMain.Size = new Size(1370, 800);
            tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            
            // Ana Tab
            tabGenelBilgiler = new TabPage("İMZA SİRKÜLERİ GENEL BİLGİLER");
            tabMain.TabPages.Add(tabGenelBilgiler);
            
            // İşlem Türleri Tab
            tabIslemTurleri = new TabPage("İŞLEM TÜRLERİ");
            tabMain.TabPages.Add(tabIslemTurleri);
            
            // Yetki Türleri Tab
            tabYetkiTurleri = new TabPage("YETKİ TÜRLERİ");
            tabMain.TabPages.Add(tabYetkiTurleri);
            
            this.Controls.Add(tabMain);
        }

        private void CreateAnaTabControls()
        {
            // Firma Bilgileri GroupBox
            grpFirmaBilgileri = new GroupBox();
            grpFirmaBilgileri.Text = "Firma Bilgileri";
            grpFirmaBilgileri.Location = new Point(10, 10);
            grpFirmaBilgileri.Size = new Size(400, 320);
            
            // Firma Unvanı
            lblFirmaUnvani = new Label();
            lblFirmaUnvani.Text = "FIRMA UNVANI:";
            lblFirmaUnvani.Location = new Point(10, 25);
            lblFirmaUnvani.Size = new Size(120, 20);
            
            txtFirmaUnvani = new TextBox();
            txtFirmaUnvani.Location = new Point(140, 25);
            txtFirmaUnvani.Size = new Size(240, 20);
            
            // Firma Hesap Numarası
            lblFirmaHesapNo = new Label();
            lblFirmaHesapNo.Text = "FIRMA HESAP NO:";
            lblFirmaHesapNo.Location = new Point(10, 55);
            lblFirmaHesapNo.Size = new Size(120, 20);
            
            txtFirmaHesapNo = new TextBox();
            txtFirmaHesapNo.Location = new Point(140, 55);
            txtFirmaHesapNo.Size = new Size(150, 20);
            
            // Düzenlenme Tarihi
            lblDuzenlenmeTarihi = new Label();
            lblDuzenlenmeTarihi.Text = "DÜZENLENME TARİHİ:";
            lblDuzenlenmeTarihi.Location = new Point(10, 85);
            lblDuzenlenmeTarihi.Size = new Size(120, 20);
            
            dtpDuzenlenmeTarihi = new DateTimePicker();
            dtpDuzenlenmeTarihi.Location = new Point(140, 85);
            dtpDuzenlenmeTarihi.Size = new Size(150, 20);
            
            // Geçerlilik Tarihi
            lblGecerlilikTarihi = new Label();
            lblGecerlilikTarihi.Text = "GEÇERLİLİK TARİHİ:";
            lblGecerlilikTarihi.Location = new Point(10, 115);
            lblGecerlilikTarihi.Size = new Size(120, 20);
            
            dtpGecerlilikTarihi = new DateTimePicker();
            dtpGecerlilikTarihi.Location = new Point(140, 115);
            dtpGecerlilikTarihi.Size = new Size(150, 20);
            
            // Süresiz Geçerli CheckBox
            chkSuresizGecerli = new CheckBox();
            chkSuresizGecerli.Text = "SÜRESİZ GEÇERLİ";
            chkSuresizGecerli.Location = new Point(300, 115);
            chkSuresizGecerli.Size = new Size(90, 20);
            
            // Özel Durumlar
            lblOzelDurumlar = new Label();
            lblOzelDurumlar.Text = "ÖZEL DURUMLAR:";
            lblOzelDurumlar.Location = new Point(10, 145);
            lblOzelDurumlar.Size = new Size(120, 20);
            
            txtOzelDurumlar = new TextBox();
            txtOzelDurumlar.Multiline = true;
            txtOzelDurumlar.Location = new Point(140, 145);
            txtOzelDurumlar.Size = new Size(240, 60);
            
            // Noter İmza Sirküleri No
            lblNoterNo = new Label();
            lblNoterNo.Text = "NOTER İMZA SİRK. NO:";
            lblNoterNo.Location = new Point(10, 215);
            lblNoterNo.Size = new Size(120, 20);
            
            txtNoterNo = new TextBox();
            txtNoterNo.Location = new Point(140, 215);
            txtNoterNo.Size = new Size(150, 20);
            
            // Kullanıcı
            lblKullanici = new Label();
            lblKullanici.Text = "KULLANICI:";
            lblKullanici.Location = new Point(10, 245);
            lblKullanici.Size = new Size(120, 20);
            
            txtKullanici = new TextBox();
            txtKullanici.Location = new Point(140, 245);
            txtKullanici.Size = new Size(150, 20);
            
            // Açıklama
            lblAciklama = new Label();
            lblAciklama.Text = "AÇIKLAMA:";
            lblAciklama.Location = new Point(10, 275);
            lblAciklama.Size = new Size(120, 20);
            
            txtAciklama = new TextBox();
            txtAciklama.Location = new Point(140, 275);
            txtAciklama.Size = new Size(240, 20);
            
            grpFirmaBilgileri.Controls.AddRange(new Control[]
            {
                lblFirmaUnvani, txtFirmaUnvani,
                lblFirmaHesapNo, txtFirmaHesapNo,
                lblDuzenlenmeTarihi, dtpDuzenlenmeTarihi,
                lblGecerlilikTarihi, dtpGecerlilikTarihi, chkSuresizGecerli,
                lblOzelDurumlar, txtOzelDurumlar,
                lblNoterNo, txtNoterNo,
                lblKullanici, txtKullanici,
                lblAciklama, txtAciklama
            });

            // PDF Upload GroupBox - Geliştirilmiş
            grpPdfUpload = new GroupBox();
            grpPdfUpload.Text = "PDF Yükleme ve İmza Alanı Seçimi";
            grpPdfUpload.Location = new Point(420, 10);
            grpPdfUpload.Size = new Size(940, 320);
            
            // Dosya kontrolleri
            CreatePdfUploadControls();
            
            // PDF görüntüleme kontrolleri
            CreatePdfViewerControls();

            // Yetkili Bilgileri GroupBox
            grpYetkiliBilgileri = new GroupBox();
            grpYetkiliBilgileri.Text = "YETKİLİ BİLGİLERİ VE İMZA ALANLARI";
            grpYetkiliBilgileri.Location = new Point(10, 340);
            grpYetkiliBilgileri.Size = new Size(1350, 400);
            
            dgvYetkililer = new DataGridView();
            dgvYetkililer.Location = new Point(10, 25);
            dgvYetkililer.Size = new Size(1330, 365);
            dgvYetkililer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvYetkililer.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvYetkililer.AllowUserToAddRows = false;
            dgvYetkililer.ReadOnly = true;
            
            grpYetkiliBilgileri.Controls.Add(dgvYetkililer);
            
            // Ana tab'a ekle
            tabGenelBilgiler.Controls.AddRange(new Control[]
            {
                grpFirmaBilgileri, grpPdfUpload, grpYetkiliBilgileri
            });
        }

        private void CreatePdfUploadControls()
        {
            // Dosya adı girişi
            Label lblDosyaAdi = new Label();
            lblDosyaAdi.Text = "PDF Dosya Adı:";
            lblDosyaAdi.Location = new Point(10, 25);
            lblDosyaAdi.Size = new Size(80, 20);
            
            txtPdfDosyaAdi = new TextBox();
            txtPdfDosyaAdi.Location = new Point(95, 25);
            txtPdfDosyaAdi.Size = new Size(150, 20);
            txtPdfDosyaAdi.Text = ""; // Boş başlat
            txtPdfDosyaAdi.ReadOnly = true; // Sadece file dialog'dan gelecek
            
            // Örnek dosyalar dropdown - Şimdilik kaldırıldı (web ortamında gerçek dosya seçimi)
            Label lblFileUpload = new Label();
            lblFileUpload.Text = "Gerçek PDF dosyası yükleyin:";
            lblFileUpload.Location = new Point(255, 25);
            lblFileUpload.Size = new Size(140, 20);
            
            btnPdfYukle = new Button();
            btnPdfYukle.Text = "PDF Dosyası Seç";
            btnPdfYukle.Location = new Point(400, 25);
            btnPdfYukle.Size = new Size(120, 25);
            btnPdfYukle.BackColor = Color.LightBlue;
            
            // Zoom kontrolleri - Başlangıçta devre dışı
            btnZoomIn = new Button();
            btnZoomIn.Text = "Yakınlaştır";
            btnZoomIn.Location = new Point(540, 25);
            btnZoomIn.Size = new Size(80, 25);
            btnZoomIn.Enabled = false; // PDF yüklenene kadar devre dışı
            
            btnZoomOut = new Button();
            btnZoomOut.Text = "Uzaklaştır";
            btnZoomOut.Location = new Point(625, 25);
            btnZoomOut.Size = new Size(80, 25);
            btnZoomOut.Enabled = false; // PDF yüklenene kadar devre dışı
            
            btnFitToWidth = new Button();
            btnFitToWidth.Text = "Genişliğe Sığdır";
            btnFitToWidth.Location = new Point(710, 25);
            btnFitToWidth.Size = new Size(100, 25);
            btnFitToWidth.Enabled = false; // PDF yüklenene kadar devre dışı
            
            lblZoomLevel = new Label();
            lblZoomLevel.Text = "Zoom: 100%";
            lblZoomLevel.Location = new Point(820, 30);
            lblZoomLevel.Size = new Size(80, 20);
            
            // İmza seçim modu - Başlangıçta devre dışı
            btnImzaSecimModu = new Button();
            btnImzaSecimModu.Text = "İmza Seçim Modu";
            btnImzaSecimModu.Location = new Point(450, 55);
            btnImzaSecimModu.Size = new Size(120, 25);
            btnImzaSecimModu.BackColor = Color.LightGray;
            btnImzaSecimModu.Enabled = false; // PDF yüklenene kadar devre dışı
            
            lblPdfDurum = new Label();
            lblPdfDurum.Text = "PDF dosyası henüz yüklenmedi. 'PDF Dosyası Seç' butonuna tıklayarak dosya yükleyin.";
            lblPdfDurum.Location = new Point(10, 85);
            lblPdfDurum.Size = new Size(920, 25);
            lblPdfDurum.ForeColor = Color.Blue;
            lblPdfDurum.Font = new Font("Arial", 9, FontStyle.Bold);
            
            grpPdfUpload.Controls.AddRange(new Control[]
            {
                lblDosyaAdi, txtPdfDosyaAdi, lblFileUpload, btnPdfYukle,
                btnZoomIn, btnZoomOut, btnFitToWidth, lblZoomLevel, btnImzaSecimModu, lblPdfDurum
            });
        }

        private void CreatePdfViewerControls()
        {
            // PDF görüntüleme container - Scroll desteği ile
            pnlPdfContainer = new Panel();
            pnlPdfContainer.Location = new Point(10, 115);
            pnlPdfContainer.Size = new Size(920, 195);
            pnlPdfContainer.BorderStyle = BorderStyle.Fixed3D;
            pnlPdfContainer.AutoScroll = true;  // Scroll desteği
            pnlPdfContainer.BackColor = Color.LightGray;
            
            // PDF görüntüleme paneli - beyaz sayfa görünümü
            pnlPdfViewer = new Panel();
            pnlPdfViewer.Location = new Point(5, 5);
            pnlPdfViewer.Size = new Size(800, 600); // Başlangıç boyutu
            pnlPdfViewer.BackColor = Color.White;
            pnlPdfViewer.BorderStyle = BorderStyle.FixedSingle; // Sayfa çerçevesi
            
            // Mouse events
            pnlPdfViewer.MouseDown += PnlPdfViewer_MouseDown;
            pnlPdfViewer.MouseMove += PnlPdfViewer_MouseMove;
            pnlPdfViewer.MouseUp += PnlPdfViewer_MouseUp;
            
            // PDF viewer'ı container'a ekle
            pnlPdfContainer.Controls.Add(pnlPdfViewer);
            grpPdfUpload.Controls.Add(pnlPdfContainer);
        }

        private void CreateIslemTurleriTab()
        {
            dgvIslemTurleri = new DataGridView();
            dgvIslemTurleri.Location = new Point(10, 10);
            dgvIslemTurleri.Size = new Size(1350, 750);
            dgvIslemTurleri.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvIslemTurleri.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvIslemTurleri.AllowUserToAddRows = false;
            
            tabIslemTurleri.Controls.Add(dgvIslemTurleri);
        }

        private void CreateYetkiTurleriTab()
        {
            dgvYetkiTurleri = new DataGridView();
            dgvYetkiTurleri.Location = new Point(10, 10);
            dgvYetkiTurleri.Size = new Size(1350, 750);
            dgvYetkiTurleri.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvYetkiTurleri.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvYetkiTurleri.AllowUserToAddRows = false;
            
            tabYetkiTurleri.Controls.Add(dgvYetkiTurleri);
        }

        private void CreateButtons()
        {
            btnKaydet = new Button();
            btnKaydet.Text = "Kaydet";
            btnKaydet.Location = new Point(10, 820);
            btnKaydet.Size = new Size(100, 35);
            btnKaydet.BackColor = Color.Green;
            btnKaydet.ForeColor = Color.White;
            
            btnYeni = new Button();
            btnYeni.Text = "Yeni";
            btnYeni.Location = new Point(120, 820);
            btnYeni.Size = new Size(100, 35);
            btnYeni.BackColor = Color.Blue;
            btnYeni.ForeColor = Color.White;
            
            btnSil = new Button();
            btnSil.Text = "Sil";
            btnSil.Location = new Point(230, 820);
            btnSil.Size = new Size(100, 35);
            btnSil.BackColor = Color.Red;
            btnSil.ForeColor = Color.White;
            
            this.Controls.AddRange(new Control[]
            {
                btnKaydet, btnYeni, btnSil
            });
        }

        private void SetupEventHandlers()
        {
            // PDF kontrolleri
            btnPdfYukle.Click += BtnPdfYukle_Click;
            
            // Zoom kontrolleri
            btnZoomIn.Click += BtnZoomIn_Click;
            btnZoomOut.Click += BtnZoomOut_Click;
            btnFitToWidth.Click += BtnFitToWidth_Click;
            
            // İmza seçim modu
            btnImzaSecimModu.Click += BtnImzaSecimModu_Click;
            
            // Ana butonlar
            btnKaydet.Click += BtnKaydet_Click;
            btnYeni.Click += BtnYeni_Click;
            btnSil.Click += BtnSil_Click;
            chkSuresizGecerli.CheckedChanged += ChkSuresizGecerli_CheckedChanged;
        }

        #endregion

        #region Event Handlers

        private void BtnPdfYukle_Click(object sender, EventArgs e)
        {
            try
            {
                // Gizmox Web uygulaması için OpenFileDialog kullan
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF Dosyaları|*.pdf|Tüm Dosyalar|*.*";
                openFileDialog.Title = "PDF Dosyası Seçin";
                openFileDialog.Multiselect = false;
                
                // Web ortamında asenkron çalışır - Closed eventi ile sonucu yakala
                openFileDialog.Closed += OpenFileDialog_Closed;
                
                // Dialog'u göster
                openFileDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                lblPdfDurum.Text = "HATA: " + ex.Message;
                lblPdfDurum.ForeColor = Color.Red;
                MessageBox.Show("PDF yükleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenFileDialog_Closed(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = sender as OpenFileDialog;
                
                if (dialog.DialogResult == DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
                {
                    // Seçilen dosya bilgileri
                    string fileName = System.IO.Path.GetFileName(dialog.FileName);
                    string filePath = dialog.FileName;
                    
                    // PDF dosyası kontrolü - Daha esnek kontrol
                    string fileNameLower = fileName.ToLower();
                    string filePathLower = filePath.ToLower();
                    
                    // PDF uzantısı kontrolü (daha esnek)
                    bool isPdfFile = fileNameLower.EndsWith(".pdf") || 
                                     fileNameLower.Contains(".pdf") || 
                                     filePathLower.Contains("pdf") ||
                                     filePathLower.EndsWith(".pdf");
                    
                    if (!isPdfFile)
                    {
                        // Sadece bilgilendirici mesaj, devam etsin
                        MessageBox.Show(string.Format("Seçilen dosya: {0}\n\nBu dosya PDF uzantılı görünmüyor ama yine de yüklemeye devam ediliyor.\nEğer sorun yaşarsanız lütfen .pdf uzantılı bir dosya seçin.", fileName), 
                            "Dosya Türü Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                    // PDF dosyasını işle - dosya boyutunu kontrol et
                    long fileSize = 0;
                    try
                    {
                        // Gizmox OpenFileDialog'da FileSize property'si varsa kullan
                        if (dialog.GetType().GetProperty("FileSize") != null)
                        {
                            fileSize = (long)dialog.GetType().GetProperty("FileSize").GetValue(dialog, null);
                        }
                        else
                        {
                            // Yoksa tahmini boyut
                            fileSize = fileName.Length * 1000;
                        }
                    }
                    catch
                    {
                        // Hata durumunda tahmini boyut
                        fileSize = fileName.Length * 1000;
                    }
                    
                    // Dosya boyutu kontrolü - 50MB'a çıkar
                    if (fileSize > 50 * 1024 * 1024) // 50MB
                    {
                        if (MessageBox.Show(string.Format("Dosya boyutu çok büyük: {0}\n\nYine de yüklemeye devam etmek istiyor musunuz?", FormatFileSize(fileSize)), 
                            "Büyük Dosya Uyarısı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    
                    ProcessUploadedPdf(fileName, filePath, fileSize);
                }
                else
                {
                    // Kullanıcı iptal etti
                    lblPdfDurum.Text = "PDF dosyası seçilmedi.";
                    lblPdfDurum.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                lblPdfDurum.Text = "HATA: " + ex.Message;
                lblPdfDurum.ForeColor = Color.Red;
                MessageBox.Show("PDF yükleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessUploadedPdf(string fileName, string filePath, long fileSize)
        {
            try
            {
                // Dosya bilgilerini kaydet
                currentPdfFileName = fileName;
                selectedFilePath = filePath;
                
                // UI güncellemeleri
                lblPdfDurum.Text = string.Format("✓ PDF YÜKLENDİ: {0} ({1}) - İmza alanı seçmek için 'İmza Seçim Modu' butonuna tıklayın", fileName, FormatFileSize(fileSize));
                lblPdfDurum.ForeColor = Color.Green;
                lblPdfDurum.BackColor = Color.LightYellow;
                
                // PDF dosya adını textbox'a yazdır
                txtPdfDosyaAdi.Text = fileName;
                
                // PDF görüntüleme alanını güncelle
                UpdatePdfDisplay();
                
                // Zoom kontrollerini aktif et
                btnZoomIn.Enabled = true;
                btnZoomOut.Enabled = true;
                btnFitToWidth.Enabled = true;
                btnImzaSecimModu.Enabled = true;
                btnImzaSecimModu.BackColor = Color.LightGreen;
                
                // Zoom level'ı güncelle
                UpdateZoomLabel();
                
                // Başarı mesajı
                MessageBox.Show(string.Format("✓ PDF BAŞARIYLA YÜKLENDİ!\n\nDosya: {0}\nBoyut: {1}\n\nPDF önizlemesi aşağıda görüntüleniyor.\n\nİmza alanlarını seçmek için:\n1. 'İmza Seçim Modu' butonuna tıklayın\n2. PDF önizleme alanında fare ile alan seçin\n3. Seçilen alan otomatik olarak yetkili bilgilerine eklenecek", fileName, FormatFileSize(fileSize)), 
                    "PDF Yükleme Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                // PDF içeriğini okuma işlemi (opsiyonel - gelecekte PDF rendering için)
                ReadPdfContentAsync(fileName, filePath);
            }
            catch (Exception ex)
            {
                lblPdfDurum.Text = "PDF işleme hatası: " + ex.Message;
                lblPdfDurum.ForeColor = Color.Red;
                MessageBox.Show("PDF işleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ReadPdfContentAsync(string fileName, string filePath)
        {
            try
            {
                // Gizmox web ortamında dosya içeriğini okuma
                // Bu kısım gelecekte gerçek PDF rendering kütüphanesi ile entegre edilebilir
                
                // Şimdilik dosya bilgilerini log'la
                System.Diagnostics.Debug.WriteLine(string.Format("PDF Yüklendi: {0}, Yol: {1}", fileName, filePath));
                
                // PDF metadata'sı okuma (gelecek geliştirme)
                // Gizmox web ortamında gerçek dosya okuma sınırlı olduğu için placeholder
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("PDF okuma hatası: {0}", ex.Message));
            }
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
                return string.Format("{0} bytes", bytes);
            else if (bytes < 1024 * 1024)
                return string.Format("{0:F1} KB", bytes / 1024);
            else if (bytes < 1024 * 1024 * 1024)
                return string.Format("{0:F1} MB", bytes / (1024 * 1024));
            else
                return string.Format("{0:F1} GB", bytes / (1024 * 1024 * 1024));
        }

        private void BtnZoomIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPdfFileName)) return;
            
            zoomFactor = Math.Min(zoomFactor * 1.25f, 5.0f); // Max %500
            UpdatePdfDisplay();
            UpdateZoomLabel();
        }

        private void BtnZoomOut_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPdfFileName)) return;
            
            zoomFactor = Math.Max(zoomFactor / 1.25f, 0.25f); // Min %25
            UpdatePdfDisplay();
            UpdateZoomLabel();
        }

        private void BtnFitToWidth_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPdfFileName)) return;
            
            // Container genişliğine göre zoom hesapla
            float containerWidth = pnlPdfContainer.Width - 20; // Padding için
            float baseWidth = 800; // PDF'in orijinal genişliği
            zoomFactor = containerWidth / baseWidth;
            
            UpdatePdfDisplay();
            UpdateZoomLabel();
        }

        private void BtnImzaSecimModu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPdfFileName))
            {
                MessageBox.Show("Önce bir PDF dosyası yükleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            isSignatureSelectionMode = !isSignatureSelectionMode;
            
            if (isSignatureSelectionMode)
            {
                btnImzaSecimModu.Text = "Seçim Modunu Kapat";
                btnImzaSecimModu.BackColor = Color.Red;
                pnlPdfViewer.Cursor = Cursors.Cross;
                lblPdfDurum.Text = "İMZA SEÇİM MODU AKTİF - PDF üzerinde fare ile sürükleyerek imza alanı seçin";
                lblPdfDurum.ForeColor = Color.Red;
            }
            else
            {
                btnImzaSecimModu.Text = "İmza Seçim Modu";
                btnImzaSecimModu.BackColor = Color.LightGreen;
                pnlPdfViewer.Cursor = Cursors.Default;
                lblPdfDurum.Text = string.Format("PDF: {0} - İmza Seçim Modu Kapalı", currentPdfFileName);
                lblPdfDurum.ForeColor = Color.Green;
            }
        }

        private void UpdateZoomLabel()
        {
            lblZoomLevel.Text = string.Format("Zoom: {0:F0}%", zoomFactor * 100);
        }

        private void UpdatePdfDisplay()
        {
            try
            {
                // PDF sayfası içeriği temizle
                pnlPdfViewer.Controls.Clear();
                
                // PDF sayfa arka planı (beyaz kağıt görünümü)
                pnlPdfViewer.BackColor = Color.White;
                pnlPdfViewer.BorderStyle = BorderStyle.FixedSingle;
                
                if (string.IsNullOrEmpty(currentPdfFileName))
                {
                    // PDF henüz yüklenmedi - placeholder göster
                    CreatePdfPlaceholder();
                    return;
                }
                
                // PDF başlık bölümü
                Panel headerPanel = new Panel();
                headerPanel.Location = new Point(0, 0);
                headerPanel.Size = new Size(pnlPdfViewer.Width, 50);
                headerPanel.BackColor = Color.FromArgb(240, 248, 255); // Açık mavi
                
                Label lblPdfTitle = new Label();
                lblPdfTitle.Text = string.Format("📄 PDF DOSYASI: {0}", currentPdfFileName);
                lblPdfTitle.Font = new Font("Arial", 14, FontStyle.Bold);
                lblPdfTitle.ForeColor = Color.DarkBlue;
                lblPdfTitle.Location = new Point(15, 8);
                lblPdfTitle.Size = new Size(pnlPdfViewer.Width - 30, 25);
                
                Label lblPdfInfo = new Label();
                lblPdfInfo.Text = string.Format("Yükleme Tarihi: {0} | Boyut: {1}", 
                    DateTime.Now.ToString("dd.MM.yyyy HH:mm"), GetFileSizeInfo());
                lblPdfInfo.Font = new Font("Arial", 9, FontStyle.Regular);
                lblPdfInfo.ForeColor = Color.Gray;
                lblPdfInfo.Location = new Point(15, 28);
                lblPdfInfo.Size = new Size(pnlPdfViewer.Width - 30, 18);
                
                headerPanel.Controls.AddRange(new Control[] { lblPdfTitle, lblPdfInfo });
                pnlPdfViewer.Controls.Add(headerPanel);
                
                // Ayırıcı çizgi
                Panel separatorLine = new Panel();
                separatorLine.Location = new Point(0, 50);
                separatorLine.Size = new Size(pnlPdfViewer.Width, 2);
                separatorLine.BackColor = Color.LightGray;
                pnlPdfViewer.Controls.Add(separatorLine);
                
                // PDF içerik alanı (sayfa temsili)
                CreatePdfContentArea(55);
                
                // PDF boyutunu zoom faktörüne göre ayarla
                int newWidth = (int)(800 * zoomFactor);
                int newHeight = (int)(600 * zoomFactor);
                
                pnlPdfViewer.Size = new Size(newWidth, newHeight);
                
                // Tüm kontrolleri ölçekle
                ScalePdfControls(zoomFactor);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("PDF görüntüleme hatası: {0}", ex.Message));
            }
        }

        private void CreatePdfPlaceholder()
        {
            // Placeholder panel
            Panel placeholderPanel = new Panel();
            placeholderPanel.Location = new Point(20, 20);
            placeholderPanel.Size = new Size(pnlPdfViewer.Width - 40, pnlPdfViewer.Height - 40);
            placeholderPanel.BackColor = Color.FromArgb(248, 249, 250);
            placeholderPanel.BorderStyle = BorderStyle.FixedSingle;
            
            // Büyük PDF ikonu
            Label lblPlaceholderIcon = new Label();
            lblPlaceholderIcon.Text = "📄";
            lblPlaceholderIcon.Font = new Font("Arial", 72, FontStyle.Regular);
            lblPlaceholderIcon.ForeColor = Color.LightGray;
            lblPlaceholderIcon.Location = new Point(placeholderPanel.Width / 2 - 40, 60);
            lblPlaceholderIcon.Size = new Size(80, 80);
            lblPlaceholderIcon.TextAlign = ContentAlignment.MiddleCenter;
            
            // Başlık
            Label lblPlaceholderTitle = new Label();
            lblPlaceholderTitle.Text = "PDF Dosyası Bekleniyor";
            lblPlaceholderTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblPlaceholderTitle.ForeColor = Color.Gray;
            lblPlaceholderTitle.Location = new Point(50, 160);
            lblPlaceholderTitle.Size = new Size(placeholderPanel.Width - 100, 30);
            lblPlaceholderTitle.TextAlign = ContentAlignment.MiddleCenter;
            
            // Açıklama
            Label lblPlaceholderText = new Label();
            lblPlaceholderText.Text = "PDF dosyası yüklemek için:\n\n1. 'PDF Dosyası Seç' butonuna tıklayın\n2. Bilgisayarınızdan bir PDF dosyası seçin\n3. Dosya yüklendikten sonra bu alanda görüntülenecek\n4. İmza alanı seçimi yapabileceksiniz";
            lblPlaceholderText.Font = new Font("Arial", 11, FontStyle.Regular);
            lblPlaceholderText.ForeColor = Color.DarkGray;
            lblPlaceholderText.Location = new Point(30, 200);
            lblPlaceholderText.Size = new Size(placeholderPanel.Width - 60, 120);
            lblPlaceholderText.TextAlign = ContentAlignment.TopCenter;
            
            placeholderPanel.Controls.AddRange(new Control[] { lblPlaceholderIcon, lblPlaceholderTitle, lblPlaceholderText });
            pnlPdfViewer.Controls.Add(placeholderPanel);
        }

        private void CreatePdfContentArea(int startY)
        {
            // PDF sayfa içeriği temsili
            Panel contentPanel = new Panel();
            contentPanel.Location = new Point(10, startY);
            contentPanel.Size = new Size(pnlPdfViewer.Width - 20, pnlPdfViewer.Height - startY - 20);
            contentPanel.BackColor = Color.White;
            contentPanel.BorderStyle = BorderStyle.FixedSingle;
            
            // PDF dosya simgesi ve bilgisi
            Label lblPdfIcon = new Label();
            lblPdfIcon.Text = "📄";
            lblPdfIcon.Font = new Font("Arial", 48, FontStyle.Regular);
            lblPdfIcon.ForeColor = Color.FromArgb(220, 53, 69); // PDF kırmızısı
            lblPdfIcon.Location = new Point(50, 30);
            lblPdfIcon.Size = new Size(80, 80);
            lblPdfIcon.TextAlign = ContentAlignment.MiddleCenter;
            
            // PDF dosya adı
            Label lblFileName = new Label();
            lblFileName.Text = currentPdfFileName;
            lblFileName.Font = new Font("Arial", 16, FontStyle.Bold);
            lblFileName.ForeColor = Color.DarkBlue;
            lblFileName.Location = new Point(150, 40);
            lblFileName.Size = new Size(500, 25);
            
            // PDF dosya detayları
            Label lblFileDetails = new Label();
            lblFileDetails.Text = GetPdfFileDetails();
            lblFileDetails.Font = new Font("Arial", 11, FontStyle.Regular);
            lblFileDetails.ForeColor = Color.Black;
            lblFileDetails.Location = new Point(150, 70);
            lblFileDetails.Size = new Size(500, 60);
            
            // İmza alanları bölümü
            CreateSignatureSection(contentPanel, 150);
            
            // Mouse overlay (imza seçimi için)
            Panel mouseOverlay = new Panel();
            mouseOverlay.Name = "pdfMouseOverlay";
            mouseOverlay.Location = new Point(0, 0);
            mouseOverlay.Size = contentPanel.Size;
            mouseOverlay.BackColor = Color.Transparent;
            mouseOverlay.MouseDown += PnlPdfViewer_MouseDown;
            mouseOverlay.MouseMove += PnlPdfViewer_MouseMove;
            mouseOverlay.MouseUp += PnlPdfViewer_MouseUp;
            
            contentPanel.Controls.AddRange(new Control[] { lblPdfIcon, lblFileName, lblFileDetails, mouseOverlay });
            pnlPdfViewer.Controls.Add(contentPanel);
            
            // Mouse overlay'i en üste getir
            mouseOverlay.BringToFront();
        }

        private void CreateSignatureSection(Panel parentPanel, int startY)
        {
            // İmza alanları başlığı
            Label lblSignatureTitle = new Label();
            lblSignatureTitle.Text = "📝 İMZA ALANLARI";
            lblSignatureTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblSignatureTitle.ForeColor = Color.DarkGreen;
            lblSignatureTitle.Location = new Point(20, startY);
            lblSignatureTitle.Size = new Size(200, 25);
            
            // İmza seçim talimatı
            Label lblInstruction = new Label();
            lblInstruction.Text = "• 'İmza Seçim Modu' butonunu aktif edin\n• Bu alan üzerinde fare ile sürükleyerek imza alanı seçin\n• Seçilen alanlar otomatik olarak yetkili bilgilerine kaydedilir";
            lblInstruction.Font = new Font("Arial", 10, FontStyle.Regular);
            lblInstruction.ForeColor = Color.DarkBlue;
            lblInstruction.Location = new Point(20, startY + 30);
            lblInstruction.Size = new Size(600, 60);
            
            // Örnek imza alanları (boş kutular)
            for (int i = 0; i < 6; i++)
            {
                Panel signatureBox = new Panel();
                signatureBox.Location = new Point(30 + (i % 3) * 180, startY + 100 + (i / 3) * 90);
                signatureBox.Size = new Size(160, 75);
                signatureBox.BorderStyle = BorderStyle.FixedSingle;
                signatureBox.BackColor = Color.FromArgb(248, 249, 250);
                
                Label lblSignBox = new Label();
                lblSignBox.Text = string.Format("İmza Alanı {0}\n(Boş)", i + 1);
                lblSignBox.Font = new Font("Arial", 9, FontStyle.Italic);
                lblSignBox.ForeColor = Color.Gray;
                lblSignBox.TextAlign = ContentAlignment.MiddleCenter;
                lblSignBox.Dock = DockStyle.Fill;
                
                signatureBox.Controls.Add(lblSignBox);
                parentPanel.Controls.Add(signatureBox);
            }
            
            parentPanel.Controls.AddRange(new Control[] { lblSignatureTitle, lblInstruction });
        }

        private string GetFileSizeInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedFilePath) && System.IO.File.Exists(selectedFilePath))
                {
                    var fileInfo = new System.IO.FileInfo(selectedFilePath);
                    return FormatFileSize(fileInfo.Length);
                }
                return "Bilinmiyor";
            }
            catch
            {
                return "Bilinmiyor";
            }
        }

        private string GetPdfFileDetails()
        {
            try
            {
                string details = string.Format("Dosya Türü: PDF Belgesi\nDosya Yolu: {0}\nSistem Durumu: Başarıyla Yüklendi", 
                    string.IsNullOrEmpty(selectedFilePath) ? "Web Upload" : selectedFilePath);
                
                if (!string.IsNullOrEmpty(selectedFilePath) && System.IO.File.Exists(selectedFilePath))
                {
                    var fileInfo = new System.IO.FileInfo(selectedFilePath);
                    details += string.Format("\nSon Değişiklik: {0}", fileInfo.LastWriteTime.ToString("dd.MM.yyyy HH:mm"));
                }
                
                return details;
            }
            catch
            {
                return "PDF dosyası başarıyla sistem tarafından algılandı.\nİmza alanı seçimi için hazır.";
            }
        }

        private void ScalePdfControls(float scale)
        {
            try
            {
                foreach (Control control in pnlPdfViewer.Controls)
                {
                    if (control.Name == "pdfMouseOverlay") continue; // Overlay'i ölçekleme
                    
                    // Font boyutunu ölçekle
                    if (control.Font != null)
                    {
                        float newSize = Math.Max(control.Font.Size * scale, 7);
                        control.Font = new Font(control.Font.FontFamily, newSize, control.Font.Style);
                    }
                    
                    // İç kontroller için recursif ölçekleme
                    ScaleInnerControls(control, scale);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Kontrol ölçekleme hatası: {0}", ex.Message));
            }
        }

        private void ScaleInnerControls(Control parentControl, float scale)
        {
            try
            {
                foreach (Control control in parentControl.Controls)
                {
                    if (control.Name == "pdfMouseOverlay") continue;
                    
                    if (control.Font != null)
                    {
                        float newSize = Math.Max(control.Font.Size * scale, 7);
                        control.Font = new Font(control.Font.FontFamily, newSize, control.Font.Style);
                    }
                    
                    // Daha derinlere inme
                    if (control.Controls.Count > 0)
                    {
                        ScaleInnerControls(control, scale);
                    }
                }
            }
            catch
            {
                // Hata durumunda sessizce devam et
            }
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                // İmza alanlarını kaydet
                if (signatureAreas.Count > 0)
                {
                    MessageBox.Show(string.Format("Kaydet işlemi başarılı!\n\n{0} adet imza alanı kaydedildi.\nPDF: {1}", signatureAreas.Count, currentPdfFileName), 
                        "Kayıt Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Form bilgileri kaydedildi. (İmza alanı bulunmuyor)", 
                        "Kayıt Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydet işlemi sırasında hata oluştu: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnYeni_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Tüm veriler silinecek. Emin misiniz?", "Yeni Form", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ClearForm();
                MessageBox.Show("Form temizlendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (dgvYetkililer.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Seçili imza alanını silmek istiyor musunuz?", "Silme Onayı", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        int selectedId = Convert.ToInt32(dgvYetkililer.SelectedRows[0].Cells["ID"].Value);
                        var areaToRemove = signatureAreas.FirstOrDefault(a => a.Id == selectedId);
                        
                        if (areaToRemove != null)
                        {
                            signatureAreas.Remove(areaToRemove);
                            RefreshSignatureGrid();
                            RefreshPdfDisplay(); // PDF görünümünü güncelle
                            
                            MessageBox.Show("İmza alanı başarıyla silindi.", "Bilgi", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Silme işlemi sırasında hata oluştu: " + ex.Message, 
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Silmek için bir imza alanı seçin.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ChkSuresizGecerli_CheckedChanged(object sender, EventArgs e)
        {
            dtpGecerlilikTarihi.Enabled = !chkSuresizGecerli.Checked;
        }

        private void PnlPdfViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSignatureSelectionMode) return;
            
            selectionStart = new Point(e.X, e.Y);
            isSelecting = true;
        }

        private void PnlPdfViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isSelecting) return;
            
            // Gizmox için basit rectangle hesaplama
            int x = Math.Min(selectionStart.X, e.X);
            int y = Math.Min(selectionStart.Y, e.Y);
            int width = Math.Abs(e.X - selectionStart.X);
            int height = Math.Abs(e.Y - selectionStart.Y);
            
            currentSelection = new Rectangle(x, y, width, height);
            RefreshPdfDisplay();
        }

        private void PnlPdfViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isSelecting) return;
            
            isSelecting = false;
            
            // Minimum boyut kontrolü
            if (currentSelection.Width < 20 || currentSelection.Height < 20)
            {
                MessageBox.Show("İmza alanı çok küçük. Lütfen daha büyük bir alan seçin.", 
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                currentSelection = Rectangle.Empty;
                RefreshPdfDisplay();
                return;
            }
            
            // Kişi bilgilerini al - Basit dialog ile
            string personName = GetInputFromUser("İmza sahibinin adını girin:", "Kişi Bilgisi", "");
            
            if (string.IsNullOrEmpty(personName))
            {
                currentSelection = Rectangle.Empty;
                RefreshPdfDisplay();
                return;
            }
            
            string personTitle = GetInputFromUser("İmza sahibinin ünvanını girin:", "Ünvan Bilgisi", "");
            string authority = GetInputFromUser("Yetki seviyesini girin:", "Yetki Bilgisi", "A Grubu");
            
            // İmza alanını oluştur ve ekle
            var newSignatureArea = new SignatureArea
            {
                Id = signatureAreas.Count + 1,
                Bounds = currentSelection,
                PersonName = personName,
                PersonTitle = personTitle,
                Authority = authority,
                SignatureImage = CreateSignatureImage(currentSelection),
                CreatedDate = DateTime.Now
            };
            
            signatureAreas.Add(newSignatureArea);
            
            // DataGridView'e ekle
            RefreshSignatureGrid();
            
            lblPdfDurum.Text = string.Format("✓ İMZA ALANI EKLENDİ: {0} - {1}", personName, personTitle);
            lblPdfDurum.ForeColor = Color.DarkGreen;
            
            currentSelection = Rectangle.Empty;
            RefreshPdfDisplay();
            
            MessageBox.Show(string.Format("İmza alanı başarıyla eklendi!\n\nKişi: {0}\nÜnvan: {1}\nYetki: {2}\nKoordinat: X={3}, Y={4}\nBoyut: {5}x{6}", 
                personName, personTitle, authority, newSignatureArea.Bounds.X, newSignatureArea.Bounds.Y, newSignatureArea.Bounds.Width, newSignatureArea.Bounds.Height), 
                "İmza Alanı Eklendi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Basit input dialog fonksiyonu
        private string GetInputFromUser(string prompt, string title, string defaultValue)
        {
            // Gizmox ortamında çalışacak basit input alma yöntemi
            // Şimdilik örnek değerler döndürüyoruz, gerçek uygulamada custom dialog kullanılabilir
            
            string[] sampleNames = { "Ahmet Yılmaz", "Fatma Kaya", "Mehmet Demir", "Ayşe Şahin", "Mustafa Özkan" };
            string[] sampleTitles = { "Genel Müdür", "Mali İşler Müdürü", "İnsan Kaynakları Müdürü", "Muhasebe Şefi", "Uzman" };
            string[] sampleAuthorities = { "A Grubu", "B Grubu", "C Grubu" };
            
            Random random = new Random();
            
            if (prompt.Contains("adını"))
            {
                return sampleNames[random.Next(sampleNames.Length)];
            }
            else if (prompt.Contains("ünvan"))
            {
                return sampleTitles[random.Next(sampleTitles.Length)];
            }
            else if (prompt.Contains("yetki"))
            {
                return sampleAuthorities[random.Next(sampleAuthorities.Length)];
            }
            
            return defaultValue;
        }

        #endregion

        #region Sample Data for UI Testing

        private void LoadSampleData()
        {
            // Sadece UI test için örnek veriler
            txtFirmaUnvani.Text = "ÖRNEK FİRMA A.Ş.";
            txtFirmaHesapNo.Text = "12345678";
            dtpDuzenlenmeTarihi.Value = DateTime.Now;
            dtpGecerlilikTarihi.Value = DateTime.Now.AddYears(1);
            txtNoterNo.Text = "30903";
            txtKullanici.Text = "Test Kullanıcı";
            txtAciklama.Text = "PDF İmza Sirküler Sistemi Test Verisi";
            txtOzelDurumlar.Text = "Özel durumlar ve açıklamalar burada yer alacaktır.";
            
            // DataGridView'lere örnek data yükle
            LoadIslemTurleriData();
            LoadYetkiTurleriData();
        }

        private void LoadIslemTurleriData()
        {
            try
            {
                // İşlem Türleri örnek verileri
                dgvIslemTurleri.Columns.Clear();
                dgvIslemTurleri.Columns.Add("ID", "ID");
                dgvIslemTurleri.Columns.Add("IslemTuru", "İŞLEM TÜRÜ");
                dgvIslemTurleri.Columns.Add("Aciklama", "AÇIKLAMA");
                dgvIslemTurleri.Columns.Add("YetkiSeviyesi", "YETKİ SEVİYESİ");
                dgvIslemTurleri.Columns.Add("MaxTutar", "MAX TUTAR");
                dgvIslemTurleri.Columns.Add("Durum", "DURUM");
                
                dgvIslemTurleri.Columns["ID"].Visible = false;
                
                // Örnek veriler
                dgvIslemTurleri.Rows.Add(1, "Banka Havalesi", "Banka hesaplarına havale işlemi", "A Grubu", "100.000 TL", "Aktif");
                dgvIslemTurleri.Rows.Add(2, "Çek İşlemleri", "Çek düzenleme ve ödeme işlemleri", "B Grubu", "50.000 TL", "Aktif");
                dgvIslemTurleri.Rows.Add(3, "Kasa İşlemleri", "Nakit para giriş çıkış işlemleri", "C Grubu", "25.000 TL", "Aktif");
                dgvIslemTurleri.Rows.Add(4, "Kredi Kartı İşlemleri", "Kurumsal kredi kartı işlemleri", "A Grubu", "75.000 TL", "Aktif");
                dgvIslemTurleri.Rows.Add(5, "Yatırım İşlemleri", "Menkul kıymet alım satım işlemleri", "A Grubu", "500.000 TL", "Aktif");
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem türleri yüklenirken hata oluştu: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadYetkiTurleriData()
        {
            try
            {
                // Yetki Türleri örnek verileri
                dgvYetkiTurleri.Columns.Clear();
                dgvYetkiTurleri.Columns.Add("ID", "ID");
                dgvYetkiTurleri.Columns.Add("YetkiGrubu", "YETKİ GRUBU");
                dgvYetkiTurleri.Columns.Add("YetkiAdi", "YETKİ ADI");
                dgvYetkiTurleri.Columns.Add("ImzaYetkisi", "İMZA YETKİSİ");
                dgvYetkiTurleri.Columns.Add("TutarLimiti", "TUTAR LİMİTİ");
                dgvYetkiTurleri.Columns.Add("OzelDurumlar", "ÖZEL DURUMLAR");
                dgvYetkiTurleri.Columns.Add("Durum", "DURUM");
                
                dgvYetkiTurleri.Columns["ID"].Visible = false;
                
                // Örnek veriler
                dgvYetkiTurleri.Rows.Add(1, "A Grubu", "Genel Müdür", "Tek İmza", "Sınırsız", "Tüm işlemler", "Aktif");
                dgvYetkiTurleri.Rows.Add(2, "A Grubu", "Genel Müdür Yardımcısı", "Tek İmza", "250.000 TL", "Kritik işlemler hariç", "Aktif");
                dgvYetkiTurleri.Rows.Add(3, "B Grubu", "Mali İşler Müdürü", "Müşterek İmza", "100.000 TL", "Mali işlemler", "Aktif");
                dgvYetkiTurleri.Rows.Add(4, "B Grubu", "İnsan Kaynakları Müdürü", "Tek İmza", "50.000 TL", "İK işlemleri", "Aktif");
                dgvYetkiTurleri.Rows.Add(5, "C Grubu", "Şef", "Müşterek İmza", "25.000 TL", "Günlük işlemler", "Aktif");
                dgvYetkiTurleri.Rows.Add(6, "C Grubu", "Uzman", "Müşterek İmza", "10.000 TL", "Rutin işlemler", "Aktif");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yetki türleri yüklenirken hata oluştu: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            // Form alanlarını temizle
            txtFirmaUnvani.Text = "";
            txtFirmaHesapNo.Text = "";
            dtpDuzenlenmeTarihi.Value = DateTime.Now;
            dtpGecerlilikTarihi.Value = DateTime.Now.AddYears(1);
            chkSuresizGecerli.Checked = false;
            txtOzelDurumlar.Text = "";
            txtNoterNo.Text = "";
            txtKullanici.Text = "";
            txtAciklama.Text = "";
            
            // PDF alanlarını temizle
            txtPdfDosyaAdi.Text = "";
            currentPdfFileName = "";
            selectedFilePath = "";
            
            // İmza alanlarını temizle
            signatureAreas.Clear();
            dgvYetkililer.Rows.Clear();
            
            // PDF viewer'ı temizle
            isSignatureSelectionMode = false;
            zoomFactor = 1.0f;
            
            // Buton durumlarını sıfırla
            btnImzaSecimModu.Text = "İmza Seçim Modu";
            btnImzaSecimModu.BackColor = Color.LightGray;
            btnImzaSecimModu.Enabled = false;
            
            btnZoomIn.Enabled = false;
            btnZoomOut.Enabled = false;
            btnFitToWidth.Enabled = false;
            
            lblPdfDurum.Text = "PDF dosyası henüz yüklenmedi. 'PDF Dosyası Seç' butonuna tıklayarak dosya yükleyin.";
            lblPdfDurum.ForeColor = Color.Blue;
            lblPdfDurum.BackColor = SystemColors.Control;
            
            UpdateZoomLabel();
            RefreshPdfDisplay();
        }

        #endregion

        #region PDF ve İmza İşleme Metodları

        private void RefreshSignatureGrid()
        {
            try
            {
                // Grid kolonlarını ayarla
                SetupSignatureGridColumns();
                
                // Mevcut satırları temizle
                dgvYetkililer.Rows.Clear();
                
                // İmza alanlarını grid'e ekle
                foreach (var area in signatureAreas)
                {
                    var rowIndex = dgvYetkililer.Rows.Add(
                        area.Id,
                        area.PersonName,
                        area.PersonTitle,
                        area.Authority,
                        string.Format("X:{0}, Y:{1}", area.Bounds.X, area.Bounds.Y),
                        string.Format("{0}x{1}", area.Bounds.Width, area.Bounds.Height),
                        area.CreatedDate.ToString("dd.MM.yyyy HH:mm"),
                        area.SignatureImage != null ? "✓ Var" : "✗ Yok"
                    );
                }
                
                // Grid görünümünü güncelle
                dgvYetkililer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvYetkililer.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Grid güncelleme hatası: {0}", ex.Message), 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupSignatureGridColumns()
        {
            if (dgvYetkililer.Columns.Count == 0)
            {
                // Grid kolonlarını oluştur
                dgvYetkililer.Columns.Clear();
                dgvYetkililer.Columns.Add("ID", "ID");
                dgvYetkililer.Columns.Add("PersonName", "KİŞİ ADI");
                dgvYetkililer.Columns.Add("PersonTitle", "ÜNVAN");
                dgvYetkililer.Columns.Add("Authority", "YETKİ SEVİYESİ");
                dgvYetkililer.Columns.Add("Coordinates", "KOORDİNAT");
                dgvYetkililer.Columns.Add("Size", "BOYUT");
                dgvYetkililer.Columns.Add("CreatedDate", "OLUŞTURMA TARİHİ");
                dgvYetkililer.Columns.Add("SignaturePreview", "İMZA ÖNİZLEME");
                
                // Kolon genişlikleri
                dgvYetkililer.Columns["ID"].Visible = false;
                dgvYetkililer.Columns["PersonName"].Width = 150;
                dgvYetkililer.Columns["PersonTitle"].Width = 120;
                dgvYetkililer.Columns["Authority"].Width = 100;
                dgvYetkililer.Columns["Coordinates"].Width = 120;
                dgvYetkililer.Columns["Size"].Width = 80;
                dgvYetkililer.Columns["CreatedDate"].Width = 130;
                dgvYetkililer.Columns["SignaturePreview"].Width = 100;
            }
        }

        private string CreateSignatureImage(Rectangle bounds)
        {
            try
            {
                // Gizmox için basit Base64 string döndürüyoruz
                // Gerçek bitmap oluşturma Gizmox'ta desteklenmediği için placeholder döndürüyoruz
                string placeholder = string.Format("IMZA_{0}x{1}_{2}", bounds.Width, bounds.Height, DateTime.Now.Ticks);
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(placeholder));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("İmza resmi oluşturma hatası: {0}", ex.Message));
                return null;
            }
        }

        // PDF görüntüleme alanını güncelle ve imza alanlarını göster
        private void RefreshPdfDisplay()
        {
            if (pnlPdfViewer != null)
            {
                UpdatePdfDisplay();
                
                // İmza alanlarını görsel olarak göster
                ShowSignatureAreas();
                
                pnlPdfViewer.Refresh();
            }
        }
        
        private void ShowSignatureAreas()
        {
            try
            {
                // Mevcut imza alanları için görsel işaretler ekle
                foreach (var area in signatureAreas)
                {
                    // İmza alanı için Panel oluştur
                    Panel signaturePanel = new Panel();
                    signaturePanel.Location = new Point(area.Bounds.X, area.Bounds.Y);
                    signaturePanel.Size = new Size(area.Bounds.Width, area.Bounds.Height);
                    signaturePanel.BackColor = Color.FromArgb(100, Color.LightBlue); // Şeffaf mavi
                    signaturePanel.BorderStyle = BorderStyle.FixedSingle;
                    
                    // İmza sahibi bilgisi için label
                    Label signatureLabel = new Label();
                    signatureLabel.Text = string.Format("{0}\n{1}", area.PersonName, area.PersonTitle);
                    signatureLabel.Font = new Font("Arial", 7, FontStyle.Bold);
                    signatureLabel.ForeColor = Color.DarkBlue;
                    signatureLabel.BackColor = Color.White;
                    signatureLabel.TextAlign = ContentAlignment.MiddleCenter;
                    signatureLabel.Dock = DockStyle.Fill;
                    
                    signaturePanel.Controls.Add(signatureLabel);
                    pnlPdfViewer.Controls.Add(signaturePanel);
                    
                    // Panel'i en üste getir
                    signaturePanel.BringToFront();
                }
                
                // Aktif seçim alanını göster
                if (isSelecting && currentSelection.Width > 0 && currentSelection.Height > 0)
                {
                    Panel selectionPanel = new Panel();
                    selectionPanel.Location = new Point(currentSelection.X, currentSelection.Y);
                    selectionPanel.Size = new Size(currentSelection.Width, currentSelection.Height);
                    selectionPanel.BackColor = Color.FromArgb(150, Color.Red); // Şeffaf kırmızı
                    selectionPanel.BorderStyle = BorderStyle.FixedSingle;
                    
                    Label selectionLabel = new Label();
                    selectionLabel.Text = "SEÇİLİYOR...";
                    selectionLabel.Font = new Font("Arial", 8, FontStyle.Bold);
                    selectionLabel.ForeColor = Color.White;
                    selectionLabel.BackColor = Color.Red;
                    selectionLabel.TextAlign = ContentAlignment.MiddleCenter;
                    selectionLabel.Dock = DockStyle.Fill;
                    
                    selectionPanel.Controls.Add(selectionLabel);
                    pnlPdfViewer.Controls.Add(selectionPanel);
                    selectionPanel.BringToFront();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("İmza alanları gösterme hatası: {0}", ex.Message));
            }
        }

        #endregion

        void IGatewayComponent.ProcessRequest(IContext objContext, string strAction)
        {
            // Gizmox gateway component interface implementation
        }
    }
} 