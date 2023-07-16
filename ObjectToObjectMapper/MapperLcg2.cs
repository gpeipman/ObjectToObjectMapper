using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ObjectToObjectMapper
{
    public class MapperLcg2<T> : ObjectCopyBase
    {
        private delegate void CopyDelegate(T source, T target);
        private CopyDelegate _copy;

        public override void MapTypes(Type source, Type target)
        {
            var key = GetMapKey(source, target);

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
            this._copy = (CopyDelegate)dm.CreateDelegate(typeof(CopyDelegate));
        }

        public override void Copy(object source, object target)
        {
            _copy((T)source, (T)target);
        }
    }
}
