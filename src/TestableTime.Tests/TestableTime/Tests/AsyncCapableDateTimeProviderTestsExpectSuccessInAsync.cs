namespace TestableTime.Tests.TestableTime.Tests;

public class AsyncCapableDateTimeProviderTestsExpectSuccessInAsync
{
	/// <summary>
	/// A "do nothing" class to simulate running some async logic that
	/// relies on a DateTimeProvider capable of being run in asynchronous
	/// mode
	/// </summary>
	public class MyAsyncClass
	{
		public async Task<DateTime> DoSomeAsyncThingReturnTheDate()
		{
			var result = await Task.Run(
				async () =>
				{
					var innerResult = DateTimeUtcProvider.UtcNow;

					var otherResult = await new MyOtherAsyncClass().DoSomeOtherAsyncThingReturnTheDate();

					Assert.Equal(innerResult, otherResult);

					return otherResult;
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
		public async Task<DateTime> DoSomeOtherAsyncThingReturnTheDate()
		{
			var result = DateTime.MinValue;

			// Make some artificial delay just for more asynchronicity
			await Task.Delay(100);

			await Task.Run(
				() => { result = DateTimeUtcProvider.UtcNow; }
			);
			return result;
		}
	}


	/// <summary>
	/// Can get nested contexts to work with parallel async calls IF we limit the
	/// Parallelism to 1 using a semaphore. Will fail with date mismatches if we
	/// allow higher degrees of concurrency though... TODO figure out why
	/// </summary>
	[Fact]
	public async Task CallAsyncContextsInNestedParallelAsyncAndExpectSuccess()
	{
		SemaphoreSlim semaphoreSlim = new(1);

		using var rootClockContext = new DateTimeUtcProviderContext(DateTime.Today);
		await Parallel.ForEachAsync(
			Enumerable.Range(0, 20),
			new ParallelOptions
			{
				// MaxDegreeOfParallelism = 12
			},
			async (i, token) =>
			{
				try
				{
					await semaphoreSlim.WaitAsync(token);

					var fakeDate = new DateTime(2020 + i, 5, 26, 10, 0, 0, DateTimeKind.Utc);
					using var innerClock = new DateTimeUtcProviderContext(
						fakeDate
					);

					var resultFromLocalContext = DateTimeUtcProvider.UtcNow;
					var resultFromNestedCalls = await new MyAsyncClass().DoSomeAsyncThingReturnTheDate();

					Assert.Equal(resultFromLocalContext, resultFromNestedCalls);
					Assert.Equal(resultFromLocalContext, fakeDate);
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		);
	}


	[Fact]
	public async Task CallAsyncContextsInParallelAsyncAndExpectSuccess()
	{
		await Parallel.ForEachAsync(
			Enumerable.Range(0, 20),
			new ParallelOptions
			{
				//MaxDegreeOfParallelism = 12
			},
			async (i, token) =>
			{
				var fakeDate = new DateTime(2020 + i, 5, 26, 10, 0, 0, DateTimeKind.Utc);
				DateTime result;
				using (new DateTimeUtcProviderContext(fakeDate))
				{
					await Task.Delay(Random.Shared.Next(3_000, 5_000), token);
					result = DateTimeUtcProvider.UtcNow;
				}

				Assert.Equal(fakeDate, result);
			}
		);
	}


	[Fact]
	public async Task CallAsyncContextsToAsyncClassesInParallelAsyncAndExpectSuccess()
	{
		await Parallel.ForEachAsync(
			Enumerable.Range(0, 20),
			new ParallelOptions
			{
				//MaxDegreeOfParallelism = 12
			},
			async (i, _) =>
			{
				var fakeDate = new DateTime(2020 + i, 5, 26, 10, 0, 0, DateTimeKind.Utc);
				using (new DateTimeUtcProviderContext(fakeDate))
				{
					var result = await new MyAsyncClass().DoSomeAsyncThingReturnTheDate();

					Assert.Equal(fakeDate, result);
				}
			}
		);
	}

	/// <summary>
	/// Because our new, async-capable implementation runs using AsyncLocal, it
	/// should still work when a call to async logic results in a thread change.
	/// </summary>
	/// <returns></returns>
	[Fact]
	public async Task CallAsyncMethodUsingAsyncTimeFakerExpectSuccess()
	{
		var mockDate01 = new DateTime(2018, 5, 26, 10, 0, 0, DateTimeKind.Utc);

		using var clockContext = new DateTimeUtcProviderContext(mockDate01);
		var result = await new MyAsyncClass().DoSomeAsyncThingReturnTheDate();

		Assert.Equal(mockDate01, result);
	}
}