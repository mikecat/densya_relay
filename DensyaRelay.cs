using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

class DensyaRelay: Form
{
	public const int MMF_SIZE = 64;
	public const int MMF_RECEIVED_DATA_START = 10;

	public static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new DensyaRelay());
	}

	private readonly static string LanguageValueName = "language";
	private readonly static string LanguageJapaneseData = "Japanese";
	private readonly static string LanguageEnglishData = "English";

	private readonly static string PeerAddressValueName = "peerAddress";
	private readonly static string DestinationPortValueName = "destinationPort";
	private readonly static string PreferIPv6ValueName = "preferIPv6";

	private readonly static string LocalPortValueName = "localPort";
	private readonly static string LocalPortSameAsDestinationPortValueName = "localPortSameAsDestinationPort";
	private readonly static string SendDataSizeValueName = "sendDataSize";
	private readonly static string ReceiveDataSizeValueName = "receiveDataSize";
	private readonly static string MmfNameValueName = "mmfName";
	private readonly static string CreateMutexValueName = "createMutex";
	private readonly static string LockMutexValueName = "lockMutex";
	private readonly static string MutexNameValueName = "mutexName";

	private int localPort = new Random().Next(49152, 65535 + 1);
	private bool localPortSameAsDestinationPort = true;
	private int sendDataSize = 4;
	private int receiveDataSize = 8;
	private string mmfName = "denconvMMF";
	private bool createMutex = false;
	private bool lockMutex = false;
	private string mutexName = "TSXMUTEX";

	private UdpClient udpClient = null;
	private bool isSenderMode = false;
	private MemoryMappedFile mmf = null;
	private MemoryMappedViewAccessor mmfView = null;
	private Mutex mutex = null;
	private System.Windows.Forms.Timer mmfPollingTimer;
	private SendKeyWindow sendKeyWindow = null;
	private ushort? dataSentSerial = null;
	private ushort? dataReceivedSerial = null;
	private byte[] prevData = null;
	private SortedSet<int> pressedKeySet = new SortedSet<int>();
	private Stopwatch stopwatchFromLastSend = new Stopwatch();
	private Stopwatch stopwatchFromLastReceived = new Stopwatch();

	private UIText uiText;
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
	private Label networkLastReceiveTimeLabel;
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
		this.Icon = ControlUtils.Icon;
		SuspendLayout();

		mainMenuStrip = new MenuStrip();
		languageMenuItem = new ToolStripMenuItem();
		languageMenuItem.Text = "言語 / Language (&L)";
		languageJapaneseMenuItem = new ToolStripMenuItem();
		languageJapaneseMenuItem.Text = "日本語 (&J)";
		languageJapaneseMenuItem.Checked = true;
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
		networkOffRadio.Checked = true;
		networkOffRadio.AutoCheck = false;
		networkSendRadio = ControlUtils.CreateControl<RadioButton>(networkModeRadioPanel, 4, 0, 9, 1);
		networkSendRadio.AutoCheck = false;
		networkReceiveRadio = ControlUtils.CreateControl<RadioButton>(networkModeRadioPanel, 13, 0, 9, 1);
		networkReceiveRadio.AutoCheck = false;
		networkModeRadioPanel.ResumeLayout();
		networkPeerAddressLabel = ControlUtils.CreateControl<Label>(networkGroup, 0.5f, 2.5f, 7, 1);
		networkPeerAddressInput = ControlUtils.CreateControl<TextBox>(networkGroup, 7.5f, 2.5f, 8.5f, 1);
		networkPortLabel = ControlUtils.CreateControl<Label>(networkGroup, 16, 2.5f, 3, 1);
		networkPortInput = ControlUtils.CreateControl<NumericUpDown>(networkGroup, 19, 2.5f, 4, 1);
		networkPortInput.Minimum = 0;
		networkPortInput.Maximum = 65535;
		networkPortInput.Value = localPort;
		preferIPv6Check = ControlUtils.CreateControl<CheckBox>(networkGroup, 23.5f, 2.5f, 5.5f, 1);
		networkLastReceiveLabel = ControlUtils.CreateControl<Label>(networkGroup, 0.5f, 4, 6, 1.5f);
		networkLastReceiveTimeLabel = ControlUtils.CreateControl<Label>(networkGroup, 6.5f, 4, 13, 1.5f);
		openKeySendWindowButton = ControlUtils.CreateControl<Button>(networkGroup, 19.5f, 4, 10, 1.5f);
		openKeySendWindowButton.Enabled = false;
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
		controllerOtherRadio.Checked = true;
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
		extBrakeBar.LargeChange = 1;
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
		atcBar.LargeChange = 1;
		atcOffCheck = ControlUtils.CreateControl<CheckBox>(receivingGroup, 22.5f, 4, 3, 1);
		atcInput = ControlUtils.CreateControl<NumericUpDown>(receivingGroup, 25.5f, 4, 4, 1);
		atcInput.Minimum = 0;
		atcInput.Maximum = 65535;
		speedLabel = ControlUtils.CreateControl<Label>(receivingGroup, 0.5f, 5.5f, 3, 1);
		speedBar = ControlUtils.CreateControl<HScrollBar>(receivingGroup, 3.5f, 5.5f, 18.5f, 1);
		speedBar.Minimum = 0;
		speedBar.Maximum = 999;
		speedBar.SmallChange = 1;
		speedBar.LargeChange = 1;
		speedInput = ControlUtils.CreateControl<NumericUpDown>(receivingGroup, 25.5f, 5.5f, 4, 1);
		speedInput.Minimum = 0;
		speedInput.Maximum = 65535;
		receivingGroup.ResumeLayout();

		mainPanel.ResumeLayout();
		ResumeLayout();

		languageJapaneseMenuItem.Click += LanguageMenuClickHandler;
		languageEnglishMenuItem.Click += LanguageMenuClickHandler;
		advancedConfigurationMenuItem.Click += AdvancedConfigurationMenuClickHandler;

		Load += LoadHandler;
		FormClosed += FormClosedHandler;
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

	private void LoadHandler(object sender, EventArgs e)
	{
		RegistryIO regIO = RegistryIO.OpenForRead();
		if (regIO != null)
		{
			string languageData = regIO.GetStringValue(LanguageValueName);
			if (languageData == LanguageEnglishData)
			{
				languageJapaneseMenuItem.Checked = false;
				languageEnglishMenuItem.Checked = true;
			}
			else if (languageData == LanguageJapaneseData)
			{
				languageJapaneseMenuItem.Checked = true;
				languageEnglishMenuItem.Checked = false;
			}
			string peerAddressData = regIO.GetStringValue(PeerAddressValueName);
			if (peerAddressData != null) {
				networkPeerAddressInput.Text = peerAddressData;
			}
			int? destinationPortData = regIO.GetIntValue(DestinationPortValueName);
			if (destinationPortData.HasValue)
			{
				networkPortInput.Value = Math.Min(Math.Max(destinationPortData.Value, 0), 65535);
			}
			int? preferIPv6Data = regIO.GetIntValue(PreferIPv6ValueName);
			if (preferIPv6Data.HasValue)
			{
				preferIPv6Check.Checked = preferIPv6Data.Value != 0;
			}

			int? localPortData = regIO.GetIntValue(LocalPortValueName);
			if (localPortData.HasValue)
			{
				localPort = Math.Min(Math.Max(localPortData.Value, 0), 65535);
			}
			int? localPortSameAsDestinationPortData = regIO.GetIntValue(LocalPortSameAsDestinationPortValueName);
			if (localPortSameAsDestinationPortData.HasValue)
			{
				localPortSameAsDestinationPort = localPortSameAsDestinationPortData.Value != 0;
			}
			int? sendDataSizeData = regIO.GetIntValue(SendDataSizeValueName);
			if (sendDataSizeData.HasValue)
			{
				sendDataSize = Math.Min(Math.Max(sendDataSizeData.Value, 0), MMF_RECEIVED_DATA_START);
			}
			int? receiveDataSizeData = regIO.GetIntValue(ReceiveDataSizeValueName);
			if (receiveDataSizeData.HasValue)
			{
				receiveDataSize = Math.Min(Math.Max(receiveDataSizeData.Value, 0), MMF_SIZE - MMF_RECEIVED_DATA_START);
			}
			string mmfNameData = regIO.GetStringValue(MmfNameValueName);
			if (mmfNameData != null)
			{
				mmfName = mmfNameData;
			}
			int? createMutexData = regIO.GetIntValue(CreateMutexValueName);
			if (createMutexData.HasValue)
			{
				createMutex = createMutexData.Value != 0;
			}
			int? lockMutexData = regIO.GetIntValue(LockMutexValueName);
			if (lockMutexData.HasValue)
			{
				lockMutex = lockMutexData.Value != 0;
			}
			string mutexNameData = regIO.GetStringValue(MutexNameValueName);
			if (mutexNameData != null)
			{
				mutexName = mutexNameData;
			}

			regIO.Close();
		}

		if (languageEnglishMenuItem.Checked)
		{
			uiText = new EnglishUIText();
		}
		else
		{
			uiText = new JapaneseUIText();
		}
		SetControlTexts();

		mmf = MemoryMappedFile.CreateOrOpen(mmfName, MMF_SIZE);
		mmfView = mmf.CreateViewAccessor();
		if (createMutex)
		{
			mutex = new Mutex(false, mutexName);
		}

		networkOffRadio.Click += NetworkOffRadioClickHandler;
		networkSendRadio.Click += NetworkConnectRadioClickHandler;
		networkReceiveRadio.Click += NetworkConnectRadioClickHandler;
		openKeySendWindowButton.Click += OpenKeySendWindowButtonClickHandler;

		brakeBar.Scroll += BrakeBarScrollHandler;
		brakeInput.ValueChanged += BrakeInputValueChangedHandler;
		powerBar.Scroll += PowerBarScrollHandler;
		powerInput.ValueChanged += PowerInputValueChangedHandler;
		controllerSanyoRadio.Click += ControllerRadioClickHandler;
		controllerReversedSanyoRadio.Click += ControllerRadioClickHandler;
		controllerRyojoRadio.Click += ControllerRadioClickHandler;
		controllerOtherRadio.Click += ControllerRadioClickHandler;
		controllerPowerInput.ValueChanged += ControllerInputValueChangedHandler;
		controllerBrakeInput.ValueChanged += ControllerInputValueChangedHandler;
		extBrakeBar.Scroll += ExtBrakeBarScrollHandler;
		extBrakeInput.ValueChanged += ExtBrakeInputValueChangedHandler;

		doorClosedCheck.Click += DoorClosedCheckClickHandler;
		doorClosedInput.ValueChanged += DoorClosedInputValueChangedHandler;
		shockLeftCheck.Click += ShockLeftCheckClickHandler;
		shockLeftInput.ValueChanged += ShockLeftInputValueChangedHandler;
		shockRightCheck.Click += ShockRightCheckClickHandler;
		shockRightInput.ValueChanged += ShockRightInputValueChangedHandler;
		ledBar.Scroll += LedBarScrollHandler;
		ledInput.ValueChanged += LedInputValueChangedHandler;
		atcBar.Scroll += AtcBarScrollHandler;
		atcOffCheck.Click += AtcOffCheckClickHandler;
		atcInput.ValueChanged += AtcInputValueChangedHandler;
		speedBar.Scroll += SpeedBarScrollHandler;
		speedInput.ValueChanged += SpeedInputValueChangedHandler;

		mmfPollingTimer = new System.Windows.Forms.Timer();
		mmfPollingTimer.Interval = 15;
		mmfPollingTimer.Tick += MmfPollingTimerTickHandler;
		mmfPollingTimer.Start();
	}

	private void FormClosedHandler(object sender, EventArgs e)
	{
		ClearKeyControl();
		RegistryIO regIO = RegistryIO.OpenForWrite();
		if (regIO != null)
		{
			if (languageJapaneseMenuItem.Checked)
			{
				regIO.SetValue(LanguageValueName, LanguageJapaneseData);
			}
			else if (languageEnglishMenuItem.Checked)
			{
				regIO.SetValue(LanguageValueName, LanguageEnglishData);
			}
			regIO.SetValue(PeerAddressValueName, networkPeerAddressInput.Text);
			regIO.SetValue(DestinationPortValueName, (int)networkPortInput.Value);
			regIO.SetValue(PreferIPv6ValueName, preferIPv6Check.Checked ? 1 : 0);
			regIO.Close();
		}

		if (udpClient != null)
		{
			udpClient.Close();
			udpClient.Dispose();
			udpClient = null;
		}
		if (mmfPollingTimer != null) mmfPollingTimer.Stop();
		if (mutex != null) mutex.Dispose();
		mutex = null;
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
		if (sendKeyWindow != null) sendKeyWindow.UpdateUIText(uiText);
	}

	private void AdvancedConfigurationMenuClickHandler(object sender, EventArgs e)
	{
		bool enablePortConfiguration = udpClient == null;
		AdvancedConfiguration dialog = new AdvancedConfiguration(uiText, enablePortConfiguration);
		dialog.LocalPort = localPort;
		dialog.LocalPortSameAsDestinationPort = localPortSameAsDestinationPort;
		dialog.SendSize = sendDataSize;
		dialog.ReceiveSize = receiveDataSize;
		dialog.MmfName = mmfName;
		dialog.CreateMutex = createMutex;
		dialog.LockMutex = lockMutex;
		dialog.MutexName = mutexName;
		DialogResult result = dialog.ShowDialog();
		if (result == DialogResult.OK)
		{
			string oldMmfName = mmfName;
			string oldMutexName = mutexName;

			if (enablePortConfiguration)
			{
				localPort = dialog.LocalPort;
				localPortSameAsDestinationPort = dialog.LocalPortSameAsDestinationPort;
			}
			sendDataSize = dialog.SendSize;
			receiveDataSize = dialog.ReceiveSize;
			mmfName = dialog.MmfName;
			createMutex = dialog.CreateMutex;
			lockMutex = dialog.LockMutex;
			mutexName = dialog.MutexName;

			if (mmfName != oldMmfName)
			{
				if (mmf != null) mmf.Dispose();
				mmf = MemoryMappedFile.CreateOrOpen(mmfName, MMF_SIZE);
				mmfView = mmf.CreateViewAccessor();
			}
			if (createMutex)
			{
				if (mutex == null || mutexName != oldMutexName)
				{
					if (mutex != null) mutex.Dispose();
					mutex = new Mutex(false, mutexName);
				}
			}
			else
			{
				if (mutex != null) mutex.Dispose();
				mutex = null;
			}

			RegistryIO regIO = RegistryIO.OpenForWrite();
			if (regIO != null)
			{
				regIO.SetValue(LocalPortValueName, localPort);
				regIO.SetValue(LocalPortSameAsDestinationPortValueName, localPortSameAsDestinationPort ? 1 : 0);
				regIO.SetValue(SendDataSizeValueName, sendDataSize);
				regIO.SetValue(ReceiveDataSizeValueName, receiveDataSize);
				regIO.SetValue(MmfNameValueName, mmfName);
				regIO.SetValue(CreateMutexValueName, createMutex ? 1 : 0);
				regIO.SetValue(LockMutexValueName, lockMutex ? 1 : 0);
				regIO.SetValue(MutexNameValueName, mutexName);
				regIO.Close();
			}
		}
		dialog.Dispose();
	}

	private const uint INPUT_KEYBOARD = 1;
	private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
	private const uint KEYEVENTF_KEYUP = 0x0002;
	private struct Input
	{
		public UIntPtr type; // リトルエンディアンを仮定する
		public ushort wVk;
		public ushort wScan;
		public uint dwFlags;
		public uint time;
		public UIntPtr dwExtraInfo;
		// パディングの値は使用しないので、「使用されていない」警告を抑制する
		#pragma warning disable 0169
		private int padding1, padding2;
		#pragma warning restore 0169
	}

	[DllImport("User32.dll", EntryPoint="SendInput")]
	private static extern uint SendInput(uint cInputs, [In] Input[] pInputs, int cbSize);

	private void UpdateKeyStatus(SortedSet<int> newPressedKeySet)
	{
		List<Input> deltaList = new List<Input>();
		foreach (int pressedKey in pressedKeySet)
		{
			if (!newPressedKeySet.Contains(pressedKey))
			{
				deltaList.Add(new Input {
					type = (UIntPtr)INPUT_KEYBOARD,
					wVk = (ushort)((pressedKey >> 16) & 0xff),
					wScan = (ushort)((pressedKey >> 8) & 0xff),
					dwFlags = KEYEVENTF_KEYUP | ((pressedKey & 1) != 0 ? KEYEVENTF_EXTENDEDKEY : 0),
					time = 0,
					dwExtraInfo = UIntPtr.Zero,
				});
			}
		}
		foreach (int pressedKey in newPressedKeySet)
		{
			if (!pressedKeySet.Contains(pressedKey))
			{
				deltaList.Add(new Input {
					type = (UIntPtr)INPUT_KEYBOARD,
					wVk = (ushort)((pressedKey >> 16) & 0xff),
					wScan = (ushort)((pressedKey >> 8) & 0xff),
					dwFlags = (pressedKey & 1) != 0 ? KEYEVENTF_EXTENDEDKEY : 0,
					time = 0,
					dwExtraInfo = UIntPtr.Zero,
				});
			}
		}
		if (deltaList.Count > 0)
		{
			SendInput((uint)deltaList.Count, deltaList.ToArray(), Marshal.SizeOf(deltaList[0]));
		}
		pressedKeySet = newPressedKeySet;
	}

	private void ClearKeyControl()
	{
		if (udpClient != null)
		{
			if (isSenderMode)
			{
				pressedKeySet.Clear();
				SendSenderMessage(prevData);
			}
			else
			{
				UpdateKeyStatus(new SortedSet<int>());
			}
		}
	}

	private void NetworkOffRadioClickHandler(object sender, EventArgs e)
	{
		ClearKeyControl();
		if (udpClient != null)
		{
			udpClient.Close();
			udpClient.Dispose();
			udpClient = null;
		}
		if (sendKeyWindow != null)
		{
			sendKeyWindow.Close();
			sendKeyWindow = null;
		}
		stopwatchFromLastSend.Stop();
		stopwatchFromLastReceived.Stop();
		networkOffRadio.Checked = true;
		networkSendRadio.Checked = false;
		networkReceiveRadio.Checked = false;
		networkPeerAddressInput.Enabled = true;
		networkPortInput.Enabled = true;
		preferIPv6Check.Enabled = true;
		openKeySendWindowButton.Enabled = false;
	}

	private void NetworkConnectRadioClickHandler(object sender, EventArgs e)
	{
		bool newSenderMode = sender == networkSendRadio;
		if (udpClient == null || newSenderMode != isSenderMode)
		{
			ClearKeyControl();
			dataSentSerial = newSenderMode ? 0 : (ushort?)null;
			dataReceivedSerial = newSenderMode ? (ushort?)null : 0;
			prevData = null;
			stopwatchFromLastSend.Stop();
			stopwatchFromLastReceived.Stop();
		}
		if (udpClient == null)
		{
			string destinationHostName = networkPeerAddressInput.Text;
			if (destinationHostName == "")
			{
				MessageBox.Show(
					this,
					uiText.DestinationAddressNotSetMessage,
					uiText.DestinationAddressNotSetTitle,
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);
				return;
			}
			UdpClient newUdpClient = null;
			int destinationPort = (int)networkPortInput.Value;
			int localPortToUse = localPortSameAsDestinationPort ? destinationPort : localPort;
			for (int i = 0; i < 2 && newUdpClient == null; i++)
			{
				try
				{
					newUdpClient = new UdpClient(
						localPortToUse,
						i == (preferIPv6Check.Checked ? 0 : 1)
							? AddressFamily.InterNetworkV6
							: AddressFamily.InterNetwork
					);
					newUdpClient.Connect(destinationHostName, destinationPort);
				}
				catch (Exception)
				{
					if (newUdpClient != null) newUdpClient.Dispose();
					newUdpClient = null;
				}
			}
			if (newUdpClient == null)
			{
				MessageBox.Show(
					this,
					uiText.ConnectionErrorMessage,
					uiText.ConnectionErrorTitle,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				return;
			}
			udpClient = newUdpClient;
			newUdpClient.ReceiveAsync().ContinueWith(OnUdpClientReceive);
		}
		networkPeerAddressInput.Enabled = false;
		networkPortInput.Enabled = false;
		preferIPv6Check.Enabled = false;
		isSenderMode = newSenderMode;
		networkOffRadio.Checked = false;
		networkSendRadio.Checked = false;
		networkReceiveRadio.Checked = false;
		((RadioButton)sender).Checked = true;
		openKeySendWindowButton.Enabled = isSenderMode && sendKeyWindow == null;
		if (!isSenderMode && sendKeyWindow != null)
		{
			sendKeyWindow.Close();
			sendKeyWindow = null;
		}
	}

	private void OpenKeySendWindowButtonClickHandler(object sender, EventArgs e)
	{
		if (udpClient != null && isSenderMode && sendKeyWindow == null)
		{
			openKeySendWindowButton.Enabled = false;
			sendKeyWindow = new SendKeyWindow(uiText);
			sendKeyWindow.Closed += SendKeyWindowClosedHandler;
			sendKeyWindow.Leave += SendKeyWindowLeaveHandler;
			sendKeyWindow.KeyMessage += SendKeyWindowKeyMessageHandler;
			sendKeyWindow.Owner = this;
			sendKeyWindow.Show();
		}
	}

	private void SendKeyWindowClosedHandler(object sender, EventArgs e)
	{
		openKeySendWindowButton.Enabled = udpClient != null && isSenderMode;
		sendKeyWindow.Dispose();
		sendKeyWindow = null;
		if (udpClient != null && isSenderMode && pressedKeySet.Count > 0)
		{
			pressedKeySet.Clear();
			SendSenderMessage(prevData);
		}
	}

	private void SendKeyWindowLeaveHandler(object sender, EventArgs e)
	{
		if (udpClient != null && isSenderMode && pressedKeySet.Count > 0)
		{
			pressedKeySet.Clear();
			SendSenderMessage(prevData);
		}
	}

	private void SendKeyWindowKeyMessageHandler(byte keyCode, byte scanCode, bool isExtKey, bool pressed)
	{
		if (udpClient != null && isSenderMode)
		{
			int code = (keyCode << 16) | (scanCode << 8) | (isExtKey ? 1 : 0);
			if (pressed)
			{
				if (!pressedKeySet.Contains(code))
				{
					pressedKeySet.Add(code);
					SendSenderMessage(prevData);
				}
			}
			else
			{
				if (pressedKeySet.Contains(code))
				{
					pressedKeySet.Remove(code);
					SendSenderMessage(prevData);
				}
			}
		}
	}

	private void SendSenderMessage(byte[] currentData)
	{
		if (currentData == null) currentData = new byte[0];
		int numKeySet = Math.Min(pressedKeySet.Count, 255);
		byte[] message = new byte[5 + currentData.Length + 3 * numKeySet];
		message[0] = 0x00;
		message[1] = (byte)(dataSentSerial.Value >> 8);
		message[2] = (byte)dataSentSerial.Value;
		message[3] = (byte)currentData.Length;
		currentData.CopyTo(message, 4);
		message[currentData.Length + 4] = (byte)numKeySet;
		int keyCount = 0;
		foreach(int keyInfo in pressedKeySet)
		{
			if (keyCount > 255) break;
			int offset = currentData.Length + 5 + 3 * keyCount;
			message[offset + 0] = (byte)(keyInfo >> 16);
			message[offset + 1] = (byte)(keyInfo >> 8);
			message[offset + 2] = (byte)((keyInfo & 0xff) != 0 ? 1 : 0);
			keyCount++;
		}
		dataSentSerial++;
		SendUdpMessage(message);
	}

	private void OnUdpClientReceive(Task<UdpReceiveResult> task)
	{
		if (task.Status == TaskStatus.RanToCompletion)
		{
			stopwatchFromLastReceived.Restart();
			OnUdpMessageReceive(task.Result.Buffer);
		}
		if (udpClient != null) udpClient.ReceiveAsync().ContinueWith(OnUdpClientReceive);
	}

	private void SendUdpMessage(byte[] message)
	{
		if (udpClient != null)
		{
			udpClient.Send(message, message.Length);
			stopwatchFromLastSend.Restart();
		}
	}

	private void OnUdpMessageReceive(byte[] message)
	{
		if (udpClient == null) return; // 通信がオフの場合、処理をしない
		if (message.Length < 1) return; // メッセージの種類すら無い場合、処理をしない
		if (isSenderMode)
		{
			// 送信モードなので、受信した受信データを処理する
			if (message[0] == 0x01 && message.Length >= 4)
			{
				ushort serial = (ushort)(((ushort)message[1] << 8) | message[2]);
				if (!IsValidSerial(dataReceivedSerial, serial)) return;
				int payloadLength = message[3];
				if (message.Length < payloadLength + 4) return;
				WriteBytesToMMF(
					MMF_RECEIVED_DATA_START,
					message,
					4,
					Math.Min(payloadLength, MMF_SIZE - MMF_RECEIVED_DATA_START)
				);
				dataReceivedSerial = serial;
			}
		}
		else
		{
			// 受信モードなので、受信した送信データを処理する
			if (message[0] == 0x00 && message.Length >= 4)
			{
				ushort serial = (ushort)(((ushort)message[1] << 8) | message[2]);
				if (!IsValidSerial(dataSentSerial, serial)) return;
				int payloadLength = message[3];
				if (message.Length < payloadLength + 5) return;
				int keyDataNum = message[payloadLength + 4];
				if (message.Length < payloadLength + 5 + 3 * keyDataNum) return;
				WriteBytesToMMF(0, message, 4, Math.Min(payloadLength, MMF_RECEIVED_DATA_START));
				SortedSet<int> newPressedKeys = new SortedSet<int>();
				for (int i = 0; i < keyDataNum; i++)
				{
					int offset = payloadLength + 5 + 3 * i;
					newPressedKeys.Add(
						(message[offset + 0] << 16) |
						(message[offset + 1] << 8) |
						(message[offset + 2] & 1)
					);
				}
				UpdateKeyStatus(newPressedKeys);
				dataSentSerial = serial;
			}
		}
	}

	private static bool IsValidSerial(ushort? nextSerial, ushort receivedSerial)
	{
		if (!nextSerial.HasValue) return true;
		int invalidMin = (int)nextSerial.Value - 0x4000;
		int invalidMax = nextSerial.Value;
		int valueToCheck = (int)receivedSerial;
		if (invalidMin <= valueToCheck && valueToCheck <= invalidMax) return false;
		valueToCheck -= 0x10000;
		if (invalidMin <= valueToCheck && valueToCheck <= invalidMax) return false;
		return true;
	}

	private void WriteBytesToMMF(int startIndex, byte[] data, int offset, int length)
	{
		Mutex mutexToLock = lockMutex ? mutex : null;
		if (mutexToLock != null)
		{
			try
			{
				mutexToLock.WaitOne();
			}
			catch (AbandonedMutexException)
			{
				// 握りつぶす
			}
		}
		mmfView.WriteArray<byte>(startIndex, data, offset, length);
		if (mutexToLock != null) mutexToLock.ReleaseMutex();
	}

	private void WriteByteToMMF(int idx, int value)
	{
		WriteBytesToMMF(idx, new byte[]{ (byte)value }, 0, 1);
	}

	private void WriteWordToMMF(int idx, int value)
	{
		byte[] dataToWrite = new byte[] { (byte)value, (byte)(value >> 8) };
		WriteBytesToMMF(idx, dataToWrite, 0, 2);
	}

	private void BrakeBarScrollHandler(object sender, EventArgs e)
	{
		brakeInput.Value = brakeBar.Value;
	}

	private void BrakeInputValueChangedHandler(object sender, EventArgs e)
	{
		brakeBar.Value = Math.Min((int)brakeInput.Value, brakeBar.Maximum);
		WriteByteToMMF(0, (int)brakeInput.Value);
	}

	private void PowerBarScrollHandler(object sender, EventArgs e)
	{
		powerInput.Value = powerBar.Value;
	}

	private void PowerInputValueChangedHandler(object sender, EventArgs e)
	{
		powerBar.Value = Math.Min((int)powerInput.Value, powerBar.Maximum);
		WriteByteToMMF(1, (int)powerInput.Value);
	}

	private void ControllerRadioClickHandler(object sender, EventArgs e)
	{
		if (controllerSanyoRadio.Checked)
		{
			controllerPowerInput.Value = 0;
			controllerBrakeInput.Value = 1;
		}
		else if (controllerReversedSanyoRadio.Checked)
		{
			controllerPowerInput.Value = 0;
			controllerBrakeInput.Value = 2;
		}
		else if (controllerRyojoRadio.Checked)
		{
			controllerPowerInput.Value = 0;
			controllerBrakeInput.Value = 3;
		}
	}

	private void ControllerInputValueChangedHandler(object sender, EventArgs e)
	{
		int controllerValue = (int)(controllerPowerInput.Value * 10 + controllerBrakeInput.Value);
		if (controllerValue > 255) controllerValue = 255;
		switch (controllerValue)
		{
			case 1: controllerSanyoRadio.Checked = true; break;
			case 2: controllerReversedSanyoRadio.Checked = true; break;
			case 3: controllerRyojoRadio.Checked = true; break;
			default: controllerOtherRadio.Checked = true; break;
		}
		WriteByteToMMF(2, controllerValue);
	}

	private void ExtBrakeBarScrollHandler(object sender, EventArgs e)
	{
		extBrakeInput.Value = extBrakeBar.Value;
	}

	private void ExtBrakeInputValueChangedHandler(object sender, EventArgs e)
	{
		extBrakeBar.Value = Math.Min((int)extBrakeInput.Value, extBrakeBar.Maximum);
		WriteByteToMMF(3, (int)extBrakeInput.Value);
	}

	private void DoorClosedCheckClickHandler(object sender, EventArgs e)
	{
		doorClosedInput.Value = doorClosedCheck.Checked ? 1 : 0;
	}

	private void DoorClosedInputValueChangedHandler(object sender, EventArgs e)
	{
		doorClosedCheck.Checked = doorClosedInput.Value == 1;
		WriteByteToMMF(MMF_RECEIVED_DATA_START, (int)doorClosedInput.Value);
	}

	private void ShockLeftCheckClickHandler(object sender, EventArgs e)
	{
		shockLeftInput.Value = shockLeftCheck.Checked ? 1 : 0;
	}

	private void ShockLeftInputValueChangedHandler(object sender, EventArgs e)
	{
		shockLeftCheck.Checked = shockLeftInput.Value == 1;
		WriteByteToMMF(MMF_RECEIVED_DATA_START + 1, (int)shockLeftInput.Value);
	}

	private void ShockRightCheckClickHandler(object sender, EventArgs e)
	{
		shockRightInput.Value = shockRightCheck.Checked ? 1 : 0;
	}

	private void ShockRightInputValueChangedHandler(object sender, EventArgs e)
	{
		shockRightCheck.Checked = shockRightInput.Value == 1;
		WriteByteToMMF(MMF_RECEIVED_DATA_START + 2, (int)shockRightInput.Value);
	}

	private void LedBarScrollHandler(object sender, EventArgs e)
	{
		ledInput.Value = ledBar.Value;
	}

	private void LedInputValueChangedHandler(object sender, EventArgs e)
	{
		ledBar.Value = Math.Min((int)ledInput.Value, ledBar.Maximum);
		WriteByteToMMF(MMF_RECEIVED_DATA_START + 3, (int)ledInput.Value);
	}

	private void AtcBarScrollHandler(object sender, EventArgs e)
	{
		if (!atcOffCheck.Checked) atcInput.Value = atcBar.Value;
	}

	private void AtcOffCheckClickHandler(object sender, EventArgs e)
	{
		atcInput.Value = atcOffCheck.Checked ? 65535 : atcBar.Value;
	}

	private void AtcInputValueChangedHandler(object sender, EventArgs e)
	{
		if (atcInput.Value <= atcBar.Maximum) atcBar.Value = (int)atcInput.Value;
		atcOffCheck.Checked = atcInput.Value > 999;
		WriteWordToMMF(MMF_RECEIVED_DATA_START + 4, (int)atcInput.Value);
	}

	private void SpeedBarScrollHandler(object sender, EventArgs e)
	{
		speedInput.Value = speedBar.Value;
	}

	private void SpeedInputValueChangedHandler(object sender, EventArgs e)
	{
		speedBar.Value = Math.Min((int)speedInput.Value, speedBar.Maximum);
		WriteWordToMMF(MMF_RECEIVED_DATA_START + 6, (int)speedInput.Value);
	}

	private void MmfPollingTimerTickHandler(object sender, EventArgs e)
	{
		Mutex mutexToLock = lockMutex ? mutex : null;
		if (mutexToLock != null)
		{
			try
			{
				mutexToLock.WaitOne();
			}
			catch (AbandonedMutexException)
			{
				// 握りつぶす
			}
		}
		byte[] mmfData = new byte[MMF_SIZE];
		mmfView.ReadArray<byte>(0, mmfData, 0, MMF_SIZE);
		if (mutexToLock != null) mutexToLock.ReleaseMutex();

		brakeInput.Value = mmfData[0];
		powerInput.Value = mmfData[1];
		controllerPowerInput.Value = mmfData[2] / 10;
		controllerBrakeInput.Value = mmfData[2] % 10;
		extBrakeInput.Value = mmfData[3];

		doorClosedInput.Value = mmfData[MMF_RECEIVED_DATA_START];
		shockLeftInput.Value = mmfData[MMF_RECEIVED_DATA_START + 1];
		shockRightInput.Value = mmfData[MMF_RECEIVED_DATA_START + 2];
		ledInput.Value = mmfData[MMF_RECEIVED_DATA_START + 3];
		atcInput.Value = mmfData[MMF_RECEIVED_DATA_START + 4] + mmfData[MMF_RECEIVED_DATA_START + 5] * 256;
		speedInput.Value = mmfData[MMF_RECEIVED_DATA_START + 6] + mmfData[MMF_RECEIVED_DATA_START + 7] * 256;

		if (udpClient != null)
		{
			int dataToSendOffset, dataToSendSize;
			if (isSenderMode)
			{
				dataToSendOffset = 0;
				dataToSendSize = Math.Min(sendDataSize, MMF_RECEIVED_DATA_START);
			}
			else
			{
				dataToSendOffset = MMF_RECEIVED_DATA_START;
				dataToSendSize = Math.Min(receiveDataSize, MMF_SIZE - MMF_RECEIVED_DATA_START);
			}
			byte[] currentData = mmfData.Skip(dataToSendOffset).Take(dataToSendSize).ToArray();
			if (prevData == null || !currentData.SequenceEqual(prevData) || stopwatchFromLastSend.ElapsedMilliseconds >= 1000)
			{
				if (isSenderMode)
				{
					SendSenderMessage(currentData);
				}
				else
				{
					byte[] message = new byte[4 + currentData.Length];
					message[0] = 0x01;
					message[1] = (byte)(dataReceivedSerial.Value >> 8);
					message[2] = (byte)dataReceivedSerial.Value;
					message[3] = (byte)currentData.Length;
					currentData.CopyTo(message, 4);
					dataReceivedSerial++;
					SendUdpMessage(message);
				}
				prevData = currentData;
			}
			if (stopwatchFromLastReceived.IsRunning)
			{
				long elapsedSeconds = stopwatchFromLastReceived.ElapsedMilliseconds / 1000;
				if (elapsedSeconds < 5)
				{
					networkLastReceiveTimeLabel.Text = uiText.LastReceiveFewSeconds;
				}
				else if (elapsedSeconds < 60)
				{
					networkLastReceiveTimeLabel.Text = string.Format(
						"{0}{1}{2}",
						uiText.LastReceiveSecondsPrefix,
						elapsedSeconds,
						uiText.LastReceiveSecondsSuffix
					);
				}
				else
				{
					networkLastReceiveTimeLabel.Text = uiText.LastReceiveOneMinute;
				}
			}
			else
			{
				networkLastReceiveTimeLabel.Text = "";
			}
		}
		else
		{
			networkLastReceiveTimeLabel.Text = "";
		}
	}
}
