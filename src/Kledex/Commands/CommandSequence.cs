﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kledex.Commands
{
    public abstract class CommandSequence : ICommandSequence
    {
        private readonly List<ICommand> _commands = new List<ICommand>();
        public ReadOnlyCollection<ICommand> Commands => _commands.AsReadOnly();

        /// <summary>
        /// Adds the command to the sequence collection.
        /// </summary>
        /// <param name="command">The command.</param>
        protected void AddCommand(ICommand command)
        {
            _commands.Add(command);
        }
    }

    public class SeqCom<TCommand> where TCommand : ICommand
    {
        public SeqCom(TCommand command)
        {
            Command = command;
        }

        public TCommand Command { get; }
    }
}
