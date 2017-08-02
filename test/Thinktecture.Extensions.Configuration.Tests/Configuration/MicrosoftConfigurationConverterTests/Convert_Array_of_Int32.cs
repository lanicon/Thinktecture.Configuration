﻿using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Array_of_Int32 : ConvertBase
	{
		[Fact]
		public void Should_convert_null_to_null()
		{
			RoundtripConvert<TestConfiguration<int[]>>("P1", null)
				.P1.Should().BeNull();
		}

		[Fact]
		public void Should_convert_empty_string_to_empty_array()
		{
			RoundtripConvert<TestConfiguration<int[]>>("P1", String.Empty)
				.P1.Should().BeEmpty();
		}

		[Fact]
		public void Should_convert_array_with_one_value()
		{
			SetupCreateFromString("42", 42);

			RoundtripConvert<TestConfiguration<int[]>>("P1:0", "42")
				.P1.ShouldBeEquivalentTo(new[] {42});
		}

		[Fact]
		public void Should_convert_array_with_two_values()
		{
			SetupCreateFromString("42", 42);
			SetupCreateFromString("43", 43);

			RoundtripConvert<TestConfiguration<int[]>>(dictionary =>
				{
					dictionary.Add("P1:0", "42");
					dictionary.Add("P1:1", "43");
				})
				.P1.ShouldBeEquivalentTo(new[] {42, 43});
		}

		[Fact]
		public void Should_convert_array_with_two_non_adjacent_values()
		{
			SetupCreateFromString("42", 42);
			SetupCreateFromString("43", 43);

			RoundtripConvert<TestConfiguration<int[]>>(dictionary =>
				{
					dictionary.Add("P1:1", "42");
					dictionary.Add("P1:3", "43");
				})
				.P1.ShouldBeEquivalentTo(new[] {0, 42, 0, 43});
		}

		[Fact]
		public void Should_ignore_invalid_indexes()
		{
			SetupCreateFromString("42", 42);
			SetupCreateFromString("43", 43);

			RoundtripConvert<TestConfiguration<int[]>>(dictionary =>
				{
					dictionary.Add("P1:0", "42");
					dictionary.Add("P1:Foo", "1");
					dictionary.Add("P1:2", "43");
				})
				.P1.ShouldBeEquivalentTo(new[] {42, 0, 43});
		}

		[Fact]
		public void Should_ignore_invalid_values()
		{
			SetupCreateFromString("42", 42);
			SetupCreateFromString("43", 43);
			SetupCreateFromString<int>("Foo", ConversionResult.Invalid);

			RoundtripConvert<TestConfiguration<int[]>>(dictionary =>
				{
					dictionary.Add("P1:0", "42");
					dictionary.Add("P1:1", "Foo");
					dictionary.Add("P1:2", "43");
				})
				.P1.ShouldBeEquivalentTo(new[] {42, 0, 43});
		}

		[Fact]
		public void Should_replace_non_empty_array_with_null()
		{
			SetupCreate(new TestConfiguration<int[]>() {P1 = new[] {1}});

			RoundtripConvert<TestConfiguration<int[]>>("P1", null, false)
				.P1.Should().BeNull();
		}

		[Fact]
		public void Should_replace_non_empty_array_with_new_empty_array()
		{
			SetupCreate(new TestConfiguration<int[]>() {P1 = new[] {1}});

			RoundtripConvert<TestConfiguration<int[]>>("P1", String.Empty, false)
				.P1.Should().BeEmpty();
		}

		[Fact]
		public void Should_replace_non_empty_array_with_new_non_empty_array()
		{
			SetupCreateFromString("42", 42);
			SetupCreate(new TestConfiguration<int[]>() {P1 = new[] {1}});

			RoundtripConvert<TestConfiguration<int[]>>("P1:0", "42", false)
				.P1.ShouldBeEquivalentTo(new[] {42});
		}

		[Fact]
		public void Should_write_a_warning_if_non_empty_array_is_replaced_by_new_array()
		{
			SetupCreateFromString("42", 42);
			SetupCreate(new TestConfiguration<int[]>() {P1 = new[] {1}});

			RoundtripConvert<TestConfiguration<int[]>>("P1:0", "42", false);

			LoggerMock.Verify(l => l.Log(LogLevel.Warning, 0, It.IsAny<object>(), null, It.IsAny<Func<object, Exception, string>>()), Times.Once);
		}
	}
}