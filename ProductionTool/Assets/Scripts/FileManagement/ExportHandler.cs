using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace FileManagement
{
    public class ExportHandler
    {
        public void Export(string url, Texture2D[] textures, FileType fileType)
        {
            if(textures == null || textures.Length == 0) { return; }

            // Check if the textures array contains multiple textures
                // if yes -> Create new folder at provided url
                // if no -> export directly as specified file type
            if(textures.Length == 1) { ExportSingleTexture(url, textures[0], fileType); }
            else { ExportMultipleTextures(url, textures, fileType); }
        }

        private void ExportSingleTexture(string path, Texture2D texture, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.PNG:
                    ExportAsPNG(path, texture);
                    break;
                case FileType.JPG:
                    ExportAsJPG(path, texture);
                    break;
            }
        }

        private void ExportMultipleTextures(string path, Texture2D[] textures, FileType fileType)
        {
            // create new folder
            Directory.CreateDirectory(path);
            // enter new folder
            path = path + "/variant";

            switch(fileType)
            {
                case FileType.PNG:
                    ExportAsPNG(path, textures);
                    break;
                case FileType.JPG: 
                    ExportAsJPG(path, textures);
                    break;
            }
        }

        private void ExportAsPNG(string path, Texture2D[] textures)
        {
            for(int i = 0; i < textures.Length; i++)
            {
                string currentPath = path + i + ".png";
                byte[] bytes = ImageConversion.EncodeToPNG(textures[i]);
                File.WriteAllBytes(currentPath, bytes);
            }
        }
        private void ExportAsPNG(string path, Texture2D texture)
        {
            string currentPath = path + ".png";
            byte[] bytes = ImageConversion.EncodeToPNG(texture);
            File.WriteAllBytes(currentPath, bytes);
        }

        private void ExportAsJPG(string path, Texture2D[] textures)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                string currentPath = path + i + ".jpg";
                byte[] bytes = ImageConversion.EncodeToJPG(textures[i]);
                File.WriteAllBytes(currentPath, bytes);
            }
        }
        private void ExportAsJPG(string path, Texture2D texture)
        {
            string currentPath = path + ".jpg";
            byte[] bytes = ImageConversion.EncodeToJPG(texture);
            File.WriteAllBytes(currentPath, bytes);
        }
    }
}
