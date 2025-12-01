using AudioToText.Interfaces;
using NAudio.Wave;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Whisper.net;

namespace AudioToText.Services
{
    /// <summary>
    /// Implementación de <see cref="IProcesadorAudio"/> utilizando Whisper.net.
    /// Procesa audio completamente offline sin depender de servicios externos.
    /// </summary>
    public class WhisperProcessorService : IProcesadorAudio
    {
        /// <summary>
        /// Nombre del modelo Whisper (ARCHIVO BIN) que debe estar en la carpeta del ejecutable.
        /// </summary>
        private const string ModelFileName = "ggml-base.bin";

        /// <summary>
        /// Convierte un audio a texto utilizando Whisper.net.
        /// </summary>
        public async Task<string> ConvertirATextoAsync(
            string rutaArchivo,
            IProgress<string> progresoTexto = null,
            IProgress<int> progresoPorcentaje = null)
        {
            if (!File.Exists(ModelFileName))
                throw new FileNotFoundException(
                    $"No se encontró el modelo Whisper '{ModelFileName}'. Debe estar en la carpeta del ejecutable.");

            if (!File.Exists(rutaArchivo))
                throw new FileNotFoundException("El archivo de audio especificado no existe.");

            try
            {
                return await ProcessAudioAsync(rutaArchivo, progresoTexto, progresoPorcentaje);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error procesando audio con Whisper: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Procesa el archivo de audio y genera la transcripción.
        /// </summary>
        private async Task<string> ProcessAudioAsync(
            string rutaArchivo,
            IProgress<string> progresoTexto,
            IProgress<int> progresoPorcentaje)
        {
            // Obtiene duración total del audio (sirve para calcular porcentaje)
            TimeSpan duracionTotal;
            using (var reader = new AudioFileReader(rutaArchivo))
            {
                duracionTotal = reader.TotalTime;
            }

            // Inicializa Whisper (modelo + procesador)
            using var whisperFactory = WhisperFactory.FromPath(ModelFileName);
            using var processor = whisperFactory.CreateBuilder()
                .WithLanguage("es")
                .Build();

            using var fileStream = File.OpenRead(rutaArchivo);

            var resultado = new StringBuilder();

            // Procesa el audio en streaming (segmentos)
            await foreach (var segment in processor.ProcessAsync(fileStream))
            {
                if (!string.IsNullOrWhiteSpace(segment.Text))
                {
                    resultado.Append(segment.Text);

                    // Reporta texto en streaming (para UI)
                    progresoTexto?.Report(segment.Text);
                }

                // Reporta porcentaje exacto
                if (progresoPorcentaje != null && duracionTotal.TotalSeconds > 0)
                {
                    double porcentaje = (segment.End.TotalSeconds / duracionTotal.TotalSeconds) * 100;
                    int porcentajeEntero = Math.Clamp((int)porcentaje, 0, 100);

                    progresoPorcentaje.Report(porcentajeEntero);
                }
            }

            // Aseguramos 100%
            progresoPorcentaje?.Report(100);

            return resultado.ToString().Trim();
        }
    }
}
