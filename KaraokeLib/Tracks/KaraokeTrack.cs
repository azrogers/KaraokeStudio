﻿using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Files;
using KaraokeLib.Files.Ksf;
using Newtonsoft.Json;

namespace KaraokeLib.Tracks
{
	/// <summary>
	/// A single track within a KaraokeFile. 
	/// A track can be more than just text - see <see cref="KaraokeTrackType"/>.
	/// </summary>
	[KsfSerializable(KsfObjectType.Track)]
	public class KaraokeTrack : IKsfBinaryObject
	{
		private IEditableConfig? _trackConfig;

		[KsfSerialize]
		private List<KaraokeEvent> _events;

		/// <summary>
		/// The events on this track.
		/// </summary>
		public List<KaraokeEvent> Events => _events;

		/// <summary>
		/// The type of this track.
		/// </summary>
		public KaraokeTrackType Type { get; private set; }

		/// <summary>
		/// The id used to uniquely identify this track.
		/// </summary>
		public int Id { get; internal set; }

		/// <summary>
		/// The order of this track for positioning alongside others.
		/// </summary>
		public int Order { get; set; }

		private IKaraokeFile? _karaokeFile;

		// used for serialization
		private KaraokeTrack()
			: this(0, KaraokeTrackType.Lyrics)
		{
		}

		internal KaraokeTrack(int id, KaraokeTrackType type)
		{
			_events = new List<KaraokeEvent>();
			Type = type;
			Id = id;
			Order = id;
		}

		internal KaraokeTrack(int id, KaraokeTrackType type, IEnumerable<KaraokeEvent> events)
		{
			_events = events.ToList();
			Type = type;
			Id = id;
			Order = id;
		}

		public KaraokeTrack(IKaraokeFile file, KaraokeTrackType type)
		{
			_karaokeFile = file;
			_events = new List<KaraokeEvent>();
			Type = type;
			Id = _karaokeFile.IdTracker.AddNewTrack(this);
			Order = _karaokeFile.GetTracks().Any() ? _karaokeFile.GetTracks().Max(t => t.Order) + 1 : 1;
		}

		public KaraokeTrack(IKaraokeFile file, KaraokeTrackType type, IEnumerable<KaraokeEvent> events)
		{
			_karaokeFile = file;
			_events = new List<KaraokeEvent>(events);
			Type = type;
			Id = _karaokeFile.IdTracker.AddNewTrack(this);
			Order = _karaokeFile.GetTracks().Max(t => t.Order) + 1;
		}

		/// <summary>
		/// Adds a new event to this track and returns the event.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public KaraokeEvent AddEvent(KaraokeEventType type, IEventTimecode start, IEventTimecode end, int linkedId = -1)
		{
			var ev = CreateEvent(type, start, end, linkedId);
			_events.Add(ev);
			return ev;
		}

		/// <summary>
		/// Adds a new event to this track of type KaraokeEventType.AudioClip.
		/// </summary>
		public AudioClipKaraokeEvent AddAudioClipEvent(AudioClipSettings settings, IEventTimecode start, IEventTimecode end)
		{
			var ev = new AudioClipKaraokeEvent(settings, CreateEvent(KaraokeEventType.AudioClip, start, end));
			_events.Add(ev);
			return ev;
		}

		/// <summary>
		/// Adds a new event to this track of type KaraokeEventType.Image.
		/// </summary>
		public ImageKaraokeEvent AddImageEvent(ImageSettings settings, IEventTimecode start, IEventTimecode end)
		{
			var ev = new ImageKaraokeEvent(settings, CreateEvent(KaraokeEventType.Image, start, end));
			_events.Add(ev);
			return ev;
		}

		/// <summary>
		/// Adds the given events to this track.
		/// </summary>
		/// <remarks>
		/// This will automatically conform events so they don't overlap, and check that their types match the track type they're on.
		/// </remarks>
		public void AddEvents(IEnumerable<KaraokeEvent> events)
		{
			if (_karaokeFile == null)
			{
				throw new InvalidOperationException("KaraokeTrack missing KaraokeFile");
			}

			_events.AddRange(events);
			ValidateEvents();
			ConformEvents();
			_karaokeFile.IdTracker.AddEvents(Id, events);
		}

		/// <summary>
		/// Removes the existing events on this track and adds the given events.
		/// </summary>
		public void ReplaceEvents(IEnumerable<KaraokeEvent> events)
		{
			if (_karaokeFile == null)
			{
				throw new InvalidOperationException("KaraokeTrack missing KaraokeFile");
			}

			_events.Clear();
			_karaokeFile.IdTracker.ReplaceTrack(Id, this);
			AddEvents(events);
		}

