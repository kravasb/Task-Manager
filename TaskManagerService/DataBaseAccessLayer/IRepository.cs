﻿#region Using

using System.Collections.Generic;
using EntitiesLibrary;

#endregion


namespace TaskManagerHost.DataBaseAccessLayer
{
    public interface IRepository
    {
        int AddTask(string name);
        ServiceTask GetTaskById(int id);
        List<ServiceTask> GetAllTasks();
        void MarkCompleted(int id);
    }
}
