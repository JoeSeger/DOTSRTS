using System;

namespace DOTSRTS.Utilities.CSharp
{
    public class EnumExtensions
    {
        public static bool HasFlag<T>(T value, T flag) where T : Enum
        {
            var intValue = Convert.ToInt32(value);
            var intFlag = Convert.ToInt32(flag);

            return (intValue & intFlag) != 0;
        }
    }
}