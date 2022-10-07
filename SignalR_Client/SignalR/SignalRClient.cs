using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SignalR_WithModel.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Client.SignalR
{
    public class SignalRClient
    {
        private string _url = @"https://localhost:7181/chatHub";
        private string _userName = "";
        private ConnectionType _connectionType;

        private HubConnection HubConnection { get; set; }

        public SignalRClient(string hubUrl, ConnectionType connectionType)
        {
            _url = hubUrl;
            _connectionType = connectionType;
        }

        public void Initialize()
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(_url)
                .WithAutomaticReconnect()
                .Build();

            SetupHandlers();
        }

        public async Task StartAsync()
        {
            await HubConnection.StartAsync();
        }

        public async Task StopAsync()
        {
            await HubConnection.StopAsync();
        }

        public async Task AuthorizeAsync()
        {
            switch (_connectionType)
            {
                case ConnectionType.ConsoleApplication:
                    {
                        Console.WriteLine($"Welcome to chat at: {_url}");
                        Console.Write("Please, enter your name: ");

                        _userName = Console.ReadLine();

                        if (!string.IsNullOrEmpty(_userName))
                        {
                            Console.WriteLine($"You authorized as: {_userName}");

                            var message = PrepareMessage();

                            await HubConnection.SendAsync(MessageActions.Authorize, message);
                        }

                        break;
                    }
                case ConnectionType.Web:
                case ConnectionType.API:
                default:
                    Console.WriteLine("Not implemented yet.");
                    break;
            }
        }

        public async Task LogoutAsync()
        {
            switch (_connectionType)
            {
                case ConnectionType.ConsoleApplication:
                    {
                        Console.WriteLine($"Thanks for using our chat.");

                        var message = PrepareMessage();

                        await HubConnection.SendAsync(MessageActions.Logout, message);
                        await StopAsync();
                        Console.ReadKey();
                        break;
                    }
                case ConnectionType.Web:
                case ConnectionType.API:
                default:
                    Console.WriteLine("Not implemented yet.");
                    break;
            }
        }

        public async Task SendMessage(string methodName, string message)
        {
            switch (_connectionType)
            {
                case ConnectionType.ConsoleApplication:
                    {
                        if (!string.IsNullOrWhiteSpace(_userName))
                        {
                            var preparedMessage = PrepareMessage(message);

                            await HubConnection.SendAsync(methodName, preparedMessage);
                        }
                        else
                        {
                            Console.WriteLine("Please authorize. Name is required.");
                            await AuthorizeAsync();

                            Console.WriteLine("Do you want to send message again? Please write 'Yes'");
                            var result = Console.ReadLine()?.Trim() ?? "";

                            if (!string.IsNullOrEmpty(result) && result == "Yes")
                            {
                                await SendMessage(methodName, message);
                            }
                        }
                        break;
                    }
                case ConnectionType.Web:
                case ConnectionType.API:
                default:
                    Console.WriteLine("Not implemented yet.");
                    break;
            }
        }

        private string PrepareMessage(string message = "")
        {
            var model = new MessageModel
            {
                Message = message,
                UserName = _userName
            };

            return JsonConvert.SerializeObject(model);
        }

        private void SetupHandlers()
        {
            switch (_connectionType)
            {
                case ConnectionType.ConsoleApplication:
                    HubConnection.On<string>(MessageActions.Send, message => Console.WriteLine(message));
                    HubConnection.On<string>(MessageActions.Authorize, message => Console.WriteLine(message));
                    HubConnection.On<string>(MessageActions.Logout, message => Console.WriteLine(message));
                    break;
                case ConnectionType.Web:
                case ConnectionType.API:
                default:
                    Console.WriteLine("Not implemented yet.");
                    break;
            }
        }
    }
}
