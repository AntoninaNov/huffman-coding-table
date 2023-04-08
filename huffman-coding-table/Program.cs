using System;
using System.IO;
using System.Collections.Generic;


class Program
{
    static void Main(string[] args)
    {
        // Change the file to your directory
        string filePath = "D:/Навчання/Терм ІІІ/Прикладні алгоритми та структури даних І/huffman-coding-table/huffman-coding-table/sherlock.txt";
        Dictionary<char, int> characterFrequency = CountCharacterFrequency(filePath);
        
        foreach (KeyValuePair<char, int> entry in characterFrequency)
        {
            Console.WriteLine($"Character: {entry.Key}, Count: {entry.Value}");
        }
        
        // sort (from min to max) characters by their frequency 
        Console.WriteLine($"\nSORTED");
        
        foreach (KeyValuePair<char, int> entry in characterFrequency.OrderBy(key => key.Value))
        {
            Console.WriteLine($"Character: {entry.Key}, Count: {entry.Value}");
        }

        Node root = HuffmanTree(characterFrequency);
        
    }

    public class Node
    {
        private char? Character;
        private int Frequency;
        private Node rightChild;
        private Node leftChild; 
        
        
        public Node(char? Character, int Frequency)
        {
            this.Character = Character;
            this.Frequency = Frequency;
        }

        public Node(Node rightChild, Node leftChild, int Frequency)
        {
            this.rightChild = rightChild;
            this.leftChild = leftChild;
            this.Frequency = Frequency;
        }
    }

    static Dictionary<char, int> CountCharacterFrequency(string filePath)
    {
        Dictionary<char, int> characterFrequency = new Dictionary<char, int>();

        // Read the file
        string fileContent = File.ReadAllText(filePath);

        // Count character frequency
        foreach (char c in fileContent)
        {
            if (characterFrequency.ContainsKey(c))
            {
                characterFrequency[c]++;
            }
            else
            {
                characterFrequency[c] = 1;
            }
        }

        return characterFrequency;
        
    }
    
    Node HuffmanTree(int[] characterFrequency)
    {
        var PriorityQueue = new PriorityQueue<Node>();
        for (int i = 0; i < 1000; i++)
        {
            if (characterFrequency[i] > 1)
            {
                PriorityQueue.Enqueue(characterFrequency[i], new Node(characterFrequency[i]));
            }
        }

        while (PriorityQueue.Count > 1)
        {
            Node rightChild = PriorityQueue.Dequeue(); 
            Node leftChild = PriorityQueue.Dequeue();
            var currentFrequency = rightChild.Frequency + leftChild.Frequency;
            Node nextNode = new Node(rightChild, leftChild, currentFrequency);
            PriorityQueue.Enqueue(nextNode, currentFrequency);
            
        }

        return PriorityQueue.Dequeue(Node);

    }
    
   
    
}