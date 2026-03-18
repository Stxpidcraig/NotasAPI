using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotaDb>(opt => opt.UseInMemoryDatabase("NotasDB"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "NotasAPI";
    config.Title = "NotasAPI v1";
    config.Version = "v1";
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "NotasAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}
app.MapGet("/notas", async (NotaDb db) =>
    await db.Notas.ToListAsync());

app.MapGet("/notas/{id}", async (int id, NotaDb db) =>
    await db.Notas.FindAsync(id)
        is Nota nota
            ? Results.Ok(nota)
            : Results.NotFound());

app.MapPost("/notas", async (Nota nota, NotaDb db) =>
{
    db.Notas.Add(nota);
    await db.SaveChangesAsync();
    return Results.Created($"/notas/{nota.Id}", nota);
});
app.MapPut("/notas/{id}", async (int id, Nota inputNota, NotaDb db) =>
{
    var nota = await db.Notas.FindAsync(id);

    if (nota is null) return Results.NotFound();

    nota.Titulo = inputNota.Titulo;
    nota.Contenido = inputNota.Contenido;
    nota.EsFavorita = inputNota.EsFavorita;

    await db.SaveChangesAsync();

    return Results.NoContent();
});
app.MapDelete("/notas/{id}", async (int id, NotaDb db) =>
{
    if (await db.Notas.FindAsync(id) is Nota nota)
    {
        db.Notas.Remove(nota);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});
app.Run();