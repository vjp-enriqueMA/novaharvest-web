# Cuestiones teóricas UT2

Proyecto: NovaHarvest

Autor: Enrique Montero Acosta

### 1. Cliente y servidor

Cuando abro la página `/status`, el cliente es el navegador, que envía una petición al servidor para obtener la página.
En NovaHarvest, nginx recibe la petición y la reenvía al backend mediante la red interna de Docker.
El servidor ASP.NET Core ejecuta el código C# de la ruta `/status` y genera el HTML con la fecha y hora actuales.
Después, el navegador recibe la respuesta y muestra la página al usuario.
Cuando NovaHarvest permita reservar productos, la comprobación de que no se reservan más kilos de los disponibles deberá hacerse obligatoriamente en el servidor.
Esto es porque el servidor contiene los datos reales del inventario y no podemos confiar en el cliente, ya que el usuario puede manipularlo.
La comprobación también podría hacerse en el cliente para avisar antes al usuario, pero siempre tendría que repetirse en el servidor.

### 2. Estático o dinámico

En mi proyecto, las páginas de Inicio y Acerca de podrían ser páginas HTML estáticas, porque muestran información que normalmente no cambia.
Sin embargo, la página Estado del servidor necesita ser dinámica porque muestra la fecha y hora actuales cada vez que se abre.
Por eso, en NovaHarvest las tres páginas se generan mediante código C# en el servidor.
Más adelante, la aplicación también necesitará mostrar un catálogo con los productos disponibles y sus cantidades.
Otra página que necesitará será la de reservas, donde cada usuario podrá consultar y reservar productos.
Estas páginas necesitan ser dinámicas porque la información puede cambiar y depende de los datos del momento.

### 3. Ejecución en el servidor

Cuando entro en `http://localhost:8080/status`, el navegador envía una petición al servicio web.
Nginx recibe la petición por el puerto 80 del contenedor y la reenvía al backend mediante `proxy_pass`.
El backend recibe la petición en el puerto 8080 y ASP.NET Core ejecuta la ruta `/status` de `Program.cs`.
Kestrel ejecuta el código C# y genera el HTML con la fecha y hora actuales.
Después, la respuesta vuelve a nginx y este se la envía al navegador.
Con CGI se crearía un proceso nuevo por cada petición, lo que sería más lento.
Con un módulo empotrado el código se ejecutaría dentro del propio servidor web, siendo más rápido, pero quedaría más acoplado a un lenguaje concreto.

### 4. Reverse proxy

Al ejecutar `curl.exe -I http://localhost:8080/status`, la respuesta muestra `Server: nginx/1.27.5`, porque la petición pasa primero por nginx.
Cuando ejecuto el backend directamente con `dotnet run`, la cabecera muestra `Server: Kestrel`, ya que accedo directamente a ASP.NET Core.
Al detener el contenedor del backend, tanto `/status` como `/css/site.css` devuelven un error `502 Bad Gateway`, porque nginx no puede conectarse al backend para obtener la respuesta.
Al volver a arrancarlo, la página `/status` vuelve a responder correctamente con un código `200 OK`.
El backend no tiene puertos publicados porque solo debe recibir peticiones desde la red interna de Docker, a través de nginx, evitando que se acceda directamente desde fuera.
Así, nginx actúa como intermediario entre el navegador y el backend.

### 5. Nginx

En mi proyecto, nginx no sirve directamente el CSS ni el logotipo porque su configuración reenvía todas las peticiones al backend.
Los archivos `site.css` y `logo.png` están dentro de `wwwroot`, y ASP.NET Core se encarga de servirlos mediante `MapStaticAssets()`.
Así, los archivos funcionan tanto cuando ejecuto el proyecto con Docker como cuando utilizo `dotnet run`.
Además de actuar como proxy inverso, nginx puede encargarse de gestionar las conexiones HTTPS.
También puede repartir las peticiones entre varios servidores backend si hubiera más de uno.

### 6. Tecnologías

Si en lugar de ASP.NET Core utilizara Python con Flask, tendría que cambiar el código del backend, especialmente `Program.cs`, y sustituir el proyecto de .NET por los archivos propios de Python.
También tendría que modificar el `Dockerfile` del backend para utilizar el entorno necesario para ejecutar Python.
Sin embargo, podría mantener nginx como proxy inverso y Docker Compose para poner en marcha los dos servicios.
Los archivos `web/Dockerfile` y `web/nginx.conf` podrían quedarse igual si el nuevo backend mantuviera el mismo nombre de servicio y el puerto 8080.
Los archivos CSS y el logotipo también podrían seguir utilizándose, aunque habría que adaptar su ubicación y la forma de servirlos al nuevo backend.

