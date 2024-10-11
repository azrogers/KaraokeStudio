using KaraokeLib.Audio;
using KaraokeLib.Events;
using KaraokeStudio.Util;
using NAudio.WaveFormRenderer;
using SkiaSharp;

namespace KaraokeStudio.Timeline.EventRenderers
{
	internal class AudioClipEventRenderer : ICustomEventRenderer
	{
		private const float BITMAP_X_SCALE = 4.0f;
		private const float BITMAP_Y_SCALE = 2.0f;

		private static readonly SKShader _waveformShader = SKShader.CreateColor(new SKColor(50, 50, 50));

		private Dictionary<int, SKBitmap> _bitmaps = new Dictionary<int, SKBitmap>();
		private WaveFormRenderer _renderer = new WaveFormRenderer();
		private SKPaint _bitmapPaint = new SKPaint()
		{
			BlendMode = SKBlendMode.Darken,
			Color = new SKColor(255, 255, 255, 127)
		};

		public void RecreateContext()
		{
			foreach (var image in _bitmaps)
			{
				image.Value.Dispose();
			}
			_bitmaps.Clear();
		}

		public void Render(SKCanvas canvas, SKRect rect, KaraokeEvent ev)
		{
			var clipEvent = (AudioClipKaraokeEvent)ev;

			if (!_bitmaps.ContainsKey(ev.Id))
			{
				var bitmapPath = FileCache.GetTempFilePath(Utility.Sha256Hash(clipEvent.Settings?.AudioFile ?? "") + ".png");

				if(File.Exists(bitmapPath))
				{
					_bitmaps[ev.Id] = SKBitmap.Decode(bitmapPath);
				}
				else
				{
					var duration = AudioUtil.GetFileInfo(clipEvent.Settings?.AudioFile ?? "")?.LengthSeconds ?? 0;
					var widthPixels = TimelineCanvas.PIXELS_PER_SECOND * duration;

					var settings = new StandardWaveFormRendererSettings();
					settings.Width = (int)(widthPixels * BITMAP_X_SCALE);
					settings.TopHeight = (int)(rect.Height / 2 * BITMAP_Y_SCALE);
					settings.BottomHeight = (int)(rect.Height / 2 * BITMAP_Y_SCALE);
					settings.DecibelScale = true;
					settings.BackgroundColor = SKColor.Empty;
					settings.TopPeakShader = _waveformShader;
					settings.BottomPeakShader = _waveformShader;

					var peakProvider = new RmsPeakProvider(2048);
					using (var stream = clipEvent.Settings?.LoadAudioFile())
					{
						if (stream != null)
						{
							_bitmaps[ev.Id] = _renderer.Render(stream, peakProvider, settings);
							using (var data = _bitmaps[ev.Id].Encode(SKEncodedImageFormat.Png, 100))
							using (var output = File.OpenWrite(bitmapPath))
							{
								data.SaveTo(output);
							}
							
						}
					}
				}
			}

			var offset = -(clipEvent.Settings?.Offset ?? 0) * TimelineCanvas.PIXELS_PER_SECOND;
			canvas.Save();
			canvas.ClipRect(rect);

			canvas.Translate(rect.Left + (float)offset, rect.Top);
			canvas.Scale(1.0f / BITMAP_X_SCALE, 1.0f / BITMAP_Y_SCALE);
			canvas.DrawBitmap(_bitmaps[ev.Id], new SKPoint(0, 0), _bitmapPaint);
			canvas.Restore();
		}
	}
}
