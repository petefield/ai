using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.Console.Tools
{
    internal class ToolAttribute : Attribute
    {
        public ToolAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }

    internal class ToolParameterAttribute : Attribute
    {
        public ToolParameterAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
