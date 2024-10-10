using CSCore.CoreAudioAPI;
using KaraokeLib.Config;
using KaraokeLib.Config.Attributes;
using Newtonsoft.Json;

namespace KaraokeLib.Audio
{
	public class AudioSettings : EditableConfig<AudioSettings>
	{
		[JsonProperty]
		public AudioOutputType OutputType = AudioOutputType.Wasapi;

		[ConfigDropdown("GetAudioDevices")]
		[JsonProperty]
		public string? AudioDevice = null;

		[ConfigRange(1)]
		[JsonProperty]
		[ConfigDisplay(FriendlyName = "Latency (ms)")]
		public int Latency = 100;

		public static IEnumerable<(string, object)> GetAudioDevices()
		{
			return MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active).Select(device => (device.FriendlyName, device.DeviceID as object));
		}

		public enum AudioOutputType
		{
			DirectSound,
			Wasapi,
			WaveOut
		}
	}
}
