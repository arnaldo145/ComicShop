# Configuração local — JWT e segredos

Este guia mostra como configurar o segredo do JWT (`Jwt:Secret`) e demais parâmetros para executar o `ComicShop.WebApi` no seu ambiente, sem precisar editar o `appsettings.json` para cada máquina.

## Visão geral

O ASP.NET Core lê configuração de várias fontes, na ordem (a última vence):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (somente em `Development`)
4. Variáveis de ambiente
5. Argumentos de linha de comando

A seção esperada é:

```json
"Jwt": {
  "Secret": "...",
  "Issuer": "ComicShop",
  "Audience": "ComicShop.Clients",
  "ExpirationMinutes": 60
}
```

Para o primeiro usuário administrador, a API também aceita uma seção de bootstrap:

```json
"Bootstrap": {
  "Admin": {
    "Name": "Admin",
    "Email": "...",
    "Password": "..."
  }
}
```

Se `Jwt:Secret` estiver vazio, a aplicação falha no startup com erro explícito.

## Gerando um segredo forte

PowerShell:

```powershell
[Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Maximum 256 }))
```

Use no mínimo 32 bytes de entropia (saída ≥ 44 caracteres em base64).

## Desenvolvimento local — User Secrets (recomendado)

Inicialize uma única vez:

```powershell
dotnet user-secrets init --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
```

Defina o segredo:

```powershell
dotnet user-secrets set "Jwt:Secret" "cole-aqui-o-valor-gerado" --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
```

Opcional, sobrescrever outros campos:

```powershell
dotnet user-secrets set "Jwt:Issuer" "ComicShop" --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
dotnet user-secrets set "Jwt:Audience" "ComicShop.Clients" --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
dotnet user-secrets set "Jwt:ExpirationMinutes" "60" --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
dotnet user-secrets set "Bootstrap:Admin:Name" "Admin" --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
dotnet user-secrets set "Bootstrap:Admin:Email" "admin@local" --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
dotnet user-secrets set "Bootstrap:Admin:Password" "uma-senha-forte" --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
```

Conferir o que está salvo:

```powershell
dotnet user-secrets list --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
```

Os valores ficam no perfil do usuário (fora do repositório), só são carregados quando `ASPNETCORE_ENVIRONMENT=Development` e sempre vencem o `appsettings.json`.

Na primeira execução, se ainda não existir nenhum administrador, a API cria automaticamente o primeiro usuário `Admin` usando `Bootstrap:Admin`. Se existir apenas o admin legado `admin@admin.com`, ele é atualizado com os valores configurados. Se `Bootstrap:Admin` não estiver configurado, nenhum admin padrão inseguro é mantido.

Executar:

```powershell
dotnet run --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
```

## Variáveis de ambiente

Em qualquer SO, o separador de seção do .NET é `__` (dois underlines).

PowerShell (sessão atual):

```powershell
$env:Jwt__Secret = "cole-aqui-o-valor-gerado"
$env:Jwt__Issuer = "ComicShop"
$env:Jwt__Audience = "ComicShop.Clients"
$env:Jwt__ExpirationMinutes = "60"
$env:Bootstrap__Admin__Name = "Admin"
$env:Bootstrap__Admin__Email = "admin@local"
$env:Bootstrap__Admin__Password = "uma-senha-forte"

dotnet run --project .\ComicShop.WebApi\ComicShop.WebApi.csproj
```

Bash:

```bash
export Jwt__Secret="cole-aqui-o-valor-gerado"
export Jwt__Issuer="ComicShop"
export Jwt__Audience="ComicShop.Clients"
export Jwt__ExpirationMinutes="60"
export Bootstrap__Admin__Name="Admin"
export Bootstrap__Admin__Email="admin@local"
export Bootstrap__Admin__Password="uma-senha-forte"

dotnet run --project ./ComicShop.WebApi/ComicShop.WebApi.csproj
```

## Docker Compose

No `docker-compose.yml`, adicione as variáveis no serviço `comic-shop-api`:

```yaml
services:
  comic-shop-api:
    environment:
      ConnectionStrings__ComicShopContext: "Data Source=host.docker.internal,1433;Initial Catalog=ComicShop;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=Yes;"
      ElasticSearch__Uri: "http://host.docker.internal:9200"
      Jwt__Secret: "${JWT_SECRET:?Set JWT_SECRET in .env or the shell before running docker compose.}"
      Jwt__Issuer: "ComicShop"
      Jwt__Audience: "ComicShop.Clients"
      Jwt__ExpirationMinutes: "60"
      Bootstrap__Admin__Name: "${BOOTSTRAP_ADMIN_NAME:?Set BOOTSTRAP_ADMIN_NAME in .env or the shell before running docker compose.}"
      Bootstrap__Admin__Email: "${BOOTSTRAP_ADMIN_EMAIL:?Set BOOTSTRAP_ADMIN_EMAIL in .env or the shell before running docker compose.}"
      Bootstrap__Admin__Password: "${BOOTSTRAP_ADMIN_PASSWORD:?Set BOOTSTRAP_ADMIN_PASSWORD in .env or the shell before running docker compose.}"
```

