using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common;
using PDFSystem.Models;
using PDFSystem.Services;

namespace PDFSystem.Forms
{
    public partial class CustomerContractsForm : Form
    {
        private ComboBox cmbCustomers;
        private DataGridView dgvContracts;
        private Button btnNewContract;
        private Button btnViewSignatures;
        private Button btnRefresh;
        private Label lblCustomer;
        private Label lblContracts;
        private Panel pnlControls;
        private Panel pnlGrid;

        private CustomerService customerService;
        private ContractService contractService;
        private Customer selectedCustomer;

        public CustomerContractsForm()
        {
            InitializeComponent();
            InitializeServices();
            LoadCustomers();
        }

        private void InitializeServices()
        {
            customerService = new CustomerService();
            contractService = new ContractService();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form Properties
            this.Text = "Müşteri Kontraktları ve İmza Sirküler Yönetimi";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Control Panel
            pnlControls = new Panel();
            pnlControls.Dock = DockStyle.Top;
            pnlControls.Height = 80;
            pnlControls.BackColor = Color.LightGray;

            // Customer Label
            lblCustomer = new Label();
            lblCustomer.Text = "Müşteri Seçin:";
            lblCustomer.Location = new Point(10, 15);
            lblCustomer.Size = new Size(80, 20);

            // Customer ComboBox
            cmbCustomers = new ComboBox();
            cmbCustomers.Location = new Point(100, 12);
            cmbCustomers.Size = new Size(200, 25);
            cmbCustomers.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCustomers.SelectedIndexChanged += cmbCustomers_SelectedIndexChanged;

            // Refresh Button
            btnRefresh = new Button();
            btnRefresh.Text = "Yenile";
            btnRefresh.Location = new Point(320, 12);
            btnRefresh.Size = new Size(80, 25);
            btnRefresh.Click += btnRefresh_Click;

            // New Contract Button
            btnNewContract = new Button();
            btnNewContract.Text = "Yeni Kontrakt";
            btnNewContract.Location = new Point(420, 12);
            btnNewContract.Size = new Size(100, 25);
            btnNewContract.Click += btnNewContract_Click;

            // View Signatures Button
            btnViewSignatures = new Button();
            btnViewSignatures.Text = "İmza Sirküler";
            btnViewSignatures.Location = new Point(540, 12);
            btnViewSignatures.Size = new Size(100, 25);
            btnViewSignatures.Click += btnViewSignatures_Click;
            btnViewSignatures.Enabled = false;

            // Contracts Label
            lblContracts = new Label();
            lblContracts.Text = "Kontraktlar:";
            lblContracts.Location = new Point(10, 50);
            lblContracts.Size = new Size(80, 20);

            // Add controls to panel
            pnlControls.Controls.Add(lblCustomer);
            pnlControls.Controls.Add(cmbCustomers);
            pnlControls.Controls.Add(btnRefresh);
            pnlControls.Controls.Add(btnNewContract);
            pnlControls.Controls.Add(btnViewSignatures);
            pnlControls.Controls.Add(lblContracts);

            // Grid Panel
            pnlGrid = new Panel();
            pnlGrid.Dock = DockStyle.Fill;

            // Contracts DataGridView
            dgvContracts = new DataGridView();
            dgvContracts.Dock = DockStyle.Fill;
            dgvContracts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvContracts.MultiSelect = false;
            dgvContracts.ReadOnly = true;
            dgvContracts.AllowUserToAddRows = false;
            dgvContracts.AllowUserToDeleteRows = false;
            dgvContracts.SelectionChanged += dgvContracts_SelectionChanged;
            
            SetupDataGridViewColumns();

            pnlGrid.Controls.Add(dgvContracts);

            // Add panels to form
            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlControls);

            this.ResumeLayout();
        }

        private void SetupDataGridViewColumns()
        {
            dgvContracts.Columns.Clear();
            
            dgvContracts.Columns.Add("ContractId", "ID");
            dgvContracts.Columns["ContractId"].Visible = false;
            
            dgvContracts.Columns.Add("ContractNumber", "Kontrakt No");
            dgvContracts.Columns["ContractNumber"].Width = 120;
            
            dgvContracts.Columns.Add("ContractTitle", "Kontrakt Başlığı");
            dgvContracts.Columns["ContractTitle"].Width = 200;
            
            dgvContracts.Columns.Add("Description", "Açıklama");
            dgvContracts.Columns["Description"].Width = 250;
            
            dgvContracts.Columns.Add("CreatedDate", "Oluşturma Tarihi");
            dgvContracts.Columns["CreatedDate"].Width = 120;
            
            dgvContracts.Columns.Add("Status", "Durum");
            dgvContracts.Columns["Status"].Width = 100;
            
            dgvContracts.Columns.Add("SignatureCount", "İmza Sirküler Sayısı");
            dgvContracts.Columns["SignatureCount"].Width = 130;
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = customerService.GetAllCustomers();
                cmbCustomers.DataSource = customers;
                cmbCustomers.DisplayMember = "CustomerName";
                cmbCustomers.ValueMember = "CustomerId";
                cmbCustomers.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Müşteriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadContracts()
        {
            if (selectedCustomer == null) return;

            try
            {
                var contracts = contractService.GetContractsByCustomerId(selectedCustomer.CustomerId);
                dgvContracts.Rows.Clear();

                foreach (var contract in contracts)
                {
                    var signatureCount = contractService.GetSignatureCircularCount(contract.ContractId);
                    dgvContracts.Rows.Add(
                        contract.ContractId,
                        contract.ContractNumber,
                        contract.ContractTitle,
                        contract.Description,
                        contract.CreatedDate.ToShortDateString(),
                        contract.Status,
                        signatureCount
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kontraktlar yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomers.SelectedValue != null)
            {
                selectedCustomer = (Customer)cmbCustomers.SelectedItem;
                LoadContracts();
                btnViewSignatures.Enabled = false;
            }
        }

        private void dgvContracts_SelectionChanged(object sender, EventArgs e)
        {
            btnViewSignatures.Enabled = dgvContracts.SelectedRows.Count > 0;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers();
            if (selectedCustomer != null)
            {
                LoadContracts();
            }
        }

        private void btnNewContract_Click(object sender, EventArgs e)
        {
            if (selectedCustomer == null)
            {
                MessageBox.Show("Lütfen önce bir müşteri seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var contractForm = new ContractForm(selectedCustomer.CustomerId);
            if (contractForm.ShowDialog() == DialogResult.OK)
            {
                LoadContracts();
            }
        }

        private void btnViewSignatures_Click(object sender, EventArgs e)
        {
            if (dgvContracts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir kontrakt seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var contractId = Convert.ToInt32(dgvContracts.SelectedRows[0].Cells["ContractId"].Value);
            var signatureForm = new SignatureCircularForm(contractId);
            signatureForm.ShowDialog();
        }
    }
} 