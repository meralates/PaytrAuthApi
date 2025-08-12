using FluentValidation.AspNetCore;
using PaytrAuthApi.Models;
using PaytrAuthApi.Services;
using PaytrAuthApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Tip güvenli ayarlar: appsettings.json içindeki "Paytr" bölümü
builder.Services.Configure<PaytrSettings>(builder.Configuration.GetSection("Paytr"));

// Controller ve Swagger servisleri
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Program.cs (.NET 6+ minimal API örneği)
builder.Services.Configure<NeoPosSettings>(builder.Configuration.GetSection("NeoPosSettings"));
builder.Services.AddHttpClient<PaymentService>();
builder.Services.AddTransient<PaymentService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpClient<AuthService>();

//validator
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PaymentRequestModelValidator>());


builder.Services.AddSingleton<TokenManager>();

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