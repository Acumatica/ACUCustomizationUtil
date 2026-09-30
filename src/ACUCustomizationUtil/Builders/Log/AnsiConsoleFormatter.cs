using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Formatting.Json;
using Serilog.Parsing;
using Serilog.Sinks.SystemConsole.Themes;

using Spectre.Console;

namespace ACUCustomizationUtils.Builders.Log;

/// <summary>
/// <see cref="MessageTemplateTextFormatter"/> adapted for <see cref="AnsiConsole.Console"/>
/// </summary>
public sealed class AnsiConsoleFormatter : IAnsiConsoleFormatter
{
    private static readonly JsonValueFormatter JsonFormatter = new("$type");

    private readonly LogTheme _theme;
    private readonly IFormatProvider? _formatProvider;
    private readonly MessageTemplate _outputTemplate;
    private readonly Action<LogEvent, List<StyledBlock>>[] _renderers;

    /// <summary>
    /// Instantiates new <see cref="AnsiConsoleFormatter"/>
    /// </summary>
    /// <param name="outputTemplate">Message template describing the output format</param>
    /// <param name="formatProvider">Supplies culture-specific formatting information, or null</param>
    public AnsiConsoleFormatter(string outputTemplate, IFormatProvider? formatProvider = null)
    {
        ArgumentNullException.ThrowIfNull(outputTemplate);
        _theme = LogTheme.Literate;
        _formatProvider = formatProvider;
        _outputTemplate = new MessageTemplateParser().Parse(outputTemplate);
        _renderers = [.. _outputTemplate.Tokens.Select(CreateRenderer)];
    }

    /// <inheritdoc/>
    public void Format(LogEvent logEvent, Paragraph output)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(output);

        List<StyledBlock> blocks = [];
        foreach (Action<LogEvent, List<StyledBlock>> renderer in _renderers)
        {
            renderer.Invoke(logEvent, blocks);
        }

