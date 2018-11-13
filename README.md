# MemTestHelper
AutoIT GUI to automate HCI MemTest

![Screenshot](https://github.com/integralfx/MemTestHelper/raw/master/memtest_helper.jpg)

## Usage
* Download my [modfied version](https://github.com/integralfx/MemTestHelper/raw/master/memtest_6.0_no_nag.exe) of HCI MemTest
* Download memtest_helper from [releases](https://github.com/integralfx/MemTestHelper/releases)
* Run memtest_helper

## Settings
### RAM to test
Exactly as its name implies. This number will be divided by the number of threads and then input in each HCI MemTest instance.
Clicking on it will automatically input the amount of free RAM your computer has.

### Number of threads
How many HCI MemTest instances to run and hence the amount of CPU threads to use.

### Number of rows
By default, MemTestHelper will use all of your CPU threads. Say your CPU has 8 threads, so there will be 8 HCI MemTest instances running. They will be centered on the primary monitor. Number of rows changes how those 8 instances are centered. If it's 2, it will put 4 instances on each row. It's best to play around with it to understand how it works.

### X/Y offset
By default, the 8 instances will be centered, but you can move them around using x and y offset. Note that higher Y values will move the instances down.

### Stop at (%)
Automatically stop each instance as they hit the coverage number entered in the textbox.

## To-do
* Be able to stop after x number of errors
