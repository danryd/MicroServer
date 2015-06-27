# Tarro
A  nano server for .Net - because micro is too big
## What use is it?
If you want to avoid hosting your webapp in IIS or have another long running service of some kind you have to write a bunch of plumbing to get it to run as a windows service (or use something like topshelf).

What you do lose anyway is the dynamic reload which is annoying when you're doing automatic deployments
This little nanoserver allows you to host an executable, without external dependencies.

## Features
* Hosting in windows service, installable from the command line
* Runnable as console app
* Built in diagnostic logging
  * To console in UserInteractive mode
  * To eventlog as a service
  
## Roadmap
* Some sort of management interface
* Multi application hosting in single server
