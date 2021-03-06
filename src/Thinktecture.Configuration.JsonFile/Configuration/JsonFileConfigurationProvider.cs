using System;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Reads configuration from <see cref="JToken"/>.
	/// </summary>
	public class JsonFileConfigurationProvider : IConfigurationProvider<JToken, JToken>
	{
		[NotNull, ItemCanBeNull]
		private readonly JToken[] _tokens; // one of the tokens may be null

		[NotNull]
		private readonly IJsonTokenConverter _tokenConverter;

		/// <summary>
		/// Creates new instance of <see cref="JsonFileConfigurationProvider"/>.
		/// </summary>
		/// <param name="tokens">Tokens the configurations are deserialized from. The first token is considered to be the main file, the others act as overrides.</param>
		/// <param name="tokenConverter">Json deserializer.</param>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="tokens"/> or <paramref name="tokenConverter"/> is <c>null</c>.</exception>
		public JsonFileConfigurationProvider([NotNull] JToken[] tokens, [NotNull] IJsonTokenConverter tokenConverter)
		{
			if (tokens == null)
				throw new ArgumentNullException(nameof(tokens));
			if (tokens.Length == 0)
				throw new ArgumentException("Token collection can not be empty.", nameof(tokens));

			_tokens = tokens;
			_tokenConverter = tokenConverter ?? throw new ArgumentNullException(nameof(tokenConverter));
		}

		/// <inheritdoc />
		public TConfiguration GetConfiguration<TConfiguration>(IConfigurationSelector<JToken, JToken> selector = null)
		{
			var tokens = selector == null ? _tokens : SelectTokens(_tokens, selector);

			return _tokenConverter.Convert<TConfiguration>(tokens);
		}

		[NotNull]
		private static JToken[] SelectTokens([NotNull] JToken[] tokens, [NotNull] IConfigurationSelector<JToken, JToken> selector)
		{
			if (tokens == null)
				throw new ArgumentNullException(nameof(tokens));
			if (selector == null)
				throw new ArgumentNullException(nameof(selector));

			var configs = new JToken[tokens.Length];

			for (var i = 0; i < tokens.Length; i++)
			{
				var config = tokens[i];

				if (config != null)
					config = selector.Select(config);

				configs[i] = config;
			}

			return configs;
		}
	}
}
