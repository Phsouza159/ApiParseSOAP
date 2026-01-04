using Microsoft.AspNetCore.Mvc;
using WebApiTeste.Autenticacao;
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

app.MapPost("/GEWSV0006_Banco", (HttpRequest request, [FromBody] ListarBancosData data) =>
{
    if (!Autenticacao.AuteticarBasic(request.Headers["Authorization"]))
        return Results.Unauthorized();

    string json = WebApiTeste.Registros.ListarBancosResponse.RecuperarLista();
    return Results.Content(json, "application/json");
});

app.MapPost("/SXWSV0030_ContratoSistema", ([FromBody] ListarContratosLinhaProdudo data) =>
{
    string json = WebApiTeste.Registros.listarContratosLinhaProdudoResponse.RecuperarLista();
    return Results.Content(json, "application/json");
});

app.Run();

