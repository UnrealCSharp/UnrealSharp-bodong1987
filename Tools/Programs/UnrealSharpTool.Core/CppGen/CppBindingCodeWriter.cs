﻿using UnrealSharp.Utils.Extensions.IO;
using UnrealSharpTool.Core.Generation;

namespace UnrealSharpTool.Core.CppGen;

internal class CppBindingCodeWriter : CSharpCodeWriter
{
    public CppBindingCodeWriter(string path) : 
        base(path) 
    {
        if(!Directory.Exists(path.GetDirectoryPath()))
        {
            Directory.CreateDirectory(path.GetDirectoryPath());
        }

        WriteCommonComment("These codes are automatically generated by UnrealSharpTool in order to realize fast function calls from C# to C++. \nPlease do not modify this file manually.");
        WriteNewLine();
    }
}