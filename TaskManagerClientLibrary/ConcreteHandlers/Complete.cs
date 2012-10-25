﻿using System.Collections.Generic;
using System.IO;
using ConnectToWcf;
using EntitiesLibrary.CommandArguments;
using NSubstitute;
using Xunit;

namespace TaskManagerClientLibrary.ConcreteHandlers
{
    public class Complete : Command<CompleteTaskArgs>
    {
        public Complete(ArgumentConverter<CompleteTaskArgs> converter, TextWriter textWriter, IClientConnection client)
            : base(converter, textWriter)
        {
            Client = client;
            Description = "Mark task by ID as completed.";
        }

        protected override void ExecuteWithGenericInput(CompleteTaskArgs input)
        {
            Client.Complete(input);
            OutText(string.Format("Task ID: {0} completed.", input.Id));
        }

        protected IClientConnection Client { get; set; }
    }

    public class CompleteTests
    {
        private readonly IClientConnection client = Substitute.For<IClientConnection>();

        private readonly ArgumentConverter<CompleteTaskArgs> converter =
            Substitute.For<ArgumentConverter<CompleteTaskArgs>>();

        private readonly Complete command;

        public CompleteTests()
        {
            command = new Complete(converter, new StringWriter(), client);
        }

        [Fact]
        public void should_send_set_date_to_client()
        {
            var completeTaskArgs = new CompleteTaskArgs { Id = 1 };
            var argument = new List<string> { "1", "10-10-2012" };
            converter.Convert(argument).Returns(completeTaskArgs);
            command.Execute(argument);
            client.Received().Complete(completeTaskArgs);
        }
    }
}
