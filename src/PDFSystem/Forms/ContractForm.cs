using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Interfaces;
using Gizmox.WebGUI.Common.Resources;

namespace PDFSystem.Forms
{
    public class ContractForm : Form
    {
        private Panel pnlMain;
        private GroupBox grpContractDetails;
        private Label lblContractNo;
        private TextBox txtContractNo;
        private Label lblContractDate;
        private DateTimePicker dtpContractDate;
        private Label lblContractType;
        private ComboBox cmbContractType;
        private Label lblDescription;
        private TextBox txtDescription;
        private Label lblStatus;
        private ComboBox cmbStatus;
        private Label lblCustomer;
        private ComboBox cmbCustomer;
        private Button btnSave;
        private Button btnCancel;
        private Button btnUploadPDF;
        private string uploadedPdfPath;

        public ContractForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.pnlMain = new Panel();
            this.grpContractDetails = new GroupBox();
            this.lblContractNo = new Label();
            this.txtContractNo = new TextBox();
            this.lblContractDate = new Label();
            this.dtpContractDate = new DateTimePicker();
            this.lblContractType = new Label();
            this.cmbContractType = new ComboBox();
            this.lblDescription = new Label();
            this.txtDescription = new TextBox();
            this.lblStatus = new Label();
            this.cmbStatus = new ComboBox();
            this.lblCustomer = new Label();
            this.cmbCustomer = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.btnUploadPDF = new Button();

            // Form
            this.Text = "Kontrat Formu";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main Panel
            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlMain.Padding = new Padding(10);

            // Contract Details Group
            this.grpContractDetails.Text = "Kontrat Bilgileri";
            this.grpContractDetails.Location = new Point(10, 10);
            this.grpContractDetails.Size = new Size(560, 380);

            // Contract No
            this.lblContractNo.Text = "Kontrat No:";
            this.lblContractNo.Location = new Point(20, 30);
            this.lblContractNo.Size = new Size(100, 23);

            this.txtContractNo.Location = new Point(130, 27);
            this.txtContractNo.Size = new Size(200, 23);

            // Contract Date
            this.lblContractDate.Text = "Kontrat Tarihi:";
            this.lblContractDate.Location = new Point(20, 60);
            this.lblContractDate.Size = new Size(100, 23);

            this.dtpContractDate.Location = new Point(130, 57);
            this.dtpContractDate.Size = new Size(200, 23);
            this.dtpContractDate.Format = DateTimePickerFormat.Short;

            // Contract Type
            this.lblContractType.Text = "Kontrat Tipi:";
            this.lblContractType.Location = new Point(20, 90);
            this.lblContractType.Size = new Size(100, 23);

            this.cmbContractType.Location = new Point(130, 87);
            this.cmbContractType.Size = new Size(200, 23);
            this.cmbContractType.Items.AddRange(new object[] {
                "Satış Sözleşmesi",
                "Hizmet Sözleşmesi",
                "Bakım Sözleşmesi",
                "Diğer"
            });

            // Customer
            this.lblCustomer.Text = "Müşteri:";
            this.lblCustomer.Location = new Point(20, 120);
            this.lblCustomer.Size = new Size(100, 23);

            this.cmbCustomer.Location = new Point(130, 117);
            this.cmbCustomer.Size = new Size(200, 23);

            // Description
            this.lblDescription.Text = "Açıklama:";
            this.lblDescription.Location = new Point(20, 150);
            this.lblDescription.Size = new Size(100, 23);

            this.txtDescription.Location = new Point(130, 147);
            this.txtDescription.Size = new Size(400, 100);
            this.txtDescription.Multiline = true;

            // Status
            this.lblStatus.Text = "Durum:";
            this.lblStatus.Location = new Point(20, 260);
            this.lblStatus.Size = new Size(100, 23);

            this.cmbStatus.Location = new Point(130, 257);
            this.cmbStatus.Size = new Size(200, 23);
            this.cmbStatus.Items.AddRange(new object[] {
                "Aktif",
                "Pasif",
                "İptal Edildi",
                "Süresi Doldu"
            });

            // Upload PDF Button
            this.btnUploadPDF.Text = "PDF Yükle";
            this.btnUploadPDF.Location = new Point(130, 300);
            this.btnUploadPDF.Size = new Size(100, 30);
            this.btnUploadPDF.Click += new EventHandler(BtnUploadPDF_Click);

