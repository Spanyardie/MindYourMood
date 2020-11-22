namespace com.spanyardie.MindYourMood.Helpers
{
    public static class TextAnalyser
    {

        public static string Analyse(string textToAnalyse)
        {
            string[] splitWords = textToAnalyse.Split(new char[] { ' ' });

            for(var a = 0; a < splitWords.Length; a++)
            {
                foreach(var lookup in GlobalData._lookupTable)
                {
                    if(splitWords[a].ToLower() == lookup.Value)
                    {
                        splitWords[a] = lookup.Value;
                        break;
                    }
                }
            }
            return string.Join(" ", splitWords);
        }
    }
}