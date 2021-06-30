namespace Assets.CaseFile.Models
{
    public class CustomImageModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int AttachedAlignment { get; set; }
        public byte[] ReferenceImage { get; set; }
        public DisplayModes DisplayMode { get; set; }
    }

    public enum DisplayModes
    {
        HalfView,
        FullView,
        FullScreenView
    }
}