﻿using System.ComponentModel;

namespace EntitiesLibrary
{
    [TypeConverter(typeof (CompleteTaskArgsConverter))]
    public class CompleteTaskArgs
    {
        public int Id { get; set; }
    }
}