Para não comitar o segredo, use um arquivo `.env` ao lado do compose:

```dotenv
JWT_SECRET=cole-aqui-o-valor-gerado
BOOTSTRAP_ADMIN_NAME=Admin
BOOTSTRAP_ADMIN_EMAIL=admin@local
BOOTSTRAP_ADMIN_PASSWORD=uma-senha-forte
```

E referencie:

```yaml
environment:
  Jwt__Secret: ${JWT_SECRET:?Set JWT_SECRET in .env or the shell before running docker compose.}
  Bootstrap__Admin__Name: ${BOOTSTRAP_ADMIN_NAME:?Set BOOTSTRAP_ADMIN_NAME in .env or the shell before running docker compose.}
  Bootstrap__Admin__Email: ${BOOTSTRAP_ADMIN_EMAIL:?Set BOOTSTRAP_ADMIN_EMAIL in .env or the shell before running docker compose.}
  Bootstrap__Admin__Password: ${BOOTSTRAP_ADMIN_PASSWORD:?Set BOOTSTRAP_ADMIN_PASSWORD in .env or the shell before running docker compose.}
```

Fluxo recomendado com Docker:

1. Copie `.env.example` para `.env`.
2. Preencha `JWT_SECRET`, `BOOTSTRAP_ADMIN_NAME`, `BOOTSTRAP_ADMIN_EMAIL` e `BOOTSTRAP_ADMIN_PASSWORD`.
3. Rode `docker compose up --build`.

Se qualquer uma dessas variáveis estiver ausente, o `docker compose` falha antes de subir os containers.

## Produção / cloud

Recomenda-se um cofre de segredos:

- Azure Key Vault com `AddAzureKeyVault`
- AWS Secrets Manager
- HashiCorp Vault

A aplicação continua lendo via `IConfiguration` (`Jwt:Secret`), sem mudança de código.

## Validando rapidamente

1. Suba a API.
2. Se for a primeira execução, o usuário administrador inicial é criado automaticamente com `Bootstrap:Admin:Name`, `Bootstrap:Admin:Email` e `Bootstrap:Admin:Password`.
3. Gere um token:

   ```http
   POST /v1/generate-token
   {
  "email": "admin@local",
  "password": "uma-senha-forte"
   }
   ```

4. Decodifique o JWT em <https://jwt.io> e confira:
   - `iss` igual ao `Jwt:Issuer` configurado
   - `aud` igual ao `Jwt:Audience` configurado
   - `exp` coerente com `Jwt:ExpirationMinutes`

## Erros comuns

| Mensagem | Causa provável | Como corrigir |
| --- | --- | --- |
| `Missing 'Jwt' configuration section.` | Nenhuma fonte forneceu a seção `Jwt`. | Definir via user-secrets ou variáveis de ambiente. |
| `'Jwt:Secret' must be configured.` | A chave existe mas está vazia. | Definir `Jwt:Secret` em user-secrets ou env var. |
| `docker compose config` falha pedindo `JWT_SECRET` ou `BOOTSTRAP_ADMIN_*` | O arquivo `.env` não existe ou está incompleto. | Copiar `.env.example` para `.env`, preencher os valores e tentar novamente. |
| Primeiro login não funciona e não existe admin | `Bootstrap:Admin` não foi configurado corretamente na primeira execução. | Definir `Name`, `Email` e `Password` e reiniciar a API. |
| `IDX10653: The encryption algorithm 'HS256' requires a key size of at least '128' bits` | Segredo muito curto. | Gerar uma chave maior (recomendado ≥ 32 bytes). |
| Token inválido após trocar o segredo | Tokens antigos foram assinados com a chave anterior. | Pedir novo login; tokens antigos ficam inválidos por design. |

## Resumo

- Nunca coloque o segredo real no `appsettings.json` comitado.
- Em dev, use **User Secrets**.
- Em containers e servidores, use **variáveis de ambiente**.
- Em produção, prefira **cofre de segredos**.
- Configure `Bootstrap:Admin` na primeira execução para criar o primeiro administrador de forma segura.
- O `appsettings.json` serve só para documentar a forma da seção `Jwt`.
