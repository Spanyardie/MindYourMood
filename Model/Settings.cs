using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace com.spanyardie.MindYourMood.Model
{
    public class Settings
    {
        private List<Setting> _settings;
        private int _currentIndex = 0;

        public List<Setting> SettingsList
        {
            get
            {
                return _settings;
            }

            set
            {
                _settings = value;
            }
        }

        public Settings()
        {
            _settings = new List<Setting>();
        }

        public bool LoadSettings()
        {
            throw new NotImplementedException();
        }

        public bool SaveSettings()
        {
            throw new NotImplementedException();
        }

        public List<Setting> GetSettingsForType(Setting.SETTING_TYPE settingType)
        {
            var chosenSettings =
                from settingItem in _settings
                where settingItem.SettingType == settingType
                select settingItem;
            return (List<Setting>)chosenSettings;
        }

        public bool MoveFirst()
        {
            if (_settings != null)
            {
                _currentIndex = 0;
                return true;
            }
            return false;
        }

        public bool MoveNext()
        {
            if(_settings != null)
            {
                if(_settings.Count > 1)
                {
                    if(_currentIndex < (_settings.Count - 2))
                        _currentIndex++;
                }
                return true;
            }
            return false;
        }

        public bool MoveLast()
        {
            if(_settings != null)
            {
                if (_settings.Count > 0)
                    _currentIndex = _settings.Count - 1;
                return true;
            }
            return false;
        }

        public Setting GetSetting()
        {
            if(_settings != null)
            {
                return _settings[_currentIndex];
            }
            return null;
        }

        public Setting GetSetting(int index)
        {
            if (_settings != null)
            {
                if (index >= 0 && index <= (_settings.Count - 1))
                {
                    return _settings[index];
                }
                else
                {
                    throw new IndexOutOfRangeException("Index out of bounds attempting to retrieve Setting at index '" + index + "'");
                }
            }
            return null;
        }

        public Setting GetSetting(string settingKey)
        {
            var settingItem =
                from item in _settings
                where item.SettingKey == settingKey
                select item;

            return (Setting)settingItem;
        }
    }
}