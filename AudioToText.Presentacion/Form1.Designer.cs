using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace AudioToText.Presentacion
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpieza de recursos
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ---------------------------
            // DECLARACIÓN DE CONTROLES
            // ---------------------------
            lblArchivoAudio = new Label();
            lblMetodo = new Label();

            txtRutaArchivo = new TextBox();
            btnSeleccionarAudio = new Button();
            ofdSeleccionarAudio = new OpenFileDialog();

            ConfigOpcionConverter = new ComboBox();

            grpConversion = new GroupBox();
            txtResultadoTexto = new TextBox();
            btnConvertirATexto = new Button();
            pbProgreso = new ProgressBar();

            grpEncriptacion = new GroupBox();
            txtResultadoEncriptado = new TextBox();
            btnEncriptar = new Button();
            btnDesencriptar = new Button();

            SuspendLayout();

            // ======================================================
            // LABEL: ARCHIVO DE AUDIO
            // ======================================================
            lblArchivoAudio.AutoSize = true;
            lblArchivoAudio.Location = new Point(35, 20);
            lblArchivoAudio.Text = "Seleccione un archivo de audio:";

            // ======================================================
            // LABEL: MÉTODO DE CONVERSIÓN
            // ======================================================
            lblMetodo.AutoSize = true;
            lblMetodo.Location = new Point(1030, 20);
            lblMetodo.Text = "Método de Conversión:";

            // ======================================================
            // TXT: RUTA DE ARCHIVO
            // ======================================================
            txtRutaArchivo.Location = new Point(35, 45);
            txtRutaArchivo.ReadOnly = true;
            txtRutaArchivo.Size = new Size(900, 23);

            // ======================================================
            // BOTÓN: SELECCIONAR AUDIO
            // ======================================================
            btnSeleccionarAudio.Location = new Point(950, 45);
            btnSeleccionarAudio.Size = new Size(75, 23);
            btnSeleccionarAudio.Text = "Seleccionar";
            btnSeleccionarAudio.Click += btnSeleccionarAudio_Click;

            // ======================================================
            // COMBOBOX: OPCIÓN DE CONVERSIÓN
            // ======================================================
            ConfigOpcionConverter.DropDownStyle = ComboBoxStyle.DropDownList;
            ConfigOpcionConverter.Items.AddRange(new object[]
            {
                "Whisper ( local )",
                "Gemini ( nube )"
            });
            ConfigOpcionConverter.Location = new Point(1030, 45);
            ConfigOpcionConverter.Size = new Size(150, 23);

            // ======================================================
            // GROUPBOX: CONVERSIÓN DE AUDIO
            // ======================================================
            grpConversion.Text = "1. Conversión de Audio a Texto";
            grpConversion.Location = new Point(35, 100);
            grpConversion.Size = new Size(1145, 235);

            txtResultadoTexto.Location = new Point(15, 25);
            txtResultadoTexto.Multiline = true;
            txtResultadoTexto.ScrollBars = ScrollBars.Vertical;
            txtResultadoTexto.Size = new Size(1110, 190);

            grpConversion.Controls.Add(txtResultadoTexto);

            // ======================================================
            // BOTÓN: CONVERTIR A TEXTO
            // ======================================================
            btnConvertirATexto.Location = new Point(1020, 340);
            btnConvertirATexto.Size = new Size(160, 30);
            btnConvertirATexto.Text = "Convertir a Texto";
            btnConvertirATexto.Click += btnConvertirATexto_Click;

            // ======================================================
            // PROGRESS BAR
            // ======================================================
            pbProgreso.Location = new Point(35, 340);
            pbProgreso.Size = new Size(780, 23);

            // ======================================================
            // GROUPBOX: ENCRIPTACIÓN
            // ======================================================
            grpEncriptacion.Text = "2. Encriptación de Texto";
            grpEncriptacion.Location = new Point(35, 390);
            grpEncriptacion.Size = new Size(1145, 270);

            txtResultadoEncriptado.Location = new Point(15, 25);
            txtResultadoEncriptado.Multiline = true;
            txtResultadoEncriptado.ScrollBars = ScrollBars.Vertical;
            txtResultadoEncriptado.Size = new Size(1110, 230);

            grpEncriptacion.Controls.Add(txtResultadoEncriptado);

            // ======================================================
            // BOTÓN: ENCRIPTAR
            // ======================================================
            btnEncriptar.Location = new Point(950, 670);
            btnEncriptar.Size = new Size(85, 30);
            btnEncriptar.Text = "Encriptar";
            btnEncriptar.Click += btnEncriptar_Click;

            // ======================================================
            // BOTÓN: DESENCRIPTAR
            // ======================================================
            btnDesencriptar.Location = new Point(1045, 670);
            btnDesencriptar.Size = new Size(100, 30);
            btnDesencriptar.Text = "Desencriptar";
            btnDesencriptar.Click += btnDesencriptar_Click;

            // ======================================================
            // CONFIGURACIÓN DEL FORMULARIO
            // ======================================================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;

            ClientSize = new Size(1220, 730);
            Text = "AudioText - Conversor y Encriptador";

            Controls.Add(lblArchivoAudio);
            Controls.Add(lblMetodo);
            Controls.Add(txtRutaArchivo);
            Controls.Add(btnSeleccionarAudio);
            Controls.Add(ConfigOpcionConverter);

            Controls.Add(grpConversion);
            Controls.Add(pbProgreso);
            Controls.Add(btnConvertirATexto);

            Controls.Add(grpEncriptacion);
            Controls.Add(btnEncriptar);
            Controls.Add(btnDesencriptar);

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // ========= CAMPOS PRIVADOS ==============

        private Label lblArchivoAudio;
        private Label lblMetodo;

        private TextBox txtRutaArchivo;
        private Button btnSeleccionarAudio;
        private OpenFileDialog ofdSeleccionarAudio;

        private ComboBox ConfigOpcionConverter;

        private GroupBox grpConversion;
        private TextBox txtResultadoTexto;
        private Button btnConvertirATexto;

        private ProgressBar pbProgreso;

        private GroupBox grpEncriptacion;
        private TextBox txtResultadoEncriptado;
        private Button btnEncriptar;
        private Button btnDesencriptar;
    }
}