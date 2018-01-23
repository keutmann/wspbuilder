using System;
using System.Collections.Generic;
using System.Text;

namespace Keutmann.SharePoint.WSPBuilder.Commands.Attributes
{
    
    public class Required : Attribute
    {
        public string Message {set; get; }

        public Required()
        {
        }

        public Required(string message)
        {
            this.Message = message;
        }
    }
}