            // Save Button
            this.btnSave.Text = "Kaydet";
            this.btnSave.Location = new Point(380, 400);
            this.btnSave.Size = new Size(90, 30);
            this.btnSave.Click += new EventHandler(BtnSave_Click);

            // Cancel Button
            this.btnCancel.Text = "İptal";
            this.btnCancel.Location = new Point(480, 400);
            this.btnCancel.Size = new Size(90, 30);
            this.btnCancel.Click += new EventHandler(BtnCancel_Click);

            // Add controls to group
            this.grpContractDetails.Controls.AddRange(new Control[] {
                this.lblContractNo,
                this.txtContractNo,
                this.lblContractDate,
                this.dtpContractDate,
                this.lblContractType,
                this.cmbContractType,
                this.lblCustomer,
                this.cmbCustomer,
                this.lblDescription,
                this.txtDescription,
                this.lblStatus,
                this.cmbStatus,
                this.btnUploadPDF
            });

            // Add controls to form
            this.pnlMain.Controls.Add(this.grpContractDetails);
            this.pnlMain.Controls.Add(this.btnSave);
            this.pnlMain.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pnlMain);
        }

        private async void BtnUploadPDF_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF Dosyaları (*.pdf)|*.pdf";
                openFileDialog.Title = "Kontrat PDF Dosyası Seç";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    if (fileExtension != ".pdf")
                    {
                        MessageBox.Show("Lütfen sadece PDF dosyası seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // PDF dosyasını geçici bir konuma kaydet
                    string tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName));
                    File.Copy(fileName, tempPath, true);
                    uploadedPdfPath = tempPath;

                    MessageBox.Show("PDF dosyası başarıyla yüklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PDF yükleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                    {
                        await conn.OpenAsync();

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO Contracts (ContractNo, ContractDate, ContractType, CustomerId, Description, Status, PDFPath) VALUES (@ContractNo, @ContractDate, @ContractType, @CustomerId, @Description, @Status, @PDFPath)", conn))
                        {
                            cmd.Parameters.AddWithValue("@ContractNo", txtContractNo.Text);
                            cmd.Parameters.AddWithValue("@ContractDate", dtpContractDate.Value);
                            cmd.Parameters.AddWithValue("@ContractType", cmbContractType.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@CustomerId", ((KeyValuePair<int, string>)cmbCustomer.SelectedItem).Key);
                            cmd.Parameters.AddWithValue("@Description", txtDescription.Text);
                            cmd.Parameters.AddWithValue("@Status", cmbStatus.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@PDFPath", uploadedPdfPath ?? (object)DBNull.Value);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    MessageBox.Show("Kontrat başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(txtContractNo.Text))
            {
                MessageBox.Show("Kontrat numarası boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir müşteri seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbContractType.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir kontrat tipi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir durum seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        public async void LoadCustomers()
        {
            try
            {
                cmbCustomer.Items.Clear();
                using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("SELECT CustomerId, CustomerName FROM Customers WHERE Status = 'Aktif'", conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int customerId = reader.GetInt32(0);
                                string customerName = reader.GetString(1);
                                cmbCustomer.Items.Add(new KeyValuePair<int, string>(customerId, customerName));
                            }
                        }
                    }
                }
                cmbCustomer.DisplayMember = "Value";
                cmbCustomer.ValueMember = "Key";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Müşteri yükleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async void LoadContract(string contractNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Contracts WHERE ContractNo = @ContractNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@ContractNo", contractNo);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                txtContractNo.Text = reader["ContractNo"].ToString();
                                dtpContractDate.Value = reader.GetDateTime(reader.GetOrdinal("ContractDate"));
                                cmbContractType.SelectedItem = reader["ContractType"].ToString();
                                txtDescription.Text = reader["Description"].ToString();
                                cmbStatus.SelectedItem = reader["Status"].ToString();
                                uploadedPdfPath = reader["PDFPath"].ToString();

                                // Müşteriyi seç
                                int customerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                                foreach (KeyValuePair<int, string> item in cmbCustomer.Items)
                                {
                                    if (item.Key == customerId)
                                    {
                                        cmbCustomer.SelectedItem = item;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kontrat yükleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 