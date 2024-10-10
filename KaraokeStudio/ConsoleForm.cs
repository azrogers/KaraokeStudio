using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaraokeStudio
{
	public partial class ConsoleForm : Form
	{
		private static string LogString = "";
		private static event Action<string>? OnLogUpdate;

		public static void LogEvent(LogEventInfo info, object[] param)
		{
			LogString += $"[{info.LoggerName}] <{info.TimeStamp.ToLongTimeString()}> {info.FormattedMessage}" + Environment.NewLine;
			if(LogString.Length > short.MaxValue)
			{
				LogString = LogString.Substring(LogString.Length - short.MaxValue, short.MaxValue);
			}

			OnLogUpdate?.Invoke(LogString);
		}

		public ConsoleForm()
		{
			InitializeComponent();
		}

		private void ConsoleForm_Load(object sender, EventArgs e)
		{
			logBox.Text = LogString;
			if (logBox.SelectionLength <= 0)
			{
				logBox.SelectionStart = logBox.Text.Length;
				logBox.ScrollToCaret();
			}
			OnLogUpdate += ConsoleForm_OnLogUpdate;
		}

		private void ConsoleForm_OnLogUpdate(string obj)
		{
			Invoke(() =>
			{
				logBox.Text = LogString;
				if (logBox.SelectionLength <= 0)
				{
					logBox.SelectionStart = logBox.Text.Length;
					logBox.ScrollToCaret();
				}
			});
		}

		private void ConsoleForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			OnLogUpdate -= ConsoleForm_OnLogUpdate;
		}
	}
}
