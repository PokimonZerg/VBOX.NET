# VBOX.NET
Simple VirtualBox webservice client on C#

## Getting Started

Start VirtualBox service 
```
> VBoxWebSrv.exe
```
Connect to it from any computer
```C#
var vbox = new VBox("http://localhost/", VBox.STANDARD_PORT);
```
Use it!
```C#
foreach (var machine in vbox.GetMachines())
{
    Console.WriteLine(machine.Name);
    Console.WriteLine(machine.State);
}

var ubuntu = vbox.GetMachines().First(m => m.Name == "Ubuntu");

ubuntu.RestoreSnapshot("example");
ubuntu.Start(StartMode.QT_UI);
ubuntu.Stop();
```
