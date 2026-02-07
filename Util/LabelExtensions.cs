namespace GodotLib.Util;

public static class LabelExtensions
{
    private const char FullBlock = '\u2588';
    
    extension(RichTextLabel label)
    {
        public void AddTitle(string title)
        {
            label.PushBold();
            
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

        public void AddProgressBar(int size, double percent)
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

        public BoldScope Bold => new BoldScope(label);
        public FontSizeScope FontSize(int size) => new FontSizeScope(label, size);
        public CellScope Cell => new CellScope(label);
        public ColorScope Color(Color color) => new ColorScope(label, color);
        public FontScope Font(Font font, int fontSize = 0) => new FontScope(label, font, fontSize);
        public NormalScope Normal => new NormalScope(label);
        public BoldItalicsScope BoldItalics => new BoldItalicsScope(label);
        public ItalicsScope Italics => new ItalicsScope(label);
        public MonoScope Mono => new MonoScope(label);
        public OutlineSizeScope OutlineSize(int size) => new OutlineSizeScope(label, size);
        public IndentScope Indent(int level) => new IndentScope(label, level);
        public ListScope List(int level, RichTextLabel.ListType type, bool capitalize, string bullet = "•") => new ListScope(label, level, type, capitalize, bullet);
        public MetaScope Meta(Variant data, RichTextLabel.MetaUnderline underlineMode = RichTextLabel.MetaUnderline.Always, string tooltip = "") => new MetaScope(label, data, underlineMode, tooltip);
        public HintScope Hint(string description) => new HintScope(label, description);
        public LanguageScope Language(string language) => new LanguageScope(label, language);
        public TableScope Table(int columns, InlineAlignment inlineAlign = InlineAlignment.TopTo, int alignToRow = -1, string name = "") => new TableScope(label, columns, inlineAlign, alignToRow, name);
        public CustomFxScope CustomFx(RichTextEffect effect, GodotDictionary env) => new CustomFxScope(label, effect, env);
        public ContextScope Context => new ContextScope(label);
        public StrikethroughScope Strikethrough => new StrikethroughScope(label);
        public UnderlineScope Underline => new UnderlineScope(label);
        public ParagraphScope Paragraph(HorizontalAlignment alignment = HorizontalAlignment.Fill, Control.TextDirection baseDirection = Control.TextDirection.Inherited, string language = "", TextServer.StructuredTextParser stParser = TextServer.StructuredTextParser.Default, TextServer.JustificationFlag justificationFlags = TextServer.JustificationFlag.WordBound | TextServer.JustificationFlag.Kashida | TextServer.JustificationFlag.SkipLastLine | TextServer.JustificationFlag.DoNotSkipSingleLine, ReadOnlySpan<float> tabStops = default) => new ParagraphScope(label, alignment, baseDirection, language, stParser, justificationFlags, tabStops);
    }

