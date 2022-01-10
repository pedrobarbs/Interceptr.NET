using Interceptr;
using Interceptr.Interfaces;
using Interceptr.Sample.IOC;
using Interceptr.Sample.Layer2;
using Interceptr.Sample.Layer3;
using Interceptr.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var stack = new InterceptrStack(new ChronometerInterceptor(), new StartingFinishingInterceptor());

builder.Services.AddScopedIntercepted<ISampleService, SampleService>(stack);

builder.Services.AddOtherLayers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
