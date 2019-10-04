using System;

namespace Jst4Pipes
{
    public static class PartialExecutionHelperMethods
    {
        /// <summary>
        /// Partial execution starting from Right parameter
        /// </summary>
        /// <param name="action">function of type (T1, T2) -> void</param>
        /// <param name="value2">parameter 2 from target action of type T2</param>
        /// <returns>function of type (T1 value1) -> void (with 1 less parameter)</returns>
        public static Func<T1, T1> PartialRight<T1, T2>(
            this Func<T1, T2, T1> function, T2 value2) =>
                (value1) => function(value1, value2);
    }
}
