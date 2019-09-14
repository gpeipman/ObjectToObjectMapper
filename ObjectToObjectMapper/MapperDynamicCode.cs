using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ObjectToObjectMapper
{
    public class MapperDynamicCode : ObjectCopyBase
    {
        private readonly Dictionary<string, Type> _comp = new Dictionary<string, Type>();

        public override void MapTypes(Type source, Type target)
        {
            var key = GetMapKey(source, target);
            if (_comp.ContainsKey(key))
            {
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("using ObjectToObjectMapper;\r\n");
            builder.Append("namespace Copy {\r\n");
            builder.Append("    public class ");
            builder.Append(key);
            builder.Append(" {\r\n");
            builder.Append("        public static void CopyProps(");
            builder.Append(target.FullName.Replace("+", "."));
            builder.Append(" source, ");
            builder.Append(target.FullName.Replace("+", "."));
            builder.Append(" target) {\r\n");

            var map = GetMatchingProperties(source, target);
            foreach (var item in map)
            {
                builder.Append("            target.");
                builder.Append(item.TargetProperty.Name);
                builder.Append(" = ");
                builder.Append("source.");
                builder.Append(item.SourceProperty.Name);
                builder.Append(";\r\n");
            }

            builder.Append("        }\r\n   }\r\n}");

            var syntaxTree = CSharpSyntaxTree.ParseText(builder.ToString());

            string assemblyName = Path.GetRandomFileName();
            var refPaths = new[] {
                typeof(Object).GetTypeInfo().Assembly.Location,
                typeof(Console).GetTypeInfo().Assembly.Location,
                Path.Combine(Path.GetDirectoryName(typeof(GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll"),
                GetType().GetTypeInfo().Assembly.Location
            };

            var references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using(var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                ms.Seek(0, SeekOrigin.Begin);

                var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                var type = assembly.GetType("Copy." + key);
                _comp.Add(key, type);
            }
        }

        public override void Copy(object source, object target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();

            var key = GetMapKey(sourceType, targetType);
            if (!_comp.ContainsKey(key))
            {
                MapTypes(sourceType, targetType);
            }

            var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod;
            var args = new[] { source, target };
            _comp[key].InvokeMember("CopyProps", flags, null, null, args);
        }
    }
}