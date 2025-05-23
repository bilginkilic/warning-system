using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarningSystem
{
    public class WarningManager
    {
        private List<MultiWarningForm> _activeWarnings;
        private readonly object _lockObject = new object();

        public WarningManager()
        {
            _activeWarnings = new List<MultiWarningForm>();
        }

        public async Task<List<WarningResult>> ShowWarningsAsync(List<Warning> warnings, WarningMode mode)
        {
            var results = new List<WarningResult>();
            var forms = new List<MultiWarningForm>();

            // Tüm formları oluştur
            for (int i = 0; i < warnings.Count; i++)
            {
                var form = new MultiWarningForm(warnings, mode);
                forms.Add(form);
            }

            // Formları göster
            foreach (var form in forms)
            {
                lock (_lockObject)
                {
                    _activeWarnings.Add(form);
                }
                form.FormClosed += (s, e) =>
                {
                    lock (_lockObject)
                    {
                        _activeWarnings.Remove(form);
                        results.Add(form.Result);
                    }
                };
                form.Show();
            }

            // Tüm formlar kapanana kadar bekle
            while (_activeWarnings.Any())
            {
                await Task.Delay(100);
            }

            return results;
        }

        public void CloseAllWarnings()
        {
            lock (_lockObject)
            {
                foreach (var warning in _activeWarnings.ToList())
                {
                    warning.Close();
                }
                _activeWarnings.Clear();
            }
        }
    }
} 