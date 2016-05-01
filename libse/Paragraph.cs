using System;

namespace Nikse.SubtitleEdit.Core
{
    public class Paragraph
    {
        public int Number { get; set; }

        public string Text { get; set; }

        public TimeCode StartTime { get; set; }

        public TimeCode EndTime { get; set; }

        public TimeCode Duration
        {
            get
            {
                return new TimeCode(EndTime.TotalMilliseconds - StartTime.TotalMilliseconds);
            }
        }

        public int StartFrame { get; set; }

        public int EndFrame { get; set; }

        public bool Forced { get; set; }

        public string Extra { get; set; }

        public bool IsComment { get; set; }

        public string Actor { get; set; }

        public string MarginL { get; set; }
        public string MarginR { get; set; }
        public string MarginV { get; set; }

        public string Effect { get; set; }

        public int Layer { get; set; }

        public string ID { get; private set; }

        public string Language { get; set; }

        public string Style { get; set; }

        public bool NewSection { get; set; }

        private string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        //mlk changes
        public bool IsolateMean { get; set; }
        public bool IsNewSection { get; set; }
        public bool CanCloseLasts { get; set; }
        //mlk changes
        public Paragraph()
        {
            //mlk changes
            IsolateMean = false;
            IsNewSection = false;
            CanCloseLasts = false;
            //mlk changes

            StartTime = TimeCode.FromSeconds(0);
            EndTime = TimeCode.FromSeconds(0);
            Text = string.Empty;
            ID = GenerateId();
        }

        public Paragraph(TimeCode startTime, TimeCode endTime, string text,bool isolateMean=false,bool isNewSection=false,bool canCloseLasts=false)
        {
            StartTime = startTime;
            EndTime = endTime;
            Text = text;

            //mlkchanges
            IsolateMean = isolateMean;
            IsNewSection = isNewSection;
            CanCloseLasts = canCloseLasts;
            //mlk changes

            ID = GenerateId();
        }

        public Paragraph(Paragraph paragraph, bool generateNewId = true)
        {
            Number = paragraph.Number;
            Text = paragraph.Text;
            StartTime = new TimeCode(paragraph.StartTime.TotalMilliseconds);
            EndTime = new TimeCode(paragraph.EndTime.TotalMilliseconds);
            StartFrame = paragraph.StartFrame;
            EndFrame = paragraph.EndFrame;
            Forced = paragraph.Forced;
            Extra = paragraph.Extra;
            IsComment = paragraph.IsComment;
            Actor = paragraph.Actor;
            MarginL = paragraph.MarginL;
            MarginR = paragraph.MarginR;
            MarginV = paragraph.MarginV;
            Effect = paragraph.Effect;
            Layer = paragraph.Layer;
            ID = generateNewId ? GenerateId() : paragraph.ID;
            Language = paragraph.Language;
            Style = paragraph.Style;
            NewSection = paragraph.NewSection;

            //mlkchanges
            IsolateMean = paragraph.IsolateMean;
            IsNewSection = paragraph.IsNewSection;
            CanCloseLasts = paragraph.CanCloseLasts;
            //mlk changes
        }

        public Paragraph(int startFrame, int endFrame, string text, bool isolateMean=false, bool isNewSection=false, bool canCloseLasts=false)
        {
            StartTime = new TimeCode(0, 0, 0, 0);
            EndTime = new TimeCode(0, 0, 0, 0);
            StartFrame = startFrame;
            EndFrame = endFrame;
            Text = text;

            //mlkchanges
            IsolateMean = isolateMean;
            IsNewSection = isNewSection;
            CanCloseLasts = canCloseLasts;
            //mlk changes

            ID = GenerateId();
        }

        public Paragraph(string text, double startTotalMilliseconds, double endTotalMilliseconds, bool isolateMean=false, bool isNewSection=false, bool canCloseLasts=false)
        {
            StartTime = new TimeCode(startTotalMilliseconds);
            EndTime = new TimeCode(endTotalMilliseconds);
            Text = text;

            //mlkchanges
            IsolateMean = isolateMean;
            IsNewSection = isNewSection;
            CanCloseLasts = canCloseLasts;
            //mlk changes

            ID = GenerateId();
        }

        public void Adjust(double factor, double adjustmentInSeconds)
        {
            if (StartTime.IsMaxTime)
                return;

            StartTime.TotalMilliseconds = StartTime.TotalMilliseconds * factor + (adjustmentInSeconds * TimeCode.BaseUnit);
            EndTime.TotalMilliseconds = EndTime.TotalMilliseconds * factor + (adjustmentInSeconds * TimeCode.BaseUnit);
        }

        public void CalculateFrameNumbersFromTimeCodes(double frameRate)
        {
            StartFrame = (int)Math.Round((StartTime.TotalMilliseconds / TimeCode.BaseUnit * frameRate));
            EndFrame = (int)Math.Round((EndTime.TotalMilliseconds / TimeCode.BaseUnit * frameRate));
        }

        public void CalculateTimeCodesFromFrameNumbers(double frameRate)
        {
            StartTime.TotalMilliseconds = StartFrame * (TimeCode.BaseUnit / frameRate);
            EndTime.TotalMilliseconds = EndFrame * (TimeCode.BaseUnit / frameRate);
        }

        public override string ToString()
        {
            return StartTime + " --> " + EndTime + " " + Text;
        }

        public int NumberOfLines
        {
            get
            {
                return Utilities.GetNumberOfLines(Text);
            }
        }

        public double WordsPerMinute
        {
            get
            {
                if (string.IsNullOrEmpty(Text))
                    return 0;
                int wordCount = HtmlUtil.RemoveHtmlTags(Text, true).Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '(', ')', '[', ']', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                return (60.0 / Duration.TotalSeconds) * wordCount;
            }
        }
    }
}
