using System.Collections;

namespace TestableTime.Tests.Dvoituron.com;

/// <summary>
/// NOTE: THIS IS SYNC-ONLY. WILL FAIL DUE TO THREAD CHANGES IF USED IN ASYNC ENVIRONS
/// </summary>
/// <see href="https://dvoituron.com/2020/01/22/UnitTest-DateTime/" />
public class DateTimeProviderContext : IDisposable
{
	/// <summary>
	/// Why did we use ThreadLocal[Stack] to keep our stack? Because if we
	/// run our tests in parallel, they will run in separate threads and since
	/// we use a static field to hold the stack, they would interfere with
	/// each other and would cause mistakes to be made.
	/// </summary>
	private static readonly ThreadLocal<Stack> ThreadScopeStack = new(() => new Stack());

	internal DateTime ContextDateTimeNow;

	public static DateTimeProviderContext? Current
	{
		get
		{
			if (ThreadScopeStack.Value == null || ThreadScopeStack.Value?.Count == 0)
			{
				return null;
			}
			return (ThreadScopeStack.Value!.Peek() as DateTimeProviderContext)!;
		}
	}

	public DateTimeProviderContext(DateTime contextDateTimeNow)
	{
		ContextDateTimeNow = contextDateTimeNow;

		ThreadScopeStack.Value ??= new Stack();

		ThreadScopeStack.Value!.Push(this);
	}


	public void Dispose()
	{
		ThreadScopeStack.Value?.Pop();
		GC.SuppressFinalize(this);
	}
}