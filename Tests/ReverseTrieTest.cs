using Emitter;
using Emitter.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class ReverseTrieTest
    {
        [TestMethod]
        public void Match()
        {
            var reverseTrie = new ReverseTrie<MessageHandler>(-1);
            reverseTrie.RegisterHandler("a/", (c, m) => { });
            reverseTrie.RegisterHandler("a/b/c/", (c, m) => { });
            reverseTrie.RegisterHandler("a/+/c/", (c, m) => { });
            reverseTrie.RegisterHandler("a/b/c/d/", (c, m) => { });
            reverseTrie.RegisterHandler("a/+/c/+/", (c, m) => { });
            reverseTrie.RegisterHandler("x/", (c, m) => { });
            reverseTrie.RegisterHandler("x/y/", (c, m) => { });
            reverseTrie.RegisterHandler("x/+/z/", (c, m) => { });

            Dictionary<string, int> tests = new Dictionary<string, int>
            {
                ["a/"] = 1,
                ["a/1/"] = 1,
                ["a/2/"] = 1,
                ["a/1/2/"] = 1,
                ["a/1/2/3/"] = 1,
                ["a/x/y/c/"] = 1,
		        ["a/x/c/"] = 2,
		        ["a/b/c/"] = 3,
		        ["a/b/c/d/"] = 5,
		        ["a/b/c/e/"] = 4,
		        ["x/y/c/e/"] = 2
            };

            foreach (var kv in tests)
            {
                var results = reverseTrie.Match(kv.Key);
                Assert.AreEqual(kv.Value, results.Count);
            }
        }

        [TestMethod]
        public void DeleteParent()
        {
            var reverseTrie = new ReverseTrie<MessageHandler>(-1);
            reverseTrie.RegisterHandler("a/", (c, m) => { });
            reverseTrie.RegisterHandler("a/b/c/", (c, m) => { });
            reverseTrie.RegisterHandler("a/+/c/", (c, m) => { });

            reverseTrie.UnregisterHandler("a/");

            Assert.AreEqual(0, reverseTrie.Match("a/").Count);
            Assert.AreEqual(0, reverseTrie.Match("a/b/").Count);
            Assert.AreEqual(2, reverseTrie.Match("a/b/c/").Count);
        }

        [TestMethod]
        public void DeleteChild()
        {
            var reverseTrie = new ReverseTrie<MessageHandler>(-1);
            reverseTrie.RegisterHandler("a/", (c, m) => { });
            reverseTrie.RegisterHandler("a/b/", (c, m) => { });

            Assert.AreEqual(2, reverseTrie.Match("a/b/").Count);

            reverseTrie.UnregisterHandler("a/b/");

            Assert.AreEqual(1, reverseTrie.Match("a/b/").Count);
        }

        [TestMethod]
        public void DeleteInexistentChild()
        {
            var reverseTrie = new ReverseTrie<MessageHandler>(-1);
            reverseTrie.RegisterHandler("a/", (c, m) => { });
            reverseTrie.RegisterHandler("a/b/", (c, m) => { });

            reverseTrie.UnregisterHandler("c/");

            Assert.AreEqual(2, reverseTrie.Match("a/b/").Count);
        }

        [TestMethod]
        public void DeleteLonelyRoot()
        {
            var reverseTrie = new ReverseTrie<MessageHandler>(-1);
            reverseTrie.RegisterHandler("a/", (c, m) => { });

            reverseTrie.UnregisterHandler("a/");

            Assert.AreEqual(0, reverseTrie.Match("a/").Count);
        }
    }
}
