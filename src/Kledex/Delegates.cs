using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public class Helper
{
    private IDictionary<string, Func<object, object>> PropertyGetters { get; set; }

    private IDictionary<string, Action<object, object>> PropertySetters { get; set; }

    public static Func<object, object> CreateGetter(PropertyInfo property)
    {
        if (property == null)
            throw new ArgumentNullException("property");

        var getter = property.GetGetMethod();
        if (getter == null)
            throw new ArgumentException("The specified property does not have a public accessor.");

        var genericMethod = typeof(Helper).GetMethod("CreateGetterGeneric");
        MethodInfo genericHelper = genericMethod.MakeGenericMethod(property.DeclaringType, property.PropertyType);
        return (Func<object, object>)genericHelper.Invoke(null, new object[] { getter });
    }

    public static Func<object, object> CreateGetterGeneric<T, R>(MethodInfo getter) where T : class
    {
        Func<T, R> getterTypedDelegate = (Func<T, R>)Delegate.CreateDelegate(typeof(Func<T, R>), getter);
        Func<object, object> getterDelegate = (Func<object, object>)((object instance) => getterTypedDelegate((T)instance));
        return getterDelegate;
    }

    public static Action<object, object> CreateSetter(PropertyInfo property)
    {
        if (property == null)
            throw new ArgumentNullException("property");

        var setter = property.GetSetMethod();
        if (setter == null)
            throw new ArgumentException("The specified property does not have a public setter.");

        var genericMethod = typeof(Helper).GetMethod("CreateSetterGeneric");
        MethodInfo genericHelper = genericMethod.MakeGenericMethod(property.DeclaringType, property.PropertyType);
        return (Action<object, object>)genericHelper.Invoke(null, new object[] { setter });
    }

    public static Action<object, object> CreateSetterGeneric<T, V>(MethodInfo setter) where T : class
    {
        Action<T, V> setterTypedDelegate = (Action<T, V>)Delegate.CreateDelegate(typeof(Action<T, V>), setter);
        Action<object, object> setterDelegate = (Action<object, object>)((object instance, object value) => { setterTypedDelegate((T)instance, (V)value); });
        return setterDelegate;
    }

    public Helper(Type type)
    {
        var Properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && !p.GetIndexParameters().Any()).AsEnumerable();
        PropertyGetters = Properties.ToDictionary(p => p.Name, p => CreateGetter(p));
        PropertySetters = Properties.Where(p => p.GetSetMethod() != null)
            .ToDictionary(p => p.Name, p => CreateSetter(p));
    }
}

public static class Test
{
    static void Main()
    {
        MethodInfo indexOf = typeof(string).GetMethod("IndexOf", new Type[] { typeof(char) });
        MethodInfo getByteCount = typeof(Encoding).GetMethod("GetByteCount", new Type[] { typeof(string) });

        Func<string, object, object> indexOfFunc = MagicMethod<string>(indexOf);
        Func<Encoding, object, object> getByteCountFunc = MagicMethod<Encoding>(getByteCount);

        Console.WriteLine(indexOfFunc("Hello", 'e'));
        Console.WriteLine(getByteCountFunc(Encoding.UTF8, "Euro sign: \u20ac"));
    }

    public static Func<T, object, object> MagicMethod<T>(MethodInfo method) where T : class
    {
        // First fetch the generic form
        MethodInfo genericHelper = typeof(Test).GetMethod("MagicMethodHelper",
            BindingFlags.Static | BindingFlags.NonPublic);

        // Now supply the type arguments
        MethodInfo constructedHelper = genericHelper.MakeGenericMethod
            (typeof(T), method.GetParameters()[0].ParameterType, method.ReturnType);

        // Now call it. The null argument is because it's a static method.
        object ret = constructedHelper.Invoke(null, new object[] { method });

        // Cast the result to the right kind of delegate and return it
        return (Func<T, object, object>)ret;
    }

    public static Func<TTarget, object, object> MagicMethodHelper<TTarget, TParam, TReturn>(MethodInfo method)
        where TTarget : class
    {
        // Convert the slow MethodInfo into a fast, strongly typed, open delegate
        Func<TTarget, TParam, TReturn> func = (Func<TTarget, TParam, TReturn>)Delegate.CreateDelegate
            (typeof(Func<TTarget, TParam, TReturn>), method);

        // Now create a more weakly typed delegate which will call the strongly typed one
        Func<TTarget, object, object> ret = (TTarget target, object param) => func(target, (TParam)param);
        return ret;
    }
}