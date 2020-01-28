using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace NaiRegEdit
{
    public class CmdOptions
    {

        [Option('s', "slot", Default = 1, Required = true, HelpText = "Save slot ( 1 - 6 )")]
        public int Slot { get; set; }

        [Option('d', "disk", Default = false, HelpText = "Save to disk")]
        public bool ToDisk { get; set; }

        [Option('r', "reg", Default = false, HelpText = "Save To Register")]
        public bool ToRegister { get; set; }
    }
}
