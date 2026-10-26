namespace ConnectedTrie.Tests
{
    public class ConnectedTrieTests
    {
        // Add

        [Fact]
        public void Add_ReturnsADifferentIdForEachEntry()
        {
            var trie = new ConnectedTrie<string>();

            var first = trie.Add("cant", "do", "first");
            var second = trie.Add("cant", "do", "second");

            Assert.NotEqual(Guid.Empty, first);
            Assert.NotEqual(first, second);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Add_WithoutAPrefix_Throws(string? aPrefix)
        {
            var trie = new ConnectedTrie<string>();

            Assert.Throws<ArgumentException>(() => trie.Add(aPrefix!, "do", "value"));
        }

        // Lookup: matching on both strings

        [Fact]
        public void Lookup_ReadmeExample_MatchesTheLongestPairOfPrefixes()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("cant", "do", "cant:do");
            trie.Add("could", "dont", "could:dont");

            var matches = trie.Lookup("cantxyz", "dontxyz");

            Assert.Equal(new[] { "cant:do" }, matches);
        }

        [Fact]
        public void Lookup_ExactStrings_Match()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "xyz", "value");

            Assert.Equal(new[] { "value" }, trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Lookup_OnEmptyTrie_ReturnsNothing()
        {
            var trie = new ConnectedTrie<string>();

            Assert.Empty(trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Lookup_WhenOnlyTheAStringMatches_ReturnsNothing()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "xyz", "value");

            Assert.Empty(trie.Lookup("abc", "qqq"));
        }

        [Fact]
        public void Lookup_WhenOnlyTheBStringMatches_ReturnsNothing()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "xyz", "value");

            Assert.Empty(trie.Lookup("qqq", "xyz"));
        }

        [Fact]
        public void Lookup_WhenTheAStringIsShorterThanTheEntrysPrefix_ReturnsNothing()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abcd", "xyz", "value");

            Assert.Empty(trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Lookup_WhenTheBStringIsShorterThanTheEntrysPrefix_ReturnsNothing()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "xyzw", "value");

            Assert.Empty(trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Lookup_WhenTheAStringDiffersPartWay_ReturnsNothing()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abcd", "xyz", "value");

            Assert.Empty(trie.Lookup("abXd", "xyz"));
        }

        [Fact]
        public void Lookup_ReturnsEveryMatch_LongestBPrefixFirst()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("a", "x", "short B");
            trie.Add("a", "xyz", "long B");
            trie.Add("a", "xy", "middle B");

            var matches = trie.Lookup("abc", "xyzw");

            Assert.Equal(new[] { "long B", "middle B", "short B" }, matches);
        }

        [Fact]
        public void Lookup_ForTheSameBPrefix_ReturnsLongestAPrefixFirst()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("a", "xy", "short A");
            trie.Add("abc", "xy", "long A");
            trie.Add("ab", "xy", "middle A");

            var matches = trie.Lookup("abcd", "xyz");

            Assert.Equal(new[] { "long A", "middle A", "short A" }, matches);
        }

        [Fact]
        public void Lookup_PrefersTheLongerBPrefixOverTheLongerAPrefix()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "x", "long A, short B");
            trie.Add("a", "xyz", "short A, long B");

            var matches = trie.Lookup("abc", "xyz");

            Assert.Equal(new[] { "short A, long B", "long A, short B" }, matches);
        }

        [Fact]
        public void Lookup_IgnoresEntriesWhoseAPrefixDoesNotMatch_AtTheSameBPrefix()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "xy", "match");
            trie.Add("abd", "xy", "no match");

            Assert.Equal(new[] { "match" }, trie.Lookup("abcz", "xyz"));
        }

        [Fact]
        public void Lookup_WorksForValueTypes()
        {
            var trie = new ConnectedTrie<int>();
            trie.Add("abc", "xyz", 42);

            Assert.Equal(new[] { 42 }, trie.Lookup("abc", "xyz"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Lookup_WithoutAPrefix_Throws(string? aPrefix)
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "xyz", "value");

            Assert.Throws<ArgumentException>(() => trie.Lookup(aPrefix!, "xyz"));
        }

        // Lookup: entries added without a B prefix

        [Fact]
        public void Lookup_WhenNoBPrefixMatches_FallsBackToEntriesWithoutABPrefix()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("ab", "", "A only");
            trie.Add("ab", "xyz", "with B");

            Assert.Equal(new[] { "A only" }, trie.Lookup("abc", "qqq"));
        }

        [Fact]
        public void Lookup_WhenABPrefixMatches_DoesNotReturnEntriesWithoutABPrefix()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "", "A only");
            trie.Add("a", "x", "with B");

            Assert.Equal(new[] { "with B" }, trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Lookup_WhenTheBPrefixMatchesButTheAPrefixDoesNot_FallsBackToEntriesWithoutABPrefix()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("ab", null!, "A only");
            trie.Add("zz", "xyz", "with B");

            Assert.Equal(new[] { "A only" }, trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Lookup_FallbackReturnsLongestAPrefixFirst()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("a", "", "short A");
            trie.Add("abc", "", "long A");
            trie.Add("ab", "", "middle A");

            var matches = trie.Lookup("abcd", "qqq");

            Assert.Equal(new[] { "long A", "middle A", "short A" }, matches);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Lookup_WithoutABString_ReturnsEntriesWithoutABPrefix(string? bPrefix)
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("ab", "", "A only");
            trie.Add("ab", "xyz", "with B");

            Assert.Equal(new[] { "A only" }, trie.Lookup("abc", bPrefix!));
        }

        [Fact]
        public void Lookup_WhenTheAStringMatchesNothing_ReturnsNothingFromTheFallback()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("ab", "", "A only");

            Assert.Empty(trie.Lookup("zz", "xyz"));
        }

        // Remove

        [Fact]
        public void Remove_RemovesTheEntry()
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "xyz", "value");

            Assert.True(trie.Remove("abc", "xyz", id));
            Assert.Empty(trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Remove_LeavesEntriesThatSharePrefixes()
        {
            var trie = new ConnectedTrie<string>();
            var longer = trie.Add("abc", "xyz", "longer");
            trie.Add("ab", "xy", "shorter");
            trie.Add("abc", "xyz", "same strings");

            trie.Remove("abc", "xyz", longer);

            Assert.Equal(new[] { "same strings", "shorter" }, trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Remove_OfAShorterEntry_LeavesLongerEntriesBelowIt()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "xyz", "longer");
            var shorter = trie.Add("ab", "xy", "shorter");

            trie.Remove("ab", "xy", shorter);

            Assert.Equal(new[] { "longer" }, trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Remove_EntryWithoutABPrefix()
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "", "A only");

            Assert.True(trie.Remove("abc", "", id));
            Assert.Empty(trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Remove_WithAnUnknownId_ReturnsFalse()
        {
            var trie = new ConnectedTrie<string>();
            trie.Add("abc", "xyz", "value");

            Assert.False(trie.Remove("abc", "xyz", Guid.NewGuid()));
            Assert.Equal(new[] { "value" }, trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Remove_Twice_ReturnsFalseTheSecondTime()
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "xyz", "value");

            trie.Remove("abc", "xyz", id);

            Assert.False(trie.Remove("abc", "xyz", id));
        }

        [Fact]
        public void Remove_WithPrefixesNotInTheTrie_ReturnsFalse()
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "xyz", "value");

            Assert.False(trie.Remove("qqq", "qqq", id));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Remove_WithoutAPrefix_ReturnsFalse(string? aPrefix)
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "xyz", "value");

            Assert.False(trie.Remove(aPrefix!, "xyz", id));
            Assert.Equal(new[] { "value" }, trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Remove_WithTheWrongBPrefix_ReturnsFalseAndKeepsTheEntry()
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "xyz", "value");

            Assert.False(trie.Remove("abc", "xy", id));
            Assert.Equal(new[] { "value" }, trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Remove_WithTheWrongAPrefix_ReturnsFalseAndKeepsTheEntry()
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "xyz", "value");

            Assert.False(trie.Remove("ab", "xyz", id));
            Assert.Equal(new[] { "value" }, trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Remove_WithTheWrongBPrefix_ThenTheRightOne_RemovesTheEntry()
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "xyz", "value");

            trie.Remove("abc", "xy", id);

            Assert.True(trie.Remove("abc", "xyz", id));
            Assert.Empty(trie.Lookup("abc", "xyz"));
        }

        [Fact]
        public void Remove_WithoutTheBPrefixOfAnEntryThatHasOne_ReturnsFalseAndKeepsTheEntry()
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "xyz", "value");

            Assert.False(trie.Remove("abc", "", id));
            Assert.True(trie.Remove("abc", "xyz", id));
        }

        [Fact]
        public void Add_AfterRemove_MatchesAgain()
        {
            var trie = new ConnectedTrie<string>();
            var id = trie.Add("abc", "xyz", "old");
            trie.Remove("abc", "xyz", id);

            trie.Add("abc", "xyz", "new");

            Assert.Equal(new[] { "new" }, trie.Lookup("abc", "xyz"));
        }

        // Thread safety

        [Fact]
        public void ConcurrentAddsAndLookups_LoseNothing()
        {
            var trie = new ConnectedTrie<int>();
            const int count = 2000;

            Parallel.For(0, count, i =>
            {
                trie.Add($"a{i}", $"b{i}", i);
                trie.Lookup($"a{i}", $"b{i}");
            });

            for (var i = 0; i < count; i++)
                Assert.Contains(i, trie.Lookup($"a{i}", $"b{i}"));
        }

        [Fact]
        public void ConcurrentRemoves_RemoveEachEntryOnce()
        {
            var trie = new ConnectedTrie<int>();
            const int count = 2000;
            var ids = Enumerable.Range(0, count).Select(i => trie.Add($"a{i}", $"b{i}", i)).ToArray();

            var removed = 0;
            Parallel.For(0, count * 2, i =>
            {
                var n = i % count;
                if (trie.Remove($"a{n}", $"b{n}", ids[n]))
                    Interlocked.Increment(ref removed);
            });

            Assert.Equal(count, removed);
            Assert.Empty(trie.Lookup("a1", "b1"));
        }
    }
}
