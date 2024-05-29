using System.ComponentModel;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml;
using AntSK.Domain.Common;
using AntSK.Domain.Utils;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel;
using HtmlAgilityPack;
using System.Collections.Generic;
using Serilog;

namespace AntSK.Domain.Domain.Service
{
    public class FunctionService
    {
        private readonly Dictionary<string, MethodInfo> _methodCache;
        private readonly Dictionary<string, (string Description, (Type ParameterType, string Description) ReturnType, (string ParameterName, Type ParameterType, string Description)[] Parameters)> _methodInfos;

        private readonly IServiceProvider _serviceProvider;
        private Assembly[] _assemblies;
        private readonly AssemblyLoadContext loadContext;

        public FunctionService(IServiceProvider serviceProvider, Assembly[] assemblies)
        {
            _methodCache = [];
            _methodInfos = [];
            _serviceProvider = serviceProvider;
            _assemblies = assemblies;
            loadContext = new AssemblyLoadContext("AntSKLoadContext", true);
        }

        public Dictionary<string, MethodInfo> Functions => _methodCache;
        public Dictionary<string, (string Description, (Type ParameterType, string Description) ReturnType, (string ParameterName, Type ParameterType, string Description)[] Parameters)> MethodInfos => _methodInfos;

        /// <summary>
        /// 查询程序集中的方法委托，后续利用Source Generators生成
        /// </summary>
        public void SearchMarkedMethods()
        {
            var markedMethods = new List<MethodInfo>();

            _methodCache.Clear();
            _methodInfos.Clear();

            foreach (var assembly in _assemblies)
            {
                // 从缓存中获取标记了ActionAttribute的方法
                foreach (var type in assembly.GetTypes())
                {
                    markedMethods.AddRange(type.GetMethods().Where(m =>
                    {
                        DescriptionAttribute da = (DescriptionAttribute)m.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                        return da != null && da.Description.Contains( "AntSK");
                    }));
                }
            }

            //动态加载部分
            var loadedAssemblies = loadContext.Assemblies.ToList();
            foreach (var assembly in loadedAssemblies)
            {
                // 从缓存中获取标记了ActionAttribute的方法
                foreach (var type in assembly.GetTypes())
                {
                    markedMethods.AddRange(type.GetMethods().Where(m =>
                    {
                        DescriptionAttribute da = (DescriptionAttribute)m.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                        return da != null && da.Description.Contains("AntSK");
                    }));
                }
            }

            // 构建方法调用
            foreach (var method in markedMethods)
            {
                var key = $"{method.DeclaringType.Assembly.GetName().Name}_{method.DeclaringType.Name}_{method.Name}";
                string pattern = "[^a-zA-Z0-9_]";
                // 使用 '-' 替换非ASCII的正则表达式的字符
                key = Regex.Replace(key, pattern, "_");
                _methodCache.TryAdd(key, method);

                var description= method.GetCustomAttribute<DescriptionAttribute>().Description.ConvertToString().Replace("AntSK:","");
                var returnType = method.ReturnParameter.GetCustomAttribute<DescriptionAttribute>().Description.ConvertToString();
                var parameters = method.GetParameters().Select(x => (x.Name, x.ParameterType,x.GetCustomAttribute<DescriptionAttribute>()?.Description)).ToArray();
                // 假设 _methodInfos 是一个已经定义好的字典，用来保存方法的相关信息
                _methodInfos.TryAdd(key, (description, (method.ReflectedType, returnType), parameters));
            }
        }


        public void FuncLoad(string pluginPath)
        {
            try
            {
                if (File.Exists(pluginPath))
                {
                    string directory = Path.GetDirectoryName(pluginPath);
                    string fileName = Path.GetFileName(pluginPath);
                    var resolver = new AssemblyDependencyResolver(directory);

                    // Create a custom AssemblyLoadContext

                    loadContext.Resolving += (context, assemblyName) =>
                    {
                        string assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
                        if (assemblyPath != null)
                        {
                            return context.LoadFromAssemblyPath(assemblyPath);
                        }
                        return null;
                    };
                    // Load your assembly
                    Assembly pluginAssembly = loadContext.LoadFromAssemblyPath(pluginPath);
                }
            }
            catch (Exception ex)
            {
               Log.Error(ex.Message + " ---- " + ex.StackTrace);
            }
        }    
    }
}