    public readonly ref struct BoldScope : IDisposable
    {
        private readonly RichTextLabel label;

        public BoldScope(RichTextLabel label)
        {
            this.label = label;
            this.label.PushBold();
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct FontSizeScope : IDisposable
    {
        private readonly RichTextLabel label;

        public FontSizeScope(RichTextLabel label, int size)
        {
            this.label = label;
            this.label.PushFontSize(size);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct CellScope : IDisposable
    {
        private readonly RichTextLabel label;

        public CellScope(RichTextLabel label)
        {
            this.label = label;
            this.label.PushCell();
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct ColorScope : IDisposable
    {
        private readonly RichTextLabel label;

        public ColorScope(RichTextLabel label, Color color)
        {
            this.label = label;
            this.label.PushColor(color);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct FontScope : IDisposable
    {
        private readonly RichTextLabel label;

        public FontScope(RichTextLabel label, Font font, int fontSize)
        {
            this.label = label;
            this.label.PushFont(font, fontSize);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct NormalScope : IDisposable
    {
        private readonly RichTextLabel label;

        public NormalScope(RichTextLabel label)
        {
            this.label = label;
            this.label.PushNormal();
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct BoldItalicsScope : IDisposable
    {
        private readonly RichTextLabel label;

        public BoldItalicsScope(RichTextLabel label)
        {
            this.label = label;
            this.label.PushBoldItalics();
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct ItalicsScope : IDisposable
    {
        private readonly RichTextLabel label;

        public ItalicsScope(RichTextLabel label)
        {
            this.label = label;
            this.label.PushItalics();
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct MonoScope : IDisposable
    {
        private readonly RichTextLabel label;

        public MonoScope(RichTextLabel label)
        {
            this.label = label;
            this.label.PushMono();
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct OutlineSizeScope : IDisposable
    {
        private readonly RichTextLabel label;

        public OutlineSizeScope(RichTextLabel label, int size)
        {
            this.label = label;
            this.label.PushOutlineSize(size);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct IndentScope : IDisposable
    {
        private readonly RichTextLabel label;

        public IndentScope(RichTextLabel label, int level)
        {
            this.label = label;
            this.label.PushIndent(level);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct ListScope : IDisposable
    {
        private readonly RichTextLabel label;

        public ListScope(RichTextLabel label, int level, RichTextLabel.ListType type, bool capitalize, string bullet)
        {
            this.label = label;
            this.label.PushList(level, type, capitalize, bullet);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct MetaScope : IDisposable
    {
        private readonly RichTextLabel label;

        public MetaScope(RichTextLabel label, Variant data, RichTextLabel.MetaUnderline underlineMode, string tooltip)
        {
            this.label = label;
            this.label.PushMeta(data, underlineMode, tooltip);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct HintScope : IDisposable
    {
        private readonly RichTextLabel label;

        public HintScope(RichTextLabel label, string description)
        {
            this.label = label;
            this.label.PushHint(description);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct LanguageScope : IDisposable
    {
        private readonly RichTextLabel label;

        public LanguageScope(RichTextLabel label, string language)
        {
            this.label = label;
            this.label.PushLanguage(language);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct TableScope : IDisposable
    {
        private readonly RichTextLabel label;

        public TableScope(RichTextLabel label, int columns, InlineAlignment inlineAlign, int alignToRow, string name)
        {
            this.label = label;
            this.label.PushTable(columns, inlineAlign, alignToRow, name);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct CustomFxScope : IDisposable
    {
        private readonly RichTextLabel label;

        public CustomFxScope(RichTextLabel label, RichTextEffect effect, GodotDictionary env)
        {
            this.label = label;
            this.label.PushCustomfx(effect, env);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct ContextScope : IDisposable
    {
        private readonly RichTextLabel label;

        public ContextScope(RichTextLabel label)
        {
            this.label = label;
            this.label.PushContext();
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct StrikethroughScope : IDisposable
    {
        private readonly RichTextLabel label;

        public StrikethroughScope(RichTextLabel label)
        {
            this.label = label;
            this.label.PushStrikethrough();
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct UnderlineScope : IDisposable
    {
        private readonly RichTextLabel label;

        public UnderlineScope(RichTextLabel label)
        {
            this.label = label;
            this.label.PushUnderline();
        }

        public void Dispose()
        {
            label.Pop();
        }
    }

    public readonly ref struct ParagraphScope : IDisposable
    {
        private readonly RichTextLabel label;

        public ParagraphScope(RichTextLabel label, HorizontalAlignment alignment, Control.TextDirection baseDirection, string language, TextServer.StructuredTextParser stParser,
            TextServer.JustificationFlag justificationFlags, ReadOnlySpan<float> tabStops)
        {
            this.label = label;
            this.label.PushParagraph(alignment, baseDirection, language, stParser, justificationFlags, tabStops);
        }

        public void Dispose()
        {
            label.Pop();
        }
    }
}