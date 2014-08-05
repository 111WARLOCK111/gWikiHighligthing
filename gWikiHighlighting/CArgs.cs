using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gWikiGrabber
{
    public class CArgs
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public bool Optional = false;

        public string Description { get; set; }
    }
}
