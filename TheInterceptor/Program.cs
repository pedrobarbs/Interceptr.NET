using TheInterceptor;
using TheInterceptor.Interfaces;
using TheInterceptor.Sample.Layer2;
using TheInterceptor.Sample.Layer3;
using TheInterceptor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScopedIntercepted<ISampleService, SampleService>(
    new ChronometerInterceptor(),
    new StartingFinishingInterceptor());

builder.Services.AddScopedIntercepted<IServiceLayer2, ServiceLayer2>(
    new ChronometerInterceptor(),
    new StartingFinishingInterceptor());

builder.Services.AddScopedIntercepted<IServiceLayer3, ServiceLayer3>(
    new ChronometerInterceptor(),
    new StartingFinishingInterceptor());

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
