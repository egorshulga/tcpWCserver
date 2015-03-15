using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tcpWCserver
{
	static class Server
	{
		private const int serverPort = 41414;
		private static readonly TcpListener serverListener = new TcpListener(IPAddress.Any, serverPort);
		private static TcpClient server;


		static public void Start()
		{
			InitializeServer();

			Listening();

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
					server = serverListener.AcceptTcpClient();
				}
			}
		}


		static private void 



	}
}
