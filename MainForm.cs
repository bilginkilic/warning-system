using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class MainForm : Form
{
    private readonly WarningService _warningService;
    private System.Windows.Forms.Timer _refreshTimer;

    public MainForm()
    {
        InitializeComponent();
        _warningService = new WarningService("http://your-api-url"); // API URL'nizi buraya girin
        SetupRefreshTimer();
    }

    private void InitializeComponent()
    {
        this.Size = new System.Drawing.Size(800, 600);
        this.Text = "Uyarı Sistemi";
        this.Load += MainForm_Load;
    }

    private void SetupRefreshTimer()
    {
        _refreshTimer = new System.Windows.Forms.Timer();
        _refreshTimer.Interval = 30000; // 30 saniye
        _refreshTimer.Tick += async (s, e) => await RefreshWarnings();
        _refreshTimer.Start();
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        await RefreshWarnings();
    }

    private async Task RefreshWarnings()
    {
        try
        {
            var warnings = await _warningService.GetWarningsAsync();
            DisplayWarnings(warnings);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Uyarılar alınırken bir hata oluştu: {ex.Message}", "Hata",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DisplayWarnings(List<Warning> warnings)
    {
        foreach (var warning in warnings)
        {
            var warningForm = new CustomWarningForm(warning);
            warningForm.Show();
        }
    }
} 