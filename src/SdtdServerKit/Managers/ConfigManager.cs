﻿using Autofac;
using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Data.IRepositories;
using System.Text;
using SdtdServerKit.Data.Entities;
using Newtonsoft.Json.Linq;

namespace SdtdServerKit.Managers
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    public static class ConfigManager
    {
        private static GlobalSettings? _globalSettings;

        /// <summary>
        /// 配置改变事件
        /// </summary>
        public static event Action<ISettings>? SettingsChanged;

        /// <summary>
        /// 全局配置
        /// </summary>
        public static GlobalSettings GlobalSettings 
        {
            get => _globalSettings ??= Get<GlobalSettings>();
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="TSettings">配置类型</typeparam>
        /// <returns></returns>
        public static TSettings Get<TSettings>(string culture = Cultures.ZhCn) where TSettings : ISettings
        {
            TSettings? settings;

            string name = typeof(TSettings).Name;
            var settingsRepository = ModApi.ServiceContainer.Resolve<ISettingsRepository>();
            var entity = settingsRepository.GetById(name);
            if (entity == null)
            {
                settings = LoadDefault<TSettings>(culture);
            }
            else
            {
                settings = JsonConvert.DeserializeObject<TSettings>(entity.SerializedValue, ModApi.JsonSerializerSettings);
                if (settings == null)
                {
                    throw new InvalidOperationException($"The persistent settings: {name} is null!");
                }
            }

            return settings;
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <typeparam name="TSettings">配置类型</typeparam>
        /// <param name="settings"></param>
        public static void Update<TSettings>(TSettings settings) where TSettings : ISettings
        {
            string name = typeof(TSettings).Name;

            try
            {
                if (settings is GlobalSettings globalSettings)
                {
                    _globalSettings = globalSettings;
                }
                else
                {
                    SettingsChanged?.Invoke(settings);
                }

                string json = JsonConvert.SerializeObject(settings, ModApi.JsonSerializerSettings);
                var settingsRepository = ModApi.ServiceContainer.Resolve<ISettingsRepository>();
                settingsRepository.InsertOrReplace(new T_Settings()
                {
                    Name = name,
                    SerializedValue = json
                });
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in ConfigManager.Update, SettingsName: {name}", name);
            }
        }

        /// <summary>
        /// 加载默认配置
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        public static TSettings LoadDefault<TSettings>(string culture = Cultures.ZhCn) where TSettings : ISettings
        {
            try
            {
                string fileName = "functionsettings." + culture.ToLower() + ".json";
                string path = ModApi.GetDefaultConfigPath(fileName);
                string json = File.ReadAllText(path, Encoding.UTF8);

                var jsonObject = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);

                if (jsonObject == null)
                {
                    throw new InvalidOperationException("Load settings faild, the json object is null.");
                }

                var settings = jsonObject[typeof(TSettings).Name]!.ToObject<TSettings>();
                if (settings == null)
                {
                    throw new InvalidOperationException("Load settings faild, the json can not deserialize.");
                }

                Update(settings);
                return settings;
            }
            catch (Exception ex)
            {
                throw new Exception("Load default settings faild.", ex);
            }
        }
    }
}