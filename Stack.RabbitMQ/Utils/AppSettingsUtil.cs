using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Stack.RabbitMQ.Utils
{
    /// <summary>
    /// 应用程序配置处理类
    /// </summary>
    public sealed class AppSettingsUtil
    {
        /// <summary>
        /// 线程对象锁
        /// </summary>
        private static readonly object locker = new object();
        /// <summary>
        /// 配置文件信息
        /// </summary>
        private static readonly Dictionary<string, IConfiguration> ConfigDic = new Dictionary<string, IConfiguration>();
        /// <summary>
        /// 获取Json配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">配置文件的Key(格式：xxx:yyy，注意中间使用':'分割)</param>
        /// <returns></returns>
        public static T GetValue<T>(string key)
        {
            string fileName = "appsettings.json";
            IConfiguration configuration = GetJsonConfiguration(fileName);
            return ConfigurationBinder.GetValue<T>(configuration, key);
        }
        /// <summary>
        /// 获取Json配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileDir">文件目录(格式：D:\JsonConfigs)</param>
        /// <param name="fileName">文件名称（格式：xxx.json）</param>
        /// <returns></returns>
        public static T GetValue<T>(string fileDir, string fileName)
        {
            IConfiguration configuration = GetJsonConfiguration(fileDir, fileName);
            return ConfigurationBinder.Get<T>(configuration);
        }
        /// <summary>
        /// 获取Json配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileDir">文件目录(格式：D:\JsonConfigs)</param>
        /// <param name="fileName">文件名称（格式：xxx.json）</param>
        /// <param name="key">配置文件的Key(格式：xxx:yyy，注意中间使用':'分割)</param>
        /// <returns></returns>
        public static T GetValue<T>(string fileDir, string fileName, string key)
        {
            IConfiguration configuration = GetJsonConfiguration(fileDir, fileName);
            return ConfigurationBinder.GetValue<T>(configuration, key);
        }
        /// <summary>
        /// 根据文件完整的路径获取Json配置
        /// </summary>
        /// <param name="fileName">文件名称（格式：xxx.json）</param>
        /// <returns></returns>
        public static IConfiguration GetJsonConfiguration(string fileName)
        {
            var fileDir = Directory.GetCurrentDirectory();
            return GetJsonConfiguration(fileDir, fileName);
        }
        /// <summary>
        /// 根据文件目录+文件名称获取Json配置
        /// </summary>
        /// <param name="fileDir">文件目录(格式：D:\JsonConfigs)</param>
        /// <param name="fileName">文件名称（格式：xxx.json）</param>
        /// <returns></returns>
        public static IConfiguration GetJsonConfiguration(string fileDir, string fileName)
        {
            string fullFileName = Path.Combine(fileDir, fileName);
            if (!ConfigDic.ContainsKey(fullFileName))
            {
                lock (locker)
                {
                    if (!ConfigDic.ContainsKey(fullFileName))
                    {
                        ConfigurationBuilder builder = new ConfigurationBuilder();
                        IConfigurationBuilder configBuilder = FileConfigurationExtensions.SetBasePath(builder, fileDir);//设置配置文件基础目录
                        IConfigurationRoot configuration = JsonConfigurationExtensions.AddJsonFile(configBuilder, fileName, true, true).Build();//返回编译结果
                        ConfigDic.Add(fullFileName, configuration);
                    }
                }
            }
            return ConfigDic[fullFileName];
        }
    }
}