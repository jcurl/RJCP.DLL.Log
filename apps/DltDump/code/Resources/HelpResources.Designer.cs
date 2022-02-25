﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RJCP.App.DltDump.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class HelpResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal HelpResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RJCP.App.DltDump.Resources.HelpResources", typeof(HelpResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DltDump parses DLT Version 1 log files, originating either from a file, serial or TCP connection..
        /// </summary>
        internal static string Help100_Description {
            get {
                return ResourceManager.GetString("Help100_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To run the tool execute it with the optional [options] and the mandatory &lt;input&gt;. The following command line options are available:.
        /// </summary>
        internal static string Help200_UsageInfo {
            get {
                return ResourceManager.GetString("Help200_UsageInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dltdump.exe {0}?.
        /// </summary>
        internal static string Help210_SimpleUsageScheme {
            get {
                return ResourceManager.GetString("Help210_SimpleUsageScheme", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to dltdump.exe [options] &lt;input&gt;.
        /// </summary>
        internal static string Help210_UsageScheme {
            get {
                return ResourceManager.GetString("Help210_UsageScheme", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Options:.
        /// </summary>
        internal static string Help300_Options {
            get {
                return ResourceManager.GetString("Help300_Options", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}? | {1}help
        /// displays this help message..
        /// </summary>
        internal static string Help310_HelpOption {
            get {
                return ResourceManager.GetString("Help310_HelpOption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}version
        /// displays the version of the software..
        /// </summary>
        internal static string Help315_VersionOption {
            get {
                return ResourceManager.GetString("Help315_VersionOption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The input stream shall be the last option in the command line. Multiple files can be specified..
        /// </summary>
        internal static string Help400_Input {
            get {
                return ResourceManager.GetString("Help400_Input", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The exit code of the application indicates if the command run with success or there was a problem..
        /// </summary>
        internal static string Help800_ExitCodes {
            get {
                return ResourceManager.GetString("Help800_ExitCodes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 0 - The program ran successfully.
        /// </summary>
        internal static string Help805_Success {
            get {
                return ResourceManager.GetString("Help805_Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 1 - There was an error processing the options.
        /// </summary>
        internal static string Help810_OptionError {
            get {
                return ResourceManager.GetString("Help810_OptionError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 2 - None of the input files could be processed.
        /// </summary>
        internal static string Help811_NoFilesProcessed {
            get {
                return ResourceManager.GetString("Help811_NoFilesProcessed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 3 - Not all of the files could be processed (partial error).
        /// </summary>
        internal static string Help812_PartialFilesProcessed {
            get {
                return ResourceManager.GetString("Help812_PartialFilesProcessed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 255 - An unhandled exception occurred.
        /// </summary>
        internal static string Help899_UnknownError {
            get {
                return ResourceManager.GetString("Help899_UnknownError", resourceCulture);
            }
        }
    }
}
