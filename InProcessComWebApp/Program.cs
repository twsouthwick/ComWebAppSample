using InProcessComWebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddComExample();

var app = builder.Build();

app.MapComExample();

app.Run();
