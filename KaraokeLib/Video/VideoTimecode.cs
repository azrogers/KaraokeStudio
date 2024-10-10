namespace KaraokeLib.Video
{
	public struct VideoTimecode
	{
		private int _frameRate;
		private int _frameCount;

		public uint FrameNumber => (uint)_frameCount;

		public int FrameRate => _frameRate;

		public VideoTimecode(double time, int framerate)
		{
			_frameRate = framerate;
			var framesPerSecond = 1.0 / framerate;
			_frameCount = (int)Math.Round(time / framesPerSecond);
		}

		public VideoTimecode(int frameCount, int frameRate)
		{
			_frameRate = frameRate;
			_frameCount = frameCount;
		}

		public double ToSeconds()
		{
			if (_frameRate == 0)
			{
				return 0;
			}

			return _frameCount / (double)_frameRate;
		}

		public static VideoTimecode operator +(VideoTimecode a, VideoTimecode b)
		{
			var targetFramerate = a._frameRate;
			if (targetFramerate != b._frameRate)
			{
				var newTimecode = new VideoTimecode(b.ToSeconds(), targetFramerate);
				newTimecode._frameCount += a._frameCount;
				return newTimecode;
			}

			return new VideoTimecode(a._frameCount + b._frameCount, targetFramerate);
		}

		public static VideoTimecode operator -(VideoTimecode a, VideoTimecode b)
		{
			var targetFramerate = a._frameRate;
			if (targetFramerate != b._frameRate)
			{
				var newTimecode = new VideoTimecode(b.ToSeconds(), targetFramerate);
				newTimecode._frameCount -= a._frameCount;
				return newTimecode;
			}

			return new VideoTimecode(a._frameCount - b._frameCount, targetFramerate);
		}

		public static VideoTimecode operator +(VideoTimecode a, int b)
		{
			return new VideoTimecode(a._frameCount + b, a._frameRate);
		}

		public static VideoTimecode operator -(VideoTimecode a, int b)
		{
			return new VideoTimecode(a._frameCount - b, a._frameRate);
		}

		public static VideoTimecode operator +(VideoTimecode a, double b)
		{
			return new VideoTimecode(a.ToSeconds() + b, a._frameRate);
		}

		public static VideoTimecode operator -(VideoTimecode a, double b)
		{
			return new VideoTimecode(a.ToSeconds() - b, a._frameRate);
		}

		public static VideoTimecode operator ++(VideoTimecode a)
		{
			return new VideoTimecode(a._frameCount + 1, a._frameRate);
		}

		public static VideoTimecode operator --(VideoTimecode a)
		{
			return new VideoTimecode(a._frameCount - 1, a._frameRate);
		}

		public static bool operator ==(VideoTimecode a, VideoTimecode b)
		{
			if (a._frameRate == b._frameRate)
			{
				return a._frameCount == b._frameCount;
			}

			return a.ToSeconds() == b.ToSeconds();
		}

		public static bool operator !=(VideoTimecode a, VideoTimecode b)
		{
			if (a._frameRate == b._frameRate)
			{
				return a._frameCount != b._frameCount;
			}

			return a.ToSeconds() == b.ToSeconds();
		}

		public static bool operator <(VideoTimecode a, int b)
		{
			return a._frameCount < b;
		}

		public static bool operator >(VideoTimecode a, int b)
		{
			return a._frameCount > b;
		}

		public static bool operator <(VideoTimecode a, VideoTimecode b)
		{
			if (a._frameRate == b._frameRate)
			{
				return a._frameCount < b._frameCount;
			}

			return a.ToSeconds() < b.ToSeconds();
		}

		public static bool operator <=(VideoTimecode a, VideoTimecode b)
		{
			return a == b || a < b;
		}

		public static bool operator >(VideoTimecode a, VideoTimecode b)
		{
			if (a._frameRate == b._frameRate)
			{
				return a._frameCount > b._frameCount;
			}

			return a.ToSeconds() > b.ToSeconds();
		}

		public static bool operator >=(VideoTimecode a, VideoTimecode b)
		{
			return a == b || a > b;
		}

		public static VideoTimecode Max(VideoTimecode a, VideoTimecode b) => a > b ? a : b;

		public static VideoTimecode Min(VideoTimecode a, VideoTimecode b) => a < b ? a : b;

		public static VideoTimecode Max(VideoTimecode a, double b) => a.ToSeconds() > b ? a : new VideoTimecode(b, a.FrameRate);

		public static VideoTimecode Min(VideoTimecode a, double b) => a.ToSeconds() < b ? a : new VideoTimecode(b, a.FrameRate);

		public static VideoTimecode Max(double a, VideoTimecode b) => Max(b, a);

		public static VideoTimecode Min(double a, VideoTimecode b) => Min(b, a);
	}
}
