using Newtonsoft.Json;
using Serilog;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace AntSK.Domain.Utils
{
    public static class ConvertUtils
    {
        /// <summary>
        /// 判断是否为空，为空返回true
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNull(this object data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否为空，为空返回true
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object data)
        {
            //如果为null
            if (data == null)
            {
                return false;
            }

            //如果为""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断是否为空，为空返回true
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNull(string data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 将obj类型转换为string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ConvertToString(this object s)
        {
            if (s == null)
            {
                return "";
            }
            else
            {
                return Convert.ToString(s);
            }
        }

        /// <summary>
        /// object 转int32
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Int32 ConvertToInt32(this object s)
        {
            int i = 0;
            if (s == null)
            {
                return 0;
            }
            else
            {
                int.TryParse(s.ToString(), out i);
            }
            return i;
        }

        /// <summary>
        /// object 转int32
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Int64 ConvertToInt64(this object s)
        {
            long i = 0;
            if (s == null)
            {
                return 0;
            }
            else
            {
                long.TryParse(s.ToString(), out i);
            }
            return i;
        }

        /// <summary>
        /// 将字符串转double
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ConvertToDouble(this object s)
        {
            double i = 0;
            if (s == null)
            {
                return 0;
            }
            else
            {
                double.TryParse(s.ToString(), out i);
            }
            return i;
        }

        /// <summary>
        /// 转换为datetime类型
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(this string s)
        {
            DateTime dt = new DateTime();
            if (s == null || s == "")
            {
                return DateTime.Now;
            }
            DateTime.TryParse(s, out dt);
            return dt;
        }

        /// <summary>
        /// 转换为datetime类型的格式字符串
        /// </summary>
        /// <param name="s">要转换的对象</param>
        /// <param name="y">格式化字符串</param>
        /// <returns></returns>
        public static string ConvertToDateTime(this string s, string y)
        {
            DateTime dt = new DateTime();
            DateTime.TryParse(s, out dt);
            return dt.ToString(y);
        }

        /// <summary>
        /// 将字符串转换成decimal
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(this object s)
        {
            decimal d = 0;
            if (s == null || s == "")
            {
                return 0;
            }

            Decimal.TryParse(s.ToString(), out d);

            return d;
        }

        /// <summary>
        /// decimal保留2位小数
        /// </summary>
        public static decimal DecimalFraction(this decimal num)
        {
            return Convert.ToDecimal(num.ToString("f2"));
        }

        /// <summary>
        /// 替换html种的特殊字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceHtml(this string s)
        {
            return s.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").Replace("&quot;", "\"");
        }

        /// <summary>
        /// 流转byte
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToByte(this Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public static string JsonToMarkDown(this string s)
        {
            return $"{Environment.NewLine}```json{Environment.NewLine}{s}{Environment.NewLine}```{Environment.NewLine}";
        }

        /// <summary>
        /// json参数转化querystring参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ToQueryString(this Dictionary<string, string> parameters)
        {
            var nameValueCollection = HttpUtility.ParseQueryString(string.Empty);

            foreach (var param in parameters)
            {
                nameValueCollection[param.Key] = param.Value;
            }

            return nameValueCollection.ToString();
        }

        /// <summary>
        /// 忽略大小写匹配
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ComparisonIgnoreCase(this string s, string value)
        {
            return s.Equals(value, StringComparison.OrdinalIgnoreCase);
        }


        /// <summary>
        /// \uxxxx转中文,保留换行符号
        /// </summary>
        /// <param name="unicodeString"></param>
        /// <returns></returns>
        public static string Unescape(this string value)
        {
            if (value.IsNull())
            {
                return "";
            }

            try
            {
                Formatting formatting = Formatting.None;

                object jsonObj = JsonConvert.DeserializeObject(value);
                string unescapeValue = JsonConvert.SerializeObject(jsonObj, formatting);
                return unescapeValue;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return "";
            }
        }


        /// <summary>
        /// 是否为流式请求
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsStream(this string value)
        {
            // 正则表达式忽略空格的情况
            string pattern = @"\s*""stream""\s*:\s*true\s*";

            // 使用正则表达式匹配
            bool contains = Regex.IsMatch(value, pattern);
            return contains;
        }

        public static string AntSKCalculateSHA256(this BinaryData binaryData)
        {
            byte[] byteArray = SHA256.HashData(binaryData.ToMemory().Span);
            return Convert.ToHexString(byteArray).ToLowerInvariant();
        }
    }
}
