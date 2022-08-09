using System;

namespace IPAddressControlLib

{
	internal class TextChangedEventArgs : EventArgs
	{
		private int _fieldIndex;

		private string _text;

		public int FieldIndex
		{
			get { return _fieldIndex; }
			set { _fieldIndex = value; }
		}

		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}
	}
}
