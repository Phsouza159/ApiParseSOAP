using Microsoft.AspNetCore.Mvc;
using WebApiTeste.Registros;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();

app.MapPost("/NumberToWords", ([FromBody] NumberToWordsData NumberToWordsData) =>
{
    var response = new NumberToWordsResponse(NumberToWordsData.ubiNum);
    return response;
});

app.MapPost("/NumberToDollars", ([FromBody] NumberToDollarsData NumberToWordsData) =>
{
    var response = new NumberToDollarsResponse(NumberToWordsData.dNum);
    return response;
});

app.MapPost("/GEWSV0006_Banco", ([FromBody] ListarBancosData data) =>
{
    string json = WebApiTeste.Registros.ListarBancosResponse.RecuperarLista();
    return Results.Content(json, "application/json");
});

app.Run();

