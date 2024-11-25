using System.Text;
using graphql_api_test.Queries;
using graphql_api_test.Repository;
using graphql_api_test.Subscriptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // todo setup this correctly 
                ValidateIssuer = false,
                ValidateAudience = false,
                SignatureValidator = (token, parameters) => new JsonWebToken(token),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("local-secret-key"))
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<MongoRepo>();
builder.Services.AddSingleton<MongoWatcher>();

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddInMemorySubscriptions()
    .AddSubscriptionType<TransactionSubscriptionType>()
    .AddQueryType<TransactionQueries>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();

app.MapControllers();
app.MapGraphQL();

app.Run();