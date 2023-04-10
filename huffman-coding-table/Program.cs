using System;
using System.IO;
using System.Collections.Generic;


class Program
{
    static void Main(string[] args)
    {
        // Change the file to your directory
        string filePath = "/Users/antoninanovak/RiderProjects/huffman-coding-table/huffman-coding-table/sherlock.txt";
        Dictionary<char, int> characterFrequency = CountCharacterFrequency(filePath);
        
        /*foreach (KeyValuePair<char, int> entry in characterFrequency)
        {
            Console.WriteLine($"Character: {entry.Key}, Count: {entry.Value}");
        }*/
        
        /*// sort (from min to max) characters by their frequency 
        Console.WriteLine($"\nSORTED");
        
        foreach (KeyValuePair<char, int> entry in characterFrequency.OrderBy(key => key.Value))
        {
            Console.WriteLine($"Character: {entry.Key}, Count: {entry.Value}");
        }*/

        Node root = HuffmanTree(characterFrequency);
        
        var encodingTable = new Dictionary<char, string>();
        GenerateEncodingTable(root, "", encodingTable);
        
        Console.WriteLine("\nHuffman Encoding Table:");
        Console.WriteLine("---------------------------");
        Console.WriteLine("| {0,-5} | {1,-15} |", "Char", "Encoding");
        Console.WriteLine("---------------------------");

        foreach (var entry in encodingTable)
        {
            Console.WriteLine("| {0,-5} | {1,-15} |", entry.Key, entry.Value);
        }

        Console.WriteLine("---------------------------");
        
        string encodedText = EncodeText(filePath, encodingTable);
        WriteTextEncodedFile("textformat.txt", encodedText);

    }

    public class Node
    {
        public char? Character;
        public int Frequency;
        public Node rightChild;
        public Node leftChild; 
        
        
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
    
    static Node HuffmanTree(Dictionary<char, int> characterFrequency)
    {
        var priorityQueue = new PriorityQueue<Node, int>();
        
        /*for (int i = 0; i < 1000; i++)
        {
            if (characterFrequency[i] > 1)
            {
                priorityQueue.Enqueue(characterFrequency[i], new Node(characterFrequency[i]));
            }
        }*/
        
        foreach (var entry in characterFrequency)
        {
            priorityQueue.Enqueue(new Node(entry.Key, entry.Value), entry.Value);
        }

        while (priorityQueue.Count > 1)
        {
            Node rightChild = priorityQueue.Dequeue(); 
            Node leftChild = priorityQueue.Dequeue();
            
            var currentFrequency = rightChild.Frequency + leftChild.Frequency;
            Node nextNode = new Node(rightChild, leftChild, currentFrequency);
            
            priorityQueue.Enqueue(nextNode, nextNode.Frequency);

        }

        return priorityQueue.Dequeue();
    }
    
    static void GenerateEncodingTable(Node node, string path, Dictionary<char, string> encodingTable)
    {
        if (node == null)
        {
            return;
        }

        if (node.Character.HasValue)
        {
            encodingTable[node.Character.Value] = path;
        }
        else
        {
            GenerateEncodingTable(node.leftChild, path + "0", encodingTable);
            GenerateEncodingTable(node.rightChild, path + "1", encodingTable);
        }
    }
    
    static string EncodeText(string filePath, Dictionary<char, string> encodingTable)
    {
        string fileContent = File.ReadAllText(filePath);
        string encodedText = string.Concat(fileContent.Select(c => encodingTable[c]));
        return encodedText;
    }

    static void WriteTextEncodedFile(string outputPath, string encodedText)
    {
        File.WriteAllText(outputPath, encodedText);
    }
}