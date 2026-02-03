namespace GodotLib.Util;

public static class LabelExtensions
{
    private const char FullBlock = '\u2588';
    
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

        public void AddTableRow(params Span<string> columns)
        {
            foreach (var column in columns)
            {
                label.PushCell();
                label.AddText(column);
                label.Pop();
            }
        }

        public void PushProgressBar(int size, double percent)
        {
            var loaded = Mathf.RoundToInt(size * percent);
        
            label.PushColor(Colors.LimeGreen);
            label.AddText(new string(FullBlock, loaded));
            label.Pop();
        
            label.PushColor(Colors.DarkGreen);
            label.AddText(new string(FullBlock, size - loaded));
            label.Pop();

            label.Newline();
        }
    }
}