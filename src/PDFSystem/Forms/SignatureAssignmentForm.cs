using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Common.Interfaces;

namespace PDFSystem.Forms
{
    public class SignatureAssignmentForm : Form
    {
        private Panel pnlMain;
        private ListView lvwSignatureAssignments;
        private Button btnAssign;
        private Button btnRemove;
        private ComboBox cmbUsers;
        private Label lblSelectedArea;
        private Button btnSave;
        private Button btnCancel;
        private int circularId;

        public SignatureAssignmentForm(int circularId)
        {
            this.circularId = circularId;
            InitializeComponent();
            LoadUsers();
            LoadExistingAssignments();
        }

        private void InitializeComponent()
        {
            this.pnlMain = new Panel();
            this.lvwSignatureAssignments = new ListView();
            this.cmbUsers = new ComboBox();
            this.btnAssign = new Button();
            this.btnRemove = new Button();
            this.lblSelectedArea = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Form
            this.Text = "İmza Atama Formu";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main Panel
            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlMain.Padding = new Padding(10);

            // Signature Assignments ListView
            this.lvwSignatureAssignments.Dock = DockStyle.Top;
            this.lvwSignatureAssignments.Height = 300;
            this.lvwSignatureAssignments.View = View.Details;
            this.lvwSignatureAssignments.Columns.Add("Kullanıcı", 200);
            this.lvwSignatureAssignments.Columns.Add("Pozisyon", 150);
            this.lvwSignatureAssignments.Columns.Add("Koordinatlar", 200);
            this.lvwSignatureAssignments.Columns.Add("Durum", 100);
            this.lvwSignatureAssignments.FullRowSelect = true;
            this.lvwSignatureAssignments.GridLines = true;

            // Users ComboBox
            this.cmbUsers.Location = new Point(10, 320);
            this.cmbUsers.Size = new Size(200, 23);

            // Selected Area Label
            this.lblSelectedArea.Location = new Point(220, 323);
            this.lblSelectedArea.Size = new Size(300, 23);
            this.lblSelectedArea.Text = "Seçili Alan: -";

            // Assign Button
            this.btnAssign.Location = new Point(530, 320);
            this.btnAssign.Size = new Size(100, 23);
            this.btnAssign.Text = "Ata";
            this.btnAssign.Click += new EventHandler(BtnAssign_Click);

            // Remove Button
            this.btnRemove.Location = new Point(640, 320);
            this.btnRemove.Size = new Size(100, 23);
            this.btnRemove.Text = "Kaldır";
            this.btnRemove.Click += new EventHandler(BtnRemove_Click);

            // Save Button
            this.btnSave.Location = new Point(530, 520);
            this.btnSave.Size = new Size(100, 23);
            this.btnSave.Text = "Kaydet";
            this.btnSave.Click += new EventHandler(BtnSave_Click);

            // Cancel Button
            this.btnCancel.Location = new Point(640, 520);
            this.btnCancel.Size = new Size(100, 23);
            this.btnCancel.Text = "İptal";
            this.btnCancel.Click += new EventHandler(BtnCancel_Click);

            // Add controls
            this.pnlMain.Controls.Add(this.lvwSignatureAssignments);
            this.pnlMain.Controls.Add(this.cmbUsers);
            this.pnlMain.Controls.Add(this.lblSelectedArea);
            this.pnlMain.Controls.Add(this.btnAssign);
            this.pnlMain.Controls.Add(this.btnRemove);
            this.pnlMain.Controls.Add(this.btnSave);
            this.pnlMain.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pnlMain);
        }

        private async void BtnAssign_Click(object sender, EventArgs e)
        {
            if (cmbUsers.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedUser = (KeyValuePair<int, string>)cmbUsers.SelectedItem;
                var coordinates = lblSelectedArea.Text.Replace("Seçili Alan: ", "");

                ListViewItem item = new ListViewItem(new string[] {
                    selectedUser.Value,
                    "İmzalayacak",
                    coordinates,
                    "Bekliyor"
                });
                item.Tag = selectedUser.Key; // UserId'yi tag olarak saklayalım
                lvwSignatureAssignments.Items.Add(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İmza atama hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lvwSignatureAssignments.SelectedItems.Count > 0)
            {
                try
                {
                    lvwSignatureAssignments.Items.Remove(lvwSignatureAssignments.SelectedItems[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Silme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    await conn.OpenAsync();

                    // Önce mevcut atamaları silelim
                    using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM SignatureAssignments WHERE CircularId = @CircularId", conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@CircularId", circularId);
                        await deleteCmd.ExecuteNonQueryAsync();
                    }

                    // Yeni atamaları ekleyelim
                    foreach (ListViewItem item in lvwSignatureAssignments.Items)
                    {
                        using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO SignatureAssignments (CircularId, UserId, Position, Coordinates, Status)
                            VALUES (@CircularId, @UserId, @Position, @Coordinates, @Status)", conn))
                        {
                            cmd.Parameters.AddWithValue("@CircularId", circularId);
                            cmd.Parameters.AddWithValue("@UserId", (int)item.Tag);
                            cmd.Parameters.AddWithValue("@Position", item.SubItems[1].Text);
                            cmd.Parameters.AddWithValue("@Coordinates", item.SubItems[2].Text);
                            cmd.Parameters.AddWithValue("@Status", item.SubItems[3].Text);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                MessageBox.Show("İmza atamaları kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void SetSelectedArea(Rectangle area)
        {
            lblSelectedArea.Text = $"Seçili Alan: X:{area.X}, Y:{area.Y}, W:{area.Width}, H:{area.Height}";
        }

        private async void LoadUsers()
        {
            try
            {
                cmbUsers.Items.Clear();
                using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("SELECT UserId, UserName FROM Users WHERE Status = 'Aktif'", conn))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int userId = reader.GetInt32(0);
                                string userName = reader.GetString(1);
                                cmbUsers.Items.Add(new KeyValuePair<int, string>(userId, userName));
                            }
                        }
                    }
                }
                cmbUsers.DisplayMember = "Value";
                cmbUsers.ValueMember = "Key";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı yükleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadExistingAssignments()
        {
            try
            {
                lvwSignatureAssignments.Items.Clear();
                using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT sa.*, u.UserName 
                        FROM SignatureAssignments sa
                        INNER JOIN Users u ON sa.UserId = u.UserId
                        WHERE sa.CircularId = @CircularId", conn))
                    {
                        cmd.Parameters.AddWithValue("@CircularId", circularId);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ListViewItem item = new ListViewItem(new string[] {
                                    reader["UserName"].ToString(),
                                    reader["Position"].ToString(),
                                    reader["Coordinates"].ToString(),
                                    reader["Status"].ToString()
                                });
                                item.Tag = reader.GetInt32(reader.GetOrdinal("UserId"));
                                lvwSignatureAssignments.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mevcut atamaları yükleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 