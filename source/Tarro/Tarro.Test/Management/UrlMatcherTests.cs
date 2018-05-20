
using Shouldly;
using Tarro.Management;

namespace Tarro.Test.Management
{
    public class UrlMatcherTests
    {
        //public void MatchesRoot()
        //{
        //    var url = "http://example.com";
        //    var matcher = new UrlRouter();
        //    matcher.Register("/");
        //    var route = matcher.Match(url);
        //    route.Found.ShouldBe(true);
        //}
        //public void MatchesOneDeep()
        //{
        //    var url = "http://example.com/one";
        //    var matcher = new UrlRouter();
        //    matcher.Register("/");
        //    matcher.Register("/one");
        //    var route = matcher.Match(url);
        //    route.Found.ShouldBe(true);
        //}
        //public void MatchesRootWithParam()
        //{
        //    var root = "http://example.com/turbo";
        //    var matcher = new UrlRouter();
        //    matcher.Register("/:aprop");
        //    var match = matcher.Match(root);
        //    match.Found.ShouldBe(true);
        //    match.Parameters.Count.ShouldBe(1);
        //    match.Parameters["aprop"].ShouldBe("turbo");
        //}

    }

    public class TrieTests
    {
        RouteTrie trie = new RouteTrie();

        public void RootIsFalseIfEmpty()
        {
            trie.Contains("").ShouldBe(false);

        }

        public void RootIsTrueIfAdded()
        {
            trie.Add("","");

            trie.Contains("").ShouldBe(true);
        }
    }
}