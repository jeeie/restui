using System;
using System.Net;

namespace Photonware.RestUI.CommonApi
{
	public interface IHttpUtils
	{
		string UserAgent {get;set;}
		bool EnableUserAgent {get;set;}
		bool EnableProxy {get;set;}
		bool UseDefaultProxy {get;set;}

		void SetProxy(Uri proxyUri, ICredentials credential);
		void RemoveProxy();

		HttpWebRequest NewHttpWebRequest(Uri uri);
		string GetHtml(string uri, string encoder);
		string GetHtml(Uri uri, System.Text.Encoding encoding);
		string GetHtml(string uri, string encoder, int cacheLevel);
		string GetHtml(Uri uri, string referer, System.Text.Encoding encoding);
		string GetHtml(Uri uri, string referer, System.Text.Encoding encoding, int cacheLevel);
		string GetHtml(Uri uri, string referer, System.Text.Encoding encoding, CookieContainer cookies, int cacheLevel);

		byte[] GetData(Uri uri);
		byte[] GetData(Uri uri, string referer);
		byte[] GetData(Uri uri, string referer, CookieContainer cookies, int cacheLevel);

		string PostData(Uri uri, string referer, byte[] postData, System.Text.Encoding responseEncoding, CookieContainer cookies, int cacheLevel);
		byte[] PostData(Uri uri, string referer, byte[] postData, CookieContainer cookies, int cacheLevel);


	}
}

