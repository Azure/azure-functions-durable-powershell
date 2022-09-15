// using Microsoft.DurableTask;
// using System;
// using System.Linq;
// using System.Threading.Tasks;

// namespace DurableEngine
// {
//     internal class WhenAnyTask : DurableSDKTask
//     {
//         internal DurableSDKTask[] tasks;
//         private OrchestrationContext sdkContext;


//         internal WhenAnyTask(DurableSDKTask[] tasks, OrchestrationContext sdkContext) :
//             base(sdkContext)
//         {
//             this.tasks = tasks;
//             this.sdkContext = sdkContext;
//         }

//         internal override OrchestrationAction CreateOrchestrationAction()
//         {
//             var compoundActions = tasks.Select((task) => {

//                 return task.CreateOrchestrationAction();
//             }).ToArray();
//             var action = new WhenAnyAction(compoundActions);
//             return action;
//         }

//         internal override object Result
//         {
//             get
//             {
//                 var task = (Task<Task>) this.dtfxTask;
//                 Task winner = task.Result;
//                 sdkContext.OrchestrationActionCollector.taskMap.TryGetValue(winner, out var result);
//                 return result;
//             }
//         }

//         internal override Exception Exception => throw new NotImplementedException();


//         internal override Task createDTFxTask()
//         {
//             var innertasks = this.tasks.Select(task => task.getDTFxTask());
//             var task = Task.WhenAny(innertasks);
//             return task;
//         }
//     }
// }
