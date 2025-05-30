using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common;
using PDFSystem.Models;
using PDFSystem.Services;

namespace PDFSystem.Forms
{
    public partial class SignatureCircularForm : Form
    {
        private int contractId;
        private SignatureCircularService signatureService;
        private ContractService contractService;

        // Controls
        private Panel pnlTop;
        private Panel pnlCenter;
        private Panel pnlBottom;
        private DataGridView dgvSignatureCirculars;
        private Button btnNewCircular;
        private Button btnViewPDF;
        private Button btnManageSignatures;
        private Button btnDelete;
        private Label lblContractInfo;
        private Label lblCirculars;

        public SignatureCircularForm(int contractId)
        {
            this.contractId = contractId;
            InitializeComponent();
            InitializeServices();
            LoadContractInfo();
            LoadSignatureCirculars();
        }

        private void InitializeServices()
        {
            signatureService = new SignatureCircularService();
            contractService = new ContractService();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Properties
            this.Text = "İmza Sirküler Yönetimi";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Top Panel
            pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 100;
            pnlTop.BackColor = Color.LightBlue;

            // Contract Info Label
            lblContractInfo = new Label();
            lblContractInfo.Location = new Point(10, 10);
            lblContractInfo.Size = new Size(800, 40);
            lblContractInfo.Font = new Font("Arial", 10, FontStyle.Bold);
            lblContractInfo.Text = "Kontrakt Bilgileri Yükleniyor...";

            // Circulars Label
            lblCirculars = new Label();
            lblCirculars.Text = "İmza Sirküler Listesi:";
            lblCirculars.Location = new Point(10, 60);
            lblCirculars.Size = new Size(150, 20);
            lblCirculars.Font = new Font("Arial", 9, FontStyle.Bold);

            pnlTop.Controls.Add(lblContractInfo);
            pnlTop.Controls.Add(lblCirculars);

            // Center Panel
            pnlCenter = new Panel();
            pnlCenter.Dock = DockStyle.Fill;

            // DataGridView for Signature Circulars
            dgvSignatureCirculars = new DataGridView();
            dgvSignatureCirculars.Dock = DockStyle.Fill;
            dgvSignatureCirculars.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSignatureCirculars.MultiSelect = false;
            dgvSignatureCirculars.ReadOnly = true;
            dgvSignatureCirculars.AllowUserToAddRows = false;
            dgvSignatureCirculars.AllowUserToDeleteRows = false;
            dgvSignatureCirculars.SelectionChanged += dgvSignatureCirculars_SelectionChanged;
            
            SetupDataGridViewColumns();

            pnlCenter.Controls.Add(dgvSignatureCirculars);

            // Bottom Panel
            pnlBottom = new Panel();
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 60;
            pnlBottom.BackColor = Color.LightGray;

            // New Circular Button
            btnNewCircular = new Button();
            btnNewCircular.Text = "Yeni İmza Sirküler";
            btnNewCircular.Location = new Point(10, 15);
            btnNewCircular.Size = new Size(120, 30);
            btnNewCircular.Click += btnNewCircular_Click;

            // View PDF Button
            btnViewPDF = new Button();
            btnViewPDF.Text = "PDF Görüntüle";
            btnViewPDF.Location = new Point(140, 15);
            btnViewPDF.Size = new Size(100, 30);
            btnViewPDF.Click += btnViewPDF_Click;
            btnViewPDF.Enabled = false;

            // Manage Signatures Button
            btnManageSignatures = new Button();
            btnManageSignatures.Text = "İmza Yönetimi";
            btnManageSignatures.Location = new Point(250, 15);
            btnManageSignatures.Size = new Size(100, 30);
            btnManageSignatures.Click += btnManageSignatures_Click;
            btnManageSignatures.Enabled = false;

            // Delete Button
            btnDelete = new Button();
            btnDelete.Text = "Sil";
            btnDelete.Location = new Point(360, 15);
            btnDelete.Size = new Size(80, 30);
            btnDelete.Click += btnDelete_Click;
            btnDelete.Enabled = false;
            btnDelete.BackColor = Color.LightCoral;

            pnlBottom.Controls.Add(btnNewCircular);
            pnlBottom.Controls.Add(btnViewPDF);
            pnlBottom.Controls.Add(btnManageSignatures);
            pnlBottom.Controls.Add(btnDelete);

            // Add panels to form
            this.Controls.Add(pnlCenter);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(pnlTop);

            this.ResumeLayout();
        }

