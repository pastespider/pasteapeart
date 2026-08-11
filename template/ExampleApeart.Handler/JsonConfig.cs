//using System.Globalization;
//using System.Text.Encodings.Web;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.Text.Json.Serialization.Metadata;

//namespace $ext_safeprojectname$.Handler
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public static class JsonConfig
//    {

//        static JsonConfig()
//        {
//            Options.Converters.Add(new SafeStringConverter());
//            Options.Converters.Add(new SafeDateTimeConverter());
//            //Options.Converters.Add(new SafeEnumConverter<EnumRoleType>());
//            //Options.Converters.Add(new SafeNullableDateTimeConverter());
//            //SafeNullableDateTimeConverter 
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public static readonly JsonSerializerOptions Options = new()
//        {
//            WriteIndented = true,
//            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//            PropertyNameCaseInsensitive = true,
//            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
//            NumberHandling = JsonNumberHandling.AllowReadingFromString,
//            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
//            ReferenceHandler = ReferenceHandler.IgnoreCycles,  // 忽略循环引用
//            // 关键在这里：组合解析器会自动查找
//            // 先在 MyJsonContext 找，找不到自动用反射
//            TypeInfoResolver = JsonTypeInfoResolver.Combine(
//        //PasteSpiderJsonContext.Default,           // 源生成器部分
//        new DefaultJsonTypeInfoResolver() // 反射兜底
//            )
//        };
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class SafeStringConverter : JsonConverter<string>
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="reader"></param>
//        /// <param name="typeToConvert"></param>
//        /// <param name="options"></param>
//        /// <returns></returns>
//        /// <exception cref="JsonException"></exception>
//        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        {
//            switch (reader.TokenType)
//            {
//                case JsonTokenType.String:
//                    return reader.GetString();

//                case JsonTokenType.Number:
//                    // 方法1：读取为 long（可能溢出）
//                    if (reader.TryGetInt64(out long longValue))
//                    {
//                        return longValue.ToString();
//                    }
//                    // 方法2：读取为 decimal（范围更大）
//                    if (reader.TryGetDecimal(out decimal decimalValue))
//                    {
//                        return decimalValue.ToString();
//                    }
//                    // 方法3：读取为 double（可能丢失精度）
//                    return reader.GetDouble().ToString();

//                case JsonTokenType.Null:
//                    return null;

//                case JsonTokenType.True:
//                    return "true";

//                case JsonTokenType.False:
//                    return "false";

//                default:
//                    throw new JsonException($"Cannot convert {reader.TokenType} to string");
//            }
//        }

//        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
//        {
//            writer.WriteStringValue(value);
//        }
//    }

//    public class SafeDateTimeConverter : JsonConverter<DateTime>
//    {
//        // 支持的常见日期时间格式
//        private static readonly string[] _supportedFormats = new[]
//        {
//        "yyyy-MM-dd HH:mm:ss",
//        "yyyy-MM-dd HH:mm:ss.fff",
//        "yyyy-MM-ddTHH:mm:ss",
//        "yyyy-MM-ddTHH:mm:ss.fff",
//        "yyyy-MM-dd HH:mm",
//        "yyyy-MM-dd",
//        "yyyy/MM/dd HH:mm:ss",
//        "yyyy/MM/dd HH:mm:ss.fff",
//        "yyyy/MM/dd",
//        "dd/MM/yyyy HH:mm:ss",
//        "dd/MM/yyyy HH:mm:ss.fff",
//        "dd/MM/yyyy"
//    };

//        // 关键改动：在不变模式下使用的备用 CultureInfo
//        private static readonly CultureInfo _safeCulture;

//        static SafeDateTimeConverter()
//        {
//            try
//            {
//                // 尝试获取 InvariantCulture（正常模式）
//                _safeCulture = CultureInfo.InvariantCulture;
//            }
//            catch
//            {
//                // 不变模式下，InvariantCulture 不可用，使用固定中文
//                _safeCulture = new CultureInfo("zh-CN");
//            }
//        }

//        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        {
//            if (reader.TokenType == JsonTokenType.String)
//            {
//                string value = reader.GetString();

//                if (string.IsNullOrWhiteSpace(value))
//                {
//                    throw new JsonException("日期时间不能为空");
//                }

//                // 尝试使用指定格式解析
//                foreach (string format in _supportedFormats)
//                {
//                    if (DateTime.TryParseExact(value, format,
//                        _safeCulture,  // 使用安全的文化
//                        DateTimeStyles.None,
//                        out DateTime date))
//                    {
//                        return date;
//                    }
//                }

//                // 最后尝试使用系统默认解析（兜底）
//                if (DateTime.TryParse(value, _safeCulture, DateTimeStyles.None, out DateTime result))
//                {
//                    return result;
//                }

//                // 都失败了，抛出明确异常
//                throw new JsonException($"无法将 '{value}' 转换为 DateTime。支持的格式包括: {string.Join(", ", _supportedFormats)}");
//            }

//            // 处理数字时间戳
//            if (reader.TokenType == JsonTokenType.Number)
//            {
//                long timestamp = reader.GetInt64();
//                return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
//            }

//            throw new JsonException($"意外的 token 类型: {reader.TokenType}，期望字符串或数字");
//        }

//        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
//        {
//            // 输出使用标准格式
//            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss", _safeCulture));
//        }
//    }

//    /// <summary>
//    /// 枚举转换器 - 反序列化支持字符串数字/数字/枚举名，序列化输出数字
//    /// </summary>
//    public class SafeEnumConverter<T> : JsonConverter<T> where T : struct, Enum
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="reader"></param>
//        /// <param name="typeToConvert"></param>
//        /// <param name="options"></param>
//        /// <returns></returns>
//        /// <exception cref="JsonException"></exception>
//        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        {
//            switch (reader.TokenType)
//            {
//                case JsonTokenType.String:
//                    string stringValue = reader.GetString();

//                    // 1. 尝试按枚举名称解析（不区分大小写）
//                    if (Enum.TryParse<T>(stringValue, true, out T enumValue))
//                        return enumValue;

//                    // 2. 尝试按数字字符串解析（如 "1", "2"）
//                    if (int.TryParse(stringValue, out int intValue))
//                    {
//                        if (Enum.IsDefined(typeof(T), intValue))
//                            return (T)(object)intValue;
//                    }
//                    break;

//                case JsonTokenType.Number:
//                    // 直接的数字值（如 1,2,3）
//                    int numValue = reader.GetInt32();
//                    if (Enum.IsDefined(typeof(T), numValue))
//                        return (T)(object)numValue;
//                    break;

//                case JsonTokenType.Null:
//                    return default;
//            }

//            throw new JsonException($"Cannot convert '{reader.TokenType}' to enum type '{typeof(T).Name}'");
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="writer"></param>
//        /// <param name="value"></param>
//        /// <param name="options"></param>
//        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
//        {
//            // 关键：序列化时输出数字值，而不是名称
//            writer.WriteNumberValue(Convert.ToInt32(value));
//        }
//    }

//}
