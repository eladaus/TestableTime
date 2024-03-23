namespace TestableTime.Tests.Dvoituron.com;

/// <summary>
/// NOTE: THIS IS SYNC-ONLY. WILL FAIL DUE TO THREAD CHANGES IF USED IN ASYNC ENVIRONS
/// </summary>
/// <see href="https://dvoituron.com/2020/01/22/UnitTest-DateTime/" />
public class DateTimeProvider
{
	public static DateTime Now
		=> DateTimeProviderContext.Current == null
			? DateTime.Now
			: DateTimeProviderContext.Current.ContextDateTimeNow;

	public static DateTime UtcNow => Now.ToUniversalTime();

	public static DateTime Today => Now.Date;
}