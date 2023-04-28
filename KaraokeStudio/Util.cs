using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio
{
	internal class Util
	{
		public static string FormatTimespan(TimeSpan timeSpan, bool fractional = false)
		{
			if(timeSpan.TotalHours >= 1)
			{
				return fractional ? $"{timeSpan:hh\\:mm\\:ss\\.fff}" : $"{timeSpan:hh\\:mm\\:ss}";
			}

			return fractional ? $"{timeSpan:mm\\:ss\\.fff}" : $"{timeSpan:mm\\:ss}";
		}
	}
}
