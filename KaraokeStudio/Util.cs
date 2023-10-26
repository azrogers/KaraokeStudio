using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio
{
	internal static class Util
	{
		/// <summary>
		/// Formats the given timespan for display.
		/// </summary>
		/// <param name="fractional">If true, milliseconds will be included.</param>
		public static string FormatTimespan(TimeSpan timeSpan, bool fractional = false)
		{
			if (timeSpan.TotalHours >= 1)
			{
				return fractional ? $"{timeSpan:hh\\:mm\\:ss\\.fff}" : $"{timeSpan:hh\\:mm\\:ss}";
			}

			return fractional ? $"{timeSpan:mm\\:ss\\.fff}" : $"{timeSpan:mm\\:ss}";
		}

		/// <summary>
		/// Turns an UpperCamelCase string into a friendlier string for display.
		/// </summary>
		/// <example>
		/// "UpperCamelCase" => "Upper Camel Case"
		/// </example>
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

		/// <summary>
		/// Resizes the given control within its container so that it's the largest size possible with the given aspect ratio.
		/// </summary>
		/// <param name="container">The control's container.</param>
		/// <param name="control">The control to resize and move.</param>
		/// <param name="size">The target size of the control.</param>
		/// <param name="verticalCenter">If ture, the control will be vertically centered within the container.</param>
		public static void ResizeContainerAspectRatio(Control container, Control control, (int Width, int Height) size, bool verticalCenter = true)
		{
			// ensure panels are the correct size
			var widthHeightRatio = (double)size.Width / size.Height;
			var heightWidthRatio = (double)size.Height / size.Width;

			Size targetSize;
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

		/// <summary>
		/// Converts a <see cref="Color"/> to a Skia color.
		/// </summary>
		public static SKColor ToSKColor(this Color c) => new SKColor(c.R, c.G, c.B);

		/// <summary>
		/// Performs a linear interpretation between two values.
		/// </summary>
		/// <param name="a">The start value.</param>
		/// <param name="b">The end value.</param>
		/// <param name="t">The normalized position between start and end.</param>
		public static double Lerp(double a, double b, double t)
		{
			return (b - a) * t + a;
		}
	}
}
