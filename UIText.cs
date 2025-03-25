abstract class UIText
{
	public abstract string AppTitle { get; }

	public abstract string Configuration { get; }
	public abstract string AdvancedConfiguration { get; }

	public abstract string Network { get; }
	public abstract string NetworkOff { get; }
	public abstract string NetworkSend { get; }
	public abstract string NetworkReceive { get; }
	public abstract string NetworkPeerAddress { get; }
	public abstract string NetworkPort { get; }
	public abstract string NetworkPreferIPv6 { get; }
	public abstract string NetworkLastReceive { get; }
	public abstract string OpenKeySendWindow { get; }

	public abstract string SendingData { get; }
	public abstract string Brake { get; }
	public abstract string Power { get; }
	public abstract string Controller { get; }
	public abstract string Sanyo { get; }
	public abstract string ReversedSanyo { get; }
	public abstract string Ryojo { get; }
	public abstract string OtherController { get; }
	public abstract string ControllerPower { get; }
	public abstract string ControllerBrake { get; }
	public abstract string ExtendedBrake { get; }

	public abstract string ReceivingData { get; }
	public abstract string DoorClosed { get; }
	public abstract string ShockLeft { get; }
	public abstract string ShockRight { get; }
	public abstract string LED { get; }
	public abstract string ATC { get; }
	public abstract string ATCOff { get; }
	public abstract string Speed { get; }
}
