using System;

namespace Photonware.RestUI.CommonApi
{
	public interface IFileStore
	{
		string PathSeparater {get;}
		string[] ReadFromFile (string path);
		string[] ReadFromFile (string path, string encoding);
		byte[] ReadDataFromFile(string path);
		void WriteToFile (string path, string content, bool overwrite);
		void WriteToFile (string path, string content, string encoding, bool overwrite);
		void WriteToFile (string path, byte[] data, bool overwrite);
		bool FileExists (string path);
		bool FolderExists (string path);
		void CreateFolder (string path);
		string GetHomeFolder();
		string GetFavoriteFolder ();
		string GetDownloadFolder ();
		string GetCacheFolder ();
        string GetPluginFolder();
		string[] QueryFiles(string basePath, string pattern, bool searchSubFolder);
		void DeleteFile(string path);
		void DeleteFolder (string path);
		void Rename(string from, string to, bool overwrite);
		string FilterPathName (string pathname);
		string FilterFilename (string filename);
		string GetFilename(string path);
        string GetFolderName(string path);
		string UrlToFilename(string url);
	}
}

