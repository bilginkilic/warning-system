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
    public partial class PDFViewerForm : Form
    {
        private int signatureCircularId;
        private SignatureCircular currentCircular;
        private SignatureCircularService signatureService;
        private List<SignatureAssignment> signatureAssignments;

        // Controls
        private Panel pnlTop;
        private Panel pnlCenter;
        private Panel pnlBottom;
        private Panel pnlPDFViewer;
        private Panel pnlSignatureList;
        private Splitter splitter;
        
        private Label lblPDFInfo;
        private Button btnZoomIn;
        private Button btnZoomOut;
        private Button btnFitToWidth;
        private Button btnActualSize;
        private Button btnMarkSignature;
        private Button btnSaveAssignments;
        
        private DataGridView dgvSignatures;
        private PictureBox picPDFPage;
        private VScrollBar vScrollPDF;
        private HScrollBar hScrollPDF;
        
        private bool isMarkingMode = false;
        private Point markStartPoint;
        private Rectangle currentMarkRectangle;
        private float zoomFactor = 1.0f;

        public PDFViewerForm(int signatureCircularId)
        {
            this.signatureCircularId = signatureCircularId;
            InitializeComponent();
            InitializeServices();
            LoadSignatureCircular();
            LoadSignatureAssignments();
        }

        private void InitializeServices()
        {
            signatureService = new SignatureCircularService();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Properties
            this.Text = "PDF Görüntüleyici ve İmza İşaretleme";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // Top Panel
            pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 60;
            pnlTop.BackColor = Color.LightGray;

            // PDF Info Label
            lblPDFInfo = new Label();
            lblPDFInfo.Location = new Point(10, 10);
            lblPDFInfo.Size = new Size(400, 20);
            lblPDFInfo.Text = "PDF: Yükleniyor...";

            // Zoom In Button
            btnZoomIn = new Button();
            btnZoomIn.Text = "Yakınlaştır";
            btnZoomIn.Location = new Point(450, 8);
            btnZoomIn.Size = new Size(80, 25);
            btnZoomIn.Click += btnZoomIn_Click;

            // Zoom Out Button
            btnZoomOut = new Button();
            btnZoomOut.Text = "Uzaklaştır";
            btnZoomOut.Location = new Point(540, 8);
            btnZoomOut.Size = new Size(80, 25);
            btnZoomOut.Click += btnZoomOut_Click;

            // Fit to Width Button
            btnFitToWidth = new Button();
            btnFitToWidth.Text = "Genişliğe Sığdır";
            btnFitToWidth.Location = new Point(630, 8);
            btnFitToWidth.Size = new Size(100, 25);
            btnFitToWidth.Click += btnFitToWidth_Click;

            // Actual Size Button
            btnActualSize = new Button();
            btnActualSize.Text = "Gerçek Boyut";
            btnActualSize.Location = new Point(740, 8);
            btnActualSize.Size = new Size(90, 25);
            btnActualSize.Click += btnActualSize_Click;

            // Mark Signature Button
            btnMarkSignature = new Button();
            btnMarkSignature.Text = "İmza İşaretle";
            btnMarkSignature.Location = new Point(450, 35);
            btnMarkSignature.Size = new Size(100, 25);
            btnMarkSignature.Click += btnMarkSignature_Click;
            btnMarkSignature.BackColor = Color.LightBlue;

            // Save Assignments Button
            btnSaveAssignments = new Button();
            btnSaveAssignments.Text = "Atamaları Kaydet";
            btnSaveAssignments.Location = new Point(560, 35);
            btnSaveAssignments.Size = new Size(120, 25);
            btnSaveAssignments.Click += btnSaveAssignments_Click;
            btnSaveAssignments.BackColor = Color.LightGreen;

            pnlTop.Controls.Add(lblPDFInfo);
            pnlTop.Controls.Add(btnZoomIn);
            pnlTop.Controls.Add(btnZoomOut);
            pnlTop.Controls.Add(btnFitToWidth);
            pnlTop.Controls.Add(btnActualSize);
            pnlTop.Controls.Add(btnMarkSignature);
            pnlTop.Controls.Add(btnSaveAssignments);

            // Center Panel
            pnlCenter = new Panel();
            pnlCenter.Dock = DockStyle.Fill;

            // PDF Viewer Panel
            pnlPDFViewer = new Panel();
            pnlPDFViewer.Dock = DockStyle.Fill;
            pnlPDFViewer.BackColor = Color.White;
            pnlPDFViewer.AutoScroll = true;

            // PDF Picture Box
            picPDFPage = new PictureBox();
            picPDFPage.SizeMode = PictureBoxSizeMode.AutoSize;
            picPDFPage.BackColor = Color.White;
            picPDFPage.MouseDown += picPDFPage_MouseDown;
            picPDFPage.MouseMove += picPDFPage_MouseMove;
            picPDFPage.MouseUp += picPDFPage_MouseUp;
            picPDFPage.Paint += picPDFPage_Paint;

            pnlPDFViewer.Controls.Add(picPDFPage);

            // Splitter
            splitter = new Splitter();
            splitter.Dock = DockStyle.Right;
            splitter.Width = 5;

            // Signature List Panel
            pnlSignatureList = new Panel();
            pnlSignatureList.Dock = DockStyle.Right;
            pnlSignatureList.Width = 300;
            pnlSignatureList.BackColor = Color.LightYellow;

            // Signatures DataGridView
            dgvSignatures = new DataGridView();
            dgvSignatures.Dock = DockStyle.Fill;
            dgvSignatures.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSignatures.MultiSelect = false;
            dgvSignatures.AllowUserToAddRows = true;
            dgvSignatures.AllowUserToDeleteRows = true;
            
            SetupSignatureGridColumns();

            pnlSignatureList.Controls.Add(dgvSignatures);

            // Add controls to center panel
            pnlCenter.Controls.Add(pnlPDFViewer);
            pnlCenter.Controls.Add(splitter);
            pnlCenter.Controls.Add(pnlSignatureList);

            // Add panels to form
            this.Controls.Add(pnlCenter);
            this.Controls.Add(pnlTop);

            this.ResumeLayout();
        }

        private void SetupSignatureGridColumns()
        {
            dgvSignatures.Columns.Clear();

            dgvSignatures.Columns.Add("AssignedPersonName", "İmzalayan Kişi");
            dgvSignatures.Columns["AssignedPersonName"].Width = 120;

            dgvSignatures.Columns.Add("AssignedPersonTitle", "Ünvan");
            dgvSignatures.Columns["AssignedPersonTitle"].Width = 100;

            dgvSignatures.Columns.Add("Status", "Durum");
            dgvSignatures.Columns["Status"].Width = 80;

            var positionColumn = new DataGridViewTextBoxColumn();
            positionColumn.Name = "Position";
            positionColumn.HeaderText = "Konum (X,Y,W,H)";
            positionColumn.Width = 150;
            positionColumn.ReadOnly = true;
            dgvSignatures.Columns.Add(positionColumn);
        }

        private void LoadSignatureCircular()
        {
            try
            {
                currentCircular = signatureService.GetSignatureCircularById(signatureCircularId);
                if (currentCircular != null)
                {
                    lblPDFInfo.Text = $"PDF: {currentCircular.PDFFileName} - {currentCircular.CircularTitle}";
                    LoadPDFContent();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF bilgileri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPDFContent()
        {
            try
            {
                if (currentCircular?.PDFContent != null)
                {
                    // PDF'yi bitmap'e çevirme işlemi (gerçek uygulamada PDF kütüphanesi kullanılmalı)
                    // Burada örnek olarak boş bir resim oluşturuyoruz
                    var bitmap = new Bitmap(800, 1000);
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
                        g.DrawString("PDF İçeriği Burada Görüntülenecek", new Font("Arial", 16), Brushes.Black, 50, 50);
                        g.DrawString($"Dosya: {currentCircular.PDFFileName}", new Font("Arial", 12), Brushes.Blue, 50, 100);
                        g.DrawString("İmza alanlarını işaretlemek için 'İmza İşaretle' butonuna tıklayın", new Font("Arial", 10), Brushes.Red, 50, 150);
                        
                        // Örnek çerçeveler çiz
                        g.DrawRectangle(Pens.LightGray, 100, 200, 200, 50);
                        g.DrawString("Örnek İmza Alanı 1", new Font("Arial", 8), Brushes.Gray, 105, 220);
                        
                        g.DrawRectangle(Pens.LightGray, 400, 300, 200, 50);
                        g.DrawString("Örnek İmza Alanı 2", new Font("Arial", 8), Brushes.Gray, 405, 320);
                    }
                    
                    picPDFPage.Image = bitmap;
                    ApplyZoom();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF içeriği yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSignatureAssignments()
        {
            try
            {
                signatureAssignments = signatureService.GetSignatureAssignmentsByCircularId(signatureCircularId);
                dgvSignatures.Rows.Clear();

                foreach (var assignment in signatureAssignments)
                {
                    var positionText = $"{assignment.SignatureX:F0},{assignment.SignatureY:F0},{assignment.SignatureWidth:F0},{assignment.SignatureHeight:F0}";
                    dgvSignatures.Rows.Add(
                        assignment.AssignedPersonName,
                        assignment.AssignedPersonTitle,
                        assignment.Status,
                        positionText
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İmza atamaları yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyZoom()
        {
            if (picPDFPage.Image != null)
            {
                var newSize = new Size(
                    (int)(picPDFPage.Image.Width * zoomFactor),
                    (int)(picPDFPage.Image.Height * zoomFactor)
                );
                picPDFPage.Size = newSize;
            }
        }

        // Event Handlers
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            zoomFactor *= 1.25f;
            ApplyZoom();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            zoomFactor /= 1.25f;
            if (zoomFactor < 0.1f) zoomFactor = 0.1f;
            ApplyZoom();
        }

        private void btnFitToWidth_Click(object sender, EventArgs e)
        {
            if (picPDFPage.Image != null)
            {
                zoomFactor = (float)pnlPDFViewer.Width / picPDFPage.Image.Width;
                ApplyZoom();
            }
        }

        private void btnActualSize_Click(object sender, EventArgs e)
        {
            zoomFactor = 1.0f;
            ApplyZoom();
        }

        private void btnMarkSignature_Click(object sender, EventArgs e)
        {
            isMarkingMode = !isMarkingMode;
            btnMarkSignature.BackColor = isMarkingMode ? Color.Red : Color.LightBlue;
            btnMarkSignature.Text = isMarkingMode ? "İşaretleme İptal" : "İmza İşaretle";
            picPDFPage.Cursor = isMarkingMode ? Cursors.Cross : Cursors.Default;
        }

        private void picPDFPage_MouseDown(object sender, MouseEventArgs e)
        {
            if (isMarkingMode && e.Button == MouseButtons.Left)
            {
                markStartPoint = e.Location;
                currentMarkRectangle = new Rectangle(e.X, e.Y, 0, 0);
            }
        }

        private void picPDFPage_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMarkingMode && e.Button == MouseButtons.Left)
            {
                currentMarkRectangle = new Rectangle(
                    Math.Min(markStartPoint.X, e.X),
                    Math.Min(markStartPoint.Y, e.Y),
                    Math.Abs(e.X - markStartPoint.X),
                    Math.Abs(e.Y - markStartPoint.Y)
                );
                picPDFPage.Invalidate();
            }
        }

        private void picPDFPage_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMarkingMode && e.Button == MouseButtons.Left && currentMarkRectangle.Width > 10 && currentMarkRectangle.Height > 10)
            {
                // İmza alanı işaretlendi, kişi atama formunu aç
                var assignmentForm = new SignatureAssignmentForm(currentMarkRectangle, zoomFactor);
                if (assignmentForm.ShowDialog() == DialogResult.OK)
                {
                    var newAssignment = assignmentForm.SignatureAssignment;
                    newAssignment.SignatureCircularId = signatureCircularId;
                    
                    // Koordinatları gerçek PDF koordinatlarına çevir
                    newAssignment.SignatureX = currentMarkRectangle.X / zoomFactor;
                    newAssignment.SignatureY = currentMarkRectangle.Y / zoomFactor;
                    newAssignment.SignatureWidth = currentMarkRectangle.Width / zoomFactor;
                    newAssignment.SignatureHeight = currentMarkRectangle.Height / zoomFactor;
                    
                    signatureAssignments.Add(newAssignment);
                    LoadSignatureAssignments();
                }
                
                currentMarkRectangle = Rectangle.Empty;
                picPDFPage.Invalidate();
                isMarkingMode = false;
                btnMarkSignature_Click(sender, e);
            }
        }

        private void picPDFPage_Paint(object sender, PaintEventArgs e)
        {
            // Mevcut imza alanlarını çiz
            foreach (var assignment in signatureAssignments)
            {
                var rect = new Rectangle(
                    (int)(assignment.SignatureX * zoomFactor),
                    (int)(assignment.SignatureY * zoomFactor),
                    (int)(assignment.SignatureWidth * zoomFactor),
                    (int)(assignment.SignatureHeight * zoomFactor)
                );
                
                e.Graphics.DrawRectangle(Pens.Blue, rect);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Blue)), rect);
                
                // Kişi adını yaz
                if (!string.IsNullOrEmpty(assignment.AssignedPersonName))
                {
                    e.Graphics.DrawString(assignment.AssignedPersonName, new Font("Arial", 8), Brushes.Blue, rect.X, rect.Y - 15);
                }
            }
            
            // Aktif işaretleme alanını çiz
            if (isMarkingMode && currentMarkRectangle.Width > 0 && currentMarkRectangle.Height > 0)
            {
                e.Graphics.DrawRectangle(Pens.Red, currentMarkRectangle);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Red)), currentMarkRectangle);
            }
        }

        private void btnSaveAssignments_Click(object sender, EventArgs e)
        {
            try
            {
                signatureService.SaveSignatureAssignments(signatureAssignments);
                MessageBox.Show("İmza atamaları başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("İmza atamaları kaydedilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 