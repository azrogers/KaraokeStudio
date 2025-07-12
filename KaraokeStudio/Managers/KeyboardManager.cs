using KaraokeStudio.Commands.Updates;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using ScintillaNET;
using SharpHook;
using SharpHook.Native;

namespace KaraokeStudio.Managers
{
	internal class KeyboardManager
	{
		public static readonly KeyboardManager Instance = new KeyboardManager();

		private TaskPoolGlobalHook _hook = new TaskPoolGlobalHook();
		private MainForm _form;

		public void Initialize(MainForm form)
		{
			_form = form;
			_hook.KeyPressed += OnKeyPressed;
			_hook.RunAsync();
		}

		public void Cleanup()
		{
			_hook.Dispose();
		}

		private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
		{
			_form.Invoke(() =>
			{
				if(Form.ActiveForm != _form)
				{
					return;
				}

				if (IsTextFieldFocused(_form.ActiveControl))
				{
					return;
				}

				switch (e.Data.KeyCode)
				{
					case KeyCode.VcDelete:
					case KeyCode.VcBackspace:
						UpdateDispatcher.Dispatch(new DeleteKeyUpdate());
						break;
					case KeyCode.VcLeft:
					case KeyCode.VcRight:
						UpdateDispatcher.Dispatch(new ArrowKeyUpdate(e.Data.KeyCode == KeyCode.VcLeft));
						break;
				}
			});
		}

		private bool IsTextFieldFocused(Control? activeControl)
		{
			var currentControlType = activeControl?.GetType();

			if (currentControlType == typeof(Scintilla) || 
				currentControlType == typeof(TextBox) || 
				currentControlType == typeof(NumericUpDown) || 
				currentControlType == typeof(ComboBox) ||
				currentControlType == typeof(BlazorWebView))
			{
				return true;
			}

			if(activeControl is ContainerControl container) {
				return IsTextFieldFocused(container.ActiveControl);
			}

			return false;
		}
	}
}
