using FFMediaToolkit.Audio;
using FFMediaToolkit.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMediaToolkit.Encoding;

namespace KaraokeLib.Audio
{
	internal class AudioFile
	{
		private MediaFile _file;
		private float[][] _samples;
		private int _numSamples;
		private AudioStreamInfo _streamInfo;
		private string _fileName;

		public string FileName => _fileName;

		public AudioFile(string filename)
		{
			_file = MediaFile.Open(filename);
			_fileName = filename;

			if(_file.AudioStreams.Length == 0)
			{
				throw new InvalidDataException("Audio file must have at least one stream");
			}

			_streamInfo = _file.AudioStreams[0].Info;

			// copy all audio data
			/*var stream = _file.AudioStreams.First();
			_samples = new float[stream.Info.NumChannels][];
			var numSamples = (int)Math.Ceiling(stream.Info.Duration.TotalSeconds * stream.Info.SampleRate);
			var samplesIdx = new int[stream.Info.NumChannels];
			for (var i = 0; i < stream.Info.NumChannels; i++)
			{
				_samples[i] = new float[numSamples];
				samplesIdx[i] = 0;
			}

			while(stream.TryGetNextFrame(out var data))
			{
				for(var i = 0; i < stream.Info.NumChannels; i++)
				{
					var channelData = data.GetChannelData((uint)i).ToArray();
					Array.CopyTyped(channelData, 0, _samples[i], samplesIdx[i], channelData.Length);
					samplesIdx[i] += channelData.Length;
				}
			}

			_numSamples = samplesIdx[0];
			for(var i = 0; i < stream.Info.NumChannels; i++)
			{
				if (samplesIdx[i] != _numSamples)
				{
					throw new InvalidDataException("Mismatched sample count for different channels?");
				}
			}*/
		}

		public AudioEncoderSettings CreateEncodingSettings(AudioCodec codec)
		{
			var numSamplesPerFrame = 0;
			// check a few frames for sample info
			var i = 0;
			do
			{
				if (_file.Audio.TryGetNextFrame(out var data))
				{
					numSamplesPerFrame = Math.Max(data.NumSamples, numSamplesPerFrame);
				}
			} while (i++ < 3);

			return new AudioEncoderSettings(_streamInfo.SampleRate, _streamInfo.NumChannels, codec)
			{
				SampleFormat = _streamInfo.SampleFormat,
				SamplesPerFrame = numSamplesPerFrame
			};
		}

		public double GetLengthSeconds()
		{
			return _file.Info.Duration.TotalSeconds;
		}

		public AudioFileSegment GetAudioData(double startPos, double endPos)
		{
			return new AudioFileSegment(_file, startPos, endPos - startPos);

			/*var streamInfo = _file.AudioStreams[0].Info;

			var length = endPos - startPos;
			var startPosSamples = (int)Math.Round(startPos * streamInfo.SampleRate);
			var numSamples =
				Math.Min(
					(int)Math.Ceiling(length * streamInfo.SampleRate),
					_numSamples - startPosSamples);

			var samples = new float[streamInfo.NumChannels * numSamples];
			// interlace channels
			for(var i = 0; i < numSamples; i++)
			{
				for (var j = 0; j < streamInfo.NumChannels; j++)
				{
					samples[i * streamInfo.NumChannels + j] = _samples[j][i];
				}
			}

			// there must be a better way of getting a segment of an audio file than encoding it to wav
			// and then opening it again...
			var format = WaveFormat.CreateIeeeFloatWaveFormat(streamInfo.SampleRate, streamInfo.NumChannels);
			using (var memoryStream = new MemoryStream())
			{
				using (var wavWriter = new WaveFileWriter(memoryStream, format))
				{
					wavWriter.WriteSamples(samples, 0, samples.Length);
				}

				memoryStream.Position = 0;
				var newFile = MediaFile.Open(memoryStream);
				while(newFile.Audio.TryGetNextFrame)
			}*/
		}
	}

	internal class AudioFileSegment
	{
		private MediaFile _file;
		private AudioStreamInfo _streamInfo;
		private double _startPos;
		private double _length;
		private double _position;

		public AudioFileSegment(MediaFile file, double startPos, double length)
		{
			_file = file;
			_streamInfo = _file.AudioStreams[0].Info;
			_startPos = startPos;
			_length = length;
			_position = 0;
		}

		public bool TryGetNextFrame(out AudioData nextFrame)
		{
			return _file.Audio.TryGetNextFrame(out nextFrame);
			if(_position - _startPos > _length)
			{
				nextFrame = new AudioData();
				return false;
			}

			do
			{
				if (_file.Audio.TryGetNextFrame(out nextFrame))
				{
					_position += nextFrame.NumSamples / (double)_streamInfo.SampleRate;
					if(_position >= _startPos)
					{
						return true;
					}
				}
				else
				{
					return false;
				}
			}
			while (_position < _startPos);

			return false;
		}
	}
}
