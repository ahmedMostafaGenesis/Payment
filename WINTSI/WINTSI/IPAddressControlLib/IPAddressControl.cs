using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace IPAddressControlLib
{
public class IPAddressControl : ContainerControl
{
	public const int FieldCount = 4;

	private bool _autoHeight = true;

	private bool _backColorChanged;

	private BorderStyle _borderStyle = BorderStyle.Fixed3D;

	private DotControl[] _dotControls = new DotControl[3];

	private FieldControl[] _fieldControls = new FieldControl[4];

	private bool _focused;

	private bool _hasMouse;

	private bool _readOnly;

	private Size Fixed3DOffset = new Size(3, 3);

	private Size FixedSingleOffset = new Size(2, 2);

	private TextBox _referenceTextBox = new TextBox();

	[Browsable(true)]
	public bool AllowInternalTab
	{
		get
		{
			FieldControl[] fieldControls = _fieldControls;
			int num = 0;
			if (num < fieldControls.Length)
			{
				return fieldControls[num].TabStop;
			}
			return false;
		}
		set
		{
			FieldControl[] fieldControls = _fieldControls;
			for (int i = 0; i < fieldControls.Length; i++)
			{
				fieldControls[i].TabStop = value;
			}
		}
	}

	[Browsable(true)]
	public bool AnyBlank
	{
		get
		{
			FieldControl[] fieldControls = _fieldControls;
			for (int i = 0; i < fieldControls.Length; i++)
			{
				if (fieldControls[i].Blank)
				{
					return true;
				}
			}
			return false;
		}
	}

	[Browsable(true)]
	public bool AutoHeight
	{
		get
		{
			return _autoHeight;
		}
		set
		{
			_autoHeight = value;
			if (_autoHeight)
			{
				AdjustSize();
			}
		}
	}

	[Browsable(false)]
	public int Baseline
	{
		get
		{
			int num = GetTextMetrics(base.Handle, Font).tmAscent + 1;
			switch (BorderStyle)
			{
			case BorderStyle.Fixed3D:
				num += Fixed3DOffset.Height;
				break;
			case BorderStyle.FixedSingle:
				num += FixedSingleOffset.Height;
				break;
			}
			return num;
		}
	}

	[Browsable(true)]
	public bool Blank
	{
		get
		{
			FieldControl[] fieldControls = _fieldControls;
			for (int i = 0; i < fieldControls.Length; i++)
			{
				if (!fieldControls[i].Blank)
				{
					return false;
				}
			}
			return true;
		}
	}

	[Browsable(true)]
	public BorderStyle BorderStyle
	{
		get
		{
			return _borderStyle;
		}
		set
		{
			_borderStyle = value;
			AdjustSize();
			Invalidate();
		}
	}

	[Browsable(false)]
	public override bool Focused
	{
		get
		{
			FieldControl[] fieldControls = _fieldControls;
			for (int i = 0; i < fieldControls.Length; i++)
			{
				if (fieldControls[i].Focused)
				{
					return true;
				}
			}
			return false;
		}
	}

	[Browsable(true)]
	public override Size MinimumSize => CalculateMinimumSize();

	[Browsable(true)]
	public bool ReadOnly
	{
		get
		{
			return _readOnly;
		}
		set
		{
			_readOnly = value;
			FieldControl[] fieldControls = _fieldControls;
			for (int i = 0; i < fieldControls.Length; i++)
			{
				fieldControls[i].ReadOnly = _readOnly;
			}
			DotControl[] dotControls = _dotControls;
			for (int i = 0; i < dotControls.Length; i++)
			{
				dotControls[i].ReadOnly = _readOnly;
			}
			Invalidate();
		}
	}

	[Bindable(true)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public override string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _fieldControls.Length; i++)
			{
				stringBuilder.Append(_fieldControls[i].Text);
				if (i < _dotControls.Length)
				{
					stringBuilder.Append(_dotControls[i].Text);
				}
			}
			return stringBuilder.ToString();
		}
		set
		{
			Parse(value);
		}
	}

	private bool HasMouse => DisplayRectangle.Contains(PointToClient(Control.MousePosition));

	public event EventHandler<FieldChangedEventArgs> FieldChangedEvent;

	public void Clear()
	{
		FieldControl[] fieldControls = _fieldControls;
		for (int i = 0; i < fieldControls.Length; i++)
		{
			fieldControls[i].Clear();
		}
	}

	public byte[] GetAddressBytes()
	{
		byte[] array = new byte[4];
		for (int i = 0; i < 4; i++)
		{
			array[i] = _fieldControls[i].Value;
		}
		return array;
	}

	public void SetAddressBytes(byte[] bytes)
	{
		Clear();
		if (bytes != null)
		{
			int num = Math.Min(4, bytes.Length);
			for (int i = 0; i < num; i++)
			{
				_fieldControls[i].Text = bytes[i].ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	public void SetFieldFocus(int fieldIndex)
	{
		if (fieldIndex >= 0 && fieldIndex < 4)
		{
			_fieldControls[fieldIndex].TakeFocus(Direction.Forward, Selection.All);
		}
	}

	public void SetFieldRange(int fieldIndex, byte rangeLower, byte rangeUpper)
	{
		if (fieldIndex >= 0 && fieldIndex < 4)
		{
			_fieldControls[fieldIndex].RangeLower = rangeLower;
			_fieldControls[fieldIndex].RangeUpper = rangeUpper;
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 4; i++)
		{
			stringBuilder.Append(_fieldControls[i].ToString());
			if (i < _dotControls.Length)
			{
				stringBuilder.Append(_dotControls[i].ToString());
			}
		}
		return stringBuilder.ToString();
	}

	public IPAddressControl()
	{
		BackColor = SystemColors.Window;
		ResetBackColorChanged();
		for (int i = 0; i < _fieldControls.Length; i++)
		{
			_fieldControls[i] = new FieldControl();
			_fieldControls[i].CreateControl();
			_fieldControls[i].FieldIndex = i;
			_fieldControls[i].Name = "FieldControl" + i.ToString(CultureInfo.InvariantCulture);
			_fieldControls[i].Parent = this;
			_fieldControls[i].CedeFocusEvent += OnCedeFocus;
			_fieldControls[i].Click += OnSubControlClicked;
			_fieldControls[i].DoubleClick += OnSubControlDoubleClicked;
			_fieldControls[i].GotFocus += OnFieldGotFocus;
			_fieldControls[i].KeyDown += OnFieldKeyDown;
			_fieldControls[i].KeyPress += OnFieldKeyPressed;
			_fieldControls[i].KeyUp += OnFieldKeyUp;
			_fieldControls[i].LostFocus += OnFieldLostFocus;
			_fieldControls[i].MouseClick += OnSubControlMouseClicked;
			_fieldControls[i].MouseDoubleClick += OnSubControlMouseDoubleClicked;
			_fieldControls[i].MouseEnter += OnSubControlMouseEntered;
			_fieldControls[i].MouseHover += OnSubControlMouseHovered;
			_fieldControls[i].MouseLeave += OnSubControlMouseLeft;
			_fieldControls[i].MouseMove += OnSubControlMouseMoved;
			_fieldControls[i].PreviewKeyDown += OnFieldPreviewKeyDown;
			_fieldControls[i].TextChangedEvent += OnFieldTextChanged;
			base.Controls.Add(_fieldControls[i]);
			if (i < 3)
			{
				_dotControls[i] = new DotControl();
				_dotControls[i].CreateControl();
				_dotControls[i].Name = "DotControl" + i.ToString(CultureInfo.InvariantCulture);
				_dotControls[i].Parent = this;
				_dotControls[i].Click += OnSubControlClicked;
				_dotControls[i].DoubleClick += OnSubControlDoubleClicked;
				_dotControls[i].MouseClick += OnSubControlMouseClicked;
				_dotControls[i].MouseDoubleClick += OnSubControlMouseDoubleClicked;
				_dotControls[i].MouseEnter += OnSubControlMouseEntered;
				_dotControls[i].MouseHover += OnSubControlMouseHovered;
				_dotControls[i].MouseLeave += OnSubControlMouseLeft;
				_dotControls[i].MouseMove += OnSubControlMouseMoved;
				base.Controls.Add(_dotControls[i]);
			}
		}
		SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.ContainerControl, value: true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
		SetStyle(ControlStyles.ResizeRedraw, value: true);
		SetStyle(ControlStyles.UserPaint, value: true);
		SetStyle(ControlStyles.FixedWidth, value: true);
		SetStyle(ControlStyles.FixedHeight, value: true);
		_referenceTextBox.AutoSize = true;
		Cursor = Cursors.IBeam;
		base.AutoScaleDimensions = new SizeF(96f, 96f);
		base.AutoScaleMode = AutoScaleMode.Dpi;
		base.Size = MinimumSize;
		base.DragEnter += IPAddressControl_DragEnter;
		base.DragDrop += IPAddressControl_DragDrop;
	}

	protected override void OnBackColorChanged(EventArgs e)
	{
		base.OnBackColorChanged(e);
		_backColorChanged = true;
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		AdjustSize();
	}

	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		_focused = true;
		_fieldControls[0].TakeFocus(Direction.Forward, Selection.All);
	}

	protected override void OnLostFocus(EventArgs e)
	{
		if (!Focused)
		{
			_focused = false;
			base.OnLostFocus(e);
		}
	}

	protected override void OnMouseEnter(EventArgs e)
	{
		if (!_hasMouse)
		{
			_hasMouse = true;
			base.OnMouseEnter(e);
		}
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		if (!HasMouse)
		{
			base.OnMouseLeave(e);
			_hasMouse = false;
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		Color color = BackColor;
		if (!_backColorChanged && (!base.Enabled || ReadOnly))
		{
			color = SystemColors.Control;
		}
		using (SolidBrush brush = new SolidBrush(color))
		{
			e.Graphics.FillRectangle(brush, base.ClientRectangle);
		}
		Rectangle bounds = new Rectangle(base.ClientRectangle.Left, base.ClientRectangle.Top, base.ClientRectangle.Width - 1, base.ClientRectangle.Height - 1);
		switch (BorderStyle)
		{
		case BorderStyle.Fixed3D:
			if (Application.RenderWithVisualStyles)
			{
				ControlPaint.DrawVisualStyleBorder(e.Graphics, bounds);
			}
			else
			{
				ControlPaint.DrawBorder3D(e.Graphics, base.ClientRectangle, Border3DStyle.Sunken);
			}
			break;
		case BorderStyle.FixedSingle:
			ControlPaint.DrawBorder(e.Graphics, base.ClientRectangle, SystemColors.WindowFrame, ButtonBorderStyle.Solid);
			break;
		}
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		AdjustSize();
	}

	private void AdjustSize()
	{
		Size minimumSize = MinimumSize;
		if (base.Width > minimumSize.Width)
		{
			minimumSize.Width = base.Width;
		}
		if (base.Height > minimumSize.Height)
		{
			minimumSize.Height = base.Height;
		}
		if (AutoHeight)
		{
			base.Size = new Size(minimumSize.Width, MinimumSize.Height);
		}
		else
		{
			base.Size = minimumSize;
		}
		LayoutControls();
	}

	private Size CalculateMinimumSize()
	{
		Size result = new Size(0, 0);
		FieldControl[] fieldControls = _fieldControls;
		foreach (FieldControl fieldControl in fieldControls)
		{
			result.Width += fieldControl.Width;
			result.Height = Math.Max(result.Height, fieldControl.Height);
		}
		DotControl[] dotControls = _dotControls;
		foreach (DotControl dotControl in dotControls)
		{
			result.Width += dotControl.Width;
			result.Height = Math.Max(result.Height, dotControl.Height);
		}
		switch (BorderStyle)
		{
		case BorderStyle.Fixed3D:
			result.Width += 6;
			result.Height += GetSuggestedHeight() - result.Height;
			break;
		case BorderStyle.FixedSingle:
			result.Width += 4;
			result.Height += GetSuggestedHeight() - result.Height;
			break;
		}
		return result;
	}

	private int GetSuggestedHeight()
	{
		_referenceTextBox.BorderStyle = BorderStyle;
		_referenceTextBox.Font = Font;
		return _referenceTextBox.Height;
	}

	private static NativeMethods.TEXTMETRIC GetTextMetrics(IntPtr hwnd, Font font)
	{
		IntPtr windowDC = NativeMethods.GetWindowDC(hwnd);
		IntPtr intPtr = font.ToHfont();
		try
		{
			IntPtr hgdiobj = NativeMethods.SelectObject(windowDC, intPtr);
			NativeMethods.GetTextMetrics(windowDC, out var lptm);
			NativeMethods.SelectObject(windowDC, hgdiobj);
			return lptm;
		}
		finally
		{
			NativeMethods.ReleaseDC(hwnd, windowDC);
			NativeMethods.DeleteObject(intPtr);
		}
	}

	private void IPAddressControl_DragDrop(object sender, DragEventArgs e)
	{
		Text = e.Data.GetData(DataFormats.Text).ToString();
	}

	private void IPAddressControl_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.Text))
		{
			e.Effect = DragDropEffects.Copy;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void LayoutControls()
	{
		SuspendLayout();
		int num = base.Width - MinimumSize.Width;
		int num2 = _fieldControls.Length + _dotControls.Length + 1;
		int num3 = num / num2;
		int num4 = num % num2;
		int[] array = new int[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = num3;
			if (i < num4)
			{
				array[i]++;
			}
		}
		int num5 = 0;
		int num6 = 0;
		switch (BorderStyle)
		{
		case BorderStyle.Fixed3D:
			num5 = Fixed3DOffset.Width;
			num6 = Fixed3DOffset.Height;
			break;
		case BorderStyle.FixedSingle:
			num5 = FixedSingleOffset.Width;
			num6 = FixedSingleOffset.Height;
			break;
		}
		int num7 = 0;
		num5 += array[num7++];
		for (int j = 0; j < _fieldControls.Length; j++)
		{
			_fieldControls[j].Location = new Point(num5, num6);
			num5 += _fieldControls[j].Width;
			if (j < _dotControls.Length)
			{
				num5 += array[num7++];
				_dotControls[j].Location = new Point(num5, num6);
				num5 += _dotControls[j].Width;
				num5 += array[num7++];
			}
		}
		ResumeLayout(performLayout: false);
	}

	private void OnCedeFocus(object sender, CedeFocusEventArgs e)
	{
		switch (e.Action)
		{
		case Action.Home:
			_fieldControls[0].TakeFocus(Action.Home);
			return;
		case Action.End:
			_fieldControls[3].TakeFocus(Action.End);
			return;
		case Action.Trim:
			if (e.FieldIndex != 0)
			{
				_fieldControls[e.FieldIndex - 1].TakeFocus(Action.Trim);
			}
			return;
		}
		if ((e.Direction != Direction.Reverse || e.FieldIndex != 0) && (e.Direction != 0 || e.FieldIndex != 3))
		{
			int fieldIndex = e.FieldIndex;
			fieldIndex = ((e.Direction != 0) ? (fieldIndex - 1) : (fieldIndex + 1));
			_fieldControls[fieldIndex].TakeFocus(e.Direction, e.Selection);
		}
	}

	private void OnFieldGotFocus(object sender, EventArgs e)
	{
		if (!_focused)
		{
			_focused = true;
			base.OnGotFocus(EventArgs.Empty);
		}
	}

	private void OnFieldKeyDown(object sender, KeyEventArgs e)
	{
		OnKeyDown(e);
	}

	private void OnFieldKeyPressed(object sender, KeyPressEventArgs e)
	{
		OnKeyPress(e);
	}

	private void OnFieldPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		OnPreviewKeyDown(e);
	}

	private void OnFieldKeyUp(object sender, KeyEventArgs e)
	{
		OnKeyUp(e);
	}

	private void OnFieldLostFocus(object sender, EventArgs e)
	{
		if (!Focused)
		{
			_focused = false;
			base.OnLostFocus(EventArgs.Empty);
		}
	}

	private void OnFieldTextChanged(object sender, TextChangedEventArgs e)
	{
		if (this.FieldChangedEvent != null)
		{
			FieldChangedEventArgs fieldChangedEventArgs = new FieldChangedEventArgs();
			fieldChangedEventArgs.FieldIndex = e.FieldIndex;
			fieldChangedEventArgs.Text = e.Text;
			this.FieldChangedEvent(this, fieldChangedEventArgs);
		}
		OnTextChanged(EventArgs.Empty);
	}

	private void OnSubControlClicked(object sender, EventArgs e)
	{
		OnClick(e);
	}

	private void OnSubControlDoubleClicked(object sender, EventArgs e)
	{
		OnDoubleClick(e);
	}

	private void OnSubControlMouseClicked(object sender, MouseEventArgs e)
	{
		OnMouseClick(e);
	}

	private void OnSubControlMouseDoubleClicked(object sender, MouseEventArgs e)
	{
		OnMouseDoubleClick(e);
	}

	private void OnSubControlMouseEntered(object sender, EventArgs e)
	{
		OnMouseEnter(e);
	}

	private void OnSubControlMouseHovered(object sender, EventArgs e)
	{
		OnMouseHover(e);
	}

	private void OnSubControlMouseLeft(object sender, EventArgs e)
	{
		OnMouseLeave(e);
	}

	private void OnSubControlMouseMoved(object sender, MouseEventArgs e)
	{
		OnMouseMove(e);
	}

	private void Parse(string text)
	{
		Clear();
		if (text == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		for (num2 = 0; num2 < _dotControls.Length; num2++)
		{
			int num3 = text.IndexOf(_dotControls[num2].Text, num, StringComparison.Ordinal);
			if (num3 < 0)
			{
				break;
			}
			_fieldControls[num2].Text = text.Substring(num, num3 - num);
			num = num3 + _dotControls[num2].Text.Length;
		}
		_fieldControls[num2].Text = text.Substring(num);
	}

	private void ResetBackColorChanged()
	{
		_backColorChanged = false;
	}
}
}