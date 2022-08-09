using System;
using System.Collections.Generic;
using System.Text;

namespace Blockbase.Web.Data.ScaffoldConfig
{
    public class ScaffoldDebugFileWriter
    {

        public ScaffoldDebugFileWriter()
        {
        }

        public void WriteDebug(string toWrite)
        {
            DateTime date = DateTime.Now;
            string path = "ScaffoldConfig/Debugging/" + DateTime.UtcNow.ToString() + ".txt";
            System.IO.File.AppendAllText(path, toWrite);
        }

    }
}