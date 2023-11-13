using KaraokeLib.Config;
using KaraokeStudio.Config.Controls;
using KaraokeStudio.Util;

namespace KaraokeStudio.Config
{
	/// <summary>
	/// A reusable editor component for classes deriving from EditorConfig.
	/// </summary>
	public partial class ConfigEditor : UserControl
	{
		private IEditableConfig? _temporaryConfig = null;
		private List<BaseConfigControl> _controls = new List<BaseConfigControl>();
		private List<Label> _labels = new List<Label>();

		/// <summary>
		/// Event called when a config value has been changed.
		/// </summary>
		public event Action? OnValueChanged = null;

		/// <summary>
		/// Gets or sets the config instance being modified by this ConfigEditor.
		/// </summary>
		/// <remarks>
		/// This does not copy the object passed in. </remarks>
		public IEditableConfig? Config
		{
			get => _temporaryConfig;
			set
			{
				var oldValue = _temporaryConfig;
				_temporaryConfig = value;

				if (value == null)
				{
					ClearControls();
				}
				else if (oldValue == null || value.GetType() != oldValue.GetType())
				{
					// type changed, rebuild the whole thing
					CreateFields();
				}

				UpdateControls();
			}
		}

		public ConfigEditor()
		{
			InitializeComponent();

			// disable horizontal scroll on configContainer
			configContainer.HorizontalScroll.Maximum = 0;
			configContainer.AutoScroll = false;
			configContainer.VerticalScroll.Visible = true;
			configContainer.AutoScroll = true;

			configContainer.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
		}

		/// <summary>
		/// Obtains the <see cref="Config"/> value currently being edited by this ConfigEditor, cast to the given type.
		/// </summary>
		public T GetConfig<T>() where T : IEditableConfig
		{
			if (_temporaryConfig == null)
			{
				return Activator.CreateInstance<T>();
			}

			return (T)_temporaryConfig;
		}

		private void CreateFields()
		{
			ClearControls();

			configContainer.ColumnCount = 2;

			if (_temporaryConfig == null)
			{
				return;
			}

			configContainer.SuspendLayout();

			foreach (var field in _temporaryConfig.Fields)
			{
				switch (field.ControlType)
				{
					case ConfigControlType.Size:
						AddField(field.Name, new SizeConfigControl() { Field = field });
						break;
					case ConfigControlType.Numeric:
						AddField(field.Name, new NumericConfigControl() { Field = field });
						break;
					case ConfigControlType.Range:
						AddField(field.Name, new RangeConfigControl() { Field = field });
						break;
					case ConfigControlType.Color:
						AddField(field.Name, new ColorConfigControl() { Field = field });
						break;
					case ConfigControlType.Font:
						AddField(field.Name, new FontConfigControl() { Field = field });
						break;
					case ConfigControlType.Enum:
						AddField(field.Name, new EnumConfigControl() { Field = field });
						break;
				}
			}

			configContainer.ResumeLayout(true);
		}

		private void AddField(string name, BaseConfigControl control)
		{
			control.Dock = DockStyle.Fill;
			control.AutoSize = true;
			control.AutoSizeMode = AutoSizeMode.GrowAndShrink;

			control.OnValueChanged += (_) =>
			{
				if (_temporaryConfig == null)
				{
					throw new InvalidDataException("Value changed but config is null?");
				}

				OnValueChanged?.Invoke();
				control.SetValue(_temporaryConfig);
			};

			var labelWidth = configContainer.ColumnStyles[0].Width;

			var label = new Label();
			label.Text = Utility.HumanizeCamelCase(name);
			label.Size = new Size((int)labelWidth, 15);
			label.AutoSize = true;

			configContainer.RowCount = _controls.Count + 1;
			configContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize, 15.0f));
			configContainer.Controls.Add(label, 0, _controls.Count);
			configContainer.Controls.Add(control, 1, _controls.Count);

			if (_temporaryConfig != null)
			{
				control.UpdateValue(_temporaryConfig);
			}

			_labels.Add(label);
			_controls.Add(control);
		}

		private void UpdateControls()
		{
			if (_temporaryConfig == null)
			{
				return;
			}

			foreach (var c in _controls)
			{
				c.UpdateValue(_temporaryConfig);
			}
		}

		private void ClearControls()
		{
			_labels.Clear();
			_controls.Clear();
			configContainer.Controls.Clear();
		}
	}
}
