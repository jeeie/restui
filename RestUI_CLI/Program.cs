using Photonware.RestUI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RestUI_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("  RestUI_CLI.exe <usercase name> [repeat times]");
                Environment.Exit(0);
            }

            int loop = 1;
            if (args.Length >= 2)
            {
                try
                {
                    loop = Convert.ToInt32(args[1]);
                    if (loop <= 0) throw new ArgumentException();
                }
                catch
                {
                    Console.WriteLine("Usage: ");
                    Console.WriteLine("  RestUI_CLI.exe <usercase name> [repeat times]");
                    Environment.Exit(0);
                }
            }

            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );

            string usercaseName = args[0];

            bool found = false;

            for (int i = 0; i < loop; i++)
            {
                if (loop > 1)
                {
                    Console.WriteLine("Start Loop " + (i + 1));
                    Log("Start Loop " + (i + 1));
                    LogDebug("Start Loop " + (i + 1));

                }
                foreach (UserCase uc in Photonware.RestUI.Core.UserCaseManager.Instance.UserCases)
                {
                    if (uc.Text == usercaseName)
                    {
                        found = true;
                        UserCase o = uc.Clone();

                        var userCaseExecution = new UserCaseExecution(o);
                        userCaseExecution.Context.LoopIndex = i;
                        userCaseExecution.StepExecuted += userCaseExecution_StepExecuted;
                        userCaseExecution.Executed += userCaseExecution_Executed;
                        userCaseExecution.ExecutionStatusChanged += userCaseExecution_ExecutionStatusChanged;

                        userCaseExecution.run();
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Error: the specified usercase not found");
                    Environment.Exit(1);
                }
            }



        }

        private static void userCaseExecution_ExecutionStatusChanged(object sender, ExecutionStatusChangedEventArgs e)
        {
            
        }

        private static void userCaseExecution_Executed(object sender, UserCaseExecutionEventArgs e)
        {
            Console.WriteLine("Usercase " + e.UserCaseExecution.Text + " " + e.UserCaseExecution.Status);
            Log("Usercase " + e.UserCaseExecution.Text + " " + e.UserCaseExecution.Status);
            LogDebug("Usercase " + e.UserCaseExecution.Text + " " + e.UserCaseExecution.Status);

        }

        private static void userCaseExecution_StepExecuted(object sender, UserCaseExecutionEventArgs e)
        {
            Console.WriteLine("Step " + e.ExecutionAction.Text + " " + e.ExecutionAction.Status);
            Log("Step " + e.ExecutionAction.Text + " " + e.ExecutionAction.Status);
            LogDebug("Step " + e.ExecutionAction.Text + " " + e.ExecutionAction.Status); 
            LogDebug(e.ExecutionAction.Result.ToString());

        }

        public static void Log(string text)
        {
            File.AppendAllText("restui.log", text+"\r\n");
        }

        public static void LogDebug(string text)
        {
            File.AppendAllText("restui_debug.log", text + "\r\n");
        }
    }
}
