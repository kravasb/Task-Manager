﻿using System;
using EntitiesLibrary;
using FluentAssertions;
using NSubstitute;
using TaskManagerService.DataBaseAccessLayer;
using Xunit;

namespace TaskManagerService.TaskManager
{
    public class TaskFactory : ITaskFactory
    {
        public ServiceTask Create()
        {
            return null;
        }
    }

    public class TaskFactoryTests
    {
    }
}