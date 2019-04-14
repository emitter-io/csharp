using System;
using System.Collections.Generic;
using System.Text;

namespace Emitter
{
    public partial class Connection
    {
        public event MeHandler Me;

        public void MeInfo()
        {
            this.Publish("emitter/", "me", new byte[]{});
        }
    }
}
