using KaraokeLib.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.LyricsEditor
{
    internal class LyricsLexer
	{
		public static IEnumerable<LyricsLexerElement> Process(IEnumerable<LyricsToken> tokens)
		{
			var currentTokens = new List<LyricsToken>();
			foreach(var token in tokens)
			{
				if(token.Type == LyricsTokenType.Text)
				{
					currentTokens.Add(token);
					continue;
				}

				if(currentTokens.Any())
				{
					yield return new LyricsLexerElement(KaraokeEventType.Lyric, currentTokens.Select(t => t.Value).ToArray());
					currentTokens.Clear();
				}

				if(token.Type == LyricsTokenType.LineBreak)
				{
					yield return new LyricsLexerElement(KaraokeEventType.LineBreak);
				}
				else if(token.Type == LyricsTokenType.ParagraphBreak)
				{
					yield return new LyricsLexerElement(KaraokeEventType.ParagraphBreak);
				}
			}
		}
	}

	internal class LyricsLexerElement
	{
		private static readonly FastHashes.XxHash32 Hash = new FastHashes.XxHash32();

		public KaraokeEventType Type { get; private set; }
		public string[] Tokens => _tokens;

		private string[] _tokens;

		public LyricsLexerElement(KaraokeEventType type, string[]? tokens = null)
		{
			_tokens = tokens ?? new string[0];
			Type = type;
		}

		public int ToHash()
		{
			var bytes = new List<byte>
			{
				(byte)Type
			};

			if (Type == KaraokeEventType.Lyric)
			{
				bytes.AddRange(BitConverter.GetBytes(Tokens.Length));
				foreach (var token in Tokens)
				{
					bytes.AddRange(Encoding.UTF8.GetBytes(token));
				}
			}

			return BitConverter.ToInt32(Hash.ComputeHash(bytes.ToArray()));
		}

		public override string ToString()
		{
			return _tokens.Length > 0 ? $"{Type} '{string.Join("", _tokens)}'" : Type.ToString();
		}
	}
}
