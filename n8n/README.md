# ⚡ AI Support Hub - n8n Automation

Este directorio contiene el workflow de **n8n** utilizado por AI Support Hub para automatizar el proceso posterior a la creación de un ticket de soporte.

El backend .NET crea el ticket en SQL Server y posteriormente ejecuta un Webhook de n8n.

n8n procesa la información, envía un correo mediante Gmail y devuelve una confirmación al backend.

---

# 🔄 Workflow

```text
.NET Web API
     │
     │ HTTP POST
     ▼
┌─────────────┐
│   Webhook   │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ Edit Fields │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│    Gmail    │
│   OAuth2    │
└──────┬──────┘
       │
       ▼
┌────────────────────┐
│ Respond to Webhook │
└─────────┬──────────┘
          │
          ▼
      .NET API
```

---

# 📁 Archivo

El workflow se encuentra en:

```text
support-ticket-workflow.json
```

Este archivo puede importarse directamente en una instancia de n8n.

---

# 📥 Importar workflow

Desde n8n:

1. Crear o abrir un proyecto.
2. Seleccionar la opción para importar un workflow.
3. Seleccionar:

```text
support-ticket-workflow.json
```

4. Abrir el nodo Gmail.
5. Configurar una credencial OAuth2 propia.
6. Revisar el Webhook.
7. Activar el workflow.

---

# 🌐 Webhook

El workflow espera una solicitud:

```http
POST /webhook/support-ticket
```

En modo test n8n puede utilizar:

```text
/webhook-test/support-ticket
```

La URL exacta dependerá de dónde esté desplegada la instancia de n8n.

---

# 📦 Payload

El backend envía un objeto similar a:

```json
{
  "ticketNumber": "SUP-000004",
  "conversationId": 7,
  "name": "Usuario Demo",
  "email": "demo@example.com",
  "description": "No puedo acceder con mis credenciales",
  "status": "OPEN",
  "createdAt": "2026-08-17T03:57:54.6457312Z"
}
```

---

# 🧩 Edit Fields

El nodo `Edit Fields` transforma la entrada del Webhook.

La entrada original tiene la estructura:

```text
body.ticketNumber
body.conversationId
body.name
body.email
body.description
body.status
body.createdAt
```

Después de `Edit Fields` queda:

```text
ticketNumber
conversationId
name
email
description
status
createdAt
```

Esto simplifica el uso de los datos en los siguientes nodos.

---

# 📧 Gmail

El workflow utiliza el nodo Gmail para enviar una confirmación al correo proporcionado por el usuario.

La autenticación se realiza mediante:

```text
Gmail OAuth2
```

La dirección del destinatario se obtiene dinámicamente:

```text
{{ $json.email }}
```

El asunto utiliza:

```text
Ticket de soporte {{ $json.ticketNumber }}
```

El correo incluye:

- Nombre
- Número del ticket
- Estado
- Descripción
- Mensaje de confirmación

El contenido utiliza HTML y estilos inline.

---

# 🔐 Gmail OAuth2

El workflow publicado en este repositorio **no contiene credenciales OAuth2**.

Después de importarlo se debe configurar una cuenta propia.

En el nodo:

```text
Send a message
```

seleccionar:

```text
Credential to connect with
→ Gmail OAuth2
```

y completar el proceso de autorización de Google.

---

# ↩️ Respond to Webhook

Después de enviar correctamente el correo, n8n devuelve al backend:

```json
{
  "success": true,
  "emailSent": true,
  "ticketNumber": "SUP-000004",
  "message": "Ticket procesado y correo enviado correctamente"
}
```

El backend puede utilizar esta información para confirmar que la automatización fue ejecutada.

---

# 🔗 Configuración en .NET

La URL del Webhook no debe escribirse directamente en el código.

En desarrollo se puede utilizar .NET User Secrets:

```powershell
dotnet user-secrets set "N8n:SupportWebhookUrl" "URL_DEL_WEBHOOK"
```

Por ejemplo, para una instalación local:

```powershell
dotnet user-secrets set "N8n:SupportWebhookUrl" "http://localhost:5678/webhook/support-ticket"
```

---

# 🧪 Test URL vs Production URL

n8n proporciona dos URLs.

## Test

```text
http://localhost:5678/webhook-test/support-ticket
```

Se utiliza durante desarrollo y pruebas.

Normalmente requiere que n8n esté esperando el evento de prueba.

## Production

```text
http://localhost:5678/webhook/support-ticket
```

Se utiliza cuando el workflow está activo.

Para una aplicación desplegada se debe utilizar la URL de producción.

---

# ⚙️ Flujo completo

```text
Usuario reporta problema
        ↓
React
        ↓
.NET API
        ↓
Se crea ticket
        ↓
SQL Server
        ↓
.NET llama n8n
        ↓
Webhook
        ↓
Edit Fields
        ↓
Gmail OAuth2
        ↓
Correo enviado
        ↓
Respond to Webhook
        ↓
.NET recibe confirmación
```

---

# 🛡️ Seguridad

El JSON disponible en este repositorio fue preparado para no almacenar credenciales reales.

No deben agregarse al workflow publicado:

- OAuth Access Tokens
- Refresh Tokens
- Client Secrets
- Passwords
- API Keys
- Credenciales de Gmail

Cada instalación debe configurar sus propias credenciales.

---

# 🚀 Despliegue

En desarrollo, n8n puede ejecutarse localmente:

```text
http://localhost:5678
```

Para un ambiente público, n8n debe estar desplegado en un servidor accesible desde Internet.

El backend deberá utilizar la Production URL correspondiente.

Ejemplo conceptual:

```text
https://automation.example.com/webhook/support-ticket
```

---

# 🔧 Requisitos

Para utilizar este workflow se necesita:

- n8n
- Cuenta Google
- Gmail OAuth2 configurado
- AI Support Hub Backend
- Acceso HTTP entre .NET y n8n

---

# 🛣️ Próximas mejoras

El workflow puede evolucionar para incluir:

- [ ] Notificación al equipo de soporte
- [ ] Clasificación automática de prioridad
- [ ] Asignación automática de tickets
- [ ] Diferentes workflows según tipo de problema
- [ ] Escalamiento de tickets críticos
- [ ] Integración con Microsoft Teams
- [ ] Integración con Slack
- [ ] Registro de métricas
- [ ] SLA y alertas
- [ ] Notificación al cerrar el ticket
- [ ] Encuesta de satisfacción

---

# 📌 Nota

Este workflow forma parte del proyecto **AI Support Hub** y demuestra la integración entre una aplicación .NET y una plataforma de automatización mediante Webhooks.

El workflow publicado funciona como plantilla y requiere configurar credenciales propias antes de ser utilizado.