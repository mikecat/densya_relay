class JapaneseUIText: UIText
{
	public override string AppTitle { get { return "電車の橋渡し"; }}

	public override string Configuration { get { return "設定 (&C)"; }}
	public override string AdvancedConfiguration { get { return "高度な設定 (&A)"; }}

	public override string Network { get { return "ネットワーク"; }}
	public override string NetworkOff { get { return "オフ"; }}
	public override string NetworkSend { get { return "送信 (変換器側)"; }}
	public override string NetworkReceive { get { return "受信 (ゲーム側)"; }}
	public override string NetworkPeerAddress { get { return "通信相手アドレス"; }}
	public override string NetworkPort { get { return "ポート"; }}
	public override string NetworkPreferIPv6 { get { return "IPv6優先"; }}
	public override string NetworkLastReceive { get { return "最終受信："; }}
	public override string OpenKeySendWindow { get { return "キー送信ウィンドウを開く"; }}

	public override string SendingData { get { return "送信情報"; }}
	public override string Brake { get { return "ブレーキ"; }}
	public override string Power { get { return "マスコン"; }}
	public override string Controller { get { return "コントローラ"; }}
	public override string Sanyo { get { return "山陽"; }}
	public override string ReversedSanyo { get { return "逆山陽"; }}
	public override string Ryojo { get { return "旅情"; }}
	public override string OtherController { get { return "その他"; }}
	public override string ControllerPower { get { return "P"; }}
	public override string ControllerBrake { get { return "B"; }}
	public override string ExtendedBrake { get { return "拡張ブレーキ"; }}

	public override string ReceivingData { get { return "受信情報"; }}
	public override string DoorClosed { get { return "戸じめ"; }}
	public override string ShockLeft { get { return "振動左"; }}
	public override string ShockRight { get { return "振動右"; }}
	public override string LED { get { return "LED"; }}
	public override string ATC { get { return "ATC"; }}
	public override string ATCOff { get { return "消灯"; }}
	public override string Speed { get { return "速度"; }}
}
