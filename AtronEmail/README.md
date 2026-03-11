
# 📧 Atron.Email

O projeto **Atron Email** é o módulo responsável por comunicação via e-mail dentro do ecossistema Atron.  
Ele foi projetado para centralizar, padronizar e abstrair o envio de e-mails transacionais, diagnósticos e notificações, seguindo boas práticas de arquitetura, separação de responsabilidades e extensibilidade.

O módulo permite o envio de e-mails utilizando SMTP tradicional ou APIs de provedores externos, mantendo a aplicação desacoplada da tecnologia específica de envio.

---

## 🎯 Objetivo do Projeto

- Centralizar toda a lógica de envio de e-mails  
- Abstrair o provedor de envio (SMTP, API, etc.)  
- Facilitar testes e diagnósticos  
- Permitir troca de provedor sem impacto na aplicação  
- Garantir padronização visual e estrutural das mensagens  
- Entender o funcionamento de envio e recebimento de e-mails entre softwares

---

## 📂 Estrutura de Pastas

A organização do projeto reflete a separação clara entre contratos, serviços, configurações e infraestrutura de envio.

### 📁 Application

Camada responsável pelos casos de uso relacionados a e-mail.

#### Services

- **SharedEmailService**  
  Serviço principal de envio de e-mails. Implementa `IEmailService`. Suporta envio via SMTP e é responsável por autenticação, montagem e envio da mensagem.

- **EmailNotificationService**  
  Serviço de alto nível para envio de notificações. Encapsula a criação de e-mails padronizados e é utilizado por outros módulos da aplicação.

- **EmailDiagnosticService**  
  Serviço voltado para diagnóstico e verificação de configuração. Permite validar credenciais, servidor SMTP e status do serviço.

---

### 📁 Interfaces

Define os contratos que isolam a aplicação da implementação técnica:

- **IEmailService**  
  Interface base para envio de e-mails. Permite múltiplas implementações (SMTP, API, mock, etc.).

- **IEmailNotificationService**  
  Contrato para envio de notificações de sistema.

- **IEmailDiagnosticService**  
  Contrato para validações e diagnósticos do serviço de e-mail.

---

### 📁 Email (Infraestrutura)

- **EmailProvider**  
  Enumeração dos provedores suportados (Gmail, Outlook, Yahoo, etc.).

- **EmailProviderSettings**  
  Configurações SMTP específicas por provedor.

- **EmailProviderIdentifier**  
  Identifica automaticamente o provedor com base no domínio do e-mail e retorna as configurações apropriadas.

---

### 📁 DTOs

- **EmailRequest**  
  Contém assunto, corpo da mensagem (HTML) e lista de destinatários.

- **EmailStatusResponse**  
  Representa o resultado do envio, incluindo informações de diagnóstico e status do serviço.

---

### 📁 ValueObjects

- **NotificationMessage**  
  Representa mensagens de erro, aviso ou sucesso.

- **NotificationBag**  
  Agrupador de notificações usado para validações e retornos de resultado.

- **Resultado**  
  Encapsula o resultado de operações, suportando sucesso, falha e mensagens associadas.

---

## ⚙️ Configuração

Exemplo de configuração via `appsettings.json`:

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "UseSsl": true,
  "UserName": "seu-email@gmail.com",
  "Password": "senha-de-app",
  "FromName": "Sistema Atron",
  "FromEmail": "seu-email@gmail.com"
}
```

> ⚠️ As credenciais não devem ser versionadas em produção. Utilize variáveis de ambiente.

---

## 🔄 Fluxo de Envio de E-mail

1. Um evento da aplicação dispara o envio  
2. O serviço de notificação constrói o e-mail  
3. O `IEmailService` é acionado  
4. O serviço concreto (SMTP ou API) envia a mensagem  
5. O resultado é retornado com sucesso ou falha

---

## 💡 Princípios Aplicados

- **Inversão de Dependência** – A aplicação depende de abstrações, não de implementações  
- **Single Responsibility** – Cada classe tem um papel claro  
- **Extensibilidade** – Novos provedores podem ser adicionados sem alterar código existente  
- **Fail Fast** – Erros de configuração são detectados rapidamente  
- **Separação de Interesses** – Notificação, envio e diagnóstico são responsabilidades distintas

---

## ✅ Casos de Uso Suportados

- E-mails transacionais  
- Notificações de sistema  
- Alertas para gestores  
- Diagnóstico de configuração  
- Testes de infraestrutura de e-mail

---

## 🚀 Evoluções Futuras

- Implementação via API (Brevo, SendGrid, etc.)  
- Templates dinâmicos  
- Modo mock para testes  
- Suporte a múltiplos remetentes  
- Log estruturado de envio

---

## 🧠 Observação Final

O **Atron.Email** foi pensado para refletir como sistemas profissionais tratam comunicação por e-mail, respeitando segurança, reputação e arquitetura limpa.

> *E-mail não é apenas envio de mensagem — é infraestrutura, confiança e responsabilidade.*
