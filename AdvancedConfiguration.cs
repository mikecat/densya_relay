using System;
using System.Drawing;
using System.Windows.Forms;

class AdvancedConfiguration: Form
{
	private bool enablePortSettings;

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

	public int LocalPort
	{
		get { return (int)localPortInput.Value; }
		set { localPortInput.Value = Math.Min(Math.Max(value, 0), 65535); }
	}
	public bool LocalPortSameAsDestinationPort
	{
		get { return localPortSameAsDestinationPortCheck.Checked; }
		set { localPortSameAsDestinationPortCheck.Checked = value; }
	}
	public int SendSize
	{
		get { return (int)sendSizeInput.Value; }
		set { sendSizeInput.Value = Math.Min(Math.Max(value, (int)sendSizeInput.Minimum), (int)sendSizeInput.Maximum); }
	}
	public int ReceiveSize
	{
		get { return (int)receiveSizeInput.Value; }
		set { receiveSizeInput.Value = Math.Min(Math.Max(value, (int)receiveSizeInput.Minimum), (int)receiveSizeInput.Maximum); }
	}
	public string MmfName
	{
		get { return mmfNameInput.Text; }
		set { mmfNameInput.Text = value; }
	}
	public bool CreateMutex
	{
		get { return createMutexCheck.Checked; }
		set { createMutexCheck.Checked = value; }
	}
	public bool LockMutex
	{
		get { return lockMutexCheck.Checked; }
		set { lockMutexCheck.Checked = value; }
	}
	public string MutexName
	{
		get { return mutexNameInput.Text; }
		set { mutexNameInput.Text = value; }
	}

	public AdvancedConfiguration(UIText uiText, bool enablePortSettings)
	{
		this.enablePortSettings = enablePortSettings;
		this.Text = uiText.AdvancedConfigurationDialogTitle;
		this.Font = ControlUtils.Font;
		this.FormBorderStyle = FormBorderStyle.FixedSingle;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.ShowInTaskbar = false;
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
		localPortInput.Enabled = enablePortSettings;
		localPortSameAsDestinationPortCheck = ControlUtils.CreateControl<CheckBox>(networkGroup, 12, 1, 10, 1);
		localPortSameAsDestinationPortCheck.Text = uiText.SameAsDestinationPort;
		localPortSameAsDestinationPortCheck.Enabled = enablePortSettings;
		sendSizeLabel = ControlUtils.CreateControl<Label>(networkGroup, 0.5f, 2.5f, 7, 1);
		sendSizeLabel.Text = uiText.SendSize;
		sendSizeInput = ControlUtils.CreateControl<NumericUpDown>(networkGroup, 7.5f, 2.5f, 4, 1);
		sendSizeInput.Minimum = 0;
		sendSizeInput.Maximum = DensyaRelay.MMF_RECEIVED_DATA_START;
		sendSizeUnitLabel = ControlUtils.CreateControl<Label>(networkGroup, 12, 2.5f, 4, 1);
		sendSizeUnitLabel.Text = uiText.SizeUnit;
		receiveSizeLabel = ControlUtils.CreateControl<Label>(networkGroup, 0.5f, 4, 7, 1);
		receiveSizeLabel.Text = uiText.ReceiveSize;
		receiveSizeInput = ControlUtils.CreateControl<NumericUpDown>(networkGroup, 7.5f, 4, 4, 1);
		receiveSizeInput.Minimum = 0;
		receiveSizeInput.Maximum = DensyaRelay.MMF_SIZE - DensyaRelay.MMF_RECEIVED_DATA_START;
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
		okButton.DialogResult = DialogResult.OK;
		this.AcceptButton = okButton;
		cancelButton = ControlUtils.CreateControl<Button>(this, 19, 12.5f, 4.5f, 1.5f);
		cancelButton.Text = uiText.Cancel;
		this.CancelButton = cancelButton;

		ResumeLayout();

		localPortSameAsDestinationPortCheck.CheckedChanged += LocalPortSameAsDestinationPortCheckCheckedChangedHandler;
		createMutexCheck.CheckedChanged += CreateMutexCheckCheckedChangedHandler;
		LocalPortSameAsDestinationPortCheckCheckedChangedHandler(null, null);
		CreateMutexCheckCheckedChangedHandler(null, null);
	}

	private void LocalPortSameAsDestinationPortCheckCheckedChangedHandler(object sender, EventArgs e)
	{
		localPortInput.Enabled = enablePortSettings && !localPortSameAsDestinationPortCheck.Checked;
	}

	private void CreateMutexCheckCheckedChangedHandler(object sender, EventArgs e)
	{
		lockMutexCheck.Enabled = createMutexCheck.Checked;
		mutexNameInput.Enabled = createMutexCheck.Checked;
	}
}
