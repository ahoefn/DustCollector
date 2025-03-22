# Code Structure

The tests are run simply by running 

    dotnet run
  
in the Tests directory. This runs the Main function in [TestProgram](https://github.com/ahoefn/DustCollector/blob/main/Tests/TestProgram.cs) where instances of a [PositionTester](https://github.com/ahoefn/DustCollector/blob/main/Tests/Tester/PositionTester.cs), a [VelocityTester](https://github.com/ahoefn/DustCollector/blob/main/Tests/Tester/VelocityTester.cs)
as well as a [ForceTester](https://github.com/ahoefn/DustCollector/blob/main/Tests/Tester/ForceTester.cs). In the constructers of these classes, all the tests are run automatically.
