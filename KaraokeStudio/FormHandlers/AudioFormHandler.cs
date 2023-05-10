using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.FormHandlers
{
	internal class AudioFormHandler
	{
		private WaveStream? _waveStream;
		private WaveOutEvent _output;

		public AudioFormHandler()
		{
			_output = new WaveOutEvent();
		}

		public void OnProjectChanged(KaraokeProject? project)
		{
			if(_waveStream != null && _waveStream != project?.AudioStream)
			{
				_output.Stop();
				_waveStream.Dispose();
			}

			_waveStream = project?.AudioStream;
			if(_waveStream != null)
			{
				_output.Init(_waveStream);
			}
		}

		public void OnPlaybackStateChanged(bool isPlaying, double position)
		{
			if(_waveStream != null)
			{
				_waveStream.CurrentTime = TimeSpan.FromSeconds(position);
				if(isPlaying && _output.PlaybackState != PlaybackState.Playing)
				{
					_output.Play();
				}
				else if(!isPlaying && _output.PlaybackState == PlaybackState.Playing)
				{
					_output.Pause();
				}
			}
		}
	}
}
