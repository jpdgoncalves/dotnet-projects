using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string ROOT_FOLDER = Path.GetFullPath("root_folder");
string NOTES_FOLDER = Path.Join(ROOT_FOLDER, "notes");


app.MapGet("/notes", () => {
    var notes = Directory.EnumerateFiles(NOTES_FOLDER).Select(
        e => Path.GetFileNameWithoutExtension(e)
    ).ToList();
    return notes;
});

app.MapGet("/notes/{notename}", (string notename) => {
    var notepath = Path.Join(NOTES_FOLDER, notename + ".txt");
    if (!File.Exists(notepath)) return Results.NotFound($"Note '{notename}' doesn't exist");
    return Results.Text(File.ReadAllText(notepath));
});

app.MapPost("/notes/{notename}", (string notename) => {
    var notepath = Path.Join(NOTES_FOLDER, notename + ".txt");
    if (File.Exists(notepath)) return Results.BadRequest($"Note '{notename}' already exists");
    File.Create(notepath);
    return Results.Created($"/notes/{notename}", $"Created note '{notename}'");
});

app.MapPut("/notes/{notename}", async (string notename, [FromBody] Stream body) => {
    var reader = new StreamReader(body);
    var text = await reader.ReadToEndAsync();
    reader.Close();
    Console.WriteLine($"Client sent this text: '{text}'");
    var notepath = Path.Join(NOTES_FOLDER, notename + ".txt");
    if (!File.Exists(notepath)) return Results.NotFound($"Note '{notename}' doesn't exist");
    File.WriteAllText(notepath, text);
    return Results.Text($"Updated note '{notename}'");
});

app.MapDelete("/notes/{notename}", (string notename) => {
    var notepath = Path.Join(NOTES_FOLDER, notename + ".txt");
    if (!File.Exists(notepath)) return Results.NotFound($"Note '{notename}' doesn't exist");
    File.Delete(notepath);
    return Results.Text($"Deleted note '{notename}'");
});

app.Run();
