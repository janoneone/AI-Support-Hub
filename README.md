# 🤖 AI Support Hub

AI Support Hub es una plataforma de soporte empresarial que integra **Inteligencia Artificial, automatización de procesos, gestión de tickets y análisis documental**.

El sistema permite que un usuario converse con un asistente inteligente, detecta solicitudes de soporte, recopila automáticamente la información necesaria, crea un ticket en SQL Server y ejecuta un workflow de n8n que envía una confirmación por correo electrónico.

El proyecto fue desarrollado como una demostración Full Stack de integración entre **React, .NET, SQL Server, Dapper, Google Gemini y n8n**.

---

## 🎯 Objetivo

El objetivo de AI Support Hub es demostrar cómo una aplicación empresarial tradicional puede complementarse con IA y automatización.

Ejemplo:

1. Un usuario escribe: `No puedo acceder al sistema`.
2. La IA identifica una solicitud de soporte.
3. El chatbot solicita nombre, correo y descripción.
4. .NET crea un ticket.
5. Dapper almacena la información en SQL Server.
6. El backend ejecuta un Webhook de n8n.
7. n8n envía un correo de confirmación mediante Gmail.
8. El administrador puede consultar tickets, conversaciones y estadísticas.

---

# 🏗️ Arquitectura

```text
                         ┌─────────────────────┐
                         │   Google Gemini AI  │
                         └──────────▲──────────┘
                                    │
                                    │
┌──────────────────┐      ┌─────────┴─────────┐
│      React       │      │    ASP.NET Core   │
│    TypeScript    │─────▶│      Web API      │
│       Vite       │◀─────│       .NET        │
└──────────────────┘      └─────────┬─────────┘
                                    │
                         ┌──────────▼──────────┐
                         │     SQL Server      │
                         │       Dapper        │
                         └──────────┬──────────┘
                                    │
                                    │ Ticket creado
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │        n8n          │
                         │      Webhook        │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │       Gmail         │
                         │       OAuth2        │
                         └─────────────────────┘
```

---

# ⚙️ Tecnologías

## Frontend

- React
- TypeScript
- Vite
- CSS
- Fetch API

## Backend

- C#
- .NET
- ASP.NET Core Web API
- REST API
- Dependency Injection
- HttpClient

## Base de datos

- Microsoft SQL Server
- Dapper
- Microsoft.Data.SqlClient

## Inteligencia Artificial

- Google Gemini API
- Clasificación de intención
- Chat contextual
- Análisis documental
- Contexto basado en documentos PDF

## Automatización

- n8n
- Webhooks
- Gmail
- OAuth2

## Procesamiento documental

- PDF
- PdfPig
- Extracción de contenido
- Contexto documental para IA

---

# ✨ Funcionalidades

AI Support Hub incluye actualmente:

- Chat con Inteligencia Artificial
- Integración con Google Gemini
- Historial de conversaciones
- Creación de nuevas conversaciones
- Renombrado de conversaciones
- Eliminación de conversaciones
- Persistencia de mensajes en SQL Server
- Detección de solicitudes de soporte
- Flujo automático de recopilación de datos
- Validación básica de correo electrónico
- Generación automática de tickets
- Numeración de tickets `SUP-XXXXXX`
- Estados de tickets
- Automatización mediante n8n
- Envío de correos mediante Gmail OAuth2
- Correos HTML personalizados
- Dashboard administrativo
- Estadísticas de tickets
- Listado de tickets recientes
- Carga de documentos PDF
- Extracción del contenido de documentos
- Uso de documentos como contexto para la IA

---

# 💬 Flujo del chatbot

Una conversación de soporte puede comenzar así:

```text
Usuario:
No puedo acceder al sistema.

        ↓

IA detecta solicitud de soporte

        ↓

Asistente:
Voy a ayudarte a generar una solicitud de soporte.
¿Cuál es tu nombre?

        ↓

WAITING_NAME

        ↓

Asistente:
¿Cuál es tu correo electrónico?

        ↓

WAITING_EMAIL

        ↓

Asistente:
Describe brevemente el problema.

        ↓

WAITING_DESCRIPTION

        ↓

Ticket creado
```

Por ejemplo:

```text
SUP-000004
```

---

# 🔄 Estados del flujo de soporte

Las conversaciones pueden utilizar los siguientes estados:

```text
NORMAL
WAITING_NAME
WAITING_EMAIL
WAITING_DESCRIPTION
TICKET_CREATED
```

