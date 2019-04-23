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
    }
}
