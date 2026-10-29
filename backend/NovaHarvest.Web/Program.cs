var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapStaticAssets();

app.MapGet("/", () => Results.Content(
    $"""
    <!DOCTYPE html>
    <html lang="es">
    <head>
        <meta charset="UTF-8">
        <title>NovaHarvest - Inicio</title>
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
            <h1>Inicio</h1>
            <p>NovaHarvest es una iniciativa para aprovechar mejor los excedentes alimentarios del Valle del Jerte.</p>
            <p>Queremos facilitar que esos alimentos puedan tener una segunda oportunidad en lugar de desperdiciarse.</p>
            <p>El proyecto busca conectar a quienes disponen de excedentes con quienes pueden aprovecharlos.</p>
            <p>Así contribuimos a un aprovechamiento más responsable de los alimentos.</p>
        </main>
        <footer>NovaHarvest · IES Valle del Jerte · {DateTime.Now.Year}</footer>
    </body>
    </html>
    """,
    "text/html; charset=utf-8"));

app.Run();