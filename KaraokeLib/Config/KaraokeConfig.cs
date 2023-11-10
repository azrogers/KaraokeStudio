using KaraokeLib.Config.Attributes;
using KaraokeLib.Util;
using KaraokeLib.Video.Transitions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Config
{
    public class KaraokeConfig : EditableConfig<KaraokeConfig>
    {
        public static KaraokeConfig Default = new KaraokeConfig();

        [ConfigRange(1)]
        public int FrameRate = 30;
        public KSize VideoSize = new(1920, 1080);

        public KColor NormalColor = new KColor(255, 255, 255);
        public KColor HighlightColor = new KColor(70, 175, 90);
        public KColor StrokeColor = new KColor(0, 0, 0);
        // TODO: replace with Graphics KaraokeTrack background element
        public KColor BackgroundColor = new KColor(230, 230, 230);

        [ConfigRange(0.0)]
        public float StrokeWidth = 3;

        public KFont Font = new KFont()
        {
            Size = 200,
            Family = "Arial",
            Weight = SKFontStyleWeight.Bold,
            Slant = SKFontStyleSlant.Upright,
            Width = SKFontStyleWidth.Normal
        };

        public KPadding Padding = new KPadding(30.0f, 60.0f, 30.0f, 60.0f);

        [ConfigRange(0.0)]
        public double LyricLeadTime = 2.0;
        [ConfigRange(0.0)]
        public double LyricTrailTime = 1.0;

        // TODO: should be moved to a per-event and per-track config
        public VideoTransitionType TransitionIn = VideoTransitionType.Fade;
        public EasingType TransitionInCurve = EasingType.InQuad;
        public VideoTransitionType TransitionOut = VideoTransitionType.Fade;
        public EasingType TransitionOutCurve = EasingType.OutQuad;

        [ConfigRange(0.0)]
        public double MaxTransitionLength = 1.0;
        [ConfigRange(0.0)]
        public double MinTransitionLength = 0.1;

        // TODO: should be moved to a separate "import settings" class with a separate window for MIDI import settings
        [ConfigRange(0.0)]
        public double MinTimeBetweenSections = 10.0;

        public HorizontalAlignment HorizontalAlign = HorizontalAlignment.Center;
        public VerticalAlignment VerticalAlign = VerticalAlignment.Center;

        public KaraokeConfig(string configStr) : base(configStr) { }
        public KaraokeConfig() { }
    }

    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center
    }

    public enum VerticalAlignment
    {
        Top,
        Bottom,
        Center
    }
}
