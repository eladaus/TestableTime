using FluentAssertions;

namespace TestableTime.Tests.Dvoituron.com.Tests;

public class SyncOnlyDateTimeProviderTestsExpectFailureInAsync
{
	/// <summary>
	/// A "do nothing" class to simulate running some async logic that
	/// relies on a DateTimeProvider capable of being run in asynchronous
	/// mode
	/// </summary>
	public class MyAsyncClass
	{
		public async Task<DateTime> DoSomeAsyncThing()
		{
			var result = DateTime.MinValue;
			await Task.Run(
				async () =>
				{
					result = DateTimeProvider.UtcNow;

					var otherResult = await new MyOtherAsyncClass().DoSomeOtherAsyncThing();

					Assert.Equal(result, otherResult);
				}
			);
			return result;
		}
	}


	/// <summary>
	/// A "do nothing" class to simulate running some async logic that
	/// relies on a DateTimeProvider capable of being run in asynchronous
	/// mode
	/// </summary>
	public class MyOtherAsyncClass
	{
		public async Task<DateTime> DoSomeOtherAsyncThing()
		{
			var result = DateTime.MinValue;

			// Make some artificial delay just for more asynchronicity
			await Task.Delay(100);

			await Task.Run(
				() => { result = DateTimeProvider.UtcNow; }
			);
			return result;
		}
	}

	/// <summary>
	/// Because the legacy, sync-only implementation runs using ThreadLocal, it
	/// will fail when a call to async logic results in a thread change.
	/// </summary>
	/// <returns></returns>
	[Fact]
	public async Task CallAsyncMethodUsingSyncTimeFakerExpectEmptyStackFailure()
	{
		var mockDate01 = new DateTime(2018, 5, 26, 10, 0, 0, DateTimeKind.Utc);

		var expectedFailure = await Assert.ThrowsAsync<InvalidOperationException>(
			async () =>
			{
				using var syncOnlyDateTimeProvider = new DateTimeProviderContext(mockDate01);
				await new MyAsyncClass().DoSomeAsyncThing();
			}
		);
		expectedFailure.Message.Should().Be("Stack empty.");
	}
}