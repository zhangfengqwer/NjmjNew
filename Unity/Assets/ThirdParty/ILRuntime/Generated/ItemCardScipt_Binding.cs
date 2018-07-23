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
    unsafe class ItemCardScipt_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ItemCardScipt);

            field = type.GetField("index", flag);
            app.RegisterCLRFieldGetter(field, get_index_0);
            app.RegisterCLRFieldSetter(field, set_index_0);
            field = type.GetField("weight", flag);
            app.RegisterCLRFieldGetter(field, get_weight_1);
            app.RegisterCLRFieldSetter(field, set_weight_1);


        }



        static object get_index_0(ref object o)
        {
            return ((ItemCardScipt)o).index;
        }
        static void set_index_0(ref object o, object v)
        {
            ((ItemCardScipt)o).index = (System.Int32)v;
        }
        static object get_weight_1(ref object o)
        {
            return ((ItemCardScipt)o).weight;
        }
        static void set_weight_1(ref object o, object v)
        {
            ((ItemCardScipt)o).weight = (System.Int32)v;
        }


    }
}
