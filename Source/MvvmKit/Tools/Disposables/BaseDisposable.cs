﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class BaseDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public void Validate()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected virtual void OnDisposed()
        {
        }

        public void Dispose()
        {
            Validate();
            IsDisposed = true;
            OnDisposed();
        }
    }
}