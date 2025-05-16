// Servicio para manejar las operaciones del generador y gestor de contraseñas

using CiberAgentes.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CiberAgentes.Services
{
    public class PasswordService
    {
        // Método para evaluar la seguridad de una contraseña del 0 al 100
        public int EvaluatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password)) return 0;

            int score = 0;

            // Longitud: máximo 25 puntos
            score += Math.Min(password.Length * 2, 25);

            // Variedad de caracteres: máximo 35 puntos
            if (Regex.IsMatch(password, "[A-Z]")) score += 5; // Mayúsculas
            if (Regex.IsMatch(password, "[a-z]")) score += 5; // Minúsculas
            if (Regex.IsMatch(password, "[0-9]")) score += 5; // Números
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]")) score += 10; // Caracteres especiales

            // Combinaciones adicionales: máximo 25 puntos
            if (Regex.IsMatch(password, "[A-Z].*[0-9]|[0-9].*[A-Z]")) score += 5; // Mayúscula y número
            if (Regex.IsMatch(password, "[a-z].*[0-9]|[0-9].*[a-z]")) score += 5; // Minúscula y número
            if (Regex.IsMatch(password, "[a-z].*[A-Z]|[A-Z].*[a-z]")) score += 5; // Mayúscula y minúscula
            if (Regex.IsMatch(password, "[a-zA-Z].*[^a-zA-Z0-9]|[^a-zA-Z0-9].*[a-zA-Z]")) score += 10; // Letra y especial

            // Penalizaciones: hasta -15 puntos
            if (password.Length < 8) score -= (8 - password.Length) * 2; // Penalización por longitud corta
            if (Regex.IsMatch(password, @"([a-zA-Z0-9])\1{2,}")) score -= 5; // Penalización por repeticiones

            // Asegurar que el score está entre 0 y 100
            return Math.Max(0, Math.Min(100, score));
        }

        // Método para generar una contraseña con nivel de seguridad personalizado
        public string GenerateSecurePassword(int length = 12, bool includeLowercase = true,
                                            bool includeUppercase = true, bool includeNumbers = true,
                                            bool includeSpecial = true)
        {
            // Validación de parámetros
            if (length < 6) length = 6;
            if (!includeLowercase && !includeUppercase && !includeNumbers && !includeSpecial)
            {
                includeLowercase = true; // Al menos un tipo debe estar habilitado
            }

            // Definir conjuntos de caracteres
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numberChars = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            // Crear el conjunto total de caracteres
            StringBuilder charSet = new StringBuilder();
            if (includeLowercase) charSet.Append(lowerChars);
            if (includeUppercase) charSet.Append(upperChars);
            if (includeNumbers) charSet.Append(numberChars);
            if (includeSpecial) charSet.Append(specialChars);

            // Usar RNGCryptoServiceProvider para generar bytes aleatorios
            using var rng = RandomNumberGenerator.Create();
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            // Convertir bytes a caracteres de nuestro conjunto
            StringBuilder result = new StringBuilder(length);
            string chars = charSet.ToString();

            // Asegurar que al menos un carácter de cada tipo requerido esté presente
            if (includeLowercase) result.Append(lowerChars[randomBytes[0] % lowerChars.Length]);
            if (includeUppercase) result.Append(upperChars[randomBytes[Math.Min(1, randomBytes.Length - 1)] % upperChars.Length]);
            if (includeNumbers) result.Append(numberChars[randomBytes[Math.Min(2, randomBytes.Length - 1)] % numberChars.Length]);
            if (includeSpecial) result.Append(specialChars[randomBytes[Math.Min(3, randomBytes.Length - 1)] % specialChars.Length]);

            // Completar el resto de la contraseña
            int startIndex = result.Length;
            for (int i = startIndex; i < length; i++)
            {
                result.Append(chars[randomBytes[i % randomBytes.Length] % chars.Length]);
            }

            // Mezclar los caracteres para evitar patrones predecibles
            char[] resultArray = result.ToString().ToCharArray();
            for (int i = 0; i < resultArray.Length; i++)
            {
                int swapIndex = randomBytes[i % randomBytes.Length] % resultArray.Length;
                (resultArray[i], resultArray[swapIndex]) = (resultArray[swapIndex], resultArray[i]);
            }

            return new string(resultArray);
        }

        // Método para encriptar una contraseña usando la contraseña maestra del usuario
        public (string encryptedPassword, string iv) EncryptPassword(string plainPassword, string masterPassword)
        {
            // Derivar una clave a partir de la contraseña maestra
            using var deriveBytes = new Rfc2898DeriveBytes(masterPassword, Encoding.UTF8.GetBytes("CyberAgentSalt"), 10000, HashAlgorithmName.SHA256);
            byte[] key = deriveBytes.GetBytes(32); // 256 bits
            byte[] iv = deriveBytes.GetBytes(16); // 128 bits para AES

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainPassword);
                cs.Write(plainBytes, 0, plainBytes.Length);
            }

            return (
                Convert.ToBase64String(ms.ToArray()), // Contraseña encriptada
                Convert.ToBase64String(iv)            // Vector de inicialización
            );
        }

        // Método para desencriptar una contraseña usando la contraseña maestra del usuario
        public string DecryptPassword(string encryptedPassword, string iv, string masterPassword)
        {
            try
            {
                // Derivar la misma clave a partir de la contraseña maestra
                using var deriveBytes = new Rfc2898DeriveBytes(masterPassword, Encoding.UTF8.GetBytes("CyberAgentSalt"), 10000, HashAlgorithmName.SHA256);
                byte[] key = deriveBytes.GetBytes(32);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = Convert.FromBase64String(iv);

                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(Convert.FromBase64String(encryptedPassword));
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cs);

                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                // En caso de error (por ejemplo, contraseña maestra incorrecta)
                return null;
            }
        }
    }
}