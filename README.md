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

If you'd like to build all projects at once from the root of the project, execute the following command:

~~~
dotnet build **/project.json
~~~

This argument can also be specified in the build task for vscode, located in the .vscode/tasks.json directory.

###Testing
Execute tests using the dotnet test command

~~~
cd test/dotnet-forge.tests/
dotnet test
~~~ 
    
