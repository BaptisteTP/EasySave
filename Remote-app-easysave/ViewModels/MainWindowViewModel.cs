﻿using Remote_app_easysave.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows;
using System.Data;
using System.IO;
using System.Text.Json;
using Remote_app_easysave.Enums;
using Remote_app_easysave.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.VisualBasic;
using System.Windows.Data;

namespace Remote_app_easysave.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{

		#region Attributes

		private Socket _clientSocket;
		private CancellationTokenSource ConnectionCloseTokenSource;

		private ICommand connectionCommand;
		public ICommand ConnectionCommand
		{
			get => connectionCommand;
			set { connectionCommand = value; OnPropertyChanged(); }
		}

		public ICommand StartSaveCommand { get; }
		public ICommand PauseSaveCommand { get; }
		public ICommand ResumeSaveCommand { get; }
		public ICommand CancelSaveCommand { get; }

		public ICollectionView FilteredSaves
		{
			get
			{
				var source = CollectionViewSource.GetDefaultView(Saves);
				source.Filter = save => FilterSaveByName((Save)save);
				return source;
			}
		}

		public ObservableCollection<Save> Saves { get; private set; } = new ObservableCollection<Save>();

		private const string connectedString = "Connected";
		private const string connectingString = "Connecting..";
		private const string notConnectedString = "Not connected";

		private string filterString = "";

		public string FilterString
		{
			get { return filterString; }
			set { filterString = value; OnPropertyChanged(); OnPropertyChanged("FilteredSaves"); }
		}

		private string connectionState;

		public string ConnectionState
		{
			get { return connectionState; }
			set { connectionState = value; OnPropertyChanged(); }
		}

		private string errorMessage;

		public string ErrorMessage
		{
			get { return errorMessage; }
			set { errorMessage = value; OnPropertyChanged(); }
		}

		private string ip = "127.0.0.1";

		public string Ip
		{
			get { return ip; }
			set { ip = value; OnPropertyChanged(); }
		}

		private int port = 8888;

		public int Port
		{
			get { return port; }
			set { port = value; OnPropertyChanged(); }
		}



		#endregion

		public MainWindowViewModel()
		{
			StartSaveCommand = new RelayCommand(StartSave);
			CancelSaveCommand = new RelayCommand(CancelSave);
			PauseSaveCommand = new RelayCommand(PauseSave);
			ResumeSaveCommand = new RelayCommand(ResumeSave);

			ConnectionState = notConnectedString;
			ConnectionCommand = new RelayCommand(ConnectToServer);
		}

		#region Client connection

