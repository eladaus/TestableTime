namespace TestableTime;

/// <summary>
/// Async-Capable, Unit-testable DateTime provider.
/// Falls back to real DateTime when not in testing
/// mode / not using DateTimeUtcProviderContext
/// </summary>
/// <see href="" />
/// <seealso href="https://dvoituron.com/2020/01/22/UnitTest-DateTime/" />
public class DateTimeUtcProvider
{
	public static DateTime UtcNow
		=> DateTimeUtcProviderContext.Current == null
			? DateTime.UtcNow
			: DateTimeUtcProviderContext.Current.ContextDateTimeUtc;
}