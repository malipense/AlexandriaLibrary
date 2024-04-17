using System;

namespace arp_poisoning_tool.DataObjects
{
    internal class NetworkCardInterface
    {
        public NetworkCardInterface(string name, string description)
        {
            Name = Format(name);
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        private string Format(string name)
        {
            string[] data = name.Split('\\');
            string qualifiedName = ""; 
            foreach (var i in data)
            {
                if (i.StartsWith("Device") || i.StartsWith("NPF"))
                {
                    qualifiedName += ("\\" + i);
                }
            }
            return qualifiedName.Trim();
        }

        public override string ToString()
        {
            return $"{Name} // {Description}"; 
        }
    }
}
