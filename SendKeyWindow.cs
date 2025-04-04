using System;
using System.Drawing;
using System.Windows.Forms;

class SendKeyWindow: Form
{
	private const int WM_KEYDOWN = 0x0100;
	private const int WM_KEYUP = 0x0101;

	private Label mainLabel;

	public delegate void KeyMessageHandler(byte keyCode, byte scanCode, bool isExtKey, bool pressed);
	public KeyMessageHandler KeyMessage = (k, s, e, p) => {};

	public SendKeyWindow(UIText uiText)
	{
		this.Font = ControlUtils.Font;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Icon = ControlUtils.Icon;
		this.ShowInTaskbar = false;
		this.ClientSize = ControlUtils.GetSizeOnGrid(20, 5);

		mainLabel = ControlUtils.CreateControl<Label>(this, 0, 0, 1, 1);
		mainLabel.Dock = DockStyle.Fill;
		mainLabel.TextAlign = ContentAlignment.MiddleCenter;

		this.UpdateUIText(uiText);
	}

	public void UpdateUIText(UIText uiText)
	{
		this.Text = uiText.SendKeyWindowTitle;
		mainLabel.Text = uiText.SendKeyWindowMessage;
	}

	protected override bool ProcessKeyMessage(ref Message m)
	{
		if (m.Msg == WM_KEYDOWN || m.Msg == WM_KEYUP)
		{
			KeyMessage(
				(byte)m.WParam.ToInt64(),
				(byte)(m.LParam.ToInt64() >> 16),
				((m.LParam.ToInt64() >> 24) & 1) != 0,
				m.Msg == WM_KEYDOWN
			);
			return true;
		}
		return false;
	}
}
