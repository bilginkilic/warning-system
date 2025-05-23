using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

public class CustomWarningForm : Form
{
    private Warning _warning;
    private Timer _fadeTimer;
    private float _opacity = 0;

    public CustomWarningForm(Warning warning)
    {
        _warning = warning;
        InitializeComponents();
        SetupForm();
    }

    private void InitializeComponents()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(400, 150);
        this.ShowInTaskbar = false;

        _fadeTimer = new Timer();
        _fadeTimer.Interval = 50;
        _fadeTimer.Tick += FadeTimer_Tick;
    }

    private void SetupForm()
    {
        this.Paint += CustomWarningForm_Paint;
        this.Click += (s, e) => this.Close();
        this.Opacity = 0;
        _fadeTimer.Start();
    }

    private void FadeTimer_Tick(object sender, EventArgs e)
    {
        if (_opacity < 1)
        {
            _opacity += 0.1f;
            this.Opacity = _opacity;
        }
        else
        {
            _fadeTimer.Stop();
        }
    }

    private void CustomWarningForm_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Form arka planı
        using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
        {
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }

        // Uyarı numarası
        using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
        using (var brush = new SolidBrush(_warning.GetSystemColor()))
        {
            e.Graphics.DrawString($"Uyarı #{_warning.WarningNo}", font, brush, new PointF(20, 20));
        }

        // Uyarı metni
        using (var font = new Font("Segoe UI", 10))
        using (var brush = new SolidBrush(Color.Black))
        {
            var textRect = new RectangleF(20, 50, this.Width - 40, this.Height - 70);
            e.Graphics.DrawString(_warning.WarningText, font, brush, textRect);
        }

        // Kenarlık
        using (var pen = new Pen(_warning.GetSystemColor(), 2))
        {
            e.Graphics.DrawRectangle(pen, 1, 1, this.Width - 3, this.Height - 3);
        }
    }
} 