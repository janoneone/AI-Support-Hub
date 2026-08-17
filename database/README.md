# AI Support Hub - Database

La aplicación utiliza SQL Server como motor de base de datos y Dapper
para el acceso a datos desde el backend .NET.

## Tablas

- Conversations
- Messages
- Tickets
- Documents

## Instalación

Ejecutar los scripts en este orden:

1. `01_create_database.sql`
2. `02_create_tables.sql`
3. `03_seed_demo_data.sql` opcional

El script de seed agrega información demostrativa para probar el
dashboard sin utilizar la API de Gemini.

## Configuración

La cadena de conexión no se almacena en el repositorio.

En desarrollo se recomienda utilizar .NET User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "TU_CONNECTION_STRING"