Code for a sync-capable mock/testable DateTime taken from https://dvoituron.com/2020/01/22/UnitTest-DateTime/

Uses the Ambient Context Model approach.

I used this code (to Big Success) in a past projects when all logic was sync-only, but 
given that we now live in an async-overlord future, the legacy sample code would crash 
when async calls resulted in thread changes.

The new AsyncLocal object comes to our rescue.


Example test suite implementation was found at https://github.com/akazemis/TestableDateTimeProvider/blob/master/test/TestableDateTimeProvider.Tests/DateTimeProviderTest.cs
and adapted to suite the https://dvoituron.com code base, then used again to test my 
async-livin'-in-da-future implementation, hopefully to be comfortable that it works as intended.

So far, so good.