		private async void ConnectToServer(object obj)
		{
			try
			{
				ConnectionCloseTokenSource = new CancellationTokenSource();
				_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);

				ErrorMessage = string.Empty;
				ConnectionState = connectingString;

				await _clientSocket.ConnectAsync(serverEndPoint);
				ListenToServer();
				AskServerForSaves();
				ConnectionState = connectedString;
				ConnectionCommand = new RelayCommand(DisconnectFromServer);

			}
			catch (SocketException ex)
			{
				ErrorMessage = "La connection au serveur n'a pas pû être établie..";
				ConnectionState = notConnectedString;
			}
			catch (Exception ex)
			{
				ErrorMessage = "Une erreur est survenue lors de la connection au serveur.";
				ConnectionState = notConnectedString;
			}
		}

		private void ListenToServer()
		{
			Task.Run(() =>
			{
				while (true)
				{
					try
					{
						ConnectionCloseTokenSource.Token.ThrowIfCancellationRequested();

						byte[] buffer = new byte[4];
						_clientSocket.Receive(buffer);

						int length = BitConverter.ToInt32(buffer, 0);
						buffer = new byte[length];
						_clientSocket.Receive(buffer);

						HandleServerResponse(buffer);
					}
					catch (SocketException)
					{
						_clientSocket.Close();
						App.Current.Dispatcher.Invoke(() =>
						{
							Saves.Clear();
						});
						ConnectionState = notConnectedString;
						ConnectionCommand = new RelayCommand(ConnectToServer);
						break;
					}
					catch (OperationCanceledException)
					{
						ConnectionCloseTokenSource.Dispose();
						break;
					}
				}
			}, ConnectionCloseTokenSource.Token);
		}

		private void HandleServerResponse(byte[] buffer)
		{
			ServerPacket serverPacket = DeserializeServerResponse(buffer);
			Save receivedSave = DeserializeSave(serverPacket.Payload);
			Save concernedSave;

			switch (serverPacket.ServerResponse)
			{
				case ServerResponses.Send_save:
					App.Current.Dispatcher.Invoke(() =>
					{
						SaveAdd(receivedSave);
					});
					break;
				case ServerResponses.Save_started:
					Saves.First(save => save.Id == receivedSave.Id).IsExecuting = true;
					break;
					
				case ServerResponses.Save_finished:
					concernedSave = Saves.First(save => save.Id == receivedSave.Id);

					concernedSave.IsExecuting = false;
					concernedSave.Progress = 0;
					break;

				case ServerResponses.Save_created:
					App.Current.Dispatcher.Invoke(() =>
					{
						SaveAdd(receivedSave);
					});
					break;

				case ServerResponses.Save_edited:
					concernedSave = Saves.First(save => save.Id == receivedSave.Id);
					App.Current.Dispatcher.Invoke(() =>
					{
						concernedSave = receivedSave;
					});
					break;

				case ServerResponses.Save_deleted:
					concernedSave = Saves.First(save => save.Id == receivedSave.Id);
					App.Current.Dispatcher.Invoke(() =>
					{
						Saves.Remove(concernedSave);
					});
					break;

				case ServerResponses.Save_paused:
					Saves.First(save => save.Id == receivedSave.Id).IsPaused = true;
					break;

				case ServerResponses.Save_resumed:
					Saves.First(save => save.Id == receivedSave.Id).IsPaused = false;
					break;

				case ServerResponses.Save_stopped:
					concernedSave = Saves.First(save => save.Id == receivedSave.Id);
					concernedSave.IsExecuting = false;
					concernedSave.IsPaused = false;
					concernedSave.Progress = 0;
					break;

				case ServerResponses.Save_progress_update:
					Saves.First(save => save.Id == receivedSave.Id).Progress = receivedSave.Progress;
					break;

				case ServerResponses.Save_already_started:
					concernedSave = Saves.First(save => save.Id == receivedSave.Id);
					concernedSave.IsExecuting = true;
					break;

				case ServerResponses.Save_already_paused:
					concernedSave = Saves.First(save => save.Id == receivedSave.Id);
					concernedSave.IsPaused = true;
					break;

				case ServerResponses.Save_already_canceled:
					concernedSave = Saves.First(save => save.Id == receivedSave.Id);
					concernedSave.IsExecuting = false;
					concernedSave.Progress = 0;
					break;

				case ServerResponses.Save_already_resumed:
					concernedSave = Saves.First(save => save.Id == receivedSave.Id);
					concernedSave.IsPaused = false;
					break;

				case ServerResponses.Save_already_deleted:
					break;

				default:
					break;
			}
		}

		private void DisconnectFromServer(object obj)
		{
			Saves.Clear();
			ConnectionCloseTokenSource.Cancel();

			ClientPacket clientPacket = new ClientPacket() { ClientRequest = ClientRequests.Disconnect, Payload = null };
			byte[] data = SerializeMessage(clientPacket);
			_clientSocket.Send(BitConverter.GetBytes(data.Length));
			_clientSocket.Send(data);
			_clientSocket.Close();

			ConnectionState = notConnectedString;
			ConnectionCommand = new RelayCommand(ConnectToServer);

		}

		#endregion

		#region Saves actions

		private void AskServerForSaves()
		{
			Saves.Clear();
			ClientPacket clientPacket = new ClientPacket() { ClientRequest = ClientRequests.Saves_request, Payload = null };
			byte[] data = SerializeMessage(clientPacket);

			_clientSocket.Send(BitConverter.GetBytes(data.Length));
			_clientSocket.Send(data);
		}

		private void ResumeSave(object obj)
		{
			if (obj is Save saveToResume)
			{
				ClientPacket clientPacket = new ClientPacket() { ClientRequest = ClientRequests.Save_resume, Payload = SerializeMessage(saveToResume) };
				byte[] data = SerializeMessage(clientPacket);

				_clientSocket.Send(BitConverter.GetBytes(data.Length));
				_clientSocket.Send(data);
			}
		}

		private void PauseSave(object obj)
		{
			if (obj is Save saveToPause)
			{
				ClientPacket clientPacket = new ClientPacket() { ClientRequest = ClientRequests.Save_pause, Payload = SerializeMessage(saveToPause) };
				byte[] data = SerializeMessage(clientPacket);

				_clientSocket.Send(BitConverter.GetBytes(data.Length));
				_clientSocket.Send(data);
			}
		}

		private void CancelSave(object obj)
		{
			if (obj is Save saveToCancel)
			{
				ClientPacket clientPacket = new ClientPacket() { ClientRequest = ClientRequests.Save_cancel, Payload = SerializeMessage(saveToCancel) };
				byte[] data = SerializeMessage(clientPacket);

				_clientSocket.Send(BitConverter.GetBytes(data.Length));
				_clientSocket.Send(data);
			}
		}

		private void StartSave(object obj)
		{
			if (obj is Save saveToStart)
			{
				ClientPacket clientPacket = new ClientPacket() { ClientRequest = ClientRequests.Save_start, Payload = SerializeMessage(saveToStart) };
				byte[] data = SerializeMessage(clientPacket);

				_clientSocket.Send(BitConverter.GetBytes(data.Length));
				_clientSocket.Send(data);
			}
		}

		#endregion

		#region Utilities
		private void SaveAdd(Save save)
		{
			Saves.Add(save);
			OnPropertyChanged("FilteredSaves");
		}

		private bool FilterSaveByName(Save save)
		{
			return FilterString == "" || save.Name.Contains(FilterString);
		}

		public byte[] SerializeMessage(object objToSerialize)
		{
			return JsonSerializer.SerializeToUtf8Bytes(objToSerialize);
		}

		private ServerPacket DeserializeServerResponse(byte[] data)
		{
			string json = Encoding.UTF8.GetString(data);
			ServerPacket serverPacket = JsonSerializer.Deserialize<ServerPacket>(json);

			return serverPacket;
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
