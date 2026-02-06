using NUnit.Framework;
using Dreamland;

namespace Dreamland.Tests
{
    public class RunbookChecklistTests
    {
        [Test]
        public void ChecklistContainsCoreSteps()
        {
            var list = RunbookChecklist.Steps;
            Assert.IsTrue(System.Array.Exists(list, step => step == "capture"));
            Assert.IsTrue(System.Array.Exists(list, step => step == "upload"));
            Assert.IsTrue(System.Array.Exists(list, step => step == "processing"));
            Assert.IsTrue(System.Array.Exists(list, step => step == "bundle"));
            Assert.IsTrue(System.Array.Exists(list, step => step == "movement"));
        }
    }
}
