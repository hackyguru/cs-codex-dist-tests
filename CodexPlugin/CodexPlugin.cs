﻿using Core;
using KubernetesWorkflow;

namespace CodexPlugin
{
    public class CodexPlugin : IProjectPlugin, IHasLogPrefix, IHasMetadata
    {
        private readonly CodexStarter codexStarter;
        private readonly IPluginTools tools;
        private readonly CodexLogLevel defaultLogLevel = CodexLogLevel.Trace;

        public CodexPlugin(IPluginTools tools)
        {
            codexStarter = new CodexStarter(tools);
            this.tools = tools;
        }

        public string LogPrefix => "(Codex) ";

        public void Announce()
        {
            tools.GetLog().Log($"Loaded with Codex ID: '{codexStarter.GetCodexId()}'");
        }

        public void AddMetadata(IAddMetadata metadata)
        {
            metadata.Add("codexid", codexStarter.GetCodexId());
        }

        public void Decommission()
        {
        }

        public RunningContainers[] StartCodexNodes(int numberOfNodes, Action<ICodexSetup> setup)
        {
            var codexSetup = new CodexSetup(numberOfNodes);
            codexSetup.LogLevel = defaultLogLevel;
            setup(codexSetup);
            return codexStarter.BringOnline(codexSetup);
        }

        public ICodexNodeGroup WrapCodexContainers(CoreInterface coreInterface, RunningContainers[] containers)
        {
            return codexStarter.WrapCodexContainers(coreInterface, containers);
        }
    }
}
