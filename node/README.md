# PasswordCrypto - Node.js Version

This is the Node.js implementation of PasswordCrypto that is **100% compatible** with the C# version. Data encrypted by one can be decrypted by the other.

## Requirements

- Node.js 14.0.0 or later (uses built-in `crypto` module)

## Installation

No dependencies needed! Just Node.js.

## Usage

### Generate a Shared Key

```bash
node passwordCrypto.js genkey
```

Example output:
```
Cq1DYxB/3+TtOAGlPt1PHptODUvnSD7kN9BRD1ka/HQ=
```

### Save Key to Config File

```bash
node passwordCrypto.js setkey "Cq1DYxB/3+TtOAGlPt1PHptODUvnSD7kN9BRD1ka/HQ="
```

### View Configured Key

```bash
node passwordCrypto.js showkey
```

### Encrypt with Configured Key

```bash
node passwordCrypto.js encrypt "MyPassword123"
```

### Decrypt with Configured Key

```bash
node passwordCrypto.js decrypt "Xx91Th8kXQ7bpWCpNydn5bp9HpWnaVe+VD17yutNWsg="
```

### Encrypt with Key on Command Line

```bash
node passwordCrypto.js encrypt "Cq1DYxB/3+TtOAGlPt1PHptODUvnSD7kN9BRD1ka/HQ=" "MyPassword123"
```

### Decrypt with Key on Command Line

```bash
node passwordCrypto.js decrypt "Cq1DYxB/3+TtOAGlPt1PHptODUvnSD7kN9BRD1ka/HQ=" "encrypted-hash-here"
```

## Cross-Platform Compatibility

This Node.js implementation uses the **exact same encryption settings** as the C# version:

- **Algorithm**: AES-256-CBC
- **Key Size**: 256 bits (32 bytes)
- **IV Size**: 128 bits (16 bytes)
- **Padding**: PKCS7
- **Format**: IV prepended to ciphertext, then base64 encoded

This means:
- ✅ Data encrypted with C# can be decrypted with Node.js
- ✅ Data encrypted with Node.js can be decrypted with C#
- ✅ Same key works on both platforms
- ✅ Config file format is identical

## Example: Cross-Platform Workflow

**Person A (using C#):**
```bash
# In C# directory
dotnet run -c Release -- setkey "Cq1DYxB/3+TtOAGlPt1PHptODUvnSD7kN9BRD1ka/HQ="
dotnet run -c Release -- encrypt "SecretPassword"
# Output: y5CZ7p8ab5wPVS5+KmPvAPXp2yIbh8DYn8AjEsXmR0I=
```

**Person B (using Node.js):**
```bash
# In node directory
node passwordCrypto.js setkey "Cq1DYxB/3+TtOAGlPt1PHptODUvnSD7kN9BRD1ka/HQ="
node passwordCrypto.js decrypt "y5CZ7p8ab5wPVS5+KmPvAPXp2yIbh8DYn8AjEsXmR0I="
# Output: SecretPassword
```

## Security Notes

- Config file is stored in the same directory as `passwordCrypto.js`
- Never commit `crypto.config.json` to version control
- Share encryption keys only through secure channels
- Each encryption generates a unique output (random IV)

## Files

- `passwordCrypto.js` - Main application
- `package.json` - NPM package configuration
- `crypto.config.json` - Auto-generated config file (not in repo)
