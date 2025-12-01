using Microsoft.VisualBasic;
using NAudio.Wave; // Biblioteca usada para leer y manipular archivos de audio (MP3, WAV, AIFF, etc.)
using System.Diagnostics; // Permite imprimir mensajes de depuración en la ventana Output

namespace AudioToText.Helpers
{
    /// <summary>
    /// Clase auxiliar encargada exclusivamente de convertir archivos de audio a un formato estándar (WAV 16kHz Mono)
    /// requerido por el motor de transcripción Whisper.
    /// Esta clase mantiene una única responsabilidad (SRP) y evita mezclar lógica de conversión con el resto del sistema.
    /// </summary>
    public static class AudioConvertHelper
    {
        /// <summary>
        /// Prepara un archivo de audio subido por el usuario para ser procesado por Whisper.
        /// Si el archivo no cumple con las especificaciones (frecuencia o canales), 
        /// se convierte automáticamente a WAV 16.000 Hz en mono.
        /// </summary>
        /// <param name="rutaArchivoOriginal">Ruta del archivo de audio original (MP3, WAV, etc.)</param>
        /// <param name="rutaDestinoTemp">Ruta donde se guardará el archivo WAV convertido temporalmente.</param>
        /// <returns>Ruta del archivo WAV generado y listo para procesar.</returns>
        public static string PrepararAudioParaProcesamiento(string rutaArchivoOriginal, string rutaDestinoTemp)
        {
            // Extrae la extensión del archivo para validaciones si se necesitan.
            string extension = Path.GetExtension(rutaArchivoOriginal).ToLower();

            try
            {
                // Registra en consola de depuración qué archivo se está procesando.
                Debug.WriteLine($"Procesando audio: {rutaArchivoOriginal}");

                // AudioFileReader permite leer múltiples formatos de audio sin convertir manualmente.
                // NAudio internamente selecciona el lector adecuado según la extensión.
                using (var reader = new AudioFileReader(rutaArchivoOriginal))
                {
                    // Whisper requiere audio con:
                    // - Frecuencia de muestreo: 16 kHz (16000)
                    // - Canal único (Mono)
                    var outFormat = new WaveFormat(16000, 1);

                    // MediaFoundationResampler realiza resampling (cambia frecuencia y canales).
                    using (var resampler = new MediaFoundationResampler(reader, outFormat))
                    {
                        // Ajuste opcional de calidad (0–60). 60 es la calidad máxima.
                        resampler.ResamplerQuality = 60;

                        // Se escribe el archivo convertido en la ruta temporal indicada.
                        // CreateWaveFile genera un archivo WAV en el formato del resampler.
                        WaveFileWriter.CreateWaveFile(rutaDestinoTemp, resampler);
                    }
                }

                // Mensaje de depuración confirmando éxito.
                Debug.WriteLine($"Audio convertido a 16kHz exitosamente: {rutaDestinoTemp}");

                // Se retorna la ruta final del archivo procesado.
                return rutaDestinoTemp;
            }
            catch (Exception ex)
            {
                // Cualquier error se encapsula en una excepción más descriptiva.
                throw new InvalidOperationException(
                    $"Error al convertir audio para Whisper (se requiere WAV 16kHz): {ex.Message}"
                );
            }
        }
    }
}
