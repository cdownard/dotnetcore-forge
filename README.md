#dotnetcore-forge
An opinionated helper for dotnetcore projects. Extends the dotnet cli tools to have rails like code generation. 

###Building
First install dotnet core by following microsoft's instructions [found here](https://www.microsoft.com/net/core)

~~~
cd src/dotnet-forge/
dotnet restore
dotnet build

cd ../../test/dotnet-forge.tests/
dotnet restore
dotnet build
~~~

###Testing
Execute tests using the dotnet test command

~~~
cd test/dotnet-forge.tests/
dotnet test
~~~ 
    
