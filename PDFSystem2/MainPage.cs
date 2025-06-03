using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common;
using PDFSystem2.DataLayer;
using System.Linq;

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
        
        // PDF Upload - Gizmox OpenFileDialog kullanarak
        private GroupBox grpPdfUpload;
        private Button btnPdfYukle;
        private Label lblPdfDurum;
        private PictureBox picPdfPreview;
        private OpenFileDialog openFileDialog;  // Gizmox OpenFileDialog kontrolü
        private string selectedFilePath = "";   // Seçilen dosya yolu
        
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
            this.Text = "İmza Sirküleri Yönetim Sistemi";
            this.Size = new Size(1200, 800);
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
            tabMain.Size = new Size(1180, 700);
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
            grpFirmaBilgileri.Size = new Size(500, 320);
            
            // Firma Unvanı
            lblFirmaUnvani = new Label();
            lblFirmaUnvani.Text = "FIRMA UNVANI:";
            lblFirmaUnvani.Location = new Point(10, 25);
            lblFirmaUnvani.Size = new Size(120, 20);
            
            txtFirmaUnvani = new TextBox();
            txtFirmaUnvani.Location = new Point(140, 25);
            txtFirmaUnvani.Size = new Size(340, 20);
            
            // Firma Hesap Numarası
            lblFirmaHesapNo = new Label();
            lblFirmaHesapNo.Text = "FIRMA HESAP NO:";
            lblFirmaHesapNo.Location = new Point(10, 55);
            lblFirmaHesapNo.Size = new Size(120, 20);
            
            txtFirmaHesapNo = new TextBox();
            txtFirmaHesapNo.Location = new Point(140, 55);
            txtFirmaHesapNo.Size = new Size(200, 20);
            
            // Düzenlenme Tarihi
            lblDuzenlenmeTarihi = new Label();
            lblDuzenlenmeTarihi.Text = "DÜZENLENME TARİHİ:";
            lblDuzenlenmeTarihi.Location = new Point(10, 85);
            lblDuzenlenmeTarihi.Size = new Size(120, 20);
            
            dtpDuzenlenmeTarihi = new DateTimePicker();
            dtpDuzenlenmeTarihi.Location = new Point(140, 85);
            dtpDuzenlenmeTarihi.Size = new Size(200, 20);
            
            // Geçerlilik Tarihi
            lblGecerlilikTarihi = new Label();
            lblGecerlilikTarihi.Text = "GEÇERLİLİK TARİHİ:";
            lblGecerlilikTarihi.Location = new Point(10, 115);
            lblGecerlilikTarihi.Size = new Size(120, 20);
            
            dtpGecerlilikTarihi = new DateTimePicker();
            dtpGecerlilikTarihi.Location = new Point(140, 115);
            dtpGecerlilikTarihi.Size = new Size(200, 20);
            
            // Süresiz Geçerli CheckBox
            chkSuresizGecerli = new CheckBox();
            chkSuresizGecerli.Text = "SÜRESİZ GEÇERLİ";
            chkSuresizGecerli.Location = new Point(350, 115);
            chkSuresizGecerli.Size = new Size(130, 20);
            
            // Özel Durumlar
            lblOzelDurumlar = new Label();
            lblOzelDurumlar.Text = "ÖZEL DURUMLAR:";
            lblOzelDurumlar.Location = new Point(10, 145);
            lblOzelDurumlar.Size = new Size(120, 20);
            
            txtOzelDurumlar = new TextBox();
            txtOzelDurumlar.Multiline = true;
            txtOzelDurumlar.Location = new Point(140, 145);
            txtOzelDurumlar.Size = new Size(340, 60);
            
            // Noter İmza Sirküleri No
            lblNoterNo = new Label();
            lblNoterNo.Text = "NOTER İMZA SİRK. NO:";
            lblNoterNo.Location = new Point(10, 215);
            lblNoterNo.Size = new Size(120, 20);
            
            txtNoterNo = new TextBox();
            txtNoterNo.Location = new Point(140, 215);
            txtNoterNo.Size = new Size(200, 20);
            
            // Kullanıcı
            lblKullanici = new Label();
            lblKullanici.Text = "KULLANICI:";
            lblKullanici.Location = new Point(10, 245);
            lblKullanici.Size = new Size(120, 20);
            
            txtKullanici = new TextBox();
            txtKullanici.Location = new Point(140, 245);
            txtKullanici.Size = new Size(200, 20);
            
            // Açıklama
            lblAciklama = new Label();
            lblAciklama.Text = "AÇIKLAMA:";
            lblAciklama.Location = new Point(10, 275);
            lblAciklama.Size = new Size(120, 20);
            
            txtAciklama = new TextBox();
            txtAciklama.Location = new Point(140, 275);
            txtAciklama.Size = new Size(340, 20);
            
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

            // PDF Upload GroupBox - Gizmox OpenFileDialog kullanarak
            grpPdfUpload = new GroupBox();
            grpPdfUpload.Text = "PDF Yükleme ve İmza Seçimi";
            grpPdfUpload.Location = new Point(520, 10);
            grpPdfUpload.Size = new Size(640, 320);
            
            // OpenFileDialog oluştur
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF Dosyaları (*.pdf)|*.pdf|Tüm Dosyalar (*.*)|*.*";
            openFileDialog.Title = "PDF Dosyası Seç";
            openFileDialog.Multiselect = false;
            
            btnPdfYukle = new Button();
            btnPdfYukle.Text = "PDF Dosyası Seç";
            btnPdfYukle.Location = new Point(10, 25);
            btnPdfYukle.Size = new Size(150, 30);
            btnPdfYukle.BackColor = Color.LightBlue;
            
            lblPdfDurum = new Label();
            lblPdfDurum.Text = "PDF dosyası seçilmedi.";
            lblPdfDurum.Location = new Point(10, 65);
            lblPdfDurum.Size = new Size(620, 20);
            lblPdfDurum.ForeColor = Color.Blue;
            
            picPdfPreview = new PictureBox();
            picPdfPreview.Location = new Point(10, 95);
            picPdfPreview.Size = new Size(620, 215);
            picPdfPreview.BackColor = Color.LightGray;
            picPdfPreview.BorderStyle = BorderStyle.FixedSingle;
            
            // Preview alanına bilgi etiketi ekle
            Label lblPreviewInfo = new Label();
            lblPreviewInfo.Text = "PDF ÖNIZLEME ALANI\n(Seçilen PDF dosyası burada görüntülenecek)";
            lblPreviewInfo.Location = new Point(200, 90);
            lblPreviewInfo.Size = new Size(300, 40);
            lblPreviewInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblPreviewInfo.ForeColor = Color.Gray;
            picPdfPreview.Controls.Add(lblPreviewInfo);
            
            grpPdfUpload.Controls.AddRange(new Control[]
            {
                btnPdfYukle, lblPdfDurum, picPdfPreview
            });

            // Yetkili Bilgileri GroupBox
            grpYetkiliBilgileri = new GroupBox();
            grpYetkiliBilgileri.Text = "YETKİLİ BİLGİLERİ";
            grpYetkiliBilgileri.Location = new Point(10, 340);
            grpYetkiliBilgileri.Size = new Size(1150, 300);
            
            dgvYetkililer = new DataGridView();
            dgvYetkililer.Location = new Point(10, 25);
            dgvYetkililer.Size = new Size(1130, 265);
            dgvYetkililer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvYetkililer.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvYetkililer.AllowUserToAddRows = false;
            
            grpYetkiliBilgileri.Controls.Add(dgvYetkililer);
            
            // Ana tab'a ekle
            tabGenelBilgiler.Controls.AddRange(new Control[]
            {
                grpFirmaBilgileri, grpPdfUpload, grpYetkiliBilgileri
            });
        }

        private void CreateIslemTurleriTab()
        {
            dgvIslemTurleri = new DataGridView();
            dgvIslemTurleri.Location = new Point(10, 10);
            dgvIslemTurleri.Size = new Size(1150, 630);
            dgvIslemTurleri.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvIslemTurleri.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvIslemTurleri.AllowUserToAddRows = false;
            
            tabIslemTurleri.Controls.Add(dgvIslemTurleri);
        }

        private void CreateYetkiTurleriTab()
        {
            dgvYetkiTurleri = new DataGridView();
            dgvYetkiTurleri.Location = new Point(10, 10);
            dgvYetkiTurleri.Size = new Size(1150, 630);
            dgvYetkiTurleri.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvYetkiTurleri.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvYetkiTurleri.AllowUserToAddRows = false;
            
            tabYetkiTurleri.Controls.Add(dgvYetkiTurleri);
        }

        private void CreateButtons()
        {
            btnKaydet = new Button();
            btnKaydet.Text = "Kaydet";
            btnKaydet.Location = new Point(10, 720);
            btnKaydet.Size = new Size(100, 35);
            btnKaydet.BackColor = Color.Green;
            btnKaydet.ForeColor = Color.White;
            
            btnYeni = new Button();
            btnYeni.Text = "Yeni";
            btnYeni.Location = new Point(120, 720);
            btnYeni.Size = new Size(100, 35);
            btnYeni.BackColor = Color.Blue;
            btnYeni.ForeColor = Color.White;
            
            btnSil = new Button();
            btnSil.Text = "Sil";
            btnSil.Location = new Point(230, 720);
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
            btnPdfYukle.Click += BtnPdfYukle_Click;
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
                // Gizmox OpenFileDialog kullan
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
                    string fileName = System.IO.Path.GetFileName(selectedFilePath);
                    
                    // PDF dosyası kontrolü
                    if (!fileName.ToLower().EndsWith(".pdf"))
                    {
                        MessageBox.Show("Lütfen sadece PDF dosyası seçin.", 
                            "Geçersiz Dosya Türü", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    // Dosya bilgilerini göster
                    lblPdfDurum.Text = string.Format("Seçilen PDF: {0}", fileName);
                    lblPdfDurum.ForeColor = Color.Green;
                    
                    // Başarı mesajı
                    MessageBox.Show(string.Format("PDF dosyası başarıyla seçildi!\n\nDosya: {0}\n\nİmza koordinatlarını seçmek için PDF üzerine tıklayabilirsiniz.", fileName), 
                        "Dosya Seçimi Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                    // Preview alanını güncelle
                    UpdatePdfPreview(fileName);
                }
            }
            catch (Exception ex)
            {
                lblPdfDurum.Text = "Dosya seçiminde hata oluştu: " + ex.Message;
                lblPdfDurum.ForeColor = Color.Red;
                MessageBox.Show("Dosya seçiminde hata oluştu: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdatePdfPreview(string fileName)
        {
            // Preview alanını temizle ve güncelle
            picPdfPreview.Controls.Clear();
            
            // Preview alanının arka plan rengini değiştir
            picPdfPreview.BackColor = Color.White;
            
            // PDF bilgi etiketi - daha görünür şekilde
            Label lblPdfInfo = new Label();
            lblPdfInfo.Text = string.Format("✓ PDF YÜKLENDİ ✓\n\n{0}\n\n(Gerçek uygulamada PDF içeriği burada görüntülenecek)\n\n⬇ İmza koordinatları seçmek için buraya tıklayın ⬇", fileName);
            lblPdfInfo.Location = new Point(10, 10);  // Sol üst köşeden başlat
            lblPdfInfo.Size = new Size(600, 195);     // Tüm alanı kapla
            lblPdfInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblPdfInfo.ForeColor = Color.DarkBlue;
            lblPdfInfo.BackColor = Color.LightCyan;   // Arka plan rengi ekle
            lblPdfInfo.BorderStyle = BorderStyle.FixedSingle;
            lblPdfInfo.Font = new Font("Arial", 12, FontStyle.Bold);
            
            // İmza koordinat simülasyonu için tıklama eventi
            picPdfPreview.Click += PicPdfPreview_Click;
            lblPdfInfo.Click += PicPdfPreview_Click;  // Label'a da tıklama eventi ekle
            picPdfPreview.Cursor = Cursors.Hand;
            lblPdfInfo.Cursor = Cursors.Hand;
            
            picPdfPreview.Controls.Add(lblPdfInfo);
            
            // Ekstra bilgi için küçük bir label daha ekle
            Label lblInstructionInfo = new Label();
            lblInstructionInfo.Text = "PDF başarıyla yüklendi - Koordinat seçimi için tıklayın";
            lblInstructionInfo.Location = new Point(170, 25);
            lblInstructionInfo.Size = new Size(280, 15);
            lblInstructionInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblInstructionInfo.ForeColor = Color.Green;
            lblInstructionInfo.BackColor = Color.Transparent;
            lblInstructionInfo.Font = new Font("Arial", 8, FontStyle.Regular);
            
            picPdfPreview.Controls.Add(lblInstructionInfo);
        }
        
        private void PicPdfPreview_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                MouseEventArgs mouseArgs = e as MouseEventArgs;
                if (mouseArgs != null)
                {
                    // Koordinatları hesapla
                    int x = mouseArgs.X;
                    int y = mouseArgs.Y;
                    
                    // Koordinat seçimi mesajı
                    string message = string.Format("✓ İMZA KOORDİNATI SEÇİLDİ!\n\nDosya: {0}\n\nKoordinatlar:\nX: {1} piksel\nY: {2} piksel\n\n(Bu koordinatlar veritabanına kaydedilecek)\n\nBaşka bir nokta seçmek için tekrar tıklayabilirsiniz.", 
                        System.IO.Path.GetFileName(selectedFilePath), x, y);
                    
                    MessageBox.Show(message, "İmza Koordinatı Seçildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Seçilen koordinatı preview'da göster
                    ShowSelectedCoordinate(x, y);
                }
            }
            else
            {
                MessageBox.Show("Önce bir PDF dosyası seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void ShowSelectedCoordinate(int x, int y)
        {
            // Önceki koordinat işaretini temizle
            var existingMarkers = picPdfPreview.Controls.OfType<Label>().Where(l => l.Name == "coordinateMarker").ToList();
            foreach (var marker in existingMarkers)
            {
                picPdfPreview.Controls.Remove(marker);
            }
            
            // Yeni koordinat işareti ekle
            Label coordinateMarker = new Label();
            coordinateMarker.Name = "coordinateMarker";
            coordinateMarker.Text = "✗";
            coordinateMarker.Location = new Point(x - 5, y - 5);
            coordinateMarker.Size = new Size(10, 10);
            coordinateMarker.ForeColor = Color.Red;
            coordinateMarker.BackColor = Color.Yellow;
            coordinateMarker.Font = new Font("Arial", 8, FontStyle.Bold);
            coordinateMarker.TextAlign = ContentAlignment.MiddleCenter;
            
            picPdfPreview.Controls.Add(coordinateMarker);
            coordinateMarker.BringToFront();
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            // UI test - sadece mesaj göster
            MessageBox.Show("Kaydet butonu çalışıyor! (Data servisleri henüz bağlanmadı)", "UI Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnYeni_Click(object sender, EventArgs e)
        {
            ClearForm();
            MessageBox.Show("Form temizlendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Silme işlemi UI testinde çalışıyor. Onaylıyor musunuz?", "UI Test", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MessageBox.Show("Sil butonu çalışıyor! (Data servisleri henüz bağlanmadı)", "UI Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ChkSuresizGecerli_CheckedChanged(object sender, EventArgs e)
        {
            dtpGecerlilikTarihi.Enabled = !chkSuresizGecerli.Checked;
        }

        #endregion

        #region Sample Data for UI Testing

        private void LoadSampleData()
        {
            // Sadece UI test için örnek veriler
            txtFirmaUnvani.Text = "şirket aş";
            txtFirmaHesapNo.Text = "9999";
            dtpDuzenlenmeTarihi.Value = new DateTime(2023, 12, 20);
            dtpGecerlilikTarihi.Value = new DateTime(2024, 12, 20);
            txtNoterNo.Text = "30903";
            txtKullanici.Text = "Test Kullanıcı";
            txtAciklama.Text = "UI Test için örnek açıklama";
            txtOzelDurumlar.Text = "Süre belirtilmemiş ve süresiz geçerli işlemler için test verisi";
            
            // DataGridView'lere örnek data yükle
            LoadYetkililerData();
            LoadIslemTurleriData();
            LoadYetkiTurleriData();
        }

        private void LoadYetkililerData()
        {
            try
            {
                var circularDetails = _mockDataService.GetCircularDetails();
                
                // DataGridView sütunlarını oluştur
                dgvYetkililer.Columns.Clear();
                dgvYetkililer.Columns.Add("ID", "ID");
                dgvYetkililer.Columns.Add("ADI_SOYADI", "ADI SOYADI");
                dgvYetkililer.Columns.Add("YETKI_SEKLI", "YETKİ ŞEKLİ");
                dgvYetkililer.Columns.Add("YETKI_SURE", "YETKİ SÜRESİ");
                dgvYetkililer.Columns.Add("IMZA_YETKI_GRUBU", "İMZA YETKİ GRUBU");
                dgvYetkililer.Columns.Add("YETKI_OLDUGU_ISLEMLER", "YETKİ OLDUĞU İŞLEMLER");
                
                // Gizle ID sütununu
                dgvYetkililer.Columns["ID"].Visible = false;
                
                // Verileri yükle
                dgvYetkililer.Rows.Clear();
                foreach (var detail in circularDetails)
                {
                    dgvYetkililer.Rows.Add(
                        detail.ID,
                        detail.ADI_SOYADI,
                        detail.YETKI_SEKLI,
                        detail.YETKI_SURE,
                        detail.IMZA_YETKI_GRUBU,
                        detail.YETKI_OLDUGU_ISLEMLER
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yetkili bilgileri yüklenirken hata: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadIslemTurleriData()
        {
            try
            {
                var operations = _mockDataService.GetOperations();
                
                // DataGridView sütunlarını oluştur
                dgvIslemTurleri.Columns.Clear();
                dgvIslemTurleri.Columns.Add("ID", "ID");
                dgvIslemTurleri.Columns.Add("OPERATION_CODE", "İŞLEM KODU");
                dgvIslemTurleri.Columns.Add("OPERATION_TYPE", "İŞLEM TÜRÜ");
                dgvIslemTurleri.Columns.Add("KAYIT_TARIHI", "KAYIT TARİHİ");
                
                // Gizle ID sütununu
                dgvIslemTurleri.Columns["ID"].Visible = false;
                
                // Verileri yükle
                dgvIslemTurleri.Rows.Clear();
                foreach (var operation in operations)
                {
                    dgvIslemTurleri.Rows.Add(
                        operation.ID,
                        operation.OPERATION_CODE,
                        operation.OPERATION_TYPE,
                        operation.KAYIT_TARIHI.ToString("dd/MM/yyyy")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem türleri yüklenirken hata: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadYetkiTurleriData()
        {
            try
            {
                var roleTypes = _mockDataService.GetRoleTypes();
                
                // DataGridView sütunlarını oluştur
                dgvYetkiTurleri.Columns.Clear();
                dgvYetkiTurleri.Columns.Add("ID", "ID");
                dgvYetkiTurleri.Columns.Add("ROLE_GROUP", "YETKİ GRUBU");
                dgvYetkiTurleri.Columns.Add("ROLE_TYPE", "YETKİ TÜRÜ");
                dgvYetkiTurleri.Columns.Add("MIN_SIGNATURE_COUNT", "MIN İMZA SAYISI");
                dgvYetkiTurleri.Columns.Add("KAYIT_TARIHI", "KAYIT TARİHİ");
                
                // Gizle ID sütununu
                dgvYetkiTurleri.Columns["ID"].Visible = false;
                
                // Verileri yükle
                dgvYetkiTurleri.Rows.Clear();
                foreach (var roleType in roleTypes)
                {
                    dgvYetkiTurleri.Rows.Add(
                        roleType.ID,
                        roleType.ROLE_GROUP,
                        roleType.ROLE_TYPE,
                        roleType.MIN_SIGNATURE_COUNT,
                        roleType.KAYIT_TARIHI.ToString("dd/MM/yyyy")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yetki türleri yüklenirken hata: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtFirmaUnvani.Clear();
            txtFirmaHesapNo.Clear();
            dtpDuzenlenmeTarihi.Value = DateTime.Now;
            dtpGecerlilikTarihi.Value = DateTime.Now;
            chkSuresizGecerli.Checked = false;
            txtOzelDurumlar.Clear();
            txtNoterNo.Clear();
            txtKullanici.Clear();
            txtAciklama.Clear();
            selectedFilePath = "";
            lblPdfDurum.Text = "PDF dosyası seçilmedi.";
            lblPdfDurum.ForeColor = Color.Blue;
            
            // Preview alanını temizle
            picPdfPreview.Controls.Clear();
            picPdfPreview.Click -= PicPdfPreview_Click;
            picPdfPreview.Cursor = Cursors.Default;
            
            Label lblPreviewInfo = new Label();
            lblPreviewInfo.Text = "PDF ÖNIZLEME ALANI\n(Seçilen PDF dosyası burada görüntülenecek)";
            lblPreviewInfo.Location = new Point(200, 90);
            lblPreviewInfo.Size = new Size(300, 40);
            lblPreviewInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblPreviewInfo.ForeColor = Color.Gray;
            picPdfPreview.Controls.Add(lblPreviewInfo);
        }

        #endregion

        #region Component Interface

        void IGatewayComponent.ProcessRequest(IContext objContext, string strAction)
        {
            // Gizmox Gateway işlemleri
        }

        #endregion
    }
} 