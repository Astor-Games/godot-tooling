namespace Turtles.addons.godot_lib.UI;

public static class PopupMenuExtension
{
    extension(PopupMenu menu)
    {
        public bool HandleCheckState(int index)
        {
            if (!menu.IsItemCheckable(index)) return false;
        
            var isChecked = menu.IsItemChecked(index);
            
            if (menu.IsItemRadioCheckable(index))
            {
                if (!isChecked)
                {
                    for (var idx = 0; idx < menu.ItemCount; idx++)
                    {
                        if (!menu.IsItemRadioCheckable(idx)) continue;
                        menu.SetItemChecked(idx, idx == index);
                    }
                }
                return true; // A checked radio button remains checked.
            }
            
            isChecked = !isChecked;
            menu.SetItemChecked(index, isChecked);
            
            return isChecked;
        }
    }
}