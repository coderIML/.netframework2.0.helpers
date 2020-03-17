﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace NET2CommonHelper.Web
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
            if (HttpContext.Current != null)
            {
                //网站类请求日志存储
                //WCF部署到IIS上的时候，HttpContext.Current还是null
                directory = HttpContext.Current.Server.MapPath("~");
            }
            return directory;
        }
    }
}
