using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

class DensyaRelay: Form
{
	public static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new DensyaRelay());
	}

	private const int fontSize = 16, gridSize = 20;

	private UIText uiText = new JapaneseUIText();
	private string versionString = "";

	private MenuStrip mainMenuStrip;
	private ToolStripMenuItem languageMenuItem;
	private ToolStripMenuItem languageJapaneseMenuItem, languageEnglishMenuItem;
	private ToolStripMenuItem configurationMenuItem;
	private ToolStripMenuItem advancedConfigurationMenuItem;

	private Panel mainPanel;

	private GroupBox networkGroup;
	private Panel networkModeRadioPanel;
	private RadioButton networkOffRadio, networkSendRadio, networkReceiveRadio;
	private Label networkPeerAddressLabel;
	private TextBox networkPeerAddressInput;
	private Label networkPortLabel;
	private NumericUpDown networkPortInput;
	private CheckBox preferIPv6Check;
	private Label networkLastReceiveLabel;
	private Button openKeySendWindowButton;

	private GroupBox sendingGroup;
	private Label brakeLabel;
	private HScrollBar brakeBar;
	private NumericUpDown brakeInput;
	private Label powerLabel;
	private HScrollBar powerBar;
	private NumericUpDown powerInput;
	private Label controllerLabel;
	private Panel controllerRadioPanel;
	private RadioButton controllerSanyoRadio, controllerReversedSanyoRadio;
	private RadioButton controllerRyojoRadio, controllerOtherRadio;
	private Label controllerPowerLabel, controllerBrakeLabel;
	private NumericUpDown controllerPowerInput, controllerBrakeInput;
	private Label extBrakeLabel;
	private HScrollBar extBrakeBar;
	private NumericUpDown extBrakeInput;

	private GroupBox receivingGroup;
	private CheckBox doorClosedCheck;
	private NumericUpDown doorClosedInput;
	private CheckBox shockLeftCheck;
	private NumericUpDown shockLeftInput;
	private CheckBox shockRightCheck;
	private NumericUpDown shockRightInput;
	private Label ledLabel;
	private HScrollBar ledBar;
	private NumericUpDown ledInput;
	private Label atcLabel;
	private HScrollBar atcBar;
	private CheckBox atcOffCheck;
	private NumericUpDown atcInput;
	private Label speedLabel;
	private HScrollBar speedBar;
	private NumericUpDown speedInput;

	public DensyaRelay()
	{
		AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
		Version assemblyVersion = assemblyName.Version;
		if (assemblyVersion != null)
		{
			versionString = string.Format(
				" Ver {0}.{1}.{2}",
				assemblyVersion.Major,
				assemblyVersion.Minor,
				assemblyVersion.Build
			);
		}

		this.Font = ControlUtils.Font;
		this.FormBorderStyle = FormBorderStyle.FixedSingle;
		this.MaximizeBox = false;
		SuspendLayout();

		mainMenuStrip = new MenuStrip();
		languageMenuItem = new ToolStripMenuItem();
		languageMenuItem.Text = "言語 / Language (&L)";
		languageJapaneseMenuItem = new ToolStripMenuItem();
		languageJapaneseMenuItem.Text = "日本語 (&J)";
		languageEnglishMenuItem = new ToolStripMenuItem();
		languageEnglishMenuItem.Text = "English (&E)";
		languageMenuItem.DropDownItems.Add(languageJapaneseMenuItem);
		languageMenuItem.DropDownItems.Add(languageEnglishMenuItem);
		mainMenuStrip.Items.Add(languageMenuItem);
		configurationMenuItem = new ToolStripMenuItem();
		advancedConfigurationMenuItem = new ToolStripMenuItem();
		configurationMenuItem.DropDownItems.Add(advancedConfigurationMenuItem);
		mainMenuStrip.Items.Add(configurationMenuItem);
		this.Controls.Add(mainMenuStrip);
		this.MainMenuStrip = mainMenuStrip;

		mainPanel = ControlUtils.CreateControl<Panel>(this, 0, 0, 31, 22);
		mainPanel.Top += mainMenuStrip.Height;
		this.ClientSize = mainPanel.Size;
		this.Height += mainMenuStrip.Height;
		mainPanel.SuspendLayout();

		networkGroup = ControlUtils.CreateControl<GroupBox>(mainPanel, 0.5f, 0.5f, 30, 6);
		networkGroup.SuspendLayout();
		networkModeRadioPanel = ControlUtils.CreateControl<Panel>(networkGroup, 0.5f, 1, 22, 1);
		networkModeRadioPanel.SuspendLayout();
		networkOffRadio = ControlUtils.CreateControl<RadioButton>(networkModeRadioPanel, 0, 0, 4, 1);
		networkSendRadio = ControlUtils.CreateControl<RadioButton>(networkModeRadioPanel, 4, 0, 9, 1);
		networkReceiveRadio = ControlUtils.CreateControl<RadioButton>(networkModeRadioPanel, 13, 0, 9, 1);
		networkModeRadioPanel.ResumeLayout();
		networkPeerAddressLabel = ControlUtils.CreateControl<Label>(networkGroup, 0.5f, 2.5f, 7, 1);
		networkPeerAddressInput = ControlUtils.CreateControl<TextBox>(networkGroup, 7.5f, 2.5f, 8.5f, 1);
		networkPortLabel = ControlUtils.CreateControl<Label>(networkGroup, 16, 2.5f, 3, 1);
		networkPortInput = ControlUtils.CreateControl<NumericUpDown>(networkGroup, 19, 2.5f, 4, 1);
		networkPortInput.Minimum = 0;
		networkPortInput.Maximum = 65535;
		preferIPv6Check = ControlUtils.CreateControl<CheckBox>(networkGroup, 23.5f, 2.5f, 5.5f, 1);
		networkLastReceiveLabel = ControlUtils.CreateControl<Label>(networkGroup, 0.5f, 4, 19, 1.5f);
		openKeySendWindowButton = ControlUtils.CreateControl<Button>(networkGroup, 19.5f, 4, 10, 1.5f);
		networkGroup.ResumeLayout();

		sendingGroup = ControlUtils.CreateControl<GroupBox>(mainPanel, 0.5f, 7, 30, 7);
		sendingGroup.SuspendLayout();
		brakeLabel = ControlUtils.CreateControl<Label>(sendingGroup, 0.5f, 1, 5, 1);
		brakeBar = ControlUtils.CreateControl<HScrollBar>(sendingGroup, 5.5f, 1, 20.5f, 1);
		brakeBar.Minimum = 0;
		brakeBar.Maximum = 10;
		brakeBar.SmallChange = 1;
		brakeBar.LargeChange = 1;
		brakeInput = ControlUtils.CreateControl<NumericUpDown>(sendingGroup, 26.5f, 1, 3, 1);
		brakeInput.Minimum = 0;
		brakeInput.Maximum = 255;
		powerLabel = ControlUtils.CreateControl<Label>(sendingGroup, 0.5f, 2.5f, 5, 1);
		powerBar = ControlUtils.CreateControl<HScrollBar>(sendingGroup, 5.5f, 2.5f, 20.5f, 1);
		powerBar.Minimum = 0;
		powerBar.Maximum = 13;
		powerBar.SmallChange = 1;
		powerBar.LargeChange = 1;
		powerInput = ControlUtils.CreateControl<NumericUpDown>(sendingGroup, 26.5f, 2.5f, 3, 1);
		powerInput.Minimum = 0;
		powerInput.Maximum = 255;
		controllerLabel = ControlUtils.CreateControl<Label>(sendingGroup, 0.5f, 4, 5, 1);
		controllerRadioPanel = ControlUtils.CreateControl<Panel>(sendingGroup, 5.5f, 4, 18, 1);
		controllerRadioPanel.SuspendLayout();
		controllerSanyoRadio = ControlUtils.CreateControl<RadioButton>(controllerRadioPanel, 0, 0, 4, 1);
		controllerReversedSanyoRadio = ControlUtils.CreateControl<RadioButton>(controllerRadioPanel, 4, 0, 6, 1);
		controllerRyojoRadio = ControlUtils.CreateControl<RadioButton>(controllerRadioPanel, 10, 0, 4, 1);
		controllerOtherRadio = ControlUtils.CreateControl<RadioButton>(controllerRadioPanel, 14, 0, 4, 1);
		controllerRadioPanel.ResumeLayout();
		controllerPowerLabel = ControlUtils.CreateControl<Label>(sendingGroup, 23.5f, 4, 1, 1);
		controllerPowerInput = ControlUtils.CreateControl<NumericUpDown>(sendingGroup, 24.5f, 4, 2, 1);
		controllerPowerInput.Minimum = 0;
		controllerPowerInput.Maximum = 25;
		controllerBrakeLabel = ControlUtils.CreateControl<Label>(sendingGroup, 26.5f, 4, 1, 1);
		controllerBrakeInput = ControlUtils.CreateControl<NumericUpDown>(sendingGroup, 27.5f, 4, 2, 1);
		controllerBrakeInput.Minimum = 0;
		controllerBrakeInput.Maximum = 9;
		extBrakeLabel = ControlUtils.CreateControl<Label>(sendingGroup, 0.5f, 5.5f, 5, 1);
		extBrakeBar = ControlUtils.CreateControl<HScrollBar>(sendingGroup, 5.5f, 5.5f, 20.5f, 1);
		extBrakeBar.Minimum = 0;
		extBrakeBar.Maximum = 255;
		extBrakeBar.SmallChange = 1;
		extBrakeBar.LargeChange = 30;
		extBrakeInput = ControlUtils.CreateControl<NumericUpDown>(sendingGroup, 26.5f, 5.5f, 3, 1);
		extBrakeInput.Minimum = 0;
		extBrakeInput.Maximum = 255;
		sendingGroup.ResumeLayout();

		receivingGroup = ControlUtils.CreateControl<GroupBox>(mainPanel, 0.5f, 14.5f, 30, 7);
		receivingGroup.SuspendLayout();
		doorClosedCheck = ControlUtils.CreateControl<CheckBox>(receivingGroup, 0.5f, 1, 6, 1);
		doorClosedInput = ControlUtils.CreateControl<NumericUpDown>(receivingGroup, 6.5f, 1, 3, 1);
		doorClosedInput.Minimum = 0;
		doorClosedInput.Maximum = 255;
		shockLeftCheck = ControlUtils.CreateControl<CheckBox>(receivingGroup, 10.5f, 1, 6, 1);
		shockLeftInput = ControlUtils.CreateControl<NumericUpDown>(receivingGroup, 16.5f, 1, 3, 1);
		shockLeftInput.Minimum = 0;
		shockLeftInput.Maximum = 255;
		shockRightCheck = ControlUtils.CreateControl<CheckBox>(receivingGroup, 20.5f, 1, 6, 1);
		shockRightInput = ControlUtils.CreateControl<NumericUpDown>(receivingGroup, 26.5f, 1, 3, 1);
		shockRightInput.Minimum = 0;
		shockRightInput.Maximum = 255;
		ledLabel = ControlUtils.CreateControl<Label>(receivingGroup, 0.5f, 2.5f, 3, 1);
		ledBar = ControlUtils.CreateControl<HScrollBar>(receivingGroup, 3.5f, 2.5f, 22.5f, 1);
		ledBar.Minimum = 0;
		ledBar.Maximum = 10;
		ledBar.SmallChange = 1;
		ledBar.LargeChange = 1;
		ledInput = ControlUtils.CreateControl<NumericUpDown>(receivingGroup, 26.5f, 2.5f, 3, 1);
		ledInput.Minimum = 0;
		ledInput.Maximum = 255;
		atcLabel = ControlUtils.CreateControl<Label>(receivingGroup, 0.5f, 4, 3, 1);
		atcBar = ControlUtils.CreateControl<HScrollBar>(receivingGroup, 3.5f, 4, 18.5f, 1);
		atcBar.Minimum = 0;
		atcBar.Maximum = 999;
		atcBar.SmallChange = 1;
		atcBar.LargeChange = 10;
		atcOffCheck = ControlUtils.CreateControl<CheckBox>(receivingGroup, 22.5f, 4, 3, 1);
		atcInput = ControlUtils.CreateControl<NumericUpDown>(receivingGroup, 25.5f, 4, 4, 1);
		atcInput.Minimum = 0;
		atcInput.Maximum = 65535;
		speedLabel = ControlUtils.CreateControl<Label>(receivingGroup, 0.5f, 5.5f, 3, 1);
		speedBar = ControlUtils.CreateControl<HScrollBar>(receivingGroup, 3.5f, 5.5f, 18.5f, 1);
		speedBar.Minimum = 0;
		speedBar.Maximum = 999;
		speedBar.SmallChange = 1;
		speedBar.LargeChange = 10;
		speedInput = ControlUtils.CreateControl<NumericUpDown>(receivingGroup, 25.5f, 5.5f, 4, 1);
		speedInput.Minimum = 0;
		speedInput.Maximum = 65535;
		receivingGroup.ResumeLayout();

		mainPanel.ResumeLayout();
		ResumeLayout();

		languageJapaneseMenuItem.Click += LanguageMenuClickHandler;
		languageEnglishMenuItem.Click += LanguageMenuClickHandler;
		advancedConfigurationMenuItem.Click += AdvancedConfigurationMenuClickHandler;
		SetControlTexts();
	}

	private void SetControlTexts()
	{
		this.Text = uiText.AppTitle + versionString;

		configurationMenuItem.Text = uiText.Configuration;
		advancedConfigurationMenuItem.Text = uiText.AdvancedConfiguration;

		networkGroup.Text = uiText.Network;
		networkOffRadio.Text = uiText.NetworkOff;
		networkSendRadio.Text = uiText.NetworkSend;
		networkReceiveRadio.Text = uiText.NetworkReceive;
		networkPeerAddressLabel.Text = uiText.NetworkPeerAddress;
		networkPortLabel.Text = uiText.NetworkPort;
		preferIPv6Check.Text = uiText.NetworkPreferIPv6;
		networkLastReceiveLabel.Text = uiText.NetworkLastReceive;
		openKeySendWindowButton.Text = uiText.OpenKeySendWindow;

		sendingGroup.Text = uiText.SendingData;
		brakeLabel.Text = uiText.Brake;
		powerLabel.Text = uiText.Power;
		controllerLabel.Text = uiText.Controller;
		controllerSanyoRadio.Text = uiText.Sanyo;
		controllerReversedSanyoRadio.Text = uiText.ReversedSanyo;
		controllerRyojoRadio.Text = uiText.Ryojo;
		controllerOtherRadio.Text = uiText.OtherController;
		controllerPowerLabel.Text = uiText.ControllerPower;
		controllerBrakeLabel.Text = uiText.ControllerBrake;
		extBrakeLabel.Text = uiText.ExtendedBrake;

		receivingGroup.Text = uiText.ReceivingData;
		doorClosedCheck.Text = uiText.DoorClosed;
		shockLeftCheck.Text = uiText.ShockLeft;
		shockRightCheck.Text = uiText.ShockRight;
		ledLabel.Text = uiText.LED;
		atcLabel.Text = uiText.ATC;
		atcOffCheck.Text = uiText.ATCOff;
		speedLabel.Text = uiText.Speed;
	}

	private void LanguageMenuClickHandler(object sender, EventArgs e)
	{
		if (sender == languageJapaneseMenuItem)
		{
			languageJapaneseMenuItem.Checked = true;
			languageEnglishMenuItem.Checked = false;
			uiText = new JapaneseUIText();
		}
		else if (sender == languageEnglishMenuItem)
		{
			languageJapaneseMenuItem.Checked = false;
			languageEnglishMenuItem.Checked = true;
			uiText = new EnglishUIText();
		}
		SetControlTexts();
	}

	private void AdvancedConfigurationMenuClickHandler(object sender, EventArgs e)
	{
		AdvancedConfiguration dialog = new AdvancedConfiguration(uiText);
		DialogResult result = dialog.ShowDialog();
	}
}
