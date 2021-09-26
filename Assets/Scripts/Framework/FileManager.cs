using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


namespace Framework
{
    public static class FileManager
    {
        private static readonly IFormatter Formatter = new BinaryFormatter(); 
        
        public static bool WriteToFile(string fileName, object fileContent)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);
            try
            {
                FileStream stream = File.OpenWrite(fullPath);
                Formatter.Serialize(stream, fileContent);
                Debug.Log($"Serialize to {fullPath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                return false;
            }
        }
        
        public static T LoadFromFile<T>(string fileName) where T: class
        {
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                FileStream stream = File.OpenRead(fullPath);
                var obj = (T)Formatter.Deserialize(stream);
                Debug.Log($"Deserialize from {fullPath}");
                return obj;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read from {fullPath} with exception {e}");
                return null;
            }
        }
    }
}