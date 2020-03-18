//-----------------------------------------------------------------------
// <copyright company="your company" file="SimpleLogHelper.cs">
//  Copyright (c)  V1.0.0.0  
//  builder  name: arson
//  build    time: 2020-03-10
//  function desc: simle log helper class
//  history  list:
//           2020-03-10 arison build the simle log helper class!
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace NET2CommonHelper
{
    /// <summary>
    /// simle log helper
    /// </summary>
    public abstract class SimpleLogHelper
    {
        /// <summary>
        /// get invoke method info
        /// </summary>
        /// <returns>method info</returns>
        private string GetMethodInfo()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            //get the method that invoke the log method
            MethodBase method = st.GetFrame(2).GetMethod();
            StringBuilder builder = new StringBuilder(256);
            //namespace and base class of the method
            builder.AppendFormat("\r\nnamespace and base class of the invoker's method:{0}\r\n", method.ReflectedType.FullName);
            //invoke the log method of invoker's method
            builder.AppendFormat("invoker's method name:{0}\r\n", method.Name);
            //get invoker's method parameters
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo info in parameters)
            {
                builder.AppendFormat("parameter type:{0},parameter name:{1}\r\n", info.ParameterType.FullName, info.Name);
            }
            return builder.ToString();
        }

        /// <summary>
        /// get entity info
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="entity">entity object</param>
        /// <returns>entity info</returns>
        private string GetEntityInfo<T>(T entity) where T : class, new()
        {
            string entityInfo = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                //create xml namespace
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(sw, entity, ns);
                entityInfo = sw.ToString();
            }
            return entityInfo;
        }

        /// <summary>
        /// record entity log
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="entity">entity object</param>
        /// <param name="content">remark</param>
        /// <param name="type">log type</param>
        /// <remarks>shall convert the entity object to serial log,maybe query it in the future!</remarks>
        public void Write<T>(T entity, string content, LogTypes type = LogTypes.Other) where T : class, new()
        {

            Write(GetEntityInfo(entity) + "\r\n" + content, GetMethodInfo(), null, type, "");
        }

        /// <summary>
        /// record entity log
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="entity">entity object</param>
        /// <param name="type">log type</param>
        public void Write<T>(T entity, LogTypes type = LogTypes.Other) where T : class, new()
        {
            Write(GetEntityInfo(entity), GetMethodInfo(), null, type, "");
        }

        /// <summary>
        /// record entity log
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="entity">entity object</param>
        /// <param name="exception">exception object</param>
        /// <param name="type">log type</param>
        public void Write<T>(T entity, Exception exception, LogTypes type = LogTypes.Exception) where T : class, new()
        {
            Write(GetEntityInfo(entity), GetMethodInfo(), exception, type, "");
        }

        /// <summary>
        /// record text log
        /// </summary>
        /// <param name="content">log content</param>
        /// <param name="type">log type</param>
        public void Write(string content, LogTypes type = LogTypes.Other)
        {
            Write(content, GetMethodInfo(), null, type, "");
        }

        /// <summary>
        /// record exception log
        /// </summary>
        /// <param name="exception">exception object</param>
        /// <param name="content">remark</param>
        /// <param name="type">log type</param>
        public void Write(Exception exception, string content, LogTypes type = LogTypes.Exception)
        {
            Write(content, GetMethodInfo(), exception, type, "");
        }

        /// <summary>
        /// record exception log
        /// </summary>
        /// <param name="exception">exception object</param>
        /// <param name="type">log type</param>
        public void Write(Exception exception, LogTypes type = LogTypes.Exception)
        {
            Write(string.Empty, GetMethodInfo(), exception, type, "");
        }

        /// <summary>
        /// record log
        /// </summary>
        /// <param name="content">custom log content</param>
        /// <param name="methodInfo">log info from invoker's method info</param>
        /// <param name="exception">exception object</param>
        /// <param name="type">log type</param>
        /// <param name="savePath">log saved path(default:the log files are saved into current application executable's path</param>
        public void Write(string content, string methodInfo, Exception exception, LogTypes type, string savePath)
        {
            if (!string.IsNullOrEmpty(content) || exception != null)
            {
                //if there are no custom log content or exception object,the log will be not record!
                string timeString = DateTime.Now.ToString();
                StringBuilder append = new StringBuilder();
                append.AppendFormat("time:{0}\r\n", timeString);
                if (!string.IsNullOrEmpty(content))
                {
                    append.AppendFormat("content:{0}\r\n", content);
                }
                if (!string.IsNullOrEmpty(methodInfo))
                {
                    append.AppendFormat("method info:{0}\r\n", methodInfo);
                }
                if (exception != null)
                {
                    append.AppendFormat("exception:{0}\r\n", exception.Message);
                    append.AppendFormat("heap:{0}\r\n", exception.StackTrace);
                }
                append.Append("****************************");
                WriteLog(append.ToString(), type, savePath);
            }
        }

        /// <summary>
        /// 获取日志保存默认路径
        /// </summary>
        /// <returns></returns>
        protected virtual string GetLogSaveDefaultPath()
        {
            return string.Empty;
        }

        /// <summary>
        /// record log
        /// </summary>
        /// <param name="info">log content</param>
        /// <param name="type">log type</param>
        /// <param name="savePath">log saved path(default:the log files are saved into current application executable's path</param>
        private void WriteLog(string info, LogTypes type, string savePath = "")
        {
            string directory = string.Empty;
            try
            {
                //日志可通过传入绝对路径保存
                if (string.IsNullOrEmpty(savePath))
                {
                    //获取当前执行程序所在绝对路径(不包最后斜杠"\"及后续执行文件名称等)
                    directory = System.AppDomain.CurrentDomain.BaseDirectory;
                    if (string.IsNullOrEmpty(directory))
                    {
                        if (directory == string.Empty)
                        {
                            directory = System.Environment.CurrentDirectory;
                        }
                    }
                }
                //加入下级log文件夹,避免程序异常导致删除其他文件夹
                string rootLogPath = directory + "\\log\\";
                //获取已当前天(年月日)命名的文件夹
                string logPath = directory + "\\log\\" + DateTime.Now.Date.ToString("yyyy-MM-dd");
                DirectoryInfo directInfo = null;
                //检测log下文件夹超过7个则删除最早的文件夹内容
                //原则上只保留30天的异常日志
                if (Directory.Exists(rootLogPath))
                {
                    directInfo = new DirectoryInfo(rootLogPath);
                    //获取log文件夹下的文件夹数量
                    DirectoryInfo[] directorys = directInfo.GetDirectories();
                    List<DirectoryInfo> willRemoveList = new List<DirectoryInfo>();
                    if (directorys.Length > 30)
                    {
                        //Array.Sort(
                        //将log文件夹下的文件夹按时间降序排序
                        Array.Sort(directorys, new DirectoryInfoComparer());
                        for (int i = 0; i < directorys.Length - 30; i++)
                        {
                            //添加待删除的文件夹
                            willRemoveList.Add(directorys[i]);
                        }
                        if (willRemoveList.Count > 0)
                        {
                            for (int i = 0; i < willRemoveList.Count; i++)
                            {
                                //删除文件夹及以下文件
                                willRemoveList[i].Delete(true);
                            }
                        }
                    }
                }
                if (!Directory.Exists(logPath))
                {

                    //以当天日期生产的文件夹不存在则重新创建此文件夹
                    directInfo = Directory.CreateDirectory(logPath);
                }
                else
                {
                    directInfo = new DirectoryInfo(logPath);
                }
                //获取当天日期文件夹下的所有文件
                FileInfo[] files = directInfo.GetFiles();
                if (files.Length >= 100)
                {
                    //单个文件夹下不允许日志文件超过100个
                    return;
                }
                string filePath = string.Empty;
                FileInfo[] logFiles = new FileInfo[files.Length];
                int index = 0;
                foreach (FileInfo file in files)
                {
                    if (file.Name.IndexOf(type.ToString(), StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        logFiles[index++] = file;
                    }
                }
                //获取日志文件数量
                int logCount = index;

                //获取日志类型名称
                string typeName = type.ToString();

                filePath = logPath + @"\" + typeName + "-" + (logCount > 0 ? (logCount - 1) : 0) + ".txt";

                //指定文件路径下的文件已存在，则根据文件大小情况，进行追加日志
                if (File.Exists(filePath))
                {
                    FileInfo f = new FileInfo(filePath);
                    //超过1M的日志文件重新生成日志文件
                    if (f.Length >= 1048576)
                    {
                        filePath = logPath + @"\" + typeName + "-" + logCount + ".txt";
                        FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.SetLength(0);
                        stream.Close();
                    }
                    using (StreamWriter wr = new StreamWriter(new FileStream(filePath, FileMode.Append)))
                    {
                        wr.WriteLine(info);
                        wr.Flush();
                    }
                }
                else
                {
                    FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.SetLength(0);
                    stream.Close();
                    using (StreamWriter wr = new StreamWriter(new FileStream(filePath, FileMode.Append)))
                    {
                        wr.WriteLine(info);
                        wr.Flush();
                    }
                }
            }
            catch 
            {
            }
        }
    }

    /// <summary>
    /// 用于比较DirectoryInfo类型对象的比较类
    /// </summary>
    /// <remarks>只用于比较DirectoryInfo对象</remarks>
    public class DirectoryInfoComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            DirectoryInfo prev = ((DirectoryInfo)x);
            DirectoryInfo next = ((DirectoryInfo)y);
            return DateTime.Compare(prev.CreationTime, next.CreationTime);
        }
    }

    /// <summary>
    /// log type enum 
    /// </summary>
    public enum LogTypes
    {
        /// <summary>
        /// Exception
        /// </summary>
        [Description("Exception")]
        Exception = 0x0,

        /// <summary>
        /// Special
        /// </summary>
        [Description("Special")]
        Special = 0x02,

        /// <summary>
        /// Operate
        /// </summary>
        [Description("Operate")]
        Operate = 0x04,

        /// <summary>
        /// Login
        /// </summary>
        [Description("Login")]
        Login = 0x08,

        /// <summary>
        /// DataChange
        /// </summary>
        [Description("DataChange")]
        DataChange = 0x16,

        /// <summary>
        /// Other
        /// </summary>
        [Description("Other")]
        Other = 0x32,

        /// <summary>
        /// Fault
        /// </summary>
        [Description("Fault")]
        Fault = 0x64,
    }
}
