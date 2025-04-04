using System.Drawing;
using System.Windows.Forms;

class ControlUtils
{
	private const int fontSize = 16, gridSize = 20;
	private static Font _Font = new Font("MS UI Gothic", fontSize, GraphicsUnit.Pixel);
	public static Font Font { get { return _Font; }}
	private static Icon _Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
	public static Icon Icon { get { return _Icon; }}

	public static Size GetSizeOnGrid(float width, float height)
	{
		return new Size((int)(gridSize * width), (int)(gridSize * height));
	}

	public static Point GetPointOnGrid(float x, float y)
	{
		return new Point((int)(gridSize * x), (int)(gridSize * y));
	}

	public static T CreateControl<T>(Control parent, float x, float y, float width, float height)
	where T: Control, new()
	{
		T control = new T();
		control.Location = GetPointOnGrid(x, y);
		control.Size = GetSizeOnGrid(width, height);
		if (parent != null) parent.Controls.Add(control);
		return control;
	}
}
