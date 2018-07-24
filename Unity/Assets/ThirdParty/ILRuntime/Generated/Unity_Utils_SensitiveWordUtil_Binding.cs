using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class Unity_Utils_SensitiveWordUtil_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Unity_Utils.SensitiveWordUtil);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("IsSensitiveWord", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsSensitiveWord_0);

            field = type.GetField("WordsDatas", flag);
            app.RegisterCLRFieldGetter(field, get_WordsDatas_0);
            app.RegisterCLRFieldSetter(field, set_WordsDatas_0);


        }


        static StackObject* IsSensitiveWord_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @str = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Unity_Utils.SensitiveWordUtil.IsSensitiveWord(@str);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_WordsDatas_0(ref object o)
        {
            return Unity_Utils.SensitiveWordUtil.WordsDatas;
        }
        static void set_WordsDatas_0(ref object o, object v)
        {
            Unity_Utils.SensitiveWordUtil.WordsDatas = (System.String[])v;
        }


    }
}
