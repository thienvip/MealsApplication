using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using MealApplication.Extensions;
using MealApplication.Filters;
using MealApplication.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NuGet.Protocol;
using src.Core;
using src.Data;
using src.Emails;
using src.Localization.Resources;
using src.NovellDirectoryLdap;
using src.Repositories;
using src.Repositories.Authentication;
using src.Repositories.Category;
using src.Repositories.EmailConfigs;
using src.Repositories.Meals;
using src.Repositories.Messages;
using src.Repositories.Parents;
using src.Repositories.PowerOfAttorneys;
using src.Repositories.Roles;
using src.Repositories.Settings;
using src.Repositories.Students;
using src.Repositories.Users;
using src.Web.Common;
using src.Web.Common.Security;
using src.Web.Common.Validation;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOptions();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<CCEmailSettings>(builder.Configuration.GetSection("CCEmailSettings"));


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer(options => options.WorkerCount = 1);

GlobalConfiguration.Configuration.UseSerializerSettings(new JsonSerializerSettings
{
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
});

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
//})
//.AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.PropertyNamingPolicy = null;
//});

builder.Services.AddAuthentication( options =>
    {
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }
    ).AddCookie(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.AccessDeniedPath = "/Common/AccessDenied";
                        options.LoginPath = "/Account/Login";
                    }
                );


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Constants.RoleNames.Administrator, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser()
            .RequireAssertion(context => context.User.HasClaim(ClaimTypes.Role, Constants.RoleNames.Administrator) || context.User.HasClaim(ClaimTypes.Role, Constants.RoleNames.User))
            .Build();
    });
    options.AddPolicy(Constants.RoleNames.User, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser()
            .RequireAssertion(context => context.User.HasClaim(ClaimTypes.Role, Constants.RoleNames.User))
            .Build();
    });
});

builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
 
}
);
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});
builder.Services.AddSignalR();
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<CheckVerification>();
builder.Services.AddScoped<CheckAccess>();
builder.Services.AddScoped<SessionTimeout>();


builder.Services.AddScoped<IParentRepository, ParentRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IEmailConfigRepository, EmailConfigRepository>();
builder.Services.AddScoped<IDbContext, AppDbContext>();
builder.Services.AddScoped<INotificationExtension, NotificationExtension>();
builder.Services.AddScoped <IPowerOfAttorneyRepository, PowerOfAttorneyRepository>();
builder.Services.AddScoped<ISignInManager, SignInManager>();
builder.Services.AddScoped<IAuthenticationService, LdapAuthenticationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBaseCategoryRepository, BaseCategoryRepository>();
builder.Services.AddScoped<IUserSession, UserSession>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IMealRepository, MealRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
builder.Services.AddScoped<ISettingRepository, SettingRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddTransient<IDateTime, DateTimeAdapter>();

builder.Services.AddMvc().AddViewLocalization()
           .AddDataAnnotationsLocalization(options =>
           {
               options.DataAnnotationLocalizerProvider = (type, factory) =>
                   factory.Create(typeof(SharedResource));
           });


builder.Services.AddMvc()
     .AddViewOptions(options =>
     {
         options.HtmlHelperOptions.ClientValidationEnabled = true;
     });

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
                    new CultureInfo("vi-VN"),
                    new CultureInfo("en-GB")

                };
    options.DefaultRequestCulture = new RequestCulture("vi-VN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<MealValidator>();

//service

builder.Services.AddHostedService<BackgroundJobService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()// You can configure this to allow specific origins.
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional:false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
// ...
// Hangfire
app.UseHangfireDashboard();

// Configure Hangfire jobs, queues, etc. as needed

// Global Hangfire filters
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Serve static files
app.UseStaticFiles();

// Enable CORS (Cross-Origin Resource Sharing)
app.UseCors("MyCorsPolicy");

// Configure the request pipeline for  sessions and routing
app.UseRouting();
app.UseSession();

// Enable authorization
app.UseAuthentication();
app.UseAuthorization();

// Configure request localization
app.UseRequestLocalization();

// Define your default route for controllers

app.MapControllerRoute(
     name: "areaRoute",
     pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
     name: "default",
     pattern: "{controller=Home}/{action=Index}/{id?}"
);

//app.UseStatusCodePagesWithRedirects("/Common/PageNotFound/{0}");
//app.UseExceptionHandler("/Common/Error");

// Cookie Policy
app.UseCookiePolicy();


// Start the application
app.Run();


