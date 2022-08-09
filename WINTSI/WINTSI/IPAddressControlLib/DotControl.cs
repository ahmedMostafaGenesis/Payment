using System;
using System.Drawing;
using System.Windows.Forms;

namespace IPAddressControlLib

{
internal class DotControl : Control
{
	private bool _backColorChanged;

	private bool _readOnly;

	private StringFormat _stringFormat;

	private SizeF _sizeText;

	public override Size MinimumSize
	{
		get
		{
			using (Graphics graphics = Graphics.FromHwnd(base.Handle))
			{
				_sizeText = graphics.MeasureString(Text, Font, -1, _stringFormat);
			}
			_sizeText.Height += 1f;
			return _sizeText.ToSize();
		}
	}

	public bool ReadOnly
	{
		get
		{
			return _readOnly;
		}
		set
		{
			_readOnly = value;
			Invalidate();
		}
	}

	public override string ToString()
	{
		return Text;
	}

	public DotControl()
	{
		Text = ".";
		_stringFormat = StringFormat.GenericTypographic;
		_stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
		BackColor = SystemColors.Window;
		base.Size = MinimumSize;
		base.TabStop = false;
		SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, value: true);
		SetStyle(ControlStyles.ResizeRedraw, value: true);
		SetStyle(ControlStyles.UserPaint, value: true);
		SetStyle(ControlStyles.FixedHeight, value: true);
		SetStyle(ControlStyles.FixedWidth, value: true);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		base.Size = MinimumSize;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		Color color = BackColor;
		if (!_backColorChanged && (!base.Enabled || ReadOnly))
		{
			color = SystemColors.Control;
		}
		Color color2 = ForeColor;
		if (!base.Enabled)
		{
			color2 = SystemColors.GrayText;
		}
		else if (ReadOnly && !_backColorChanged)
		{
			color2 = SystemColors.WindowText;
		}
		using (SolidBrush brush = new SolidBrush(color))
		{
			e.Graphics.FillRectangle(brush, base.ClientRectangle);
		}
		using SolidBrush brush2 = new SolidBrush(color2);
		float num = (float)base.ClientRectangle.Width / 2f - _sizeText.Width / 2f;
		e.Graphics.DrawString(Text, Font, brush2, new RectangleF(num, 0f, _sizeText.Width, _sizeText.Height), _stringFormat);
	}

	protected override void OnParentBackColorChanged(EventArgs e)
	{
		base.OnParentBackColorChanged(e);
		BackColor = base.Parent.BackColor;
		_backColorChanged = true;
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
}
}