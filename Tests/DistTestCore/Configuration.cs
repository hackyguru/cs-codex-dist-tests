﻿using Core;
using KubernetesWorkflow;

namespace DistTestCore
{
    public class Configuration
    {
        private readonly string? kubeConfigFile;
        private readonly string logPath;
        private readonly bool logDebug;
        private readonly string dataFilesPath;

        public Configuration()
        {
            kubeConfigFile = GetNullableEnvVarOrDefault("KUBECONFIG", null);
            logPath = GetEnvVarOrDefault("LOGPATH", "CodexTestLogs");
            logDebug = true;// GetEnvVarOrDefault("LOGDEBUG", "false").ToLowerInvariant() == "true";
            dataFilesPath = GetEnvVarOrDefault("DATAFILEPATH", "TestDataFiles");
        }

        public Configuration(string? kubeConfigFile, string logPath, bool logDebug, string dataFilesPath)
        {
            this.kubeConfigFile = kubeConfigFile;
            this.logPath = logPath;
            this.logDebug = logDebug;
            this.dataFilesPath = dataFilesPath;
        }

        public KubernetesWorkflow.Configuration GetK8sConfiguration(ITimeSet timeSet, string k8sNamespace)
        {
            return GetK8sConfiguration(timeSet, new DoNothingK8sHooks(), k8sNamespace);
        }

        public KubernetesWorkflow.Configuration GetK8sConfiguration(ITimeSet timeSet, IK8sHooks hooks, string k8sNamespace)
        {
            var config = new KubernetesWorkflow.Configuration(
                kubeConfigFile: kubeConfigFile,
                operationTimeout: timeSet.K8sOperationTimeout(),
                retryDelay: timeSet.WaitForK8sServiceDelay(),
                kubernetesNamespace: k8sNamespace
            );

            config.AllowNamespaceOverride = false;
            config.Hooks = hooks;

            return config;
        }

        public Logging.LogConfig GetLogConfig()
        {
            return new Logging.LogConfig(logPath, debugEnabled: logDebug);
        }

        public string GetFileManagerFolder()
        {
            return dataFilesPath;
        }

        private static string GetEnvVarOrDefault(string varName, string defaultValue)
        {
            var v = Environment.GetEnvironmentVariable(varName);
            if (v == null) return defaultValue;
            return v;
        }

        private static string? GetNullableEnvVarOrDefault(string varName, string? defaultValue)
        {
            var v = Environment.GetEnvironmentVariable(varName);
            if (v == null) return defaultValue;
            return v;
        }
    }
}
