# Table of Contents
1. [Setup](#setup)
2. [Expectations/Limitations](#expectationslimitations)
   1. [Motherboard](#motherboard)
   2. [ICs](#integrated-circuits-ics)
      1. [Thaiphoon Report](#thaiphoon-report)
      2. [A Note on Ranks and Density](#a-note-on-ranks-and-density)
      3. [Voltage Scaling](#voltage-scaling)
      4. [Expected Max Frequency](#expected-max-frequency)
      5. [Binning](#binning)
      6. [Maximum Recommended Daily Voltage](#maximum-recommended-daily-voltage)
   3. [Integrated Memory Controller (IMC)](#integrated-memory-controller-imc)
      1. [Intel - LGA1151](#intel---lga1151)
      2. [AMD - AM4](#amd---am4)
3. [Overclocking](#overclocking)
   1. [Finding the Maximum Frequency](#finding-the-maximum-frequency)
   2. [Trying Higher Frequencies](#trying-higher-frequencies)
   3. [Tightening Timings](#tightening-timings)
   4. [Miscellaneous Tips](#miscellaneous-tips)
      1. [Intel](#intel)
      2. [AMD](#amd)
4. [Useful Information](#useful-information)

# Setup
* Ensure your sticks are in the recommended DIMM slots (usually 2 and 4).
* Make sure your CPU is fully stable before overclocking RAM, as an unstable CPU can lead to memory errors. When pushing high frequency with tight timings, it's possible that your CPU can become unstable.
* Make sure your UEFI is up to date.
* [Thaiphoon](http://www.softnology.biz/files.html) to show what ICs (integrated circuits or RAM chips) your sticks use. This will give you an idea of what frequency and timings to expect.
* [MemTestHelper](https://github.com/integralfx/MemTestHelper/releases) or your memory tester of choice. [Karhu RAM Test](https://www.karhusoftware.com/ramtest/) (paid) is also a good choice. I wouldn't recommend AIDA64 memory test and [Memtest64](https://forums.anandtech.com/threads/techpowerups-memtest-64-is-it-better-than-hci-memtest-for-determining-stability.2532209/) as they are both not very good at finding memory errors.
* To view timings in Windows: 
  * Intel: [Asrock Timing Configurator v4.0.4](https://www.asrock.com/MB/Intel/X299%20OC%20Formula/index.asp#Download) (don't need an Asrock motherboard, though EVGA motherboards don't seem to work with this).
  * AMD: 
    * Ryzen 1000/2000: [Ryzen Timing Checker](https://www.techpowerup.com/download/ryzen-timing-checker/).
    * Ryzen 3000: [Ryzen Master](https://www.amd.com/en/technologies/ryzen-master).
* Benchmarks:
  * [AIDA64](https://www.aida64.com/downloads) - free 30 day trial. We'll be using the cache and memory benchmark (found under tools) to see how our memory is performing. You can right click the start benchmark button and run memory tests only to skip the cache tests.
  * [MaxxMEM2](https://www.softpedia.com/get/System/Benchmarks/MaxxMEM2.shtml) - free alternative to AIDA64, but bandwidth tests seem to be a lot lower so it isn't directly comparable to AIDA64.
  * [Super Pi Mod v1.5 XS](https://www.techpowerup.com/download/super-pi/) - another memory sensitive benchmark, but I haven't used it as much as AIDA64. 1M - 8M digits should be enough for a quick benchmark. You only need to look at the last (total) time, where lower is better.
  * [HWBOT x265 Benchmark](https://hwbot.org/benchmark/hwbot_x265_benchmark_-_1080p/) - I've heard that this benchmark is also sensitive to memory, but I haven't really tested it myself.

# Expectations/Limitations
## Motherboard
* Motherboards with 2 DIMM slots will be able to achieve the highest frequencies.
* For motherboards with 4 DIMM slots, the number of sticks installed will affect your maximum memory frequency. 
  * On motherboards that use a daisy chain [memory trace layout](https://www.youtube.com/watch?v=3vQwGGbW1AE), 2 sticks are preferred. Using 4 sticks may significantly impact your maximum memory frequency.
  * On the other hand, motherboards that use T-topology will overclock the best with 4 sticks. Using 2 sticks won't impact your maximum memory frequency as much as using 4 sticks on a daisy chain motherboard (?).
  * Most vendors don't advertise what memory trace layout they use, but you can make an educated guess based on the QVL. For example, the [Z390 Aorus Master](http://download.gigabyte.asia/FileList/Memory/mb_memory_z390-aorus-master_20190214.pdf) *probably* uses a T-toplogy layout as its highest validated frequency is with 4 DIMMs. If the highest validated frequency were done with 2 DIMMs, it *probably* uses a daisy chain layout.
* Lower end motherboard may not overclock as well, possibly due to the lower PCB quality and number of layers (?).
  
## Integrated Circuits (ICs)
### Thaiphoon Report
* [Single rank 8Gb Hynix CJR](https://i.imgur.com/hbFyKB2.jpg).
* [Single rank 8Gb Micron Revision E](https://i.imgur.com/3pQjQIG.jpg) (source: Coleh#4297).
  * SpecTek is supposedly lower binned Micron ICs.
  * Esoteric note: Many people have started calling this Micron E-die or even just E-die. The former is fine, but the latter can cause confusion as letter-die is typically used for Samsung ICs, i.e. 4Gbit Samsung E-die. Samsung is implied when you say E-die, but as people are calling Micron Rev. E E-die, it'd probably be a good idea to prefix the manufacturer.
* [Dual rank 8Gb Samsung B-die](https://i.imgur.com/Nqn8s76.jpg).

### A Note on Ranks and Density
* Single rank sticks can usually clock higher than dual rank sticks, but at the same frequency and timings, dual rank sticks can perform a bit better due to rank interleaving.
* Density matters when determining how far your ICs can go. For example, 4Gb AFR and 8Gb AFR will not overclock the same despite being AFR.

### Voltage Scaling
* Voltage scaling simply means how the IC responds to voltage.
* On many ICs, tCL scales with voltage, meaning giving it more voltage can allow you to drop tCL. Conversely, tRCD and tRP typically do not scale with voltage on many ICs, meaning no matter how much voltage you pump into it, it will not budge.  
As far as I know, tCL, tRCD, tRP and possibly tRFC can (or can not) see voltage scaling.
* Similarly, if a timing scales with voltage that means you can increase the voltage to run the same timing at a higher frequency.
![8Gbit CJR CL11 Voltage Scaling](https://i.imgur.com/TSiwR7H.png)
  * You can see that tCL scales linearly with voltage on 8Gb CJR.
* Some older Micron ICs (before Rev. E), are known to scale negatively with voltage. That is, they become unstable at the same frequency and timings just by increasing the voltage (usually above 1.35v).
* Here are a table of common ICs and if the timing scales with voltage:

  | IC | tCL | tRCD | tRP |
  | :-: | :-: | :--: | :-: |
  | 8Gb AFR | Y | N | N | 
  | 8Gb CJR | Y | Y (?) | N |
  | 8Gb Rev. E | Y | N | Y |
  | 8Gb B-die | Y | Y | Y |
  * The timings that don't scale with voltage usually need to be increased as you increase frequency.
  
### Expected Max Frequency
* Below are the expected max frequency for some of the common ICs:

  | IC | Expected Max Frequency (MHz) |
  | :-: | :-------------------------: |
  | 8Gb AFR | 3600 |
  | 8Gb CJR | 4000<sup>1</sup> |
  | 8Gb Rev. E | 4000+ |
  | 8Gb B-die | 4000+ |
  * <sup>1</sup>CJR is a bit inconsistent in my testing. I've tested 3 RipJaws V 3600 CL19 8GB sticks. One of them was stuck at 3600MHz, another at 3800MHz but the last could do 4000MHz, all at CL16 with 1.45v.
  * Don't expect lower binned ICs to overclock nearly as well as higher binned ICs. This is especially true for [B-die](https://www.youtube.com/watch?v=rmrap-Jrfww).
  
### Binning
* Binning is basically separating components based on their frequency.  
  Manufacturers would separate ICs into different containers/bins depending on their frequency. Hence the term binning.
* B-die binned for 2400 15-15-15 is significantly worse than good B-die binned for 3200 14-14-14 or even 3000 14-14-14. Don't expect it to have the same voltage scaling characteristics as good B-die.
* To figure out which frequency and timings are a better (tighter) bin within the same IC at the same voltage, find out which timing doesn't scale from voltage.  
  Simply divide the frequency by that timing and the higher value is the tighter bin.
  * For example, Crucial Ballistix 3000 15-16-16 and 3200 16-18-18 both use Micron Rev. E ICs. Simply dividing the frequency by tCL gives us the same value (200), so does that mean they're the same bin?  
  No.  
  tRCD doesn't scale with voltage, which means it needs to be increased as you increase frequency.  
  `3000 / 16 = 187.5` but `3200 / 18 = 177.78`.  
  As you can see, 3000 15-16-16 is a tighter bin than 3200 16-18-18. This means that a kit rated for 3000 15-16-16 will probably be able to do 3200 16-18-18 but a kit rated for 3200 16-18-18 might not be able to do 3000 15-16-16.
  
### Maximum Recommended Daily Voltage
* [JEDEC (p.174)](http://www.softnology.biz/pdf/JESD79-4B.pdf) specifies that the absolute maximum is 1.50v.
  > Stresses greater than those listed under “Absolute Maximum Ratings” may cause permanent damage to the device. This is a stress rating only and functional operation of the device at these or any other conditions above those indicated in the operational sections of this specification is not implied. Exposure to absolute maximum rating conditions for extended periods may affect reliability.
* That being said, I'd only recommend running 1.50v on B-die as it's known to have high voltage tolerance. At least for the common ICs (4/8Gb AFR, 8Gb CJR, 8Gb Rev. E, 4/8Gb MFR), the max recommended voltage is 1.45v. Some of the lesser known ICs like [8Gb C-die](https://www.hardwareluxx.de/community/f13/samsung-8gbit-ddr4-c-die-k4a8g045wc-overclocking-ergebnisse-im-startbeitrag-1198323.html) have been reported to scale negatively or even die above 1.20v, though YMMV.
  
## Integrated Memory Controller (IMC)
### Intel - LGA1151
* Intel's IMC is pretty strong, so it shouldn't be the bottleneck when overclocking.  
  What would you expect from 14+++++?
* There are 2 voltages you need to change if overclocking RAM: system agent (VCCSA) and IO (VCCIO).  
  **DO NOT** leave these on auto, as they can pump dangerous levels of voltage into your IMC, potentially degrading or even killing it. Most of the time you can keep VCCSA and VCCIO the same, but sometimes too much can harm stability. I wouldn't recommend going above 1.25v on each.  
  Below are my suggested VCCSA and VCCIO for 2 single rank DIMMs:

  | Frequency (MHz) | VCCSA/VCCIO (v) |
  | :-------------: | :-------------: |
  | 3000 - 3600 | 1.10 - 1.15 |
  | 3600 - 4000 | 1.15 - 1.20 |
  | 4000+ | 1.20 - 1.25 |
  * With more DIMMs and/or dual rank DIMMs, you may need higher VCCSA and VCCIO than suggested.
* tRCD and tRP are linked, meaning if you set tRCD 16 but tRP 17, both will run at the higher timing (17). This limitation is why many ICs don't do as well on Intel and why B-die is a good match for Intel.
  * On Asrock and EVGA UEFIs, they're combined into tRCDtRP. On ASUS UEFIs, tRP is hidden. On MSI and Gigabyte UEFIs, tRCD and tRP are visible but setting them to different values just sets both of them to the higher value.
* Expected memory latency range: 40ns - 50ns.
  
### AMD - AM4
* Ryzen 1000 and 2000's IMC can be a bit finnicky when overclocking and can't hit as high frequencies as Intel can. Ryzen 3000's IMC is much better and is more or less on par with Intel.
* SOC voltage is the voltage to the IMC and like with Intel, it's not recommended to leave it on auto. You typically want 1.0 - 1.1v as above 1.1v doesn't help much if at all.  
  On Ryzen 2000 (possibly 1000 and 3000 as well), above 1.15v can negatively impact overclocking.
  > There are clear differences in how the memory controller behaves on the different CPU specimens. The majority of the CPUs will do 3466MHz or higher at 1.050V SoC voltage, however the difference lies in how the different specimens react to the voltage. Some of the specimens seem scale with the increased SoC voltage, while the others simply refuse to scale at all or in some cases even illustrate negative scaling. All of the tested samples illustrated negative scaling (i.e. more errors or failures to train) when higher than 1.150V SoC was used. In all cases the maximum memory frequency was achieved at =< 1.100V SoC voltage.  
  [~ The Stilt](https://forums.anandtech.com/threads/ryzen-strictly-technical.2500572/page-72#post-39391302)
  * On Ryzen 3000, there's also CLDO_VDDG (not to be confused with CLDO_VDD**P**), which is the voltage to the Infinity Fabric. I've read that SOC voltage should be 40mV above CLDO_VDDG, but other than that there's not much information about it.
* Below are the expected frequency ranges for 2 single rank DIMMs, provided your motherboard and ICs are capable:

  | Ryzen | Expected Frequency (MHz) |
  | :---: | :----------------------: |
  | 1000 | 3000 - 3466 |
  | 2000 | 3200 - 3600 |
  | 3000 | 3466 - 3800 (1:1 MCLK:FCLK) <br/> 3800+ (2:1 MCLK:FCLK) |
  * With more DIMMs and/or dual rank DIMMs, the expected frequency can be lower.
  * 2 CCD Ryzen 3000 CPUs (3900X and 3950X) seem to prefer 2 dual rank sticks over 4 single rank sticks.
    > For 2 CCD SKUs, 2 DPC SR configuration seems to be the way to go.
Both the 3600 and 3700X did 1800MHz UCLK on 1 DPC DR config, but most likely due to the discrepancy of the two CCDs in 3900X, it barely does 1733MHz on those DIMMs.
Meanwhile with 2 DPC SR config there is no issue in reaching 1866MHz FCLK/UCLK.  
[~ The Stilt](https://www.overclock.net/forum/10-amd-cpus/1728758-strictly-technical-matisse-not-really-26.html)
* tRCD is split into tRCDRD (read) and tRCDWR (write). Usually, tRCDWR can go lower than tRCDRD, but I haven't noticed any performance improvements from lowering tRCDWR. It's best to keep them the same.
* Geardown mode (GDM) is automatically enabled above 2666MHz, which forces even tCL, even tCWL and CR 1T. If you want to run odd tCL, disable GDM. If you're unstable try running CR 2T, but that may negate the performance gain from dropping tCL.
  * For example, if you try to run 3000 CL15 with GDM enabled, CL will be rounded up to 16.
  * In terms of performance: GDM disabled CR 1T > GDM enabled CR 1T > GDM disabled CR 2T.
* On single CCD Ryzen 3000 CPUs (CPUs below 3900X), write bandwidth is halved.
  > In memory bandwidth, we see something odd, the write speed of AMD's 3700X, and that's because of the CDD to IOD connection, where the writes are 16B/cycle on the 3700X, but it's double that on the 3900X. AMD said this let them conserve power, which accounts for part of the lower TDP AMD aimed for. AMD says applications rarely do pure writes, but it did hurt the 3700X's performance in one of our benchmarks on the next page.  
  [~ TweakTown](https://www.tweaktown.com/reviews/9051/amd-ryzen-3900x-3700x-zen2-review/index3.html)
* Expected memory latency range:

  | Ryzen | Latency (ns) |
  | :---: | :----------: |
  | 1000 | 65 - 75 |
  | 2000 | 60 - 70 |
  | 3000 | 65 - 75 (1:1 MCLK:FCLK) <br/> 75+ (2:1 MCLK:FCLK) |
  
# Overclocking
* Disclaimer: The silicon lottery will affect your overclocking potential so there may be some deviation from my suggestions.

## Finding the Maximum Frequency
1. On Intel, start off with 1.15v VCCSA and VCCIO. On AMD, start off with 1.10v SOC.
2. Set DRAM voltage to 1.40v. If you're using Micron/SpecTek ICs, exluding Rev. E, set 1.35v.
3. Set primary timings to 16-20-20-40 (tCL-tRCD-tRP-tRAS).
4. Increase the DRAM frequency until it doesn't boot into Windows any more. Keep in mind the expectations detailed above.
   * If you're on Intel, a quick way of knowing if you're unstable is to examine the RTLs and IOLs. Each group of RTLs and IOLs correspond to a channel. Within each group, there are 2 values which correspond to each DIMM.  
   [Asrock Timing Configurator](https://i.imgur.com/EQBl2wd.jpg)  
   As I have my sticks installed in channel A slot 2 and channel B slot 2, I need to look at D1 within each group of RTLs and IOLs.  
   RTLs should be no more than 2 apart and IOLs should be no more than 1 apart.  
   In my case, RTLs are 53 and 55 which are exactly 2 apart and IOLs are both 7.
   Note that having RTLs and IOLs within those ranges doesn't mean you're stable.
   * If you're on Ryzen 3000, make sure that the Infinity Fabric frequency (FCLK) is set to half your effective DRAM frequency.
5. Run a memory tester of your choice.  
   * Windows will use ~2000MB so make sure to account for that when entering the amount of RAM to test. I have 16GB of RAM and usually test 14000MB.
   * Minimum recommended coverage:
     * MemTestHelper (HCI MemTest): 200% per thread.
     * Karhu RAMTest: 5000%.
       * In the advanced tab, make sure CPU cache is set to enabled. This will speed up testing by ~20%.
6. If you crash/freeze/BSOD or get an error, drop the DRAM frequency by a notch and test again.
7. Save your overclock profile in your UEFI.
8. From this point on you can either: try to go for a higher frequency or work on tightening the timings.
   * Keep in mind the expectations detailed above. If you're at the limit of your ICs and/or IMC it's best just to tighten the timings.
   
## Trying Higher Frequencies
* This section is applicable if you're not at the limit of your motherboard, ICs and IMC.  
  This section is not for those who are having trouble stabilising frequencies within the expected range.
1. Intel:
   * Increase VCCSA and VCCIO to 1.25v.
   * Set command rate (CR) to 2T if it isn't already.
   * Set tCCDL to 8. Asus UEFIs don't expose this timing.
   
   Ryzen 3000:
   * Desynchronising MCLK and FCLK can incur a massive latency penalty, so you're better off tightening timings to keep your MCLK:FCLK 1:1.
   * Otherwise, set FCLK to whatever is stable (1600MHz if you're unsure).
2. Loosen primary timings to 18-22-22-42.
3. Increase DRAM voltage to 1.45v.
5. Follow steps 4-7 from [Finding the Maximum Frequency](#finding-the-maximum-frequency).
6. Proceed to [Tightening Timings](#tightening-timings).
   
## Tightening Timings
* Make sure to run a memory test and benchmark after each change to ensure performance is improving.
  * I would recommend to benchmark 3 to 5 times and average the results, as memory benchmarks can have a bit of variance.
  * Thereotical maximum bandwidth (MB/s) = `ddr_freq * num_channels * 64 / 8`.
  
    | Frequency (MHz) | Max Dual Channel Bandwidth (MB/s) |
    | :-------------: | :------------------------: |
    | 3000 | 48000 |
    | 3200 | 51200 |
    | 3400 | 54440 |
    | 3466 | 55456 |
    | 3600 | 57600 |
    | 3733 | 59728 |
    | 3800 | 60800 |
    | 4000 | 64000 |
    * Your read and write bandwidth should be 90% - 95% of the theoretical maximum bandwidth.
      * On single CCD Ryzen 3000 CPUs, write bandwidth should be 90% - 95% of half of the theoretical maximum bandwidth.

1. AMD:
   * Try disabling GDM and setting CR to 1T. If that doesn't work, leave GDM enabled.
   
   Intel:
   * Try setting CR to 1T. If that doesn't work, leave CR on 2T.

2. I would recommend to tighten some of the secondary timings first, as they can speed up memory testing.  
   My suggestions:
   
   | Timing | Safe | Tight | Extreme |
   | ------ | ---- | ----- | ------- |
   | tRRDS tRRDL tFAW | 6 6 24 | 4 6 16 | 4 4 16 |
   | tWR | 16 | 12 | 10 |
   * Minimum tFAW can be is tRRDS * 4.
   * You don't have to run all of the timings at one preset. You might only be able to run tRRDS tRRDL tFAW at the tight preset, but you may be able to run tWR at the extreme preset.
   
3. Next are the primary timings (tCL, tRCD, tRP).
   * Start with tCL and drop that by 1 until you get instability.
   * Do the same with tRCD and tRP.
   * After the above timings are as tight as they can go, set `tRAS = tCL + tRCD(RD) + 2` and `tRC = tRP + tRAS`.
     * Setting tRAS lower than this can incur a [performance penalty](https://www.overclock.net/forum/25801780-post3757.html).
     * tRC is only available on AMD and some Intel UEFIs.
     
4. Next is tRFC. Default for 8Gb ICs is 350**ns** (note the units).
   * To convert to ns: `2000 * timing / ddr_freq`.  
   For example, tRFC 250 at 3200MHz is `2000 * 250 / 3200 = 156.25ns`.
   * To convert from ns: `ns * ddr_freq / 2000`.  
   For example, 180ns at 3600MHz is `180 * 3600 / 2000 = 324`.
   * Below are the typical tRFC in ns for the common ICs:
   
     | IC | tRFC (ns) |
     | :-: | :-------: |
     | 8Gb AFR | 260 - 280 |
     | 8Gb CJR | 260 - 280 |
     | 8Gb Rev. E | 300 - 350 |
     | 8Gb B-die | 160 - 180 |
     
5. Here are my suggestions for the rest of the secondaries:

   | Timing | Safe | Tight | Extreme |
   | :----: | :--: | :---: | :-----: |
   | tWTRS tWTRL | 4 12 | 4 8 | - |
   | tRTP | 12 | 10 | 8 |
   | tCWL | tCL | tCL - 1 | tCL - 2 |
   * On Intel, tWTRS/L should be left on auto and controlled with tWRRD_dg/sg respectively. Dropping tWRRD_dg by 1 will drop tWTRS by 1. Likewise with tWRRD_sg. Once they're as low as you can go, manually set tWTRS/L.
   
6. Now for the tertiaries:
    * If you're on AMD, refer to [this post](https://redd.it/ahs5a2).  
      My suggestion:
  
       | Timing | Safe | Tight | Extreme |
       | ------ | ---- | ----- | ------- |
       | tRDRDSCL tWRWRSCL | 4 4 | 3 3 | 2 2 |
     
    * If you're on Intel, tune the tertiaries one group at a time.
      * For example, drop all of tRDRD_sg/dg/dr/dd by 1 and run a memory test.  
      Note that dr only affects dual rank sticks, so if you have single rank sticks you can ignore this timing.  
      Do the same with the next group until you've done all the tertiaries.  
      [These](https://i.imgur.com/61ZtPpR.jpg) are my timings on B-die, for reference.
      * tREFI is also a timing that can help with performance. Unlike all the other timings, higher is better for tREFI.  
      It's typically not a good idea to increase tREFI too much as ambient temperature changes (e.g. winter to summer) can be enough to cause instability.
    
## Miscellaneous Tips
* Usually a 200MHz increase in DRAM frequency negates the latency penalty of loosening tCL, tRCD and tRP by 1, but has the benefit of higher bandwidth.  
  For example, 3000 15-17-17 has the same latency as 3200 16-18-18, but 3200 16-18-18 has higher bandwidth.

### Intel
* Higher cache (aka uncore, ring) frequency can increase bandwidth and reduce latency.
* Increase IOL offsets to reduce RTLs and IOLs. Make sure to run a memory test after.  
  More info [here](https://hwbot.org/newsflash/3058_advanced_skylake_overclocking_tune_ddr4_memory_rtlio_on_maximus_viii_with_alexaros_guide).
* If you have an Asus Maximus motherboard and you can't boot, you can try tweaking the skew control values.  
  More info [here](https://rog.asus.com/forum/showthread.php?47670-Maximus-7-Gene-The-road-to-overclocking-memory-without-increasing-voltage).

### AMD
* Try playing around with ProcODT if you can't boot. On Ryzen 1000 and 2000, you should try values between 40Ω and 68.6Ω.  
On Ryzen 3000, [1usmus](https://www.overclock.net/forum/13-amd-general/1640919-new-dram-calculator-ryzena-1-5-1-overclocking-dram-am4-membench-0-7-dram-bench-480.html#post28049664) suggests 28Ω - 40Ω.  
This seems to line up with [The Stilt's](https://www.overclock.net/forum/10-amd-cpus/1728758-strictly-technical-matisse-not-really-26.html) settings.
  > Phy at AGESA defaults, except ProcODT of 40.0Ohm, which is an ASUS auto-rule for Optimem III.
* Lower SOC voltage may help with stability.
* On Ryzen 3000, higher CLDO_VDDP can help with stability above 3600MHz.
  > Increasing cLDO_VDDP seems beneficial > 3600MHz MEMCLKs, as increasing it seems to improve the margins and hence help with potential training issues.  
  [~ The Stilt](https://www.overclock.net/forum/10-amd-cpus/1728758-strictly-technical-matisse-not-really-26.html)

# Useful Information
* [r/overclocking Wiki - DDR4](https://www.reddit.com/r/overclocking/wiki/ram/ddr4)
* [Demystifying Memory Overclocking on Ryzen: OC Guidelines and Explaining Subtimings, Resistances, Voltages, and More! by varexos717](https://redd.it/ahs5a2)
* [Ryzen 3000 Memory / Fabric (X370/X470/X570) by elmor](https://www.overclock.net/forum/13-amd-general/1728878-ryzen-3000-memory-fabric-x370-x470-x570.html)
* [Intel Memory Overclocking Quick Reference by sdch](https://www.overclock.net/forum/27784556-post7836.html)
* [The road to overclocking memory without increasing voltage by Raja@ASUS](https://rog.asus.com/forum/showthread.php?47670-Maximus-7-Gene-The-road-to-overclocking-memory-without-increasing-voltage)
* [Advanced Skylake Overclocking: Tune DDR4 Memory RTL/IO on Maximus VIII with Alex@ro's Guide](https://hwbot.org/newsflash/3058_advanced_skylake_overclocking_tune_ddr4_memory_rtlio_on_maximus_viii_with_alexaros_guide)
