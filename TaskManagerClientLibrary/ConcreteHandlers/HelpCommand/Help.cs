﻿using System.Collections.Generic;
using NSubstitute;
using TaskManagerClientLibrary.ComandContainer;
using Xunit;

namespace TaskManagerClientLibrary.ConcreteHandlers.HelpCommand
{
    public class Help : ICommand
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        private readonly ICommandContainer commands;
        private readonly IHelpDisplayer display;

        public void Execute(List<string> argument)
        {
            foreach (var command in commands.GetCommands())
                display.Show(command);
        }

        public Help(IHelpDisplayer display, ICommandContainer commands)
        {
            Name = "?";
            this.commands = commands;
            this.display = display;
            Description = "Causes help.";
        }
    }

    public class HelpTests
    {

        private readonly ICommand command = Substitute.For<ICommand>();
        private readonly ICommandContainer container = Substitute.For<ICommandContainer>();
        private readonly IHelpDisplayer display = Substitute.For<IHelpDisplayer>();
        private readonly Help help;

        public HelpTests()
        {
            help = new Help(display, container);
        }

        [Fact]
        public void execute_method_test()
        {
            var commands = new List<ICommand> { command };
            container.GetCommands().Returns(commands);
            help.Execute(null);
            foreach (var c in commands)
                display.Received().Show(c);
        }
    }
}