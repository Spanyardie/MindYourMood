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

namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class AutomaticThoughtsBase
    {
        private int _automaticThoughtsId;
        private long _thoughtRecordId;
        private string _thought;
        private bool _isHotThought;

        private bool _isNew;
        private bool _isDirty;

        public int AutomaticThoughtsId
        {
            get
            {
                return _automaticThoughtsId;
            }

            set
            {
                _automaticThoughtsId = value;
            }
        }

        public long ThoughtRecordId
        {
            get
            {
                return _thoughtRecordId;
            }

            set
            {
                _thoughtRecordId = value;
            }
        }

        public string Thought
        {
            get
            {
                return _thought;
            }

            set
            {
                _thought = value;
            }
        }

        public bool IsHotThought
        {
            get
            {
                return _isHotThought;
            }

            set
            {
                _isHotThought = value;
            }
        }

        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }

            set
            {
                _isDirty = value;
            }
        }

        public bool IsNew
        {
            get
            {
                return _isNew;
            }

            set
            {
                _isNew = value;
            }
        }
    }
}