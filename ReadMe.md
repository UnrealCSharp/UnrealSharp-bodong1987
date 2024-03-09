# UnrealSharp
UnrealSharp is a plug-in developed for UnrealEngine 5. Through this plug-in, you can use pure C# to develop UnrealEngine 5 projects.  

## Main Features
* Supports `.NET 6.0`~`.NET 8.0` [default is `.NET 8.0`], supports `C#12  `
* Supports creating new `Unreal classes`, `Unreal structures`, `Unreal enumerations`, etc. in C#  
* Supports creating new `Unreal properties`, `Unreal functions`, and `Unreal multicast delegates` for C# class.  
* Supports C# classes to inherit `Unreal C++ classes`, which means you can implement your own `UObject`, `UActorComponent`, and `AActor` subclasses in C#   
* Supports rewriting C++ Event functions in C# classes  
* Support access to all C# classes, structures, enumerations, methods, delegates, etc. from Unreal Blueprints  
* Supports Unreal Blueprint classes inheriting C# classes and overriding C# Events in blueprints  
* Supports automatically generating corresponding C# binding code for Unreal C++ or Unreal Blueprint [optional]. As long as Unreal's classes, structures, enumerations, functions, etc. can be accessed in blueprints, almost all of them can be accessed in C#.  
* Support `debugging` C# code through `Visual Studio` or `Rider`, and support simultaneous debugging of C++ and C#  
* Most C++ APIs are automatically generated as versions based on C# Function Pointers to improve calling speed.  
* This interactive function framework based on C# Function Pointers is public. You can register a new C++ API with the framework if necessary.    
* Automatically align Unreal and C# memory management and object lifetimes so you almost never have to handle anything yourself.  
* The style of C# binding code remains the same as Unreal C++, and there is almost no difference between calling these C++ interfaces and in C++.
* AOT friendly, support for AOT has been considered from the initial design.  
* Pure plug-in design, you don’t need to modify any engine source code  
* In addition to the three special projects reserved by the framework, you can add any number of C# projects, and it also supports implementing corresponding Unreal types in these projects.  


## Supported platforms
| Chip  | Windows  | Linux    | macOS    | IOS      | Android |
| :---- | :------: | :------: | :------: | :------: |:------: |
| x64   | &#x2714; | &#x003F; | &#x2714; | &#x2212; |&#x2716; |
| Arm64 | &#x2716; | &#x2716; | &#x2714; | &#x003F; |&#x003F; |

* &#x2714; Supported and verified
* &#x2716; Not supported and no follow-up plans
* &#x003F; It is theoretically supported or planned to be supported, but it has not yet been verified and may require further testing and modification.  
* &#x2212; target platform is not exists.  
**32-bit architecture is not supported on all platforms**

## How can I use it?
To use this library, you need these few steps, you can click on the link to get the details, or directly check the [video tutorial list on youtube](#youtube-turtorials):  
1. Install [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) for your development platform  
2. Install [Visual Studio](https://visualstudio.microsoft.com/vs/) or [Rider](https://www.jetbrains.com/rider/) [[Visual Studio Code](https://code.visualstudio.com/) has not been tested yet]  
3. Clone or download this project. Copy the Plugins directory and GameScripts directory to your Unreal C++ project (pure Blueprint projects are not supported)  
4. [Prepare UnrealSharp ThirdParty](./Docs/PrepareThirdParty.md)  
5. [Add a dependency on UnrealSharp](./Docs/AddUnrealSharpDependency.md) in Build.cs of your game project
6. Build your Unreal Project and then start UnrealEditor  
7. Export Unreal C++ type database or Unreal Blueprint type database from: Tools>UnrealSharp>Export Database or use console command :
    * UnrealSharp.ExportDatabase
    * UnrealSharp.ExportUnrealCppDatabase
    * UnrealSharp.ExportBlueprintDatabase
8. open GameScripts/UnrealSharp.sln, **[Select your development platform's Configuration](./Docs/CSharpBuildConfiguration.md)**, compile this solution.  
C# binding code and C++ interaction APIs will be automatically generated during the compilation process [if you enable this function]. 
9. If you enable C++ interactive code generation, you need to recompile your C++ code and start Unreal Editor again. The deployment is complete.   
10. The next thing you have to do is [learn how to](#turtorials) add C# code in C# according to the specifications to implement Unreal Class, Unreal Struct, Unreal Enums, etc.  

## YouTube Turtorials
[![How to Add UnrealSharp To UnrealEngine 5](https://img.youtube.com/vi/70CmysenDOM/0.jpg)](https://www.youtube.com/watch?v=70CmysenDOM)  

[Full playlist](https://www.youtube.com/watch?v=70CmysenDOM&list=PL-bgMFxHDR7beg5BE_4MwqkpwqFsbsc4c)

## Turtorials
### Setup
* [How to prepare ThirdParty?](./Docs/PrepareThirdParty.md)
* [Add UnrealSharp to your project's Build.cs](./Docs/AddUnrealSharpDependency.md)

### Development
* [C# projects specifications](./Docs/CSharpProjectsSpecifications.md)
* [C# Programming Specifications](./Docs/CSharpProgrammingSpecifications.md)
* [What types can I use in C#?](./Docs/AllSupportedTypes.md)
* [How to add a new Unreal type in C#?](./Docs/NewUnrealTypes.md)  
* [How to add a UFunction to UClass?](./Docs/NewUnrealFunctionInCSharp.md)
* [About Unreal Collections In C#](./Docs/AboutCollections.md)
* [How to debug UnrealSharp's C# codes?](./Docs/DebugUnrealSharp.md)

### Advanced Topics
* [The principle of UnrealSharp](./Docs/ThePrincipleOfUnrealSharp.md)

## Known Issues
There are some known issues here that may affect you; these issues may or may not be resolved in subsequent plans, so be sure to pay attention.  
[Known Issues](./Docs/KnownIssues.md)  

## To-do List
This is a newly born small project, there is still a lot to do before it can grow into a useful project, but one person's energy is really limited. If you are interested in this, take a look and try to choose a topic to complete. Anyone is welcome to submit a PullRequest, you just need to make sure it makes sense and runs correctly.  
[To-do List](./Docs/TodoList.md)

## Reporting security issues and security bugs  
You can directly visit the [issues page](https://github.com/bodong1987/UnrealSharp/issues) of this project to submit bug reports or suggestions.  

## License
UnrealSharp is licensed under the [MIT](LICENSE) license.






