﻿using System.Text.RegularExpressions;

namespace TaskManagerConsole.ConcreteHandlers
{
    public abstract class Command<T> : ICommandHandler
    {
        protected ArgumentConverter<T> ArgConverter;
        public string Name { get; set; }

        protected abstract void Execute(T input);

        public void Execute(object argument)
        {
            Execute((T)argument);
        }

        public object Convert(object input)
        {
            var converter = new ArgumentConverter<T>();
            return converter.Convert((string)input);
        }
    }
}