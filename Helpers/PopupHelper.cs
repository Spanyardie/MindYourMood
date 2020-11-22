using System;
using Android.Widget;
using Android.App;
using Android.Views;
using Android.Content;

namespace MindYourMood
{
	public class PopupHelper : PopupWindow
	{
		public struct Point
		{
			public int x;
            public int y;

			public Point(int xval, int yval)
			{
				x = xval;
				y = yval;
			}
		}

		public struct Dimension
		{
            public int height;
            public int width;

			public Dimension(int dimHeight, int dimWidth)
			{
				height = dimHeight;
				width = dimWidth;
			}
		}

		public const string TAG = "M:PopupHelper";

		private Activity _activity;

		public string ActivityText { get; set; }
		public int ActivityAchievement { get; set; }
		public int ActivityIntimacy { get; set; }
		public int ActivityPleasure { get; set; }

        private RelativeLayout _root;
        private EditText _activityText;
        private SeekBar _achievement;
        private SeekBar _intimacy;
        private SeekBar _pleasure;
        private ImageButton _goBack;
        private ImageButton _confirm;

        private PopupWindow _popupWindow;

        public bool Cancelled { get; set; }

        public PopupHelper(Activity activity, string activityText, int achievement, int intimacy, int pleasure)
		{
			_activity = activity;
			ActivityText = activityText;
			ActivityAchievement = achievement;
			ActivityIntimacy = intimacy;
			ActivityPleasure = pleasure;
            Cancelled = false;
		}

		public void Show(Dimension dimension, Point position)
		{
            View view = null;

            LayoutInflater inflater = (LayoutInflater)_activity.GetSystemService(Context.LayoutInflaterService);
            if(inflater != null)
            {
                view = inflater.Inflate(Resource.Layout.ActivityPopupLayout, null);
                if (view != null)
                {
                    GetFieldComponents(view);
                    SetupCallbacks();

                    _popupWindow = new PopupWindow(view, dimension.width, dimension.height, true);

                    if(_popupWindow != null)
                    {
                        _popupWindow.ShowAtLocation(view, 0, position.x, position.y);
                    }
                }
            }
		}

        private void SetupCallbacks()
        {
            if(_goBack != null)
                _goBack.Click += GoBack_Click;
            if(_confirm != null)
                _confirm.Click += Confirm_Click;
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            if(_activityText != null)
            {
                if(!string.IsNullOrEmpty(_activityText.Text.Trim()))
                {
                    ActivityText = _activityText.Text.Trim();
                }
                else
                {
                    Toast.MakeText(_activity, Resource.String.PopupConfirmNoSelectionToast, ToastLength.Short).Show();
                    return;
                }
                if(_achievement != null)
                    ActivityAchievement = _achievement.Progress;
                if(_intimacy != null)
                    ActivityIntimacy = _intimacy.Progress;
                if(_pleasure != null)
                    ActivityPleasure = _pleasure.Progress;

                Cancelled = false;

                _popupWindow.Dismiss();
            }
        }

        private void GoBack_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            _popupWindow.Dismiss();
        }

        private void GetFieldComponents(View view)
        {
            _root = view.FindViewById<RelativeLayout>(Resource.Id.popupWindow);
            _activityText = view.FindViewById<EditText>(Resource.Id.edtActivitiesPopupText);
            _achievement = view.FindViewById<SeekBar>(Resource.Id.skbActivitiesPopupAchievement);
            _intimacy = view.FindViewById<SeekBar>(Resource.Id.skbActivitiesPopupIntimacy);
            _pleasure = view.FindViewById<SeekBar>(Resource.Id.skbActivitiesPopupPleasure);
            _goBack = view.FindViewById<ImageButton>(Resource.Id.imgbtnActivitiesPopupGoBack);
            _confirm = view.FindViewById<ImageButton>(Resource.Id.imgbtnActivitiesPopupConfirm);
        }
    }
}

