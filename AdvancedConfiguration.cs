using System;
using System.Drawing;
using System.Windows.Forms;

class AdvancedConfiguration: Form
{
	private GroupBox networkGroup;
	private Label localPortLabel;
	private NumericUpDown localPortInput;
	private CheckBox localPortSameAsDestinationPortCheck;
	private Label sendSizeLabel, sendSizeUnitLabel;
	private NumericUpDown sendSizeInput;
	private Label receiveSizeLabel, receiveSizeUnitLabel;
	private NumericUpDown receiveSizeInput;

	private GroupBox mmfGroup;
	private Label mmfNameLabel;
	private TextBox mmfNameInput;
	private CheckBox createMutexCheck;
	private CheckBox lockMutexCheck;
	private Label mutexNameLabel;
	private TextBox mutexNameInput;

	private Button okButton;
	private Button cancelButton;

	public AdvancedConfiguration(UIText uiText)
	{
		this.Text = uiText.AdvancedConfigurationDialogTitle;
		this.Font = ControlUtils.Font;
		this.FormBorderStyle = FormBorderStyle.FixedSingle;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.ClientSize = ControlUtils.GetSizeOnGrid(24, 14.5f);
		SuspendLayout();

		networkGroup = ControlUtils.CreateControl<GroupBox>(this, 0.5f, 0.5f, 23, 5.5f);
		networkGroup.Text = uiText.NetworkConfiguration;
		networkGroup.SuspendLayout();
		localPortLabel = ControlUtils.CreateControl<Label>(networkGroup, 0.5f, 1, 7, 1);
		localPortLabel.Text = uiText.LocalPort;
		localPortInput = ControlUtils.CreateControl<NumericUpDown>(networkGroup, 7.5f, 1, 4, 1);
		localPortInput.Minimum = 0;
		localPortInput.Maximum = 65535;
		localPortSameAsDestinationPortCheck = ControlUtils.CreateControl<CheckBox>(networkGroup, 12, 1, 10, 1);
		localPortSameAsDestinationPortCheck.Text = uiText.SameAsDestinationPort;
		sendSizeLabel = ControlUtils.CreateControl<Label>(networkGroup, 0.5f, 2.5f, 7, 1);
		sendSizeLabel.Text = uiText.SendSize;
		sendSizeInput = ControlUtils.CreateControl<NumericUpDown>(networkGroup, 7.5f, 2.5f, 4, 1);
		sendSizeUnitLabel = ControlUtils.CreateControl<Label>(networkGroup, 12, 2.5f, 4, 1);
		sendSizeUnitLabel.Text = uiText.SizeUnit;
		receiveSizeLabel = ControlUtils.CreateControl<Label>(networkGroup, 0.5f, 4, 7, 1);
		receiveSizeLabel.Text = uiText.ReceiveSize;
		receiveSizeInput = ControlUtils.CreateControl<NumericUpDown>(networkGroup, 7.5f, 4, 4, 1);
		receiveSizeUnitLabel = ControlUtils.CreateControl<Label>(networkGroup, 12, 4, 8, 1);
		receiveSizeUnitLabel.Text = uiText.SizeUnit;
		networkGroup.ResumeLayout();

		mmfGroup = ControlUtils.CreateControl<GroupBox>(this, 0.5f, 6.5f, 23, 5.5f);
		mmfGroup.Text = uiText.MmfConfiguration;
		mmfGroup.SuspendLayout();
		mmfNameLabel = ControlUtils.CreateControl<Label>(mmfGroup, 0.5f, 1, 8, 1);
		mmfNameLabel.Text = uiText.MmfName;
		mmfNameInput = ControlUtils.CreateControl<TextBox>(mmfGroup, 8.5f, 1, 14, 1);
		createMutexCheck = ControlUtils.CreateControl<CheckBox>(mmfGroup, 0.5f, 2.5f, 10, 1);
		createMutexCheck.Text = uiText.CreateMutex;
		lockMutexCheck = ControlUtils.CreateControl<CheckBox>(mmfGroup, 10.5f, 2.5f, 12, 1);
		lockMutexCheck.Text = uiText.LockMutex;
		mutexNameLabel = ControlUtils.CreateControl<Label>(mmfGroup, 0.5f, 4, 8, 1);
		mutexNameLabel.Text = uiText.MutexName;
		mutexNameInput = ControlUtils.CreateControl<TextBox>(mmfGroup, 8.5f, 4, 14, 1);
		mmfGroup.ResumeLayout();

		okButton = ControlUtils.CreateControl<Button>(this, 14, 12.5f, 4.5f, 1.5f);
		okButton.Text = uiText.OK;
		this.AcceptButton = okButton;
		cancelButton = ControlUtils.CreateControl<Button>(this, 19, 12.5f, 4.5f, 1.5f);
		cancelButton.Text = uiText.Cancel;
		this.CancelButton = cancelButton;

		ResumeLayout();
	}
}
