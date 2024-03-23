using FluentAssertions;

namespace TestableTime.Tests.TestableTime.Tests;

/// <summary>
/// We expect that our new async-capable DateTime-testable class should
/// behave also when running synchronous code
/// Taken from https://github.com/akazemis as good examples to test against
/// the code by https://dvoituron.com/
/// </summary>
/// <see
///     href="https://github.com/akazemis/TestableDateTimeUtcProvider/blob/master/test/TestableDateTimeUtcProvider.Tests/DateTimeUtcProviderTest.cs" />
public class AsyncCapableDateTimeUtcProviderTestsExpectSuccessInSync
{
	[Fact]
	public void GetUtcNow_WhenMultipleContext_ReturnsCorrectFakeUtcNow()
	{
		var fakeDate1 = new DateTime(2018, 5, 26, 10, 0, 0, DateTimeKind.Utc);
		var fakeDate2 = new DateTime(2020, 7, 15, 12, 30, 0, DateTimeKind.Utc);
		DateTime result1;
		DateTime result2;

		using (var context1 = new DateTimeUtcProviderContext(fakeDate1))
		{
			result1 = DateTimeUtcProvider.UtcNow;
			using (var context2 = new DateTimeUtcProviderContext(fakeDate2))
			{
				result2 = DateTimeUtcProvider.UtcNow;
			}
		}

		result1.Should().Be(fakeDate1);
		result2.Should().Be(fakeDate2);
	}


	[Fact]
	public void GetUtcNow_WhenMultipleContextRunningInParallel_ReturnsCorrectFakeUtcNow()
	{
		var fakeDate11 = new DateTime(2018, 5, 26, 10, 0, 0, DateTimeKind.Utc);
		var fakeDate12 = new DateTime(2020, 7, 15, 12, 30, 0, DateTimeKind.Utc);
		var fakeDate21 = new DateTime(2021, 8, 21, 10, 0, 0, DateTimeKind.Utc);
		var fakeDate22 = new DateTime(2022, 9, 25, 13, 45, 0, DateTimeKind.Utc);
		var fakeDate31 = new DateTime(2015, 10, 21, 13, 0, 0, DateTimeKind.Utc);
		var fakeDate32 = new DateTime(2014, 11, 18, 10, 10, 10, DateTimeKind.Utc);
		var result1 = default(DateTime);
		var result2 = default(DateTime);
		var result3 = default(DateTime);
		var action1 =
			() =>
			{
				using var context1 = new DateTimeUtcProviderContext(fakeDate11);
				using var context2 = new DateTimeUtcProviderContext(fakeDate12);
				result1 = DateTimeUtcProvider.UtcNow;
			};
		var action2 =
			() =>
			{
				using var context1 = new DateTimeUtcProviderContext(fakeDate21);
				using var context2 = new DateTimeUtcProviderContext(fakeDate22);
				result2 = DateTimeUtcProvider.UtcNow;
			};
		var action3 =
			() =>
			{
				using var context1 = new DateTimeUtcProviderContext(fakeDate31);
				using var context2 = new DateTimeUtcProviderContext(fakeDate32);
				result3 = DateTimeUtcProvider.UtcNow;
			};

		Parallel.Invoke(action1, action2, action3);
		while (result1 == DateTime.MinValue || result2 == DateTime.MinValue || result3 == DateTime.MinValue) { }

		result1.Should().Be(fakeDate12);
		result2.Should().Be(fakeDate22);
		result3.Should().Be(fakeDate32);
	}

	[Fact]
	public void GetUtcNow_WhenNoContext_ReturnsCorrectUtcNow()
	{
		var timeDifferenceTolerance = TimeSpan.FromMicroseconds(2_000);
		var result = DateTimeUtcProvider.UtcNow;
		result.Should().BeCloseTo(DateTime.UtcNow, timeDifferenceTolerance);
	}


	[Fact]
	public void GetUtcNow_WhenSingleContext_ReturnsCorrectFakeUtcNow()
	{
		var fakeDate = new DateTime(2018, 5, 26, 10, 0, 0, DateTimeKind.Utc);
		DateTime result;

		using (var context = new DateTimeUtcProviderContext(fakeDate))
		{
			result = DateTimeUtcProvider.UtcNow;
		}

		result.Should().Be(fakeDate);
	}
}