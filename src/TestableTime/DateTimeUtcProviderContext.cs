using System.Collections;

namespace TestableTime;

/// <summary>
/// </summary>
public class DateTimeUtcProviderContext : IDisposable
{
	/// <summary>
	/// Why did we use ThreadLocal[Stack] to keep our stack? Because if we
	/// run our tests in parallel, they will run in separate threads and since
	/// we use a static field to hold the stack, they would interfere with
	/// each other and would cause mistakes to be made.
	/// </summary>
	private static readonly AsyncLocal<Stack> AsyncLocalScopeStack = new();

	internal DateTime ContextDateTimeUtc;

	public static DateTimeUtcProviderContext? Current
	{
		get
		{
			if (AsyncLocalScopeStack.Value == null || AsyncLocalScopeStack.Value?.Count == 0)
			{
				return null;
			}
			return (AsyncLocalScopeStack.Value!.Peek() as DateTimeUtcProviderContext)!;
		}
	}


	public DateTimeUtcProviderContext(DateTime contextDateTimeUtc)
	{
		ContextDateTimeUtc = contextDateTimeUtc;

		AsyncLocalScopeStack.Value ??= new Stack();

		AsyncLocalScopeStack.Value!.Push(this);
	}


	public void Dispose()
	{
		AsyncLocalScopeStack.Value?.Pop();
		GC.SuppressFinalize(this);
	}
}