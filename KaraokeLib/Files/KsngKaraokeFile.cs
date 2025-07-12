using KaraokeLib.Audio;
using KaraokeLib.Events;
using KaraokeLib.Tracks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace KaraokeLib.Files
{
    public class KsngKaraokeFile : KaraokeFile<KsngKaraokeProvider>
    {
        public KsngKaraokeFile(string filename) : base(new KsngKaraokeProvider(filename)) { }

        public KsngKaraokeFile(Stream stream) : base(new KsngKaraokeProvider(stream)) { }
        public KsngKaraokeFile(IKaraokeFile otherFile) : base(new KsngKaraokeProvider(otherFile.GetTracks())) { }
        public KsngKaraokeFile(IEnumerable<KaraokeTrack> tracks) : base(new KsngKaraokeProvider(tracks)) { }
    }

    public class KsngKaraokeProvider : IKaraokeProvider
    {
        const uint MAGIC_NUMBER = 0x474E534B;
        const ushort FILE_VERSION = 0;

        private List<KaraokeTrack> _tracks;

        /// <inheritdoc />
        public bool CanRead => false;

        /// <inheritdoc />
        public bool CanWrite => true;

        public KsngKaraokeProvider(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                _tracks = new List<KaraokeTrack>();
                Load(stream);
            }
        }

        public KsngKaraokeProvider(Stream stream)
        {
            _tracks = new List<KaraokeTrack>();
            Load(stream);
        }

        public KsngKaraokeProvider(IEnumerable<KaraokeTrack> tracks)
        {
            _tracks = tracks.ToList();
        }

        /// <inheritdoc />
        public double GetLengthSeconds()
        {
            return _tracks.Max(t => t.Events.Max(e => e.EndTimeSeconds));
        }

        /// <inheritdoc />
        public IEnumerable<KaraokeTrack> GetTracks()
        {
            return _tracks;
        }

        /// <inheritdoc />
        public KaraokeTrack AddTrack(IKaraokeFile file, KaraokeTrackType type) => throw new NotImplementedException("KsngLyricsProvider.AddTrack not implemented");

        /// <inheritdoc />
        public KaraokeTrack AddTrack(KaraokeTrack track) => throw new NotImplementedException("KsngLyricsProvider.AddTrack not implemented");

        /// <inheritdoc />
        public void RemoveTrack(int trackId) => throw new NotImplementedException("KsngLyricsProvider.RemoveTrack not implemented");

        /// <inheritdoc />
        public void Save(Stream outStream)
        {
            using (var writer = new BinaryWriter(outStream))
            {
                writer.Write(MAGIC_NUMBER);
                writer.Write(FILE_VERSION);
                // Config
                WriteUsizeString(writer, "{}");
                // Metadata
                WriteUsizeString(writer, "{}");

                writer.Write((ulong)_tracks.Count);
                foreach (var track in _tracks)
                {
                    var id = Guid.NewGuid();
                    writer.Write(id.ToByteArray());
                    writer.Write((uint)track.Order);
                    switch (track.Type)
                    {
                        case KaraokeTrackType.Lyrics:
                            writer.Write((byte)0);
                            WriteUsizeString(writer, "null");
                            break;
                        case KaraokeTrackType.Audio:
                            writer.Write((byte)1);
                            var config = track.GetTrackConfig<AudioTrackSettings>();
                            var json = JsonConvert.SerializeObject(new
                            {
                                Audio = new
                                {
                                    muted = config.Muted,
                                    volume = config.Volume
                                }
                            });
                            WriteUsizeString(writer, json);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    writer.Write((ulong)track.Events.Count);
                    var trackEventIds = new Dictionary<int, Guid>();
                    foreach (var ev in track.Events)
                    {
                        var evId = Guid.NewGuid();
                        if (trackEventIds.ContainsKey(ev.Id))
                        {
                            evId = trackEventIds[ev.Id];
                        }
                        else
                        {
                            trackEventIds.Add(ev.Id, evId);
                        }

                        writer.Write(evId.ToByteArray());
                        writer.Write(ev.LinkedId != -1);
                        if (ev.LinkedId != -1)
                        {
                            var linkedId = Guid.NewGuid();
                            if (trackEventIds.ContainsKey(ev.LinkedId))
                            {
                                linkedId = trackEventIds[ev.LinkedId];
                            }
                            else
                            {
                                trackEventIds.Add(ev.LinkedId, linkedId);
                            }

                            writer.Write(linkedId.ToByteArray());
                        }

                        writer.Write((uint)ev.StartTimeMilliseconds);
                        writer.Write((uint)ev.EndTimeMilliseconds);
                        switch (ev.Type)
                        {
                            case KaraokeEventType.Lyric:
                                writer.Write((byte)0);
                                WriteUsizeString(writer, JsonConvert.SerializeObject(new { Lyric = new { text = ev.RawValue } }));
                                break;
                            case KaraokeEventType.LineBreak:
                                writer.Write((byte)1);
                                WriteUsizeString(writer, "null");
                                break;
                            case KaraokeEventType.ParagraphBreak:
                                writer.Write((byte)2);
                                WriteUsizeString(writer, "null");
                                break;
                            case KaraokeEventType.AudioClip:
                                writer.Write((byte)8);
                                var audioEv = (AudioClipKaraokeEvent)ev;
                                var settings = audioEv.Settings ?? default;
                                if (settings == null)
                                {
                                    throw new InvalidDataException();
                                }
                                var audioInfo = AudioUtil.GetFileInfo(settings.AudioFile);
                                WriteUsizeString(writer, JsonConvert.SerializeObject(new
                                {
                                    AudioClip = new
                                    {
                                        offset = (uint)Math.Floor(settings.Offset * 1000),
                                        file = new
                                        {
                                            id = Guid.NewGuid(),
                                            file_type = AudioFileTypeStr(audioInfo.FormatType),
                                            source = new
                                            {
                                                Path = settings.AudioFile
                                            }
                                        }
                                    }
                                }));
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        private void Load(Stream inStream)
        {
            throw new NotImplementedException();
        }

        private string AudioFileTypeStr(AudioUtil.AudioFormatType formatType)
        {
            switch (formatType)
            {
                case AudioUtil.AudioFormatType.Mp3:
                    return "Mp3";
                case AudioUtil.AudioFormatType.Ogg:
                    return "Ogg";
                case AudioUtil.AudioFormatType.Wav:
                    return "Wave";
                default:
                    return "Invalid";
            }
        }

        private void WriteUsizeString(BinaryWriter writer, string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            writer.Write((ulong)bytes.Length);
            writer.Write(bytes);
        }
    }
}
