﻿using Logging;
using Utils;

namespace KubernetesWorkflow
{
    public class WorkflowCreator
    {
        private readonly NumberSource numberSource = new NumberSource(0);
        private readonly NumberSource containerNumberSource = new NumberSource(0);
        private readonly KnownK8sPods knownPods = new KnownK8sPods();
        private readonly K8sCluster cluster;
        private readonly ILog log;
        private readonly string testNamespace;

        public WorkflowCreator(ILog log, Configuration configuration, string testNamespace)
        {
            cluster = new K8sCluster(configuration);
            this.log = log;
            this.testNamespace = testNamespace.ToLowerInvariant();
        }

        public IStartupWorkflow CreateWorkflow()
        {
            var workflowNumberSource = new WorkflowNumberSource(numberSource.GetNextNumber(),
                                                                    containerNumberSource);

            return new StartupWorkflow(log, workflowNumberSource, cluster, knownPods, testNamespace);
        }
    }
}
