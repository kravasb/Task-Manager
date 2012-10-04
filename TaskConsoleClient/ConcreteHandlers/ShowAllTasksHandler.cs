﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EntitiesLibrary;
using FluentAssertions;
using NSubstitute;
using TaskManagerConsole.Manager;
using Xunit;

namespace TaskManagerConsole.UI.ConcreteHandlers
{
    public class ShowAllTasksHandler : ICommandHandler
    {
        private readonly IClientConnection manager;
        private const string Pattern = @"^(list)$";

        public ShowAllTasksHandler(IClientConnection manager)
        {
            this.manager = manager;
        }

        public bool Matches(string input)
        {
            var regex = new Regex(Pattern);
            return regex.IsMatch(input);
        }

        public void Execute(string input)
        {
            manager
                .GetAllTasks()
                .ForEach(x =>
                         Console.WriteLine("ID: {0}\tTask: {1}\t\t\t\tCompleted: {2}", x.Id, x.Name, x.IsCompleted ? "+" : "-"));
        }
    }
    public class ConcreteHandlerShowAllTests
    {
        readonly IClientConnection manager = Substitute.For<IClientConnection>();
        readonly ICommandHandler handler;

        public ConcreteHandlerShowAllTests()
        {
            handler = new ShowAllTasksHandler(manager);
        }

        [Fact]
        public void should_check_if_string_matches_pattern()
        {
            var matches = handler.Matches("list");
            matches.Should().BeTrue();
        }

        [Fact]
        public void should_execute_the_command()
        {
            manager.GetAllTasks().Returns(new List<ContractTask>());
            handler.Execute("list 1");
            manager.Received().GetAllTasks();
        }
    }
}
