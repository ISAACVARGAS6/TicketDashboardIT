# Panel de Incidentes

Aplicación inicial para registrar, asignar y supervisar tickets de incidentes internos. El proyecto está pensado como una base limpia para una mesa de ayuda pequeña: presenta los tickets recientes, sus prioridades, responsable y estado operativo.

## Características

- Panel de resumen con tickets abiertos, pendientes, críticos y resueltos.
- Búsqueda y filtros por estado en la interfaz.
- Gestión de incidentes mediante API REST.
- Asignación de cada incidente a un solicitante y, opcionalmente, a un técnico.
- Prioridades baja, media, alta y crítica.
- Estados pendiente, en pausa y resuelto.
- Estado atrasado calculado cuando un incidente pendiente supera su fecha límite.
- Inicio de sesión con JSON Web Tokens (JWT) y roles de administrador, técnico y solicitante.
- Datos de demostración creados automáticamente al iniciar.
- Entorno reproducible con Docker Compose.

## Tecnologías

| Área | Tecnología |
| --- | --- |
| Backend | ASP.NET Core 8 Web API |
| Persistencia | Entity Framework Core 8 |
| Base de datos | PostgreSQL 16 |
| Seguridad | JWT Bearer Authentication y BCrypt |
| Frontend | React, TypeScript, Vite y Tailwind CSS |
| Servidor web | Nginx |
| Contenedores | Docker y Docker Compose |

## Estructura del repositorio

```text
.
├── backend/
│   └── IncidentDashboard.Api/
│       ├── Controllers/      # Endpoints HTTP
│       ├── Data/             # Contexto de Entity Framework Core
│       ├── DTOs/             # Contratos de entrada
│       ├── Models/           # Entidades y enumeraciones
│       └── Services/         # Generación de JWT
├── frontend/
│   └── src/                  # Interfaz React
├── docker-compose.yml
└── README.md
```

## Requisitos

- Docker Engine 24 o posterior
- Docker Compose v2

No es necesario instalar .NET, Node.js ni PostgreSQL en el equipo cuando se utiliza Docker.

## Inicio rápido

1. Clona el repositorio y entra en su carpeta.

   ```bash
   git clone https://github.com/ISAACVARGAS6/TicketDashboardIT.git
   cd TicketDashboardIT
   ```

2. Crea tu archivo local de configuración. No subas este archivo al repositorio.

   ```bash
   cp .env.example .env
   ```

   Abre `.env` y sustituye todos los valores que comienzan con `REEMPLAZA_`.

3. Construye e inicia los servicios.

   ```bash
   docker compose up --build
   ```

4. Abre las siguientes direcciones en el navegador:

   | Servicio | Dirección |
   | --- | --- |
   | Dashboard | http://localhost:5173 |
   | API | http://localhost:8080 |
   | Swagger | http://localhost:8080/swagger |

Para detener los servicios, usa `docker compose down`. La información de PostgreSQL se conserva en el volumen `postgres_data`.

Para eliminar también los datos locales de demostración:

```bash
docker compose down -v
```

## Cuentas iniciales

En el primer inicio, la aplicación crea cuentas iniciales para administrador, técnico y solicitante. Define sus contraseñas en tu archivo local `.env` mediante `SEED_ADMIN_PASSWORD`, `SEED_TECHNICIAN_PASSWORD` y `SEED_REQUESTER_PASSWORD`. El archivo `.env` no se versiona ni debe compartirse.

## Modelo de datos

### Usuario

Representa a quienes participan en la atención de tickets. Un usuario tiene nombre, correo, contraseña cifrada con BCrypt y uno de los roles siguientes:

- `Admin`: administra los incidentes y el sistema.
- `Technician`: recibe y actualiza incidentes asignados.
- `Requester`: registra o consulta sus solicitudes.

### Incidente

| Campo | Descripción |
| --- | --- |
| `title` | Resumen breve del problema. |
| `description` | Detalle del incidente. |
| `priority` | `Low`, `Medium`, `High` o `Critical`. |
| `status` | `Pending`, `Paused` o `Resolved`. |
| `requesterId` | Usuario que reportó el incidente. |
| `technicianId` | Técnico asignado; puede ser nulo. |
| `createdAt` | Fecha de creación en UTC. |
| `dueAt` | Fecha límite opcional en UTC. |
| `resolvedAt` | Fecha de resolución, si corresponde. |

Un ticket aparece como `Overdue` en las respuestas de la API cuando sigue en estado `Pending` y `dueAt` ya venció. No se almacena como un estado separado.

## API

La documentación interactiva está disponible en Swagger. Los endpoints principales son los siguientes:

| Método | Ruta | Autorización | Descripción |
| --- | --- | --- | --- |
| `POST` | `/api/auth/login` | No | Inicia sesión y devuelve un JWT. |
| `GET` | `/api/incidents` | Bearer token | Obtiene los incidentes. Admite `status` y `priority` como filtros. |
| `POST` | `/api/incidents` | Admin o Technician | Crea un incidente. |
| `PATCH` | `/api/incidents/{id}` | Admin o Technician | Actualiza un incidente. |

### Iniciar sesión

```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@incidentes.local",
    "password": "<TU_CONTRASENA_DE_ADMINISTRADOR>"
  }'
```

El resultado contiene `token`. Inclúyelo en solicitudes protegidas con el encabezado `Authorization: Bearer <token>`.

### Crear un incidente

```bash
curl -X POST http://localhost:8080/api/incidents \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{
    "title": "No puedo conectar a la VPN",
    "description": "La conexión falla al validar las credenciales.",
    "priority": "High",
    "requesterId": 3,
    "technicianId": 2,
    "dueAt": "2026-09-13T18:00:00Z"
  }'
```

## Configuración

El repositorio incluye `.env.example` como plantilla. Copíalo a `.env`, completa sus valores y conserva `.env` fuera de Git. El archivo `docker-compose.yml` consume las siguientes variables:

| Variable | Uso |
| --- | --- |
| `POSTGRES_DB` | Nombre de la base de datos. |
| `POSTGRES_USER` | Usuario de PostgreSQL. |
| `POSTGRES_PASSWORD` | Contraseña de PostgreSQL. |
| `ConnectionStrings__DefaultConnection` | Cadena de conexión que utiliza la API. |
| `Jwt__Key` | Clave usada para firmar tokens JWT. |

Antes de desplegar, utiliza un gestor de secretos y valores distintos por entorno. No publiques contraseñas, tokens, archivos `.env`, volcados de la base de datos ni claves JWT.

## Desarrollo local sin Docker

El backend requiere .NET SDK 8 y una instancia de PostgreSQL. El frontend requiere Node.js 20 o posterior.

```bash
# Backend
cd backend/IncidentDashboard.Api
dotnet restore
dotnet run

# En otra terminal, frontend
cd frontend
npm install
npm run dev
```

Al ejecutar el frontend fuera de Docker, configura un proxy local o ajusta la URL base de la API según sea necesario.

## Consideraciones para producción

- Sustituir `Database.EnsureCreated()` por migraciones de Entity Framework Core.
- Guardar secretos y cadenas de conexión fuera del repositorio.
- Restringir CORS a los dominios permitidos.
- Añadir renovación de tokens, control de intentos de inicio de sesión y recuperación de contraseñas.
- Agregar validación de DTOs, registro estructurado, pruebas automatizadas y auditoría de cambios.
- Conectar el dashboard de React directamente a los endpoints autenticados para reemplazar los datos de muestra visuales incluidos en esta primera interfaz.

## Licencia

Puedes añadir aquí la licencia que corresponda a tu proyecto antes de publicarlo.
