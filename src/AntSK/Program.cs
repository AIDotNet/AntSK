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
using LLama.Native;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
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
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "Directly enter bearer {token} in the box below (note that there is a space between bearer and token)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, Array.Empty<string>()
                    }
                });
});
//Mapper
builder.Services.AddMapper();
//后台队列任务
builder.Services.AddBackgroundTaskBroker().AddHandler<ImportKMSTaskReq, BackGroundTaskHandler>("ImportKMSTask");
// 读取连接字符串配置
{
    builder.Configuration.GetSection("DBConnection").Get<DBConnectionOption>();
    builder.Configuration.GetSection("Login").Get<LoginOption>();
    builder.Configuration.GetSection("LLamaSharp").Get<LLamaSharpOption>();
    builder.Configuration.GetSection("KernelMemory").Get<KernelMemoryOption>();
    if (LLamaSharpOption.RunType.ToUpper() == "CPU")
    {
        NativeLibraryConfig
           .Instance
           .WithCuda(false)
           .WithLogs(true);
    }
    else if (LLamaSharpOption.RunType.ToUpper() == "GPU")
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
LoadFun(app);

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
        _repository.GetDB().CodeFirst.InitTables(typeof(Funs));
        //创建vector插件如果数据库没有则需要提供支持向量的数据库
        _repository.GetDB().Ado.ExecuteCommandAsync($"CREATE EXTENSION IF NOT EXISTS vector;");
    }
}

void LoadFun(WebApplication app)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            //codefirst 创建表
            var funRep = scope.ServiceProvider.GetRequiredService<IFuns_Repositories>();
            var functionService = scope.ServiceProvider.GetRequiredService<FunctionService>();
            var funs= funRep.GetList();
            foreach (var fun in funs)
            {
                functionService.FuncLoad(fun.Path);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message + " ---- " + ex.StackTrace);
    }
}