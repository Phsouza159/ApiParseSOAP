var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();

app.MapPost("/NumberToWords", () =>
{
    var forecast =
        new NumberToWordsResponse()
        {
            NumberToWordsResult = "ten",
        };

    return forecast;
});

app.Run();

internal record NumberToWordsResponse()
{
    public string NumberToWordsResult { get; set; }
}