Esto permite mantener el estado de la conversación en SQL Server y continuar el flujo entre diferentes mensajes HTTP.

---

# 🎫 Tickets

Los tickets contienen información como:

```text
Ticket Number
Conversation ID
Nombre
Correo
Descripción
Estado
Fecha de creación
```

Los estados disponibles son:

```text
OPEN
IN_PROGRESS
CLOSED
```

Ejemplo:

```json
{
  "ticketNumber": "SUP-000004",
  "conversationId": 7,
  "name": "Usuario Demo",
  "email": "demo@example.com",
  "description": "No puedo acceder con mis credenciales",
  "status": "OPEN"
}
```

---

# 🔄 Automatización con n8n

Cuando .NET crea un ticket correctamente, envía los datos a un Webhook de n8n.

```text
Ticket creado
      ↓
.NET Web API
      ↓
HTTP POST
      ↓
n8n Webhook
      ↓
Edit Fields
      ↓
Gmail
      ↓
Respond to Webhook
      ↓
.NET recibe confirmación
```

El workflow se encuentra en:

```text
n8n/support-ticket-workflow.json
```

La documentación específica está disponible en:

```text
n8n/README.md
```

---

# 📧 Confirmación por correo

n8n utiliza Gmail OAuth2 para enviar una confirmación al usuario.

El correo incluye:

- Nombre del usuario
- Número del ticket
- Estado
- Descripción del problema
- Confirmación del registro

El correo utiliza HTML y estilos inline para mantener compatibilidad con clientes de correo.

---

# 📊 Dashboard

El proyecto incluye un dashboard administrativo conectado directamente al backend.

Actualmente muestra:

- Total de tickets
- Tickets abiertos
- Tickets cerrados
- Total de conversaciones
- Tickets recientes

Los datos provienen directamente de SQL Server.

Endpoints:

```http
GET /api/dashboard/summary
```

y:

```http
GET /api/tickets
```

---

# 📄 Documentos e IA

AI Support Hub también permite utilizar documentos como contexto para el asistente.

Actualmente soporta documentos PDF.

El flujo es:

```text
PDF
 ↓
.NET API
 ↓
PdfPig
 ↓
Extracción de texto
 ↓
SQL Server
 ↓
Contexto documental
 ↓
Gemini
 ↓
Respuesta
```

Esto permite que el chatbot responda preguntas utilizando información proveniente de documentos cargados al sistema.

---

# 🗄️ Base de datos

El proyecto utiliza Microsoft SQL Server.

Las principales tablas son:

```text
Conversations
Messages
Tickets
Documents
```

Los scripts se encuentran en:

```text
database/
```

Ejecutar en el siguiente orden:

```text
01_create_database.sql
02_create_tables.sql
```

Opcionalmente:

```text
03_seed_demo_data.sql
```

El script de datos demo permite probar algunas funcionalidades sin consumir la API de Gemini.

---

# 📁 Estructura

```text
AI-Support-Hub/
│
├── backend/
│   └── AiSupportHub.Api/
│       ├── Controllers/
│       ├── Models/
│       ├── Repositories/
│       ├── Services/
│       ├── Program.cs
│       └── AiSupportHub.Api.csproj
│
├── frontend/
│   ├── public/
│   ├── src/
│   │   ├── pages/
│   │   ├── App.tsx
│   │   └── App.css
│   ├── package.json
│   └── vite.config.ts
│
├── database/
│   ├── 01_create_database.sql
│   ├── 02_create_tables.sql
│   ├── 03_seed_demo_data.sql
│   └── README.md
│
├── n8n/
│   ├── support-ticket-workflow.json
│   └── README.md
│
├── docs/
│
├── .gitignore
└── README.md
```

---

# 🚀 Instalación

## Requisitos

Para ejecutar el proyecto localmente se necesita:

- .NET SDK
- Node.js
- npm
- SQL Server
- n8n
- Cuenta de Google
- API Key de Google Gemini

---

# 🔧 Backend

Entrar al proyecto:

```powershell
cd backend/AiSupportHub.Api
```

Restaurar paquetes:

```powershell
dotnet restore
```

Compilar:

```powershell
dotnet build
```

Configurar secretos:

### Gemini

```powershell
dotnet user-secrets set "Gemini:ApiKey" "TU_API_KEY"
```

