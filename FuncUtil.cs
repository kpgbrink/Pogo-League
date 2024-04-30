using System;

namespace Assets.Scripts
{
    // The natural thing that any C# programmer who also has used JavaScript before would write.
    class FuncUtil
    {
        public static Func<T, TResult> Infer<T, TResult>(Func<T, TResult> func) => func;
        public static T Invoke<T>(
            Func<T> func)
        {
            return func();
        }
    }
}
