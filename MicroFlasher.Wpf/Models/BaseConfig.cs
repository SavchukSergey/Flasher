using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using MicroFlasher.Annotations;

namespace MicroFlasher.Models {
    public abstract class BaseConfig : INotifyPropertyChanged {
        private readonly string _keyPrefix;

        protected BaseConfig(string keyPrefix) {
            _keyPrefix = keyPrefix;
        }

        protected string KeyPrefix { get { return _keyPrefix; } }

        protected string GetKey(string key) {
            if (!string.IsNullOrWhiteSpace(KeyPrefix)) {
                key = KeyPrefix + key;
            }
            return key;
        }

        protected void UpdateConfig(string value, string key) {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");
            key = GetKey(key);

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var set = config.AppSettings.Settings[key];
            if (set != null) {
                config.AppSettings.Settings[key].Value = value;
            } else {
                config.AppSettings.Settings.Add(new KeyValueConfigurationElement(key, value));
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        protected void UpdateConfigBool(string key, bool val) {
            UpdateConfig(val ? "true" : "false", key);
        }

        protected void UpdateConfigEnum<TEnum>(string key, TEnum val) where TEnum : struct {
            UpdateConfig(val.ToString(), key);
        }

        private string GetConfig(string key) {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");
            key = GetKey(key);
            return ConfigurationManager.AppSettings[key];
        }

        protected string GetConfigString(string defaultValue, string key) {
            return GetConfig(key) ?? defaultValue;
        }

        protected int GetConfigInt(int defaultValue, string key) {
            var raw = GetConfig(key);
            int res;
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out res) ? res : defaultValue;
        }

        protected bool GetConfigBool(bool defaultValue, string key) {
            var raw = (GetConfig(key) ?? "").ToLowerInvariant();
            switch (raw) {
                case "true":
                    return true;
                case "false":
                    return false;
            }
            return defaultValue;
        }

        protected TEnum GetConfigEnum<TEnum>(TEnum defaultValue, string key) where TEnum : struct {
            var raw = GetConfig(key);
            TEnum res;
            return Enum.TryParse(raw, true, out res) ? res : defaultValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void Save();

        public abstract void ReadFromConfig();
    }
}
