using AudioToText.Interfaces;
using Mscc.GenerativeAI; // Librería oficial para interactuar con Gemini
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AudioText.Services
{
    /// <summary>
    /// Servicio que implementa la interfaz IProcesadorAudio utilizando
    /// el modelo Gemini 2.5 Flash para convertir audio a texto.
    ///
    /// PRINCIPIOS APLICADOS:
    /// - DIP (Dependency Inversion Principle):
    ///     Esta clase depende de la abstracción IProcesadorAudio,
    ///     permitiendo reemplazar Gemini por OpenAI, Azure, etc. sin tocar el UI.
    ///
    /// - SRP (Single Responsibility Principle):
    ///     La clase solo se encarga de la comunicación con Gemini
    ///     y la transcripción del audio.
    ///
    /// - Asincronía real:
    ///     Permite mantener la UI responsiva mientras se procesa el audio.
    /// </summary>
    public class GeminiProcessorService : IProcesadorAudio
    {
        // ⚠ Importante: En producción la API Key debe almacenarse en un archivo seguro,
        // variables de entorno o Secret Manager.
        private const string ApiKey = "AIzaSyD3srAvEGOk91qoQdhCicx-KSz9Cws5u4k";

        /// <summary>
        /// Toma un archivo de audio, lo envía al modelo Gemini y devuelve su transcripción.
        /// Permite reportar progreso mediante IProgress (opcional).
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de audio.</param>
        /// <param name="progresoTexto">Mensajes de progreso para mostrar en pantalla (opcional).</param>
        /// <param name="progresoPorcentaje">Indicador de avance numérico (0-100).</param>
        /// <returns>Transcripción en texto.</returns>
        public async Task<string> ConvertirATextoAsync(string rutaArchivo,
            IProgress<string> progresoTexto = null,
            IProgress<int> progresoPorcentaje = null)
        {
            // Validación inicial: el archivo debe existir
            if (!File.Exists(rutaArchivo))
                throw new FileNotFoundException("Archivo no encontrado.");

            return await Task.Run(async () =>
            {
                try
                {
                    // Reporte de avance en UI
                    progresoTexto?.Report("Conectando con Gemini 1.5 Flash...\r\n");
                    progresoPorcentaje?.Report(10);

                    // 1. Inicializamos el cliente con nuestra API Key
                    var googleAI = new GoogleAI(ApiKey);

                    // Se selecciona explícitamente el modelo a utilizar.
                    // Esta corrección evita problemas de incompatibilidad con el enum.
                    var model = googleAI.GenerativeModel("gemini-2.5-flash");

                    // 2. Cargar el archivo en memoria
                    byte[] audioBytes = File.ReadAllBytes(rutaArchivo);
                    string base64Audio = Convert.ToBase64String(audioBytes);

                    // Determinar MIME según la extensión
                    string mimeType = ObtenerMimeType(rutaArchivo);

                    progresoTexto?.Report($"Subiendo audio ({mimeType}) a la IA...\r\n");
                    progresoPorcentaje?.Report(30);

                    // 3. Estructura de la solicitud para Gemini (GenerateContentRequest)
                    var request = new GenerateContentRequest
                    {
                        Contents = new List<Content>
                        {
                            new Content
                            {
                                Role = Mscc.GenerativeAI.Role.User,

                                // Lista explícita de partes para evitar problemas de tipado
                                Parts = new List<IPart>
                                {
                                    // Instrucción textual para Gemini
                                    new TextData
                                    {
                                        Text = "Por favor, transcribe este audio a texto en español. Solo dame la transcripción exacta."
                                    },

                                    // Audio enviado como base64
                                    new InlineData
                                    {
                                        MimeType = mimeType,
                                        Data = base64Audio
                                    }
                                }
                            }
                        }
                    };

                    progresoTexto?.Report("Gemini está procesando el audio...\r\n");
                    progresoPorcentaje?.Report(50);

                    // 4. Enviar la solicitud al modelo
                    var response = await model.GenerateContent(request);

                    progresoPorcentaje?.Report(100);

                    // 5. Verificamos la respuesta
                    if (response.Text != null)
                    {
                        return response.Text; // Transcripción devuelta por Gemini
                    }
                    else
                    {
                        // Mensaje informativo si no se obtuvo texto
                        return "Gemini no devolvió texto (posible audio vacío).";
                    }
                }
                catch (Exception ex)
                {
                    // Error controlado y transformado en InvalidOperationException
                    throw new InvalidOperationException($"Error Gemini: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Determina el MIME TYPE apropiado según el tipo de archivo de audio.
        /// Esto es necesario para que Gemini interprete correctamente los datos.
        /// </summary>
        private string ObtenerMimeType(string ruta)
        {
            string ext = Path.GetExtension(ruta).ToLower();

            return ext switch
            {
                ".mp3" => "audio/mp3",
                ".wav" => "audio/wav",
                ".m4a" => "audio/mp4",
                ".ogg" => "audio/ogg",
                // Valor por defecto si la extensión no está contemplada
                _ => "audio/mp3"
            };
        }
    }
}
