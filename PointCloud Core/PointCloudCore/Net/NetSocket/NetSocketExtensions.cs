using Newtonsoft.Json;
using PointCloudCore.DomainCore;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PointCloudCore.Net.NetSocket
{
    /// <summary>
    /// 框架：NetSocket模块拓展方法
    /// </summary>
    public static class NetSocketExtensions
    {
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="encoding">编码</param>
        /// <param name="String">字符串</param>
        /// <returns></returns>
        public static byte[] stringSerializer(this string String, Encoding encoding)
        {
            try
            {
                return encoding.GetBytes(String);
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return null;
            }
        }

        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="encoding">编码</param>
        /// <param name="bytes">二进制流</param>
        /// <returns></returns>
        public static string stringDeserializer(this byte[] bytes, Encoding encoding)
        {
            try
            {
                return encoding.GetString(bytes);
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return null;
            }
        }

        /// <summary>
        /// 将对象转换成Json字符串
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <returns></returns>
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        /// <summary>
        /// 将JSON字符串转化为对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="Json">JSON字符串</param>
        /// <returns></returns>
        public static T Json2Object<T>(this string Json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(Json);
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return default(T);
            }
        }

        /// <summary>
        /// 对象直接序列化成Json二进制流
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">对象</param>
        /// <returns></returns>
        public static byte[] ToJsonByte<T>(this T data)
        {
            try
            {
                return data.ToJson().stringSerializer(Encoding.UTF8);
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return null;
            }
        }

        /// <summary>
        /// JSON二进制流反序列化为对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="bytes">二进制流</param>
        /// <returns></returns>
        public static T Json2ObjectDeserialize<T>(this byte[] bytes)
        {
            try
            {
                return bytes.stringDeserializer(Encoding.UTF8).Json2Object<T>();
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return default(T);
            }
        }

        /// <summary>
        /// 系统序列化方式
        /// </summary>
        /// <param name="data">对象</param>
        /// <returns></returns>
        public static byte[] SystemSerializer<T>(this T data)
        {
            try
            {
                byte[] bytes = new byte[NetSocketCore.ByteSize];
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, data);
                    ms.Flush();
                    bytes = ms.ToArray();
                }
                return bytes;
            }
            catch (ErrorException ex)
            {
                ErrorMessage.GetError(ex);
                return null;
            }
        }

        /// <summary>
        /// 系统反序列化为对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="bytes">二进制流</param>
        /// <returns></returns>
        public static T SystemDeserializer<T>(this byte[] bytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(bytes, 0, bytes.Length);
                    ms.Flush();
                    ms.Position = 0;
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Binder = new SerializableFind();
                    return (T)bf.Deserialize(ms);
                }
            }
            catch (ErrorException e)
            {
                ErrorMessage.GetError(e);
                return default(T);
            }
        }
    }
}
