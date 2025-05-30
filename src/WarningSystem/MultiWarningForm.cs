using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace WarningSystem
{
    public class WarningResult
    {
        public bool IsConfirmed { get; set; }
        public DialogResult Result { get; set; }
    }

    public class MultiWarningForm : Form
    {
        private List<Warning> _warnings;
        private Panel warningsPanel;
        private Button btnOk;
        private Button btnContinue;
        private Button btnCancel;
        private WarningMode _mode;
        private const int PADDING = 15;
        private const int WARNING_HEIGHT = 80;
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_WIDTH = 120;

        public WarningResult Result { get; private set; }

        public MultiWarningForm(List<Warning> warnings, WarningMode mode = WarningMode.Simulation)
        {
            _warnings = warnings;
            _mode = mode;
            Result = new WarningResult();
            InitializeComponents();
            LoadWarnings();
        }

        private void InitializeComponents()
        {
            this.Text = "Sistem Uyarıları";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);

            // Ana panel
            warningsPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoScroll = true,
                Padding = new Padding(PADDING),
                Height = this.ClientSize.Height - (BUTTON_HEIGHT + 3 * PADDING)
            };
            this.Controls.Add(warningsPanel);

            // Butonlar için alt panel
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = BUTTON_HEIGHT + (2 * PADDING),
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(PADDING)
            };

            if (_mode == WarningMode.Simulation)
            {
                // Tek buton - ortada
                btnOk = new Button
                {
                    Text = "Tamam",
                    Size = new Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Anchor = AnchorStyles.None,
                    Cursor = Cursors.Hand
                };

                btnOk.Location = new Point(
                    (buttonPanel.ClientSize.Width - BUTTON_WIDTH) / 2,
                    (buttonPanel.ClientSize.Height - BUTTON_HEIGHT) / 2
                );

                btnOk.Click += (s, e) =>
                {
                    Result.Result = DialogResult.OK;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };

                btnOk.MouseEnter += (s, e) => btnOk.BackColor = Color.FromArgb(0, 100, 190);
                btnOk.MouseLeave += (s, e) => btnOk.BackColor = Color.FromArgb(0, 120, 215);

                buttonPanel.Controls.Add(btnOk);
            }
            else
            {
                // İki buton - ortada yan yana
                var totalWidth = (2 * BUTTON_WIDTH) + PADDING;
                var startX = (buttonPanel.ClientSize.Width - totalWidth) / 2;

                btnContinue = new Button
                {
                    Text = "Devam Et",
                    Size = new Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                    Location = new Point(startX, (buttonPanel.ClientSize.Height - BUTTON_HEIGHT) / 2),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.None
                };

                btnCancel = new Button
                {
                    Text = "İptal",
                    Size = new Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                    Location = new Point(startX + BUTTON_WIDTH + PADDING, (buttonPanel.ClientSize.Height - BUTTON_HEIGHT) / 2),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(200, 200, 200),
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.None
                };

                btnContinue.MouseEnter += (s, e) => btnContinue.BackColor = Color.FromArgb(0, 100, 190);
                btnContinue.MouseLeave += (s, e) => btnContinue.BackColor = Color.FromArgb(0, 120, 215);

                btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.FromArgb(180, 180, 180);
                btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = Color.FromArgb(200, 200, 200);

                btnContinue.Click += (s, e) =>
                {
                    Result.IsConfirmed = true;
                    Result.Result = DialogResult.Yes;
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                };

                btnCancel.Click += (s, e) =>
                {
                    Result.IsConfirmed = false;
                    Result.Result = DialogResult.No;
                    this.DialogResult = DialogResult.No;
                    this.Close();
                };

                buttonPanel.Controls.Add(btnContinue);
                buttonPanel.Controls.Add(btnCancel);
            }

            buttonPanel.Paint += (s, e) =>
            {
                using (var brush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(brush, buttonPanel.ClientRectangle);
                }
            };

            this.Controls.Add(buttonPanel);

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Resize += (s, e) =>
            {
                if (_mode == WarningMode.Simulation)
                {
                    btnOk.Location = new Point(
                        (buttonPanel.ClientSize.Width - BUTTON_WIDTH) / 2,
                        (buttonPanel.ClientSize.Height - BUTTON_HEIGHT) / 2
                    );
                }
                else
                {
                    var totalWidth = (2 * BUTTON_WIDTH) + PADDING;
                    var startX = (buttonPanel.ClientSize.Width - totalWidth) / 2;
                    btnContinue.Location = new Point(startX, (buttonPanel.ClientSize.Height - BUTTON_HEIGHT) / 2);
                    btnCancel.Location = new Point(startX + BUTTON_WIDTH + PADDING, (buttonPanel.ClientSize.Height - BUTTON_HEIGHT) / 2);
                }
            };
        }

        private void LoadWarnings()
        {
            var groupedWarnings = _warnings
                .GroupBy(w => w.WarningColor.ToLower())
                .OrderBy(g => g.Key);

            int currentY = 0;

            foreach (var group in groupedWarnings)
            {
                var headerLabel = new Label
                {
                    Text = GetColorDisplayName(group.Key),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml(group.Key),
                    Location = new Point(0, currentY),
                    AutoSize = true
                };
                warningsPanel.Controls.Add(headerLabel);
                currentY += 30;

                foreach (var warning in group)
                {
                    var warningPanel = CreateWarningPanel(warning, currentY);
                    warningsPanel.Controls.Add(warningPanel);
                    currentY += WARNING_HEIGHT + 10;
                }

                currentY += 20;
            }
        }

        private Panel CreateWarningPanel(Warning warning, int yPosition)
        {
            var panel = new Panel
            {
                Size = new Size(warningsPanel.Width - 40, WARNING_HEIGHT),
                Location = new Point(10, yPosition),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            var numberLabel = new Label
            {
                Text = $"#{warning.WarningNo}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml(warning.WarningColor),
                Location = new Point(10, 10),
                AutoSize = true
            };
            panel.Controls.Add(numberLabel);

            var textLabel = new Label
            {
                Text = warning.WarningText,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                ForeColor = Color.Black,
                Location = new Point(10, 35),
                Size = new Size(panel.Width - 20, WARNING_HEIGHT - 45),
                AutoEllipsis = true
            };
            panel.Controls.Add(textLabel);

            panel.Paint += (s, e) =>
            {
                using (var pen = new Pen(ColorTranslator.FromHtml(warning.WarningColor), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            };

            return panel;
        }

        private string GetColorDisplayName(string colorName)
        {
            return colorName.ToLower() switch
            {
                "red" => "Kritik Uyarılar",
                "black" => "Önemli Uyarılar",
                "darkgray" => "Bilgilendirme",
                _ => "Diğer Uyarılar"
            };
        }
    }
} 