        IEnumerable<StyledBlock> validBlocks = blocks.Where(r => r.Text.Length > 0);
        foreach (StyledBlock block in validBlocks)
        {
            _ = output.Append(block.Text, block.Style);
        }
    }

    private Action<LogEvent, List<StyledBlock>> CreateRenderer(MessageTemplateToken token)
    {
        if (token is TextToken text)
            return (_, output) => output.Add(new StyledBlock(text.Text, _theme[ConsoleThemeStyle.TertiaryText]));

        PropertyToken property = (PropertyToken)token;
        return property.PropertyName switch
        {
            OutputProperties.MessagePropertyName => (logEvent, output) => RenderMessage(logEvent, property, output),
            OutputProperties.PropertiesPropertyName => (logEvent, output) => RenderProperties(logEvent, property, output),
            OutputProperties.ExceptionPropertyName => RenderException,
            OutputProperties.LevelPropertyName => RenderBySerilogFormatter(property, logEvent => _theme[logEvent.Level.ToThemeStyle()]),
            OutputProperties.NewLinePropertyName => RenderBySerilogFormatter(property, _ => Style.Plain),
            _ => RenderBySerilogFormatter(property, _ => _theme[ConsoleThemeStyle.SecondaryText]),
        };
    }

    private void RenderMessage(LogEvent logEvent, PropertyToken token, List<StyledBlock> output)
    {
        bool isLiteral = token.Format?.Contains('l') ?? false;
        bool isJson = token.Format?.Contains('j') ?? false;

        Align(output, token.Alignment, runs =>
        {
            foreach (MessageTemplateToken part in logEvent.MessageTemplate.Tokens)
            {
                if (part is TextToken text)
                {
                    runs.Add(new StyledBlock(text.Text, _theme[ConsoleThemeStyle.Text]));
                    continue;
                }

                PropertyToken property = (PropertyToken)part;
                if (logEvent.Properties.TryGetValue(property.PropertyName, out LogEventPropertyValue? value))
                {

                    Render(value, property, isLiteral, isJson, runs);
                }
                else
                {

                    runs.Add(new StyledBlock(property.ToString(), _theme[ConsoleThemeStyle.Invalid]));
                }
            }
        });
    }

    private void RenderProperties(LogEvent logEvent, PropertyToken token, List<StyledBlock> output)
    {
        bool isJson = token.Format?.Contains('j') ?? false;
        IEnumerable<LogEventProperty> rest = logEvent.Properties
            .Where(p => !HasProperty(logEvent.MessageTemplate, p.Key) && !HasProperty(_outputTemplate, p.Key))
            .Select(p => new LogEventProperty(p.Key, p.Value));

        Align(output, token.Alignment, runs => Render(new StructureValue(rest), isJson, runs));
    }

    private void RenderException(LogEvent logEvent, List<StyledBlock> output)
    {
        if (logEvent.Exception is null)
            return;

        using StringReader reader = new(logEvent.Exception.ToString());
        while (reader.ReadLine() is { } line)
        {
            ConsoleThemeStyle style = line.StartsWith("   ", StringComparison.Ordinal) 
                ? ConsoleThemeStyle.SecondaryText 
                : ConsoleThemeStyle.Text;
            output.Add(new StyledBlock(line + Environment.NewLine, _theme[style]));
        }
    }

    private Action<LogEvent, List<StyledBlock>> RenderBySerilogFormatter(PropertyToken token, Func<LogEvent, Style> style)
    {
        MessageTemplateTextFormatter formatter = new(token.ToString(), _formatProvider);
        return (logEvent, output) =>
        {
            using StringWriter writer = new();
            formatter.Format(logEvent, writer);
            output.Add(new StyledBlock(writer.ToString(), style(logEvent)));
        };
    }

    private void Render(LogEventPropertyValue value, PropertyToken token, bool isLiteral, bool isJson, List<StyledBlock> output)
    {
        Align(output, token.Alignment, runs => Render(value, token.Format, isLiteral, isJson, runs));
    }

    private void Render(LogEventPropertyValue value, string? format, bool isLiteral, bool isJson, List<StyledBlock> output)
    {
        switch (value)
        {
            case ScalarValue { Value: string text } when isLiteral:
                output.Add(new StyledBlock(text, _theme[ConsoleThemeStyle.String]));
                break;
            case ScalarValue scalar when isJson && !isLiteral:
                Render(scalar, output);
                break;
            case ScalarValue scalar:
                Render(scalar, format, output);
                break;
            default:
                Render(value, isJson, output);
                break;
        }
    }

    private void Render(LogEventPropertyValue value, bool isJson, List<StyledBlock> output)
    {
        switch (value)
        {
            case ScalarValue scalar when isJson:
                Render(scalar, output);
                break;
            case ScalarValue scalar:
                Render(scalar, null, output);
                break;
            case SequenceValue sequence:
                Render(sequence, isJson, output);
                break;
            case StructureValue structure:
                Render(structure, isJson, output);
                break;
            case DictionaryValue dictionary:
                Render(dictionary, isJson, output);
                break;
            default:
                output.Add(new StyledBlock(value.ToString(), _theme[ConsoleThemeStyle.Text]));
                break;
        }
    }

    private void Render(ScalarValue scalar, string? format, List<StyledBlock> output)
    {
        string text = scalar.Value switch
        {
            null => "null",
            string s => format == "l" ? s : Quote(s),
            bool b => b ? "True" : "False",
            char c => $"'{c}'",
            _ => Render(scalar, format),
        };
        output.Add(new StyledBlock(text, _theme[scalar.Value.ToThemeStyle()]));
    }

    private string Render(ScalarValue scalar, string? format)
    {
        using StringWriter writer = new();
        scalar.Render(writer, format, _formatProvider);
        return writer.ToString();
    }

    private void Render(ScalarValue scalar, List<StyledBlock> output)
    {
        using StringWriter writer = new();
        JsonFormatter.Format(scalar, writer);
        output.Add(new StyledBlock(writer.ToString(), _theme[scalar.Value.ToThemeStyle()]));
    }

    private void Render(SequenceValue sequence, bool isJson, List<StyledBlock> output)
    {
        Punctuate("[", output);
        for (int i = 0; i < sequence.Elements.Count; i++)
        {
            if (i > 0)
                Punctuate(", ", output);
            Render(sequence.Elements[i], isJson, output);
        }
        Punctuate("]", output);
    }

    private void Render(StructureValue structure, bool isJson, List<StyledBlock> output)
    {
        if (!isJson && structure.TypeTag is not null)
        {
            output.Add(new StyledBlock(structure.TypeTag, _theme[ConsoleThemeStyle.Name]));
            output.Add(new StyledBlock(" ", Style.Plain));
        }

        Punctuate("{", output);
        for (int i = 0; i < structure.Properties.Count; i++)
        {
            if (i > 0)
                Punctuate(", ", output);
            LogEventProperty property = structure.Properties[i];
            output.Add(new StyledBlock(isJson ? Quote(property.Name) : property.Name, _theme[ConsoleThemeStyle.Name]));
            Punctuate(isJson ? ": " : "=", output);
            Render(property.Value, isJson, output);
        }

        if (isJson && structure.TypeTag is not null)
        {
            if (structure.Properties.Count > 0)
                Punctuate(", ", output);
            output.Add(new StyledBlock(Quote("$type"), _theme[ConsoleThemeStyle.Name]));
            Punctuate(": ", output);
            output.Add(new StyledBlock(Quote(structure.TypeTag), _theme[ConsoleThemeStyle.String]));
        }
        Punctuate("}", output);
    }

    private void Render(DictionaryValue dictionary, bool isJson, List<StyledBlock> output)
    {
        Punctuate("{", output);
        bool first = true;
        foreach ((ScalarValue key, LogEventPropertyValue value) in dictionary.Elements)
        {
            if (!first)
                Punctuate(", ", output);
            first = false;

            if (isJson)
            {
                output.Add(new StyledBlock(Quote(key.Value?.ToString() ?? "null"), _theme[key.Value.ToThemeStyle()]));
                Punctuate(": ", output);
            }
            else
            {
                Punctuate("[", output);
                Render(key, null, output);
                Punctuate("]=", output);
            }
            Render(value, isJson, output);
        }
        Punctuate("}", output);
    }

    private static void Align(List<StyledBlock> output, Alignment? alignment, Action<List<StyledBlock>> render)
    {
        if (alignment is null)
        {
            render(output);
            return;
        }

        List<StyledBlock> blocks = [];
        render(blocks);
        string padding = new(' ', Math.Max(0, alignment.Value.Width - blocks.Sum(r => r.Text.Length)));
        if (alignment.Value.Direction == AlignmentDirection.Right)
            output.Add(new StyledBlock(padding, Style.Plain));
        output.AddRange(blocks);
        if (alignment.Value.Direction == AlignmentDirection.Left)
            output.Add(new StyledBlock(padding, Style.Plain));
    }

    private void Punctuate(string text, List<StyledBlock> output)
    {
        output.Add(new StyledBlock(text, _theme[ConsoleThemeStyle.TertiaryText]));
    }

    private static string Quote(string text)
    {
        using StringWriter writer = new();
        JsonValueFormatter.WriteQuotedJsonString(text, writer);
        return writer.ToString();
    }

    private static bool HasProperty(MessageTemplate template, string name)
    {
        return template.Tokens.OfType<PropertyToken>().Any(t => t.PropertyName == name);
    }

    private readonly record struct StyledBlock(string Text, Style Style);
}
