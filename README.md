# NovaHarvest

## 1. Arranque

### Con Docker

Desde la carpeta raíz del proyecto, ejecuta:

```powershell
docker compose up -d --build
```

Este comando construye las imágenes y arranca los servicios `web` y `backend`.

Para comprobar que están funcionando:

```powershell
docker compose ps
```

Accede a la aplicación desde el navegador:

- Inicio: http://localhost:8080/
- Acerca de: http://localhost:8080/about
- Estado del servidor: http://localhost:8080/status
- Hoja de estilos: http://localhost:8080/css/site.css
- Logotipo: http://localhost:8080/img/logo.png
- Excedente destacado: [http://localhost:8080/surplus](http://localhost:8080/surplus)
- Justificante de reserva: [http://localhost:8080/ticket](http://localhost:8080/ticket)

Para detener y eliminar los contenedores:

```powershell
docker compose down
```

### Sin Docker

Es necesario tener instalado el SDK de .NET 10.

Desde la carpeta raíz del proyecto, ejecuta:

```powershell
cd backend/NovaHarvest.Web
dotnet run
```

La terminal mostrará la dirección local en la que se ejecuta la aplicación. Abre esa dirección en el navegador y añade `/`, `/about`, `/status`, `/surplus` o `/ticket` para consultar las páginas.

Para detener la aplicación, pulsa `Ctrl + C` en la terminal.

## 2. Arquitectura

NovaHarvest utiliza dos servicios conectados mediante Docker Compose: `web` y `backend`.

El funcionamiento de la aplicación es el siguiente:

```text
Navegador
    |
    | http://localhost:8080
    v
web (nginx - puerto 80)
    |
    | http://backend:8080
    v
backend (ASP.NET Core - Kestrel - puerto 8080)
```

El navegador envía las peticiones al puerto 8080 del equipo. Estas llegan al servicio `web`, que utiliza nginx y recibe las peticiones por el puerto 80 del contenedor.

Después, nginx reenvía las peticiones al servicio `backend` mediante la red interna de Docker.

El backend utiliza ASP.NET Core y Kestrel para generar las páginas `/`, `/about`, `/status`, `/surplus` y `/ticket` mediante Razor Pages. Las páginas que necesitan preparar datos utilizan clases PageModel para realizar los cálculos necesarios.

La página `/ticket` tiene su propio documento HTML y no utiliza el layout común del resto de páginas.

También se encarga de servir los archivos estáticos `site.css` y `logo.png`.

El servicio `backend` no publica ningún puerto hacia el exterior. El único puerto publicado es el 8080 del equipo, que corresponde al servicio `web`.

## 3. Páginas

| URL | Archivo .cshtml | PageModel |
|---|---|---|
| `/` | `Pages/Index.cshtml` | No tiene |
| `/surplus` | `Pages/Surplus.cshtml` | `Pages/Surplus.cshtml.cs` (SurplusModel) |
| `/about` | `Pages/About.cshtml` | No tiene |
| `/status` | `Pages/Status.cshtml` | `Pages/Status.cshtml.cs` (StatusModel) |
| `/ticket` | `Pages/Reservations/Ticket.cshtml` | `Pages/Reservations/Ticket.cshtml.cs` (TicketModel) |

### Archivos especiales de Pages

- `_ViewImports.cshtml`: contiene las directivas comunes `@using` y `@namespace` para las páginas Razor.
- `_ViewStart.cshtml`: establece el layout común que utilizan las páginas.
- `Shared/_Layout.cshtml`: contiene la estructura común de la web, con la cabecera, el menú, el contenido y el pie de página.