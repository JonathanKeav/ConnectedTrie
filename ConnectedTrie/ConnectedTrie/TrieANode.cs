using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectedTrie
{
    internal class TrieANode<T>
    {
        private readonly Dictionary<char, TrieANode<T>> childeren;
        private readonly Dictionary<Guid, TrieEntry<T>> entries;

        public bool IsRoot { get; set; }
        public char Character { get; set; }
        public int Depth { get; set; }
        public TrieANode<T> Parent { get; set; }

        public TrieANode(char value, int debth, TrieANode<T> parent)
        {
            Character = value;
            Depth = debth;
            Parent = parent;
            childeren = new Dictionary<char, TrieANode<T>>();
            entries = new Dictionary<Guid, TrieEntry<T>>();
        }

        public TrieANode<T> GetOrCreateChild(char character)
        {
            if (childeren.TryGetValue(character, out TrieANode<T> child))
            {
                return child;
            }
            else
            {
                child = new TrieANode<T>(character, this.Depth + 1, this);
                childeren.Add(character, child);
                return child;
            }
        }
        public void RemoveChildIfCan(char character)
        {
            if (childeren.TryGetValue(character, out TrieANode<T> child))
            {
                if (child.childeren.Count == 0 && child.entries.Count == 0) // if nothing else connected to child node
                    childeren.Remove(child.Character);
            }
        }

        public void RemoveEntry(Guid id)
        {
            entries.Remove(id);
        }

        public TrieANode<T> GetChild(char character)
        {
            if (childeren.TryGetValue(character, out TrieANode<T> child))
            {
                return child;
            }
            else
            {
                return null;
            }
        }

        public void AddEntry(TrieEntry<T> entry)
        {
            entry.AParentNode = this;
            entries.Add(entry.Id, entry);
        }

        public bool TryGetEntry(Guid id, out TrieEntry<T> entry)
        {
            if (entries.TryGetValue(id, out entry))
                return true;
            else
                return false;
        }

        public List<TrieEntry<T>> GetEntries()
        {
            return entries.Select(x => x.Value).ToList();
        }
    }
}
