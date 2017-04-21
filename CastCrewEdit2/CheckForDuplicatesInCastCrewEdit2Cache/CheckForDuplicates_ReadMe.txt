
WARNING: Do NOT run "Check for Duplicates in Cast/Crew Edit 2 Cache" and "Cast/Crew Edit 2" at the
         same time.



DVD Profiler can distinguish people only by four criteria: 
Last Name
First Name
Middle Name
Birth Year

If these four things are all identical DVD Profiler cannot tell these two people apart.

"Check for Duplicates in Cast/Crew Edit 2 Cache" helps you to find such possible collsions.

In the tab "Everything identical" you see all the entries that could be such mismatches.

"Could" because of the introduction of fake birth years, derived from the IMDb ID of the
actors/actresses. 

The comparison if two actors possibly clash is done under the exclusion of the fake birth year and
only looks at the "real" data.

But IMDb is not perfect either. Sometimes they create a new actor page for an actor that already
exists.

When they notice their mistake, one of the IDs is forwarded to the other, pretty much transparent to
the user (only visible in the URL of the website).

By the time IMDb notices their mistake, you might already have both IDs in your local cache.

Until now this wasn't a problem because of DVD Profiler's concept. It simply merged these two entries
again.

With the introduction of fake birth years it might happen that this one person is actually
considered two different people in your local database.

Here's an example: The actor Benjamin Hoffman was listed under the IDs nm2200880 and nm1887263 in
my local database, but both IDs led to the latter one, see: http://www.imdb.com/name/nm2200880/

To find these possibly identical people, switch to the "Everything identical" tab and check if
the pairs are actually only one person. If so, simply remove the outdated ID.

If this outdated ID already has a fake birth year assigned you should check your DVD Profiler
database and merge the actors there, too.

Another interesting case was  Elizabeth Rodriguez whom I had as nm1879985 and nm3318694 but both
IDs led to nm0735300 which I hadn't.

So I saved and closed "Check for Duplicates in Cast/Crew Edit 2 Cache", opened "Cast/Crew Edit 2"
and scanned one of her works that has few cast in it (for time-saving purposes), in her case 
"Beadhead" http://www.imdb.com/title/tt0165634/

Now I had three IDs of hers in my cache and could savely remove the two outdated ones.