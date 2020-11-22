
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Views;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class SystemHelper
    {
        public static ConstantsAndTypes.ScreenSize GetScreenSize()
        {
            var bitMaskResult = Application.Context.Resources.Configuration.ScreenLayout & ScreenLayout.SizeMask;

            switch(bitMaskResult)
            {
                case ScreenLayout.SizeSmall:
                    return ConstantsAndTypes.ScreenSize.Small;

                case ScreenLayout.SizeNormal:
                    return ConstantsAndTypes.ScreenSize.Normal;

                case ScreenLayout.SizeLarge:
                    return ConstantsAndTypes.ScreenSize.Large;

                case ScreenLayout.SizeXlarge:
                    return ConstantsAndTypes.ScreenSize.ExtraLarge;

                default:
                    return ConstantsAndTypes.ScreenSize.Normal;
            }
        }

        public static string GetIsoCountryAlias()
        {
            string isoCountry = "GBR";
            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    isoCountry = "GBR";
                    break;
                case "spa":
                    isoCountry = "ESP";
                    break;
                default:
                    isoCountry = "GBR";
                    break;
            }

            return isoCountry;
        }
    }
}