# TestableTime

Async-capable Ambient Context Pattern for unit/integration testable DateTimes in C# .NET 

For any and all future updates, the code releases will utilizing SemVer semantic versioning style. 

Conveniently, eladaus.TestableTime is [available on NuGet](https://www.nuget.org/packages/eladaus.TestableTime/) as `eladaus.TestableTime`. Install it from NuGet Package Manager Console with:
	
~~~~
Install-Package eladaus.TestableTime
~~~~

---
## Known Issues

- None currently

---
## Description

The TestableTime library is a modernized, async-capable implementation of a 
DateTime provider context. It allows developers to retrieve 
DateTime.UtcNow values in code under normal execution operations but also to 
mock / programmatically assign the point-in-time for unit testing operations.

It is modelled on the Ambient Context Model (see dvoituron.com in links below)
however that code fails under modern asynchronous scenarios due to thread 
changes during execution.

Using the new AsyncLocal concept, we can now pass a predefined DateTime instance 
through chained/nested async logic during unit and integration testing.

---
## How to Use:

```csharp

	var fakeDate = new DateTime(1981, 08, 20, 0, 0, 0, DateTimeKind.Utc);
	using var innerClock = new DateTimeUtcProviderContext(
		fakeDate
	);

	var resultFromLocalContext = DateTimeUtcProvider.UtcNow;
	var resultFromNestedCalls = await new MyAsyncClass().DoSomeAsyncThingReturnTheDate();

	Assert.Equal(resultFromLocalContext, resultFromNestedCalls);
	Assert.Equal(resultFromLocalContext, fakeDate);

```

---
## Further reading / inspirational resources

- https://dvoituron.com/2020/01/22/UnitTest-DateTime/ 
- https://vainolo.com/2022/02/23/storing-context-data-in-c-using-asynclocal/
- https://github.com/akazemis/TestableDateTimeProvider/blob/master/test/TestableDateTimeProvider.Tests/DateTimeProviderTest.cs

