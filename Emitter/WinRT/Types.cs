/*
Copyright (c) 2016 Roman Atachiants
Copyright (c) 2013, 2014 Paolo Patierno

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution.

The Eclipse Public License:  http://www.eclipse.org/legal/epl-v10.html
The Eclipse Distribution License: http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Paolo Patierno - initial API and implementation and/or initial documentation
   Roman Atachiants - integrating with emitter.io
*/

using System.Collections.Generic;

namespace Emitter
{
    /// <summary>
    /// Wrapper Hashtable class for generic Dictionary<TKey,TValue> (the only available in WinRT)
    /// </summary>
    public class Hashtable : Dictionary<object, object>
    {
    }

    /// <summary>
    /// Represents a untyped object stack.
    /// </summary>
    public class Stack : Stack<object>
    {
    }

    /// <summary>
    /// Represents a heterogeneous list of objects.
    /// </summary>
    public class ArrayList : List<object>
    {
    }
}

namespace System.Collections
{
    /// <summary>
    /// Wrapper Queue class for generic Queue<T> (the only available in WinRT)
    /// </summary>
    public class Queue : Queue<object>
    {
    }
}