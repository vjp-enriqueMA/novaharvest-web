var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

var app = builder.Build();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();