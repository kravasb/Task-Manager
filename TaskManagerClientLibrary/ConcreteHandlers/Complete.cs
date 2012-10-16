using System;
using ConnectToWcf;
using NSubstitute;
using TaskManagerServiceLibrary;
using Xunit;

namespace TaskManagerClientLibrary.ConcreteHandlers
{
    public class Complete : Command<int>
    {
        public Complete(IClientConnection client, ArgumentConverter<int> converter)
            : base(client, converter) { }

        protected override void ExecuteWithGenericInput(int input)
        {
            try
            {
                client.Complete(input);
                Console.WriteLine("Task ID: {0} completed.", input);
            }
            catch (TaskNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CompleteTests
    {
        private readonly ArgumentConverter<int> converter = Substitute.For<ArgumentConverter<int>>();
        private readonly IClientConnection client = Substitute.For<IClientConnection>();
        private readonly Complete handler;

        public CompleteTests()
        {
            handler = new Complete(client, converter);
        }

        [Fact]
        public void should_send_string_return_id()
        {
            converter.Convert("5").Returns(5);
            handler.Execute("5");
            client.Received().Complete(5);
        }
    }
}
