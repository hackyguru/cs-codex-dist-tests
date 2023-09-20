﻿using CodexPlugin;
using DistTestCore;
using KubernetesWorkflow;
using NUnit.Framework;
using Utils;

namespace Tests.BasicTests
{
    [TestFixture]
    public class TwoClientTests : DistTest
    {
        [Test]
        public void TwoClientTest()
        {
            var group = Ci.StartCodexNodes(2);

            var primary = group[0];
            var secondary = group[1];

            PerformTwoClientTest(primary, secondary);
        }

        [Test]
        public void TwoClientsTwoLocationsTest()
        {
            var primary = Ci.StartCodexNode(s => s.At(Location.One));
            var secondary = Ci.StartCodexNode(s => s.At(Location.Two));

            PerformTwoClientTest(primary, secondary);
        }

        private void PerformTwoClientTest(ICodexNode primary, ICodexNode secondary)
        {
            PerformTwoClientTest(primary, secondary, 1.MB());
        }

        private void PerformTwoClientTest(ICodexNode primary, ICodexNode secondary, ByteSize size)
        {
            primary.ConnectToPeer(secondary);

            var testFile = GenerateTestFile(size);

            var contentId = primary.UploadFile(testFile);

            var downloadedFile = secondary.DownloadContent(contentId);

            testFile.AssertIsEqual(downloadedFile);
        }
    }
}
