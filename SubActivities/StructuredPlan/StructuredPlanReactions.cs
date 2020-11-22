using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Runtime;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.StructuredPlan;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.SubActivities.StructuredPlan
{
    [Activity(Label = "Structured Plan Reactions")]
    public class StructuredPlanReactions : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanReactions";

        private Toolbar _toolbar;

        private ListView _reactionsList;

        private Button _done;

        private int _selectedItemPosition = -1;

        private ImageLoader _imageLoader = null;

        public StructuredPlanReactions()
        {

        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemPosition", _selectedItemPosition);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ReactionsMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemPosition = savedInstanceState.GetInt("selectedItemPosition");

                SetContentView(Resource.Layout.StructuredPlanReactionsLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.reactionsToolbar, Resource.String.StructuredPlanReactionsActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanreactionspager,
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

                UpdateAdapter();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanReactionsCreate), "StructuredPlanReactions.OnCreate");
            }
        }
        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_reactionsList != null)
                _reactionsList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_reactionsList != null)
                {
                    _reactionsList.ItemClick += ReactionsList_ItemClick;
                    _reactionsList.ItemLongClick += ReactionsList_ItemLongClick;
                }
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanReactionsSetCallbacks), "StructuredPlanReactions.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void ReactionsList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var reactionID = GlobalData.StructuredPlanReactions[e.Position].ReactionsID;

            Intent intent = new Intent(this, typeof(StructuredPlanReactionsDialogActivity));
            intent
                .PutExtra("reactionsID", reactionID)
                .PutExtra("activityTitle", "Edit Reaction");

            StartActivityForResult(intent, ConstantsAndTypes.EDIT_REACTIONS);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            int reactionsID = -1;
            string to = "";
            int strength = -1;
            ConstantsAndTypes.REACTION_TYPE reaction = ConstantsAndTypes.REACTION_TYPE.Positive;
            ConstantsAndTypes.ACTION_TYPE intention = ConstantsAndTypes.ACTION_TYPE.Maintain;
            string actionOf = "";

            base.OnActivityResult(requestCode, resultCode, data);

            if(resultCode == Result.Ok)
            {
                try
                {
                    if (data.HasExtra("reactionsID"))
                        reactionsID = data.GetIntExtra("reactionsID", -1);
                    if (data.HasExtra("to"))
                        to = data.GetStringExtra("to");
                    if (data.HasExtra("strength"))
                        strength = data.GetIntExtra("strength", -1);
                    if (data.HasExtra("reaction"))
                        reaction = (ConstantsAndTypes.REACTION_TYPE)data.GetIntExtra("reaction", -1);
                    if (data.HasExtra("intention"))
                        intention = (ConstantsAndTypes.ACTION_TYPE)data.GetIntExtra("intention", -1);
                    if (data.HasExtra("actionOf"))
                        actionOf = data.GetStringExtra("actionOf");

                    Reactions reactionItem = null;
                    if (reactionsID == -1)
                    {
                        //new item
                        reactionItem = new Reactions();
                        reactionItem.IsNew = true;
                        reactionItem.IsDirty = false;
                    }
                    else
                    {
                        reactionItem = GlobalData.StructuredPlanReactions.Find(react => react.ReactionsID == reactionsID);
                        reactionItem.IsNew = false;
                        reactionItem.IsDirty = true;
                    }

                    reactionItem.ToWhat = to;
                    reactionItem.Action = intention;
                    reactionItem.ActionOf = actionOf;
                    reactionItem.Strength = strength;
                    reactionItem.Type = reaction;
                    reactionItem.Save();

                    if (reactionsID == -1)
                        GlobalData.StructuredPlanReactions.Add(reactionItem);

                    UpdateAdapter();
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "ConfirmPlanItemAddition: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanReactionsConfirm), "StructuredPlanReactions.ConfirmPlanItemAddition");
                }
            }

            if (resultCode == Result.Canceled)
            {
                Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
            }
        }

        private void Remove()
        {
            if (_selectedItemPosition != -1)
            {
                try
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.StructuredPlanReactionsRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.StructuredPlanReactionsRemoveAlertQuestion);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "Remove: Exception - " + ex.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanReactionsRemove), "StructuredPlanReactions.Remove");
                }
            }
            else
            {
                string warning = "";
                if(GlobalData.StructuredPlanReactions.Count == 0)
                {
                    warning = GetString(Resource.String.StructuredPlanReactionsWarning1);
                }
                else
                {
                    warning = GetString(Resource.String.StructuredPlanReactionsWarning2);
                }
                Toast.MakeText(this, warning, ToastLength.Short).Show();
            }
        }

        private void ReactionsList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemPosition = e.Position;
            Log.Info(TAG, "ReactionsList_ItemClick: Selected item at position " + _selectedItemPosition.ToString());
            UpdateAdapter();
            _reactionsList.SetSelection(_selectedItemPosition);
        }

        private void Add()
        {
            try
            {
                if (_reactionsList != null)
                {
                    Intent intent = new Intent(this, typeof(StructuredPlanReactionsDialogActivity));
                    intent
                        .PutExtra("reactionsID", -1)
                        .PutExtra("activityTitle", "Add Reaction");

                    StartActivityForResult(intent, ConstantsAndTypes.ADD_REACTIONS);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanReactionsAdd), "StructuredPlanReactions.Add_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.reactionsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.reactionsActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.reactionsActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanReactions.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(StructuredPlanActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void GetFieldComponents()
        {
            try
            {
                _reactionsList = FindViewById<ListView>(Resource.Id.lstStructuredPlanReactions);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanReactionsGetComponents), "StructuredPlanReactions.GetFieldComponents");
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    GoBack();
                    return true;
                }

                switch (item.ItemId)
                {
                    case Resource.Id.reactionsActionAdd:
                        Add();
                        return true;

                    case Resource.Id.reactionsActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.reactionsActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanReactionsHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void UpdateAdapter()
        {
            try
            {
                StructuredPlanReactionsListAdapter adapter = new StructuredPlanReactionsListAdapter(this);
                if (_reactionsList != null)
                    _reactionsList.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanReactionsUpdateAdapter), "StructuredPlanReactions.UpdateAdapter");
            }
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemPosition;
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                var reaction = GlobalData.StructuredPlanReactions[_selectedItemPosition];
                reaction.Remove();
                GlobalData.StructuredPlanReactions.Remove(reaction);
                UpdateAdapter();
                _selectedItemPosition = -1;
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Removing Reaction", "StructuredPlanReactions.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
        }
    }
}