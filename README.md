# SAILibrary - C# Client for AI Services

[![NuGet](https://img.shields.io/nuget/v/Sai_Library.svg)](https://nuget.org/packages/Sai_Library)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com)
[![GitHub Actions](https://github.com/<elias-chacon>/<csharp-sai-library>/workflows/Publish/badge.svg)](https://github.com/<elias-chacon>/<csharp-sai-library>/actions)

Uma biblioteca C# robusta e type-safe para interagir com serviços de IA, baseada na biblioteca Java original. Projetada com princípios SOLID, Clean Code e padrões de projeto modernos.

## 📦 Características

- **✅ Completo** - Suporte completo para chat, modelos, templates, conversas, arquivos, espaços de trabalho e mais
- **🔧 Configurável** - Builders para configuração flexível com logging, retry e timeouts personalizados
- **🛡️ Resiliente** - Padrões de retry exponencial e decorators HTTP para maior confiabilidade
- **🔒 Type-safe** - Tipos seguros em tempo de compilação e estruturas imutáveis onde apropriado
- **🧩 Extensível** - Arquitetura baseada em interfaces permite customização fácil
- **📊 Produto** - Logging detalhado e tratamento de erros robusto com padrão Result<T>

## 🚀 Instalação

### NuGet (recomendado)

```powershell
Install-Package Sai_Library
```

### .NET CLI

```bash
dotnet add package Sai_Library
```

### PackageReference

```xml
<PackageReference Include="Sai_Library" Version="1.0.0" />
```

### GitHub Packages (alternativo)

Adicione ao seu `NuGet.config`:

```xml
<packageSources>
    <add key="github" value="https://nuget.pkg.github.com/elias-chacon/index.json" />
</packageSources>
```

## ⚙️ Configuração Rápida

### Variáveis de Ambiente (opcional)

```bash
export SAI_API_KEY="sua-chave-api"
export SAI_API_BASE_URL="https://api.servicedominio.com"
```

### Criação Simples

```csharp
using Sai_Library;

var sai = SAILibrary.Factory.Create("sua-chave-api");
```

### Configuração Avançada com Builder

```csharp
var sai = new SAILibrary.ConfigBuilder()
    .WithApiKey("sua-chave-api")
    .WithBaseUrl("https://api.servicedominio.com")
    .WithTimeout(60) // segundos
    .EnableRequestLogging() // Logs detalhados
    .EnableRetryLogic(3) // 3 tentativas com backoff exponencial
    .Build();
```

## 🎯 Uso Básico

### Teste de Conexão

```csharp
var health = sai.TestConnection();
if (health.IsSuccess)
{
    Console.WriteLine("✅ Conectado com sucesso!");
}
else
{
    Console.WriteLine($"❌ Falha na conexão: {health.ErrorMessage}");
}
```

### Gerenciamento de Modelos

```csharp
// Carregar modelos disponíveis
sai.RefreshModels();

// Listar modelos por categoria
var chatModels = sai.GetChatModels();
var audioModels = sai.GetAudioModels();
var imageModels = sai.GetImageModels();

// Selecionar modelo para uso
sai.SetModel("gpt-4-turbo");
```

### Envio de Mensagem Simples

```csharp
var response = sai.SendMessage(
    "Explique o conceito de machine learning",
    "Você é um assistente de IA útil",
    new Dictionary<string, object>
    {
        ["temperature"] = 0.7,
        ["max_tokens"] = 1000
    }
);

if (response.IsSuccess)
{
    var texts = SAILibrary.Extensions.ExtractTextFromChatResponse(response);
    foreach (var text in texts)
    {
        Console.WriteLine($"🤖: {text}");
    }
}
```

## 🔌 Serviços Disponíveis

A biblioteca oferece 12 serviços especializados:

### Chat Service
```csharp
var chat = sai.Chat();
var messages = new List<Dictionary<string, object>>
{
    new ChatMessage("user", "Olá, como você está?").ToDictionary()
};
var completion = chat.SendCompletion(messages, "gpt-4");
```

### Model Service
```csharp
var models = sai.Models();
var allModels = models.GetModels();
var realtimeModels = models.GetRealtimeModels();
```

### Template Service
```csharp
var templates = sai.Templates();
var templateList = templates.GetTemplates(new Dictionary<string, object>
{
    ["category"] = "finance"
});

var execution = templates.ExecuteTemplate(
    "template-id",
    new Dictionary<string, object> { ["input1"] = "valor" },
    new Dictionary<string, object> { ["workspaceId"] = "workspace-123" }
);
```

### File Service (Upload Assíncrono)
```csharp
var files = sai.Files();
var uploadResult = await files.UploadFileAsync("/caminho/para/arquivo.pdf", "gpt-4-vision");

// Obter token de upload
var token = files.GetUploadToken(containerName: "uploads", filename: "document.pdf");
```

### Conversation Service
```csharp
var conversations = sai.Conversations();
var newConversation = conversations.CreateConversation(
    "Minha Conversa Técnica",
    "template-id",
    "workspace-id"
);
```

### Todos os Serviços
```csharp
sai.Health()          // HealthService - Verificação de saúde
sai.Profile()         // ProfileService - Perfil do usuário
sai.Models()          // ModelService - Gerenciamento de modelos
sai.Chat()            // ChatService - Conversação
sai.Templates()       // TemplateService - Templates
sai.Conversations()   // ConversationService - Conversas
sai.Workspaces()      // WorkspaceService - Espaços de trabalho
sai.ToolHistory()     // ToolHistoryService - Histórico
sai.Categories()      // CategoryService - Categorias
sai.Files()           // FileService - Arquivos
sai.UserSecrets()     // UserSecretsService - Segredos
sai.Notifications()   // NotificationService - Notificações
```

## 🖼️ Mensagens Multimodais

```csharp
// Mensagem de texto simples
ChatMessage textMessage = sai.CreateMessage("user", "Olá mundo");

// Mensagem com imagem
ChatMessage imageMessage = sai.CreateMessageWithImage(
    "user",
    "Descreva esta imagem",
    "https://example.com/image.jpg",
    "high" // detalhe: low, high, auto
);

// Uso em conversas
var messages = new List<Dictionary<string, object>>
{
    textMessage.ToDictionary(),
    imageMessage.ToDictionary()
};

var response = sai.SendChatWithHistory(messages);
```

## 🏗️ Padrões Avançados

### Decorators HTTP
```csharp
IHttpClientBase client = new SystemNetHttpClient();
client = new LoggingHttpClient(client);  // Adiciona logging
client = new RetryHttpClient(client, 3); // Adiciona retry com backoff

SAILibrary sai = SAILibrary.Factory.CreateWithCustomHttpClient(
    "api-key",
    "base-url",
    client
);
```

### Retry com Backoff Exponencial
```csharp
// Via ConfigBuilder (automático)
var sai = new SAILibrary.ConfigBuilder()
    .EnableRetryLogic(5) // 5 tentativas
    .Build();

// Via Extensions (manual)
Result<JsonNode> result = SAILibrary.Extensions.SendMessageWithRetry(
    sai,
    "Mensagem importante",
    "Sistema de contexto",
    3
);
```

### Builder Pattern Avançado
```csharp
SAILibrary sai = new SAILibrary.ConfigBuilder()
    .WithApiKey(Environment.GetEnvironmentVariable("SAI_API_KEY"))
    .WithBaseUrl("https://api.custom-domain.com")
    .WithTimeout(120)        // 2 minutos
    .EnableRequestLogging()  // Logs detalhados
    .EnableRetryLogic(5)     // 5 tentativas com backoff
    .Build();
```

## 🚨 Tratamento de Erros

### Padrão Result<T>
```csharp
Result<JsonNode> result = sai.SendMessage("mensagem");

if (result.IsSuccess)
{
    // Processar dados com segurança
    JsonNode data = result.Data;
    var metadata = result.Metadata;
    
    if (metadata.TryGetValue("status", out var status))
    {
        Console.WriteLine($"Status HTTP: {status}");
    }
}
else
{
    // Tratamento consistente de erros
    Console.Error.WriteLine($"Erro: {result.ErrorMessage}");
    
    // Metadata pode conter informações adicionais
    foreach (var meta in result.Metadata)
    {
        Console.WriteLine($"{meta.Key}: {meta.Value}");
    }
}
```

### Exceções para Erros de Configuração
```csharp
try
{
    SAILibrary sai = SAILibrary.Factory.Create(null); // API key ausente
}
catch (ArgumentException e)
{
    Console.Error.WriteLine($"Configuração inválida: {e.Message}");
}

try
{
    sai.SetModel("modelo-inexistente");
}
catch (InvalidOperationException e)
{
    Console.Error.WriteLine($"Modelo não disponível: {e.Message}");
}
```

## 🔧 Utilitários

### Extração de Texto de Respostas
```csharp
Result<JsonNode> chatResponse = sai.SendMessage("Pergunta técnica");
List<string> texts = SAILibrary.Extensions.ExtractTextFromChatResponse(chatResponse);

foreach (var text in texts)
{
    Console.WriteLine($"Resposta: {text}");
}
```

### Análise de Contexto de Conversação
```csharp
List<Dictionary<string, object>> messages = new()
{
    new Dictionary<string, object> { ["role"] = "user", ["content"] = "Olá" },
    new Dictionary<string, object> { ["role"] = "assistant", ["content"] = "Oi! Como posso ajudar?" },
    new Dictionary<string, object> { ["role"] = "user", ["content"] = "Preciso de ajuda com C#" }
};

var context = SAILibrary.Extensions.CreateConversationContext(messages);
// Resultado: 
// {
//   "MessageCount": 3,
//   "Roles": ["user", "assistant", "user"],
//   "TotalLength": 43,
//   "UniqueRoles": ["user", "assistant"]
// }
```

## 📁 Estrutura do Projeto

```
Sai_Library/
├── Enums/
│   ├── Env.cs              # Variáveis de ambiente (SAI_API_KEY, SAI_API_BASE_URL)
│   ├── ModelType.cs        # Tipos de modelo: Chat, Audio, Image
│   └── RequestMethod.cs    # Métodos HTTP: GET, POST, PUT, PATCH, DELETE
├── Http/
│   ├── IHttpClientBase.cs      # Interface base para clientes HTTP
│   ├── SystemNetHttpClient.cs  # Implementação usando HttpClient do .NET
│   ├── LoggingHttpClient.cs    # Decorator para logging de requisições
│   └── RetryHttpClient.cs      # Decorator para retry com backoff exponencial
├── Models/
│   ├── ChatMessage.cs      # Modelo para mensagens de chat (suporte a imagens)
│   └── Result.cs           # Resultado type-safe para todas as operações
├── Services/
│   ├── BaseService.cs           # Classe base abstrata para todos os serviços
│   ├── HealthService.cs         # Verificação de saúde da API
│   ├── ProfileService.cs        # Informações do perfil do usuário
│   ├── ModelService.cs          # Gerenciamento de modelos disponíveis
│   ├── ChatService.cs           # Envio de mensagens e completions
│   ├── TemplateService.cs       # Execução e gerenciamento de templates
│   ├── ConversationService.cs   # Gerenciamento de conversas
│   ├── WorkspaceService.cs      # Espaços de trabalho
│   ├── ToolHistoryService.cs    # Histórico de ferramentas
│   ├── CategoryService.cs       # Categorias
│   ├── FileService.cs           # Upload/download de arquivos
│   ├── UserSecretsService.cs    # Gerenciamento de segredos
│   └── NotificationService.cs   # Notificações
├── Utils/
│   └── UriBuilder.cs       # Construtor de URLs com query parameters
└── SAILibrary.cs           # Classe principal com Factory, ConfigBuilder e Extensions
```

## 🛠️ Requisitos

- **.NET 8.0** ou superior
- **Dependências NuGet**:
    - `Newtonsoft.Json` (13.0.3+)
    - `System.Text.Json` (8.0.4+)
    - `Microsoft.Extensions.Logging.Abstractions` (8.0.0+)

## 🚢 Publicação

### GitHub Packages (automático)
O projeto está configurado para publicação automática no GitHub Packages:

```bash
# 1. Atualize a versão no .csproj
# 2. Crie uma tag
git tag v1.0.1
git push origin v1.0.1

# 3. O GitHub Actions publica automaticamente
```

### Publicação Manual
```bash
dotnet pack --configuration Release
dotnet nuget push bin/Release/*.nupkg --source https://nuget.pkg.github.com/elias-chacon/index.json
```

## 📋 Exemplo Completo

```csharp
using Sai_Library;
using System.Text.Json.Nodes;

class Program
{
    static async Task Main(string[] args)
    {
        // Configuração
        var ai = new SAILibrary.ConfigBuilder()
            .WithApiKey(Environment.GetEnvironmentVariable("SAI_API_KEY"))
            .WithBaseUrl("https://api.ai-service.com")
            .EnableRequestLogging()
            .EnableRetryLogic(3)
            .Build();

        // Health check
        if (!ai.TestConnection().IsSuccess)
        {
            Console.Error.WriteLine("❌ Falha na conexão");
            return;
        }

        // Carregar e selecionar modelo
        ai.RefreshModels();
        ai.SetModel("gpt-4-turbo");

        // Enviar mensagem com contexto
        var response = ai.SendMessage(
            "Explique blockchain em termos simples para um iniciante",
            "Você é um professor de tecnologia paciente e didático",
            new Dictionary<string, object>
            {
                ["temperature"] = 0.5,
                ["max_tokens"] = 500,
                ["seed"] = 42
            }
        );

        // Processar resposta
        if (response.IsSuccess)
        {
            var texts = SAILibrary.Extensions.ExtractTextFromChatResponse(response);
            Console.WriteLine($"📚 Resposta ({texts.Count} parte(s)):");
            foreach (var text in texts)
            {
                Console.WriteLine($"\n{text}\n");
            }
        }
        else
        {
            Console.WriteLine($"⚠️ Erro: {response.ErrorMessage}");
        }

        // Informações da API
        var apiInfo = ai.GetApiInfo();
        Console.WriteLine($"\n📊 Status: {apiInfo["SelectedModel"]}");
        Console.WriteLine($"Modelos carregados: {apiInfo["AvailableModelsCount"]}");
    }
}
```

## 🤝 Contribuição

1. **Fork** o repositório
2. Crie uma **branch** para sua feature:
   ```bash
   git checkout -b feature/nova-funcionalidade
   ```
3. **Commit** suas mudanças:
   ```bash
   git commit -am 'Adiciona nova funcionalidade'
   ```
4. **Push** para a branch:
   ```bash
   git push origin feature/nova-funcionalidade
   ```
5. Abra um **Pull Request**

## 📄 Licença

Este projeto está licenciado sob a **MIT License** - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🔗 Links

- **Repositório**: [https://github.com/elias-chacon/csharp-sai-library](https://github.com/elias-chacon/csharp-sai-library)
- **Issues**: [https://github.com/elias-chacon/csharp-sai-library/issues](https://github.com/elias-chacon/csharp-sai-library/issues)
- **NuGet**: [https://www.nuget.org/packages/Sai_Library](https://www.nuget.org/packages/Sai_Library)
- **Documentação da API**: [https://api.ai-service.com/docs](https://api.ai-service.com/docs)

---

Desenvolvido com ❤️ por [Seu Nome]. Baseado na biblioteca Java original de [Elias Alves Chacon](https://github.com/elias-chacon/csharp-sai-library).