### 7. Marcado y código

En mi proyecto, la página Estado del servidor se genera mediante una cadena de texto en C#:

```csharp
app.MapGet("/status", () => Results.Content(
    $"""
    <!DOCTYPE html>
    <html lang="es">
    <head>
        <meta charset="UTF-8">
        <title>NovaHarvest - Estado del servidor</title>
        <link rel="stylesheet" href="/css/site.css">
    </head>
    <body>
        <header>
            <img src="/img/logo.png" alt="Logotipo de NovaHarvest" width="100">
            <p>NovaHarvest</p>
            <nav>
                <a href="/">Inicio</a>
                <a href="/about">Acerca de</a>
                <a href="/status">Estado</a>
            </nav>
        </header>
        <main>
            <h1>Estado del servidor</h1>
            <p>Fecha y hora del servidor: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
            <p>Página generada por ASP.NET Core (Kestrel)</p>
        </main>
        <footer>NovaHarvest · IES Valle del Jerte · {DateTime.Now.Year}</footer>
    </body>
    </html>
    """,
    "text/html; charset=utf-8"));
```

Generar HTML de esta forma tiene algunos inconvenientes. El código puede resultar difícil de leer y mantener porque se mezcla C# con HTML. Además, es fácil cometer errores al escribir las etiquetas o las comillas, y puede costar encontrar dónde está el problema.

En las tres páginas se repiten partes como la estructura HTML, el encabezado con el logotipo y el menú de navegación, el enlace al archivo CSS y el pie de página.

### 8. Docker

En NovaHarvest tengo dos imágenes y dos contenedores en funcionamiento: uno para nginx y otro para el backend ASP.NET Core.
Una imagen es una plantilla que contiene lo necesario para crear un contenedor, mientras que un contenedor es una instancia de esa imagen que se ejecuta.
En el Dockerfile del backend utilizo una primera etapa con el SDK de .NET 10 para compilar y publicar la aplicación.
Después, en la segunda etapa, utilizo la imagen de ASP.NET Core 10, que contiene lo necesario para ejecutar la aplicación sin incluir el SDK.
De esta forma, la imagen final ocupa menos espacio y no incluye herramientas que no hacen falta para ejecutar el proyecto.
El archivo `.dockerignore` evita que las carpetas `bin` y `obj` se copien al contexto de construcción de Docker.
Si no lo utilizara, se podrían incluir archivos de compilaciones anteriores, aumentando el contenido que se envía a Docker y pudiendo causar problemas durante la publicación.

### 9. Herramientas

| Herramienta | Uso en NovaHarvest y problema que resuelve |
|---|---|
| Visual Studio Code | Lo he utilizado para crear y editar los archivos del proyecto. |
| PowerShell | Me ha permitido ejecutar comandos para trabajar con Docker y .NET. |
| Git y GitHub | Los he utilizado para guardar los cambios del proyecto mediante commits y mantener una copia del repositorio. |
| Docker | Permite crear y ejecutar los contenedores necesarios para la aplicación. |
| Docker Compose | Permite configurar y poner en marcha los servicios web y backend conjuntamente. |
| nginx | Actúa como proxy inverso y reenvía las peticiones del navegador al backend. |
| ASP.NET Core y Kestrel | Permiten ejecutar el backend, gestionar las rutas y generar las páginas HTML. |
| curl.exe | Lo he utilizado para comprobar las respuestas HTTP y las cabeceras del servidor. |

### 10. Observación

Al ejecutar el proyecto directamente con `dotnet run`, la página `/status` muestra la fecha y hora `30/10/2026 12:14:37`.
En cambio, al ejecutarlo mediante Docker, muestra `30/10/2026 11:15:18`.
Por tanto, las horas no coinciden y existe aproximadamente una hora de diferencia.
Esto se debe a que `DateTime.Now` utiliza la zona horaria configurada en cada entorno.
En mi ordenador se utiliza la hora local de España, mientras que el contenedor parece utilizar UTC.
Por eso, aunque la aplicación sea la misma, muestra una hora diferente dependiendo de dónde se esté ejecutando.