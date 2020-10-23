using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BackEndTest.Extensions;

namespace BackEndTest.Implementations
{
    internal partial class ListNodeJsonSerializer : IListSerializer
    {
        public async Task Serialize(ListNode head, Stream stream)
        {
            var listLength = head.GetListLength();

            var (dictionary, list) = GetListNodeCollections(head, listLength);

            await using var sw = new StreamWriter(stream);

            await sw.WriteLineAsync($"{FileStart}{listLength}{ElementsStart}");
            foreach (var node in list)
            {
                await sw.WriteLineAsync(GetElementWithSeparator(node, dictionary));
            }
            await sw.WriteLineAsync(FileFinish);
        }

        public async Task<ListNode> Deserialize(Stream stream)
        {
            using var sr = new StreamReader(stream);

            var line = await sr.ReadLineAsync();
            if (line == null || !line.StartsWith(FileStart))
            {
                return null;
            }

            if (!int.TryParse(line.Split()[CountIndex].Trim(','), out var listLength))
            {
                return null;
            }
            
            return await GetDeserializedHeadListNode(listLength, sr);
        }
        
        public Task<ListNode> DeepCopy(ListNode head)
        {
            var listLength = head.GetListLength();

            var (dictionary, _) = GetListNodeCollections(head, listLength);

            var newHead = new ListNode
            {
                Data = head.Data
            };
            
            var newList = new List<(ListNode, int)>(listLength);
            var newDictionary = new Dictionary<int, ListNode>(listLength);

            var currentNode = head;
            var currentNewNode = newHead;
            var i = 0;
            while (currentNode != null)
            {
                var randomNodeNumber = GetRandomNodeNumber(currentNode, dictionary);
                
                newList.Add((currentNewNode, randomNodeNumber));
                newDictionary.Add(i++, currentNewNode);

                currentNode = currentNode.Next;
                if (currentNode != null)
                {
                    currentNewNode = currentNewNode.AddNode(currentNode.Data);
                }
            }

            RestoreRandomNodes(newList, newDictionary);

            return Task.FromResult(newHead);
        }

        private static (Dictionary<ListNode, int>, List<ListNode> list) GetListNodeCollections(ListNode head, int listLength)
        {
            var dictionary = new Dictionary<ListNode, int>(listLength);
            var list = new List<ListNode>(listLength);

            var currentNode = head;
            var i = 0;
            while (currentNode != null)
            {
                dictionary.Add(currentNode, i++);
                list.Add(currentNode);
                currentNode = currentNode.Next;
            }

            return (dictionary, list);
        }
        
        private static string GetElementWithSeparator(ListNode node, Dictionary<ListNode, int> dictionary) => 
            $"{ElementStart}{node.Data}{ElementMiddle}{GetRandomNodeNumber(node, dictionary)}{ElementFinish}{(node.IsNotTail() ? ElementSeparator : string.Empty)}";

        private static int GetRandomNodeNumber(ListNode currentNode, Dictionary<ListNode, int> dictionary) =>
            currentNode.Random == null
                ? NullRandomNodeNumber
                : dictionary[currentNode.Random];

        private static async Task<ListNode> GetDeserializedHeadListNode(int listLength, StreamReader sr)
        {
            var headNode = new ListNode();

            var dictionary = new Dictionary<int, ListNode>(listLength);
            var list = new List<(ListNode, int)>(listLength);
            var i = 0;

            var currentNode = headNode;
            var line = await sr.ReadLineAsync();
            while (line != null && line != FileFinish)
            {
                currentNode.Data = GetNodeData(line);

                dictionary.Add(i++, currentNode);
                list.Add((currentNode, GetRandomNodeNumber(line)));

                currentNode = currentNode.AddNode();
                
                line = await sr.ReadLineAsync();
            }

            RestoreRandomNodes(list, dictionary);

            return headNode;
        }

        private static string GetNodeData(string line)
        {
            return line.Split()[DataIndex].TrimEnd(',').Trim('"');
        }

        private static int GetRandomNodeNumber(string line)
        {
            var strNumber = line.Split()[RandomNodeNumberIndex].TrimEnd(',').TrimEnd('}');
            
            if (!int.TryParse(strNumber, out var randomNodeNumber))
            {
                randomNodeNumber = NullRandomNodeNumber;
            }

            return randomNodeNumber;
        }

        private static void RestoreRandomNodes(IEnumerable<(ListNode, int)> list, IReadOnlyDictionary<int, ListNode> dictionary)
        {
            foreach (var (listNode, randomNodeNumber) in list)
            {
                if (dictionary.TryGetValue(randomNodeNumber, out var randomNode))
                {
                    listNode.Random = randomNode;
                }
            }
        }
    }
}