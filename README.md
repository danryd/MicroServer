# Tarro
A  nano application container for .Net - because micro is too big
## What use is it?
If you want to avoid hosting your webapp in IIS or have another long running service of some kind you have to write a bunch of plumbing to get it to run as a windows service (or use something like topshelf).

What you do lose anyway is the dynamic reload which is annoying when you're doing automatic deployments
This little nanoserver allows you to host executables, without external dependencies.

## Features
* Hosting in windows service, installable from the command line
* Runnable as console app
* Built in diagnostic logging
  * To console in UserInteractive mode
  * To eventlog as a service
  
## Roadmap
* Some sort of management interface


## Instructions
Download, build and run.
### Configure
Edit the tarro.config, add an applicaiton set the path to the target application, and the exe to the target executable, ex:

    <Tarro>
     <add name="sample" pathToApp="..\..\..\Tarro.SampleApp\bin\debug" executable="Tarro.SampleApp.exe"/>
    </Tarro

###Run in console
At this point you can runt the tarro.exe and it will host your applicaiton. If you run it on the command line it will log to the console. If you run it as a service it will log errors to the eventlog.

###Install as a service
Take the tarro.exe and tarro.config and copy/move to a folder of choice.
At an administrator command prompt, in the selected folder, type:

    tarro install [optional but recommended:servicename]

Now you can start the service

###Run code on shutdown
Simple add an eventhander to the AppDomain unload event:

    AppDomain.CurrentDomain.DomainUnload += (o,e)=> Cleanup();

