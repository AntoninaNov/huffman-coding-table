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
        
        foreach (KeyValuePair<char, int> entry in characterFrequency)
        {
            Console.WriteLine($"Character: {entry.Key}, Count: {entry.Value}");
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
}