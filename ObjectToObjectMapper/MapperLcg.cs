using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ObjectToObjectMapper
{
    public class MapperLcg : ObjectCopyBase
    {
        private readonly Dictionary<string, DynamicMethod> _del = new Dictionary<string, DynamicMethod>();

        public override void MapTypes(Type source, Type target)
        {
            var key = GetMapKey(source, target);
            if (_del.ContainsKey(key))
            {
                return;
            }

            var args = new[] { source, target };
            var mod = typeof(Program).Module;

            var dm = new DynamicMethod(key, null, args, mod);
            var il = dm.GetILGenerator();
            var maps = GetMatchingProperties(source, target);

            foreach (var map in maps)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Callvirt, map.SourceProperty.GetGetMethod(), null);
                il.EmitCall(OpCodes.Callvirt, map.TargetProperty.GetSetMethod(), null);
            }
            il.Emit(OpCodes.Ret);
            _del.Add(key, dm);
        }

        public override void Copy(object source, object target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();
            var key = GetMapKey(sourceType, targetType);

            var del = _del[key];
            var args = new[] { source, target };
            del.Invoke(null, args);
        }
    }
}
