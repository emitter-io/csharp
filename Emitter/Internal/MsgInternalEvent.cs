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


using Emitter.Messages;

namespace Emitter.Internal
{
    /// <summary>
    /// Internal event with a message
    /// </summary>
    public class MsgInternalEvent : InternalEvent
    {
        #region Properties ...

        /// <summary>
        /// Related message
        /// </summary>
        public MqttMsgBase Message
        {
            get { return this.msg; }
            set { this.msg = value; }
        }

        #endregion

        // related message
        protected MqttMsgBase msg;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msg">Related message</param>
        public MsgInternalEvent(MqttMsgBase msg)
        {
            this.msg = msg;
        }
    }
}
