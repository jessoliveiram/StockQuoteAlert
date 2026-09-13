# StockQuoteAlert

Aplicação .NET que monitora a cotação de uma ação e envia alertas por e-mail quando o preço atinge as condições configuradas.

A aplicação utiliza o **State Machine Pattern** para gerenciar os estados de compra, venda e neutro, consultando a API BRAPI para obter cotações em tempo real.

## Pré-requisitos

- Docker e Docker Compose, para executar em containers
- .NET SDK 10, para executar diretamente com .NET

## 1. Executar com Docker Compose

Nesta opção, o e-mail é enviado para o MailPit (servidor SMTP de teste). Não é necessário preencher `.env`.

Execute a aplicação informando ticker, preço de venda e preço de compra:

```bash
docker compose run --rm --build stockquotealert PETR4 22.67 22.59
```

O comando acima irá:
1. Iniciar o MailPit automaticamente (com health check)
2. Aguardar o MailPit estar saudável antes de iniciar a aplicação
3. Executar o monitoramento da cotação

A interface web para visualizar os e-mails fica disponível em: <http://localhost:8025>

Para parar os containers:

```bash
docker compose down
```

### Testes no Docker

Execute os testes no container do SDK .NET:

```bash
docker compose --profile test run --rm tests
```


## 2. Executar diretamente com .NET

Entre na pasta da aplicação:

```bash
cd StockQuoteAlert
```

### Usar MailPit

Inicie o MailPit na raiz do projeto. Não preencha `.env` para usar o MailPit:

```bash
cd ..
docker compose up -d mailpit
cd StockQuoteAlert
```

Execute a aplicação:

```bash
dotnet run -- PETR4 22.67 22.59
```

Visualize os e-mails em <http://localhost:8025>.

### Usar um servidor SMTP real

Para servidores SMTP reais, defina `EnableSsl: true` no `appsettings.json` e configure `EMAIL_PASSWORD` no `.env`

**Nota**: O Gmail requer uma "Senha de App". É necessário acessar as configurações de segurança da sua conta Google para gerar uma. O endereço em `FromAddress` é usado como usuário SMTP e deve coincidor com o email em que a senha foi gerada. 

Exemplo de configuração do servidor, porta e SSL em `StockQuoteAlert/appsettings.json` para Gmail:

```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "EnableSsl": true,
  "FromAddress": "seu-email@gmail.com"
}
```

Preencha a senha SMTP no `StockQuoteAlert/.env`:

```env
EMAIL_PASSWORD=sua_senha_smtp
```

Execute a aplicação:

```bash
dotnet run -- PETR4 22.67 22.59
```

### Gerar e executar o binário

```bash
dotnet publish -c Release
dotnet ./bin/Release/net10.0/StockQuoteAlert.dll PETR4 22.67 22.59
```

No Linux ou macOS, também é possível executar o binário nativo publicado:

```bash
./bin/Release/net10.0/StockQuoteAlert PETR4 22.67 22.59
```

## Testes com .NET

Na raiz do projeto, execute:

```bash
dotnet test StockQuoteAlert.Tests/StockQuoteAlert.Tests.csproj
```

## Arquitetura

### Padrão State Machine

A aplicação implementa o padrão **State Machine** para gerenciar os estados de alerta:

- **Neutral**: Estado inicial, monitora a cotação aguardando atingir os limites
- **BuyAlert**: Acionado quando o preço cai abaixo do limite de compra
- **SellAlert**: Acionado quando o preço sobe acima do limite de venda

A transição entre estados é automática e um alerta por e-mail é enviado a cada mudança de estado.

### Componentes principais

- **QuoteMonitor**: Orquestra o monitoramento periódico das cotações
- **BRAPIClient**: Cliente HTTP para a API de cotações BRAPI
- **EmailService**: Serviço de envio de e-mails usando FluentEmail e SMTP
- **StockStateContext**: Gerenciador do State Machine
- **States**: Implementações concretas de cada estado (Neutral, BuyAlert, SellAlert)

## Tecnologias utilizadas

- **.NET 10**: Framework principal
- **FluentEmail**: Biblioteca para envio de e-mails
- **BRAPI**: API pública de cotações da bolsa brasileira
- **MailPit**: Servidor SMTP de testes
- **xUnit**: Framework de testes unitários

## Referências

- [BRAPI - API de Cotações](https://brapi.dev/docs)
- [State Pattern - Refactoring Guru](https://refactoring.guru/pt-br/design-patterns/state/csharp/example)
- [FluentEmail - GitHub](https://github.com/lukencode/FluentEmail)
- [Mailtrap - Sending Emails in C#](https://mailtrap.io/blog/csharp-send-email/)
- [MailPit - Inspect captured messages](https://mailpit.axllent.org/docs/usage/)
- [Interfaces in Go and C#](https://rselbach.com/interfaces-go-c/)