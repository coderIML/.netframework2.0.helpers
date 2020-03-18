using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace NET2CommonHelper.Win
{
    public sealed class SimpleLogHelper : NET2CommonHelper.SimpleLogHelper
    {

        protected override string GetLogSaveDefaultPath()
        {
            string directory = string.Empty;
            //get current application absolute path(last char is not slash and not contain application name)
            directory = System.AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(directory))
            {
                if (directory == string.Empty)
                {
                    directory = System.Environment.CurrentDirectory;
                }
            }
            return directory;
        }
    }
}
