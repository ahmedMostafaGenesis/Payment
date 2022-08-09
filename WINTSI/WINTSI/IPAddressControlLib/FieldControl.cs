using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace IPAddressControlLib
{
internal class FieldControl : TextBox
{
	public const byte MinimumValue = 0;

	public const byte MaximumValue = byte.MaxValue;

	private int _fieldIndex = -1;

	private byte _rangeLower;

	private byte _rangeUpper = byte.MaxValue;

	private TextFormatFlags _textFormatFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;

	public bool Blank => TextLength == 0;

	public int FieldIndex
	{
		get
		{
			return _fieldIndex;
		}
		set
		{
			_fieldIndex = value;
		}
	}

	public override Size MinimumSize
	{
		get
		{
			Graphics graphics = Graphics.FromHwnd(base.Handle);
			Size result = TextRenderer.MeasureText(graphics, "333", Font, base.Size, _textFormatFlags);
			graphics.Dispose();
			return result;
		}
	}

	public byte RangeLower
	{
		get
		{
			return _rangeLower;
		}
		set
		{
			if (value < 0)
			{
				_rangeLower = 0;
			}
			else if (value > _rangeUpper)
			{
				_rangeLower = _rangeUpper;
			}
			else
			{
				_rangeLower = value;
			}
			if (Value < _rangeLower)
			{
				Text = _rangeLower.ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	public byte RangeUpper
	{
		get
		{
			return _rangeUpper;
		}
		set
		{
			if (value < _rangeLower)
			{
				_rangeUpper = _rangeLower;
			}
			else if (value > byte.MaxValue)
			{
				_rangeUpper = byte.MaxValue;
			}
			else
			{
				_rangeUpper = value;
			}
			if (Value > _rangeUpper)
			{
				Text = _rangeUpper.ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	public byte Value
	{
		get
		{
			if (!byte.TryParse(Text, out var result))
			{
				return RangeLower;
			}
			return result;
		}
	}

	public event EventHandler<CedeFocusEventArgs> CedeFocusEvent;

	public event EventHandler<TextChangedEventArgs> TextChangedEvent;

	public void TakeFocus(Action action)
	{
		Focus();
		switch (action)
		{
		case Action.Trim:
			if (TextLength > 0)
			{
				int length = TextLength - 1;
				base.Text = Text.Substring(0, length);
			}
			base.SelectionStart = TextLength;
			break;
		case Action.Home:
			base.SelectionStart = 0;
			SelectionLength = 0;
			break;
		case Action.End:
			base.SelectionStart = TextLength;
			break;
		}
	}

	public void TakeFocus(Direction direction, Selection selection)
	{
		Focus();
		if (selection == Selection.All)
		{
			base.SelectionStart = 0;
			SelectionLength = TextLength;
		}
		else
		{
			base.SelectionStart = ((direction != 0) ? TextLength : 0);
		}
	}

	public override string ToString()
	{
		return Value.ToString(CultureInfo.InvariantCulture);
	}

	public FieldControl()
	{
		base.BorderStyle = BorderStyle.None;
		MaxLength = 3;
		base.Size = MinimumSize;
		base.TabStop = false;
		base.TextAlign = HorizontalAlignment.Center;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		switch (e.KeyCode)
		{
		case Keys.Home:
			SendCedeFocusEvent(Action.Home);
			return;
		case Keys.End:
			SendCedeFocusEvent(Action.End);
			return;
		}
		if (IsCedeFocusKey(e))
		{
			SendCedeFocusEvent(Direction.Forward, Selection.All);
			e.SuppressKeyPress = true;
		}
		else if (IsForwardKey(e))
		{
			if (e.Control)
			{
				SendCedeFocusEvent(Direction.Forward, Selection.All);
			}
			else if (SelectionLength == 0 && base.SelectionStart == TextLength)
			{
				SendCedeFocusEvent(Direction.Forward, Selection.None);
			}
		}
		else if (IsReverseKey(e))
		{
			if (e.Control)
			{
				SendCedeFocusEvent(Direction.Reverse, Selection.All);
			}
			else if (SelectionLength == 0 && base.SelectionStart == 0)
			{
				SendCedeFocusEvent(Direction.Reverse, Selection.None);
			}
		}
		else if (IsBackspaceKey(e))
		{
			HandleBackspaceKey(e);
		}
		else if (!IsNumericKey(e) && !IsEditKey(e) && !IsEnterKey(e))
		{
			e.SuppressKeyPress = true;
		}
	}

	protected override void OnParentBackColorChanged(EventArgs e)
	{
		base.OnParentBackColorChanged(e);
		BackColor = base.Parent.BackColor;
	}

	protected override void OnParentForeColorChanged(EventArgs e)
	{
		base.OnParentForeColorChanged(e);
		ForeColor = base.Parent.ForeColor;
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		base.Size = MinimumSize;
	}

	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
		if (!Blank)
		{
			if (!int.TryParse(Text, out var result))
			{
				base.Text = string.Empty;
			}
			else if (result > RangeUpper)
			{
				base.Text = RangeUpper.ToString(CultureInfo.InvariantCulture);
				base.SelectionStart = 0;
			}
			else if (TextLength == MaxLength && result < RangeLower)
			{
				base.Text = RangeLower.ToString(CultureInfo.InvariantCulture);
				base.SelectionStart = 0;
			}
			else
			{
				int textLength = TextLength;
				int num = base.SelectionStart;
				base.Text = result.ToString(CultureInfo.InvariantCulture);
				if (TextLength < textLength)
				{
					num -= textLength - TextLength;
					base.SelectionStart = Math.Max(0, num);
				}
			}
		}
		if (this.TextChangedEvent != null)
		{
			TextChangedEventArgs textChangedEventArgs = new TextChangedEventArgs();
			textChangedEventArgs.FieldIndex = FieldIndex;
			textChangedEventArgs.Text = Text;
			this.TextChangedEvent(this, textChangedEventArgs);
		}
		if (TextLength == MaxLength && Focused && base.SelectionStart == TextLength)
		{
			SendCedeFocusEvent(Direction.Forward, Selection.All);
		}
	}

	protected override void OnValidating(CancelEventArgs e)
	{
		base.OnValidating(e);
		if (!Blank && Value < RangeLower)
		{
			Text = RangeLower.ToString(CultureInfo.InvariantCulture);
		}
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg != 123)
		{
			base.WndProc(ref m);
		}
	}

	private void HandleBackspaceKey(KeyEventArgs e)
	{
		if (!base.ReadOnly && (TextLength == 0 || (base.SelectionStart == 0 && SelectionLength == 0)))
		{
			SendCedeFocusEvent(Action.Trim);
			e.SuppressKeyPress = true;
		}
	}

	private static bool IsBackspaceKey(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Back)
		{
			return true;
		}
		return false;
	}

	private bool IsCedeFocusKey(KeyEventArgs e)
	{
		if ((e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal || e.KeyCode == Keys.Space) && TextLength != 0 && SelectionLength == 0 && base.SelectionStart != 0)
		{
			return true;
		}
		return false;
	}

	private static bool IsEditKey(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
		{
			return true;
		}
		if (e.Modifiers == Keys.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.V || e.KeyCode == Keys.X))
		{
			return true;
		}
		return false;
	}

	private static bool IsEnterKey(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Return)
		{
			return true;
		}
		return false;
	}

	private static bool IsForwardKey(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
		{
			return true;
		}
		return false;
	}

	private static bool IsNumericKey(KeyEventArgs e)
	{
		if ((e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9) && (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9))
		{
			return false;
		}
		return true;
	}

	private static bool IsReverseKey(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
		{
			return true;
		}
		return false;
	}

	private void SendCedeFocusEvent(Action action)
	{
		if (this.CedeFocusEvent != null)
		{
			CedeFocusEventArgs cedeFocusEventArgs = new CedeFocusEventArgs();
			cedeFocusEventArgs.FieldIndex = FieldIndex;
			cedeFocusEventArgs.Action = action;
			this.CedeFocusEvent(this, cedeFocusEventArgs);
		}
	}

	private void SendCedeFocusEvent(Direction direction, Selection selection)
	{
		if (this.CedeFocusEvent != null)
		{
			CedeFocusEventArgs cedeFocusEventArgs = new CedeFocusEventArgs();
			cedeFocusEventArgs.FieldIndex = FieldIndex;
			cedeFocusEventArgs.Action = Action.None;
			cedeFocusEventArgs.Direction = direction;
			cedeFocusEventArgs.Selection = selection;
			this.CedeFocusEvent(this, cedeFocusEventArgs);
		}
	}
}
}