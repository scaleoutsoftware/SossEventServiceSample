# ScaleOut event handling service sample.

This sample project illustrates how to write a dedicated, long-running Windows Service process that handles expirations for objects that reside in the ScaleOut StateServer service.

Many ScaleOut StateServer users choose to handle expiration events in a dedicated process that runs locally on each host StateServer host. Doing so offers several advantages over handling events in their main client application (which is often hosted in an IIS worker process):

 - Improved performance by reducing network usage: The ScaleOut service automatically routes events to the local event handling process.
 - Predictable process lifetime: IIS's w3wp.exe processes are designed to be ephemeral. For example, IIS might decide to stop the worker process because it has been idle for too long, causing missed events if another remote client isn't available to take over.
 - It protects your web app: Generally you only want your w3wp.exe process to be concerned with handling web requests. Event handling logic is run on a thread that's outside of the web request pipeline, so an unhandled exception in your event handling code would bring down the whole w3wp.exe worker process.
 - It matches the availability/scalability model of hosts in the SOSS cluster: the count lifetime of event handling services is the same as the count/lifetime of SOSS hosts, so adding and removing SOSS hosts

## Usage

 - Specify the name of your named cache in the ExpirationEventService's `_cacheName` field.
 - Customize the `ExpirationEventService.HandleObjectExpired` method with your custom expiration event handling logic.
 - Modify the properties of ProjectInstaller.cs to customize the behavior of Windows service--for example, the name of the Windows Service and the account that it runs under can be changed in the VS designer.
 - Use [installutil.exe](https://msdn.microsoft.com/en-us/library/sd8zc8ha.aspx) to install the service on each of your hosts running the ScaleOut StateServer service.
   - Note that the executable can be started with a `-debug` argument during development/debugging if you need to run the process from the command line or from Visual Studio.

## Prerequisites

 - .NET 4.6 or higher.
 - [ScaleOut StateServer](https://www.scaleoutsoftware.com/products/stateserver/) or [ScaleOut ComputeServer](https://www.scaleoutsoftware.com/products/computeserver/) (an instance of this service should be run locally on every ScaleOut host).