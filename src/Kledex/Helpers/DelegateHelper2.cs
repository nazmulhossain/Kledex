using Kledex.Commands;
using System;
using System.Reflection;

namespace Kledex.Helpers
{
    public static class DelegateHelper2
    {
        public static Func<object, object, object> CreateDelegate(MethodInfo method, object param)
        {
            var paramType = param.GetType();
            var handlerType = typeof(ICommandHandlerAsync<>).MakeGenericType(paramType);

            // First fetch the generic form
            MethodInfo genericHelper = typeof(DelegateHelper2).GetMethod("CreateDelegateHelper",
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod
                (handlerType, paramType, method.ReturnType);

            // Now call it. The null argument is because it's a static method.
            object ret = constructedHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<object, object, object>)ret;
        }

        private static Func<object, object, object> CreateDelegateHelper<TTarget, TParam, TReturn>(MethodInfo method)
            where TTarget : class
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Func<TTarget, TParam, TReturn> func = (Func<TTarget, TParam, TReturn>)Delegate.CreateDelegate
                (typeof(Func<TTarget, TParam, TReturn>), method);

            // Now create a more weakly typed delegate which will call the strongly typed one
            Func<object, object, object> ret = (object target, object param) => func((TTarget)target, (TParam)param);
            return ret;
        }
    }
}
