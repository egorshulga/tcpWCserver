using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace tcpWCserver
{
	static class Server
	{
		private const int serverPort = 41414;
		private static readonly TcpListener serverListener = new TcpListener(IPAddress.Any, serverPort);


		static public void Start()
		{
			InitializeServer();

			try
			{
				Listening();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally 
			{
				serverListener.Stop();
			}
		}


		static private void InitializeServer()
		{
			serverListener.Start();
		}


		private const int sleepingTime = 1000;
		static private void Listening()
		{
			Console.WriteLine("Server: waiting for connections...");
			while (true)
			{
				if (!serverListener.Pending())
				{
					Thread.Sleep(sleepingTime);
				}
				else
				{
					ProcessPendingConnection();
					Thread.Sleep(sleepingTime / 10);
					Console.WriteLine("Server: waiting for connections...");
				}
			}
		}


		private static void ProcessPendingConnection()
		{
			try
			{
				TcpClient server = serverListener.AcceptTcpClient();
//				Console.WriteLine("Server: client {0} has connected to {1} just now.", (IPEndPoint)server.Client.RemoteEndPoint, (IPEndPoint)server.Client.LocalEndPoint);
				Console.WriteLine("Server: client {0} has connected just now.", (IPEndPoint)server.Client.RemoteEndPoint);

				Thread processing = new Thread(TextReceivingAndWordsCounting);

				processing.Start(server);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}


		private static void TextReceivingAndWordsCounting(object obj)
		{
			TcpClient server = (TcpClient) obj;
			try
			{
				string text = ReceiveText(server);

				int wordsCount = CountWordsByRegex(text);

				SendWordsCount(server, wordsCount);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}


		private static string ReceiveText(TcpClient server)
		{
			List<byte> receivingBuffer = new List<byte>();
			NetworkStream receivingStream = server.GetStream();

			while (receivingStream.DataAvailable)
			{
				receivingBuffer.Add((byte)receivingStream.ReadByte());
			}
			
			string text = Encoding.Unicode.GetString(receivingBuffer.ToArray());
			PrintClientsText(text);
			return text;
		}


		static private void PrintClientsText(string text)
		{
			Console.WriteLine("Server: received text:");
			Console.WriteLine(text);
		}


		static private int CountWords(string text)
		{
			int wordsCount = text.Split(' ').Length;
			Console.WriteLine("Server: {0} words in text.", wordsCount);
			return wordsCount;
		}
		

		static private int CountWordsByRegex(string text)
		{
			Regex word = new Regex(@"\w+");
			int wordsCount = word.Matches(text).Count;
			Console.WriteLine("Server: {0} words in text.", wordsCount);
			return wordsCount;
		}


		static private void SendWordsCount(TcpClient server, int wordsCount)
		{
			byte[] sendingbuffer = ConvertIntToByteArray(wordsCount);
			NetworkStream sendingStream = server.GetStream();

			sendingStream.Write(sendingbuffer, 0, sendingbuffer.Length);
		}


		static private byte[] ConvertIntToByteArray(int intValue)
		{
			byte[] array = BitConverter.GetBytes(intValue);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(array);
			}
			return array;
		}

	}
}
