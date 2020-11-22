using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.SubActivities.SafetyPlan;
using Android.Graphics;
using Android.Content;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class TellMyselfListAdapter : BaseAdapter
    {
        public const string TAG = "M:TellMyselfListAdapter";

        List<TellMyself> _tellMyselfEntries;
        Activity _activity;

        private ImageView _typeImage;
        private TextView _tellText;
        private ImageButton _tellPlay;

        private int _currentPosition;

        private bool _isPlaying = false;

        public TellMyselfListAdapter(Activity activity, int selectedItemIndex, bool isPlaying)
        {
            _activity = activity;
            _currentPosition = selectedItemIndex;
            _isPlaying = isPlaying;

            GetAllTellMyselfData();
        }

        private void GetAllTellMyselfData()
        {
            if (GlobalData.TellMyselfItemsList != null)
            {
                _tellMyselfEntries = GlobalData.TellMyselfItemsList;
                Log.Info(TAG, "GetAllTellMyselfData: Global not null, consists of " + GlobalData.TellMyselfItemsList.Count.ToString() + " items");
            }
            else
            {
                Log.Info(TAG, "GetAllTellMyselfData: Global item list is NULL!");
            }
        }

        public override int Count
        {
            get
            {
                if(_tellMyselfEntries != null)
                {
                    return _tellMyselfEntries.Count;
                }
                return -1;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if(_tellMyselfEntries != null)
            {
                if(_tellMyselfEntries.Count > 0)
                {
                    return _tellMyselfEntries[position].ID;
                }
            }
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.TellMyselfListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.TellMyselfListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                _typeImage = convertView.FindViewById<ImageView>(Resource.Id.imgTellTypeImage);

                _tellText = convertView.FindViewById<TextView>(Resource.Id.txtTellText);
                _tellPlay = convertView.FindViewById<ImageButton>(Resource.Id.imgbtnTellPlay);

                if (_typeImage != null)
                {
                    Log.Info(TAG, "GetView: _typeImage found, setting for Type...");
                    SetImageForType(_tellMyselfEntries[position].TellType);
                    _typeImage.SetMaxHeight(48);
                    _typeImage.SetMaxWidth(48);
                    _typeImage.Focusable = false;
                }
                if (_tellText != null)
                {
                    _tellText.Text = _tellMyselfEntries[position].TellType == ConstantsAndTypes.TELL_TYPE.Audio ? _tellMyselfEntries[position].TellTitle : _tellMyselfEntries[position].TellText;
                    Log.Info(TAG, "GetView: _tellText found, set to '" + _tellText.Text + "'");
                    _tellText.Focusable = false;
                }

                if (_tellPlay != null)
                {
                    Log.Info(TAG, "GetView: _tellPlay found...");
                    if (_tellMyselfEntries[position].TellType == ConstantsAndTypes.TELL_TYPE.Audio)
                    {
                        if (!_isPlaying)
                        {
                            Log.Info(TAG, "GetView: Not playing - setting default view.");
                            _tellPlay.Visibility = ViewStates.Visible;
                            _tellPlay.Enabled = true;
                            _tellPlay.Click += TellPlay_Click;
                        }
                        else
                        {
                            Log.Info(TAG, "GetView: PLAYING - setting greyscale image.");
                            _tellPlay.SetImageResource(Resource.Drawable.playClipGreyscale);
                            _tellPlay.Visibility = ViewStates.Visible;
                            _tellPlay.Enabled = false;
                        }
                    }
                    else
                    {
                        _tellPlay.Visibility = ViewStates.Invisible;
                        _tellPlay.Enabled = false;
                        Log.Info(TAG, "GetView: Not an Audio item, tellPlay visibility is invisible and enabled FALSE");
                    }
                    _tellPlay.Focusable = false;
                    _tellPlay.Tag = position;
                }
                else
                {
                    Log.Info(TAG, "GetView: _tellPlay NOT found!");
                }

                var parentHeldSelectedItemIndex = ((TellMyselfActivity)_activity).GetSelectedItemIndex();
                Log.Info(TAG, "GetView: parentHeldSelectedItemIndex - " + parentHeldSelectedItemIndex.ToString());
                if (parentHeldSelectedItemIndex != -1)
                {
                    if (position == parentHeldSelectedItemIndex)
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        _typeImage.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        _tellText.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        _tellPlay.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                    else
                    {
                        convertView.SetBackgroundDrawable(null);
                        _typeImage.SetBackgroundDrawable(null);
                        _tellText.SetBackgroundDrawable(null);
                        _tellPlay.SetBackgroundDrawable(null);
                    }
                }
                return convertView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Tell Myself Items View", "TellMyselfListAdapter.GetView");
                return convertView;
            }
        }

        private void TellPlay_Click(object sender, EventArgs e)
        {
            ImageButton clickedButton = null;

            try
            {
                clickedButton = (ImageButton)sender;
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "TellPlay_Click: Exception on cast - " + ex.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, "Playing Tell Myself Text Audio", "TellMyselfListAdapter.TellPlay_Click");
                return;
            }

            //what if another item is still playing its clip?
            if (_activity != null)
                ((TellMyselfActivity)_activity).PlayAudio((int)clickedButton.Tag);
        }

        private void SetImageForType(ConstantsAndTypes.TELL_TYPE tellType)
        {
            try
            {
                if (_typeImage != null)
                {
                    switch (tellType)
                    {
                        case ConstantsAndTypes.TELL_TYPE.Audio:
                            _typeImage.SetImageResource(Resource.Drawable.mic);
                            break;
                        case ConstantsAndTypes.TELL_TYPE.Textual:
                            _typeImage.SetImageResource(Resource.Drawable.text);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetImageForType: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting Tell Myself Image for Type", "TellMyselfListAdapter.SetImageForType");
            }
        }

    }
}