using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CityExplorerServer
{
    public class FileDataSerializer : IDataSerializer
    {
        private string filePath;

        public FileDataSerializer(string absoluteFilePath)
        {
            filePath = absoluteFilePath;
        }
        
        public void Save<T>(List<T> data) where T : IStringSerializable
        {
            List<string> content = new List<string>();
            foreach (T serializable in data)
                content.AddRange(serializable.Serialize());
            
            File.WriteAllLines(filePath, content, Encoding.UTF8);
            Console.WriteLine("Saved");
        }

        public List<T> Load<T>(ISerializableFabric<T> serializableFabric) where T : IStringSerializable
        {
            List<T> result = new List<T>();
            List<string> fileLines = File.ReadAllLines(filePath, Encoding.UTF8).ToList();

            while (fileLines.Count > 0)
            {
                T nextObject = serializableFabric.Create();
                int linesSize = nextObject.GetLinesSize();
                
                if (linesSize > fileLines.Count)
                    throw new IOException("Save file is corrupted!");
                
                nextObject.Deserialize(fileLines);
                result.Add(nextObject);
                fileLines = fileLines.Skip(linesSize).ToList();
            }

            return result;
        }
    }
}