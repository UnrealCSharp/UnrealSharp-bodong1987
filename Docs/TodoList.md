# To-do List
I have roughly estimated the difficulty of these tasks on a 10-point scale. Everyone is welcome to challenge.  

## Add more extension APIs for C# binding types
* Difficulty: &#9734;   
  

In the Bindings.Extends directory of Unreal Sharp.UnrealEngine, you can see many files ending with .extends.cs. These files extend the generated C# code through the C# [partial](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods) mechanism or the [Extension Methods](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods) mechanism, providing more convenient use. Interface.  

<details>
    <summary style="color: green; font-weight: bold;">Click To See Example Codes</summary>
    
```C#
    // FVector.extends.cs
    /// <summary>
    /// Squareds the length.
    /// </summary>
    /// <returns>System.Double.</returns>
    public double SquaredLength()
    {
        return SizeSquared();
    }

    /// <summary>
    /// Determines whether [is nearly zero] [the specified tolerance].
    /// </summary>
    /// <param name="Tolerance">The tolerance.</param>
    /// <returns><c>true</c> if [is nearly zero] [the specified tolerance]; otherwise, <c>false</c>.</returns>
    public bool IsNearlyZero(double Tolerance = UnrealConstants.KindaSmallNumber)
    {
        return System.Math.Abs(X) <= Tolerance && System.Math.Abs(Y) <= Tolerance && System.Math.Abs(Z) <= Tolerance;
    }

    /// <summary>
    /// Determines whether [is nearly equal] [the specified other].
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if [is nearly equal] [the specified other]; otherwise, <c>false</c>.</returns>
    public bool IsNearlyEqual(FVector other)
    {
        return (other - this).IsNearlyZero();
    }
```
```C#
    // AActor.extends.cs
	partial class AActor
	{
		/// <summary>
		/// Gets the world.
		/// </summary>
		/// <returns>System.Nullable&lt;UWorld&gt;.</returns>
		public UWorld? GetWorld()
		{
			return ActorInteropUtils.GetWorld(this);
		}

		/// <summary>
		/// Gets the game instance.
		/// </summary>
		/// <returns>System.Nullable&lt;UGameInstance&gt;.</returns>
		public UGameInstance? GetGameInstance()
		{
			return ActorInteropUtils.GetGameInstance(this);
		}

		/// <summary>
		/// Destroys the actor.
		/// </summary>
		public void DestroyActor()
		{
			K2_DestroyActor();
		}
	}
```
```C#
    // IEnhancedInputSubsystemInterface.Extends.cs
    /// <summary>
    /// Class IEnhancedInputSubsystemInterfaceExtensions.
    /// </summary>
    public static class IEnhancedInputSubsystemInterfaceExtensions
    {
        /// <summary>
        /// Adds the mapping context.
        /// </summary>
        /// <param name="interface">The interface.</param>
        /// <param name="MappingContext">The mapping context.</param>
        /// <param name="Priority">The priority.</param>
        public static void AddMappingContext(this IEnhancedInputSubsystemInterface @interface, UInputMappingContext? MappingContext, int Priority = 0)
        {
            FModifyContextOptions Options = new FModifyContextOptions();

            @interface.AddMappingContext(MappingContext, Priority, ref Options);
        }
    }
``` 
    
</details>  
  
   

These new interfaces may be completed directly in C#, or they may require the help of other C++ interactive interfaces.  
So to make UnrealSharp more usable, we may need more of these extension interfaces, not just the automatically generated ones.  


## Reimplement a better C# version of FText
* Difficulty: &#9733;&#9733;  
Now FText is just a string? Packaging, this may not be a good practice. I hope that FText on the C# side can better match and work together with C++, and can make data changes faster to provide higher function calling efficiency.  


