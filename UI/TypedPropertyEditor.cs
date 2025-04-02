namespace Turtles;

public partial class PropertyEditor<[MustBeVariant] T> : PropertyEditor
{
    public Predicate<T> Validation;
}