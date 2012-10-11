﻿using System.Collections.Generic;
using EntitiesLibrary;
using Xunit;

namespace TaskManagerClientLibrary.ConcreteHandlers.TaskFormatter
{
    public interface ITaskFormatter
    {
        void Show(List<ContractTask> tasks);
        IEnumerable<int> CountRange { get; set; }
    }
}
