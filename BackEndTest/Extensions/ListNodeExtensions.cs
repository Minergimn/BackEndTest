using System;

namespace BackEndTest.Extensions
{
    internal static class ListNodeExtensions
    {
        private static Random _random = new Random();
        
        internal static bool IsNotTail(this ListNode node) => node.Next != null;
        
        internal static ListNode AddNode(this ListNode currentNode, string data = default)
        {
            var newNode = new ListNode
            {
                Previous = currentNode,
                Next = null,
                Data = data
            };

            currentNode.Next = newNode;
            
            return newNode;
        }
        
        internal static int GetListLength(this ListNode node)
        {
            var listLength = 0;
            
            var currentNode = node;
            while (currentNode != null)
            {
                listLength++;
                currentNode = currentNode.Next;
            }

            return listLength;
        }
    }
}