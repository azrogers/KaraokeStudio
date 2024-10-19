using KaraokeLib.Events;
using KaraokeStudio.Util;
using SkiaSharp;

namespace KaraokeStudio.Timeline.EventRenderers
{
	internal class ImageEventRenderer : ICustomEventRenderer
	{
		private const int IMAGE_THUMBNAIL_SIZE = 128;
		private const float PADDING = 2.0f;

		private Dictionary<int, SKBitmap> _bitmaps = [];
		private SKPaint _bitmapPaint = new SKPaint()
		{
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
			var imageEvent = (ImageKaraokeEvent)ev;

			if (!_bitmaps.ContainsKey(ev.Id))
			{
				_bitmaps[ev.Id] = SKBitmap.Decode(FileCache.Get(new ImageThumbnailCacheRequest(imageEvent.Settings?.File ?? "")));
			}
			canvas.Save();
			canvas.ClipRect(rect);

			var scale = _bitmaps[ev.Id].Height / (rect.Height - PADDING * 2);

			canvas.Translate(rect.Left + PADDING, rect.Top + PADDING);
			canvas.Scale(1.0f / scale, 1.0f / scale);
			canvas.DrawBitmap(_bitmaps[ev.Id], new SKPoint(0, 0), _bitmapPaint);

			canvas.Restore();
		}

		internal class ImageThumbnailCacheRequest : ICacheRequest
		{
			public string Key => _imageFile;

			public string Category => "thumbnails";

			private string _imageFile;

			public ImageThumbnailCacheRequest(string imageFile)
			{
				_imageFile = imageFile;
			}

			public void Create(Stream output)
			{
				using (var bitmap = SKBitmap.Decode(_imageFile))
				{
					if (bitmap == null)
					{
						return;
					}

					var thumbSize = new SKSizeI(IMAGE_THUMBNAIL_SIZE, IMAGE_THUMBNAIL_SIZE);

					if (bitmap.Width > bitmap.Height)
					{
						thumbSize.Height = (int)((float)bitmap.Height / bitmap.Width * IMAGE_THUMBNAIL_SIZE);
					}
					else
					{
						thumbSize.Width = (int)((float)bitmap.Width / bitmap.Height * IMAGE_THUMBNAIL_SIZE);
					}

					using (var resized = bitmap.Resize(thumbSize, SKFilterQuality.High))
					using (var data = resized.Encode(SKEncodedImageFormat.Png, 100))
					{
						data.SaveTo(output);
					}
				}
			}
		}
	}
}
