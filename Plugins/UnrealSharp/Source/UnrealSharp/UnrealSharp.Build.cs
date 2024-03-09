﻿/*
	MIT License

	Copyright (c) 2024 UnrealSharp

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.

	Project URL: https://github.com/bodong1987/UnrealSharp
*/
using System.Collections.Generic;
using System.IO;
using UnrealBuildTool;

/// <summary>
/// This Module is the core of the UnrealSharp runtime. 
/// It is responsible for starting and managing the C# runtime, 
/// forwarding C# type UFunction function calls to the C# runtime for execution, 
/// and converting the return value into the Unreal type and returning it to Unreal. 
/// At the same time, he is also responsible for forwarding function calls for Unreal in C# to Unreal's virtual machine or Unreal's C++ code.
/// </summary>
public class UnrealSharp : ModuleRules
{
	// try use CoreCLR if it is Available
	private bool bUseCoreCLRIfAvailable = false;

	private string DotnetVersion = "net8.0";

	public enum SharpRuntimeType
	{
		CoreCLR,
		Mono
	}

	public readonly SharpRuntimeType RuntimeType;


	public UnrealSharp(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

		if((Target.Platform == UnrealTargetPlatform.Win64 || 
			Target.Platform == UnrealTargetPlatform.Mac ||
			Target.Platform == UnrealTargetPlatform.Linux ||
			Target.Platform == UnrealTargetPlatform.LinuxArm64) &&
			bUseCoreCLRIfAvailable
			)
		{
			RuntimeType = SharpRuntimeType.CoreCLR;
		}
		else
		{
			RuntimeType = SharpRuntimeType.Mono;
		}

		PublicDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
				"CoreUObject",
				"Engine",
                "DeveloperSettings"
				// ... add other public dependencies that you statically link with here ...
			}
		);

		if(RuntimeType == SharpRuntimeType.CoreCLR)
		{
			AddCoreCLREnvironment();
		}
		else
		{
			AddMonoEnvironment();
		}

		PublicDefinitions.Add($"DOTNET_VERSION=\"{DotnetVersion}\"");
	}

	private void AddCoreCLREnvironment()
	{
		PublicDefinitions.Add("WITH_CORECLR=1");
		PublicDefinitions.Add("WITH_MONO=0");

		throw new System.Exception("Unsupport now.");
	}

	private string GetPlatformName()
	{
		if(Target.Platform == UnrealTargetPlatform.Win64)
		{			
			return "windows";
		}
		else if(Target.Platform == UnrealTargetPlatform.Mac)
		{
			return "osx";
		}
		else if(Target.Platform == UnrealTargetPlatform.IOS)
		{
			return "ios";
		}
		else if(Target.Platform == UnrealTargetPlatform.Android)
		{
			return "android";
		}
		else if(Target.Platform == UnrealTargetPlatform.Linux || Target.Platform == UnrealTargetPlatform.LinuxArm64)
		{
			return "linux";
		}
		else if(Target.Platform == UnrealTargetPlatform.TVOS)
		{
			return "tvos";
		}

		return Target.Platform.ToString();
	}

	private string GetArchTypeName()
	{
		return Target.Architecture.ToString();
	}

	private string GetCoreCLRLibName()
	{
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			return "coreclr.dll";
		}
		else if(Target.Platform == UnrealTargetPlatform.Mac ||
			Target.Platform == UnrealTargetPlatform.IOS||
			Target.Platform == UnrealTargetPlatform.TVOS)
		{
			return "libcoreclr.dylib";
		}

		return "libcoreclr.so";
	}

	private string GetModuleExtension()
	{
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			return ".dll";
		}
		else if (Target.Platform == UnrealTargetPlatform.Mac ||
			Target.Platform == UnrealTargetPlatform.IOS ||
			Target.Platform == UnrealTargetPlatform.TVOS)
		{
			return ".dylib";
		}

		return ".so";
	}

	private string GetModulePrefix()
	{
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			return "";
		}

		return "lib";
	}


	private void AddMonoEnvironment()
	{
		System.Console.WriteLine("Add Mono Environment...");

		PublicDefinitions.Add("WITH_CORECLR=0");
		PublicDefinitions.Add("WITH_MONO=1");

		// change this if you want to debug mono code
		System.Console.WriteLine($"BuildConfiguration:{Target.Configuration}");

		bool bIsDebug = Target.Configuration.ToString().Contains("DEBUG", System.StringComparison.CurrentCultureIgnoreCase);
		string configurationTag = bIsDebug ? "debug" : "release";
		string configurationTagRaw = bIsDebug ? "Debug" : "Release";
		string platformTag = GetPlatformName();
		string archTypeTag = GetArchTypeName();

        if (Target.Platform == UnrealTargetPlatform.Win64 || 
			Target.Platform == UnrealTargetPlatform.Mac)
		{
            string platformDirectoryName = $"{platformTag}.{archTypeTag}.{configurationTag}";
            string managedRuntimeDirectoryName = $"{DotnetVersion}-{platformTag}-{configurationTagRaw}-{archTypeTag}";
			System.Console.WriteLine($"Platform Directory:{platformDirectoryName}");
			System.Console.WriteLine($"Managed Runtime Directory:{managedRuntimeDirectoryName}");

			string NativeLibDirectoryRelativePath = $"ThirdParty/mono/{platformDirectoryName}";
			string SystemManagedLibDirectoryRelativePath = $"ThirdParty/runtime/{managedRuntimeDirectoryName}";

			System.Console.WriteLine($"Native library relative path:{NativeLibDirectoryRelativePath}");
			System.Console.WriteLine($"System managed library relative path:{SystemManagedLibDirectoryRelativePath}");

			PublicDefinitions.Add($"UNREALSHARP_NATIVE_LIBDIRECTORY_RELATIVE_PATH=\"{NativeLibDirectoryRelativePath}\"");
            PublicDefinitions.Add($"UNREALSHARP_SYSTEM_MANAGED_LIBDIRECTORY_RELATIVE_PATH=\"{SystemManagedLibDirectoryRelativePath}\"");
			PublicDefinitions.Add($"UNREALSHARP_CORECLR_LIBNAME=\"{GetCoreCLRLibName()}\"");

			string IncludePath = Path.GetFullPath(Path.Combine(ModuleDirectory, $"../../ThirdParty/mono/{platformDirectoryName}/include/mono-2.0/"));
            PublicIncludePaths.Add(IncludePath);

            List<string> runtimeLibs = new List<string>()
                {
                    Path.Combine(ModuleDirectory, $"../../ThirdParty/mono/{platformDirectoryName}/{GetModulePrefix()}coreclr{GetModuleExtension()}"),
                    Path.Combine(ModuleDirectory, $"../../ThirdParty/mono/{platformDirectoryName}/System.Private.CoreLib.dll")
                };

            if (bIsDebug)
            {
                runtimeLibs.Add(Path.Combine(ModuleDirectory, $"../../ThirdParty/mono/{platformDirectoryName}/coreclr.pdb"));
                runtimeLibs.Add(Path.Combine(ModuleDirectory, $"../../ThirdParty/mono/{platformDirectoryName}/System.Private.CoreLib.pdb"));
            }

            foreach (var managedRuntimeFile in Directory.GetFiles(Path.Combine(ModuleDirectory, $"../../ThirdParty/runtime/{managedRuntimeDirectoryName}")))
            {
                if (managedRuntimeFile.EndsWith(".dll", System.StringComparison.CurrentCultureIgnoreCase) || bIsDebug)
                {
                    runtimeLibs.Add(managedRuntimeFile);
                }
            }

            foreach (var file in runtimeLibs)
            {
                RuntimeDependencies.Add(file);
            }

            var ManagedDir = Path.Combine(ModuleDirectory, "../../../../Managed/");

            if (Directory.Exists(ManagedDir))
            {
                foreach (var file in Directory.GetFiles(ManagedDir))
                {
                    string extension = Path.GetExtension(file);
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    if ((extension == ".dll" || extension == ".json"))
                    {
                        RuntimeDependencies.Add(file);
                    }
                }
            }
        }
        else if(Target.Platform == UnrealTargetPlatform.Mac ||
			Target.Platform == UnrealTargetPlatform.IOS ||
			Target.Platform == UnrealTargetPlatform.TVOS) 
		{
			// temp... it will be not working
			PublicDefinitions.Add($"MONO_LIBRARY_NAME=\"libmonosgen-2.0.dylib\"");
		}
		else
		{
			// temp... it will be not working
			PublicDefinitions.Add($"MONO_LIBRARY_NAME=\"libmonosgen-2.0.so\"");
		}
	}
}
