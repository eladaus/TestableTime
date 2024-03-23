using FluentAssertions;

namespace TestableTime.Tests.Dvoituron.com.Tests;

/// <summary>
/// We expect that the popular, sync-only DateTime-testable class should
/// behave when running synchronous code
/// Taken from https://github.com/akazemis as good examples to test against
/// the code by https://dvoituron.com/
/// </summary>
/// <see
///     href="https://github.com/akazemis/TestableDateTimeProvider/blob/master/test/TestableDateTimeProvider.Tests/DateTimeProviderTest.cs" />
public class SyncOnlyDateTimeProviderTestsExpectSuccessInSync
{
	[Fact]
	public void GetUtcNow_WhenMultipleContext_ReturnsCorrectFakeUtcNow()
	{
		var fakeDate1 = new DateTime(2018, 5, 26, 10, 0, 0, DateTimeKind.Utc);
		var fakeDate2 = new DateTime(2020, 7, 15, 12, 30, 0, DateTimeKind.Utc);
		DateTime result1;
		DateTime result2;

		using (var context1 = new DateTimeProviderContext(fakeDate1))
		{
			result1 = DateTimeProvider.UtcNow;
			using (var context2 = new DateTimeProviderContext(fakeDate2))
			{
				result2 = DateTimeProvider.UtcNow;
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
				using var context1 = new DateTimeProviderContext(fakeDate11);
				using var context2 = new DateTimeProviderContext(fakeDate12);
				result1 = DateTimeProvider.UtcNow;
			};
		var action2 =
			() =>
			{
				using var context1 = new DateTimeProviderContext(fakeDate21);
				using var context2 = new DateTimeProviderContext(fakeDate22);
				result2 = DateTimeProvider.UtcNow;
			};
		var action3 =
			() =>
			{
				using var context1 = new DateTimeProviderContext(fakeDate31);
				using var context2 = new DateTimeProviderContext(fakeDate32);
				result3 = DateTimeProvider.UtcNow;
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
		var timeDifferenceTolerance = TimeSpan.FromMicroseconds(2000);
		var result = DateTimeProvider.UtcNow;
		result.Should().BeCloseTo(DateTime.UtcNow, timeDifferenceTolerance);
	}


	[Fact]
	public void GetUtcNow_WhenSingleContext_ReturnsCorrectFakeUtcNow()
	{
		var fakeDate = new DateTime(2018, 5, 26, 10, 0, 0, DateTimeKind.Utc);
		DateTime result;

		using (var context = new DateTimeProviderContext(fakeDate))
		{
			result = DateTimeProvider.UtcNow;
		}

		result.Should().Be(fakeDate);
	}
}