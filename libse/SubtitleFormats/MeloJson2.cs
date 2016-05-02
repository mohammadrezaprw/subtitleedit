using System;
using System.Collections.Generic;
using System.Text;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public class MeloJson2 : SubtitleFormat
    {
        public override string Extension
        {
            get { return ".json"; }
        }

        public override string Name
        {
            get { return "Melo Json 2"; }
        }

        public override bool IsTimeBased
        {
            get { return true; }
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            var subtitle = new Subtitle();
            LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > _errorCount;
        }

        public override string ToText(Subtitle subtitle, string title)
        {
            var sb = new StringBuilder(@"[");
            int count = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (count > 0)
                    sb.Append(',');
                sb.Append("{\"start_millis\":");
                sb.Append(p.StartTime.TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture));      
                sb.Append(",\"end_millis\":");
                sb.Append(p.EndTime.TotalMilliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture));

                sb.Append(",\"start_of_paragraph\":");
                sb.Append(p.StartOfParagraph.ToString(System.Globalization.CultureInfo.InvariantCulture));

                sb.Append(",\"start_of_statement\":");
                sb.Append(p.StartOfStatement.ToString(System.Globalization.CultureInfo.InvariantCulture));

                sb.Append(",\"isolate_mean\":");
                sb.Append(p.IsolateMean.ToString(System.Globalization.CultureInfo.InvariantCulture));

                sb.Append(",\"end_of_statement\":");
                sb.Append(p.EndOfStatement.ToString(System.Globalization.CultureInfo.InvariantCulture));

                sb.Append(",\"end_of_paragraph\":");
                sb.Append(p.EndOfParagraph.ToString(System.Globalization.CultureInfo.InvariantCulture));


                sb.Append(",\"text\":\"");
                sb.Append(Json.EncodeJsonText(p.Text));
                sb.Append("\"}");
                count++;
            }
            sb.Append(']');
            return sb.ToString().Trim();
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            _errorCount = 0;

            var sb = new StringBuilder();
            foreach (string s in lines)
                sb.Append(s);
            if (!sb.ToString().TrimStart().StartsWith("[{\"", StringComparison.Ordinal))
                return;

            foreach (string line in sb.ToString().Replace("},{", Environment.NewLine).SplitToLines())
            {
                string s = line.Trim() + "}";
                string start = Json.ReadTag(s, "start_millis");
                string end = Json.ReadTag(s, "end_millis");
                string start_of_paragraph = Json.ReadTag(s, "start_of_paragraph");
                string start_of_statement = Json.ReadTag(s, "start_of_statement");
                string isolate_mean = Json.ReadTag(s, "isolate_mean");
                string end_of_statement = Json.ReadTag(s, "end_of_statement");
                string end_of_paragraph = Json.ReadTag(s, "end_of_paragraph");
                string text = Json.ReadTag(s, "text");
                if (start != null && end != null && text != null)
                {
                    double startSeconds;
                    double endSeconds;
                    if (double.TryParse(start, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out startSeconds) &&
                        double.TryParse(end, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out endSeconds))
                    {
                        subtitle.Paragraphs.Add(new Paragraph(Json.DecodeJsonText(text), startSeconds, endSeconds,Convert.ToBoolean(start_of_paragraph), Convert.ToBoolean(start_of_statement)
                            , Convert.ToBoolean(isolate_mean), Convert.ToBoolean(end_of_statement), Convert.ToBoolean(end_of_paragraph)));
                    }
                    else
                    {
                        _errorCount++;
                    }
                }
                else
                {
                    _errorCount++;
                }
            }
            subtitle.Renumber();
        }

    }
}
