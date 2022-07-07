using Dynamitey.DynamicObjects;
using System;
using System.Threading.Tasks;

namespace DurableEngine.Tasks
{
    internal abstract class AtomicTask : DurableSDKTask
    {
        internal AtomicTask(OrchestrationContext sdkContext) : base(sdkContext) { }

        internal override bool hasResult()
        {
            return this.dtfxTask.IsCompleted;
        }

        internal override bool isFaulted()
        {
            return this.dtfxTask.IsFaulted;
        }

        internal override object Result
        {
            get
            {
                Task<object> atomicTask = (Task<object>) this.dtfxTask;
                return atomicTask.Result;
            }
        }

        internal override Exception Exception
        {
            get
            {
                Task<object> atomicTask = (Task<object>) this.dtfxTask;
                return atomicTask.Exception;
            }
        }
    }
}
