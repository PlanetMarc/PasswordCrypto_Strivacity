# PasswordCrypto - AES-256 Password Encryption Utility

A simple, cross-platform command-line utility for encrypting and decrypting passwords using AES-256 encryption. Works on Windows, macOS, and Linux.

## Features

- **AES-256 Encryption**: Industry-standard encryption algorithm
- **Base64 Encoding**: All keys and encrypted data are base64 encoded for easy storage and transmission
- **Random IV**: Each encryption operation uses a unique initialization vector for enhanced security
- **Cross-Platform**: Runs on Windows, macOS, and Linux with .NET 8
- **Configuration File Support**: Store your encryption key in a config file for easy sharing and reuse

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

## Building

To build the application:

```bash
dotnet build PasswordCrypto.csproj --configuration Release
```

## Usage

### Option 1: Using a Configuration File (Recommended)

The utility can store your encryption key in a configuration file (`crypto.config.json`) so you don't need to specify it every time.

#### 1. Generate a Shared Key

First, generate a 256-bit encryption key:

```bash
dotnet run --project PasswordCrypto.csproj --configuration Release -- genkey
```

Example output:
```
6vTAd9c3NCkEDxk6fUEcSoc08WkEEK6Voy3np3rsfAk=
```

#### 2. Save the Key to Config File

Store the key in the configuration file:

```bash
dotnet run --project PasswordCrypto.csproj --configuration Release -- setkey "6vTAd9c3NCkEDxk6fUEcSoc08WkEEK6Voy3np3rsfAk="
```

This creates a `crypto.config.json` file in the application directory.

#### 3. View the Configured Key

To see what key is currently configured:

```bash
dotnet run --project PasswordCrypto.csproj --configuration Release -- showkey
```

#### 4. Encrypt and Decrypt Using Configured Key

Once the key is configured, you only need to provide the password:

```bash
# Encrypt
dotnet run --project PasswordCrypto.csproj --configuration Release -- encrypt "MySecretPassword123!"

# Decrypt
dotnet run --project PasswordCrypto.csproj --configuration Release -- decrypt "UcZKS+3i4cdQYiWrhyA3Bycj0L5fNZCKT..."
```

### Option 2: Specify Key on Command Line

You can still provide the key directly on the command line if preferred.

### Generate a Shared Key

First, generate a 256-bit encryption key:

```bash
dotnet run --project PasswordCrypto.csproj --configuration Release -- genkey
```

Example output:
```
6vTAd9c3NCkEDxk6fUEcSoc08WkEEK6Voy3np3rsfAk=
```

**Important**: Store this key securely. Anyone with this key can decrypt your passwords.

#### Encrypt a Password

Encrypt a password using your shared key:

```bash
dotnet run --project PasswordCrypto.csproj --configuration Release -- encrypt "<your-key>" "<password>"
```

Example:
```bash
dotnet run --project PasswordCrypto.csproj --configuration Release -- encrypt "6vTAd9c3NCkEDxk6fUEcSoc08WkEEK6Voy3np3rsfAk=" "MySecretPassword123!"
```

Example output:
```
UcZKS+3i4cdQYiWrhyA3Bycj0L5fNZCKT/rcASgyjKmsgxdve9LGgB1Z6szo8bRR
```

#### Decrypt a Password

Decrypt an encrypted password using your shared key:

```bash
dotnet run --project PasswordCrypto.csproj --configuration Release -- decrypt "<your-key>" "<encrypted-password>"
```

Example:
```bash
dotnet run --project PasswordCrypto.csproj --configuration Release -- decrypt "6vTAd9c3NCkEDxk6fUEcSoc08WkEEK6Voy3np3rsfAk=" "UcZKS+3i4cdQYiWrhyA3Bycj0L5fNZCKT/rcASgyjKmsgxdve9LGgB1Z6szo8bRR"
```

Example output:
```
MySecretPassword123!
```

## Sharing the Key with Others

To share your encryption key with someone else:

### Method 1: Share the Key String (Secure Channel)
1. Use `showkey` or `genkey` to get your key
2. Share the key string through a secure channel (encrypted email, secure messaging, password manager)
3. The recipient runs `setkey "<your-key>"` to configure it

