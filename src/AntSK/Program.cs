using AntDesign.ProLayout;
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Common.Map;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Other;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Options;
using AntSK.Domain.Repositories;
using AntSK.Domain.Utils;
using AntSK.plugins.Functions;
using AntSK.Services.Auth;
using Blazored.LocalStorage;
using LLama.Native;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(config =>
{
    //此设定解决JsonResult中文被编码的问题
    config.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

    config.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    config.JsonSerializerOptions.Converters.Add(new DateTimeNullableConvert());
});

builder.AddServiceDefaults();

builder.Configuration.GetSection("DBConnection").Get<DBConnectionOption>();
builder.Configuration.GetSection("Login").Get<LoginOption>();
builder.Configuration.GetSection("KernelMemory").Get<KernelMemoryOption>();
builder.Configuration.GetSection("FileDir").Get<FileDirOption>();

builder.Services.Configure<OtlpExporterOptions>(
    o => o.Headers = $"x-otlp-api-key=antsk");

Log.Logger = new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration)
.CreateLogger();

var loggerFactory = LoggerFactory.Create(builder => {
    builder.ClearProviders();
    builder.AddSerilog();
});
ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
InitExtensions.InitLog(logger);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAntDesign();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AntSKAuthProvider>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(sp.GetService<NavigationManager>()!.BaseUri)
});
builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
builder.Services.AddServicesFromAssemblies("AntSK");
builder.Services.AddServicesFromAssemblies("AntSK.Domain");
builder.Services.AddSingleton(sp => new FunctionService(sp, [typeof(AntSK.App).Assembly]));
builder.Services.AddScoped<FunctionTest>();
builder.Services.AddAntSKSwagger();
builder.Services.AddBlazoredLocalStorage(config =>
        config.JsonSerializerOptions.WriteIndented = true);
//Mapper
builder.Services.AddMapper();
//后台队列任务
builder.Services.AddBackgroundTaskBroker().AddHandler<ImportKMSTaskReq, BackGroundTaskHandler>("ImportKMSTask");

//增加API允许跨域调用
builder.Services.AddCors(options => options.AddPolicy("Any",
    builder =>
    {
        builder.AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    }));    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseCors("Any");

app.UseStaticFiles();

//扩展初始化实现
app.CodeFirst();
app.LoadFun();
app.InitDbData();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.UseSwagger();
//配置Swagger UI
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AntSK API"); //注意中间段v1要和上面SwaggerDoc定义的名字保持一致
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();

