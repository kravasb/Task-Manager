﻿using System;
using System.Collections.Generic;
using System.IO;
using ConnectToWcf;
using EntitiesLibrary.CommandArguments;
using FluentAssertions;
using NSubstitute;
using Xunit;
using System.Linq;

namespace TaskManagerClientLibrary.ConcreteCommands
{
    public class CompleteCommand : ICommand
    {
        private readonly IClient client;
        public string Name { get { return GetType().Name.Split(new[] { "Command" }, StringSplitOptions.None)[0].ToLower(); } }
        public string Description { get; private set; }
        private readonly TaskArgsConverter converter;
        private readonly TextWriter textWriter;

        public CompleteCommand(TaskArgsConverter converter, TextWriter textWriter, IClient client)
        {
            this.converter = converter;
            this.textWriter = textWriter;
            this.client = client;
            Description = "Mark task by ID as completed.";
        }

        public void Execute(List<string> argument)
        {
            var completeTaskArgs = ConvertToArgs(argument);

            client.ExecuteCommand(completeTaskArgs);
            PrintInfo(completeTaskArgs);
        }

        private void PrintInfo(CompleteTaskArgs completeTaskArgs)
        {
            textWriter.WriteLine("Task ID: {0} completed.", completeTaskArgs.Id);
        }

        private CompleteTaskArgs ConvertToArgs(List<string> argument)
        {
            return converter.Convert(argument, new List<Type>{typeof(CompleteTaskArgs)}) as CompleteTaskArgs;
        }
    }

    public class CompleteTests
    {
        private readonly IClient client = Substitute.For<IClient>();
        private readonly TaskArgsConverter converter = Substitute.For<TaskArgsConverter>();
        private readonly CompleteCommand command;
        readonly CompleteTaskArgs args = new CompleteTaskArgs { Id = 1 };
        readonly List<string> argument = new List<string> { "1", "10-10-2012" };

        public CompleteTests()
        {
            command = new CompleteCommand(converter, new StringWriter(), client);
            converter
                .Convert(argument, Arg.Is<List<Type>>(list => list.SequenceEqual(new List<Type> {typeof (CompleteTaskArgs)})))
                .Returns(args);
        }

        [Fact]
        public void property_name_should_be_classname()
        {
            command.Name.Should().Be("complete");
        }

        [Fact]
        public void should_send_set_date_to_client()
        {
            command.Execute(argument);
            client.Received().ExecuteCommand(args);
        }
    }
}
