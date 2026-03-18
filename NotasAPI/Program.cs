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

var notas = app.MapGroup("/notas");

notas.MapGet("/", GetAllNotas);
notas.MapGet("/{id}", GetNota);
notas.MapPost("/", CreateNota);
notas.MapPut("/{id}", UpdateNota);
notas.MapPatch("/{id}", PatchNota);
notas.MapDelete("/{id}", DeleteNota);

app.Run();

static async Task<IResult> GetAllNotas(NotaDb db)
{
    return TypedResults.Ok(await db.Notas.ToArrayAsync());
}

static async Task<IResult> GetNota(int id, NotaDb db)
{
    return await db.Notas.FindAsync(id)
        is Nota nota
            ? TypedResults.Ok(nota)
            : TypedResults.NotFound();
}

static async Task<IResult> CreateNota(Nota nota, NotaDb db)
{
    db.Notas.Add(nota);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/notas/{nota.Id}", nota);
}

static async Task<IResult> UpdateNota(int id, Nota inputNota, NotaDb db)
{
    var nota = await db.Notas.FindAsync(id);

    if (nota is null) return TypedResults.NotFound();

    nota.Titulo = inputNota.Titulo;
    nota.Contenido = inputNota.Contenido;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> PatchNota(int id, NotaPatchDto inputNota, NotaDb db)
{
    var nota = await db.Notas.FindAsync(id);

    if (nota is null) return TypedResults.NotFound();

    if (inputNota.Titulo != null)
        nota.Titulo = inputNota.Titulo;

    if (inputNota.Contenido != null)
        nota.Contenido = inputNota.Contenido;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteNota(int id, NotaDb db)
{
    if (await db.Notas.FindAsync(id) is Nota nota)
    {
        db.Notas.Remove(nota);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}