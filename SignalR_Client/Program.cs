using SignalR_Client.SignalR;
using SignalR_WithModel.Domain.Models;

namespace SignalR_Client
{
    public class Program
    {
        public static SignalRClient Client { get; private set; }

        static Program()
        {
            Client = new SignalRClient(@"https://localhost:7181/chatHub", ConnectionType.ConsoleApplication);
            Client.Initialize();
        }

        static async Task Main(string[] args)
        {
            await Client.StartAsync();

            await Client.AuthorizeAsync();

            bool isExit = false;

            while (!isExit)
            {
                string message = Console.ReadLine() ?? "";

                if (message != "exit")
                {
                    await Client.SendMessage(MessageActions.Send, message);
                }
                else
                {
                    isExit = true;
                }
            }
            await Client.LogoutAsync();
        }
    }
}
