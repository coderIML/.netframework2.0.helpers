﻿//-----------------------------------------------------------------------
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
    /// 日志帮助类
    /// </summary>
    /// <remarks>自定义的日志操作类</remarks>
    public class SimpleLogHelper
    {
        private SimpleLogHelper() { }

        /// <summary>
        /// 获取调用方法信息
        /// </summary>
        /// <returns>方法信息</returns>
        private static string GetMethodInfo()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            //获取调用此日志的方法信息(日志内部调用此方法，所以Frame基数为2开始)
            MethodBase method = st.GetFrame(2).GetMethod();
            StringBuilder builder = new StringBuilder(256);
            //方法名所在命名空间和类
            builder.AppendFormat("\r\n方法所在命名空间和类:{0}\r\n", method.ReflectedType.FullName);
            //调用此日志方法的方法名称
            builder.AppendFormat("方法名称:{0}\r\n", method.Name);
            //获取方法参数签名
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo info in parameters)
            {
                builder.AppendFormat("参数类型:{0},参数名称:{1}\r\n", info.ParameterType.FullName, info.Name);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取实体信息
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns>实体信息</returns>
        private static string GetEntityInfo<T>(T entity) where T : class, new()
        {
            string entityInfo = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                //创建XML命名空间
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(sw, entity, ns);
                entityInfo = sw.ToString();
            }
            return entityInfo;
        }

        /// <summary>
        /// 记录实体日志
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="content">备注</param>
        /// <param name="type">日志类型</param>
        /// <remarks>将对象序列化记录到文本日志中去,方便后期查询日志</remarks>
        public static void Write<T>(T entity, string content, LogTypes type = LogTypes.Other) where T : class, new()
        {

            Write(GetEntityInfo(entity) + "\r\n" + content, GetMethodInfo(), null, type, "");
        }

        /// <summary>
        /// 记录实体日志
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="type">日志类型</param>
        public static void Write<T>(T entity, LogTypes type = LogTypes.Other) where T : class, new()
        {
            Write(GetEntityInfo(entity), GetMethodInfo(), null, type, "");
        }

        /// <summary>
        /// 记录实体日志
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="exception">异常对象</param>
        /// <param name="type">日志类型</param>
        public static void Write<T>(T entity, Exception exception, LogTypes type = LogTypes.Exception) where T : class, new()
        {
            Write(GetEntityInfo(entity), GetMethodInfo(), exception, type, "");
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="type">日志类型</param>
        public static void Write(string content, LogTypes type = LogTypes.Other)
        {
            Write(content, GetMethodInfo(), null, type, "");
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="content">备注内容</param>
        /// <param name="type">日志类型</param>
        public static void Write(Exception exception, string content, LogTypes type = LogTypes.Exception)
        {
            Write(content, GetMethodInfo(), exception, type, "");
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="exception">异常对象</param>
        /// <param name="type">日志类型</param>
        public static void Write(Exception exception, LogTypes type = LogTypes.Exception)
        {
            Write(string.Empty, GetMethodInfo(), exception, type, "");
        }

        /// <summary>
        /// 系统日志写入
        /// </summary>
        /// <param name="content">自定义日志内容</param>
        /// <param name="methodInfo">日志发生所在方法信息</param>
        /// <param name="exception">异常日志</param>
        /// <param name="type">日志类型</param>
        /// <param name="savePath">日志保存路径(默认存在应用程序所在文件夹下),后期扩展可用于网络传输日志操作</param>
        public static void Write(string content, string methodInfo, Exception exception, LogTypes type, string savePath)
        {
            if (!string.IsNullOrEmpty(content) || exception != null)
            {
                //不存在自定义日志内容和异常对象时候无需写入日志文件
                string timeString = DateTime.Now.ToString();
                StringBuilder append = new StringBuilder();
                append.AppendFormat("时间:{0}\r\n", timeString);
                if (!string.IsNullOrEmpty(content))
                {
                    append.AppendFormat("内容:{0}\r\n", content);
                }
                if (!string.IsNullOrEmpty(methodInfo))
                {
                    append.AppendFormat("方法信息:{0}\r\n", methodInfo);
                }
                if (exception != null)
                {
                    append.AppendFormat("异常:{0}\r\n", exception.Message);
                    append.AppendFormat("堆栈:{0}\r\n", exception.StackTrace);
                }
                append.Append("****************************");
                WriteLog(append.ToString(), type, savePath);
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="info">日志内容</param>
        /// <param name="type">日志类型</param>
        /// <param name="savePath">日志保存路径(默认存在应用程序所在文件夹下),后期扩展可用于网络传输日志操作</param>
        private static void WriteLog(string info, LogTypes type, string savePath = "")
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
            catch (System.Exception ex)
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
    /// 记录日志类型
    /// </summary>
    public enum LogTypes
    {
        /// <summary>
        /// 异常日志
        /// </summary>
        [Description("异常日志")]
        Exception = 0x0,

        /// <summary>
        /// 特殊日志
        /// </summary>
        [Description("特殊日志")]
        Special = 0x02,

        /// <summary>
        /// 操作日志
        /// </summary>
        [Description("操作日志")]
        Operate = 0x04,

        /// <summary>
        /// 登陆注销
        /// </summary>
        [Description("登陆注销")]
        Login = 0x08,

        /// <summary>
        /// 数据变更
        /// </summary>
        [Description("数据变更")]
        DataChange = 0x16,

        /// <summary>
        /// 其他日志
        /// </summary>
        [Description("其他日志")]
        Other = 0x32,

        /// <summary>
        /// 普通错误
        /// </summary>
        [Description("普通错误")]
        Fault = 0x64,
    }
}