using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectedTrie
{
    internal class TrieB<T>
    {
        private readonly TrieBNode<T> root;

        public TrieB()
        {
            root = new TrieBNode<T>('^', 0, null) { IsRoot = true };
        }
        public TrieBNode<T> Add(string str, TrieEntry<T> Value)
        {
            // loop through interleaved string and add value to final node
            TrieBNode<T> currentNode = root; ;
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
                TrieBNode<T> current = leafNode;
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



        public TrieBNode<T> Get(string str)
        {
            TrieBNode<T> currentNode = root; ;
            foreach (var c in str)
            {
                currentNode = currentNode.GetChild(c);

                if (currentNode == null)
                    return null;

            }
            return currentNode;
        }

        public TrieBNode<T> Search(string str)
        {
            TrieBNode<T> currentNode = root;
            TrieBNode<T> nextNode = null;
            foreach (var c in str)
            {
                nextNode = currentNode.GetChild(c);

                if (nextNode == null)
                    return currentNode;
                else
                    currentNode = nextNode;

            }
            return nextNode;
        }
    }
}
