using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace ObjectToObjectMapper
{
    public class MapperDynamicCode : ObjectCopyBase
    {
        private readonly Dictionary<string, Type> _comp =
        new Dictionary<string, Type>();

        public override void MapTypes(Type source, Type target)
        {
            var key = GetMapKey(source, target);
            if (_comp.ContainsKey(key))
            {
                return;
            }

            var builder = new StringBuilder();
            builder.Append("namespace Copy {\r\n");
            builder.Append("    public class ");
            builder.Append(key);
            builder.Append(" {\r\n");
            builder.Append("        public static void CopyProps(");
            builder.Append(source.FullName);
            builder.Append(" source, ");
            builder.Append(target.FullName);
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

            // Write out method body
            //Debug.WriteLine(builder.ToString());

            var myCodeProvider = new CSharpCodeProvider();
            var myCodeCompiler = myCodeProvider.CreateCompiler();
            var myCompilerParameters = new CompilerParameters();
            myCompilerParameters.ReferencedAssemblies.Add(this.GetType().Assembly.Location);
            myCompilerParameters.GenerateInMemory = true;
            
            var results = myCodeCompiler.CompileAssemblyFromSource(myCompilerParameters, builder.ToString());

            // Compiler output
            //foreach (var line in results.Output)
            //{
            //    Debug.WriteLine(line);
            //}

            var copierType = results.CompiledAssembly.GetType("Copy." + key);
            _comp.Add(key, copierType);
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