## Implement more structures as FastAccessable
* Difficulty: &#9733;&#9733;  
When the memory size, memory layout, attribute type and number, attribute offset, etc. of the target structure are completely consistent on the C# and C++ sides, and all attributes of the structure are also FastAccessable, the structure is FastAccessable.   
UnrealSharp cannot automatically determine it through the application. Currently, the manual method is used. I add the structure type that meets the conditions to the configuration, and then the generation tool will automatically generate FastAccessable code, such as FVector. However, there are actually many structures that support FastAccessable, which is actually a point that can be optimized. If a structure that was not FastAccessable becomes FastAccessable, then the property access and function calls related to it will be faster.  
If you compile the Debug version of C# code, it will try to analyze and find some structures that may have FastAccessable potential every time it is started.   
<details>
    <summary style="color: green; font-weight: bold;">Click to see current reports</summary>
    Please note that this report may not be accurate:  
    ```log
    Find 207 potential fast access types, Please check the possibility of modifying it so that it can be fast access:
    UnrealSharp.UnrealEngine.FAIRequestID    
    UnrealSharp.UnrealEngine.FAnchorData    
    UnrealSharp.UnrealEngine.FAnchors    
    UnrealSharp.UnrealEngine.FAsyncPhysicsTimestamp    
    UnrealSharp.UnrealEngine.FBoneMirrorInfo    
    UnrealSharp.UnrealEngine.FBox2D    
    UnrealSharp.UnrealEngine.FBox2f    
    UnrealSharp.UnrealEngine.FBox3d    
    UnrealSharp.UnrealEngine.FBox3f    
    // you can find remain types in logs...
    ....
```
You can refer to FVector and FRotator to complete this operation.   

</details>  
  
  
**In addition, special attention needs to be paid to the alignas tag of C++.**   
I don’t seem to have found the matching ability on the C# side, so if memory mapping is performed directly on both ends, there may be memory errors. You need to be careful, such as: FTransform
```C++
template<typename T>
struct alignas( TAlignOfTransform<T>::Value ) TTransform
```




## Performance testing and optimization
* Difficulty: &#9733;&#9733;&#9733;  
You can test all functions, compare with plug-ins in other languages, and try to optimize points where there may be performance problems.  
Before starting the test, be sure to go to the project settings and turn on the `PerformanceMode` option of UnrealSharp so that you can obtain more accurate data.  

## Add more sample programs  
* Difficulty: &#9733;&#9733;&#9733;   
Port some existing sample programs using UnrealSharp and C#

## Test iOS compatibility and issues
* Difficulty: &#9733;&#9733;&#9733;  
We use mono's runtime. In theory, we only need to correctly import mono's API on the mono platform to run. You can try to run C# in interpreted mode, and you can also try to complete aot-related development.

## Test Android compatibility and issues
* Difficulty: &#9733;&#9733;&#9733;&#9733;  

Android is much more open than iOS. In theory, there won't be any obstacles. It just takes some time to complete a small amount of code modifications to be compatible with the corresponding pipeline.

## Test Linux compatibility and issues
* Difficulty: &#9733;&#9733;&#9733;&#9733;  
I don't have a Linux device, so theoretically it should be supported.  

## Add CoreCLR runtime for supported platform
* Difficulty: &#9733;&#9733;&#9733;&#9733;&#9733;&#9733;&#9733; 

At present, only Mono's runtime has been implemented, and a large number of mono's APIs are directly called internally. Consider providing CoreCLR's runtime on supported platforms, that is, only using CoreCLR's interface to create UnrealSharp's runtime.

## Complete research related to AOT release
* Difficulty: &#9733;&#9733;&#9733;&#9733;&#9733;&#9733;&#9733;&#9733;&#9733;   
When originally designed, AOT compatibility was considered. You can see a large number of AOT-related Attributes in the C# code. Therefore, giving UnrealSharp the ability to publish AOT is also a very important component of the entire system.  
This challenge requires completing all compatibility tests and code modifications that support the AOT publishing platform, and adjusting all C# code to ensure compatibility and optimize the code volume as much as possible.  

## Try Solve Known Issues
* Difficulty: &#9733;&#9733;&#9733;&#9733;&#9733;&#9733;&#9733;&#9733;&#9733;&#9733;  
see [KnownIssues](./KnownIssues.md) for more information  
