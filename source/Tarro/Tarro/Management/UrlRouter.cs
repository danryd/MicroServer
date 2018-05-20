using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarro.Management
{
    class UrlRouter
    {
        private RouteTrie trie = new RouteTrie();
        public Route Match(string actual)
        {
            var segments = actual.Split('/');
            var route = FindRoute(segments);
            if (route == null)
                return new Route(false, null);
            return new Route(true, null);
        }

        private object FindRoute(string[] segments)
        {
            throw new NotImplementedException();
        }

        public void Register(string path)
        {

        }


    }

    public class RouteTrie
    {
        private Node root;

        public void Add(string route, string value)
        {
            if (root == null)
                root = new Node();
            var node = root;
           
            var segements = route.Split('/');
            foreach (var segment in segements)
            {
                Node found = null;
                foreach (var child in node.Children)
                {
                    if (child.Key == segment)
                        found = child;
                }
                if (found != null)
                {
                    found = new Node {Key = segment};
                    node.Children.Add(found);
                }
                node = found;

            }

        }

        public bool Contains(string route)
        {
            return false;
        }
        class Node
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public List<Node> Children { get; set; }
        }
    }
    public class Route
    {
        public bool Found { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }

        public Route(bool found, Dictionary<string, string> parameters)
        {
            Parameters = parameters;
            Found = found;
        }
    }
}
