using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Thatnetwork.Entities;
using Thatnetwork.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Thatnetwork.Middlewares;
using Thatnetwork;
using Thatnetwork.Notes;
using Thatnetwork.Users;
using System.IdentityModel.Tokens.Jwt;
using Thatnetwork.Clubs;
using System.Reflection;
using Thatnetwork.Photos;
using Thatnetwork.Challenges;
using Thatnetwork.Chats;
using AutoMapper;
using Thatnetwork.AppHub;
using Thatnetwork.Extensions;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data source=mydb.db"));
builder.Services.AddScoped<ValidateModelAttribute>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            //builder.Configuration.GetSection("Token").Value!))
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ycyo89m3d6f94zyks763fp5qmgry4qgl")),
    };
    options.MapInboundClaims = false;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

// Automapper
builder.Services.AddAutoMapper(typeof(AppMappingProfile));



//builder.Services.Configure<IdentityOptions>(options =>
//     options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddSignalR();

// Мои сервисы
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClubService>();
builder.Services.AddScoped<PhotoService>();
builder.Services.AddScoped<ChallengeService>();
builder.Services.AddScoped<ChatService>();

builder.Services.AddHttpContextAccessor();

//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var app = builder.Build();
app.UseCors("AllowAll");

app.UseAuthentication();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<PopulateUserMiddleware>();
app.UseWebSockets();
app.UseStaticFiles();
app.UseFileServer();

app.MapControllers();

app.MapHub<UserHub>("/user-hub");
app.MapHub<ChatHub>("/chat-hub");
app.MapHub<AppHub>("/app-hub");

Timer timer = new Timer(_ =>
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dateNow = DateTime.Now.TrimSeconds();
        var marathons = dbContext.Marathons
            .Where(m => m.StartDate == dateNow)
            .ToList();
        var chatService = scope.ServiceProvider.GetRequiredService<ChatService>();
        foreach (var marathon in marathons)
        {
            int chatRoomId = marathon.ChatRoomId;
            chatService.AddMessageAsync(
                new Thatnetwork.Chats.Dtos.AddMessageDto { ChatRoomId = chatRoomId, Text = "Марафон начался. Удачи!" },
                null);
        }
    }
}, null, 0, 60000);

Timer timer2 = new Timer(_ =>
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dateNow = DateTime.Now.TrimSeconds();
        var marathons = dbContext.Marathons
            .Where(m => m.EndDate == dateNow)
            .ToList();
        var chatService = scope.ServiceProvider.GetRequiredService<ChatService>();
        foreach (var marathon in marathons)
        {
            int chatRoomId = marathon.ChatRoomId;
            chatService.AddMessageAsync(
                new Thatnetwork.Chats.Dtos.AddMessageDto { ChatRoomId = chatRoomId, Text = "Марафон закончился. Лишь сильнейшие дошли до конца!" },
                null);
        }
    }
}, null, 0, 60000);
app.Run();
