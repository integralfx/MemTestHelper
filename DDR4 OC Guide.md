# Table of Contents
1. [Setup](#setup)
   1. [Memory Testing Software](#memory-testing-software)
      1. [Avoid](#avoid)
      2. [Recommended](#recommended)
      3. [Alternatives](#alternatives)
      4. [Comparison](#comparison)
   2. [Timings Software](#timings-software)
   3. [Benchmarks](#benchmarks)
2. [General RAM Info](#general-ram-info)
   1. [Frequency and Timings Relation](#frequency-and-timings-relation)
   2. [Primary, Secondary and Tertiary Timings](#primary-secondary-and-tertiary-timings)
3. [Expectations/Limitations](#expectationslimitations)
   1. [Motherboard](#motherboard)
   2. [ICs](#integrated-circuits-ics)
      1. [Thaiphoon Report](#thaiphoon-report)
      2. [Label on Sticks](#label-on-sticks)
      3. [A Note on Logical Ranks and Density](#a-note-on-logical-ranks-and-density)
      4. [Voltage Scaling](#voltage-scaling)
      5. [Expected Max Frequency](#expected-max-frequency)
      6. [Binning](#binning)
      7. [Maximum Recommended Daily Voltage](#maximum-recommended-daily-voltage)
      8. [Ranking](#ranking)
      9. [Temperatures and Its Effect on Stability](#temperatures-and-its-effect-on-stability)
   3. [Integrated Memory Controller (IMC)](#integrated-memory-controller-imc)
      1. [Intel - LGA1151](#intel---lga1151)
      2. [AMD - AM4](#amd---am4)
4. [Overclocking](#overclocking)
   1. [Finding a Baseline](#finding-a-baseline)
   2. [Trying Higher Frequencies](#trying-higher-frequencies)
   3. [Tightening Timings](#tightening-timings)
   4. [Miscellaneous Tips](#miscellaneous-tips)
      1. [Intel](#intel)
      2. [AMD](#amd)
5. [Useful Links](#useful-links)
   1. [Benchmarks](#benchmarks-1)
   2. [Information](#information)

# Setup
## Memory Testing Software
You should always test with a variety of stress tests to ensure your overclock is stable.
### Avoid
* I wouldn't recommend the AIDA64 memory test and the [Memtest64](https://forums.anandtech.com/threads/techpowerups-memtest-64-is-it-better-than-hci-memtest-for-determining-stability.2532209/) as they are both not very good at finding memory errors.
### Recommended
* [TM5](http://testmem.tz.ru/tm5.rar) with any of the configs listed:
  * [Extreme config by anta777](https://drive.google.com/file/d/1uegPn9ZuUoWxOssCP4PjMjGW9eC_1VJA) (recommended). Make sure to load the config. It should say 'Customize: Extreme1 @anta777' if loaded.  
  Credits: [u/nucl3arlion](https://www.reddit.com/r/overclocking/comments/dlghvs/micron_reve_high_training_voltage_requirement/f4zcs04/).
  * [Here](https://drive.google.com/drive/u/0/folders/1E-5ZsrlKwr1SlEWxK3iLebms2k3EkQc) is a link to TM5 pre-packaged with many configs.
  * [LMHz Universal 2 config](https://www.hardwareluxx.de/community/threads/ryzen-ram-oc-m%C3%B6gliche-limitierungen.1216557/page-159#post-27506598)
  * If you experience issues with all threads crashing upon launch with the extreme config it might help to edit the row "Testing Window Size (Mb)=1408". Replace the window size with your total RAM (minus some margin for Windows) divided by your processor's available threads (e.g. 12800/16 = 800 MB per thread).
* [OCCT](https://www.ocbase.com/) with the dedicated memory test using either SSE or AVX instructions.
  * Note that AVX and SSE can vary in error detection speed. On Intel-based systems, SSE appears better for testing IMC voltages while AVX appears better for DRAM voltage.
  * The Large AVX2 CPU test is a great stability test for your CPU and RAM at the same time. The more you tune your ram the harder it'll be to stable in this test. Be sure to run Normal Mode as Extreme will not use as much RAM.
  * The VRAM test at maximum utilization in conjunction with Prime95 Large FFTs will stress FCLK and is recommended when testing FCLK stability.
### Alternatives
* [GSAT](https://github.com/stressapptest/stressapptest).
  1. [Install WSL](https://docs.microsoft.com/en-us/windows/wsl/install-win10) and [Ubuntu](https://www.microsoft.com/en-us/p/ubuntu/9nblggh4msv6?activetab=pivot:overviewtab).
  2. Open an Ubuntu bash shell and type `sudo apt update`.
  3. Type `sudo apt-get install stressapptest`.
  4. To start testing type `stressapptest -M 13000 -s 3600 -W --pause_delay 3600`.
     * `-M` is the amount of memory to test (MB).
     * `-s` is how long to test for (seconds).
     * `--pause_delay` is the delay between power spikes. It should be the same as the `-s` argument to skip the power spikes test.
* [Karhu RAM Test](https://www.karhusoftware.com/ramtest/) (paid).
* [y-cruncher](http://www.numberworld.org/y-cruncher/) with [this config](https://pastebin.com/dJQgFtDH).
  * Paste this in a new file called `memtest.cfg` in the same folder as `y-cruncher.exe`.
  * Create a shortcut to `y-cruncher.exe` and add `pause:1 config memtest.cfg` to the target field.
    Your target field should look something like this: `"path\to\y-cruncher\y-cruncher.exe" pause:1 config memtest.cfg`
  * Credits: [u/Nerdsinc](https://www.reddit.com/r/overclocking/comments/iyp1n7/ycruncher_is_a_really_effective_tool_for_testing/)
* [Prime95](https://www.mersenne.org/download/) large FFTs is also decent at finding memory errors.
  * I've been using a custom FFT range of 800k - 800k, though I think any FFT value inside the large FFTs range should work.
    * Make sure 'Run FFTs in place' is not checked.
    * In `prime.txt`, add `TortureAlternateInPlace=0` under `TortureWeak` to prevent P95 from testing in place. Testing in place will only use a bit of RAM, which we don't want.
  * You can create a shortcut to `prime95.exe` and add `-t` to the 'Properties > Target' field to immediately start testing using the settings in `prime.txt`.  
    Your target field should look something like: `"path\to\prime95\prime95.exe" -t`.
  * You can also change the working directory of Prime95's config files, so that you can have one config to stress test your CPU and another config to stress test your RAM.
    1. In the folder with `prime95.exe`, create another folder. For this example, it'll be called 'RAM' (without the quotes).
    2. Copy `prime.txt` and `local.txt` to the folder you just created.
    3. Adjust the settings in `prime.txt` as required.
    4. Create another shortcut to `prime95.exe` and in the target field add `-t -W<folder_name>`.  
       Your target field should look something like: `"path\to\prime95\prime95.exe" -t -WRAM`.
    5. You can now use the shortcut to instantly start Prime95 with the settings provided.
* [randomx-stress](https://github.com/00-matt/randomx-stress/releases) - Can be used to test FCLK stability.
### Comparison
[Comparison](https://imgur.com/a/jhrFGhg) between Karhu RAMTest, TM5 with the extreme config, and GSAT.
  * TM5 is the fastest and most stressful by quite a margin, though I have had instances where I would pass 30 mins of TM5 but fail within 10 mins of Karhu. Another user had a similar experience. YMMV.
    
## Timings Software
* To view timings in Windows: 
  * Intel: 
    * Z370(?)/Z390: [Asrock Timing Configurator v4.0.4](https://www.asrock.com/MB/Intel/X299%20OC%20Formula/index.asp#Download) (don't need an Asrock motherboard).
    * EVGA motherboards and Z170/Z270(?)/Z490: [Asrock Timing Configurator v4.0.3](https://www.asrock.com/mb/Intel/Z170%20OC%20Formula/#Download).
    * For Rocket Lake: [ASRock Timing Configurator v4.0.10](http://picx.xfastest.com/nickshih/asrock/AsrTCSetup(v4.0.10).rar)
    * For Alder Lake: [ASRock Timing Configuator v4.0.13](https://drive.google.com/file/d/11A2CCcXbvAFLVNHPVP9EtZ4hwmsn2yFt/edit) or [MSI Dragon Ball](https://drive.google.com/file/d/1XmKv13D0MgC9fPaA91535wCe9ztoeaHV/view?usp=sharing)
  * AMD: 
    * [ZenTimings](https://zentimings.protonrom.com/).
    
## Benchmarks
* [AIDA64](https://www.aida64.com/downloads) - free 30 day trial. We'll be using the cache and memory benchmark (found under tools) to see how our memory is performing. You can right-click the start benchmark button and run memory tests only to skip the cache tests.
* [Intel Memory Latency Checker](https://software.intel.com/content/www/us/en/develop/articles/intelr-memory-latency-checker.html) - contains a lot of useful tests for measuring memory performance. More extensive data than AIDA64 and bandwidth numbers differ between the tests. Note that it must be run as an administrator to disable prefetching. On AMD systems you may have to disable it in bios.
* [xmrig](https://github.com/xmrig/xmrig) is very memory sensitive so it's useful to test the effects of specific timings. Run as admin with `--bench=1M` as a command-line argument to start the benchmark. Use the benchmark time to compare.
* [MaxxMEM2](https://www.softpedia.com/get/System/Benchmarks/MaxxMEM2.shtml) - free alternative to AIDA64, but bandwidth tests seem to be a lot lower so it isn't directly comparable to AIDA64.
* [Super Pi Mod v1.5 XS](https://www.techpowerup.com/download/super-pi/) - another memory-sensitive benchmark, but I haven't used it as much as AIDA64. 1M - 8M digits should be enough for a quick benchmark. You only need to look at the last (total) time, where lower is better.
* [HWBOT x265 Benchmark](https://hwbot.org/benchmark/hwbot_x265_benchmark_-_1080p/) - I've heard that this benchmark is also sensitive to memory, but I haven't really tested it myself.
* [PYPrime 2.x](https://github.com/monabuntur/PYPrime-2.x) - This benchmark is quick, and scales very well with CPU core clock, cache/FCLK, memory frequency, and timings

# General RAM Info
## Frequency and Timings Relation
* RAM frequency is measured in megahertz (MHz) or million cycles per second. Higher frequency means more cycles per second, which means better performance.
* Esoteric note: People often refer to DDR4-3200 as being 3200**MHz**, however in reality the real frequency of the RAM is only 1600MHz. As data is transferred on both the rising clock edge and falling clock edge in DDR (Double Data Rate) memory, the real frequency of the RAM is half of the number of transfers it makes per second. DDR4-3200 transfers 3200 million bits per second, and so is 3200**MT/s** (MegaTransfers per second) operating at a frequency of 1600**MHz**.
* RAM timings are measured in clock cycles or ticks. Lower timings mean fewer cycles to perform an operation, which means better performance.
  * The exception to this is tREFI, which is the refresh interval. As its name suggests, tREFI is the time between refreshes. While the RAM is refreshing it can't do anything, so you'd want to refresh as infrequently as possible. To do that, you'd want the time between refreshes to be as long as possible. This means you'd want tREFI as high as possible.
* While lower timings may be better, this also depends on the frequency the RAM is running at. For example, DDR4-3000 CL15 and DDR4-3200 CL16 have the same latency, despite DDR4-3000 running at a lower absolute CL. This is because the higher frequency offsets the increase in CL.
* To calculate the actual time in nanoseconds (ns) of a given timing: `2000 * timing / ddr_freq`.
  * For example, CL15 at DDR4-3000 is `2000 * 15 / 3000 = 10ns`.
  * Similarly, CL16 at DDR4-3200 is `2000 * 16 / 3200 = 10ns`.

## Primary, Secondary and Tertiary Timings
* Intel

  ![image](https://user-images.githubusercontent.com/69487009/156914502-104b0cbd-bbc4-4c94-a583-426e84f732eb.png)

* AMD

  ![image](https://user-images.githubusercontent.com/69487009/156914518-5510c6a2-6187-41ee-8acd-a8f76a5590c4.png)

* RAM timings are split into 3 categories: primary, secondary, and tertiary. These are indicated by 'P', 'S', and 'T' respectively.
  * Primary and secondary timings affect latency and bandwidth.
  * Tertiary timings affect bandwidth.
    * The exception to this is tREFI/tREF which affects latency and bandwidth, though it isn't modifiable on AMD.

# Expectations/Limitations
* This section goes through 3 components that may influence your overclocking experience: ICs, motherboard, and IMC.

## Motherboard
* Motherboards with 2 DIMM slots will be able to achieve the highest frequencies.
* For motherboards with 4 DIMM slots, the number of sticks installed will affect your maximum memory frequency. 
  * On motherboards that use a daisy chain [memory trace layout](https://www.youtube.com/watch?v=3vQwGGbW1AE), 2 sticks are preferred. Using 4 sticks may significantly impact your maximum memory frequency.
  * On the other hand, motherboards that use T-topology will overclock the best with 4 sticks. Using 2 sticks won't impact your maximum memory frequency as much as using 4 sticks on a daisy chain motherboard (?).
  * Most vendors don't advertise what memory trace layout they use, but you can make an educated guess based on the QVL. For example, the [Z390 Aorus Master](http://download.gigabyte.asia/FileList/Memory/mb_memory_z390-aorus-master_20190214.pdf) uses a T-Topology layout as its highest validated frequency is with 4 DIMMs. If the highest validated frequency were done with 2 DIMMs, it *probably* uses a daisy chain layout.
  * According to Buildzoid, Daisy Chain vs T-Topology only matters above DDR4-4000. Following Buildzoid's logic, if you're on Ryzen 3000, this doesn't matter as DDR4-3800 is the typical max memory frequency when running MCLK:FCLK 1:1.
* Lower end motherboard may not overclock as well, possibly due to the lower PCB quality and the number of layers (?).
  
## Integrated Circuits (ICs)
* Knowing what ICs (sometimes referred to as "dies") are in your RAM will give you an idea of what to expect. Even if you don't know them you can still overclock your RAM.

### Thaiphoon Report
* Note: Thaiphoon is known to guess ICs so it shouldn't be fully trusted. It's highly recommended to check the label on the sticks if possible.
  * See [here](https://www.reddit.com/r/overclocking/comments/ig9d76/thaiphoon_burner_cluelessly_guessing_memory_ics/) for more info.
* Single rank 8Gb Hynix CJR.

   ![image](https://user-images.githubusercontent.com/69487009/156914589-ad2f0410-f66f-4fa1-8fb5-1c4461c6beb5.png)

* Single rank 8Gb Micron Revision E (source: Coleh#4297).

   ![image](https://user-images.githubusercontent.com/69487009/156914627-fa8f1938-abd1-4401-b9cc-13310ef3877a.png)

  * [SpecTek](https://www.micron.com/support/spectek-support) ICs are lower binned Micron ICs.
  * Esoteric note: Many people have started calling this Micron E-die or even just E-die. The former is fine, but the latter can cause confusion as letter-die is typically used for Samsung ICs, i.e. 4Gbit Samsung E-die. Samsung is implied when you say E-die, but as people are calling Micron Rev. E E-die, it'd probably be a good idea to prefix the manufacturer.
* Dual rank 8Gb Samsung B-die.

   ![image](https://user-images.githubusercontent.com/69487009/156914678-6eed4b9c-5874-4c71-89d3-dd77fba51b5f.png)


### Label on Sticks

Sometimes the Thaiphoon report won't tell you the IC or it may misidentify the IC. To confirm/deny this, you can check the label on the sticks. Currently, only Corsair, G.Skill, and Kingston have a label to identify the IC.

#### Corsair Version Number
* Corsair has a 3 digit version number on the label on the sticks which indicates what ICs are on the stick.
* The first digit is the manufacturer.
  * 3 = Micron
  * 4 = Samsung
  * 5 = Hynix
  * 8 = Nanya
* The second digit is the density.
  * 1 = 2Gb
  * 2 = 4Gb
  * 3 = 8Gb
  * 4 = 16Gb
* The last digit is the revision.
* See the [r/overclocking wiki](https://www.reddit.com/r/overclocking/wiki/ram/ddr4#wiki_corsair) for a full list.
#### G.Skill 042 Code
* Similar to Corsair, G.Skill uses a 042 code to indicate the ICs.
* Example: 04213X**8**8**1**0B
  * The first bolded character is the density. 4 for 4Gb, 8 for 8Gb, and S for 16Gb.
  * The second bolded number is the manufacturer. 1 for Samsung, 2 for Hynix, 3 for Micron, 4 for PSC (powerchip), 5 for Nanya, and 9 for JHICC.
  * The last character is the revision.
  * This is the code for Samsung 8Gb B-die.
* See the [r/overclocking wiki](https://www.reddit.com/r/overclocking/wiki/ram/ddr4#wiki_new_markings_-_.22042_code.22_table) for a full list.
#### Kingston Code
* Example: DPM**M**16A1823
  * The bolded letter indicates the manufacturer. H for Hynix, M for Micron, and S for Samsung.
  * The next 2 digits indicate ranks. 08 = single rank and 16 = dual rank.
  * The next letter indicates the production month. 1-9, A, B, C.
  * THe next 2 digits indicate the production year.
  * This is the code for dual-rank Micron produced in October 2018.
* [Source](http://www.xtremesystems.org/forums/showthread.php?285750-Interesting-memory-deals-thread&p=5230258&viewfull=1#post5230258)

### A Note on Logical Ranks and Density
* Single rank sticks usually clock higher than dual-rank sticks, but depending on the benchmark the performance gain from rank interleaving<sup>1</sup> can be significant enough to outperform faster single-rank sticks. [This can be observed in both synthetics and games](https://kingfaris.co.uk/ram).
   * On recent platforms (Comet Lake and Zen3), BIOS and memory controller support for dual-rank has improved greatly. On many Z490 boards, dual rank Samsung 8Gb B-die (2x16GB) will clock just as high as single-rank B-die, meaning you have all the performance gains of rank interleaving with little to no downsides.
   * <sup>1</sup>Rank interleaving allows the memory controller to parallelize memory requests, for example writing on one rank while the other is refreshing. The impact of this is easily observed in AIDA64 copy bandwidth. From the eyes of the memory controller, it doesn't matter whether the second rank is on the same DIMM (two ranks on one DIMM) or a different DIMM (two DIMM in one channel). It does however matter from an overclocking perspective when you consider memory trace layouts and BIOS support.
   * Having a second rank of the same IC also means that there are twice as many bank groups available. That means that short timings - such as RRD_S rather than RRD_L - can be used more often, as it's more likely for there to be a fresh bank group available. The long timing (L) is required when operating on the same bank group twice in a row, and when there are 7 other bank groups instead of 3 you have a lot more choice to avoid doing that.
   * It also means that there are twice as many banks, and thus twice as many memory rows can be open at any given time. It's more likely that the row that you need will be open.
You won't have to close row A, open row B, and then close B to open A again as often.
You're held up by operations like RAS/RC/RCD (when waiting for a row to open because it was closed) and RP (when waiting for a row to close so that you can open another one) less often.
   * x16 configurations will have half as many banks and bank groups compared to the traditional x8 configurations which mean less performance. See [buildzoid's video](https://www.youtube.com/watch?v=k6SIdxq2yxE) for more information.
* Density matters when determining how far your ICs can go. For example, 4Gb AFR and 8Gb AFR will not overclock the same despite sharing the same name. The same can be said for Micron Rev. B which exists as both 8Gb and 16Gb. The 16Gb ICs overclock better and are sold both in 16GB and 8GB capacities despite the DIMMs using 8 chips. The 8GB sticks have their SPD modified and can be found in higher-end Crucial kits (BLM2K8G51C19U4B).
* As the total count of ranks in a system increases, so does the load on the memory controller. This usually means that more memory ranks will require higher voltage, especially VCCSA on Intel and SOC voltage on AMD.

### Voltage Scaling
* Voltage scaling simply means how the IC responds to voltage.
* On many ICs, tCL scales with voltage, meaning giving it more voltage can allow you to drop tCL. Conversely, tRCD and/or tRP typically do not scale with voltage on many ICs, meaning no matter how much voltage you pump into it, it will not budge.  
As far as I know, tCL, tRCD, tRP, and possibly tRFC can (or can not) see voltage scaling.
* Similarly, if a timing scales with voltage that means you can increase the voltage to run the same timing at a higher frequency.
![CL11 Voltage Scaling](https://i.imgur.com/66GrCz3.png)
  * You can see that tCL scales almost linearly up to DDR4-2533 with voltage on 8Gb CJR.
  * tCL on Samsung B-Die has perfect linear scaling with voltage.
  * tCL on Micron Rev. E also has perfect linear scaling with voltage.
  * I've adapted this data into a [calculator](https://www.desmos.com/calculator/psisrpx3oh). Change the *f* and *v* sliders to the frequency and voltage you want and it will output the frequencies and voltages achievable for a given CL (assuming that CL scales linearly up to 1.50v). For example, DDR4-3200 CL14 at 1.35V should be able to do ~DDR4-3333 CL14 at 1.40V, ~DDR4-3533 CL14 at 1.45V, and DDR4-3733 CL14 at 1.50V.

* B-die tRFC Voltage Scaling
![B-die tRFC Voltage Scaling](https://i.imgur.com/Wngug1M.png)
  * Here you can see that tRFC scales pretty well on B-die.

* Some older Micron ICs (before 8Gb Rev. E), are known to scale negatively with voltage. That is, they become unstable at the same frequency and timings just by increasing the voltage (usually above 1.35V).
* Here is a table of ICs I have tested and if the timing scales with voltage:

  | IC                 | tCL | tRCD | tRP | tRFC |
  | :-:                | :-: | :--: | :-: | :--: |
  | Hynix 8Gb AFR      | Y   | N    | N   | ?    |
  | Hynix 8Gb CJR      | Y   | N    | N   | Y    |
  | Hynix 8Gb DJR      | Y   | N    | N   | Y    |
  | Micron 8Gb Rev. B  | Y   | N    | N   | N    |
  | Micron 8Gb Rev. E  | Y   | N    | N   | N    |
  | Micron 16Gb Rev. B  | Y   | N    | N   | N    |
  | Nanya 8Gb B-die    | Y   | N    | N   | N    |
  | Samsung 4Gb E-die  | Y   | N    | N   | N    |
  | Samsung 8Gb B-die  | Y   | Y    | Y   | Y    |
  | Samsung 8Gb D-die  | Y   | N    | N   | N    |
  * The timings that don't scale with voltage usually need to be increased as you increase frequency.
  
### Expected Max Frequency
* Below are the expected max frequency for some of the common ICs:

  | IC | Expected Max Effective Speed (MT/s) |
  | :-: | :------------: |
  | Hynix 8Gb AFR | 3600 |
  | Hynix 8Gb CJR | 4133<sup>1</sup> |
  | Hynix 8Gb DJR | 5000+ |
  | Nanya 8Gb B-die | 4000+ |
  | Micron 8Gb Rev. B | 3600 |
  | Micron 8Gb Rev. E | 5000+ |
  | Micron 16Gb Rev. B | 5000+ |
  | Samsung 4Gb E-die | 4200+ |
  | Samsung 8Gb B-die | 5000+ |
  | Samsung 8Gb D-die | 4200+ |
  * <sup>1</sup>CJR is a bit inconsistent in my testing. I've tested 3 RipJaws V 3600 CL19 8GB sticks. One of them was stuck at DDR4-3600, another at DDR4-3800 but the last could do DDR4-4000, all at CL16 with 1.45V.
  * Don't expect lower binned ICs to overclock nearly as well as higher binned ICs. This is especially true for [B-die](https://www.youtube.com/watch?v=rmrap-Jrfww).
  * These values are simply referring to the IC's average capabilities, however other factors, such as the motherboard and CPU, have a substantial impact on whether or not said values are attainable.
  
### Binning
* Binning is basically grading components based on their performance characteristics.  
  Manufacturers would separate ICs into different containers/bins depending on their frequency. Hence the term binning.
* G.Skill is one manufacturer known for extensive binning and categorization. Oftentimes, several different SKUs of G.Skill memory will belong to the same factory bin (i.e. DDR4-3600 16-16-16-36 1.35V bin of B-Die being binned the same as DDR4-3200 14-14-14-34 1.35V B-Die).
* B-die binned for 2400 15-15-15 is significantly worse than good B-die binned for DDR4-3200 14-14-14 or even DDR4-3000 14-14-14. Don't expect it to have the same voltage scaling characteristics as a good B-Die.
* To figure out which frequency and timings are a better (tighter) bin within the same IC at the same voltage, find out which timing doesn't scale from voltage.  
  Simply divide the frequency by that timing and the higher value is the tighter bin.
  * For example, Crucial Ballistix DDR4-3000 15-16-16 and DDR4-3200 16-18-18 both use Micron Rev. E ICs. Simply dividing the frequency by tCL gives us the same value (200), so does that mean they're the same bin?  
  No.  
  tRCD doesn't scale with voltage, which means it needs to be increased as you increase frequency.  
  `3000 / 16 = 187.5` but `3200 / 18 = 177.78`.  
  As you can see, DDR4-3000 15-16-16 is a tighter bin than DDR4-3200 16-18-18. This means that a kit rated for DDR4-3000 15-16-16 will probably be able to do DDR4-3200 16-18-18 but a kit rated for DDR4-3200 16-18-18 might not be able to do DDR4-3000 15-16-16. The frequency and timings difference is pretty small, so they'll probably overclock very similarly.
  
### Maximum Recommended Daily Voltage
* [JEDEC JESD79-4B (p.174)](http://www.softnology.biz/pdf/JESD79-4B.pdf) specifies that the absolute maximum is 1.50V.
  > Stresses greater than those listed under “Absolute Maximum Ratings” may cause permanent damage to the device. This is a stress rating only and functional operation of the device at these or any other conditions above those indicated in the operational sections of this specification is not implied. Exposure to absolute maximum rating conditions for extended periods may affect reliability.
* This value is the official maximum of the DDR4 Spec for which all DDR4 are rated, however, numerous ICs are unable to remain safe at such high sustained voltages. [Samsung 8Gb C-die](https://www.hardwareluxx.de/community/f13/samsung-8gbit-ddr4-c-die-k4a8g045wc-overclocking-ergebnisse-im-startbeitrag-1198323.html) can degrade with voltages as low as 1.35V under the right thermal and power delivery conditions. Furthermore, there are other ICs, such as Hynix 8Gb DJR or Samsung 8Gb B-Die that have been observed dailying voltages well over 1.55V. Do your research about what voltages are safe on your IC, or stick to 1.35v or similar if this value is not known. Due to random chance and silicon variance, YMMV compared to other people, so be safe.
* One common limiting factor for the maximum safe voltage on which you can operate is your CPU's architecture. According to [JEDEC](https://www.jedec.org/standards-documents/dictionary/terms/output-stage-drain-power-voltage-vddq), VDDQ, the voltage of data output, is tied to VDD, colloquially referred to as VDIMM or DRAM Voltage. This voltage interacts with the PHY or Physical Layer present on the CPU and may lead to long-term degradation of the IMC if set too high. As a result, daily use of VDIMM voltages above 1.60V on Ryzen 3000 and 5000, 1.65V on Intel Consumer Lake-series Processors is not advisable. Err on the side of caution with this, as CPU degradation of the PHY is difficult to measure or notice until the issue becomes serious.
* It may be safe to daily 1.60V as there are kits on the [B550 Unify-X QVL](https://www.msi.com/Motherboard/support/MEG-B550-UNIFY-X#support-mem-20) rated for 1.60V. B-Die, 8Gb Rev. E, DJR, and 16Gb Rev. B *should* be fine at running 1.60V daily, though it's recommended to have active airflow. Higher voltages lead to higher temperatures and high temperatures can themselves lower the threshold for what voltages are considered safe.
  
### Ranking
* Below is how most of the common ICs rank in terms of frequency and timings.
  | Tier | ICs | Description |
  | :-:  | :-: | :--:        |
  | S | Samsung 8Gb B-Die | Best DDR4 IC for all-around performance |
  | A | Hynix 8Gb DJR, Micron 8Gb Rev. E<sup>1</sup>, Micron 16Gb Rev. B | Top Performing ICs. Known not to clock wall and generally scale with voltage. |
  | B | Hynix 8Gb CJR, Samsung 4Gb E-Die, Nanya 8Gb B-Die | High-end ICs with the ability to run high frequencies with good timings. |
  | C | Hynix 8Gb JJR, Hynix 16Gb MJR, Hynix 16Gb CJR, Micron 16Gb Rev. E, Samsung 8Gb D-Die | Decent ICs with good performance and decent frequency scaling.|
  | D | Hynix 8Gb AFR, Micron 8Gb Rev. B, Samsung 8Gb C-Die, Samsung 4Gb D-Die | Low-end ICs commonly found in average cheap kits. Most are EOL and no longer relevant. 
  | F | Hynix 8Gb MFR, Micron 4Gb Rev. A, Samsung 4Gb S-Die, Nanya 8Gb C-Die | Terrible ICs unable to reliably attain even the highest standard of the base JEDEC Specification.|
  * Partially based on [Buildzoid's older ranking](https://www.reddit.com/r/overclocking/comments/8cjla5/the_best_manufacturerdie_of_ddr_ram_in_order/dxfgd4x/). Some ICs are not included in this list due to the age of the post.
  * <sup>1</sup>Revisions of 8Gb Rev. E mainly differ in the minimum tRCD achievable and how high they can clock without modification of VTT while maintaining stability. Generally, newer revisions of 8Gb Rev. E (C9BKV, C9BLL, etc.) do tighter tRCD and clock higher without modification of VTT.
 
### Temperatures and Its Effect on Stability
* Generally, the hotter your RAM is the less stability it will have at higher frequencies and/or tighter timings.
* The tRFC timings are very dependent on temperatures, as they are related to capacitor leakage which is affected by temperature. Higher temperatures will need higher tRFC values. tRFC2 and tRFC4 are timings that activate when the operating temperature of DRAM hits 85°C. Below these temperatures, these timings don't do anything.
* B-Die is temperature sensitive and its ideal range is ~30-40°C. Some may be able to withstand higher temperatures so YMMV.
* Rev. E, on the other hand, doesn't seem to be as strongly temperature-sensitive, demonstrated by [buildzoid](https://www.youtube.com/watch?v=OeHEtULQg3Q).
* You might find that you're stable when running a memory test yet crash while gaming. This is because your CPU and/or GPU dump heat in the case, raising the RAM temperatures in the process. Thus, it is a good idea to stress test your GPU while running a memory test to simulate stability while gaming.
 
## Integrated Memory Controller (IMC)
### Intel - LGA1151
* Intel's Skylake IMC is pretty strong, so it shouldn't be the bottleneck when overclocking.  
  What would you expect from 14+++++?
* The Rocket Lake IMC, aside from the limitations regarding Gear 1 and Gear 2 memory support, has the strongest memory controller of all Intel consumer CPUs, and by a fair margin.
* There are 2 voltages you need to change if overclocking RAM: system agent (VCCSA) and IO (VCCIO).  
  **DO NOT** leave these on auto, as they can pump dangerous levels of voltage into your IMC, potentially degrading or even killing it. Most of the time you can keep VCCSA and VCCIO the same, but sometimes too much can harm stability (credits: Silent_Scone). I wouldn't recommend going above 1.25V on each.
  
  ![image](https://user-images.githubusercontent.com/69487009/156914787-b9eba0e9-69a6-4bd6-a5a1-f7d794e64f00.png)

  Below are my suggested VCCSA and VCCIO for 2 single rank DIMMs:

  | Effective Speed (MT/s) | VCCSA/VCCIO (V) |
  | :-------------: | :-------------: |
  | 3000 - 3600 | 1.10 - 1.15 |
  | 3600 - 4000 | 1.15 - 1.20 |
  | 4000 - 4200 | 1.20 - 1.25 |
  | 4200 - 4400 | 1.25 - 1.30 |
  * With more DIMMs and/or dual-rank DIMMs, you may need higher VCCSA and VCCIO than suggested.
* tRCD and tRP are linked, meaning if you set tRCD 16 but tRP 17, both will run at the higher timing (17). This limitation is why many ICs don't do as well on Intel and why B-die is a good match for Intel.
  * On Asrock and EVGA UEFIs, they're combined into tRCDtRP. On ASUS UEFIs, tRP is hidden. On MSI and Gigabyte UEFIs, tRCD and tRP are visible but setting them to different values just sets both of them to the higher value.
* Expected memory latency range: 40ns - 50ns.
   * Expected memory latency range for Samsung B-Die: 35ns - 45ns.
   * Overall, latency varies between generations due to a difference in die size (ring bus). As a result, a 9900K will have a slightly lower latency than a 10700K at the same settings since the 10700K has the same die as a 10900K.
   * Latency is affected by the RTLs and IOLs. Generally speaking, higher quality boards and overclocking oriented boards will be more direct in their routing of the memory traces and will likely have lower RTLs and IOLs. On some motherboards, changing RTLs and IOLs have no effect.
  
### AMD - AM4
Some terminology:
* MCLK: Real memory clock (half of the effective RAM speed). For example, for DDR4-3200 the MCLK is 1600MHz.
* FCLK: Infinity Fabric clock.
* UCLK: Unified memory controller clock. Half of MCLK when MCLK and FCLK are not equal (desynchronised, 2:1 mode).
* On Zen and Zen+, MCLK == FCLK == UCLK. However, on Zen2 and Zen3, you can specify FCLK. If MCLK is 1600MHz (DDR4-3200) and you set FCLK to 1600MHz, UCLK will also be 1600MHz unless you set MCLK:UCLK ratio to 2:1 (also known as UCLK DIV mode, etc.). However, if you set FCLK to 1800MHz, UCLK will run at 800MHz (desynchronised).

* Ryzen 1000 and 2000's IMC can be a bit finicky when overclocking and can't hit as high frequencies as Intel can. Ryzen 3000 and 5000's IMCs are much better and are more or less on par with Intel's newer Skylake based CPUs ie. 9th gen and 10th gen.
* SOC voltage is the voltage to the IMC and like with Intel, it's not recommended to leave it on auto. Typical ranges for this value range around 1.00V and 1.10V. Higher values are generally acceptable and may be necessary for stabilizing higher capacity memory and may aid in attaining FCLK stability.
* By contrast, when SOC voltage is set too high, memory instability can occur. This negative scaling typically occurs between 1.15V and 1.25V on most Ryzen CPUs.
  > There are clear differences in how the memory controller behaves on the different CPU specimens. The majority of the CPUs will do DDR4-3466 or higher at 1.050V SoC voltage, however, the difference lies in how the different specimens react to the voltage. Some of the specimens seem to scale with the increased SoC voltage, while the others simply refuse to scale at all or in some cases even illustrate negative scaling. All of the tested samples illustrated negative scaling (i.e. more errors or failures to train) when higher than 1.150V SoC was used. In all cases, the maximum memory frequency was achieved at =< 1.100V SoC voltage.  
  [~ The Stilt](https://forums.anandtech.com/threads/ryzen-strictly-technical.2500572/page-72#post-39391302)
* On Ryzen 3000, there's also CLDO_VDDG (commonly abbreviated to VDDG, not to be confused with CLDO_VDD**P**), which is the voltage to the Infinity Fabric. SOC voltage should be at least 40mV above CLDO_VDDG as CLDO_VDDG is derived from SOC voltage.
  > Most cLDO voltages are regulated from the two main power rails of the CPU. In the case of cLDO_VDDG and cLDO_VDDP, they are regulated from the VDDCR_SoC plane.
Because of this, there are a couple of rules. For example, if you set the VDDG to 1.100V, while your actual SoC voltage under load is 1.05V the VDDG will stay roughly at 1.01V max.
Likewise, if you have VDDG set to 1.100V and start increasing the SoC voltage, your VDDG will raise as well. I don't have the exact figure, but you can assume that the minimum drop-out voltage (Vin-Vout) is around 40mV.
Meaning your ACTUAL SoC voltage has to be at least by this much higher, than the requested VDDG for it to take effect as it is requested.  
Adjusting the SoC voltage alone, unlike on previous gen. parts doesn't do much if anything at all.
The default value is fixed at .1.100V and AMD recommends keeping it at that level. Increasing the VDDG helps with the fabric overclocking in certain scenarios, but not always.
1800MHz FCLK should be doable at the default 0.9500V value and for pushing the limits it might be beneficial to increase it to =< 1.05V (1.100 - 1.125V SoC, depending on the load-line).  
  [~ The Stilt](https://www.overclock.net/forum/28031966-post35.html)  
  * On AGESA 1.0.0.4 or newer VDDG is separated into VDDG IOD and VDDG CCD for the I/O die and the chiplets parts, respectively.

* Below are the expected memory frequency ranges for 2 single rank DIMMs, provided your motherboard and ICs are capable:

  | Ryzen | Expected Effective Speed (MT/s) |
  | :---: | :----------------------: |
  | 1000 | 3000 - 3600 |
  | 2000 | 3400 - 3800<sup>1</sup> |
  | 3000 | 3600 - 3800 (1:1 MCLK:FCLK) <br/> 3800+ (2:1 MCLK:FCLK) |
  * With more DIMMs and/or dual rank DIMMs, the expected frequency can be lower.
  * <sup>1</sup>3600+ is typically achieved on a 1 DIMM per channel (DPC)/2 DIMM slot motherboard and with a very good IMC.
    * See [here](https://docs.google.com/spreadsheets/d/1dsu9K1Nt_7apHBdiy0MWVPcYjf6nOlr9CtkkfN78tSo/edit#gid=1814864213).
  * <sup>1</sup>DDR4-3400 - DDR4-3533 is what most, if not all, Ryzen 2000 IMCs should be able to hit.
    > On the tested samples, the distribution of the maximum achievable memory frequency was following:  
    > DDR4-3400 – 12.5% of the samples   
    > DDR4-3466 – 25.0% of the samples  
    > DDR4-3533 – 62.5% of the samples  
    [~ The Stilt](https://forums.anandtech.com/threads/ryzen-strictly-technical.2500572/page-72#post-39391302)
  * 2 CCD Ryzen 3000 CPUs (3900X and 3950X) seem to prefer 4 single rank sticks over 2 dual rank sticks.
    > For 2 CCD SKUs, 2 DPC SR configuration seems to be the way to go.
    > Both the 3600 and 3700X did 1800MHz UCLK on 1 DPC DR config, but most likely due to the discrepancy of the two CCDs in 3900X, it barely does 1733MHz on those DIMMs.
    > Meanwhile with the 2 DPC SR config there is no issue in reaching 1866MHz FCLK/UCLK.  
[~ The Stilt](https://www.overclock.net/forum/10-amd-cpus/1728758-strictly-technical-matisse-not-really-26.html#post28052342)
* tRCD is split into tRCDRD (read) and tRCDWR (write). Usually, tRCDWR can go lower than tRCDRD, but I haven't noticed any performance improvements from lowering tRCDWR. It's best to keep them the same.
* Geardown mode (GDM) is automatically enabled above DDR4-2666, which forces even tCL, even tCWL, even tRTP, even tWR, and CR 1T. If you want to run odd tCL, disable GDM. If you're unstable try running CR 2T, but that may negate the performance gain from dropping tCL, and may even be less stable than GDM enabled.
  * For example, if you try to run DDR4-3000 CL15 with GDM enabled, CL will be rounded up to 16.
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
* On Ryzen 3000 and 5000, high enough FCLK can overcome the penalties from desynchronising MCLK and FCLK, provided that you can lock your UCLK to MCLK.
  
  ![Chart](https://i.imgur.com/F9HpkO2.png) 
  * (Credits: [Buildzoid](https://www.youtube.com/watch?v=10pYf9wqFFY))
  
# Overclocking
* **Disclaimer**: The silicon lottery will affect your overclocking potential so there may be some deviation from my suggestions.
* **Warning**: Data corruption is possible when overclocking RAM. It's advised to run `sfc /scannow` every so often to ensure any corrupted system files are fixed.
* The overclocking process is pretty simple and boils down to 3 steps:
  * Set very loose (high) timings.
  * Increase DRAM frequency until unstable.
  * Tighten (lower) timings.

## Finding a Baseline
1. * Ensure your sticks are in the recommended DIMM slots (usually 2 and 4).
   * Make sure your CPU overclock is disabled when tuning RAM, as an unstable CPU can lead to memory errors. When pushing high frequency with tight timings, your CPU may become unstable and may need to be re-done.
   * Make sure your UEFI/BIOS is up to date.

2. On Intel, start with 1.15V VCCSA and VCCIO.  
   On AMD, start with 1.10V SOC.
   * SOC voltage might be named differently depending on the manufacturer.
     * Asrock: CPU VDDCR_SOC Voltage. If you can't find that, you can use SOC Overclock VID hidden in the AMD CBS menu.
       * [VID values](https://www.reddit.com/r/Amd/comments/842ehb/asrock_ab350_pro4_guide_bios_overclocking_raven/).
     * Asus: VDDCR SOC.
     * Gigabyte: (Dynamic) Vcore SOC.
       * Note that dynamic Vcore SOC is an offset voltage. The base voltage can change automatically when increasing DRAM frequency. +0.100V at DDR4-3000 might result in 1.10V actual, but +0.100V at DDR4-3400 might result in 1.20v actual.
     * MSI: CPU NB/SOC.
3. Set DRAM voltage to 1.40V. If you're using ICs that roll over above 1.35V, set 1.35V.
   * "Roll over" means that the IC becomes more unstable as you increase the voltage, sometimes to the point of not even POSTing.
   * ICs that are known to roll over above 1.35V include but is not limited to: 8Gb Samsung C-die, older Micron/SpecTek ICs (before 8Gb Rev. E).
4. Set primary timings to 16-20-20-40 (tCL-tRCD-tRP-tRAS) and tCWL to 16.
   * Most ICs need loose tRCD and/or tRP which is why I recommend 20.
   * See [this post](https://redd.it/ahs5a2) for more information on these timings.
5. Increase the DRAM frequency until it doesn't boot into Windows anymore. Keep in mind the expectations detailed above.
   * If you're on Intel, a quick way of knowing if you're unstable is to examine the RTLs and IOLs. Each group of RTLs and IOLs correspond to a channel. Within each group, 2 values correspond to each DIMM.  
   Asrock Timing Configurator:
   
   ![image](https://user-images.githubusercontent.com/69487009/156914822-7443c639-1e4a-41d1-a282-5b0022af9154.png)

   As I have my sticks installed in channel A slot 2 and channel B slot 2, I need to look at D1 within each group of RTLs and IOLs.  
   RTLs should be no more than 2 apart and IOLs should be no more than 1 apart.  
   In my case, RTLs are 53 and 55 which are exactly 2 apart and IOLs are both 7.
   Note that having RTLs and IOLs within those ranges doesn't mean you're stable.
   * If you're on Ryzen 3000 or 5000, make sure that the Infinity Fabric frequency (FCLK) is set to half your effective DRAM frequency. Confirm this in ZenTimings by ensuring that FCLK matches up with UCLK and MCLK.
6. Run a memory tester of your choice.
   * Windows will use ~2000MB so make sure to account for that when entering the amount of RAM to test if the test has manual input. I have 16GB of RAM and usually test 14000MB.
   * Minimum recommended coverage/runtime:
     * **For AMD run Prime95 Large FFTs and OCCT VRAM with max utilization at the same time to stress the FCLK and to ensure FCLK stability. This should be run after any frequency/FCLK change.**
     * MemTestHelper (HCI MemTest): 200% per thread.
     * Karhu RAMTest: 5000%.
       * In the advanced tab, make sure CPU cache is set to enabled. This will speed up testing by ~20%.
       * Testing for 6400% coverage and a 1 hour duration has an error cover rate of 99,41% and 98,43%, respectively ([Source - FAQ section](https://www.karhusoftware.com/ramtest/)).
     * TM5 anta777 Extreme: 3 cycles.
       * Runtime varies with density. For 16GB RAM, it usually takes between 1.5-2 hours. If you run 32GB RAM you can set the 12th row of the config (Time (%)) to half and you'll get roughly the same runtime as 16GB.
     * OCCT Memory: 30 minutes each for SSE and AVX.
     * **You can run more tests such as other TM5 configs to ensure stability. It is recommended to run various tests for maximum error coverage.**
7. If you crash/freeze/BSOD or get an error, drop the DRAM frequency by a notch and test again.
9. Save your overclock profile in your UEFI.
10. From this point on you can either: try to go for a higher frequency or work on tightening the timings.
   * Keep in mind the expectations detailed above. If you're at the limit of your ICs and/or IMC it's best just to tighten the timings.
   
## Trying Higher Frequencies
* This section is applicable if you're not at the limit of your motherboard, ICs, and IMC.  
  This section is not for those who are having trouble stabilising frequencies within the expected range.
     * Note that some boards have auto rules that can stifle your progress, an example being tCWL = tCL - 1 which can lead to uneven values of tCWL. Reading the [Miscellaneous Tips](#miscellaneous-tips) might give you insight into your particular platform and your motherboard's functionality.
1. Intel:
   * Increase VCCSA and VCCIO to 1.25V. For ADL, VCCIO does not exist.
   * Set command rate (CR) to 2T if it isn't already.
   * Set tCCDL to 8. Asus UEFIs don't expose this timing.
   
   Ryzen 3000/5000:
   * Desynchronising MCLK and FCLK can incur a massive latency penalty, so you're better off tightening timings to keep your MCLK:FCLK 1:1. See [AMD - AM4](#amd---am4) for more information.
   * Otherwise, set FCLK to whatever is stable (1600MHz if you're unsure).
2. Loosen primary timings to 18-22-22-42 and set tCWL to 18.
3. Increase DRAM voltage to 1.45v if it is safe for your IC.
5. Follow steps 4-7 from [Finding a Baseline](#finding-a-baseline).
6. Proceed to [Tightening Timings](#tightening-timings).
   
## Tightening Timings
* Make sure to run a memory test and benchmark after each change to ensure performance is improving.
  * I would recommend to benchmark 3 to 5 times and averaging the results, as memory benchmarks can have a bit of variance.
  * Theoretical maximum bandwidth (MB/s) = `Transfers per clock * Actual Clock * Channel Count * Bus Width * Bit to Byte ratio`.
       * Transfers per clock refers to the number of data transfers that can occur in one full memory clock cycle. This occurs twice per cycle on DDR RAM, on the rising and falling clock edges.
       * Actual Clock is the real clock of the memory, simply measured in MHz. This is generally shown as the real memory frequency by programs such as CPU-Z.
       * Channel Count is the number of memory channels active on your CPU.
       * Bus Width is the width of each memory channel, measured in bits. Since DDR1, this is 64 bits.
       * Bit to Byte ratio is a constant 1/8, or 0.125
 
    | Effective Memory Speed (MT/s) | Max Dual Channel Bandwidth (MB/s) |
    | :-------------: | :------------------------: |
    | 3000 | 48000 |
    | 3200 | 51200 |
    | 3400 | 54440 |
    | 3466 | 55456 |
    | 3600 | 57600 |
    | 3733 | 59728 |
    | 3800 | 60800 |
    | 4000 | 64000 |
    
    * Your read and write bandwidth should be 90% - 98% of the theoretical maximum bandwidth.
      * On single CCD Ryzen 3000-5000 CPUs, write bandwidth should be 90% - 98% of half of the theoretical maximum bandwidth.  
        It is possible to hit half of the theoretical maximum write bandwidth. See [here](https://redd.it/cgc9bh).
      * Percentage of theoretically max bandwidth is inversely proportional to most memory timings. Generally speaking, as RAM timings are tightened, this value will increase.

1. I would recommend tightening some of the secondary timings first, as they can speed up memory testing.  
   My suggestions:
   
   | Timing | Safe | Tight | Extreme |
   | ------ | ---- | ----- | ------- |
   | tRRDS tRRDL tFAW | 6, 6, 24 | 4, 6, 16 | 4, 4, 16 |
   | tWR | 16 | 12 | 10 |
   * The minimum value for which lowering tFAW will affect the performance of RAM is `tRRDS * 4` or `tRRDL * 4`, whichever is lower.
   * You don't have to run all of the timings at one preset. You might only be able to run tRRDS tRRDL tFAW at the tight preset, but you may be able to run tWR at the extreme preset.
   * On some Intel motherboards, tWR in the UEFI does nothing and instead needs to be controlled through tWRPRE (sometimes tWRPDEN). Dropping tWRPRE by 1 will drop tWR by 1, following the rule tWR = tWRPRE - tCWL - 4.
     
2. Next is tRFC. Default for 8Gb ICs is 350**ns** (note the units).
   * Note: Tightening tRFC too much can result in system freezes/lock-ups.
   * tRFC is the number of cycles for which the DRAM capacitors are "recharged" or refreshed. Because capacitor charge loss is proportional to temperature, RAM operating at higher temperatures may need substantially higher tRFC values.
   * To convert to ns: `2000 * timing / ddr_freq`.  
   For example, tRFC 250 at DDR4-3200 is `2000 * 250 / 3200 = 156.25ns`.
   * To convert from ns (this is what you would type in your UEFI): `ns * ddr_freq / 2000`.  
   For example, 180ns at DDR4-3600 is `180 * 3600 / 2000 = 324`, so you would type 324 in your UEFI.
   * Below are the typical tRFC in ns for the common ICs:
   
     | IC | tRFC (ns) |
     | :-: | :-------: |
     | Hynix 8Gb AFR | 260 - 280 |
     | Hynix 8Gb CJR | 260 - 280 |
     | 8Gb DJR | 260 - 280 |
     | Micron 8Gb Rev. E | 280 - 310 |
     | Micron 16Gb Rev. B | 290 - 310 |
     | Samsung 8Gb B-Die | 120 - 180 |
     | Samsung 8Gb C-Die | 300 - 340 |
     
   * For all other ICs, I would recommend doing a binary search to find the lowest stable tRFC.  
   For example, say your tRFC is 630. The next tRFC you should try is half of that (315). If that is unstable, you know that your lowest tRFC is somewhere between 315 and 630, so you try the midpoint (`(315 + 630) / 2 = 472.5, round down to 472`). If that is stable, you know that your lowest tRFC is between 315 and 472, so you try the midpoint and so on.
   * [tRFC table by Reous](https://www.hardwareluxx.de/community/threads/hynix-8gbit-ddr4-cjr-c-die-h5an8g8ncjr-djr-2020-update.1206340/)(bottom of page).
3. Here are my suggestions for the rest of the secondaries:

   | Timing | Safe | Tight | Extreme |
   | :----: | :--: | :---: | :-----: |
   | tWTRS tWTRL | 4 12 | 4 10 | 4 8 |
   | tRTP | 12 | 10 | 8 |
   | tCWL<sup>1</sup> | tCL | tCL - 1 | tCL - 2 |
   * On Intel, tWTRS/L should be left on auto and controlled with tWRRD_dg/sg respectively. Dropping tWRRD_dg by 1 will drop tWTRS by 1. Likewise with tWRRD_sg. Once they're as low as you can go, manually set tWTRS/L.
   * On Intel, changing tCWL will affect tWRRD_dg/sg and thus tWTR_S/L. If you lower tCWL by 1 you need to lower tWRRD_dg/sg by 1 to keep the same tWTR values. Note that this might also affect tWR per the relationship described earlier.
   * <sup>1</sup>Some motherboards don't play nice with odd tCWL. For example, I'm stable at 4000 15-19-19 tCWL 14, yet tCWL 15 doesn't even POST. Another user has had similar experiences. Some motherboards may seem fine but have issues with it at higher frequencies (Asus). Manually setting tCWL equal to tCL if tCL is even or one below if tCL is uneven should alleviate this (eg. if tCL = 18 try tCWL = 18 or 16, if tCL = 17 try tCWL = 16).
   * The extreme preset is not the minimum floor in this case. tRTP can go as low as 5 (6 with Gear Down Mode on), while tWTRS/L can go as low as 1/6. Some boards are fine doing tCWL as low as tCL - 6. Keep in mind that this *will* increase the load on your memory controller.
   * On AMD, tCWL can often be set to tCL - 2 but is known to require higher tWRRD.
   
4. Now for the tertiaries:
    * If you're on AMD, refer to [this post](https://redd.it/ahs5a2).  
      My suggestion:
  
       | Timing | Safe | Tight | Extreme |
       | ------ | ---- | ----- | ------- |
       | tRDRDSCL tWRWRSCL | 4 4 | 3 3 | 2 2 |
     
        * A lot of ICs are known to have issues with low SCLs. Values such as 2 are extremely difficult for all but ICs such as Samsung 8Gb B-Die. These values are not necessarily linked, and values such as 5 are acceptable. Mixing and matching is possible, and more often than not tRDRDSCL will be the one that needs to be run 1 or even 2 values higher. Values above 5 greatly hurt bandwidth and so their use is not advised.
     
    * If you're on Intel, tune the tertiaries one group at a time.  
      My suggestions:
      
      | Timing | Safe | Tight | Extreme |
      | ------ | ---- | ----- | ------- |
      | tRDRD_sg/dg/dr/dd | 8/4/8/8 | 7/4/7/7 | 6/4/6/6 |
      | tWRWR_sg/dg/dr/dd | 8/4/8/8 | 7/4/7/7 | 6/4/6/6 |
      * For tWRRD_sg/dg, see step 3. For tWRRD_dr/dd, drop them both by 1 until you get instability or performance degrades.
      * For tRDWR_sg/dg/dr/dd, drop them all by 1 until you get instability or performance degrades. You can usually run them all the same e.g. 9/9/9/9.
        * Setting these too tight can cause system freezes.
      * Note that dr only affects dual rank sticks, so if you have single rank sticks you can ignore this timing. In the same way, dd only needs to be considered when you run two DIMMs per channel. You can also set them to 0 or 1 if you really wanted to.  
        These are my timings on B-die, for reference.
        
        ![image](https://user-images.githubusercontent.com/69487009/156914848-edfa7660-efc1-44ee-b8fb-56bb789fdb20.png)

      * For dual rank setups (see [notes on ranks](#a-note-on-ranks-and-density)):
         * tRDRD_dr/dd can be lowered a step further to 5 for a large bump in read bandwidth.
         * tWRWR_sg 6 can cause write bandwidth regression over tWRWR_sg 7, despite being stable.
    
5. Drop tCL by 1 until it's unstable.
   * On AMD, if GDM is enabled drop tCL by 2.   
 
6. On Intel, drop tRCD and tRP by 1 until unstable.  

   On AMD, drop tRCD by 1 until unstable. Repeat with tRP.
   * Note: More IMC voltage may be necessary to stabilise tighter tRCD.
   
7. Set `tRAS = tRCD(RD) + tRTP`. Increase if unstable.
   * This is the absolute minimum tRAS can be.  
   ![tRAS](https://user-images.githubusercontent.com/16512539/118769121-298a6000-b8c3-11eb-8793-7d90e885ca67.png)
   Here you can see that tRAS is the time between ACT and PRE commands.
     * ACT to READ = tRCD
     * READ to PRE = tRTP
     * Hence, tRAS = tRCD + tRTP.


8. Set `tRC = tRP + tRAS`. Increase if unstable.
   * tRC is only available on AMD and some Intel UEFIs.
   * On Intel UEFIs, tRC does seem to be affected by tRP and tRAS, even if it is hidden.
     * (1) [tRP 19 tRAS 42](https://i.imgur.com/gz1YDcO.png) - fully stable.
     * (2) [tRP 19 tRAS 36](https://i.imgur.com/lHjbLjC.png) - instant error.
     * (3) [tRP 25 tRAS 36](https://i.imgur.com/7c46Qes.png) - stable up to 500%.
     * In (1) and (3), tRC is 61 and isn't completely unstable. However, in (2) tRC is 55 and RAMTest finds an error instantly. This indicates that my RAM can do low tRAS, but not low tRC. Since tRC is hidden, I need higher tRAS to get higher tRC to ensure stability.

9. Increase tREFI until it's unstable. The binary search method explained in finding the lowest tRFC can also be applied here.  
   Otherwise, here are my suggestions:
   | Timing | Safe | Tight | Extreme |
   | ------ | ---- | ----- | ------- |
   | tREFI | 32768 | 40000 | Max (65535 or 65534) |
   * It's typically not a good idea to increase tREFI too much as ambient temperature changes (e.g. winter to summer) can be enough to cause instability.
   * Keep in mind that running max tREFI can corrupt files so tread with caution.

10. Finally onto command rate.

    AMD:
    * Getting GDM disabled and CR 1 stable can be pretty difficult but if you've come this far down the rabbit hole it's worth a shot.
    * If you can get GDM disabled and CR 1 stable without touching anything then you can skip this section.
    * CR 1 becomes significantly harder to run as the frequency increases. Oftentimes, running CR 2 can help with achieving higher frequencies.
    * On AMD, Gear Down Mode will override Command Rate. For this reason, disabling Gear Down Mode to set CR 2 may be beneficial to overall stability.
    
    1. One possibility is to set the drive strengths to 60-20-20-24 and setup times to 63-63-63.
       * Drive strengths are ClkDrvStr, AddrCmdDrvStr, CsOdtDrvStr and CkeDrvStr.
       * Setup times are AddrCmdSetup, CsOdtSetup and CkeSetup.
    2. If you can't POST, adjust the setup times until you can (you should adjust them all together).
    3. Run a memory test.
    4. Adjust setup times then drive strengths if unstable.
    * My stable GDM off CR 1 settings

      ![image](https://user-images.githubusercontent.com/69487009/156914875-ce4d1d15-edf1-464f-be7a-62315ed20a1d.png)

    5. Oftentimes, a drive strength above 24 ohms may hurt stability. Furthermore, running non-zero setup times is rarely needed, however may aid in the stabilization of CR 1.
   
    Intel:
    * If below DDR4-4400, try setting CR to 1T. If that doesn't work, leave CR on 2T.
    * On Asus Maximus boards enabling Trace Centering can help greatly with pushing CR 1T to higher frequencies.

11. You can also increase DRAM voltage to drop timings even more. Keep in mind the [voltage scaling characteristics of your ICs](#voltage-scaling) and the [maximum recommended daily voltage](#maximum-recommended-daily-voltage).
    
## Miscellaneous Tips
* Usually a 200MHz increase in effective DRAM frequency negates the latency penalty of loosening tCL, tRCD, and tRP by 1, but has the benefit of higher bandwidth.  
  For example, DDR4-3000 15-17-17 has the same latency as DDR4-3200 16-18-18, but DDR4-3200 16-18-18 has higher bandwidth. This is typically after initial tuning has been completed, and not at XMP.
* Generally speaking, frequency should be prioritized over tighter timings, so long as performance is not negatively impacted by FCLK sync, Command Rate, or Memory Gear mode.
* Secondary and tertiary timings (except for tRFC) don't change much, if at all, across the frequency range. If you have stable secondary and tertiary timings at DDR4-3200, you could probably run them at DDR4-3600, even DDR4-4000, provided your ICs, IMC, and motherboard are capable.

### Intel
* Loosening tCCDL to 8 may help with stability, especially above DDR4-3600. This does not bring a significant penalty to latency but may affect memory read and write bandwidth considerably.
* Higher cache (aka uncore, ring) frequency can increase bandwidth and reduce latency.
* After you've finished tightening the timings, you can increase IOL offsets to reduce IOLs. Make sure to run a memory test after. More info [here](https://hwbot.org/newsflash/3058_advanced_skylake_overclocking_tune_ddr4_memory_rtlio_on_maximus_viii_with_alexaros_guide).
  * In general, RTL and IOL values impact memory performance. Lowering them will increase bandwidth and decrease latency quite significantly.
  
    ![image](https://user-images.githubusercontent.com/69487009/156914900-da840c47-c4bb-4cad-8a0e-227e4e482024.png)

  * Lower values will in some cases also help with stability and lower memory controller voltage requirements. Some boards train them very well on their own. Some boards allow for easy tuning while other boards simply ignore any user input.
  * If all else fails, you can try manually decreasing the RTL and IOL pair.
* For Asus Maximus boards:
   * Play around with the Maximus Tweak Modes, sometimes one will post where the other does not.
   * You can enable Round Trip Latency under Memory Training Algorithms to let the board attempt to train RTL and IOL values.
   * If you can't boot, you can try tweaking the skew control values.  
     More info [here](https://rog.asus.com/forum/showthread.php?47670-Maximus-7-Gene-The-road-to-overclocking-memory-without-increasing-voltage).
* tXP (and subsequently PPD) has a major impact on AIDA64 memory latency.
* RTT Wr, Park, and Nom can have a massive impact on overclocking. The ideal values may depend on your board, your memory IC and density. The "optimal" values will let you clock higher with less memory controller voltage. Some boards reveal the auto values (MSI) while others don't (Asus). Finding the perfect combination is time-consuming but very helpful for advanced tuning.
* On some motherboards, enabling XMP can allow for better overclocking.
  * Thanks to Bored and Muren for finding and verifying this on their Asrock motherboards.

### AMD
* Try playing around with ProcODT if you can't boot. This setting determines the processor on-die termination impedance. According to [Micron](https://www.micron.com/support/~/media/D546161C2C6140BCB0BAEE954AA53433.pdf), higher settings of ProcODT can lead to more stable RAM but trade-off potentially needing higher voltages. On Ryzen 1000 and 2000, you should try values between 40Ω and 68.6Ω due to the considerably weaker memory controller. 
On Ryzen 3000 and 5000, [1usmus](https://www.overclock.net/forum/13-amd-general/1640919-new-dram-calculator-ryzena-1-5-1-overclocking-dram-am4-membench-0-7-dram-bench-480.html#post28049664) suggests 28Ω - 40Ω. Lower settings may be harder to run but potentially helps with voltage requirements. Higher values may aid with stability, though according to [Micron](https://media-www.micron.com/-/media/client/global/documents/products/technical-note/dram/tn4040_ddr4_point_to_point_design_guide.pdf?la=en&rev=d58bc222192d411aae066b2577a12677), values of ODT above 60Ω are only suitable for extremely weak memory controllers and lower power solutions.
This seems to line up with [The Stilt's](https://www.overclock.net/forum/10-amd-cpus/1728758-strictly-technical-matisse-not-really-26.html) settings.
  > Phy at AGESA defaults, except ProcODT of 40.0Ohm, which is an ASUS auto-rule for Optimem III.
* Lower SOC voltage and/or VDDG IOD may help with stability.
* On Ryzen 3000 and 5000, higher CLDO_VDDP can help with stability above DDR4-3600.
  > Increasing cLDO_VDDP seems beneficial > 3600MHz MEMCLKs, as increasing it seems to improve the margins and hence help with potential training issues. Source: [The Stilt](https://www.overclock.net/forum/10-amd-cpus/1728758-strictly-technical-matisse-not-really-26.html).
 
  > This value is not to exceed 1.10V on Ryzen 3000 and 5000, and should always be restricted to at least 0.10V less than DRAM Voltage. Source: [AMD](https://community.amd.com/t5/blogs/community-update-4-let-s-talk-dram/ba-p/415902)
* When pushing FCLK around 1800MHz intermittent RAM training errors may be alleviated or eliminated by increasing VDDG CCD.

# Useful Links
## Benchmarks
* [Impact of RAM on Intel's Skylake desktop architecture by KingFaris](https://kingfaris.co.uk/ram)
* [RAM timings and their influence on games and applications (AMD) by Reous](https://www.hardwareluxx.de/community/threads/ram-timings-und-deren-einfluss-auf-spiele-und-anwendungen-amd-update-23-05-2020.1269156/)
## Information
* [r/overclocking Wiki - DDR4](https://www.reddit.com/r/overclocking/wiki/ram/ddr4)
* [Demystifying Memory Overclocking on Ryzen: OC Guidelines and Explaining Subtimings, Resistances, Voltages, and More! by varexos717](https://redd.it/ahs5a2)
* [DDR4 OC Rankings](https://docs.google.com/spreadsheets/d/1NN3-m-mvqxoebSUO_22efkMrnGog2y1atWUgiZSZTOc/edit?usp=sharing)
* [HardwareLUXX Ryzen RAM OC Thread](https://www.hardwareluxx.de/community/f13/ryzen-ram-oc-thread-moegliche-limitierungen-1216557.html)
* [Ryzen 3000 Memory / Fabric (X370/X470/X570) by elmor](https://www.overclock.net/forum/13-amd-general/1728878-ryzen-3000-memory-fabric-x370-x470-x570.html)
* [Intel Memory Overclocking Quick Reference by sdch](https://www.overclock.net/forum/27784556-post7836.html)
* [The road to overclocking memory without increasing voltage by Raja@ASUS](https://rog.asus.com/forum/showthread.php?47670-Maximus-7-Gene-The-road-to-overclocking-memory-without-increasing-voltage)
* [Advanced Skylake Overclocking: Tune DDR4 Memory RTL/IO on Maximus VIII with Alex@ro's Guide](https://hwbot.org/newsflash/3058_advanced_skylake_overclocking_tune_ddr4_memory_rtlio_on_maximus_viii_with_alexaros_guide)
* [BSOD codes when OC'ing and possible actions](https://www.reddit.com/r/overclocking/comments/atwtt5/psa_bsod_codes_when_ocing_and_possible_actions/)
