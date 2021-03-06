﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface INotifyDisposable: IDisposable
    {
        void Attach(IDisposable child, bool keepAlive = false);

        void Dettach(IDisposable child);

        bool IsDisposed { get; }
    }
}
