using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WKClientsImporter.Views
{
    // Diálogo mínimo para elegir el formato (extensión) de fichero.
    public class TemplateFormatDialog : Form
    {
        private readonly ComboBox _cbFormats;
        private readonly Button _btnOk;
        private readonly Button _btnCancel;

        public string SelectedExtension { get; private set; }

        public TemplateFormatDialog(IList<string> extensions)
        {
            Text = "Select template format";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(360, 110);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            var lbl = new Label
            {
                Text = "Format:",
                Location = new Point(12, 15),
                AutoSize = true
            };
            Controls.Add(lbl);

            _cbFormats = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(70, 10),
                Size = new Size(270, 24)
            };
            Controls.Add(_cbFormats);

            _btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(184, 60),
                Size = new Size(75, 25)
            };
            _btnOk.Click += BtnOk_Click;
            Controls.Add(_btnOk);

            _btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(265, 60),
                Size = new Size(75, 25)
            };
            Controls.Add(_btnCancel);

            // Rellenar combobox con las extensiones. Mostrar texto legible.
            var items = (extensions ?? Enumerable.Empty<string>()).Select(e => e.Trim()).Where(e => !string.IsNullOrEmpty(e)).ToList();
            foreach (var ext in items)
            {
                // Mostrar "CSV (.csv)" o similar
                var display = ext.StartsWith(".") ? $"{ext.TrimStart('.').ToUpper()} ({ext})" : $"{ext.ToUpper()} ({(ext.StartsWith(".") ? ext : "." + ext)})";
                _cbFormats.Items.Add(new ComboBoxItem(display, ext.StartsWith(".") ? ext : "." + ext));
            }

            if (_cbFormats.Items.Count > 0)
            {
                _cbFormats.SelectedIndex = 0;
            }

            AcceptButton = _btnOk;
            CancelButton = _btnCancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            var sel = _cbFormats.SelectedItem as ComboBoxItem;
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