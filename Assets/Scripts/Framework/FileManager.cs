using System;
using System.IO;
using UnityEngine;


namespace Framework
{
    public static class FileManager
    {
        public static bool WriteToFile(string fileName, byte[] fileContent)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);
            try
            {
                FileStream stream = File.OpenWrite(fullPath);
                stream.Write(fileContent, 0, fileContent.Length);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                return false;
            }
        }
        
        public static BinaryReader LoadFromFile(string fileName)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                Stream stream = File.OpenRead(fullPath);
                return new BinaryReader(stream);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read from {fullPath} with exception {e}");
                return null;
            }
        }

        public static void Copy(string fileNameIn, string fileNameOut)
        {
            string fullPathIn = Path.Combine(Application.persistentDataPath, fileNameIn);
            string fullPathOut = Path.Combine(Application.persistentDataPath, fileNameOut);
            try
            {
                File.Copy(fullPathIn, fullPathOut);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to copy from {fullPathIn} to {fullPathOut} with exception {e}");
            }
        }
    }
}