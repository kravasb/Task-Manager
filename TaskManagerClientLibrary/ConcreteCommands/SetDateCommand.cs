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
    public class SetDateCommand : ICommand
    {
        private readonly TaskArgsConverter converter;
        private readonly TextWriter textWriter;
        private readonly IClient client;
        public string Name { get { return GetType().Name.Split(new[] { "Command" }, StringSplitOptions.None)[0].ToLower(); } }
        public string Description { get; private set; }

        public SetDateCommand(TaskArgsConverter converter, TextWriter textWriter, IClient client)
        {
            this.converter = converter;
            this.textWriter = textWriter;
            this.client = client;
            Description = "Sets due date for task, specified by ID.";
        }

        public void Execute(List<string> argument)
        {
            var setDateArgs = ConvertToArgs(argument);
            client.ExecuteCommand(setDateArgs);
            PrintInfo(setDateArgs);
        }

        private void PrintInfo(SetDateTaskArgs setDateArgs)
        {
            textWriter.WriteLine("Due date to task assigned. Task Id:" + setDateArgs.Id);
        }

        private SetDateTaskArgs ConvertToArgs(List<string> argument)
        {
            var setDateArgs = converter.Convert(argument, new List<Type>{typeof(SetDateTaskArgs)}) as SetDateTaskArgs;
            return setDateArgs;
        }
    }

    public class SetDateTests
    {
        private readonly IClient client = Substitute.For<IClient>();
        private readonly ICommand command;
        private readonly TaskArgsConverter converter = Substitute.For<TaskArgsConverter>();
        private readonly TextWriter writer = Substitute.For<TextWriter>();
        readonly SetDateTaskArgs args = new SetDateTaskArgs { Id = 5, DueDate = DateTime.Parse("10-10-2012") };
        readonly List<string> argument = new List<string> { "1", "10-10-2012" };

        public SetDateTests()
        {
            command = new SetDateCommand(converter, writer, client);
            converter
                .Convert(argument, Arg.Is<List<Type>>(list => list.SequenceEqual(new List<Type> {typeof (SetDateTaskArgs)})))
                .Returns(args);
        }

        [Fact]
        public void property_name_should_be_classname()
        {
            command.Name.Should().Be("setdate");
        }

        [Fact]
        public void should_send_set_date_to_client()
        {

            command.Execute(argument);
            client.Received().ExecuteCommand(args);
        }
    }
}