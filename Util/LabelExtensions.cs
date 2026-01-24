namespace GodotLib.Util;

public static class LabelExtensions
{
    extension(RichTextLabel label)
    {
        public void AddTitle(string title)
        {
            label.PushFontSize(18);
            label.AddText(title);
            label.Pop();
            label.AddHr(100);
            label.Newline();
        }
    }
}