using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common;
using PDFSystem2.DataLayer;
using System.Linq;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
            txtPdfDosyaAdi.Text = "imza_sirkuleri.pdf";
            
            // Örnek dosyalar dropdown
            Label lblOrnekDosyalar = new Label();
            lblOrnekDosyalar.Text = "veya seçin:";
            lblOrnekDosyalar.Location = new Point(255, 25);
            lblOrnekDosyalar.Size = new Size(60, 20);
            
            cmbOrrnekDosyalar = new ComboBox();
            cmbOrrnekDosyalar.Location = new Point(320, 25);
            cmbOrrnekDosyalar.Size = new Size(120, 20);
            cmbOrrnekDosyalar.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOrrnekDosyalar.Items.AddRange(new string[]
            {
                "-- Seçin --",
                "imza_sirkuleri.pdf", 
                "yetki_belgesi.pdf",
                "banka_sirkuleri.pdf",
                "noter_belgesi.pdf"
            });
            cmbOrrnekDosyalar.SelectedIndex = 0;
            
            btnPdfYukle = new Button();
            btnPdfYukle.Text = "PDF Yükle";
            btnPdfYukle.Location = new Point(450, 25);
            btnPdfYukle.Size = new Size(80, 25);
            btnPdfYukle.BackColor = Color.LightBlue;
            
            // Zoom kontrolleri
            btnZoomIn = new Button();
            btnZoomIn.Text = "Yakınlaştır";
            btnZoomIn.Location = new Point(540, 25);
            btnZoomIn.Size = new Size(80, 25);
            
            btnZoomOut = new Button();
            btnZoomOut.Text = "Uzaklaştır";
            btnZoomOut.Location = new Point(625, 25);
            btnZoomOut.Size = new Size(80, 25);
            
            btnFitToWidth = new Button();
            btnFitToWidth.Text = "Genişliğe Sığdır";
            btnFitToWidth.Location = new Point(710, 25);
            btnFitToWidth.Size = new Size(100, 25);
            
            lblZoomLevel = new Label();
            lblZoomLevel.Text = "Zoom: 100%";
            lblZoomLevel.Location = new Point(820, 30);
            lblZoomLevel.Size = new Size(80, 20);
            
            // İmza seçim modu
            btnImzaSecimModu = new Button();
            btnImzaSecimModu.Text = "İmza Seçim Modu";
            btnImzaSecimModu.Location = new Point(450, 55);
            btnImzaSecimModu.Size = new Size(120, 25);
            btnImzaSecimModu.BackColor = Color.LightGreen;
            
            lblPdfDurum = new Label();
            lblPdfDurum.Text = "PDF dosyası henüz seçilmedi. Yukarıdan dosya adı girin veya örnek seçin.";
            lblPdfDurum.Location = new Point(10, 85);
            lblPdfDurum.Size = new Size(920, 25);
            lblPdfDurum.ForeColor = Color.Blue;
            lblPdfDurum.Font = new Font("Arial", 9, FontStyle.Bold);
            
            grpPdfUpload.Controls.AddRange(new Control[]
            {
                lblDosyaAdi, txtPdfDosyaAdi, lblOrnekDosyalar, cmbOrrnekDosyalar, btnPdfYukle,
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
            
            // İç PDF görüntüleme paneli
            pnlPdfViewer = new Panel();
            pnlPdfViewer.Location = new Point(0, 0);
            pnlPdfViewer.Size = new Size(800, 600); // Başlangıç boyutu
            pnlPdfViewer.BackColor = Color.White;
            pnlPdfViewer.Paint += PnlPdfViewer_Paint;
            pnlPdfViewer.MouseDown += PnlPdfViewer_MouseDown;
            pnlPdfViewer.MouseMove += PnlPdfViewer_MouseMove;
            pnlPdfViewer.MouseUp += PnlPdfViewer_MouseUp;
            
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
            cmbOrrnekDosyalar.SelectedIndexChanged += CmbOrrnekDosyalar_SelectedIndexChanged;
            
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

        private void CmbOrrnekDosyalar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbOrrnekDosyalar.SelectedIndex > 0)
            {
                txtPdfDosyaAdi.Text = cmbOrrnekDosyalar.SelectedItem.ToString();
            }
        }

        private void BtnPdfYukle_Click(object sender, EventArgs e)
        {
            try
            {
                string dosyaAdi = txtPdfDosyaAdi.Text.Trim();
                
                if (string.IsNullOrEmpty(dosyaAdi))
                {
                    MessageBox.Show("Lütfen bir PDF dosya adı girin veya yukarıdaki listeden seçin.", 
                        "Dosya Adı Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (!dosyaAdi.ToLower().EndsWith(".pdf"))
                {
                    MessageBox.Show("Dosya adı .pdf uzantısı ile bitmelidir.", 
                        "Geçersiz Dosya Türü", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // PDF yükleme simülasyonu
                selectedFilePath = "C:\\temp\\" + dosyaAdi;
                currentPdfFileName = dosyaAdi;
                
                // UI güncellemeleri
                lblPdfDurum.Text = $"✓ PDF YÜKLENDİ: {dosyaAdi} - İmza alanı seçmek için 'İmza Seçim Modu' butonuna tıklayın";
                lblPdfDurum.ForeColor = Color.Green;
                lblPdfDurum.BackColor = Color.LightYellow;
                
                // PDF görüntüleme alanını güncelle
                UpdatePdfViewer();
                
                MessageBox.Show($"✓ PDF BAŞARIYLA YÜKLENDİ!\n\nDosya: {dosyaAdi}\n\nİmza alanlarını seçmek için:\n1. 'İmza Seçim Modu' butonuna tıklayın\n2. PDF üzerinde fare ile alan seçin\n3. Seçilen alan otomatik olarak yetkili bilgilerine eklenecek", 
                    "PDF Yükleme Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblPdfDurum.Text = "HATA: " + ex.Message;
                lblPdfDurum.ForeColor = Color.Red;
                MessageBox.Show("PDF yükleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnZoomIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPdfFileName)) return;
            
            zoomFactor = Math.Min(zoomFactor * 1.25f, 5.0f); // Max %500
            UpdatePdfViewer();
            UpdateZoomLabel();
        }

        private void BtnZoomOut_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPdfFileName)) return;
            
            zoomFactor = Math.Max(zoomFactor / 1.25f, 0.25f); // Min %25
            UpdatePdfViewer();
            UpdateZoomLabel();
        }

        private void BtnFitToWidth_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPdfFileName)) return;
            
            // Container genişliğine göre zoom hesapla
            float containerWidth = pnlPdfContainer.Width - 20; // Padding için
            float baseWidth = 800; // PDF'in orijinal genişliği
            zoomFactor = containerWidth / baseWidth;
            
            UpdatePdfViewer();
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
                lblPdfDurum.Text = $"PDF: {currentPdfFileName} - İmza Seçim Modu Kapalı";
                lblPdfDurum.ForeColor = Color.Green;
            }
        }

        private void UpdateZoomLabel()
        {
            lblZoomLevel.Text = $"Zoom: {(zoomFactor * 100):F0}%";
        }

        private void UpdatePdfViewer()
        {
            if (string.IsNullOrEmpty(currentPdfFileName)) return;
            
            // PDF boyutunu zoom faktörüne göre ayarla
            int newWidth = (int)(800 * zoomFactor);
            int newHeight = (int)(600 * zoomFactor);
            
            pnlPdfViewer.Size = new Size(newWidth, newHeight);
            pnlPdfViewer.Invalidate(); // Yeniden çizim için
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                // İmza alanlarını kaydet
                if (signatureAreas.Count > 0)
                {
                    MessageBox.Show($"Kaydet işlemi başarılı!\n\n{signatureAreas.Count} adet imza alanı kaydedildi.\nPDF: {currentPdfFileName}", 
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
                            pnlPdfViewer.Invalidate(); // PDF görünümünü güncelle
                            
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
            
            selectionStart = e.Location;
            isSelecting = true;
        }

        private void PnlPdfViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isSelecting) return;
            
            currentSelection = new Rectangle(selectionStart, e.Location - selectionStart);
            pnlPdfViewer.Invalidate();
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
                pnlPdfViewer.Invalidate();
                return;
            }
            
            // Kişi bilgilerini al - Basit dialog ile
            string personName = GetInputFromUser("İmza sahibinin adını girin:", "Kişi Bilgisi", "");
            
            if (string.IsNullOrEmpty(personName))
            {
                currentSelection = Rectangle.Empty;
                pnlPdfViewer.Invalidate();
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
            
            lblPdfDurum.Text = $"✓ İMZA ALANI EKLENDİ: {personName} - {personTitle}";
            lblPdfDurum.ForeColor = Color.DarkGreen;
            
            currentSelection = Rectangle.Empty;
            pnlPdfViewer.Invalidate();
            
            MessageBox.Show($"İmza alanı başarıyla eklendi!\n\nKişi: {personName}\nÜnvan: {personTitle}\nYetki: {authority}\nKoordinat: X={newSignatureArea.Bounds.X}, Y={newSignatureArea.Bounds.Y}\nBoyut: {newSignatureArea.Bounds.Width}x{newSignatureArea.Bounds.Height}", 
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
            cmbOrrnekDosyalar.SelectedIndex = 0;
            currentPdfFileName = "";
            selectedFilePath = "";
            
            // İmza alanlarını temizle
            signatureAreas.Clear();
            dgvYetkililer.Rows.Clear();
            
            // PDF viewer'ı temizle
            isSignatureSelectionMode = false;
            zoomFactor = 1.0f;
            btnImzaSecimModu.Text = "İmza Seçim Modu";
            btnImzaSecimModu.BackColor = Color.LightGreen;
            
            lblPdfDurum.Text = "PDF dosyası henüz seçilmedi. Yukarıdan dosya adı girin veya örnek seçin.";
            lblPdfDurum.ForeColor = Color.Blue;
            lblPdfDurum.BackColor = SystemColors.Control;
            
            UpdateZoomLabel();
            pnlPdfViewer.Invalidate();
        }

        #endregion

        void IGatewayComponent.ProcessRequest(IContext objContext, string strAction)
        {
            // Gizmox gateway component interface implementation
        }
    }
} 
} 