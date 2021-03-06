using System;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// The result of the conversion.
	/// </summary>
	public class ConversionResult : IConversionResult
	{
		/// <summary>
		/// Represents an invalid conversion result.
		/// </summary>
		public static readonly ConversionResult Invalid = new ConversionResult(null, false);

		/// <inheritdoc />
		public bool IsValid { get; }

		private readonly object _value;

		/// <inheritdoc />
		public object Value => IsValid ? _value : throw new InvalidOperationException("Value is not valid");

		/// <summary>
		/// Initializes new instance of <see cref="ConversionResult"/>.
		/// </summary>
		/// <param name="value">Valid value to be used by the <see cref="IMicrosoftConfigurationConverter"/>.</param>
		public ConversionResult(object value)
			: this(value, true)
		{
		}

		private ConversionResult(object value, bool isValid)
		{
			IsValid = isValid;
			_value = value;
		}
	}
}
