using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WKClientsImporter.Localization;

namespace WKClientsImporter.Views
{
    // Diálogo mínimo para elegir el formato (extensión) de fichero.
    public partial class TemplateFormatDialog : Form
    {
        public string SelectedExtension { get; private set; }
        private readonly IStringLocalizer _localizer;


        public TemplateFormatDialog(IStringLocalizer _localizer, IList<string> extensions)
        {
            InitializeComponent();
            this._localizer = _localizer;
            ApplyLocalizer();
            PopulateComboBox(extensions);
        }

        private void ApplyLocalizer()
        {
            this.Text = _localizer.Get("TemplateFormatDialogTitle");
            lblFormat.Text = _localizer.Get("LabelFormat");
            btnOk.Text = _localizer.Get("ButtonOk");
            btnCancel.Text = _localizer.Get("ButtonCancel");
        }

        private void PopulateComboBox(IList<string> extensions)
        {
            cbFormats.Items.Clear();

            var items = (extensions ?? Enumerable.Empty<string>())
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e))
                .ToList();

            foreach (var ext in items)
            {
                var normalized = ext.StartsWith(".") ? ext : "." + ext;
                var display = normalized.StartsWith(".")
                    ? $"{normalized.TrimStart('.').ToUpper()} ({normalized})"
                    : $"{normalized.ToUpper()} ({normalized})";

                cbFormats.Items.Add(new ComboBoxItem(display, normalized));
            }

            if (cbFormats.Items.Count > 0)
            {
                cbFormats.SelectedIndex = 0;
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            var sel = cbFormats.SelectedItem as ComboBoxItem;
            SelectedExtension = sel?.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private class ComboBoxItem
        {
            public string Text { get; }
            public string Value { get; }

            public ComboBoxItem(string text, string value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString() => Text;
        }
    }
}