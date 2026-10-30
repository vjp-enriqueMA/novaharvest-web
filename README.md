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

La terminal mostrará la dirección local en la que se ejecuta la aplicación. Abre esa dirección en el navegador y añade `/`, `/about` o `/status` para consultar las páginas.

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

El backend utiliza ASP.NET Core y Kestrel para generar las páginas `/`, `/about` y `/status` mediante código C#. También se encarga de servir los archivos estáticos `site.css` y `logo.png`.

El servicio `backend` no publica ningún puerto hacia el exterior. El único puerto publicado es el 8080 del equipo, que corresponde al servicio `web`.