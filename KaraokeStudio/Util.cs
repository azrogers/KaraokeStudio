using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio
{
	internal static class Util
	{
		public static string FormatTimespan(TimeSpan timeSpan, bool fractional = false)
		{
			if (timeSpan.TotalHours >= 1)
			{
				return fractional ? $"{timeSpan:hh\\:mm\\:ss\\.fff}" : $"{timeSpan:hh\\:mm\\:ss}";
			}

			return fractional ? $"{timeSpan:mm\\:ss\\.fff}" : $"{timeSpan:mm\\:ss}";
		}

		public static bool IsNumericType(Type t)
		{
			switch (Type.GetTypeCode(t))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}

		public static string HumanizeCamelCase(string s)
		{
			var builder = new StringBuilder();
			var chars = s.ToCharArray();
			for(var i = 0; i < chars.Length; i++)
			{
				var c = chars[i];
				if(i == 0)
				{
					builder.Append(char.ToUpper(c));
				}
				else if(char.IsUpper(c))
				{
					builder.Append(' ');
					builder.Append(c);
				}
				else
				{
					builder.Append(c);
				}
			}

			return builder.ToString();
		}

		public static void ResizeContainerAspectRatio(Control container, Control control, (int Width, int Height) size, bool verticalCenter = true)
		{
			// ensure panels are the correct size
			var widthHeightRatio = (double)size.Width / size.Height;
			var heightWidthRatio = (double)size.Height / size.Width;

			var targetSize = new Size(0, 0);
			if (((double)container.Size.Width / container.Size.Height) > widthHeightRatio)
			{
				targetSize = new Size((int)(container.Size.Height * widthHeightRatio), container.Size.Height);
			}
			else
			{
				targetSize = new Size(container.Size.Width, (int)(container.Size.Width * heightWidthRatio));
			}

			// update and center
			control.Size = targetSize;
			control.Location = new Point(
				container.Size.Width / 2 - targetSize.Width / 2,
				verticalCenter ? container.Size.Height / 2 - targetSize.Height / 2 : 0);
		}
	}
}
