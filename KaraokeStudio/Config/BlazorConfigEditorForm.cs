using KaraokeLib.Config;
using KaraokeStudio.Blazor;
using KaraokeStudio.Util;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace KaraokeStudio.Config
{
    public partial class BlazorConfigEditorForm : Form
    {
        private Type _configType;
        private Action<IEditableConfig>? _applyConfigCallback;
        private bool _isDirty = false;
        private IEditableConfig? _originalConfig;

        public BlazorConfigEditorForm()
        {
            InitializeComponent();
            // dummy type to throw error if actually used
            _configType = typeof(BlazorConfigEditorForm);

            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            services.AddMudServices();
            services.AddBlazorWebViewDeveloperTools();
            services.AddLogging();

            blazorView.HostPage = "wwwroot/index.html";
            blazorView.Services = services.BuildServiceProvider();
            blazorView.RootComponents.Add<BlazorConfigEditor>("#app");
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
            //configEditor.Config = configObj.Copy();

            Text = $"Editing {Utility.HumanizeCamelCase(_configType.Name)}";
            UpdateButtons();
            Show();
        }

        public void ReplaceConfigIfNotChanged(IEditableConfig newConfig)
        {
            if (_isDirty)
            {
                return;
            }

            _originalConfig = newConfig;
            //configEditor.Config = newConfig.Copy();
        }

        private void UpdateButtons()
        {
            applyButton.Enabled = _isDirty;
            revertButton.Enabled = _isDirty;
            okButton.Enabled = _isDirty;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            /*if (_applyConfigCallback != null && configEditor.Config != null)
			{
				_originalConfig = configEditor.Config.Copy();
				_applyConfigCallback(_originalConfig);
			}*/

            Hide();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            /*if (_applyConfigCallback != null && configEditor.Config != null)
			{
				_originalConfig = configEditor.Config.Copy();
				_applyConfigCallback(_originalConfig);
				_isDirty = false;
			}*/

            UpdateButtons();
        }

        private void revertButton_Click(object sender, EventArgs e)
        {
            //configEditor.Config = _originalConfig;
            _isDirty = false;

            UpdateButtons();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
