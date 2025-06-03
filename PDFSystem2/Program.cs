using System;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common;

namespace PDFSystem2
{
    static class Program
    {
        /// <summary>
        /// Gizmox web uygulamasının ana giriş noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Gizmox web uygulaması başlatılıyor
            new Gizmox.WebGUI.Server.WebHost().Start();
        }
    }
} 