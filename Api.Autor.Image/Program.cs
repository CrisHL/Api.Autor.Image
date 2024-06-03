using Api.Autor.Image.Services;
using Api.Autor.Image.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.MaxReceiveMessageSize = null; // Sin l�mite en el tama�o de recepci�n
    options.MaxSendMessageSize = null;    // Sin l�mite en el tama�o de env�o
});

builder.Services.AddOptions<MongoDbSettings>()
    .Bind(builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<ImageUploaderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ImageUploaderService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
