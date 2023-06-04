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
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
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
        ///   Looks up a localized string similar to {1}position
        ///  Show the offset of the stream on text output..
        /// </summary>
        internal static string Help320_Position {
            get {
                return ResourceManager.GetString("Help320_Position", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}format{2}&lt;format&gt;
        ///  Defines the input format. This can be one of the following (by default it is automatic):.
        /// </summary>
        internal static string Help325_InputFormat {
            get {
                return ResourceManager.GetString("Help325_InputFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * automatic|auto - choose based on the URI, or the file name extension..
        /// </summary>
        internal static string Help325_InputFormatAuto {
            get {
                return ResourceManager.GetString("Help325_InputFormatAuto", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * file - interpret the URI as a file with a storage header..
        /// </summary>
        internal static string Help325_InputFormatFile {
            get {
                return ResourceManager.GetString("Help325_InputFormatFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * network|net - interpret the URI as a network stream, each packet starting with the standard header..
        /// </summary>
        internal static string Help325_InputFormatNetwork {
            get {
                return ResourceManager.GetString("Help325_InputFormatNetwork", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * pcap|pcapng - interpret the URI as a PCAP or PCAPNG file.
        /// </summary>
        internal static string Help325_InputFormatPcap {
            get {
                return ResourceManager.GetString("Help325_InputFormatPcap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * serial|ser - interpret the URI as a serial stream with a DLS\1 header..
        /// </summary>
        internal static string Help325_InputFormatSerial {
            get {
                return ResourceManager.GetString("Help325_InputFormatSerial", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}retries{2}&lt;count&gt;
        ///  For input URIs that need a connection (e.g. tcp), specify the number of retries when connecting. When the retries is negative, attempts go on forever. When the retries is more than zero, it tries &apos;n&apos; times again. If retries are enabled, an attempt to reconnect after the stream is closed is always made until the connection attempts fails. This allows for connecting to remote targets that reset forever, and ending only if the remote target doesn&apos;t recover..
        /// </summary>
        internal static string Help330_Retries {
            get {
                return ResourceManager.GetString("Help330_Retries", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}s | {1}string{2}&lt;string&gt;
        ///  Search for lines that have the text content containing &lt;string&gt;..
        /// </summary>
        internal static string Help335_StringSearch {
            get {
                return ResourceManager.GetString("Help335_StringSearch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}r | {1}regex{2}&lt;regex&gt;
        ///  Search for lines that match the .NET regular expression for &lt;regex&gt;..
        /// </summary>
        internal static string Help340_RegexSearch {
            get {
                return ResourceManager.GetString("Help340_RegexSearch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}i | {1}ignorecase
        ///  If provided, the searches are made case insensitive..
        /// </summary>
        internal static string Help345_IgnoreCase {
            get {
                return ResourceManager.GetString("Help345_IgnoreCase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}ecuid{2}&lt;id1&gt;[,&lt;id2&gt;[,...]]
        ///  Specify a list of ECU Identifiers that should match..
        /// </summary>
        internal static string Help350_EcuId {
            get {
                return ResourceManager.GetString("Help350_EcuId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}appid{2}&lt;id1&gt;[,&lt;id2&gt;[,...]]
        ///  Specify a list of Application Identifiers that should match..
        /// </summary>
        internal static string Help355_AppId {
            get {
                return ResourceManager.GetString("Help355_AppId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}ctxid{2}&lt;id1&gt;[,&lt;id2&gt;[,...]]
        ///  Specify a list of context identifiers that should match..
        /// </summary>
        internal static string Help360_CtxId {
            get {
                return ResourceManager.GetString("Help360_CtxId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}sessionid{2}&lt;id1&gt;[,&lt;id2&gt;[,...]]
        ///  Specify a list of session identifiers that should match. Lines that don&apos;t have a session identifier do not match..
        /// </summary>
        internal static string Help365_SessionId {
            get {
                return ResourceManager.GetString("Help365_SessionId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}type{2}&lt;types&gt;
        ///  Filter for specific message types. Types are:
        ///  * Log: fatal,error,warn,info,debug,verbose
        ///  * Network: ipc,can,flexray,network,ethernet.someip,user1,..,user9
        ///  * Control: request,response,time
        ///  * Trace: variable,functionin,functionout,state,vfb
        ///  You can use an integer 0-254 for comparison for other values, e.g. 48 is the same as &apos;warn&apos;..
        /// </summary>
        internal static string Help370_DltTypes {
            get {
                return ResourceManager.GetString("Help370_DltTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}verbose
        ///  Match lines that are verbose (have an extended header and the verbose bit is set)..
        /// </summary>
        internal static string Help375_Verbose {
            get {
                return ResourceManager.GetString("Help375_Verbose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}nonverbose
        ///  Match lines that are nonverbose (have no extended header, or where the verbose bit is not set and is not a control message)..
        /// </summary>
        internal static string Help380_NonVerbose {
            get {
                return ResourceManager.GetString("Help380_NonVerbose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}control
        ///  Match lines that are a control message (including the timing message)..
        /// </summary>
        internal static string Help385_Control {
            get {
                return ResourceManager.GetString("Help385_Control", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}messageid{2}value, value, value
        /// filters for the specific message identifiers given.
        /// </summary>
        internal static string Help386_MessageId {
            get {
                return ResourceManager.GetString("Help386_MessageId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}not-after{2}&lt;datetime&gt;
        ///  Filter for messages that are at this date/time and earlier. Local time can be given as YYYY-MM-DDTHH:MM:SS (2022-06-18T10:30:00), universal time can be given as  YY-MM-DDZHH:MM:SS (2022-06-18Z08:30:00)..
        /// </summary>
        internal static string Help390_DateNotAfter {
            get {
                return ResourceManager.GetString("Help390_DateNotAfter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}not-before{2}&lt;datetime&gt;
        ///  Filter for messages that are at this date/time and later. Time is in Local Time, unless {0}utc is given..
        /// </summary>
        internal static string Help395_DateNotBefore {
            get {
                return ResourceManager.GetString("Help395_DateNotBefore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}B | {1}before-context{2}&lt;lines&gt;
        ///  On a match, print &lt;lines&gt; of text before the match..
        /// </summary>
        internal static string Help400_BeforeContext {
            get {
                return ResourceManager.GetString("Help400_BeforeContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}A | {1}after-context{2}&lt;lines&gt;
        ///  On a match, print &lt;lines&gt; of text after the match..
        /// </summary>
        internal static string Help405_AfterContext {
            get {
                return ResourceManager.GetString("Help405_AfterContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}o | {1}output{2}&lt;filename&gt;
        ///  Write the output to the file specified. The file format is given by the extension (.dlt for DLT with storage header, all others are text files). If not provided, write to the console. When writing text files, the {0}position argument can optionally prepend each line with the offset from the input stream. The output file name may contain substitution with %VAR%, where the name is an environment variable which will be substituted. Some special variables are allowed:.
        /// </summary>
        internal static string Help410_OutputFileName {
            get {
                return ResourceManager.GetString("Help410_OutputFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * %FILE% - use the name of the input file.
        /// </summary>
        internal static string Help411_OutputFile {
            get {
                return ResourceManager.GetString("Help411_OutputFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * %CDATETIME% - use the local date/time of the first line.
        /// </summary>
        internal static string Help412_OutputDateTime {
            get {
                return ResourceManager.GetString("Help412_OutputDateTime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * %CDATE% - use the local date of the first line.
        /// </summary>
        internal static string Help413_OuputDate {
            get {
                return ResourceManager.GetString("Help413_OuputDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * %CTIME% - use the local time of the first line.
        /// </summary>
        internal static string Help414_OutputTime {
            get {
                return ResourceManager.GetString("Help414_OutputTime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to * %CTR% - use a counter for how many times this output file has been split with the option {1}split{2}&lt;bytes&gt;.
        /// </summary>
        internal static string Help415_OutputSplit {
            get {
                return ResourceManager.GetString("Help415_OutputSplit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}f | {1}force
        ///  When using {1}output, overwrite the output file if it already exists..
        /// </summary>
        internal static string Help420_Force {
            get {
                return ResourceManager.GetString("Help420_Force", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}split{2}&lt;bytes&gt;
        ///  Split the input, so that the output files are approximately the size of bytes that are requested. Output are split so that lines are intact. Standard byte modifiers can be given, e.g. 64kB, 100M, 1Gb, and are case insensitive, the units being in bytes. The smallest split is 64kB..
        /// </summary>
        internal static string Help425_Split {
            get {
                return ResourceManager.GetString("Help425_Split", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}log
        ///  Dump internal logs to a crash dump file. This log file can assist in understand errors while reading, file corruption, etc..
        /// </summary>
        internal static string Help430_Log {
            get {
                return ResourceManager.GetString("Help430_Log", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The input stream shall be the last option in the command line. Multiple files can be specified, or a single URI can be specified. The URIs supported are:.
        /// </summary>
        internal static string Help500_Input {
            get {
                return ResourceManager.GetString("Help500_Input", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to file.dlt or file:///home/user/file.dlt - read a specific file. The time stamps are read from the input file if available..
        /// </summary>
        internal static string Help505_InputFile {
            get {
                return ResourceManager.GetString("Help505_InputFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to file.pcap - read a PCAP legacy recorded file. The timestamps are obtained from the packet capture time which are expected to be UTC..
        /// </summary>
        internal static string Help506_InputFilePcap {
            get {
                return ResourceManager.GetString("Help506_InputFilePcap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to file.pcapng - read a PCAP NG recorded file. Timestamps are obtained from the packet capture time up to nanosecond resolution. All interfaces are parsed. Filter out double captures on multiple interfaces before running..
        /// </summary>
        internal static string Help507_InputFilePcapNg {
            get {
                return ResourceManager.GetString("Help507_InputFilePcapNg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to tcp://&lt;hostname&gt;[:&lt;port&gt;] - connect to a trace server via TCP. If the port is not provided, the default of 3490 is assumed. Time stamps are taken from the local computer..
        /// </summary>
        internal static string Help510_InputTcp {
            get {
                return ResourceManager.GetString("Help510_InputTcp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to udp://&lt;localip|mcast&gt;[:port] - listen on the local IPv4 address specified (for all interfaces, provide 0.0.0.0), or on a multicast IPv4 address in the range 224.0.0.0-239.255.255.255. The default port 3490 is assumed. Time stamps are taken from the local computer..
        /// </summary>
        internal static string Help515_InputUdp {
            get {
                return ResourceManager.GetString("Help515_InputUdp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ser:port,baud,databits,parity,stopbits[,handshake] - receive data arriving via the serial port. Time stamps are taken from the local computer. By default, it is assumed that format is serial, with {1}format{2}ser. Valid values for the handshake are xon,rts,dtr..
        /// </summary>
        internal static string Help520_InputSerial {
            get {
                return ResourceManager.GetString("Help520_InputSerial", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Non-verbose messages need external description files to decode. The following options provide the additional information needed. If these options are used without input files, the files are checked for correctness..
        /// </summary>
        internal static string Help600_NonVerbose {
            get {
                return ResourceManager.GetString("Help600_NonVerbose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}F | {1}fibex{2}&lt;file1&gt;[,&lt;file2&gt;[,...]]
        ///  Specify a list FIBEX files that should be loaded to decode non-verbose.
        /// </summary>
        internal static string Help610_Fibex {
            get {
                return ResourceManager.GetString("Help610_Fibex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}nv-multiecu
        ///  If provided, ensure the decoded ECUID matches the Fibex ECUID. Useful for loading multiple Fibex files from different ECUs at once..
        /// </summary>
        internal static string Help620_NvEcuId {
            get {
                return ResourceManager.GetString("Help620_NvEcuId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}nv-noexthdr
        ///  If provided, ignore the Application and Context identifiers as decoded when mapping to frames in the Fibex file. Useful to ensure uniqueness of message identifiers if it&apos;s known in advance that the captured traces do not have an extended header..
        /// </summary>
        internal static string Help625_NvNoExtHdr {
            get {
                return ResourceManager.GetString("Help625_NvNoExtHdr", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}nv-verbose
        ///  If provided, write non-verbose files as verbose if there is an appropriate FIBEX file provided. The original message identifier will be replaced with the arguments in the output file. Only applicable for converting to DLT files..
        /// </summary>
        internal static string Help630_NvVerbose {
            get {
                return ResourceManager.GetString("Help630_NvVerbose", resourceCulture);
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
        ///   Looks up a localized string similar to 0 - The program ran successfully..
        /// </summary>
        internal static string Help805_Success {
            get {
                return ResourceManager.GetString("Help805_Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 1 - There was an error processing the options..
        /// </summary>
        internal static string Help810_OptionError {
            get {
                return ResourceManager.GetString("Help810_OptionError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 2 - There was an error parsing the input URIs..
        /// </summary>
        internal static string Help811_InputUnknown {
            get {
                return ResourceManager.GetString("Help811_InputUnknown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 3 - None of the input files could be processed..
        /// </summary>
        internal static string Help812_NoFilesProcessed {
            get {
                return ResourceManager.GetString("Help812_NoFilesProcessed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 4 - Not all of the files could be processed (partial error)..
        /// </summary>
        internal static string Help813_PartialFilesProcessed {
            get {
                return ResourceManager.GetString("Help813_PartialFilesProcessed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 5 - There was a problem creating the output file..
        /// </summary>
        internal static string Help814_OutputError {
            get {
                return ResourceManager.GetString("Help814_OutputError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 6 - There was a problem loading the non-verbose FIBEX files..
        /// </summary>
        internal static string Help815_FibexError {
            get {
                return ResourceManager.GetString("Help815_FibexError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 255 - An unhandled exception occurred..
        /// </summary>
        internal static string Help899_UnknownError {
            get {
                return ResourceManager.GetString("Help899_UnknownError", resourceCulture);
            }
        }
    }
}
