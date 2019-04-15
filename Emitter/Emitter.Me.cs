using System;
using System.Collections.Generic;
using System.Text;
using Emitter.Messages;

namespace Emitter
{
    public partial class Connection
    {
        /// <summary>
        /// Represents a Me event handler.
        /// </summary>
        /// <param name="meResponse"></param>
        public delegate void MeHandler(MeResponse meResponse);

        public event MeHandler Me;

        public void MeInfo()
        {
            this.Publish("emitter/", "me", new byte[]{});
        }
    }
}
