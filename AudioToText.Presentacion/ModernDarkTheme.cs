using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioToText.Presentacion
{
    public static class ModernDarkTheme
    {
        private static readonly Color BackgroundColor = Color.FromArgb(32, 32, 32);
        private static readonly Color PanelColor = Color.FromArgb(45, 45, 45);
        private static readonly Color TextColor = Color.WhiteSmoke;
        private static readonly Color AccentColor = Color.FromArgb(0, 120, 215);
        private static readonly Color BorderColor = Color.FromArgb(70, 70, 70);
        private static readonly Color HoverColor = Color.FromArgb(60, 60, 60);
        public static readonly Color ProgressFillColor = Color.FromArgb(0, 120, 215);

        public static void ApplyTheme(Form form)
        {
            form.BackColor = BackgroundColor;
            form.ForeColor = TextColor;

            foreach (Control c in form.Controls)
                ApplyThemeToControl(c);
        }

        private static void ApplyThemeToControl(Control control)
        {
            if (control is Panel)
            {
                control.BackColor = PanelColor;
            }
            else if (control is Label)
            {
                control.ForeColor = TextColor;
            }
            else if (control is Button btn)
            {
                btn.BackColor = PanelColor;
                btn.ForeColor = TextColor;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = BorderColor;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.MouseOverBackColor = HoverColor;
                btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 25, 25);
            }
            else if (control is TextBox txt)
            {
                txt.BackColor = Color.FromArgb(38, 38, 38);
                txt.ForeColor = TextColor;
                txt.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is ComboBox cb)
            {
                cb.BackColor = Color.FromArgb(38, 38, 38);
                cb.ForeColor = TextColor;
                cb.FlatStyle = FlatStyle.Flat;
            }
            else if (control is CheckBox chk)
            {
                chk.ForeColor = TextColor;
                chk.BackColor = BackgroundColor;
            }
            else if (control is DataGridView dgv)
            {
                ApplyThemeToDataGridView(dgv);
            }
            else
            {
                // Intentar aplicar color si el control lo soporta
                TryApplyGenericStyle(control);
            }

            // Aplicar a controles hijos
            foreach (Control child in control.Controls)
                ApplyThemeToControl(child);
        }

        private static void TryApplyGenericStyle(Control c)
        {
            try
            {
                c.BackColor = PanelColor;
                c.ForeColor = TextColor;
            }
            catch { }

            // --- Soporte especial para controles personalizados con ProgressFill ---
            var type = c.GetType();

            if (type.GetProperty("ProgressFill") != null)
            {
                type.GetProperty("ProgressFill")?.SetValue(c, ProgressFillColor);
            }

            if (type.GetProperty("FillColor") != null)
            {
                type.GetProperty("FillColor")?.SetValue(c, PanelColor);
            }

            if (type.GetProperty("HoverColor") != null)
            {
                type.GetProperty("HoverColor")?.SetValue(c, HoverColor);
            }

            if (type.GetProperty("BorderRadius") != null)
            {
                type.GetProperty("BorderRadius")?.SetValue(c, 8);
            }

            if (type.GetProperty("BorderSize") != null)
            {
                type.GetProperty("BorderSize")?.SetValue(c, 1);
            }
        }

        private static void ApplyThemeToDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = BackgroundColor;
            dgv.ForeColor = TextColor;
            dgv.EnableHeadersVisualStyles = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = PanelColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = AccentColor;

            dgv.RowsDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
            dgv.RowsDefaultCellStyle.ForeColor = TextColor;
            dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(60, 60, 60);

            dgv.GridColor = BorderColor;
        }
    }
}
