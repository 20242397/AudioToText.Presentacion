using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioToText.Presentacion
{
    public static class ModernDarkTheme
    {
        // ===== PALETA PRINCIPAL =====
        private static readonly Color BackgroundColor = Color.FromArgb(32, 32, 32);
        private static readonly Color PanelColor = Color.FromArgb(45, 45, 45);
        private static readonly Color TextColor = Color.WhiteSmoke;
        private static readonly Color AccentColor = Color.FromArgb(0, 120, 215);
        private static readonly Color BorderColor = Color.FromArgb(70, 70, 70);
        private static readonly Color HoverColor = Color.FromArgb(60, 60, 60);

        public static readonly Color ProgressFillColor = Color.FromArgb(0, 120, 215);

        // ================== APLICAR TEMA A FORM ==================
        public static void ApplyTheme(Form form)
        {
            form.BackColor = BackgroundColor;
            form.ForeColor = TextColor;

            foreach (Control c in form.Controls)
                ApplyThemeToControl(c);
        }

        // ================== APLICAR TEMA A CADA CONTROL ==================
        private static void ApplyThemeToControl(Control control)
        {
            switch (control)
            {
                case Panel:
                    control.BackColor = PanelColor;
                    break;

                case Label:
                    control.ForeColor = TextColor;
                    break;

                case Button btn:
                    ApplyButtonStyle(btn);
                    break;

                case TextBox txt:
                    ApplyTextBoxStyle(txt);
                    break;

                case ComboBox cb:
                    ApplyComboBoxStyle(cb);
                    break;

                case CheckBox chk:
                    chk.ForeColor = TextColor;
                    chk.BackColor = BackgroundColor;
                    break;

                case DataGridView dgv:
                    ApplyThemeToDataGridView(dgv);
                    break;

                default:
                    ApplyGenericStyle(control);
                    break;
            }

            // ---- Aplicar a hijos ----
            foreach (Control child in control.Controls)
                ApplyThemeToControl(child);
        }

        // ================== ESTILOS ESPECÍFICOS ==================

        private static void ApplyButtonStyle(Button btn)
        {
            btn.BackColor = PanelColor;
            btn.ForeColor = TextColor;
            btn.FlatStyle = FlatStyle.Flat;

            btn.FlatAppearance.BorderColor = BorderColor;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = HoverColor;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(25, 25, 25);
        }

        private static void ApplyTextBoxStyle(TextBox txt)
        {
            txt.BackColor = Color.FromArgb(38, 38, 38);
            txt.ForeColor = TextColor;
            txt.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void ApplyComboBoxStyle(ComboBox cb)
        {
            cb.BackColor = Color.FromArgb(38, 38, 38);
            cb.ForeColor = TextColor;
            cb.FlatStyle = FlatStyle.Flat;
        }

        private static void ApplyGenericStyle(Control c)
        {
            // Intentar aplicar colores genéricos
            try
            {
                c.BackColor = PanelColor;
                c.ForeColor = TextColor;
            }
            catch { }

            // ========== REFLEXIÓN PARA CONTROLES PERSONALIZADOS ==========
            var type = c.GetType();

            ApplyPropertyIfExists(type, c, "ProgressFill", ProgressFillColor);
            ApplyPropertyIfExists(type, c, "FillColor", PanelColor);
            ApplyPropertyIfExists(type, c, "HoverColor", HoverColor);
            ApplyPropertyIfExists(type, c, "BorderRadius", 8);
            ApplyPropertyIfExists(type, c, "BorderSize", 1);
        }

        // Método seguro para aplicar propiedades vía reflexión
        private static void ApplyPropertyIfExists(Type type, Control control, string propName, object value)
        {
            var prop = type.GetProperty(propName);
            if (prop != null && prop.CanWrite)
            {
                try { prop.SetValue(control, value); }
                catch { }
            }
        }

        // ================== ESTILOS ESPECIALES PARA DATAGRID ==================
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
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }
    }
}