		public void UpdateEvents()
		{
			ConformEvents();
		}

		/// <summary>
		/// Returns events within the given bounds.
		/// </summary>
		public IEnumerable<KaraokeEvent> GetRelevantEvents((double Start, double End) bounds)
		{
			foreach (var ev in _events)
			{
				if (
					ev.StartTimeSeconds >= bounds.Start && ev.StartTimeSeconds < bounds.End ||
					ev.EndTimeSeconds >= bounds.Start && ev.EndTimeSeconds < bounds.End ||
					bounds.Start >= ev.StartTimeSeconds && bounds.Start < ev.EndTimeSeconds ||
					bounds.End >= ev.StartTimeSeconds && bounds.End < ev.EndTimeSeconds)
				{
					yield return ev;
				}
			}
		}

		public IEditableConfig GetTrackConfig()
		{
			if (_trackConfig == null)
			{
				_trackConfig = Activator.CreateInstance(KaraokeTrackTypeMapping.GetTrackSettingsType(Type)) as IEditableConfig;
			}

			if (_trackConfig == null)
			{
				throw new InvalidOperationException("KaraokeTrack _trackConfig is null?");
			}

			return _trackConfig;
		}

		public T GetTrackConfig<T>() where T : IEditableConfig
		{
			var config = GetTrackConfig();
			if (!typeof(T).IsAssignableFrom(config.GetType()))
			{
				throw new ArgumentException($"Track config is of type {config.GetType().Name} but type {typeof(T).Name} was requested");
			}

			return (T)config;
		}

		public void SetTrackConfig(IEditableConfig config)
		{
			if (!KaraokeTrackTypeMapping.GetTrackSettingsType(Type).IsAssignableFrom(config.GetType()))
			{
				throw new ArgumentException($"Can't assign track config of type {config.GetType().Name} to track of type {Type}");
			}

			_trackConfig = config;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(Id);
			writer.Write((byte)Type);
			writer.Write(Order);
			writer.WriteNullTerminatedString(JsonConvert.SerializeObject(GetTrackConfig()));
		}

		public object Read(BinaryReader reader, byte version)
		{
			Id = reader.ReadInt32();
			Type = (KaraokeTrackType)reader.ReadByte();
			if (version >= 1)
			{
				Order = reader.ReadInt32();
			}
			else
			{
				Order = Id;
			}

			var trackConfigType = KaraokeTrackTypeMapping.GetTrackSettingsType(Type);
			_trackConfig = (IEditableConfig?)JsonConvert.DeserializeObject(reader.ReadNullTerminatedString(), trackConfigType);
			return this;
		}

		internal void SetFile(IKaraokeFile file)
		{
			_karaokeFile = file;
		}

		private KaraokeEvent CreateEvent(KaraokeEventType type, IEventTimecode start, IEventTimecode end, int linkedId = -1)
		{
			if (_karaokeFile == null)
			{
				throw new InvalidOperationException("KaraokeTrack missing KaraokeFile");
			}

			return new KaraokeEvent(type, _karaokeFile.IdTracker, start, end, linkedId);
		}

		private void ConformEvents()
		{
			_events = _events.OrderBy(ev => ev.StartTimeMilliseconds).ToList();
			if (_events.Count < 2)
			{
				// nothing to do
				return;
			}

			// fix overlapping events
			for (var i = 1; i < _events.Count; i++)
			{
				var lastEvent = _events[i - 1];
				var ev = _events[i];
				if (lastEvent != null && lastEvent.EndTimeSeconds > ev.StartTimeSeconds)
				{
					// if they overlap but we can move the start time of ev without losing it entirely, let's do that
					if (lastEvent.EndTimeSeconds < ev.EndTimeSeconds)
					{
						ev.SetTiming(lastEvent.EndTime, ev.EndTime);
					}
					// if they overlap but we can move the end time of the last event without losing it entirely, let's do that
					else if (lastEvent.StartTimeSeconds < ev.StartTimeSeconds)
					{
						lastEvent.SetTiming(lastEvent.StartTime, ev.StartTime);
					}
					// otherwise, just move this event to the end of the last one
					else
					{
						ev.SetTiming(lastEvent.EndTime, new TimeSpanTimecode(lastEvent.EndTimeSeconds + ev.LengthSeconds));
					}
				}
			}
		}

		private void ValidateEvents()
		{
			foreach (var ev in _events)
			{
				if (!KaraokeTrackTypeMapping.IsEventValid(Type, ev.Type))
				{
					throw new InvalidDataException($"Can't have event of type {ev.Type} on track of type {Type}!");
				}
			}
		}
	}
}
