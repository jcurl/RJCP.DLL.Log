# Test Cases for DltDump

Normally to execute all test cases on the console, run

```cmd
dotnet test dltdumptest.dll
```

If you're not in the directory where the DLL is, provide the full path. It will
*not* test the integration tests which are marked as explicit.$$

## Integration Tests

Integration tests are those that might cause problems when running on a CI,
because they have timing dependencies, or depend on other OS resources. In
particular, the following tests are marked as explicit:

* Network tests using TCP
  * TcpClientStream
  * TcpServer (test code)

To run the integration tests on the console

```cmd
dotnet test dltdumptest.dll --filter "FullyQualifiedName~RJCP.App.DltDump"
```

## Code Coverage

To capture code coverage

```cmd
dotnet tool install --global dotnet-coverage
```

Then to get the code coverage for everything, including the explicit tests

```cmd
dotnet coverage collect "dotnet test dltdumptest.dll --filter FullyQualifiedName~RJCP.App.DltDump"
```
