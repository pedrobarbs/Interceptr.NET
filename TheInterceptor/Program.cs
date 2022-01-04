using TheInterceptor;
using TheInterceptor.Interfaces;
using TheInterceptor.Services;
using TheInterceptor.SourceGenerator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScopedIntercepted<ISampleService, SampleService>(
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
