using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioToText.Interfaces
{
    /// <summary>
    /// Interfaz que define el contrato para cualquier servicio encargado de convertir
    /// archivos de audio en texto. 
    ///
    /// OBJETIVO:
    ///   Establecer un punto de abstracción que permita utilizar diferentes motores
    ///   de reconocimiento de voz (Whisper, Google Speech-to-Text, Azure STT, etc.)
    ///   sin modificar las capas superiores de la aplicación.
    ///
    /// PRINCIPIOS DE ARQUITECTURA APLICADOS:
    /// - DIP (Dependency Inversion Principle):
    ///       El formulario, control o clase que consuma este servicio dependerá de
    ///       esta *abstracción*, no de una implementación concreta.
    ///
    /// - OCP (Open/Closed Principle):
    ///       Se pueden agregar nuevos procesadores de audio simplemente implementando
    ///       la interfaz, sin cambiar nada del código existente.
    ///
    /// Resultado: mayor flexibilidad, escalabilidad y mantenibilidad del sistema.
    /// </summary>
    public interface IProcesadorAudio
    {
        /// <summary>
        /// Ejecuta de forma asincrónica el proceso de convertir un archivo de audio en texto.
        ///
        /// Detalles importantes:
        /// - Se declara como Task<string> porque la transcripción es una operación 
        ///   potencialmente costosa y debe correr de manera asíncrona.
        ///
        /// - Se incluyen dos canales opcionales de reporte:
        ///     * progresoTexto: permite enviar mensajes informativos (ej: "Preparando audio...", "Transcribiendo...")
        ///     * progresoPorcentaje: permite informar porcentaje de avance (0-100)
        ///
        ///   Esto facilita la retroalimentación al usuario final sin acoplarla 
        ///   directamente al algoritmo de transcripción.
        /// </summary>
        /// <param name="rutaArchivo">Ruta absoluta del archivo de audio a procesar.</param>
        /// <param name="progresoTexto">Canal opcional para reportes de texto durante el proceso.</param>
        /// <param name="progresoPorcentaje">Canal opcional para reportar el avance en porcentaje.</param>
        /// <returns>
        ///   Una tarea que produce como resultado la transcripción final en forma de cadena de texto.
        /// </returns>
        Task<string> ConvertirATextoAsync(
            string rutaArchivo, 
            IProgress<string> progresoTexto = null, 
            IProgress<int> progresoPorcentaje = null
        );
    }
}
