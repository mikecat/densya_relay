class EnglishUIText: UIText
{
	public override string AppTitle { get { return "Densya Relay"; }}

	public override string Configuration { get { return "&Configuration"; }}
	public override string AdvancedConfiguration { get { return "&Advanced Configuration"; }}

	public override string Network { get { return "Network"; }}
	public override string NetworkOff { get { return "Off"; }}
	public override string NetworkSend { get { return "TX (converter side)"; }}
	public override string NetworkReceive { get { return "RX (game side)"; }}
	public override string NetworkPeerAddress { get { return "Peer Address"; }}
	public override string NetworkPort { get { return "Port"; }}
	public override string NetworkPreferIPv6 { get { return "Prefer IPv6"; }}
	public override string NetworkLastReceive { get { return "Last received："; }}
	public override string OpenKeySendWindow { get { return "Open Send Key Window"; }}

	public override string SendingData { get { return "Data Sent"; }}
	public override string Brake { get { return "Brake"; }}
	public override string Power { get { return "Power"; }}
	public override string Controller { get { return "Controller"; }}
	public override string Sanyo { get { return "Sanyo"; }}
	public override string ReversedSanyo { get { return "Rev. Sanyo"; }}
	public override string Ryojo { get { return "Ryojo"; }}
	public override string OtherController { get { return "Other"; }}
	public override string ControllerPower { get { return "P"; }}
	public override string ControllerBrake { get { return "B"; }}
	public override string ExtendedBrake { get { return "Ext. Brake"; }}

	public override string ReceivingData { get { return "Data Received"; }}
	public override string DoorClosed { get { return "Door Closed"; }}
	public override string ShockLeft { get { return "Left Shock"; }}
	public override string ShockRight { get { return "Right Shock"; }}
	public override string LED { get { return "LED"; }}
	public override string ATC { get { return "ATC"; }}
	public override string ATCOff { get { return "Off"; }}
	public override string Speed { get { return "Speed"; }}

	public override string AdvancedConfigurationDialogTitle { get { return "Advanced Configuration"; }}

	public override string NetworkConfiguration { get { return "Network Configuration"; }}
	public override string LocalPort { get { return "Local Port"; }}
	public override string SameAsDestinationPort { get { return "Same as Destination Port"; }}
	public override string SendSize { get { return "Send Data Size"; }}
	public override string ReceiveSize { get { return "Receive Data Size"; }}
	public override string SizeUnit { get { return "bytes"; }}

	public override string MmfConfiguration { get { return "Memory Mapped File Configuration"; }}
	public override string MmfName { get { return "Name of MMF"; }}
	public override string CreateMutex { get { return "Create Mutex"; }}
	public override string LockMutex { get { return "Lock Mutex on MMF Access"; }}
	public override string MutexName { get { return "Name of Mutex"; }}

	public override string OK { get { return "OK"; }}
	public override string Cancel { get { return "Cancel"; }}
}
