using Microsoft.AspNetCore.Mvc;

string ROOT_FOLDER = Path.GetFullPath("root_folder");
string NOTES_FOLDER = Path.Join(ROOT_FOLDER, "notes");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(CustomMiddleware.CheckPassword);

var notesGroup = app.MapGroup("/notes");
notesGroup.AddEndpointFilter(CustomMiddleware.ValidateNotename);

notesGroup.MapGet("", () =>
{
    var notes = Directory.EnumerateFiles(NOTES_FOLDER).Select(
        e => Path.GetFileNameWithoutExtension(e)
    ).ToList();
    return notes;
});

notesGroup.MapGet("/{notename}", ([FromRoute(Name = "notename")] string notename) =>
{
    var notepath = Path.Join(NOTES_FOLDER, notename + ".txt");
    if (!File.Exists(notepath)) return Results.NotFound($"Note '{notename}' doesn't exist");
    return Results.Text(File.ReadAllText(notepath));
});

notesGroup.MapPost("/{notename}", ([FromRoute(Name = "notename")] string notename) =>
{
    var notepath = Path.Join(NOTES_FOLDER, notename + ".txt");
    if (File.Exists(notepath)) return Results.BadRequest($"Note '{notename}' already exists");
    File.Create(notepath);
    return Results.Created($"/{notename}", $"Created note '{notename}'");
});

notesGroup.MapPut("/{notename}", async ([FromRoute(Name = "notename")] string notename, [FromBody] Stream body) =>
{
    var reader = new StreamReader(body);
    var text = await reader.ReadToEndAsync();
    reader.Close();
    Console.WriteLine($"Client sent this text: '{text}'");
    var notepath = Path.Join(NOTES_FOLDER, notename + ".txt");
    if (!File.Exists(notepath)) return Results.NotFound($"Note '{notename}' doesn't exist");
    File.WriteAllText(notepath, text);
    return Results.Text($"Updated note '{notename}'");
});

notesGroup.MapDelete("/{notename}", ([FromRoute(Name = "notename")] string notename) =>
{
    var notepath = Path.Join(NOTES_FOLDER, notename + ".txt");
    if (!File.Exists(notepath)) return Results.NotFound($"Note '{notename}' doesn't exist");
    File.Delete(notepath);
    return Results.Text($"Deleted note '{notename}'");
});

app.Run();
