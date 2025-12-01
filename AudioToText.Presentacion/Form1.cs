using AudioToText.Helpers;
using AudioToText.Interfaces;
using AudioToText.Services;
using System;
using System.IO;
using System.Windows.Forms;

namespace AudioToText.Presentacion
{
    public partial class Form1 : Form
    {
        // Servicio usado para convertir audio a texto
        private IProcesadorAudio _procesadorAudio;

        // Servicio para encriptar y desencriptar texto con AES
        private readonly IEncriptador _encriptador;

        // Variable donde guardamos temporalmente el texto transcrito
        private string _textoTranscritorio = string.Empty;

        public Form1()
        {
            InitializeComponent();

            // Aplicar modo oscuro moderno
            ModernDarkTheme.ApplyTheme(this);

            // Selecciona por defecto la opción Whisper
            ConfigOpcionConverter.SelectedIndex = 0;

            // Servicio por defecto
            _procesadorAudio = new WhisperProcessorService();

            // Encriptador AES
            _encriptador = new AesTextEncriptor();

            // Evento para progress bar moderna
            pbProgreso.Paint += pbProgreso_Paint;

            // Ajustes para que la ProgressBar personalizada funcione correctamente
            pbProgreso.Style = ProgressBarStyle.Continuous;
            pbProgreso.Maximum = 100;
            pbProgreso.Value = 0;
        }

        // ------------------------------
        // SECCIÓN: CARGAR ARCHIVO
        // ------------------------------
        private void btnSeleccionarAudio_Click(object sender, EventArgs e)
        {
            ofdSeleccionarAudio.Filter = "Audio (*.mp3;*.wav)|*.mp3;*.wav|Todos (*.*)|*.*";
            ofdSeleccionarAudio.Title = "Seleccionar Archivo de Audio";

            if (ofdSeleccionarAudio.ShowDialog() == DialogResult.OK)
            {
                txtRutaArchivo.Text = ofdSeleccionarAudio.FileName;

                txtResultadoTexto.Clear();
                txtResultadoEncriptado.Clear();
                _textoTranscritorio = string.Empty;

                MessageBox.Show(
                    $"Archivo seleccionado: {Path.GetFileName(ofdSeleccionarAudio.FileName)}",
                    "Archivo Cargado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        // ------------------------------------------
        // SECCIÓN: CONVERTIR AUDIO A TEXTO
        // ------------------------------------------
        private async void btnConvertirATexto_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRutaArchivo.Text) || !File.Exists(txtRutaArchivo.Text))
            {
                MessageBox.Show("Seleccione un archivo de audio válido.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string opcion = ConfigOpcionConverter.Text;

            if (opcion.Contains("Whisper"))
                _procesadorAudio = new WhisperProcessorService();
            else
                _procesadorAudio = new GeminiProcessorService();

            try
            {
                btnConvertirATexto.Enabled = false;

                txtResultadoTexto.Clear();
                txtResultadoTexto.Text = $"Iniciando transcripción con {opcion}...\r\n";
                pbProgreso.Value = 0;
                pbProgreso.Refresh();

                string rutaOriginal = txtRutaArchivo.Text;
                string rutaParaProcesar = rutaOriginal;

                if (opcion.Contains("Whisper"))
                {
                    txtResultadoTexto.AppendText("Preparando audio para Whisper...\r\n");

                    rutaParaProcesar = await Task.Run(() =>
                        AudioConvertHelper.PrepararAudioParaProcesamiento(
                            rutaOriginal,
                            Path.Combine(Path.GetTempPath(), "temp_whisper_16k.wav")
                        )
                    );
                }
                else
                {
                    string ext = Path.GetExtension(rutaOriginal).ToLower();

                    if (ext != ".mp3" && ext != ".wav" && ext != ".m4a" && ext != ".ogg")
                    {
                        txtResultadoTexto.AppendText("Formato no estándar. Convirtiendo...\r\n");

                        rutaParaProcesar = await Task.Run(() =>
                            AudioConvertHelper.PrepararAudioParaProcesamiento(
                                rutaOriginal,
                                Path.Combine(Path.GetTempPath(), "temp_convertido.wav")
                            )
                        );
                    }
                }

                var reporteTexto = new Progress<string>(texto =>
                {
                    txtResultadoTexto.AppendText(texto);
                    txtResultadoTexto.SelectionStart = txtResultadoTexto.Text.Length;
                    txtResultadoTexto.ScrollToCaret();
                });

                var reportePorcentaje = new Progress<int>(p =>
                {
                    // Aseguramos límites válidos
                    if (p < pbProgreso.Minimum) p = pbProgreso.Minimum;
                    if (p > pbProgreso.Maximum) p = pbProgreso.Maximum;

                    pbProgreso.Value = p;
                    pbProgreso.Refresh();
                });

                string textoFinal = await _procesadorAudio.ConvertirATextoAsync(
                    rutaParaProcesar,
                    reporteTexto,
                    reportePorcentaje
                );

                txtResultadoTexto.AppendText("\r\n--- TRANSCRIPCIÓN COMPLETA ---\r\n");
                txtResultadoTexto.AppendText(textoFinal);

                _textoTranscritorio = textoFinal;

                MessageBox.Show("Transcripción completada.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                pbProgreso.Value = 0;
            }
            finally
            {
                btnConvertirATexto.Enabled = true;
            }
        }

        // ------------------------------------------
        // SECCIÓN: ENCRIPTAR TEXTO
        // ------------------------------------------
        private void btnEncriptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_textoTranscritorio))
            {
                MessageBox.Show("Debe convertir un audio antes de encriptar.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string cifrado = _encriptador.Encriptar(_textoTranscritorio);
                txtResultadoEncriptado.Text = cifrado;

                MessageBox.Show("Texto encriptado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al encriptar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ------------------------------------------
        // SECCIÓN: DESENCRIPTAR TEXTO
        // ------------------------------------------
        private void btnDesencriptar_Click(object sender, EventArgs e)
        {
            string textoCifrado = txtResultadoEncriptado.Text;

            if (string.IsNullOrEmpty(textoCifrado))
            {
                MessageBox.Show("No hay texto para desencriptar.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string texto = _encriptador.Desencriptar(textoCifrado);

                // En vez de sobreescribir la transcripción original, la añadimos para preservar historial
                txtResultadoTexto.AppendText("\r\n\r\n--- TEXTO DESENCRIPTADO ---\r\n");
                txtResultadoTexto.AppendText(texto);

                MessageBox.Show("Texto desencriptado.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al desencriptar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ------------------------------------------
        // PROGRESSBAR MODERNA (CUSTOM PAINT)
        // ------------------------------------------
        private void pbProgreso_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = pbProgreso.ClientRectangle;

            // Fondo oscuro
            using (var bgBrush = new SolidBrush(System.Drawing.Color.FromArgb(50, 50, 55)))
            {
                e.Graphics.FillRectangle(bgBrush, rect);
            }

            if (pbProgreso.Value > 0)
            {
                int width = (int)(rect.Width * ((double)pbProgreso.Value / pbProgreso.Maximum));

                using (var fillBrush = new SolidBrush(ModernDarkTheme.ProgressFillColor))
                {
                    e.Graphics.FillRectangle(fillBrush, new System.Drawing.Rectangle(0, 0, width, rect.Height));
                }
            }
        }
    }
}
