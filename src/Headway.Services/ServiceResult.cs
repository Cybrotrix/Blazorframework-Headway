﻿using Headway.Core.Interface;

namespace Headway.Services
{
    public class ServiceResult<T> : IServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}
