using KaraokeLib.Config;
using KaraokeStudio.Util;

namespace KaraokeStudio.Config
{
	public partial class GenericConfigEditorForm : Form
	{
		private Type _configType;
		private Action<IEditableConfig>? _applyConfigCallback;
		private bool _isDirty = false;
		private IEditableConfig? _originalConfig;

		public GenericConfigEditorForm()
		{
			InitializeComponent();
			// dummy type to throw error if actually used
			_configType = typeof(GenericConfigEditorForm);

			configEditor.OnValueChanged += ConfigEditor_OnValueChanged;
		}

		private void ConfigEditor_OnValueChanged()
		{
			_isDirty = true;

			UpdateButtons();
		}

		public void Open(IEditableConfig configObj, Action<IEditableConfig> applyConfigCallback)
		{
			_applyConfigCallback = applyConfigCallback;
			_configType = configObj.GetType();
			_originalConfig = configObj;
			configEditor.Config = configObj.Copy();

			Text = $"Editing {Utility.HumanizeCamelCase(_configType.Name)}";
			UpdateButtons();
			Show();
		}

		public void ReplaceConfigIfNotChanged(IEditableConfig newConfig)
		{
			if(_isDirty)
			{
				return;
			}

			_originalConfig = newConfig;
			configEditor.Config = newConfig.Copy();
		}

		private void UpdateButtons()
		{
			applyButton.Enabled = _isDirty;
			revertButton.Enabled = _isDirty;
			okButton.Enabled = _isDirty;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (_applyConfigCallback != null && configEditor.Config != null)
			{
				_originalConfig = configEditor.Config.Copy();
				_applyConfigCallback(_originalConfig);
			}

			Hide();
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			if (_applyConfigCallback != null && configEditor.Config != null)
			{
				_originalConfig = configEditor.Config.Copy();
				_applyConfigCallback(_originalConfig);
				_isDirty = false;
			}

			UpdateButtons();
		}

		private void revertButton_Click(object sender, EventArgs e)
		{
			configEditor.Config = _originalConfig;
			_isDirty = false;

			UpdateButtons();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Hide();
		}
	}
}
