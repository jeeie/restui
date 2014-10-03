using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core
{
    public class CustomLoopAction : CustomAction
    {
        private List<CustomAction> actions = new List<CustomAction>();

        public IEnumerable<CustomAction> SubActions
        {
            get
            {
                return actions.AsEnumerable<CustomAction>();

            }
        }
        public void AddSubAction(CustomAction action)
        {
            this.actions.Add(action);

        }
    }
}
