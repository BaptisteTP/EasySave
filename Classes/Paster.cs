using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Easy_Save.Classes
{
    internal class Paster
    {
        public event EventHandler OnExecute;
        public event EventHandler OnDirectoryCopy;
        public event EventHandler OnFileCopy;

        public List<string> SearchAllPathFiles(string sourcePath, string basePath)
        {
            List<string> allFiles = new List<string>();
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine($"The directory '{sourcePath}' does not exist.");
                return null;
            }

            string[] files = Directory.GetFiles(sourcePath);
            string[] directories = Directory.GetDirectories(sourcePath);

            foreach (string file in files)
            {
                string relativePath = Path.GetRelativePath(basePath, file);
                allFiles.Add(relativePath);
            }

            foreach (string directory in directories)
            {
                SearchAllPathFiles(directory, basePath); // Recursively search in subdirectories
            }
            return allFiles;
        }
    }
}
