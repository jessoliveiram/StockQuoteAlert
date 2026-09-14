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

### Usar MailPit

Inicie o MailPit na raiz do projeto. Não preencha `.env` para usar o MailPit:

```bash
docker compose up -d mailpit
```

Entre na pasta `StockQuoteAlert` e execute a aplicação:

```bash
cd StockQuoteAlert
```

```bash
dotnet run PETR4 22.67 22.59
```

Visualize os e-mails em <http://localhost:8025>.

### Usar um servidor SMTP real

As configurações do servidor SMTP podem ser sobrescritas por variáveis de ambiente. Configure as variáveis de acordo com o servidor SMTP escolhido.  

**Nota**: O Gmail requer uma "Senha de App". É necessário acessar as configurações de segurança da sua conta Google para gerar uma. O endereço em `FromAddress` é usado como usuário SMTP e deve coincidir com o e-mail em que a senha foi gerada. 

Copie o arquivo de exemplo `.env.example` para `.env` dentro da pasta `StockQuoteAlert`:

```bash
cp .env.example .env
```

Para Gmail, use as seguintes configurações:

```env
EMAIL_PASSWORD=sua_senha_de_app
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
ENABLE_SSL=true
```

Execute a aplicação:

```bash
dotnet run PETR4 22.67 22.59
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

## Uso de inteligência artificial

Foi utilizado o **GitHub Copilot** como ferramenta de apoio durante o desenvolvimento, nas seguintes atividades:

- elaboração dos testes unitários, sempre com avaliação e validação manual dos resultados;
- esclarecimento sobre a linguagem C# e o ecossistema .NET;
- revisão de código.

A implementação da State Machine foi baseada integralmente na implementação de referência indicada em [State Pattern - Refactoring Guru](https://refactoring.guru/pt-br/design-patterns/state/csharp/example), que também está listada nas referências deste documento.

As decisões técnicas, a definição da arquitetura, a adaptação da referência ao problema e a solução final foram de responsabilidade do autor. O uso da IA serviu como apoio e não substituiu a análise crítica.

## Referências

- [BRAPI - API de Cotações](https://brapi.dev/docs)
- [State Pattern - Refactoring Guru](https://refactoring.guru/pt-br/design-patterns/state/csharp/example)
- [FluentEmail - GitHub](https://github.com/lukencode/FluentEmail)
- [Mailtrap - Sending Emails in C#](https://mailtrap.io/blog/csharp-send-email/)
- [MailPit - Inspect captured messages](https://mailpit.axllent.org/docs/usage/)
- [Interfaces in Go and C#](https://rselbach.com/interfaces-go-c/)

## Evolução do projeto

Como próximos passos para a evolução da aplicação, estão previstos:

- **Observabilidade**: capturar e estruturar os logs da aplicação para facilitar o monitoramento, utilizando o OpenTelemetry. A documentação está disponível em [OpenTelemetry .NET - Logs getting started](https://opentelemetry.io/docs/languages/dotnet/logs/getting-started-console/).
- **Desacoplamento do envio de e-mails**: substituir o envio direto de e-mails por uma arquitetura baseada em mensageria, utilizando, por exemplo, o RabbitMQ. O monitoramento publicará as notificações em uma fila, e um consumidor ficará responsável pelo envio. A documentação está disponível em [RabbitMQ Tutorials](https://www.rabbitmq.com/tutorials).