### SQL Server

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "TU_CONNECTION_STRING"
```

### n8n

```powershell
dotnet user-secrets set "N8n:SupportWebhookUrl" "TU_WEBHOOK_N8N"
```

Ejecutar:

```powershell
dotnet run
```

---

# ⚛️ Frontend

Entrar a:

```powershell
cd frontend
```

Instalar dependencias:

```powershell
npm install
```

Ejecutar en desarrollo:

```powershell
npm run dev
```

Compilar:

```powershell
npm run build
```

---

# 🤖 Configuración de Gemini

El backend utiliza una abstracción:

```text
IAIService
    ↑
GeminiService
```

Esto permite desacoplar el proveedor de IA del resto de la aplicación.

La API Key no debe escribirse directamente en el código.

En desarrollo se recomienda utilizar:

```powershell
dotnet user-secrets
```

En producción debe utilizarse un gestor de secretos o variables de entorno.

---

# 🔌 Configuración de n8n

Importar:

```text
n8n/support-ticket-workflow.json
```

Después:

1. Configurar Gmail OAuth2.
2. Asignar la credencial al nodo Gmail.
3. Revisar el Webhook.
4. Activar el workflow.
5. Copiar la Production URL.
6. Configurarla en el backend.

Ejemplo:

```powershell
dotnet user-secrets set "N8n:SupportWebhookUrl" "http://localhost:5678/webhook/support-ticket"
```

---

# 🔐 Seguridad

Este repositorio no debe contener credenciales reales.

No deben publicarse:

```text
Gemini API Keys
Passwords de SQL Server
OAuth tokens
OAuth client secrets
Credenciales de Gmail
Archivos .env
User Secrets
```

Las credenciales deben configurarse de manera independiente en cada ambiente.

---

# 🌐 Principales endpoints

## Chat

```http
POST /api/chat
```

## Conversaciones

```http
GET /api/conversations
```

## Mensajes de una conversación

```http
GET /api/conversations/{id}/messages
```

## Tickets

```http
GET /api/tickets
```

## Ticket específico

```http
GET /api/tickets/{id}
```

## Actualizar estado

```http
PUT /api/tickets/{id}/status
```

## Dashboard

```http
GET /api/dashboard/summary
```

## Documentos

Los endpoints de documentos permiten cargar y procesar contenido utilizado posteriormente como contexto del asistente.

---

# 🧠 Conceptos demostrados

Este proyecto busca demostrar conocimientos en:

- Arquitectura Full Stack
- React
- TypeScript
- C#
- ASP.NET Core
- REST APIs
- SQL Server
- Dapper
- Dependency Injection
- Repository Pattern
- Integraciones HTTP
- Inteligencia Artificial
- LLM
- Prompt Engineering
- Automatización de procesos
- n8n
- Webhooks
- OAuth2
- Gmail API
- Procesamiento documental
- Persistencia de conversaciones
- Gestión de tickets
- Separación de responsabilidades

---

# 🛣️ Roadmap

Próximas mejoras consideradas:

- [ ] Gestión completa de tickets desde React
- [ ] Cambio de estado desde el panel administrativo
- [ ] Vista detallada de conversaciones
- [ ] Administración visual de documentos
- [ ] Autenticación de administradores
- [ ] JWT
- [ ] Roles y permisos
- [ ] Gráficos del dashboard
- [ ] Métricas de tiempos de resolución
- [ ] Búsqueda de tickets
- [ ] Filtros y paginación
- [ ] RAG con embeddings
- [ ] Vector database
- [ ] Docker
- [ ] Docker Compose
- [ ] CI/CD con GitHub Actions
- [ ] Despliegue público

---

# 👨‍💻 Propósito

AI Support Hub fue desarrollado como proyecto de portafolio para demostrar la integración de tecnologías de desarrollo de software, inteligencia artificial y automatización de procesos dentro de una solución empresarial.

El proyecto no busca solamente implementar un chatbot, sino demostrar un flujo completo:

```text
Usuario
   ↓
Frontend
   ↓
Backend
   ↓
Inteligencia Artificial
   ↓
Base de datos
   ↓
Ticket
   ↓
Automatización
   ↓
Servicio externo
   ↓
Confirmación
```

---

# 📌 Estado

🚧 Proyecto en desarrollo.

Las funcionalidades principales se encuentran implementadas, mientras que el panel administrativo, despliegue y capacidades avanzadas de IA continúan en desarrollo.