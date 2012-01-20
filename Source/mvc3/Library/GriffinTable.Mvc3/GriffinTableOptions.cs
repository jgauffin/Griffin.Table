using System;

namespace GriffinTable.Mvc3
{
	/// <summary>
	/// Options used when generating a table
	/// </summary>
	[Flags]
	public enum GriffinTableOptions
	{
		/// <summary>
		/// No options
		/// </summary>
		None = 0,

		/// <summary>
		/// Create the <![CDATA[<script>]]> tag to initialize the table
		/// </summary>
		CreateScript = 0x1
	}
}