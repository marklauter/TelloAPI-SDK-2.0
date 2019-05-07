using System;

namespace Tello.Types
{
    internal static class Primitive
    {
        public static bool IsNumeric<T>(this T value)
        {
            if (value == null)
            {
                return false;
            }

            var type = typeof(T);
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsString<T>(this T value)
        {
            if (value == null)
            {
                return false;
            }

            var type = typeof(T);
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Char:
                case TypeCode.String:
                    return true;
                default:
                    return false;
            }
        }
    }
}
