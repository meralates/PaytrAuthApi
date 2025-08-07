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


builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpClient<AuthService>();


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