using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

public static class DebugHelper 
{
    static Queue<LogItem> m_vLogs;
    static FileInfo m_logFileInfo;
    static bool m_isInited = false;

    //static DebugHelper()
    //{
    //    Init();
    //}

   public static void FixedUpdate()
    {
        if (!m_isInited)
        {
            return;
        }
        Refresh(Time.fixedDeltaTime);
    }

    public static void Init()
    {
        if (m_isInited)
        {
            return;
        }
        m_isInited = true;
        // 创建文件
        DateTime timeNow = DateTime.Now;
        string path = Application.persistentDataPath + "/GameLog/" + timeNow.ToString("yyyyMMddHHmmss") + ".txt";
        m_logFileInfo = new FileInfo(path);
        var sw = m_logFileInfo.CreateText();
        sw.WriteLine("[{0}] - {1}", Application.productName, timeNow.ToString("yyyy/MM/dd HH:mm:ss"));
        sw.Close();
        Debug.Log("日志文件已创建：" + path);

        // 注册回调
        m_vLogs = new Queue<LogItem>();
        Application.logMessageReceived += OnLogMessage;
        Debug.Log("日志系统已启动");
    }


    private static void Refresh(float dt)
    {
        if (m_vLogs.Count > 0)
        {
            try
            {
                var sw = m_logFileInfo.AppendText();
                var item = m_vLogs.Peek(); // 取队首元素但先不移除
                var timeStr = item.time.ToString("HH:mm:ss.ff");
                var logStr = string.Format("{0}-[{1}]{2}", timeStr, item.logType, item.messageString);
                if (item.logType.Equals(LogType.Error))
                {
                    logStr = string.Format("{0}-[{1}]{2}==>{3}", timeStr, item.logType, item.messageString, item.stackTrace);
                }
                sw.WriteLine(logStr);
                sw.Close();
                m_vLogs.Dequeue(); // 成功执行了再移除队首元素
            }
            catch (IOException)
            {
            }
        }
    }

    private static void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if(type == LogType.Log)
            m_vLogs.Enqueue(new LogItem()
            {
                messageString = condition,
                stackTrace = stackTrace,
                logType = type,
                time = DateTime.Now
            });
    }

    static public void Log(object message)
    {
        Debug.LogFormat("{1}\n[{0}]\n", System.DateTime.Now, message);
    }

    public static void LogFormat(string format, params object[] args)
    {
        format = new StringBuilder(100).AppendFormat("{1}\n[{0}]\n", System.DateTime.Now, format).ToString();
        
        Debug.LogFormat(format, args);
    }

}

public struct LogItem
{
    /// <summary>
    /// 日志内容
    /// </summary>
    public string messageString;

    /// <summary>
    /// 调用堆栈
    /// </summary>
    public string stackTrace;

    /// <summary>
    /// 日志类型
    /// </summary>
    public LogType logType;

    /// <summary>
    /// 记录时间
    /// </summary>
    public DateTime time;
}