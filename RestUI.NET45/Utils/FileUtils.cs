using Photonware.RestUI.CommonApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestUI.Utils
{
    class FileUtils : IFileStore
    {
        public string PathSeparater
        {
            get { return System.IO.Path.DirectorySeparatorChar + ""; }
        }

        public string[] ReadFromFile(string path)
        {
            return this.ReadFromFile(path, "UTF-8");
        }

        public string[] ReadFromFile(string path, string encoding)
        {
            return System.IO.File.ReadAllLines(path, Encoding.GetEncoding(encoding));
        }

        public byte[] ReadDataFromFile(string path)
        {
            throw new NotImplementedException();
        }

        public void WriteToFile(string path, string content, bool overwrite)
        {
            this.WriteToFile(path, content, "UTF-8", true);
        }

        public void WriteToFile(string path, string content, string encoding, bool overwrite)
        {
            string tempFilename = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(tempFilename, content, Encoding.GetEncoding(encoding));
            if (System.IO.File.Exists("usercases.json"))
            {
                System.IO.File.Copy("usercases.json", "usercases.json.bak", true);
            }
            System.IO.File.Copy(tempFilename, path, true);
        }

        public void WriteToFile(string path, byte[] data, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string path)
        {
            throw new NotImplementedException();
        }

        public bool FolderExists(string path)
        {
            throw new NotImplementedException();
        }

        public void CreateFolder(string path)
        {
            throw new NotImplementedException();
        }

        public string GetHomeFolder()
        {
            throw new NotImplementedException();
        }

        public string GetFavoriteFolder()
        {
            throw new NotImplementedException();
        }

        public string GetDownloadFolder()
        {
            throw new NotImplementedException();
        }

        public string GetCacheFolder()
        {
            throw new NotImplementedException();
        }

        public string GetPluginFolder()
        {
            throw new NotImplementedException();
        }

        public string[] QueryFiles(string basePath, string pattern, bool searchSubFolder)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string path)
        {
            throw new NotImplementedException();
        }

        public void DeleteFolder(string path)
        {
            throw new NotImplementedException();
        }

        public void Rename(string from, string to, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public string FilterPathName(string pathname)
        {
            throw new NotImplementedException();
        }

        public string FilterFilename(string filename)
        {
            throw new NotImplementedException();
        }

        public string GetFilename(string path)
        {
            throw new NotImplementedException();
        }

        public string GetFolderName(string path)
        {
            throw new NotImplementedException();
        }

        public string UrlToFilename(string url)
        {
            throw new NotImplementedException();
        }
    }
}
