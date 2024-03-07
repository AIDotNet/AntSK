using AntDesign.ProLayout;
using Microsoft.AspNetCore.Components;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using AntSK.Domain.Utils;
using AntSK.Services;
using AntSK.Domain.Common.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using AntSK.Domain.Options;
using Microsoft.KernelMemory.ContentStorage.DevTools;
using Microsoft.KernelMemory.FileSystem.DevTools;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.KernelMemory.Postgres;
using AntSK.Domain.Repositories;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.AspNetCore.Components.Authorization;
using AntSK.Services.Auth;
using LLama.Native;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using AntSK.Domain.Model;
using AntSK.Domain.Domain.Service;
using AntSK.Domain.Common.Map;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(config =>
{
    //此设定解决JsonResult中文被编码的问题
    config.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

    config.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    config.JsonSerializerOptions.Converters.Add(new DateTimeNullableConvert());
});
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAntDesign();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AntSKAuthProvider>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(sp.GetService<NavigationManager>()!.BaseUri)
});
builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
builder.Services.AddServicesFromAssemblies("AntSK");
builder.Services.AddServicesFromAssemblies("AntSK.Domain");

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AntSK.Api", Version = "v1" });
    //添加Api层注释（true表示显示控制器注释）
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);
    //添加Domain层注释（true表示显示控制器注释）
    var xmlFile1 = $"{Assembly.GetExecutingAssembly().GetName().Name.Replace("Api", "Domain")}.xml";
    var xmlPath1 = Path.Combine(AppContext.BaseDirectory, xmlFile1);
    c.IncludeXmlComments(xmlPath1, true);
    c.DocInclusionPredicate((docName, apiDes) =>
    {
        if (!apiDes.TryGetMethodInfo(out MethodInfo method))
            return false;
        var version = method.DeclaringType.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
        if (docName == "v1" && !version.Any())
            return true;
        var actionVersion = method.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
        if (actionVersion.Any())
            return actionVersion.Any(v => v == docName);
        return version.Any(v => v == docName);
    });
});
//Mapper
builder.Services.AddMapper();
//后台队列任务
builder.Services.AddBackgroundTaskBroker().AddHandler<ImportKMSTaskReq, BackGroundTaskHandler>("ImportKMSTask");
// 读取连接字符串配置
{
    builder.Configuration.GetSection("DBConnection").Get<DBConnectionOption>();
    builder.Configuration.GetSection("OpenAIOption").Get<OpenAIOption>();
    builder.Configuration.GetSection("Login").Get<LoginOption>();
    builder.Configuration.GetSection("LLamaSharp").Get<LLamaSharpOption>();
    if (LLamaSharpOption.RunType.ToUpper() == "CPU")
    {
        NativeLibraryConfig
           .Instance
           .WithCuda(false)
           .WithLogs(true);
    }
    else if(LLamaSharpOption.RunType.ToUpper() == "GPU")
    {
        NativeLibraryConfig
        .Instance
        .WithCuda(true)
        .WithLogs(true);
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

InitDB(app);

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
void InitDB(WebApplication app) 
{
    using (var scope = app.Services.CreateScope())
    {
        //codefirst 创建表
        var _repository = scope.ServiceProvider.GetRequiredService<IApps_Repositories>();
        _repository.GetDB().DbMaintenance.CreateDatabase();
        _repository.GetDB().CodeFirst.InitTables(typeof(Apps));
        _repository.GetDB().CodeFirst.InitTables(typeof(Kmss));
        _repository.GetDB().CodeFirst.InitTables(typeof(KmsDetails));
        _repository.GetDB().CodeFirst.InitTables(typeof(Users));
        _repository.GetDB().CodeFirst.InitTables(typeof(Apis));
        _repository.GetDB().CodeFirst.InitTables(typeof(AIModels));
        //创建vector插件如果数据库没有则需要提供支持向量的数据库
        _repository.GetDB().Ado.ExecuteCommandAsync($"CREATE EXTENSION IF NOT EXISTS vector;");
    }
}


