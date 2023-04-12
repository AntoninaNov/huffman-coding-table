using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // Change the file to your directory
        string filePath = "/Users/antoninanovak/RiderProjects/huffman-coding-table/huffman-coding-table/sherlock.txt";
        //"/Users/antoninanovak/RiderProjects/huffman-coding-table/huffman-coding-table/sherlock.txt";
        //"D:/Навчання/Терм ІІІ/Прикладні алгоритми та структури даних І/huffman-coding-table/huffman-coding-table/sherlock.txt";
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
        
        string decodedText = DecodeBinaryFile("binaryformat.bin", root);
        WriteDecodedFile("decodedBinary.txt", decodedText);

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
    
    public class MinHeap
    {
        public int size;
        public Node[] heap;

        public MinHeap(int capacity)
        {
            size = 0;
            heap = new Node[capacity];
        }

        public int ParentNode(int i)
        {
            return (i - 1) / 2;
        }

        public int RightChild(int i)
        {
            return (2 * i + 2);
        }

        public int LeftChild(int i)
        {
            return (2 * i + 1);
        }

        public void Insert(Node node)
        {
            size++;
            heap[size - 1] = node;
            int i = size - 1;
            
            // не юзаємо хіпіфай, а просто переміщуємо вгору
            while (i > 0 && heap[ParentNode(i)].Frequency > heap[i].Frequency)
            {
                Swap(i, ParentNode(i));
                i = ParentNode(i);
            }
        }

        public Node DeleteMin()
        {
            // мінімальний заміняємо останнім
            Node min = heap[0];
            heap[0] = heap[size - 1];
            size--;

            // а ось тут юзаємо хіпіфай бо переміщуємо вниз
            Heapify(0);
            return min;
        }

        private void Swap(int i, int j) // Rider запропонував зробити "Swap via deconstruction"
        {
            (heap[i], heap[j]) = (heap[j], heap[i]);
        }


        private void Heapify(int i)
        {
            int smallest;
            int leftchildOfNode = LeftChild(i);
            int rightchildOfNode = RightChild(i);
            
            // враховуємо частоту!
            if (leftchildOfNode < size && heap[leftchildOfNode].Frequency < heap[i].Frequency)
            {
                smallest = leftchildOfNode;
            }
            else
            {
                smallest = i;
            }

            if (rightchildOfNode < size && heap[rightchildOfNode].Frequency < heap[smallest].Frequency)
            {
                smallest = rightchildOfNode;
            }

            if (smallest != i)
            {
                Swap(i, smallest);
                Heapify(smallest);
            }
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
        // var priorityQueue = new PriorityQueue<Node, int>();
        var Heap = new MinHeap(characterFrequency.Count);

        foreach (var entry in characterFrequency)
        {
            // priorityQueue.Enqueue(new Node(entry.Key, entry.Value), entry.Value);
            Heap.Insert(new Node(entry.Key, entry.Value));
        }

        //while (priorityQueue.Count > 1)
        while (Heap.size > 1)
        {
            //Node rightChild = priorityQueue.Dequeue(); 
            //Node leftChild = priorityQueue.Dequeue();
            Node rightChild = Heap.DeleteMin();
            Node leftChild = Heap.DeleteMin();
            
            var currentFrequency = rightChild.Frequency + leftChild.Frequency;
            Node nextNode = new Node(rightChild, leftChild, currentFrequency);
            
            // priorityQueue.Enqueue(nextNode, nextNode.Frequency);
            Heap.Insert(nextNode);

        }

        // return priorityQueue.Dequeue();
        return Heap.DeleteMin();
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
                buffer <<= 1; // 1011 << 1 = 10110 додаємо 0 як молодший біт
                if (bit == '1')
                {
                    buffer |= 1; // замінюємо молодший біт на 1 (bitwise OR)
                }
                bitsInBuffer++;
                
                // Якщо в буфері вже 8 бітів = 1 байт, записуємо його
                if (bitsInBuffer == 8)
                {
                    stream.WriteByte((byte)buffer);
                    buffer = 0;
                    bitsInBuffer = 0;
                }
            }

            // Доповнюємо залишкові біти у буфері нулями
            {
                buffer <<= 8 - bitsInBuffer; // Зсув бітів вліво в буфері 
                stream.WriteByte((byte)buffer);
            }
        }
    }
    
    /*static string DecodeBinaryFile(string binaryFilePath, Node huffmanTreeRoot)
    {
        StringBuilder decodedText = new StringBuilder();
        using (var stream = new FileStream(binaryFilePath, FileMode.Open))
        {
            int currentByte;
            Node currentNode = huffmanTreeRoot;

            while ((currentByte = stream.ReadByte()) != -1)
            {
                for (int bitPos = 7; bitPos >= 0; bitPos--)
                {
                    int currentBit = (currentByte >> bitPos) & 1;
                    currentNode = currentBit == 0 ? currentNode.leftChild : currentNode.rightChild;

                    if (currentNode.Character.HasValue)
                    {
                        decodedText.Append(currentNode.Character.Value);
                        currentNode = huffmanTreeRoot;
                    }
                }
            }
        }

        return decodedText.ToString();
    }

    static void WriteDecodedFile(string outputPath, string decodedText)
    {
        File.WriteAllText(outputPath, decodedText);
    }
    */
}