#!/usr/bin/env node

/**
 * PasswordCrypto - AES-256 Password Encryption Utility (Node.js)
 * Cross-platform compatible with Windows, macOS, and Linux.
 * Matches the C# implementation exactly.
 */

const crypto = require('crypto');
const fs = require('fs');
const path = require('path');

const CONFIG_FILE_NAME = 'crypto.config.json';
const CONFIG_FILE_PATH = path.join(__dirname, CONFIG_FILE_NAME);

// Algorithm settings (must match C# implementation)
const ALGORITHM = 'aes-256-cbc';
const KEY_SIZE = 32; // 256 bits
const IV_SIZE = 16;  // 128 bits (AES block size)

/**
 * Display usage information
 */
function showUsage() {
    console.log('PasswordCrypto - AES-256 Password Encryption Utility');
    console.log();
    console.log('Usage:');
    console.log('  node passwordCrypto.js genkey');
    console.log('    Generate a new 256-bit encryption key (base64 encoded)');
    console.log();
    console.log('  node passwordCrypto.js setkey <key>');
    console.log('    Save encryption key to config file (' + CONFIG_FILE_NAME + ')');
    console.log();
    console.log('  node passwordCrypto.js showkey');
    console.log('    Display the key stored in the config file');
    console.log();
    console.log('  node passwordCrypto.js encrypt [<key>] <password>');
    console.log('    Encrypt a password. Uses config key if only password provided.');
    console.log();
    console.log('  node passwordCrypto.js decrypt [<key>] <encrypted-password>');
    console.log('    Decrypt an encrypted password. Uses config key if only encrypted-password provided.');
    console.log();
    console.log('Examples:');
    console.log('  node passwordCrypto.js genkey');
    console.log('  node passwordCrypto.js setkey "abc123..."');
    console.log('  node passwordCrypto.js encrypt "MyPassword123"');
    console.log('  node passwordCrypto.js decrypt "xyz789..."');
    console.log('  node passwordCrypto.js encrypt "abc123..." "MyPassword123"');
    console.log('  node passwordCrypto.js decrypt "abc123..." "xyz789..."');
}

/**
 * Load encryption key from configuration file
 * @returns {string|null} Encryption key or null if not found
 */
function loadKeyFromConfig() {
    try {
        if (!fs.existsSync(CONFIG_FILE_PATH)) {
            return null;
        }

        const json = fs.readFileSync(CONFIG_FILE_PATH, 'utf8');
        const config = JSON.parse(json);
        
        return config.encryptionKey || null;
    } catch (error) {
        return null;
    }
}

/**
 * Get the encryption key from config file or command line arguments
 * @param {string[]} args - Command line arguments
 * @returns {string|null} Encryption key or null if not found
 */
function getKey(args) {
    // Always try config file first
    const configKey = loadKeyFromConfig();
    if (configKey) {
        return configKey;
    }

    // Fall back to command line argument if config key not available
    if (args.length >= 4) {
        return args[3];
    }

    return null;
}

/**
 * Generate a new 256-bit AES encryption key and output as base64
 */
function generateKey() {
    const key = crypto.randomBytes(KEY_SIZE);
    console.log(key.toString('base64'));
}

/**
 * Save encryption key to configuration file
 * @param {string} keyBase64 - Base64 encoded encryption key
 */
function setKey(keyBase64) {
    try {
        // Validate the key by trying to decode it
        const key = Buffer.from(keyBase64, 'base64');
        if (key.length !== KEY_SIZE) {
            console.error(`Error: Key must be 256 bits (32 bytes) when base64 decoded (got ${key.length} bytes)`);
            process.exit(1);
        }

        const config = {
            encryptionKey: keyBase64,
            created: new Date().toISOString()
        };

        fs.writeFileSync(CONFIG_FILE_PATH, JSON.stringify(config, null, 2), 'utf8');
        console.log('Key saved to ' + CONFIG_FILE_PATH);
    } catch (error) {
        if (error.message.includes('Invalid')) {
            console.error('Error: Invalid base64 key format');
        } else {
            console.error('Error saving key: ' + error.message);
        }
        process.exit(1);
    }
}

/**
 * Display the encryption key from configuration file
 */
