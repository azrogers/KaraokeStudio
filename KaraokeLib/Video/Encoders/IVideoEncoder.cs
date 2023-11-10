using KaraokeLib.Config;
using NAudio.Wave;
using SkiaSharp;

namespace KaraokeLib.Video.Encoders
{
	public interface IVideoEncoder
	{
		/// <summary>
		/// The type of this encoder.
		/// </summary>
		VideoEncoderType EncoderType { get; }

		/// <summary>
		/// Sets the current exporter using this encoder.
		/// </summary>
		void SetExporterObject(VideoExporter exporter);

		/// <summary>
		/// Returns the current config object for this encoder.
		/// </summary>
		IEditableConfig GetConfigObject();

		/// <summary>
		/// Validates the given config, returning an array of error messages or none if valid.
		/// </summary>
		string[] ValidateConfigObject(IEditableConfig config);

		/// <summary>
		/// Sets the current config object for this encoder.
		/// </summary>
		void SetConfigObject(IEditableConfig configObject);

		/// <summary>
		/// Returns the supported output extensions for the current config object.
		/// Each extension includes the "."
		/// </summary>
		(string Extension, string Title)[] GetOutputExtensions();

		/// <summary>
		/// If true, this encoder outputs multiple files and should take a directory as input.
		/// If false, it outputs a single file and takes that file path as an input.
		/// </summary>
		bool DoesEncodeMultipleFiles { get; }

		/// <summary>
		/// Begins the render.
		/// </summary>
		void StartRender(string outFile, WaveStream audio, int width, int height, int frameRate, double length);

		/// <summary>
		/// Renders a single frame of video.
		/// </summary>
		void RenderFrame(VideoTimecode timecode, SKBitmap frameBitmap);

		/// <summary>
		/// Ends a render started with <see cref="StartRender(string, WaveStream, int, int, int, double)"/>.
		/// </summary>
		void EndRender(bool success);
	}

	public enum VideoEncoderType
	{
		Ffmpeg,
		ImageStrip
	}
}
