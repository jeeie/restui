using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Photonware.RestUI.Core;

namespace Photonware.RestUI.Actions
{
    public class SleepAction : Photonware.RestUI.Core.Action
    {

        public SleepAction()
            : base()
        {

            Key = "sleep_action";
            Text = "Wait";
            Description = "Waiting for specified seconds";
            this.Examples.Add(new ActionExample() { Content = "10000", Text = "Waiting for 10 Seconds" });
        }

        public override bool Execute(ExecutionAction action, UserCaseExecutionContext context, out object result)
        {
            try
            {
                ManualResetEvent mre = new ManualResetEvent(false);

                int time = Convert.ToInt32(action.Content);
                if (time <= 0)
                {
                    result = string.Empty;
                    return true;
                }
                System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
                {
                    mre.WaitOne();
                });

                t.Start();
                t.Wait(time);

                result = string.Format("{0} milliseconds slept", time);
                return true;
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            
            return false;
        }

    }
}
