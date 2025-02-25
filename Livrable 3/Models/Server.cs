using c.Models.Server_Related;
using EasySave2._0.CustomEventArgs;
using EasySave2._0.Enums;
using EasySave2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasySave2._0.Models
{
	public class Server
	{
		private static Server? _instance;
		private List<Socket> _clientsSockets = [];
		private Socket? _serverSocket;
		private int _maxNumerOfConcurrentClients = 10;
		private const int _port = 8888;
		private SaveStore _saveStore;

		#region Singleton 

		private Server()
		{
			_saveStore = Creator.GetSaveStoreInstance();
			StartServer();
		}

		public static Server GetServerInstance()
		{
			return _instance ??= new Server();
		}

		#endregion

		private void StartServer()
		{
			_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _port);
			_serverSocket.Bind(localEndPoint);
			_serverSocket.Listen(_maxNumerOfConcurrentClients);

			Task.Run(() =>
			{
				while (true)
				{
					Socket _newClientSocket = _serverSocket.Accept();
					_clientsSockets.Add(_newClientSocket);
					ListenToClient(_newClientSocket);
				}
			});
		}

		private void SendSavesToClient(Socket _clientSocket)
		{
			List<Save> savesList = Creator.GetSaveStoreInstance().GetAllSaves();
			ServerPacket serverPacket;
			foreach (Save save in savesList)
			{
				serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Send_save, Payload = SerializeMessage(save) };
				SendMessageToClient(serverPacket, _clientSocket);
			}
		}

		private void ListenToClient(Socket _clientSocket)
		{
			Task.Run(() =>
			{
				while (true)
				{
					try
					{
						byte[] buffer = new byte[4];
						_clientSocket.Receive(buffer);

						int length = BitConverter.ToInt32(buffer, 0);
						buffer = new byte[length];
						_clientSocket.Receive(buffer);

						if(buffer.Length > 0)
						{
							HandleClientRequest(_clientSocket, buffer);
						}
					}
					catch(SocketException e)
					{
						_clientSocket.Close();
						_clientsSockets.Remove(_clientSocket);
						break;
					}

				}
			});
		}

		private void HandleClientRequest(Socket _clientSocket, byte[] data)
		{
			ClientPacket clientPacket = DeserializeMessage(data);
			Save saveReceived;
			Save concernedSave;
			ServerPacket serverPacket;

			switch (clientPacket.ClientRequest)
			{
				case ClientRequests.Disconnect:
					_clientsSockets.Remove(_clientSocket);
					break;

				case ClientRequests.Saves_request:
					SendSavesToClient(_clientSocket);
					break;
				case ClientRequests.Update_saves__request:
					SendSavesToClient(_clientSocket);
					break;

				case ClientRequests.Save_start:
					saveReceived = DeserializeSave(clientPacket.Payload);
					concernedSave = _saveStore.GetSave(saveReceived.Id);

					if (concernedSave.IsExecuting == false)
					{
						concernedSave.Execute();
					}
					else
					{
						serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_already_started, Payload = SerializeMessage(saveReceived) };
						SendMessageToClient(serverPacket, _clientSocket);
					}
					break;

				case ClientRequests.Save_cancel:
					saveReceived = DeserializeSave(clientPacket.Payload);
					concernedSave = _saveStore.GetSave(saveReceived.Id);

					if (concernedSave.IsExecuting == true)
					{
						_saveStore.StopSave(concernedSave.Id);
					}
					else
					{
						serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_already_canceled, Payload = SerializeMessage(saveReceived) };
						SendMessageToClient(serverPacket, _clientSocket);
					}
					break;

				case ClientRequests.Save_resume:
					saveReceived = DeserializeSave(clientPacket.Payload);
					concernedSave = _saveStore.GetSave(saveReceived.Id);

					if (concernedSave.IsPaused == true)
					{
						_saveStore.ResumeSave(concernedSave.Id);
					}
					else
					{
						serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_already_resumed, Payload = SerializeMessage(saveReceived) };
						SendMessageToClient(serverPacket, _clientSocket);
					}
					break;

				case ClientRequests.Save_pause:
					saveReceived = DeserializeSave(clientPacket.Payload);
					concernedSave = _saveStore.GetSave(saveReceived.Id);

					if (concernedSave.IsPaused == false)
					{
						_saveStore.PauseSave(concernedSave.Id, true);
					}
					else
					{
						serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_already_paused, Payload = SerializeMessage(saveReceived) };
						SendMessageToClient(serverPacket, _clientSocket);
					}
					break;

				default:
					break;
			}
		}

		private void SendMessageToClient(ServerPacket serverPacket, Socket _clientSocket)
		{
			byte[] serverPacketData = SerializeMessage(serverPacket);
			_clientSocket.Send(BitConverter.GetBytes(serverPacketData.Length));
			_clientSocket.Send(serverPacketData);
		}

		#region Event Handler

		public void OnCopyFilePreview(object sender, FileCopyPreviewEventArgs eventArgs)
		{
			Task.Run(() =>
			{
				ServerPacket serverPacket;
				foreach (Socket _clientSocket in _clientsSockets)
				{
					serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_progress_update, Payload = SerializeMessage(eventArgs.ExecutedSave) };
					SendMessageToClient(serverPacket, _clientSocket);
				}
			});
		}

		public void OnSaveStarted(object sender, Save save)
		{
			Task.Run(() =>
			{
				ServerPacket serverPacket;
				foreach (Socket _clientSocket in _clientsSockets)
				{
					serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_started, Payload = SerializeMessage(save) };
					SendMessageToClient(serverPacket, _clientSocket);
				}
			});
		}

		public void OnSaveFinished(object sender, Save save)
		{
			Task.Run(() =>
			{
				ServerPacket serverPacket;
				foreach (Socket _clientSocket in _clientsSockets)
				{
					serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_finished, Payload = SerializeMessage(save) };
					SendMessageToClient(serverPacket, _clientSocket);
				}
			});
		}

		public void OnSaveCreated(object  sender, Save saveCreated)
		{
			Task.Run(() =>
			{
				ServerPacket serverPacket;
				foreach (Socket _clientSocket in _clientsSockets)
				{
					serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_created, Payload = SerializeMessage(saveCreated) };
					SendMessageToClient(serverPacket, _clientSocket);
				}
			});
		}

		public void OnSaveDeleted(object sender, Save saveDeleted)
		{
			Task.Run(() =>
			{
				ServerPacket serverPacket;
				foreach (Socket _clientSocket in _clientsSockets)
				{
					serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_deleted, Payload = SerializeMessage(saveDeleted) };
					SendMessageToClient(serverPacket, _clientSocket);
				}
			});
		}

		public void OnSaveEdited(object sender, Save saveEdited)
		{
			Task.Run(() =>
			{
				ServerPacket serverPacket;
				foreach (Socket _clientSocket in _clientsSockets)
				{
					serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_edited, Payload = SerializeMessage(saveEdited) };
					SendMessageToClient(serverPacket, _clientSocket);
				}
			});
		}

		public void OnSavePaused(object sender, Save save)
		{
			Task.Run(() =>
			{
				ServerPacket serverPacket;
				foreach (Socket _clientSocket in _clientsSockets)
				{
					serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_paused, Payload = SerializeMessage(save) };
					SendMessageToClient(serverPacket, _clientSocket);
				}
			});
		}

		public void OnSaveResumed(object sender, Save save)
		{
			Task.Run(() =>
			{
				ServerPacket serverPacket;
				foreach (Socket _clientSocket in _clientsSockets)
				{
					serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_resumed, Payload = SerializeMessage(save) };
					SendMessageToClient(serverPacket, _clientSocket);
				}
			});
		}

		public void OnSaveStopped(object sender, Save save)
		{
			Task.Run(() =>
			{
				ServerPacket serverPacket;
				foreach (Socket _clientSocket in _clientsSockets)
				{
					serverPacket = new ServerPacket() { ServerResponse = ServerResponses.Save_stopped, Payload = SerializeMessage(save) };
					SendMessageToClient(serverPacket, _clientSocket);
				}
			});
		}

		#endregion

		#region Serialization / Deserialization

		private byte[] SerializeMessage(object messageToSerialize)
		{
			return JsonSerializer.SerializeToUtf8Bytes(messageToSerialize);
		}

		private ClientPacket DeserializeMessage(byte[] data)
		{
			string json = Encoding.UTF8.GetString(data);
			ClientPacket clientPacket = JsonSerializer.Deserialize<ClientPacket>(json);

			return clientPacket;
		}

		private Save DeserializeSave(byte[] data)
		{
			string json = Encoding.UTF8.GetString(data);
			Save save = JsonSerializer.Deserialize<Save>(json);

			return save;
		}

		#endregion
	}
}
