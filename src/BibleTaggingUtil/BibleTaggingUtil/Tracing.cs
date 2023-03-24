using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil
{
    internal class Tracing
    {
        private const int maxLogsPerDay = 10;
        private const int maxTotalLogs = 100;
        private const int maxFileSize = 2000000;

        private static string traceFolder = string.Empty;



        private static string Params2String(params object[] list)
        {
            StringBuilder sb = new StringBuilder();
            if (list.Length > 0)
            {
                foreach (object item in list)
                {
                    string s = item as string;
                    if (sb.Length > 0)
                        sb.Append(string.Format(", [{0}]", s));
                    else
                        sb.Append(string.Format("[{0}]", s));
                }
            }
            return sb.ToString();
        }

        public static void TraceEntry(string source, params object[] list)
        {
             WriteTrace(source + "_Entry", Params2String(list), TraceEventType.Information);
        }

        public static void TraceExit(string source, params object[] list)
        {
            WriteTrace(source + "_Exit", Params2String(list), TraceEventType.Information);
        }

        public static void TraceInfo(string source, params object[] list)
        {
            WriteTrace(source, Params2String(list), TraceEventType.Information);
        }

        public static void TraceError(string source, params object[] list)
        {
            WriteTrace(source, Params2String(list), TraceEventType.Error);
        }

        public static void TraceException(string source, params object[] list)
        {
            WriteTrace(source, Params2String(list), TraceEventType.Critical);
        }

        public static void WriteTrace(string source, string message, TraceEventType traceEventType)
        {
            Trace.Listeners["tagging"].TraceOutputOptions = TraceOptions.DateTime;
            switch (traceEventType)
            { case TraceEventType.Critical:
                    Trace.Listeners["tagging"].TraceOutputOptions |= TraceOptions.Callstack;
                    break;
            }

            try
            {
                Trace.Listeners["tagging"].TraceEvent(
                            new TraceEventCache(),
                            source,
                            traceEventType, 1, message);
                Trace.Listeners["tagging"].Flush();
            }
            catch (InvalidOperationException ex)
            {
                ((FileLogTraceListener)(Trace.Listeners["tagging"])).CustomLocation =
                    GetNewTraceLogName(traceFolder, false);

                Trace.Listeners["tagging"].TraceEvent(
                            new TraceEventCache(),
                            source,
                            traceEventType, 1, message);
                Trace.Listeners["tagging"].Flush();

            }
        }

        public static void InitialiseTrace(string folder)
        {
            traceFolder = Path.Combine(folder, "Trace");

            if (!Directory.Exists(traceFolder))
            {
                Directory.CreateDirectory(traceFolder);
            }

            FileLogTraceListener myTextListener = new FileLogTraceListener();
            myTextListener.CustomLocation = GetNewTraceLogName(traceFolder, true);
            myTextListener.MaxFileSize = maxFileSize;
            myTextListener.DiskSpaceExhaustedBehavior = DiskSpaceExhaustedOption.ThrowException;

            int idx = Trace.Listeners.Add(myTextListener);
            Trace.Listeners[idx].Name = "tagging";
            Trace.Listeners["tagging"].TraceOutputOptions = TraceOptions.DateTime;
            Trace.Listeners["tagging"].Flush();

            TraceInfo("============= New Trace =============", "============= New Trace =============");
        }

        public static string GetNewTraceLogName(string traceFolder, bool initialising)
        {
            string traceFolderName = string.Empty;

            string traceFileNamePrefix = string.Format("TggingTrace_{0:s}__", DateTime.Now.ToString("yyyy_MM_dd"));

            DirectoryInfo traceDirectory = new DirectoryInfo(traceFolder);

            DirectoryInfo[] allLogFolders = traceDirectory.GetDirectories();
            while (allLogFolders.Length >= maxTotalLogs)
            {
                DeleteOldestLog(allLogFolders);
                allLogFolders = traceDirectory.GetDirectories();
            }

            DirectoryInfo[] todaysLogFolders = traceDirectory.GetDirectories(traceFileNamePrefix + "*"); 

            if(todaysLogFolders.Length == 0)
            {
                // this is the first time today
                traceFolderName = traceFileNamePrefix + "1";
                return Path.Combine(traceFolder, traceFolderName);
            }

            while (todaysLogFolders.Length >= maxLogsPerDay)
            {
                DeleteOldestLog(todaysLogFolders);
                todaysLogFolders = traceDirectory.GetDirectories();
            }

            // find latest folder
            string latestFolder = string.Empty;
            DateTime lastModified = DateTime.MinValue;

            foreach (DirectoryInfo folder in todaysLogFolders)
            {
                if (folder.LastWriteTime > lastModified)
                {
                    lastModified = folder.LastWriteTime;
                    latestFolder = folder.Name;
                }
            }

            if (initialising)
            {
                return Path.Combine(traceFolder, latestFolder);
            }
            string name = Path.GetFileName(latestFolder);
            int idx = name.IndexOf("__");
            if (idx != -1) 
            {
                int logNum = 0;
                if(int.TryParse(name.Substring(idx + 2), out logNum))
                {
                    logNum++;
                    traceFolderName = name.Substring(0, idx + 2) + logNum.ToString();
                }
            }
            return Path.Combine(traceFolder, traceFolderName);
        }

        private static void DeleteOldestLog(DirectoryInfo[] logFolders)
        {
            string OldestFolder = string.Empty;
            DateTime lastModified = DateTime.MaxValue;

            foreach (DirectoryInfo folder in logFolders)
            {
                if (folder.LastWriteTime < lastModified)
                {
                    lastModified = folder.LastWriteTime;
                    OldestFolder = folder.FullName;
                }
            }

            Directory.Delete(OldestFolder, true);
        }

    }
}
