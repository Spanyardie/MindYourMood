
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.Helpers;
using Android.Widget;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class AboutActivity : AppCompatActivity
    {
        private Toolbar _toolbar;
        private Button _done;
        private LinearLayout _aboutMain;

        private ImageLoader _imageLoader = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.About);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.aboutToolbar, Resource.String.AboutTitle, Color.White);

            GetFieldComponents();

            _imageLoader = ImageLoader.Instance;

            _imageLoader.LoadImage
            (
                "drawable://" + Resource.Drawable.philosophise,
                new ImageLoadingListener
                (
                    loadingComplete: (imageUri, view, loadedImage) =>
                    {
                        var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                        ImageLoader_LoadingComplete(null, args);
                    }
                )
            );

            SetupCallbacks();
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_aboutMain != null)
                _aboutMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void GetFieldComponents()
        {
            _done = FindViewById<Button>(Resource.Id.btnDone);
            _aboutMain = FindViewById<LinearLayout>(Resource.Id.linAboutMain);
        }

        private void SetupCallbacks()
        {
            if(_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, System.EventArgs e)
        {
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    Finish();
                    return true;
                }
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}