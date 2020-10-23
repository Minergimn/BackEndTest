using System;
using System.IO;
using System.Threading.Tasks;
using BackEndTest.Extensions;
using BackEndTest.Implementations;

namespace BackEndTest
{
    class Program
    {
        private const int ListMaxLength = 10000;
        
        private static readonly Random Random = new Random();
        
        static async Task Main(string[] args)
        {
            IListSerializer serializer = new ListNodeJsonSerializer();
            
            var headNode = InitRandomTestNodes();
            
            var copy = await serializer.DeepCopy(headNode);

            var fileName = $"test{DateTime.Now:hh_mm_ss}.json";

            await using var writeFileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            await serializer.Serialize(copy, writeFileStream);

            await using var readFileStream = new FileStream(fileName, FileMode.Open);
            var newHeadNode = await serializer.Deserialize(readFileStream);

            if (newHeadNode.Random?.Data != headNode.Random?.Data)
            {
                Console.WriteLine("fail");
            }
            
            Console.ReadKey();
        }

        private static ListNode InitRandomTestNodes()
        {
            var headNode = new ListNode
            {
                Data = "head"
            };
            
            var listLength = Random.Next(0, ListMaxLength);
            var currentNode = headNode;

            for (var i = 1; i < listLength; i++)
            {
                var data = $"data_{i}";
                currentNode = currentNode.AddNode(data);
            }
            
            currentNode = headNode;
            while (currentNode != null)
            {
                currentNode.Random = GetRandomNode(currentNode, listLength);
                currentNode = currentNode.Next;
            }

            return headNode;
        }
        
        private static ListNode GetRandomNode(ListNode node, int max)
        {
            var headNode = node;
            while (headNode.Previous != null)
            {
                headNode = headNode.Previous;
            }
            
            var randomNumber = Random.Next(0, max);

            if (randomNumber % Random.Next(1, 10) == 0)
            {
                return null;
            }
            
            var i = 0;

            var currentNode = headNode.Next;

            if (currentNode == null)
            {
                return null;
            }
            
            while (currentNode.IsNotTail() && i++ <= randomNumber)
            {
                currentNode = currentNode.Next;
            }

            return currentNode;
        }
    }
    
}