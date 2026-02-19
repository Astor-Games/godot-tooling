namespace GodotLib.Util;

public static class MenuExtensions
{
    extension(Godot.PopupMenu menu)
    {
        public void AddCheckItem<T>(T value) where T : Enum
        {
            menu.AddCheckItem(Enum.GetName(value.GetType(), value).FormatSentence(), (int)(object)value);
        }
    }
}