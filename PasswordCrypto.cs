using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

/// <summary>
/// Simple password encryption/decryption utility using AES-256 encryption.
/// Cross-platform compatible with Windows, macOS, and Linux.
/// </summary>
class PasswordCrypto
{
    private const string ConfigFileName = "crypto.config.json";
    private static string ConfigFilePath => Path.Combine(AppContext.BaseDirectory, ConfigFileName);
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowUsage();
            return;
        }

        string command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "genkey":
                    GenerateKey();
                    break;
                case "setkey":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Error: setkey requires <key>");
                        ShowUsage();
                        return;
                    }
                    SetKey(args[1]);
                    break;
                case "showkey":
                    ShowKey();
                    break;
                case "encrypt":
                    string encryptKey = GetKey(args);
                    string encryptPassword = args.Length >= 3 ? args[2] : (args.Length >= 2 ? args[1] : null);
                    if (encryptKey == null || encryptPassword == null)
                    {
                        Console.WriteLine("Error: encrypt requires <password> (with key in config) or <key> <password>");
                        ShowUsage();
                        return;
                    }
                    Encrypt(encryptKey, encryptPassword);
                    break;
                case "decrypt":
                    string decryptKey = GetKey(args);
                    string encryptedPassword = args.Length >= 3 ? args[2] : (args.Length >= 2 ? args[1] : null);
                    if (decryptKey == null || encryptedPassword == null)
                    {
                        Console.WriteLine("Error: decrypt requires <encrypted-password> (with key in config) or <key> <encrypted-password>");
                        ShowUsage();
                        return;
                    }
                    Decrypt(decryptKey, encryptedPassword);
                    break;
                default:
                    Console.WriteLine("Error: Unknown command '" + command + "'");
                    ShowUsage();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Display usage information
    /// </summary>
    static void ShowUsage()
    {
        Console.WriteLine("PasswordCrypto - AES-256 Password Encryption Utility");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  PasswordCrypto genkey");
        Console.WriteLine("    Generate a new 256-bit encryption key (base64 encoded)");
        Console.WriteLine();
        Console.WriteLine("  PasswordCrypto setkey <key>");
        Console.WriteLine("    Save encryption key to config file (" + ConfigFileName + ")");
        Console.WriteLine();
        Console.WriteLine("  PasswordCrypto showkey");
        Console.WriteLine("    Display the key stored in the config file");
        Console.WriteLine();
        Console.WriteLine("  PasswordCrypto encrypt [<key>] <password>");
        Console.WriteLine("    Encrypt a password. Uses config key if only password provided.");
        Console.WriteLine();
        Console.WriteLine("  PasswordCrypto decrypt [<key>] <encrypted-password>");
        Console.WriteLine("    Decrypt an encrypted password. Uses config key if only encrypted-password provided.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  PasswordCrypto genkey");
        Console.WriteLine("  PasswordCrypto setkey \"abc123...\"");
        Console.WriteLine("  PasswordCrypto encrypt \"MyPassword123\"");
        Console.WriteLine("  PasswordCrypto decrypt \"xyz789...\"");
        Console.WriteLine("  PasswordCrypto encrypt \"abc123...\" \"MyPassword123\"");
        Console.WriteLine("  PasswordCrypto decrypt \"abc123...\" \"xyz789...\"");
    }

    /// <summary>
    /// Get the encryption key from config file or command line arguments
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Encryption key or null if not found</returns>
    static string GetKey(string[] args)
    {
        // Always try config file first
        string configKey = LoadKeyFromConfig();
        if (configKey != null)
        {
            return configKey;
        }

        // Fall back to command line argument if config key not available
        if (args.Length >= 3)
        {
            return args[1];
        }

        return null;
    }

    /// <summary>
    /// Load encryption key from configuration file
    /// </summary>
    /// <returns>Encryption key or null if not found</returns>
    static string LoadKeyFromConfig()
    {
        try
        {
            if (!File.Exists(ConfigFilePath))
            {
                return null;
            }

            string json = File.ReadAllText(ConfigFilePath);
            var config = JsonSerializer.Deserialize<JsonElement>(json);
            
            if (config.TryGetProperty("encryptionKey", out JsonElement keyElement))
            {
                return keyElement.GetString();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Save encryption key to configuration file
    /// </summary>
    /// <param name="key">Base64 encoded encryption key</param>
    static void SetKey(string key)
    {
        try
        {
            // Validate the key by trying to decode it
            byte[] keyBytes = Convert.FromBase64String(key);
            if (keyBytes.Length != 32)
            {
                Console.WriteLine("Error: Key must be 256 bits (32 bytes) when base64 decoded");
                Environment.Exit(1);
            }

            var config = new
            {
                encryptionKey = key,
                created = DateTime.UtcNow.ToString("o")
            };

            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(ConfigFilePath, json);
            Console.WriteLine("Key saved to " + ConfigFilePath);
        }
        catch (FormatException)
        {
            Console.WriteLine("Error: Invalid base64 key format");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving key: " + ex.Message);
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Display the encryption key from configuration file
    /// </summary>
    static void ShowKey()
    {
        string key = LoadKeyFromConfig();
        if (key == null)
        {
            Console.WriteLine("No key configured. Use 'setkey' to set one.");
            Environment.Exit(1);
        }
        Console.WriteLine(key);
    }

    /// <summary>
    /// Generate a new 256-bit AES encryption key and output as base64
    /// </summary>
    static void GenerateKey()
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.GenerateKey();
            string keyBase64 = Convert.ToBase64String(aes.Key);
            Console.WriteLine(keyBase64);
        }
    }

    /// <summary>
    /// Encrypt a password using AES-256 encryption
    /// </summary>
    /// <param name="keyBase64">Base64 encoded encryption key</param>
    /// <param name="plaintext">Password to encrypt</param>
    static void Encrypt(string keyBase64, string plaintext)
    {
        byte[] key = Convert.FromBase64String(keyBase64);
        
        using (var aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = key;
            aes.GenerateIV(); // Generate random IV for each encryption
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var encryptor = aes.CreateEncryptor())
            using (var ms = new MemoryStream())
            {
                // Write IV first (needed for decryption)
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(plaintext);
                }

                byte[] encrypted = ms.ToArray();
                string encryptedBase64 = Convert.ToBase64String(encrypted);
                Console.WriteLine(encryptedBase64);
            }
        }
    }

    /// <summary>
    /// Decrypt an encrypted password using AES-256 decryption
    /// </summary>
    /// <param name="keyBase64">Base64 encoded encryption key</param>
    /// <param name="encryptedBase64">Base64 encoded encrypted password (includes IV)</param>
    static void Decrypt(string keyBase64, string encryptedBase64)
    {
        byte[] key = Convert.FromBase64String(keyBase64);
        byte[] encrypted = Convert.FromBase64String(encryptedBase64);

        using (var aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Extract IV from the beginning of the encrypted data
            byte[] iv = new byte[aes.BlockSize / 8];
            Array.Copy(encrypted, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor())
            using (var ms = new MemoryStream(encrypted, iv.Length, encrypted.Length - iv.Length))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs))
            {
                string decrypted = reader.ReadToEnd();
                Console.WriteLine(decrypted);
            }
        }
    }
}
