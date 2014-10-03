using Photonware.RestUI.CommonApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photonware.RestUI.SL8.Utils
{
    public class FileUtils : IFileStore
    {
        public string PathSeparater
        {
            get { return System.IO.Path.DirectorySeparatorChar.ToString(); }
        }

        public string[] ReadFromFile(string path)
        {
            return ReadFromFile(path, "UTF-8");
        }

        public string[] ReadFromFile(string path, string encoding)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            if (store.FileExists(path))
            {
                using (var stream = new IsolatedStorageFileStream(path, FileMode.Open, store))
                {
                    using (var fileReader = new StreamReader(stream, Encoding.GetEncoding(encoding)))
                    {
                        var result = fileReader.ReadToEnd();
                        return result.Split('\n');
                    }
                }
            }
            return null;
        }

        public byte[] ReadDataFromFile(string path)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            
            if (store.FileExists(path))
            {
                using (var stream = new IsolatedStorageFileStream(path, FileMode.Open, store))
                {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    return data;
                }
            }
            return null;
        }

        public void WriteToFile(string path, string content, bool overwrite)
        {
            this.WriteToFile(path, content, "UTF-8", overwrite);
        }

        public void WriteToFile(string path, string content, string encoding, bool overwrite)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            //string _path = Path.Combine(GetHomeFolder(), path);

            if (!store.FileExists(path) || (overwrite))
            {
                using (var stream = new IsolatedStorageFileStream(path, FileMode.Create, store))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.GetEncoding(encoding)))
                    {
                        writer.Write(content);
                    }
                }
            }
        }

        public void WriteToFile(string path, byte[] data, bool overwrite)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            string _path = Path.Combine(GetHomeFolder(), path);

            if (!store.FileExists(_path) || (overwrite))
            {
                using (var stream = new IsolatedStorageFileStream(_path, FileMode.Create, store))
                {
                    stream.Write(data, 0, data.Length);
                }
            }
        }

        public bool FileExists(string path)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            return store.FileExists(path);
        }

        public bool FolderExists(string path)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            return store.DirectoryExists(path);
        }

        public void CreateFolder(string path)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            store.CreateDirectory(path);
        }

        public string GetHomeFolder()
        {
            return "";
        }

        public string GetFavoriteFolder()
        {
            return Path.Combine(GetHomeFolder(), "Favorites");
        }

        public string GetDownloadFolder()
        {
            return Path.Combine(GetHomeFolder(), "Downloads");
        }

        public string GetCacheFolder()
        {
            return Path.Combine(GetHomeFolder(), "Cache");
        }

        public string GetPluginFolder()
        {
            return Path.Combine(GetHomeFolder(), "Plugins");
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
