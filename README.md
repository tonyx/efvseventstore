# Purpose of this test

This test is to compare the style and the performance of Entity Framework with C# respect to the equivalent logic using F# and Sharpino.

# How to run

1. Clone this repository
2. Specify your postgres connection string in the appsettings.json file
2. Setup the database using ef (dotnet ef database update - in case of problems:refer to official documentation)
3. Enter in the directory efexample.Tests
4. Run the massiv insert tests (dotnet test)
You will see the results like the following:
```
Inserted 5000 students in 354ms (14124,29 students/second)
Inserted 1000 students in 68ms (14705,88 students/second)
Inserted 10000 students in 469ms (21321,96 students/second)
Inserted 100000 students in 2763ms (36192,54 students/second)
```

Note: the equivalent in Sharpino version is:
```
Inserting 1000 students in batch took 31 ms
Inserting 5000 students in batch took 144 ms
Inserting 10000 students in batch took 326 ms
Inserting 100000 students in batch took 3855 ms
```


Sharpino is faster than Entity Framework for smaller batches.
(improvement will come with the next version of Sharpino by working on array like data structure rather than lists)


You can find the equivalent example in [sharpino/F#](https://github.com/tonyx/Sharpino/tree/main/Sharpino.Sample.11)
