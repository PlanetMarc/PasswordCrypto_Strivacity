# PasswordCrypto Quick Start Guide

## Initial Setup (Do Once)

1. **Generate a new encryption key:**
   ```bash
   dotnet run --project PasswordCrypto.csproj -c Release -- genkey
   ```
   Example output: `cqPzxS0dcDGP+Uh4EiNajLjo8A0UG/hsqJgGvzkD13k=`

2. **Save the key to config file:**
   ```bash
   dotnet run --project PasswordCrypto.csproj -c Release -- setkey "cqPzxS0dcDGP+Uh4EiNajLjo8A0UG/hsqJgGvzkD13k="
   ```

3. **Share the key with your collaborator** (through a secure channel)

## Daily Usage

**Encrypt a password:**
```bash
dotnet run --project PasswordCrypto.csproj -c Release -- encrypt "MyPassword123"
```

**Decrypt a password:**
```bash
dotnet run --project PasswordCrypto.csproj -c Release -- decrypt "nWeYFMnPeJGu4dEU7xMoYK/u+ngJCQJH..."
```

**View configured key:**
```bash
dotnet run --project PasswordCrypto.csproj -c Release -- showkey
```

## Sharing with Others

### Option 1: Share the Key String
1. Get your key: `dotnet run --project PasswordCrypto.csproj -c Release -- showkey`
2. Send key through secure channel (Signal, password manager, encrypted email)
3. Recipient sets key: `dotnet run --project PasswordCrypto.csproj -c Release -- setkey "<shared-key>"`

### Option 2: Share the Config File
1. Copy `bin/Release/net8.0/crypto.config.json`
2. Send file through secure channel
3. Recipient places it in their `bin/Release/net8.0/` directory

## Without Config File (Alternative)

You can still specify the key on each command:

```bash
dotnet run --project PasswordCrypto.csproj -c Release -- encrypt "key-here" "password-here"
dotnet run --project PasswordCrypto.csproj -c Release -- decrypt "key-here" "encrypted-here"
```

## Building Standalone Executable

For easier use without typing long commands:

```bash
dotnet publish PasswordCrypto.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./publish/win
```

Then use:
```bash
.\publish\win\PasswordCrypto.exe encrypt "MyPassword"
```

## Security Reminders

- ⚠️ **Never** share keys through plain email, Slack, Teams, or other unencrypted channels
- ⚠️ **Never** commit `crypto.config.json` to git (it's in .gitignore)
- ✅ **Always** use encrypted communication for sharing keys
- ✅ **Consider** storing keys in a password manager or secrets vault for production use
