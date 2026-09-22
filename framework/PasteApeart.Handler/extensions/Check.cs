using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PasteApeart.Handler.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerStepThrough]
    public static class Check
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T NotNull<T>([NotNull] T value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T NotNull<T>([NotNull] T value, string parameterName, string message)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName, message);
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="maxLength"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string NotNull([NotNull] string value, string parameterName, int maxLength = int.MaxValue, int minLength = 0)
        {
            if (value == null)
            {
                throw new ArgumentException(parameterName + " can not be null!", parameterName);
            }

            if (value.Length > maxLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
            }

            if (minLength > 0 && value.Length < minLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="maxLength"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string NotNullOrWhiteSpace([NotNull] string value, string parameterName, int maxLength = int.MaxValue, int minLength = 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(parameterName + " can not be null, empty or white space!", parameterName);
            }

            if (value.Length > maxLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
            }

            if (minLength > 0 && value.Length < minLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="maxLength"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string NotNullOrEmpty([NotNull] string value, string parameterName, int maxLength = int.MaxValue, int minLength = 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            if (value.Length > maxLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
            }

            if (minLength > 0 && value.Length < minLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static ICollection<T> NotNullOrEmpty<T>([NotNull] ICollection<T> value, string parameterName)
        {
            if (value == null || value.Count <= 0)
            {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            return value;
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="TBaseType"></typeparam>
        ///// <param name="type"></param>
        ///// <param name="parameterName"></param>
        ///// <returns></returns>
        ///// <exception cref="ArgumentException"></exception>
        //public static Type AssignableTo<TBaseType>(Type type, string parameterName)
        //{
        //    NotNull(type, parameterName);
        //    if (!type.IsAssignableTo<TBaseType>())
        //    {
        //        throw new ArgumentException($"{parameterName} (type of {type.AssemblyQualifiedName}) should be assignable to the {typeof(TBaseType).GetFullNameWithAssemblyName()}!");
        //    }

        //    return type;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="maxLength"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string Length(string value, string parameterName, int maxLength, int minLength = 0)
        {
            if (minLength > 0)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
                }

                if (value.Length < minLength)
                {
                    throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
                }
            }

            if (value != null && value.Length > maxLength)
            {
                throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static short Positive(short value, string parameterName)
        {
            if (value == 0)
            {
                throw new ArgumentException(parameterName + " is equal to zero");
            }

            if (value < 0)
            {
                throw new ArgumentException(parameterName + " is less than zero");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int Positive(int value, string parameterName)
        {
            if (value == 0)
            {
                throw new ArgumentException(parameterName + " is equal to zero");
            }

            if (value < 0)
            {
                throw new ArgumentException(parameterName + " is less than zero");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static long Positive(long value, string parameterName)
        {
            if (value == 0L)
            {
                throw new ArgumentException(parameterName + " is equal to zero");
            }

            if (value < 0)
            {
                throw new ArgumentException(parameterName + " is less than zero");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static float Positive(float value, string parameterName)
        {
            if (value == 0f)
            {
                throw new ArgumentException(parameterName + " is equal to zero");
            }

            if (value < 0f)
            {
                throw new ArgumentException(parameterName + " is less than zero");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double Positive(double value, string parameterName)
        {
            if (value == 0.0)
            {
                throw new ArgumentException(parameterName + " is equal to zero");
            }

            if (value < 0.0)
            {
                throw new ArgumentException(parameterName + " is less than zero");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static decimal Positive(decimal value, string parameterName)
        {
            if (value == 0m)
            {
                throw new ArgumentException(parameterName + " is equal to zero");
            }

            if (value < 0m)
            {
                throw new ArgumentException(parameterName + " is less than zero");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="minimumValue"></param>
        /// <param name="maximumValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static short Range(short value, string parameterName, short minimumValue, short maximumValue = short.MaxValue)
        {
            if (value < minimumValue || value > maximumValue)
            {
                throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="minimumValue"></param>
        /// <param name="maximumValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int Range(int value, string parameterName, int minimumValue, int maximumValue = int.MaxValue)
        {
            if (value < minimumValue || value > maximumValue)
            {
                throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="minimumValue"></param>
        /// <param name="maximumValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static long Range(long value, string parameterName, long minimumValue, long maximumValue = long.MaxValue)
        {
            if (value < minimumValue || value > maximumValue)
            {
                throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="minimumValue"></param>
        /// <param name="maximumValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static float Range(float value, string parameterName, float minimumValue, float maximumValue = float.MaxValue)
        {
            if (value < minimumValue || value > maximumValue)
            {
                throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="minimumValue"></param>
        /// <param name="maximumValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double Range(double value, string parameterName, double minimumValue, double maximumValue = double.MaxValue)
        {
            if (value < minimumValue || value > maximumValue)
            {
                throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <param name="minimumValue"></param>
        /// <param name="maximumValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static decimal Range(decimal value, string parameterName, decimal minimumValue, decimal maximumValue = decimal.MaxValue)
        {
            if (value < minimumValue || value > maximumValue)
            {
                throw new ArgumentException($"{parameterName} is out of range min: {minimumValue} - max: {maximumValue}");
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T NotDefaultOrNull<T>([NotNull] T? value, string parameterName) where T : struct
        {
            if (!value.HasValue)
            {
                throw new ArgumentException(parameterName + " is null!", parameterName);
            }

            if (value.Value.Equals(default(T)))
            {
                throw new ArgumentException(parameterName + " has a default value!", parameterName);
            }

            return value.Value;
        }
    }
}
