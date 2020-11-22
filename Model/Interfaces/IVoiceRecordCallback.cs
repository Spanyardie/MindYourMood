namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IVoiceRecordCallback
    {
        void RecordingCompleted(string path, string title);
        void RecordingCancelled(string filePath);
    }
}