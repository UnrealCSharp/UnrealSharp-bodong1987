﻿using System.Diagnostics.CodeAnalysis;
using UnrealSharp.Utils.CommandLine;
using UnrealSharp.Utils.Extensions.IO;
using UnrealSharp.Utils.Misc;
using UnrealSharpTool.Core.CodeGen;
using UnrealSharpTool.Core.CppGen;
using UnrealSharpTool.Core.DefCodeGen;
using UnrealSharpTool.Core.TypeInfo;
using UnrealSharpTool.Core.Utils;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace UnrealSharpTool.Processors;

/// <summary>
/// Class BindingCodeExportProcessor.
/// Implements the <see cref="AbstractBaseWorkModeProcessor{BindingCodeExportOptions}" />
/// </summary>
/// <seealso cref="AbstractBaseWorkModeProcessor{BindingCodeExportOptions}" />
[Export("UnrealSharpTools", typeof(IBaseWorkModeProcessor))]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
internal class BindingCodeExportProcessor : AbstractBaseWorkModeProcessor<BindingCodeExportOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindingCodeExportProcessor" /> class.
    /// </summary>
    public BindingCodeExportProcessor() :
        base("codegen")
    {
    }

    /// <summary>
    /// Checks the options.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> success, <c>false</c> otherwise.</returns>
    protected override bool CheckOptions(BindingCodeExportOptions value)
    {
        if (!value.UnrealProjectDirectory.IsDirectoryExists())
        {
            Logger.LogError("generate binding code need your unreal project directory. add argument by -p or --project");
            return false;
        }

        if (!value.InputPath.IsFileExists() && !value.InputPath.IsDirectoryExists())
        {
            Logger.LogError($"Input path: {value.InputPath} does not exist.");
            return false;
        }

        if(value.GenerateSourceType == BindingCodeGenerateSourceType.Assembly && !value.SourceDirectory.IsDirectoryExists())
        {
            Logger.LogError($"Generate from assembly need source directory exists:{value.SourceDirectory}");
            return false;
        }

        return base.CheckOptions(value);
    }

    /// <summary>
    /// Processes the specified value.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>System.Int32.</returns>
    [RequiresDynamicCode("Calls UnrealSharpTool.Core.CodeGen.BindingContext.Create(TypeDefinitionDocument, String, EBindingSchemaType)")]
    protected override int Process(BindingCodeExportOptions options)
    {
        Logger.EnsureNotNull(CommandArguments);

        ITypeDefinitionDocumentInitializeOptions? globalOptions = options.GenerateSourceType switch
        {
            BindingCodeGenerateSourceType.Assembly => LoadExtraOptions<AssemblySearchOptions>(),
            BindingCodeGenerateSourceType.CSharpCode => LoadExtraOptions<CSharpCodeBasedGenerateOptions>(),
            _ => LoadExtraOptions<JsonDocBasedGenerateOptions>()
        };

        Logger.EnsureNotNull(globalOptions);

        Logger.Log("Tool Version: {0}", CoreVersion.Version);
        Logger.Log("Start Generate Binding Code, Mode: {0}", options.GenerateSourceType);
        Logger.Log("Input Source Path:{0}", options.InputPath.CanonicalPath());
        Logger.Log("Unreal Project Path:{0}", options.UnrealProjectDirectory.CanonicalPath());
        Logger.Log("Export Schema Type:{0}", options.Schema);

        Logger.Log("Start Create TypeDefinitionDocument, Please wait...");            

        var document = TypeDefinitionDocument.CreateDocument(
            options.GenerateSourceType,
            options.InputPath,
            globalOptions,
            options.GenerateSourceType == BindingCodeGenerateSourceType.Assembly ?
                EnumerateSourceFileUtils.EnumerateSourceFiles(options.SourceDirectory, options.SourceFileIgnoreRegex) : []
        );

        if (document == null)
        {
            Logger.LogError("No valid input file.");
            return -1;
        }
            
        Logger.Log("Document Generated by Unreal Engine {0}", document.EngineVersion);

        if (options.GenerateSourceType != BindingCodeGenerateSourceType.JsonDoc)
        {
            try
            {
                var tempDocumentDirectory = Path.Combine(options.UnrealProjectDirectory, "Intermediate/UnrealSharp");
                if (!tempDocumentDirectory.IsDirectoryExists())
                {
                    Directory.CreateDirectory(tempDocumentDirectory);
                }

                var tempPath = options.GenerateSourceType == BindingCodeGenerateSourceType.CSharpCode
                    ? Path.Combine(tempDocumentDirectory,
                        options.InputPath.GetDirectoryName() + ".Bindings.Defs.tmp.json")
                    : Path.Combine(tempDocumentDirectory, options.InputPath.GetFileName() + ".tmp.json");

                document.SaveToFile(tempPath);
            }
            catch
            {
                // ignored
            }
        }

        Logger.Log("Create TypeDefinitionDocument Success, Start Export, Please wait...");

        var context = BindingContext.Create(document, options.UnrealProjectDirectory, options.Schema);

        Logger.Log("Document Attribute:{0}", document.Attributes);

        if (options.Schema == EBindingSchemaType.NativeBinding && document.Attributes.HasFlag(ETypeDefinitionDocumentAttributes.AllowFastInvokeGeneration))
        {
            var cppExporter = new CppBindingExporter(context);

            Logger.Log("Enable Unreal C++ Function Fast Invocation, Generate C++ Interop Functions to:{0}", cppExporter.RootDirectory);
                
            if(!cppExporter.Export())
            {
                Logger.LogError("Failed export C++ binding codes.");
                return -1;
            }

            context.FastInvokeFunctions.Clear();

            foreach(var f in cppExporter.ExportableFunctions)
            {
                context.FastInvokeFunctions.Add(f);
            }
        }

        var exporter = new CSharpBindingExporter(context);

        if (!exporter.Export())
        {
            Logger.LogError("Failed export binding codes.");
            return -1;
        }

        Logger.Log("Export Placeholder code, Please wait...");
                        
        var defExporter = new CSharpBindingDefinitionPlaceholdersExporter(context);

        if (!defExporter.Export())
        {
            Logger.LogError("Failed export def codes.");
            return -1;
        }

        Logger.Log("Success.");

        return 0;
    }
}

