using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;


class Program
{
    static void Main(string[] args)
    {
        // Change the file to your directory
        string filePath = "D:/Навчання/Терм ІІІ/Прикладні алгоритми та структури даних І/huffman-coding-table/huffman-coding-table/sherlock.txt";
        //"/Users/antoninanovak/RiderProjects/huffman-coding-table/huffman-coding-table/sherlock.txt";
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
        WriteEncodedFile("binaryformat.bin", encodedText);
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
        //var minHeap = new MinHeap();
        
        
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
            //minHeap.Insert();
        }

        while (priorityQueue.Count > 1)
        {
            Node rightChild = priorityQueue.Dequeue(); 
            //Node rightChild = minHeap.Delete(minHeap.heap, minHeap.size); 
            Node leftChild = priorityQueue.Dequeue();
            
            
            var currentFrequency = rightChild.Frequency + leftChild.Frequency;
            Node nextNode = new Node(rightChild, leftChild, currentFrequency);
            
            priorityQueue.Enqueue(nextNode, nextNode.Frequency);

        }

        return priorityQueue.Dequeue();
    }
    
    public class MinHeap
    {
        public int size;
        public int[] heap;
        
        public int Parent(int i)
        {
            return (i - 1) / 2;
        }
        
        public static int RightChild(int i)
        {
            return (2 * i + 2);
        }
        
        public static int LeftChild(int i)
        {
            return (2 * i + 1);
        }

        public int Insert(int[] heap, int size, int Key)
        {
            size++;
            heap[size - 1] = Key;
            Heapify(heap, size, size - 1);
            return size;
        }

        public int Delete(int[] heap, int size)
        {
            int lastElement = heap[size - 1];
            heap[0] = lastElement;
            size = size - 1;
            Heapify(heap, size, 0);
            return size;
        }
        
        public void GetMin(int[] heap)
        {
            for (int n = size / 2 - 1; n >= 0; n--)
            {
                Swap(heap, 0, n);
                Heapify(heap, size, n);
            }
        }
        
        public static void Swap(int[] heap, int i, int j)
        {
            int temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }
        
        static void Heapify(int[] heap, int size, int m)
        {
            int smallest = m;
            int leftchild = LeftChild(m);
            int rightchild = RightChild(m);
            
            // check if the left child is smaller than the current smallest element.
            if (leftchild < m && heap[leftchild] < heap[smallest])
                smallest = leftchild;

            // check if the right child is smaller than the current smallest element.
            if (rightchild < m && heap[rightchild] < heap[smallest])
                smallest = rightchild;

            // if the largest element is not the current element, swap them and call Heapify recursively.
            if (smallest != m)
            {
                Swap(heap, m, smallest);
                Heapify(heap, size, smallest);
            }
        }

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
    
    static void WriteEncodedFile(string outputPath, string encodedText)
    {
        using (var stream = new FileStream(outputPath, FileMode.Create))
        {
            int buffer = 0; // Буфер для зберігання бітів перед записом у файл
            int bitsInBuffer = 0; // Кількість бітів, які вже зберігаються в буфері
            
            foreach (char bit in encodedText)
            {
                // Зсуваємо біти в буфері на 1 позицію вліво і додаємо новий біт
                buffer = (buffer << 1) | (bit == '1' ? 1 : 0);
                bitsInBuffer++;

                // Якщо у буфері вже 8 бітів, записуємо байт у файл і скидаємо буфер та лічильник
                if (bitsInBuffer == 8)
                {
                    stream.WriteByte((byte)buffer);
                    buffer = 0;
                    bitsInBuffer = 0;
                }
            }

            // Якщо у буфері залишилися біти, додаємо їх до файлу, доповнюючи недостаючі біти нулями
            if (bitsInBuffer > 0)
            {
                buffer <<= 8 - bitsInBuffer;
                stream.WriteByte((byte)buffer);
            }
        }
    }

}