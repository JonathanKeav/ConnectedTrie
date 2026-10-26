using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectedTrie
{
    internal class TrieA<T>
    {
        private readonly TrieANode<T> root;

        public TrieA()
        {
            root = new TrieANode<T>('^', 0, null) { IsRoot = true };
        }
        public TrieANode<T> Add(string str, TrieEntry<T> Value)
        {
            TrieANode<T> currentNode = root; ;
            foreach (var c in str)
            {
                currentNode = currentNode.GetOrCreateChild(c);
            }
            currentNode.AddEntry(Value);
            return currentNode;

        }

        public bool Remove(string str, Guid id)
        {
            var leafNode = this.Get(str);
            if (leafNode == null)
                return false;

            if (leafNode.TryGetEntry(id, out TrieEntry<T> entry))
            {
                // Remove the value
                leafNode.RemoveEntry(id);
                TrieANode<T> current = leafNode;
                while (!current.IsRoot)
                {
                    var parent = current.Parent;
                    parent.RemoveChildIfCan(current.Character);
                    current = parent;
                }
                return true;
            }
            else
                return false;
        }

        public bool TryGetEntry(string str, Guid id, out TrieEntry<T> entry)
        {
            var leafNode = this.Get(str);
            if (leafNode == null)
            {
                entry = null;
                return false;
            }

            return leafNode.TryGetEntry(id, out entry);
        }

        public TrieANode<T> Get(string str)
        {
            TrieANode<T> currentNode = root; ;
            foreach (var c in str)
            {
                currentNode = currentNode.GetChild(c);

                if (currentNode == null)
                    return null;

            }
            return currentNode;
        }

        public TrieANode<T> Search(string str)
        {
            TrieANode<T> currentNode = root;
            foreach (var c in str)
            {
                var nextNode = currentNode.GetChild(c);

                if (nextNode == null)
                    return currentNode;
                else
                    currentNode = nextNode;
            }
            return currentNode;
        }
    }
}
