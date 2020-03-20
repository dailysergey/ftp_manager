using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ftp_manager
{
	class Program
	{
		string host = "ftp://localhost";
		string UserId = "admin";
		string Password = "password";
		static void Main(string[] args)
		{

			Program c = new Program();
			string host = c.host;
			var dirs = c.GetDirectories(host);
			foreach (var dir in dirs)
			{
				var files = c.GetFilesFromDirectory($"{host}/{dir}");
				foreach (var file in files)
				{
					var detinationFile = $"{host}/{dir}/{file}";
					string resultFilePath = Path.Combine(Directory.GetCurrentDirectory(), file);
					var sizeDestinationFile = c.GetFileSize(detinationFile);
					Console.WriteLine($"File {file} has size: {sizeDestinationFile}");
					long downloadedFile = 0;
					while (downloadedFile != sizeDestinationFile)
					{
						c.DownloadFile(detinationFile, resultFilePath);
						downloadedFile = new FileInfo(resultFilePath).Length;
					}
				}
			}
		}

		/// <summary>
		/// Get files in parentFolder
		/// </summary>
		/// <param name="ParentFolder">Related path ro folder</param>
		/// <returns></returns>
		private List<string> GetDirectories(string parentFolder)
		{
			try
			{
				FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(parentFolder);
				ftpRequest.Credentials = new NetworkCredential(UserId, Password);
				ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
				FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
				StreamReader streamReader = new StreamReader(response.GetResponseStream());
				List<string> directories = new List<string>();
				string line = streamReader.ReadLine();
				while (!string.IsNullOrEmpty(line))
				{
					var lineArr = line.Split('/');
					line = lineArr[lineArr.Length - 1];
					directories.Add(line);
					line = streamReader.ReadLine();
				}
				streamReader.Close();
				return directories;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private long GetFileSize(string destinationFile)
		{
			try
			{
				WebRequest sizeRequest = WebRequest.Create(destinationFile);
				sizeRequest.Credentials = new NetworkCredential(UserId, Password);
				sizeRequest.Method = WebRequestMethods.Ftp.GetFileSize;
				return (long)sizeRequest.GetResponse().ContentLength;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private List<string> GetFilesFromDirectory(string parentFolder)
		{
			try
			{
				FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(parentFolder);
				ftpRequest.Credentials = new NetworkCredential(UserId, Password);
				ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
				FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
				StreamReader streamReader = new StreamReader(response.GetResponseStream());
				List<string> directories = new List<string>();
				string line = streamReader.ReadLine();
				while (!string.IsNullOrEmpty(line))
				{
					var lineArr = line.Split('/');
					line = lineArr[lineArr.Length - 1];
					directories.Add(line);
					line = streamReader.ReadLine();
				}
				streamReader.Close();
				return directories;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Download file 
		/// </summary>
		/// <param name="destinationFile">path to destination file</param>
		/// <param name="resultFile">path of storage for downloaded file</param>
		private void DownloadFile(string destinationFile, string resultFile)
		{
			try
			{
				FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(destinationFile);
				ftpRequest.Credentials = new NetworkCredential(UserId, Password);
				ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
				using (Stream ftpStream = ftpRequest.GetResponse().GetResponseStream())
				using (Stream fileStream = File.Create(resultFile))
				{
					ftpStream.CopyTo(fileStream);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private void DownloadFileWithProgress(string destinationFile, string resultFile)
		{
			try
			{
				FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(destinationFile);
				ftpRequest.Credentials = new NetworkCredential(UserId, Password);
				ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
				using (Stream ftpStream = ftpRequest.GetResponse().GetResponseStream())
				using (Stream fileStream = File.Create(resultFile))
				{
					byte[] buffer = new byte[10240];
					int read;
					while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
					{
						fileStream.Write(buffer, 0, read);
						Console.WriteLine("Downloaded {0} bytes", fileStream.Position);
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