        private void SetupDataGridViewColumns()
        {
            dgvSignatureCirculars.Columns.Clear();

            dgvSignatureCirculars.Columns.Add("SignatureCircularId", "ID");
            dgvSignatureCirculars.Columns["SignatureCircularId"].Visible = false;

            dgvSignatureCirculars.Columns.Add("CircularTitle", "Sirküler Başlığı");
            dgvSignatureCirculars.Columns["CircularTitle"].Width = 200;

            dgvSignatureCirculars.Columns.Add("Description", "Açıklama");
            dgvSignatureCirculars.Columns["Description"].Width = 250;

            dgvSignatureCirculars.Columns.Add("PDFFileName", "PDF Dosyası");
            dgvSignatureCirculars.Columns["PDFFileName"].Width = 150;

            dgvSignatureCirculars.Columns.Add("CreatedDate", "Oluşturma Tarihi");
            dgvSignatureCirculars.Columns["CreatedDate"].Width = 120;

            dgvSignatureCirculars.Columns.Add("DueDate", "Son Tarih");
            dgvSignatureCirculars.Columns["DueDate"].Width = 120;

            dgvSignatureCirculars.Columns.Add("Status", "Durum");
            dgvSignatureCirculars.Columns["Status"].Width = 100;

            dgvSignatureCirculars.Columns.Add("AssignmentCount", "İmza Sayısı");
            dgvSignatureCirculars.Columns["AssignmentCount"].Width = 100;
        }

        private void LoadContractInfo()
        {
            try
            {
                var contract = contractService.GetContractById(contractId);
                if (contract != null)
                {
                    lblContractInfo.Text = $"Kontrakt: {contract.ContractNumber} - {contract.ContractTitle}" +
                                          $"\nMüşteri: {contract.Customer?.CustomerName ?? "Bilinmiyor"}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kontrakt bilgileri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSignatureCirculars()
        {
            try
            {
                var circulars = signatureService.GetSignatureCircularsByContractId(contractId);
                dgvSignatureCirculars.Rows.Clear();

                foreach (var circular in circulars)
                {
                    var assignmentCount = signatureService.GetSignatureAssignmentCount(circular.SignatureCircularId);
                    dgvSignatureCirculars.Rows.Add(
                        circular.SignatureCircularId,
                        circular.CircularTitle,
                        circular.Description,
                        circular.PDFFileName,
                        circular.CreatedDate.ToShortDateString(),
                        circular.DueDate?.ToShortDateString() ?? "",
                        circular.Status,
                        assignmentCount
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İmza sirküler listesi yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSignatureCirculars_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dgvSignatureCirculars.SelectedRows.Count > 0;
            btnViewPDF.Enabled = hasSelection;
            btnManageSignatures.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
        }

        private void btnNewCircular_Click(object sender, EventArgs e)
        {
            var newCircularForm = new NewSignatureCircularForm(contractId);
            if (newCircularForm.ShowDialog() == DialogResult.OK)
            {
                LoadSignatureCirculars();
            }
        }

        private void btnViewPDF_Click(object sender, EventArgs e)
        {
            if (dgvSignatureCirculars.SelectedRows.Count == 0) return;

            var circularId = Convert.ToInt32(dgvSignatureCirculars.SelectedRows[0].Cells["SignatureCircularId"].Value);
            var pdfViewerForm = new PDFViewerForm(circularId);
            pdfViewerForm.ShowDialog();
        }

        private void btnManageSignatures_Click(object sender, EventArgs e)
        {
            if (dgvSignatureCirculars.SelectedRows.Count == 0) return;

            var circularId = Convert.ToInt32(dgvSignatureCirculars.SelectedRows[0].Cells["SignatureCircularId"].Value);
            var signatureManagementForm = new SignatureManagementForm(circularId);
            if (signatureManagementForm.ShowDialog() == DialogResult.OK)
            {
                LoadSignatureCirculars();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSignatureCirculars.SelectedRows.Count == 0) return;

            var result = MessageBox.Show("Seçili imza sirküler silinecek. Emin misiniz?", "Silme Onayı", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var circularId = Convert.ToInt32(dgvSignatureCirculars.SelectedRows[0].Cells["SignatureCircularId"].Value);
                    signatureService.DeleteSignatureCircular(circularId);
                    LoadSignatureCirculars();
                    MessageBox.Show("İmza sirküler başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme işlemi sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
} 