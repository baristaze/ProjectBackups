using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pic4Pic.ObjectModel
{
    public partial class Logger
    {
        public static String AppType = "ServerSide";

        public static LogBag bag()
        {
            return bag(0);
        }

        public static LogBag bag(bool useRootServiceCall)
        {
            return bag(AppType, useRootServiceCall);
        }

        public static LogBag bag(int stackTraceOffset)
        {
            return bag(AppType, stackTraceOffset);
        }

        protected static LogBag bag(String appType, int stackTraceOffset)
        {
            LogBag bag = new LogBag();
            bag.Add(LogBag.TagAppType, appType);
            bag.Add(LogBag.TagLogId, Guid.NewGuid().ToString());
            addCallerStackTrace(bag, stackTraceOffset);
            return bag;
        }

        protected static LogBag bag(String appType, bool useRootServiceCall)
        {
            LogBag bag = new LogBag();
            bag.Add(LogBag.TagAppType, appType);
            bag.Add(LogBag.TagLogId, Guid.NewGuid().ToString());
            addCallerStackTrace(bag, useRootServiceCall);
            return bag;
        }

        public static String GetCallerMethod(int stackTraceOffset)
        { 
            StackTrace stackTrace = new StackTrace();           // get call stack
            if (stackTrace.FrameCount >= (2 + stackTraceOffset))
            {
                StackFrame caller = stackTrace.GetFrame(2 + stackTraceOffset);
                System.Reflection.MethodBase method = caller.GetMethod();
                return method.Name;
            }

            return null;
        }

        public static String GetCallerMethod(bool useRootServiceCall)
        {
            int i = 0;
            StackTrace stackTrace = new StackTrace();           // get call stack
            while (i < stackTrace.FrameCount)
            {
                StackFrame caller = stackTrace.GetFrame(i++);
                System.Reflection.MethodBase method = caller.GetMethod();
                String className = method.DeclaringType != null ? method.DeclaringType.Name : null;
                String methodName = method.Name;
                String fileName = System.IO.Path.GetFileNameWithoutExtension(caller.GetFileName());
                int lineNumber = caller.GetFileLineNumber();

                if (!String.IsNullOrWhiteSpace(className) && 
                    !String.IsNullOrWhiteSpace(methodName) && 
                    className == "Pic4PicSvc" && 
                    !methodName.StartsWith("_") && 
                    method.IsPublic)
                {
                    return methodName;
                }
            }

            return null;
        }

        protected static void addCallerStackTrace(LogBag bag, int stackTraceOffset)
        {
            StackTrace stackTrace = new StackTrace();           // get call stack
            if (stackTrace.FrameCount >= (2 + stackTraceOffset))
            {
                StackFrame caller = stackTrace.GetFrame(2 + stackTraceOffset);
                System.Reflection.MethodBase method = caller.GetMethod();
                String className = method.DeclaringType != null ? method.DeclaringType.Name : null;
                String methodName = method.Name;
                String fileName = System.IO.Path.GetFileNameWithoutExtension(caller.GetFileName());
                int lineNumber = caller.GetFileLineNumber();

                if (!String.IsNullOrWhiteSpace(fileName)) 
                {
                    bag.Add(LogBag.TagFileName, fileName);
                }

                if (!String.IsNullOrWhiteSpace(className))
                {
                    bag.Add(LogBag.TagClassName, className);
                }

                if (!String.IsNullOrWhiteSpace(methodName))
                {
                    bag.Add(LogBag.TagMethodName, methodName);
                }

                if (lineNumber > 0)
                {
                    bag.Add(LogBag.TagLineNumber, lineNumber.ToString());
                }
            }
        }

        protected static void addCallerStackTrace(LogBag bag, bool useRootServiceCall)
        {
            int i = 0;
            StackTrace stackTrace = new StackTrace();           // get call stack
            while (i < stackTrace.FrameCount)
            {
                StackFrame caller = stackTrace.GetFrame(i++);
                System.Reflection.MethodBase method = caller.GetMethod();
                String className = method.DeclaringType != null ? method.DeclaringType.Name : null;
                String methodName = method.Name;
                String fileName = System.IO.Path.GetFileNameWithoutExtension(caller.GetFileName());
                int lineNumber = caller.GetFileLineNumber();

                if (!String.IsNullOrWhiteSpace(className) && 
                    !String.IsNullOrWhiteSpace(methodName) && 
                    className == "Pic4PicSvc" && 
                    !methodName.StartsWith("_") && 
                    method.IsPublic)
                {
                    if (!String.IsNullOrWhiteSpace(fileName))
                    {
                        bag.Add(LogBag.TagFileName, fileName);
                    }

                    if (!String.IsNullOrWhiteSpace(className))
                    {
                        bag.Add(LogBag.TagClassName, className);
                    }

                    if (!String.IsNullOrWhiteSpace(methodName))
                    {
                        bag.Add(LogBag.TagMethodName, methodName);
                    }

                    if (lineNumber > 0)
                    {
                        bag.Add(LogBag.TagLineNumber, lineNumber.ToString());
                    }

                    break;
                }
            }
        }
    }
}
