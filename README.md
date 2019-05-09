# MemTestHelper
C# WPF to automate HCI MemTest

![Screenshot](https://github.com/integralfx/MemTestHelper/blob/master/memtesthelper.png)

## Usage
* Download [HCI MemTest](https://hcidesign.com/memtest/download.html)
* Download MemTestHelper from [releases](https://github.com/integralfx/MemTestHelper/releases)
* Ensure 'memtest.exe' is in the same folder as 'MemTestHelper.exe'
* Run MemTestHelper

## Settings
### RAM to test
Exactly as its name implies. This number will be divided by the number of threads and then input in each HCI MemTest instance.
Clicking on it will automatically input the amount of free RAM your computer has. If left empty, amount of free RAM will be automatically entered when clicking run.

### Number of threads
How many HCI MemTest instances to run and hence the amount of CPU threads to use.

### Number of rows
By default, MemTestHelper will use all of your CPU threads. Say your CPU has 8 threads, so there will be 8 HCI MemTest instances running. They will be centered on the primary monitor. Number of rows changes how those 8 instances are centered. If it's 2, it will put 4 instances on each row. It's best to play around with it to understand how it works.

### X/Y offset
By default, the 8 instances will be centered, but you can move them around using X and Y offset. Note that higher Y values will move the instances down.

### X/Y spacing
Spacing between each of the MemTest windows.

### Stop at coverage %
Automatically stop each instance as they hit the coverage number entered in the textbox.
Checking "Total" (indicated by the 'T' in the coverage and error info list) will use the total coverage rather than each MemTest instance's coverage.

### Stop on error (default)
Automatically stop if any errors are found

Checking both will stop if either of the above conditions are met.

### Start minimised (default)
Start MemTest instances minimised

### Window height
Height of MemTestHelper

## Error Messages
### Amount of RAM must be at most X
HCI MemTest only (reliably) allows 2048MB per instance. If you have a CPU with 4 threads, then that means you can only test up to 8192MB using 4 threads. To get around this, you can increase the number of threads and hence the number of HCI MemTest instances.
