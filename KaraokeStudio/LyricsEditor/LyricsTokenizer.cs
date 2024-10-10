using System.Text;

namespace KaraokeStudio.LyricsEditor
{
	internal class LyricsTokenizer
	{
		public static IEnumerable<LyricsToken> Tokenize(string input)
		{
			using (var reader = new StringReader(input))
			{
				int nextCh = -1;
				var lineCount = 0;
				var blockLevel = 0;
				var currentType = LyricsTokenType.Invalid;
				var currentValue = new StringBuilder();
				var isEscaped = false;
				while ((nextCh = reader.Read()) != -1)
				{
					var ch = (char)nextCh;
					if (ch == '\n')
					{
						if (currentType == LyricsTokenType.Text || currentType == LyricsTokenType.Whitespace)
						{
							yield return new LyricsToken(currentType, currentValue.ToString());
							currentValue.Clear();
							currentType = LyricsTokenType.Invalid;
						}

						lineCount++;
						if (lineCount > 1)
						{
							yield return new LyricsToken(LyricsTokenType.ParagraphBreak);
							lineCount = 0;
						}

						continue;
					}

					// handle CRLF line break
					if (ch == '\r' && reader.Peek() == '\n')
					{
						continue;
					}

					// not a line break or paragraph break, so push the last line break if we need to
					if (lineCount > 0)
					{
						yield return new LyricsToken(LyricsTokenType.LineBreak);
						lineCount = 0;
					}

					// handle spaces and tabs
					if (char.IsWhiteSpace(ch))
					{
						if (blockLevel <= 0)
						{
							if (currentType == LyricsTokenType.Text)
							{
								yield return new LyricsToken(LyricsTokenType.Text, currentValue.ToString());
								currentValue.Clear();
							}
							currentType = LyricsTokenType.Whitespace;
						}

						currentValue.Append(ch);
						continue;
					}

					if (currentType == LyricsTokenType.Whitespace)
					{
						yield return new LyricsToken(LyricsTokenType.Whitespace, currentValue.ToString());
						currentValue.Clear();

						currentType = LyricsTokenType.Text;
					}

					// handle escaping and syllable separator
					if (ch == LyricsConstants.ESCAPE_CHAR && !isEscaped)
					{
						isEscaped = true;
						continue;
					}

					if (isEscaped)
					{
						isEscaped = false;
						if (
							ch != LyricsConstants.ESCAPE_CHAR &&
							ch != LyricsConstants.SYLLABLE_SEPERATOR &&
							ch != LyricsConstants.BLOCK_OPEN &&
							ch != LyricsConstants.BLOCK_CLOSE)
						{
							// if it's not escaping something that needs to be escaped, just include it
							currentValue.Append(LyricsConstants.ESCAPE_CHAR);
						}

						currentType = LyricsTokenType.Text;
						currentValue.Append(ch);
						continue;
					}

					if (ch == LyricsConstants.SYLLABLE_SEPERATOR)
					{
						yield return new LyricsToken(LyricsTokenType.Text, currentValue.ToString());
						currentType = LyricsTokenType.Invalid;
						currentValue.Clear();
						continue;
					}

					if (ch == LyricsConstants.BLOCK_OPEN)
					{
						blockLevel++;
					}
					else if (ch == LyricsConstants.BLOCK_CLOSE)
					{
						blockLevel--;
					}

					currentType = LyricsTokenType.Text;
					currentValue.Append(ch);
				}

				// push last token
				if (currentType != LyricsTokenType.Invalid)
				{
					yield return new LyricsToken(currentType, currentValue.ToString());
				}

				// push last line
				if (lineCount > 0)
				{
					yield return new LyricsToken(LyricsTokenType.LineBreak);
				}
			}
		}
	}

	internal class LyricsToken
	{
		public LyricsTokenType Type { get; private set; }

		public string Value => _value ?? "";

		private string? _value;

		public LyricsToken(LyricsTokenType type, string val = null)
		{
			Type = type;
			_value = val;
		}

		public override string ToString()
		{
			return $"{Type} '{Value}'";
		}
	}

	internal enum LyricsTokenType
	{
		Invalid = -1,
		Text = 0,
		Whitespace = 1,
		LineBreak = 2,
		ParagraphBreak = 3,
	}
}
