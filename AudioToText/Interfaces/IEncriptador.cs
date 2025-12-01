using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioToText.Interfaces
{
    /// <summary>
    /// Esta interfaz define el *contrato base* para cualquier mecanismo de encriptación reversible.
    /// 
    /// PRINCIPIOS APLICADOS:
    /// - OCP (Open/Closed Principle): El código que depende de esta interfaz no necesitará modificarse
    ///   cuando se añada un nuevo algoritmo de encriptación (AES, RSA, TripleDES, etc.).
    ///   Solo debe crearse una nueva clase que implemente IEncriptador.
    ///
    /// - DIP (Dependency Inversion Principle): Las clases no dependen de implementaciones concretas,
    ///   sino de una abstracción. Esto permite inyectar cualquier encriptador en tiempo de ejecución.
    ///
    /// OBJETIVO:
    /// Garantizar que el sistema pueda cambiar, sustituir o extender el mecanismo de encriptación
    /// sin romper el código existente, mejorando la mantenibilidad y escalabilidad del proyecto.
    /// </summary>
    public interface IEncriptador
    {
        /// <summary>
        /// Método encargado de convertir un texto plano en un texto encriptado.
        /// La implementación dependerá del algoritmo usado por la clase concreta.
        ///
        /// USO:
        ///   string resultado = servicio.Encriptar("mensaje secreto");
        ///
        /// RESPONSABILIDAD:
        ///   Definir la operación de encriptación sin imponer cómo debe hacerse.
        /// </summary>
        /// <param name="textoPlano">Cadena original sin encriptar.</param>
        /// <returns>Cadena resultante después de la encriptación.</returns>
        string Encriptar(string textoPlano);

        /// <summary>
        /// Método que toma un texto previamente encriptado y lo restaura a su forma original.
        /// 
        /// NOTA:
        /// Este método implica que el mecanismo de encriptación es *reversible*.
        /// No todos los algoritmos lo son —por ejemplo, el hashing— y por eso este método
        /// se incluye como parte del contrato solo para aquellos algoritmos que lo soporten.
        ///
        /// CRITERIO 1.3:
        /// Este requerimiento permite validación y recuperación de datos sensibles.
        /// </summary>
        /// <param name="textoEncriptado">Cadena encriptada que se desea desencriptar.</param>
        /// <returns>El texto plano original, sin modificaciones.</returns>
        string Desencriptar(string textoEncriptado);
    }
}
