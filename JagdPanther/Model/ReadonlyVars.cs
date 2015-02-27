using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace JagdPanther.Model
{
    public static class ReadonlyVars
    {
        public static Version ProgramVer
        { get; }
        = Assembly
            .GetExecutingAssembly()
            .GetName().Version;
        public static string CurrentFolder { get; } = Directory.GetCurrentDirectory();

        public static string ProgramName
        { get; }
        = Assembly
            .GetExecutingAssembly()
            .GetName().Name;

    }
}