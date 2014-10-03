using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photonware.RestUI.Core.Scripts
{
    public interface IScriptExecutor
    {

        string Name { get; }

        void Run(string script);

        T Evaluate<T>(string script);

        void SetVariable(string name, object value);

        T GetVariable<T>(string name);

    }
}
