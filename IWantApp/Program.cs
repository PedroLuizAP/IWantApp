using IWantApp.Domain.Users;
using IWantApp.Endpoints.Categories;
using IWantApp.Endpoints.Client;
using IWantApp.Endpoints.Employees;
using IWantApp.Endpoints.Orders;
using IWantApp.Endpoints.Products;
using IWantApp.Endpoints.Security;
using IWantApp.Infra.Data;
using IWantApp.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseSerilog((context, configuration) =>
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.WriteTo.Console().WriteTo.MSSqlServer(
        context.Configuration["ConnectionString:IWantDb"],
        sinkOptions: new MSSqlServerSinkOptions()
        {
            AutoCreateSqlDatabase = true,
            TableName = "LogAPI",
        });
});

builder.Services.AddSqlServer<DataContext>(builder.Configuration["ConnectionString:IWantDb"]);

#if DEBUG
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;

}).AddEntityFrameworkStores<DataContext>();
#else
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<DataContext>();
#endif

builder.Services.AddAuthorization(options =>
{
#if !DEBUG
    options.FallbackPolicy = new AuthorizationPolicyBuilder().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
#endif
    options.AddPolicy("EmployeePolicy", p => p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));
    options.AddPolicy("DocumentPolicy", p => p.RequireAuthenticatedUser().RequireClaim("Document"));
});


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]!))
    };
});

builder.Services.AddScoped<QueryAllUsersWithClaimName>();
builder.Services.AddScoped<UserCreator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);
app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);

app.MapMethods(OrderPost.Template, OrderPost.Methods, OrderPost.Handle);
app.MapMethods(OrderGet.Template, OrderGet.Methods, OrderGet.Handle);

app.MapMethods(ClientPost.Template, ClientPost.Methods, ClientPost.Handle);
app.MapMethods(ClientGet.Template, ClientGet.Methods, ClientGet.Handle);

app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);

app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);

app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);
app.MapMethods(ProductGetShowcase.Template, ProductGetShowcase.Methods, ProductGetShowcase.Handle);

app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext http) =>
{
    var error = http.Features.Get<IExceptionHandlerFeature>()?.Error;

    if (error != null)
    {        
        switch (error)
        {
            
            case SqlException:
                return Results.Problem(title: Messages.DatabaseOut, statusCode: 500);

            case BadHttpRequestException:
                return Results.Problem(title: Messages.InvalidFormat, statusCode: 500);
        }
    }

    return Results.Problem(title: Messages.UnexpectedError, statusCode: 500);
});

app.Run();
