namespace KaraokeLib.Video.Transitions
{
	internal static class TransitionManager
	{
		private static readonly Dictionary<VideoTransitionType, IVideoTransition> _transitions = new Dictionary<VideoTransitionType, IVideoTransition>()
		{
			{ VideoTransitionType.None, new NoneVideoTransition() },
			{ VideoTransitionType.Fade, new FadeVideoTransition() },
			{ VideoTransitionType.Swipe, new SwipeVideoTransition() }
		};

		internal static IVideoTransition Get(VideoTransitionType type)
		{
			return _transitions[type];
		}
	}
}
