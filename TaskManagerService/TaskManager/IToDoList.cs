﻿#region Using

using System.Collections.Generic;
using EntitiesLibrary;

#endregion


namespace TaskManagerHost.TaskManager
{
    public interface IToDoList
    {
        ContractTask AddTask(ContractTask task);
        ContractTask GetTaskById(int id);
        List<ContractTask> GetAllTasks();
        ContractTask EditTask(ContractTask task);
    }
}