﻿namespace RJCP.App.DltDump.View
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Application;
    using Domain;
    using RJCP.Diagnostics.Log.Dlt;
    using RJCP.Diagnostics.Log.Dlt.NonVerbose.Fibex;
    using Services;

    public class FilterCommand : ICommand
    {
        private readonly CmdOptions m_Options;

        public FilterCommand(CmdOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            m_Options = options;
        }

        public ExitCode Run()
        {
            FilterConfig config = new(m_Options.Arguments) {
                ShowPosition = m_Options.Position,
                InputFormat = m_Options.InputFormat,
                OutputFileName = m_Options.OutputFileName,
                Force = m_Options.Force,
                Split = m_Options.Split,
                ConvertNonVerbose = m_Options.NonVerboseWriteVerbose,
                OutputFormat = OutputFormat.Automatic,
                ConnectRetries = m_Options.ConnectRetries,
                BeforeContext = m_Options.BeforeContext,
                AfterContext = m_Options.AfterContext
            };

            // Load the Fibex files, and ensure that they're consistent.
            if (m_Options.Fibex.Count > 0) {
                FibexOptions fibexOptions = FibexService.GetOptions(m_Options);
                config.FrameMap = FibexService.LoadFrameMap(m_Options.Fibex, fibexOptions);
                if (config.FrameMap is null)
                    return ExitCode.FibexError;
            }
            BuildFilter(m_Options, config);
            FilterApp app = new(config);

            Task<ExitCode> filterTask = app.Run();
            filterTask.ConfigureAwait(false);
            return filterTask.GetAwaiter().GetResult();
        }

        private static void BuildFilter(CmdOptions options, FilterConfig config)
        {
            if (options.None) {
                config.None();
                return;
            }

            HashSet<string> identifiers = new();
            HashSet<int> sessionIds = new();
            HashSet<int> messageIds = new();
            HashSet<DltType> types = new();

            foreach (string ecuId in options.EcuId) {
                if (identifiers.Contains(ecuId)) continue;
                config.AddEcuId(ecuId);
                identifiers.Add(ecuId);
            }
            identifiers.Clear();

            foreach (string appId in options.AppId) {
                if (identifiers.Contains(appId)) continue;
                config.AddAppId(appId);
                identifiers.Add(appId);
            }
            identifiers.Clear();

            foreach (string ctxId in options.CtxId) {
                if (identifiers.Contains(ctxId)) continue;
                config.AddCtxId(ctxId);
                identifiers.Add(ctxId);
            }
            identifiers.Clear();

            foreach (int session in options.SessionId) {
                if (sessionIds.Contains(session)) continue;
                config.AddSessionId(session);
                sessionIds.Add(session);
            }
            sessionIds.Clear();

            foreach (int messageId in options.MessageId) {
                if (messageIds.Contains(messageId)) continue;
                config.AddMessageId(messageId);
                messageIds.Add(messageId);
            }
            messageIds.Clear();

            foreach (DltType type in options.DltTypeFilters) {
                if (types.Contains(type)) continue;
                config.AddMessageType(type);
                types.Add(type);
            }
            types.Clear();

            config.AddTimeRange(options.NotBefore, options.NotAfter);

            foreach (string search in options.SearchString) {
                config.AddSearchString(search, options.IgnoreCase);
            }

            foreach (string regex in options.SearchRegex) {
                config.AddRegexString(regex, options.IgnoreCase);
            }

            if (options.VerboseMessage) config.SetVerbose();
            if (options.NonVerboseMessage) config.SetNonVerbose();
            if (options.ControlMessage) config.SetControlMessage();
        }
    }
}
