﻿using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Files;
using KaraokeLib.Tracks;
using KaraokeStudio.Video;
using SkiaSharp;

namespace KaraokeStudio.Config
{
	internal class ConfigPreviewHandler : IDisposable
	{
		private const string TEXT = @"Lo-rem ip-sum do-lor sit am-et, con-sect-te-tur ad-ip-is-cing el-it.
Vest-ib-u-lum vi-tae ar-cu vel el-it sus-ci-pit ul-tri-cies ac quis do-lor. In-te-ger a con-gue la-cus.
Nul-la so-da-les el-e-men-tum est ut el-ei-fend. Du-is tris-tique pur-us vel li-ber-o da-pi-bus, in con-val-lis dui dic-tum.
Ves-tib-u-lum an-te ip-sum pri-mis in fau-ci-bus or-ci luc-tus et ul-tri-ces pos-u-ere cu-bil-ia cu-rae; Du-is sed est quis fe-lis 
hen-dre-rit con-gue. Quis-que sol-lic-it-u-din acc-um-san tor-tor, ac max-i-mus quam sem-per dig-nis-sim. Quis-que quis sap-ien sem-per,
vi-ver-ra el-it ia-cu-lis tem-pus e-ros. In vol-ut-pat, el-it a ul-tri-ces con-val-lis, ne-que ip-sum im-per-diet quam, in var-i-us sap-ien
mas-sa quis sem. Cur-a-bit-ur cur-sus or-ci vit-ae con-seq-uat hen-dre-rit. Sus-pen-disse lao-reet mal-es-ua-da quam.";

		private VideoGenerationState _generationState;
		private KaraokeEvent[] _events;
		private KaraokeTrack[] _tracks;
		private DummyKaraokeFile _file;
		private int _frameRate;

		public ConfigPreviewHandler()
		{
			_generationState = new VideoGenerationState();
			_file = new DummyKaraokeFile();
			_events = CreateEventsFromText(TEXT).ToArray();
			var track = _file.AddTrack(KaraokeTrackType.Lyrics);
			track.AddEvents(_events);
			_tracks = _file.GetTracks().ToArray();
		}

		public void Dispose()
		{
			_generationState.Dispose();
		}

		public void Render(SKSurface surface)
		{
			// render half-way through our preview text
			_generationState.Render(_tracks, new KaraokeLib.Video.VideoTimecode(_events.Last().EndTimeSeconds / 2.0, _frameRate), surface);
		}

		public void UpdatePreview(KaraokeConfig config, (int Width, int Height) size)
		{
			_frameRate = config.FrameRate;
			_generationState.UpdateVideoContext(_events.Last().EndTimeSeconds, config, size);
		}

		private IEnumerable<KaraokeEvent> CreateEventsFromText(string text)
		{
			var words = text.Split(' ');
			var id = 0;
			var time = 0.0f;
			foreach (var word in words)
			{
				var parts = word.Split('-');
				var firstId = id;
				foreach (var part in parts)
				{
					var ev = new KaraokeEvent(
						KaraokeEventType.Lyric,
						id,
						new TimeSpanTimecode(TimeSpan.FromSeconds(time)),
						new TimeSpanTimecode(TimeSpan.FromSeconds(time + 0.25f)),
						(firstId != id ? firstId : -1));
					id++;
					time += 0.5f;
					ev.RawValue = part.Trim();
					yield return ev;
				}
			}
		}
	}
}
