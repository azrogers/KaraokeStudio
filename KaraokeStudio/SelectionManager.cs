using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio
{
	/// <summary>
	/// Handles the current selection in the application.
	/// </summary>
	public static class SelectionManager
	{
		private static List<KaraokeTrack> _selectedTracks = new List<KaraokeTrack>();
		private static List<KaraokeEvent> _selectedEvents = new List<KaraokeEvent>();

		/// <summary>
		/// The tracks that are currently selected, if any.
		/// </summary>
		public static IEnumerable<KaraokeTrack> SelectedTracks => _selectedTracks;

		/// <summary>
		/// The events that are currently selected, if any.
		/// </summary>
		public static IEnumerable<KaraokeEvent> SelectedEvents => _selectedEvents;

		/// <summary>
		/// Called when the selected tracks have changed.
		/// </summary>
		public static event Action? OnSelectedTracksChanged;

		/// <summary>
		/// Called when the selected events have changed.
		/// </summary>
		public static event Action? OnSelectedEventsChanged;

		static SelectionManager()
		{
			UpdateDispatcher.RegisterHandler<EventsUpdate>(update =>
			{
				if(update.Type == EventsUpdate.UpdateType.Replace)
				{
					var idsLookup = new HashSet<int>(update.EventIds);
					_selectedEvents.RemoveAll(ev => !idsLookup.Contains(ev.Id));
				}
			});

			UpdateDispatcher.RegisterHandler<TracksUpdate>(update =>
			{
				foreach(var id in update.TrackIds)
				{
					_selectedTracks.RemoveAll(t => t.Id == id);
				}

				OnSelectedTracksChanged?.Invoke();
			});
		}

		public static void Deselect()
		{
			var hadEvents = _selectedEvents.Any();
			var hadTracks = _selectedTracks.Any();

			_selectedEvents.Clear();
			_selectedTracks.Clear();

			if(hadEvents)
			{
				OnSelectedEventsChanged?.Invoke();
			}
			if(hadTracks)
			{
				OnSelectedTracksChanged?.Invoke();
			}
		}

		public static void Select(KaraokeTrack track, bool replace)
		{
			DeselectEvents();
			if(replace)
			{
				DeselectTracks();
			}

			_selectedTracks.Add(track);
			OnSelectedTracksChanged?.Invoke();
		}

		public static void Select(KaraokeEvent ev, bool replace)
		{
			DeselectTracks();
			if(replace)
			{
				DeselectEvents();
			}

			_selectedEvents.Add(ev);
			OnSelectedEventsChanged?.Invoke();
		}

		private static void DeselectTracks()
		{
			if (_selectedTracks.Any())
			{
				_selectedTracks.Clear();
				OnSelectedTracksChanged?.Invoke();
			}
		}

		private static void DeselectEvents()
		{
			if(_selectedEvents.Any())
			{
				_selectedEvents.Clear();
				OnSelectedEventsChanged?.Invoke();
			}
		}
	}
}
