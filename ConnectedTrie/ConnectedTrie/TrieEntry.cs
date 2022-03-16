using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectedTrie
{
    internal class TrieEntry<T>
    {
        public TrieEntry(T value)
        {
            Id = Guid.NewGuid();
            Value = value;
        }
        public T Value { get; set; }
        //public int Id { get; set; }
        public TrieANode<T> AParentNode { get; set; }
        public TrieBNode<T> BParentNode { get; set; }
        public Guid Id { get; set; }
    }
}
