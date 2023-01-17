using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dan.UncannyErrors.Scripts
{
    internal static class UncannyErrorsLogListener
    {
        internal static HashSet<string> errorList = new();

        [InitializeOnLoadMethod]
        private static void OnLoad() => Application.logMessageReceived += LogMessageReceived;
        
        private static void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Error) return;
            
            errorList.Add(condition);
            UncannyErrorsManager.OnError();
        }
        
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnWaitRecompile()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                errorList = new HashSet<string>();
                EditorApplication.delayCall += OnWaitRecompile;
                return;
            }

            EditorApplication.delayCall += UncannyErrorsManager.OnRecompile;
        }
    }
}