function showKey() {
    const key = loadKeyFromConfig();
    if (!key) {
        console.error("No key configured. Use 'setkey' to set one.");
        process.exit(1);
    }
    console.log(key);
}

/**
 * Encrypt a password using AES-256 encryption
 * @param {string} keyBase64 - Base64 encoded encryption key
 * @param {string} plaintext - Password to encrypt
 */
function encrypt(keyBase64, plaintext) {
    try {
        const key = Buffer.from(keyBase64, 'base64');
        
        if (key.length !== KEY_SIZE) {
            throw new Error(`Key must be ${KEY_SIZE} bytes`);
        }

        // Generate random IV for each encryption (matches C# behavior)
        const iv = crypto.randomBytes(IV_SIZE);
        
        // Create cipher with AES-256-CBC and PKCS7 padding (default)
        const cipher = crypto.createCipheriv(ALGORITHM, key, iv);
        
        // Encrypt the plaintext
        let encrypted = cipher.update(plaintext, 'utf8');
        encrypted = Buffer.concat([encrypted, cipher.final()]);
        
        // Prepend IV to encrypted data (matches C# format)
        const result = Buffer.concat([iv, encrypted]);
        
        // Output as base64
        console.log(result.toString('base64'));
    } catch (error) {
        console.error('Error: ' + error.message);
        process.exit(1);
    }
}

/**
 * Decrypt an encrypted password using AES-256 decryption
 * @param {string} keyBase64 - Base64 encoded encryption key
 * @param {string} encryptedBase64 - Base64 encoded encrypted password (includes IV)
 */
function decrypt(keyBase64, encryptedBase64) {
    try {
        const key = Buffer.from(keyBase64, 'base64');
        const encrypted = Buffer.from(encryptedBase64, 'base64');
        
        if (key.length !== KEY_SIZE) {
            throw new Error(`Key must be ${KEY_SIZE} bytes`);
        }

        if (encrypted.length < IV_SIZE) {
            throw new Error('Invalid encrypted data: too short');
        }

        // Extract IV from the beginning of the encrypted data (matches C# format)
        const iv = encrypted.slice(0, IV_SIZE);
        const ciphertext = encrypted.slice(IV_SIZE);
        
        // Create decipher with AES-256-CBC and PKCS7 padding (default)
        const decipher = crypto.createDecipheriv(ALGORITHM, key, iv);
        
        // Decrypt the ciphertext
        let decrypted = decipher.update(ciphertext, null, 'utf8');
        decrypted += decipher.final('utf8');
        
        console.log(decrypted);
    } catch (error) {
        console.error('Error: ' + error.message);
        process.exit(1);
    }
}

/**
 * Main entry point
 */
function main() {
    const args = process.argv;

    if (args.length < 3) {
        showUsage();
        return;
    }

    const command = args[2].toLowerCase();

    try {
        switch (command) {
            case 'genkey':
                generateKey();
                break;
                
            case 'setkey':
                if (args.length < 4) {
                    console.error('Error: setkey requires <key>');
                    showUsage();
                    process.exit(1);
                }
                setKey(args[3]);
                break;
                
            case 'showkey':
                showKey();
                break;
                
            case 'encrypt':
                const encryptKey = getKey(args);
                const encryptPassword = args.length >= 4 ? args[3] : (args.length >= 3 ? args[2] : null);
                
                if (!encryptKey || !encryptPassword) {
                    console.error('Error: encrypt requires <password> (with key in config) or <key> <password>');
                    showUsage();
                    process.exit(1);
                }
                encrypt(encryptKey, encryptPassword);
                break;
                
            case 'decrypt':
                const decryptKey = getKey(args);
                const encryptedPassword = args.length >= 4 ? args[3] : (args.length >= 3 ? args[2] : null);
                
                if (!decryptKey || !encryptedPassword) {
                    console.error('Error: decrypt requires <encrypted-password> (with key in config) or <key> <encrypted-password>');
                    showUsage();
                    process.exit(1);
                }
                decrypt(decryptKey, encryptedPassword);
                break;
                
            default:
                console.error("Error: Unknown command '" + command + "'");
                showUsage();
                process.exit(1);
        }
    } catch (error) {
        console.error('Error: ' + error.message);
        process.exit(1);
    }
}

// Run the application
main();
