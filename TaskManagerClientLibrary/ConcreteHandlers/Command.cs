using System;
using ConnectToWcf;
using TaskManagerService;

namespace TaskManagerClientLibrary.ConcreteHandlers
{
    public abstract class Command<T> : ICommand
    {
        private readonly ArgumentConverter<T> converter;
        protected readonly IClientConnection client;

        public string Name { get; private set; }

        protected Command(Type derived) : this(null, derived, null) { }
        protected Command(IClientConnection client, Type derived, ArgumentConverter<T> converter)
        {
            this.client = client;
            Name = derived.Name.ToLower();
            this.converter = converter;
        }


        protected abstract void ExecuteWithGenericInput(T input);

        public virtual void Execute(object argument)
        {
            var converted = Convert(argument);
            ExecuteWithGenericInput((T)converted);
        }

        private object Convert(object input)
        {
            return converter.Convert((string)input);
        }
    }
}
