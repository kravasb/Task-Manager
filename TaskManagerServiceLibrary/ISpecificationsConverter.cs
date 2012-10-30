﻿using EntitiesLibrary.CommandArguments;
using CommandQueryLibrary.ServiceSpecifications;

namespace TaskManagerServiceLibrary
{
    public interface ISpecificationsConverter
    {
        IServiceSpecification GetQuerySpecification(IListCommandArguments specification);
    }
}
