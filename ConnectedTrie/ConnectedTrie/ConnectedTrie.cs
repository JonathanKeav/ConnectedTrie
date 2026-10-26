namespace ConnectedTrie
{
    public class ConnectedTrie<T>
    {
        TrieA<T> aTrie;
        TrieB<T> bTrie; // int is debth of linked node so we can choose deepest first
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public ConnectedTrie()
        {
            aTrie = new TrieA<T>();
            bTrie = new TrieB<T>();
        }
        public Guid Add(string APrefix, string BPrefix, T value)
        {
            Guid returnValue;
            
            if (string.IsNullOrEmpty(APrefix))
                throw new ArgumentException("Cannot load empty ANumber into trie", nameof(APrefix));

            cacheLock.EnterWriteLock();
            try
            {
                var trieEntry = new TrieEntry<T>(value);
                returnValue = trieEntry.Id;

                aTrie.Add(APrefix, trieEntry);

                if (!string.IsNullOrEmpty(BPrefix))
                    bTrie.Add(BPrefix, trieEntry);
            }
            finally
            {
                cacheLock.ExitWriteLock();   
            }
            return returnValue;
        }

        public bool Remove(string APrefix, string BPrefix, Guid id)
        {
            if (string.IsNullOrEmpty(APrefix))
                return false;

            cacheLock.EnterWriteLock();
            try
            {
                // Check the entry is in both tries before removing it from either, so a wrong
                // prefix can't leave it half removed
                if (!aTrie.TryGetEntry(APrefix, id, out TrieEntry<T> entry))
                    return false;

                if (string.IsNullOrEmpty(BPrefix))
                {
                    if (entry.BParentNode != null)
                        return false;
                }
                else if (!bTrie.TryGetEntry(BPrefix, id, out _))
                    return false;

                aTrie.Remove(APrefix, id);
                if (!string.IsNullOrEmpty(BPrefix))
                    bTrie.Remove(BPrefix, id);

                return true;
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public List<T> Lookup(string APrefix, string BPrefix)
        {
            // Need a non-null APrefix to search. 
            if (string.IsNullOrEmpty(APrefix))
                throw new ArgumentException("Need a non-null APrefix to search", nameof(APrefix));

            var returnList = new List<T>();

            cacheLock.EnterReadLock();
            try
            {
                var currentBNode = bTrie.Search(BPrefix ?? string.Empty);
                while (!currentBNode.IsRoot)
                {
                    var entries = currentBNode.GetEntry().OrderByDescending(x => x.AParentNode.Depth);

                    foreach (var entry in entries)
                    {
                        var currentANode = entry.AParentNode;
                        bool keepClimbing = true;
                        while (keepClimbing)
                        {
                            if (APrefix.Length >= currentANode.Depth)
                            {
                                string aString = GetFirstXCharactersInReverse(APrefix, currentANode.Depth);
                                foreach (var c in aString)
                                {
                                    if (currentANode.Character == c)
                                    {
                                        currentANode = currentANode.Parent;
                                    }
                                    else
                                    {
                                        keepClimbing = false;
                                        break;
                                    }

                                    if (currentANode.IsRoot)
                                    {
                                        returnList.Add(entry.Value); // Got to climb to top of A tree so value is matching prefix
                                        keepClimbing = false;
                                        break;
                                    }
                                }
                            }
                            else
                                keepClimbing = false;
                        }
                    }

                    // Climb up B tree
                    currentBNode = currentBNode.Parent;
                }

                // If nothing returned above try searching aTrie
                if (returnList.Count == 0)
                {
                    var currentNode = aTrie.Search(APrefix);

                    while (!currentNode.IsRoot)
                    {
                        var entries = currentNode.GetEntries().Where(x => x.BParentNode is null); 
                        foreach (var entry in entries)
                        {
                            if (entry != null)
                            {
                                returnList.Add(entry.Value);
                            }

                        }
                        // Climb up A tree
                        currentNode = currentNode.Parent;
                    }
                }
                
                return returnList;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        private static string GetFirstXCharactersInReverse(string ANumber, int x)
        {
            char[] charArray = ANumber.Substring(0, x).ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}