### Method 2: Share the Config File (Secure Channel)
1. Copy the `crypto.config.json` file from your application's output directory
   - Location: `bin/Release/net8.0/crypto.config.json` (when using `dotnet run`)
   - Or next to the executable if published as standalone
2. Send the file through a secure channel
3. The recipient places it in their application's directory

**Security Note**: Never share encryption keys through insecure channels like plain email, chat, or public repositories.

## Compiling to Native Executable (Optional)

For easier distribution, you can publish as a self-contained executable:

### Windows
```bash
dotnet publish PasswordCrypto.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./publish/win
```

### macOS (Intel)
```bash
dotnet publish PasswordCrypto.csproj -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -o ./publish/mac
```

### macOS (Apple Silicon)
```bash
dotnet publish PasswordCrypto.csproj -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true -o ./publish/mac-arm
```

### Linux
```bash
dotnet publish PasswordCrypto.csproj -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -o ./publish/linux
```

After publishing, you can run the executable directly without `dotnet run`:

```bash
# Windows
.\publish\win\PasswordCrypto.exe genkey

# macOS/Linux
./publish/mac/PasswordCrypto genkey
```

## Usage in Scripts

### PowerShell Example (with config file)
```powershell
# One-time setup: Generate and configure key
$key = dotnet run --project PasswordCrypto.csproj -c Release -- genkey
dotnet run --project PasswordCrypto.csproj -c Release -- setkey $key

# Daily use: Encrypt and decrypt
$encrypted = dotnet run --project PasswordCrypto.csproj -c Release -- encrypt "MyPassword"
$decrypted = dotnet run --project PasswordCrypto.csproj -c Release -- decrypt $encrypted

Write-Host "Encrypted: $encrypted"
Write-Host "Decrypted: $decrypted"
```

### PowerShell Example (with key on command line)
```powershell
# Generate key
$key = dotnet run --project PasswordCrypto.csproj -c Release -- genkey

# Encrypt password
$encrypted = dotnet run --project PasswordCrypto.csproj -c Release -- encrypt $key "MyPassword"

# Decrypt password
$decrypted = dotnet run --project PasswordCrypto.csproj -c Release -- decrypt $key $encrypted

Write-Host "Decrypted password: $decrypted"
```

### Bash Example (with config file)
```bash
# One-time setup: Generate and configure key
KEY=$(dotnet run --project PasswordCrypto.csproj -c Release -- genkey)
dotnet run --project PasswordCrypto.csproj -c Release -- setkey "$KEY"

# Daily use: Encrypt and decrypt
ENCRYPTED=$(dotnet run --project PasswordCrypto.csproj -c Release -- encrypt "MyPassword")
DECRYPTED=$(dotnet run --project PasswordCrypto.csproj -c Release -- decrypt "$ENCRYPTED")

echo "Encrypted: $ENCRYPTED"
echo "Decrypted: $DECRYPTED"
```

### Bash Example (with key on command line)
```bash
# Generate key
KEY=$(dotnet run --project PasswordCrypto.csproj -c Release -- genkey)

# Encrypt password
ENCRYPTED=$(dotnet run --project PasswordCrypto.csproj -c Release -- encrypt "$KEY" "MyPassword")

# Decrypt password
DECRYPTED=$(dotnet run --project PasswordCrypto.csproj -c Release -- decrypt "$KEY" "$ENCRYPTED")

echo "Decrypted password: $DECRYPTED"
```

## Security Considerations

1. **Key Storage**: Store your encryption key securely (e.g., Azure Key Vault, AWS Secrets Manager, environment variables)
2. **Key Sharing**: Never commit encryption keys to source control
3. **Encrypted Data**: Each encryption generates a unique ciphertext due to random IV generation
4. **Transport**: Use secure channels (HTTPS, SSH) when transmitting keys or encrypted data
5. **Key Rotation**: Periodically rotate encryption keys and re-encrypt sensitive data

## How It Works

1. **Key Generation**: Creates a random 256-bit (32-byte) AES key
2. **Encryption**: 
   - Generates a random 128-bit initialization vector (IV)
   - Encrypts the plaintext using AES-256-CBC
   - Prepends the IV to the ciphertext
   - Encodes the result as base64
3. **Decryption**:
   - Decodes the base64 string
   - Extracts the IV from the first 16 bytes
   - Decrypts the remaining bytes using AES-256-CBC

## License

This utility is provided as-is for internal use.
