//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.DurableTask;

//namespace DurableEngine
//{
//    internal class WhenAllTask : DurableSDKTask
//    {
//        internal DurableSDKTask[] tasks;

//        internal override object Result {
//            get
//            {
//                List<object> allResults = new List<object>();
//                foreach (var subTask in this.tasks)
//                {
//                    var res = subTask.Result;
//                    allResults.Add(res);
//                }
//                return allResults; // cache
//            }
//        }

//        internal override Exception Exception => throw new NotImplementedException();

//        internal WhenAllTask(DurableSDKTask[] tasks, OrchestrationContext sdkContext) :
//            base(sdkContext)
//        {
//            this.tasks = tasks;
//        }

//        internal override OrchestrationAction CreateOrchestrationAction()
//        {
//            var compoundActions = tasks.Select((task) => {

//                return task.CreateOrchestrationAction();
//            }).ToArray();
//            var action = new WhenAllAction(compoundActions);
//            return action;
//        }

//        internal override Task createDTFxTask()
//        {
//            var innertasks = this.tasks.Select(task => task.getDTFxTask());
//            var task = Task.WhenAll(innertasks);
//            return task;
//        }
//    }
//}
