using KaraokeLib.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.LyricsEditor
{
    internal class LyricsMatcher
	{
		public static void Match(LyricsEditorTextElement[] oldElements, LyricsLexerElement[] newElements)
		{
			/*
			 * we want to know strings of oldElements that match newElements, so timing information can be transferred
			 * for this, we need to return an index into oldElements, an index into newElements, and a length
			 * 
			 * scenarios:
			 * - oldElements and newElements match exactly
			 * - newElements has additions that don't exist in oldElements
			 * - newElements has removals that exist in oldElements
			 * 
			 * if there's an addition, we want to skip the addition until we find the next element that exists in both
			 * if there's a removal, we want to skip the old elements that were removed until we find the next element that exists in both
			 * 
			 * if oe != ne, 
			 *		we want to check forward in newElements for the next string that continues oldElements (handling additions)
			 *		we want to check forward in oldElements for the next string that continues newElements (handling removals)
			 *		we want to take the better of the above two strings (which string is longer)
			 *		
			 *	algorithm:
			 *		oldI = 0, newI = 0, length = 0
			 *		if old[oldI] == new[newI] then
			 *			oldI++
			 *			newI++
			 *		else
			 *			if length > 0 then
			 *				push_string(oldI - length, newI - length, length)
			 *				length = 0
			 *			end
			 *			
			 *			
			 */

			var oldArr = oldElements.Select(o => o.ToHash()).ToArray() ?? new int[0];
			var newArr = newElements.Select(o => o.ToHash()).ToArray() ?? new int[0];

			var results = Diff.DiffInt(oldArr, newArr);
		}

		private static int LongestCommonSubsequence(LyricsEditorTextElement[] oldElements, LyricsLexerElement[] newElements, LyricsIndex startOld, LyricsIndex startNew)
		{
			if(startOld.ElementIndex >= oldElements.Length || startNew.ElementIndex >= newElements.Length)
			{
				return 0;
			}

			var oldEv = oldElements[startOld.ElementIndex].Events[startOld.EventIndex];
			var newElem = newElements[startNew.ElementIndex];

			if (DoElementsMatch(oldEv, newElem, startNew.EventIndex))
			{
				return 1 + LongestCommonSubsequence(oldElements, newElements, startOld.Increment(oldElements), startNew.Increment(newElements));
			}

			return Math.Max(
				LongestCommonSubsequence(oldElements, newElements, startOld.Increment(oldElements), startNew),
				LongestCommonSubsequence(oldElements, newElements, startOld, startNew.Increment(newElements)));
		}

		private static bool DoElementsMatch(KaraokeEvent oldEvent, LyricsLexerElement newElement, int eventIndex)
		{
			if (oldEvent.Type != newElement.Type)
			{
				return false;
			}

			if (newElement.Type == KaraokeEventType.Lyric)
			{
				return newElement.Tokens[eventIndex] == oldEvent.GetText(null);
			}

			return true;
		}

		private struct LyricsIndex
		{
			public int ElementIndex;
			public int EventIndex;

			public LyricsIndex(int elementIndex, int eventIndex)
			{
				ElementIndex = elementIndex;
				EventIndex = eventIndex;
			}

			public LyricsIndex Increment(LyricsEditorTextElement[] arr)
			{
				var elem = arr[ElementIndex];
				if(EventIndex < elem.Events.Length - 1)
				{
					return new LyricsIndex(ElementIndex, EventIndex + 1);
				}
				
				return new LyricsIndex(ElementIndex + 1, 0);
			}

			public LyricsIndex Increment(LyricsLexerElement[] arr)
			{
				var elem = arr[ElementIndex];
				if(EventIndex < elem.Tokens.Length - 1)
				{
					return new LyricsIndex(ElementIndex, EventIndex + 1);
				}

				return new LyricsIndex(ElementIndex + 1, 0);
			}
		}
		private record struct LyricsMatch(int OldIndex, int NewIndex, int Length);
	}
}