#region Options
/// <summary>
/// Class BindingCodeExportOptions.
/// Implements the <see cref="AssemblySearchOptions" />
/// </summary>
/// <seealso cref="AssemblySearchOptions" />
internal class BindingCodeExportOptions
{
    /// <summary>
    /// Gets or sets the type of the generate source.
    /// </summary>
    /// <value>The type of the generate source.</value>
    [Option('t', "type", Required = true, HelpText = "your generate type:Assembly,JsonDocument,CSharpCodes")]
    public BindingCodeGenerateSourceType GenerateSourceType { get; set; } = BindingCodeGenerateSourceType.JsonDoc;

    /// <summary>
    /// Gets or sets the project directory.
    /// </summary>
    /// <value>The project directory.</value>
    [Option('p', "project", HelpText = "unreal project directory, your .uproject file location.")]
    public string UnrealProjectDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the input paths.
    /// </summary>
    /// <value>The input paths.</value>
    [Option('i', "input", Required = true, HelpText = "Input file path or directory path, based on generate type.")]
    public string InputPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source directory.
    /// </summary>
    /// <value>The source directory.</value>
    [Option("sourceDirectory", Required = true, HelpText = "source file directory")]
    public string SourceDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ignore regex.
    /// </summary>
    /// <value>The ignore regex.</value>
    [Option("sourceFileIgnoreRegex", Required = false, HelpText = "used to filter source files")]
    public string SourceFileIgnoreRegex { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the schema.
    /// </summary>
    /// <value>The schema.</value>
    [Option('s', "schema", HelpText = "Schema: NativeBinding/BlueprintBinding/CSharpBinding")]
    public EBindingSchemaType Schema { get; set; } = EBindingSchemaType.CSharpBinding;
}
#endregion