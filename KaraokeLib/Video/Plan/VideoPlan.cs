using KaraokeLib.Video.Elements;

namespace KaraokeLib.Video.Plan
{
	/// <summary>
	///	Records when each element starts and stops being displayed for an entire video.
	/// </summary>
	public class VideoPlan
	{
		private Dictionary<IVideoElement, (uint StartFrame, uint EndFrame)> _videoElementLookup = new Dictionary<IVideoElement, (uint, uint)>();

		// faster structure for per-frame generation, created in FinalizePlan
		private List<(uint StartFrame, uint EndFrame, IVideoElement Element)> _videoElements = new List<(uint StartFrame, uint EndFrame, IVideoElement Element)>();

		private bool _finalized = false;

		/// <summary>
		/// Records the given element as visible on the specified timecode.
		/// </summary>
		internal void RecordElementFrame(IVideoElement element, VideoTimecode videoTimecode)
		{
			_finalized = false;

			if (_videoElementLookup.TryGetValue(element, out var frameBounds))
			{
				frameBounds.StartFrame = Math.Min(videoTimecode.FrameNumber, frameBounds.StartFrame);
				frameBounds.EndFrame = Math.Max(videoTimecode.FrameNumber, frameBounds.EndFrame);
				_videoElementLookup[element] = frameBounds;
			}
			else
			{
				_videoElementLookup[element] = (videoTimecode.FrameNumber, videoTimecode.FrameNumber);
			}
		}

		/// <summary>
		/// Finalizes this video plan, readying it for use in rendering a video.
		/// </summary>
		internal void FinalizePlan()
		{
			if (_finalized)
			{
				// nothing to do
				return;
			}

			_videoElements.Clear();
			foreach (var kv in _videoElementLookup)
			{
				_videoElements.Add((kv.Value.StartFrame, kv.Value.EndFrame, kv.Key));
			}

			_videoElements = _videoElements.OrderBy(v => v.StartFrame).ToList();

			_finalized = true;
		}

		/// <summary>
		/// Returns all elements relevant to the given frame.
		/// </summary>
		internal IEnumerable<(VideoTimecode StartTime, VideoTimecode EndTime, IVideoElement Element)> GetElementsForFrame(VideoTimecode frame)
		{
			if (!_finalized)
			{
				FinalizePlan();
			}

			foreach (var elem in _videoElements)
			{
				// skip this element, we're not there yet
				if (elem.EndFrame < frame.FrameNumber)
				{
					continue;
				}

				// this is after the element we need, we're done with all the elements we should consider
				if (elem.StartFrame > frame.FrameNumber)
				{
					break;
				}

				yield return (new VideoTimecode(elem.StartFrame, frame.FrameRate), new VideoTimecode(elem.EndFrame, frame.FrameRate), elem.Element);
			}
		}
	}
}
