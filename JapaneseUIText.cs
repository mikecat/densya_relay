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
	public override string LastReceiveFewSeconds { get { return "数秒以内前"; }}
	public override string LastReceiveSecondsPrefix { get { return ""; }}
	public override string LastReceiveSecondsSuffix { get { return "秒前"; }}
	public override string LastReceiveOneMinute { get { return "1分以上前"; }}
	public override string OpenKeySendWindow { get { return "キー送信ウィンドウを開く"; }}
	public override string ConnectionErrorTitle { get { return "エラー"; }}
	public override string ConnectionErrorMessage { get { return "ソケットの初期化に失敗しました。\n接続先を確認してください。"; }}
	public override string DestinationAddressNotSetTitle { get { return "エラー"; }}
	public override string DestinationAddressNotSetMessage { get { return "通信相手アドレスを入力してください。"; }}

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

	public override string AdvancedConfigurationDialogTitle { get { return "高度な設定"; }}

	public override string NetworkConfiguration { get { return "ネットワーク設定"; }}
	public override string LocalPort { get { return "ローカルポート"; }}
	public override string SameAsDestinationPort { get { return "送信先ポートと同じ"; }}
	public override string SendSize { get { return "送信情報サイズ"; }}
	public override string ReceiveSize { get { return "受信情報サイズ"; }}
	public override string SizeUnit { get { return "バイト"; }}

	public override string MmfConfiguration { get { return "メモリマップドファイル設定"; }}
	public override string MmfName { get { return "メモリマップドファイル名"; }}
	public override string CreateMutex { get { return "ミューテックスを作成する"; }}
	public override string LockMutex { get { return "読み書き時ミューテックスをロック"; }}
	public override string MutexName { get { return "ミューテックス名"; }}

	public override string OK { get { return "OK"; }}
	public override string Cancel { get { return "キャンセル"; }}

	public override string SendKeyWindowTitle { get { return "キー送信ウィンドウ"; }}
	public override string SendKeyWindowMessage { get { return "このウィンドウでキー操作を行うと、\n受信側に転送されます。"; }}
}
