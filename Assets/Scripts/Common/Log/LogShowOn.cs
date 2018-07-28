using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class LogShowOn:MonoBehaviour
{
    
    internal void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        Log = true;
    }
    internal void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    private string m_logs;
    /// <summary>
    ///
    /// </summary>
    /// <param name="logString">错误信息</param>
    /// <param name="stackTrace">跟踪堆栈</param>
    /// <param name="type">错误类型</param>
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        m_logs += logString + "\n";
    }
    public bool Log;
    private Vector2 m_scroll;
    internal void OnGUI()
    {
        if (!Log)
            return;
        m_scroll = GUILayout.BeginScrollView(m_scroll);
        GUILayout.Label(m_logs);
        GUILayout.EndScrollView();
    }
}
