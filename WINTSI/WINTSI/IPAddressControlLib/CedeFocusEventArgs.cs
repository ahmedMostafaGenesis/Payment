using System;

namespace IPAddressControlLib
{
internal class CedeFocusEventArgs : EventArgs
{
	private int _fieldIndex;

	private Action _action;

	private Direction _direction;

	private Selection _selection;

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

	public Action Action
	{
		get
		{
			return _action;
		}
		set
		{
			_action = value;
		}
	}

	public Direction Direction
	{
		get
		{
			return _direction;
		}
		set
		{
			_direction = value;
		}
	}

	public Selection Selection
	{
		get
		{
			return _selection;
		}
		set
		{
			_selection = value;
		}
	}
}
}