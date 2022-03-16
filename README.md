# ConnectedTrie
A data structure for efficient matching of two strings to the longest pair of prefixes

This data structure is based on the trie or prefix tree. It was created to facilitate the matching of a pair of strings to entities that are lookup-able by matching to the longest pair of prefixes.

![image](https://github.com/JonathanKeav/ConnectedTrie/blob/master/Connected%20Trie.drawio.png?raw=true)

The leaf nodes of the BNumber trie and the the ANumber trie are connected by sharing the same rule, i.e. the leaf nodes are linked by the rule. Therefore when checking if a particular CDR ANumber and Bumber matchs to a rule we traverse the BNumber trie to find the rule with the longest BNumber and then “climb” backup the ANumber trie to reach the head node. If we are successful then the rule is matched and we have the longest B number and then the longest A number in the set of rules. If not we traverse back up the BNumber trie, and at each rule we find, we again try to “climb” backup the ANumber trie to reach the head node. If we never get to the ANumber trie head node n matches are found. 

Below is an image that will demonstrate the lookup process using letters instead of numbers. Given the rules shown in the image, and given an ANumebr of “cantxyz” and a BNumber of “dontxyz” we would traverse the BNumber trie down four nodes, “d”, “o”, ”n” and “t”. We cannot descend any further so we move to the connected leaf node in the ANumber trie and try to “climb” to the root node. The first check will fail as the node contains the character “d” and the 5th character in the ANumber “cantxyz” is “x”. We are comparing the 5th character as the “d” node is 5 deep in the ANumebr trie and therefore should be matched to the 5th character. 
