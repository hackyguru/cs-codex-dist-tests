﻿using CodexDistTestCore;
using NUnit.Framework;

namespace Tests.BasicTests
{
    [TestFixture]
    public class SimpleTests : DistTest
    {
        [Test]
        public void GetDebugInfo()
        {
            var dockerImage = new CodexDockerImage();

            var node = SetupCodexNodes(1).BringOnline()[0];

            var debugInfo = node.GetDebugInfo();

            Assert.That(debugInfo.spr, Is.Not.Empty);
            Assert.That(debugInfo.codex.revision, Is.EqualTo(dockerImage.GetExpectedImageRevision()));
        }

        [Test]
        public void OneClientTest()
        {
            var primary = SetupCodexNodes(1)
                                .WithLogLevel(CodexLogLevel.Trace)
                                .WithStorageQuota(2.MB())
                                .BringOnline()[0];

            var testFile = GenerateTestFile(1.MB());

            var contentId = primary.UploadFile(testFile);

            var downloadedFile = primary.DownloadContent(contentId);

            testFile.AssertIsEqual(downloadedFile);
        }

        //[Test]
        //public void TwoClientTest()
        //{
        //    var primary = SetupCodexNode()
        //                        .WithLogLevel(CodexLogLevel.Trace)
        //                        .WithStorageQuota(1024 * 1024 * 2)
        //                        .BringOnline();

        //    var secondary = SetupCodexNode()
        //                        .WithLogLevel(CodexLogLevel.Trace)
        //                        .WithBootstrapNode(primary)
        //                        .BringOnline();

        //    var testFile = GenerateTestFile(1024 * 1024);

        //    var contentId = primary.UploadFile(testFile);

        //    var downloadedFile = secondary.DownloadContent(contentId);

        //    testFile.AssertIsEqual(downloadedFile);
        //}
    }
}
