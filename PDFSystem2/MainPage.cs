using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Gizmox.WebGUI.Forms;
using PDFSystem2.DataLayer;
using PDFSystem2.Models;

namespace PDFSystem2
{
    public partial class MainPage : Form, IGatewayComponent
    {
        #region Private Members

        private MockDataService _mockDataService;
        private int _currentCircularId = 1;

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
        
        // PDF Upload
        private GroupBox grpPdfUpload;
        private Button btnPdfYukle;
        private Label lblPdfDurum;
        private PictureBox picPdfPreview;
        
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
            InitializeComponent();
            InitializeForm();
            _mockDataService = new MockDataService();
            LoadMockData();
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
            
            // GroupBox'a kontrolleri ekle
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

            // PDF Upload GroupBox
            grpPdfUpload = new GroupBox();
            grpPdfUpload.Text = "PDF Yükleme ve İmza Seçimi";
            grpPdfUpload.Location = new Point(520, 10);
            grpPdfUpload.Size = new Size(640, 320);
            
            btnPdfYukle = new Button();
            btnPdfYukle.Text = "PDF Yükle";
            btnPdfYukle.Location = new Point(10, 25);
            btnPdfYukle.Size = new Size(100, 30);
            
            lblPdfDurum = new Label();
            lblPdfDurum.Text = "PDF dosyası seçilmedi.";
            lblPdfDurum.Location = new Point(120, 32);
            lblPdfDurum.Size = new Size(300, 20);
            
            picPdfPreview = new PictureBox();
            picPdfPreview.Location = new Point(10, 65);
            picPdfPreview.Size = new Size(620, 240);
            picPdfPreview.BackColor = Color.LightGray;
            picPdfPreview.BorderStyle = BorderStyle.FixedSingle;
            
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
            openFileDialog.Title = "PDF Dosyası Seçin";
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                lblPdfDurum.Text = $"Seçilen dosya: {openFileDialog.FileName}";
                // PDF önizleme ve imza seçimi işlemleri burada implement edilecek
                MessageBox.Show("PDF yüklendi. İmza alanlarını seçebilirsiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                SaveCurrentData();
                MessageBox.Show("Veriler başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnYeni_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Seçili kaydı silmek istediğinizden emin misiniz?", "Silme İşlemi", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DeleteCurrentRecord();
            }
        }

        private void ChkSuresizGecerli_CheckedChanged(object sender, EventArgs e)
        {
            dtpGecerlilikTarihi.Enabled = !chkSuresizGecerli.Checked;
        }

        #endregion

        #region Data Methods

        private void LoadMockData()
        {
            try
            {
                LoadCircularData();
                LoadYetkililerData();
                LoadIslemTurleriData();
                LoadYetkiTurleriData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yükleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCircularData()
        {
            var circular = _mockDataService.GetCircularById(_currentCircularId);
            if (circular != null)
            {
                txtFirmaUnvani.Text = circular.FIRMA_UNVANI;
                txtFirmaHesapNo.Text = circular.FIRMA_HESAP_NUMARASI;
                dtpDuzenlenmeTarihi.Value = circular.IMZA_SIRKULERI_DUZENLEME_TARIHI ?? DateTime.Now;
                dtpGecerlilikTarihi.Value = circular.IMZA_SIRKULERI_GECERLILIK_TARIHI ?? DateTime.Now;
                chkSuresizGecerli.Checked = circular.SURESIZ_GECERLI;
                txtOzelDurumlar.Text = circular.OZEL_DURUMLAR;
                txtNoterNo.Text = circular.NOTER_IMZA_SIRKULERI_NO;
                txtKullanici.Text = circular.KULLANICI;
                txtAciklama.Text = circular.ACIKLAMA;
            }
        }

        private void LoadYetkililerData()
        {
            var details = _mockDataService.GetCircularDetails(_currentCircularId);
            dgvYetkililer.DataSource = details;
            
            if (dgvYetkililer.Columns.Count > 0)
            {
                dgvYetkililer.Columns["ID"].Visible = false;
                dgvYetkililer.Columns["SGN_CIRCULAR_ID"].Visible = false;
                dgvYetkililer.Columns["AKTIF_PASIF"].Visible = false;
                dgvYetkililer.Columns["KAYIT_TARIHI"].Visible = false;
                dgvYetkililer.Columns["IMZA_KOORDINAT_X"].Visible = false;
                dgvYetkililer.Columns["IMZA_KOORDINAT_Y"].Visible = false;
                dgvYetkililer.Columns["IMZA_KOORDINAT_WIDTH"].Visible = false;
                dgvYetkililer.Columns["IMZA_KOORDINAT_HEIGHT"].Visible = false;
                dgvYetkililer.Columns["IMZA_GORUNTUSU"].Visible = false;
            }
        }

        private void LoadIslemTurleriData()
        {
            var operations = _mockDataService.GetOperations(_currentCircularId);
            dgvIslemTurleri.DataSource = operations;
            
            if (dgvIslemTurleri.Columns.Count > 0)
            {
                dgvIslemTurleri.Columns["ID"].Visible = false;
                dgvIslemTurleri.Columns["SGN_CIRCULAR_ID"].Visible = false;
                dgvIslemTurleri.Columns["AKTIF_PASIF"].Visible = false;
            }
        }

        private void LoadYetkiTurleriData()
        {
            var roleTypes = _mockDataService.GetRoleTypes(_currentCircularId);
            dgvYetkiTurleri.DataSource = roleTypes;
            
            if (dgvYetkiTurleri.Columns.Count > 0)
            {
                dgvYetkiTurleri.Columns["ID"].Visible = false;
                dgvYetkiTurleri.Columns["SGN_CIRCULAR_ID"].Visible = false;
                dgvYetkiTurleri.Columns["AKTIF_PASIF"].Visible = false;
            }
        }

        private void SaveCurrentData()
        {
            var circular = new SgnCircular
            {
                ID = _currentCircularId,
                FIRMA_UNVANI = txtFirmaUnvani.Text,
                FIRMA_HESAP_NUMARASI = txtFirmaHesapNo.Text,
                IMZA_SIRKULERI_DUZENLEME_TARIHI = dtpDuzenlenmeTarihi.Value,
                IMZA_SIRKULERI_GECERLILIK_TARIHI = chkSuresizGecerli.Checked ? null : (DateTime?)dtpGecerlilikTarihi.Value,
                SURESIZ_GECERLI = chkSuresizGecerli.Checked,
                OZEL_DURUMLAR = txtOzelDurumlar.Text,
                NOTER_IMZA_SIRKULERI_NO = txtNoterNo.Text,
                KULLANICI = txtKullanici.Text,
                ACIKLAMA = txtAciklama.Text
            };

            _mockDataService.UpdateCircular(circular);
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
            lblPdfDurum.Text = "PDF dosyası seçilmedi.";
        }

        private void DeleteCurrentRecord()
        {
            _mockDataService.DeleteCircular(_currentCircularId);
            ClearForm();
            dgvYetkililer.DataSource = null;
            dgvIslemTurleri.DataSource = null;
            dgvYetkiTurleri.DataSource = null;
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