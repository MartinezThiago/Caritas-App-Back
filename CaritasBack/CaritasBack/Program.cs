using CaritasBack.Configuraciones;
using CaritasBack.Models;
using CaritasBack.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Agrega configuración desde appsettings.json
builder.Configuration.AddJsonFile("appsettings.json");
// Configuración de la inyección de dependencias para acceder a la configuración
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    //titulo
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "CARITAS BACK", Version = "v3" });

    //boton de autorize
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Coloque token para validar identidad",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[] {}
        }
    });
});

//el cors sirve para poder hacerle peticiones a la api desde disintos dominios y no solo del dominio que sirvió el primer recurso.
builder.Services.AddCors(p => p.AddPolicy("corsAplicacion", builder =>
{
    builder.WithOrigins("*")//reemplazar el asterisco por el dominio, o el localHost que quieras. Si son  varios van separados por coma.
    .AllowAnyMethod()
    .AllowAnyHeader();

}));


//se agrega al contenedor de dependencias las Interfaces y las clases q las implementan.
builder.Services.AddScoped<IAutenticarUsuario, AutenticarUsuario>();
builder.Services.AddScoped<IGenerateToken, GenerateToken>();
builder.Services.AddScoped<IBuilder, Builder>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IConfigurationBuilder, ConfigurationBuilder>();
builder.Services.AddScoped<IValidarToken, Token>();

//builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));

//middleware , token expiro? , es valido? ,etc
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UsuarioAdmin", policy =>
        policy.RequireClaim("Rol", "admin_centro"));

    options.AddPolicy("voluntario", policy =>
        policy.RequireClaim("Rol", "voluntario"));

    options.AddPolicy("usuarioBasico", policy =>
        policy.RequireClaim("Rol", "usuario_basico")); 

    options.AddPolicy("TodosLosRolesCombinados", policy =>
        {
            policy.RequireClaim("Rol", "admin_centro", "voluntario", "usuario_basico");
        });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
// Clase de configuración

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("corsAplicacion");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
