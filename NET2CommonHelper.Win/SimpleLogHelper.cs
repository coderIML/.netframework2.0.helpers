using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace NET2CommonHelper.Win
{
    public sealed class SimpleLogHelper : NET2CommonHelper.SimpleLogHelper
    {
        private SimpleLogHelper instance = null;
        private static readonly object padlock = new object();

        public SimpleLogHelper Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SimpleLogHelper();
                    }
                    return instance;
                }
            }
        }

        private SimpleLogHelper()
        {

        }

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
