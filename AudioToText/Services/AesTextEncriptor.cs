using AudioToText.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AudioToText.Services
{
    /// <summary>
    /// Implementación concreta de IEncriptador utilizando el algoritmo AES (Advanced Encryption Standard).
    /// 
    /// PRINCIPIOS APLICADOS:
    /// - SRP (Single Responsibility Principle):
    ///       Esta clase solo se encarga de encriptar y desencriptar texto.
    ///       No gestiona archivos, UI, configuración ni lógica de negocio.
    ///
    /// - OCP (Open/Closed Principle):
    ///       Otras clases pueden crear nuevas implementaciones (RSA, DES, etc.)
    ///       sin modificar esta clase.
    ///
    /// NOTA IMPORTANTE:
    /// Esta clase usa una clave fija de ejemplo. En un entorno real,
    /// la clave debe provenir de un almacén seguro (Key Vault, user secrets, configuración cifrada, etc.).
    /// </summary>
    public class AesTextEncriptor : IEncriptador
    {
        // AES utiliza claves de 128, 192 o 256 bits.
        // Esta clave es de 32 bytes (256 bits). Es fija solo para fines didácticos.
        private readonly byte[] _claveBytes = Encoding.UTF8.GetBytes("ClaveSecretaParaElProyectoAudioT");

        /// <summary>
        /// Encripta un texto plano usando AES en modo CBC con un IV generado dinámicamente.
        ///
        /// DETALLES RELEVANTES:
        /// - El IV (vector de inicialización) se genera automáticamente para garantizar
        ///   que cada encriptación produzca un resultado distinto aunque el texto sea igual.
        ///
        /// - El IV se almacena al inicio del arreglo resultante, lo cual es un patrón estándar
        ///   porque el IV debe ser conocido para poder desencriptar.
        ///
        /// - El resultado final se devuelve en Base64 para que pueda almacenarse y transmitirse sin problema.
        /// </summary>
        /// <param name="textoPlano">Texto sin encriptar.</param>
        /// <returns>Texto encriptado y codificado en Base64 (incluye el IV al inicio).</returns>
        public string Encriptar(string textoPlano)
        {
            // Aes.Create() crea una instancia del proveedor AES del sistema operativo.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _claveBytes;

                // Cada cifrado debe usar un IV único para evitar ataques basados en patrones.
                aesAlg.GenerateIV();
                byte[] iv = aesAlg.IV;

                // Transformación de encriptación (AES + key + iv).
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // MemoryStream donde se almacenará el IV y el texto cifrado.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Escribir el IV al inicio permite reconstruir la encriptación al desencriptar.
                    msEncrypt.Write(iv, 0, iv.Length);

                    // CryptoStream que realiza la encriptación mientras se escribe.
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Se escribe el texto original, pero encriptado sobre la marcha.
                            swEncrypt.Write(textoPlano);
                        }
                    }

                    // Convertimos todo el arreglo (IV + ciphertext) a Base64.
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <summary>
        /// Desencripta una cadena previamente encriptada con este mismo algoritmo AES.
        ///
        /// PROCESO:
        /// 1. Se decodifica la cadena Base64 a bytes.
        /// 2. Se extraen los primeros 16 bytes (IV).
        /// 3. Se extrae el resto como ciphertext.
        /// 4. Se aplica la transformación inversa para obtener el texto original.
        ///
        /// VALIDACIÓN:
        /// - Se asegura que el texto encriptado sea válido y tenga el tamaño mínimo requerido.
        /// </summary>
        /// <param name="textoEncriptado">Texto encriptado en Base64 (incluye IV).</param>
        /// <returns>El texto plano original.</returns>
        public string Desencriptar(string textoEncriptado)
        {
            // Convertimos de Base64 a bytes (IV + ciphertext).
            byte[] textoCompletoBytes = Convert.FromBase64String(textoEncriptado);

            // AES usa bloques de 16 bytes → tamaño del IV.
            const int IvLongitud = 16;

            // Validación mínima de integridad.
            if (textoCompletoBytes.Length < IvLongitud)
            {
                throw new CryptographicException("El formato del texto encriptado es inválido.");
            }

            // 1. Extraer el IV (primeros 16 bytes).
            byte[] iv = new byte[IvLongitud];
            Array.Copy(textoCompletoBytes, 0, iv, 0, IvLongitud);

            // 2. Extraer el ciphertext (lo que queda después del IV).
            int cipherLongitud = textoCompletoBytes.Length - IvLongitud;
            byte[] cipherBytes = new byte[cipherLongitud];
            Array.Copy(textoCompletoBytes, IvLongitud, cipherBytes, 0, cipherLongitud);

            // Inicializamos AES nuevamente para la operación inversa.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _claveBytes;
                aesAlg.IV = iv;

                // Creamos el transformador de desencriptación.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // MemoryStream que contiene únicamente el ciphertext.
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    // CryptoStream que aplica desencriptación mientras se lee.
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Leemos el contenido desencriptado y lo devolvemos.
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
