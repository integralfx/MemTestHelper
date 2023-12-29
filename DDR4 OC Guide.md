# Table of Contents
- [Table of Contents](#table-of-contents)
- [Setup](#setup)
  - [Memory Testing Software](#memory-testing-software)
    - [Avoid](#avoid)
    - [Recommended](#recommended)
    - [Alternatives](#alternatives)
    - [Comparison](#comparison)
  - [Timings Software](#timings-software)
  - [Benchmarks](#benchmarks)
- [General RAM Info](#general-ram-info)
  - [Frequency and Timings Relation](#frequency-and-timings-relation)
  - [Primary, Secondary and Tertiary Timings](#primary-secondary-and-tertiary-timings)
- [Expectations/Limitations](#expectationslimitations)
  - [Motherboard](#motherboard)
  - [Integrated Circuits (ICs)](#integrated-circuits-ics)
    - [Shorthand Notation](#shorthand-notation)
    - [Label on Sticks](#label-on-sticks)
      - [Corsair Version Number](#corsair-version-number)
      - [G.Skill 042 Code](#gskill-042-code)
      - [Kingston Code](#kingston-code)
    - [A Note on Logical Ranks and Density](#a-note-on-logical-ranks-and-density)
    - [Voltage Scaling](#voltage-scaling)
    - [Expected Max Frequency](#expected-max-frequency)
    - [Binning](#binning)
    - [Maximum Recommended Daily Voltage](#maximum-recommended-daily-voltage)
    - [Ranking](#ranking)
    - [Temperatures and Its Effect on Stability](#temperatures-and-its-effect-on-stability)
  - [Integrated Memory Controller (IMC)](#integrated-memory-controller-imc)
    - [Intel IMC](#intel-imc)
    - [AMD IMC](#amd-imc)
- [Overclocking](#overclocking)
  - [Miscellaneous Tips](#miscellaneous-tips)
    - [Intel](#intel)
    - [AMD](#amd)
  - [Finding a Baseline](#finding-a-baseline)
  - [Tightening Timings](#tightening-timings)
- [Useful Links](#useful-links)
  - [Benchmarks](#benchmarks-1)
  - [Information](#information)

# Setup
## Memory Testing Software
You should always test with various stress tests to ensure your overclock is stable.
### Avoid
* I wouldn't recommend the AIDA64 memory test and [Memtest64](https://forums.anandtech.com/threads/techpowerups-memtest-64-is-it-better-than-hci-memtest-for-determining-stability.2532209/) as they are both not very good at finding memory errors.
### Recommended
* [TM5](https://mega.nz/file/vLhxBahB#WwJIpN3mQOaq_XsJUboSIcaMg3RlVBWvFnVspgJpcLY) with any of the configs listed:
  * [Extreme by anta777](TM5-Configs/extreme@anta777.cfg) (recommended). Make sure to load the config. It should say 'Customize: Extreme1 @anta777' if loaded.  
  Credits: [u/nucl3arlion](https://www.reddit.com/r/overclocking/comments/dlghvs/micron_reve_high_training_voltage_requirement/f4zcs04/).
  * [Absolut](TM5-Configs/absolutnew.cfg)
  * [PCBdestroyer](TM5-Configs/PCBdestroyer.cfg)
  * [LMHz Universal 2](TM5-Configs/Universal-2@LMhz.cfg)
  * If you experience issues with all threads crashing upon launch with the extreme config, it might help to edit the row "Testing Window Size (Mb)=1408". Replace the window size with your total RAM (minus some margin for Windows) divided by your processor's available threads (e.g., 12800/16 = 800 MB per thread).
* [OCCT](https://www.ocbase.com/) with the dedicated memory test using SSE or AVX instructions.
  * Note that AVX and SSE can vary in error detection speed. On Intel-based systems, SSE appears better for testing IMC voltages, while AVX appears better for DRAM voltage.
  * The Large AVX2 CPU test is a great stability test for your CPU and RAM at the same time. The more you tune your ram, the harder it'll be to be stable in this test. Be sure to run Normal Mode as Extreme will not use as much RAM.
  * The VRAM test at maximum utilization in conjunction with Prime95 Large FFTs will stress FCLK and is recommended when testing FCLK stability.
### Alternatives
* [GSAT](https://github.com/stressapptest/stressapptest).
  1. [Install WSL](https://docs.microsoft.com/en-us/windows/wsl/install-win10) and [Ubuntu](https://www.microsoft.com/en-us/p/ubuntu/9nblggh4msv6?activetab=pivot:overviewtab).
  2. Open an Ubuntu bash shell and type `sudo apt update`.
  3. Type `sudo apt-get install stressapptest`.
  4. To start testing, type `stressapptest -M 13000 -s 3600 -W --pause_delay 3600`.
     * `-M` is the amount of memory to test (MB).
     * `-s` is how long to test for (seconds).
     * `--pause_delay` is the delay between power spikes. It should be the same as the `-s` argument to skip the power spikes test.
* [Karhu RAM Test](https://www.karhusoftware.com/ramtest/) (paid).
* [y-cruncher](http://www.numberworld.org/y-cruncher/) with [this config](https://pastebin.com/dJQgFtDH).
  * Paste this in a new file called `memtest.cfg` in the same folder as `y-cruncher.exe`.
  * Adjust the following fields if necessary:
    * `LogicalCores`: CPU threads, e.g. `[0 1 2 3 4 5]` on 6C**6**T or `[0 1 2 3 4 5 6 7]` on 4C**8**T
    * `TotalMemory`: Memory (bytes) used by y-cruncher
  * Create a shortcut to `y-cruncher.exe` and add `pause:1 config memtest.cfg` to the target field.
    Your target field should look something like this: `"path\to\y-cruncher\y-cruncher.exe" pause:1 config memtest.cfg`
  * Credits: [u/Nerdsinc](https://www.reddit.com/r/overclocking/comments/iyp1n7/ycruncher_is_a_really_effective_tool_for_testing/)
* [Prime95](https://www.mersenne.org/download/) large FFTs is also decent at finding memory errors.
  * I've been using a custom FFT range of 800K - 800K, though I think any FFT value inside the large FFTs range should work.
    * Make sure 'Run FFTs in place' is not checked.
    * In `prime.txt`, add `TortureAlternateInPlace=0` under `TortureWeak` to prevent P95 from testing in place. Testing in place will only use a bit of RAM, which we don't want.
  * You can create a shortcut to `prime95.exe` and add `-t` to the 'Properties > Target' field to immediately start testing using the settings in `prime.txt`.  
    Your target field should look like: `"path\to\prime95\prime95.exe" -t`.
  * You can also change the working directory of Prime95's config files to have one config to stress test your CPU and another config to stress test your RAM.
    1. In the folder with `prime95.exe`, create another folder. This example will be called 'RAM' (without the quotes).
    2. Copy `prime.txt` and `local.txt` to the folder you just created.
    3. Adjust the settings in `prime.txt` as required.
    4. Create another shortcut to `prime95.exe`, and in the target field, add `-t -W<folder_name>`.  
       Your target field should look like: `"path\to\prime95\prime95.exe" -t -WRAM`.
    5. You can now use the shortcut to start Prime95 with the settings provided.
* [randomx-stress](https://github.com/00-matt/randomx-stress/releases) - Can be used to test FCLK stability.
### Comparison
[Comparison](https://imgur.com/a/jhrFGhg) between Karhu RAMTest, TM5 with the extreme config, and GSAT.
  * TM5 is the fastest and most stressful by quite a margin, though I have had instances where I would pass 30 mins of TM5 but fail within 10 mins of Karhu. Another user had a similar experience. YMMV.
    
## Timings Software
* To view timings in Windows: 
  * Intel: 
    * X99: [Asrock Timing Configurator v3.0.6](https://www.asrock.com/mb/Intel/X99%20OC%20Formula3.1/#Download).
    * Z370(?)/Z390: [Asrock Timing Configurator v4.0.4](https://www.asrock.com/MB/Intel/X299%20OC%20Formula/index.asp#Download) (don't need an Asrock motherboard).
    * EVGA motherboards and Z170/Z270(?)/Z490: [Asrock Timing Configurator v4.0.3](https://www.asrock.com/mb/Intel/Z170%20OC%20Formula/#Download).
    * For Rocket Lake: [ASRock Timing Configurator v4.0.10](https://web.archive.org/web/20211010085116/http://picx.xfastest.com/nickshih/asrock/AsrTCSetup(v4.0.10).rar).
    * For Alder Lake: [ASRock Timing Configuator v4.0.14](https://picx.xfastest.com/nickshih/asrock/AsrTCSetup(v4.0.14).rar) or [MSI Dragon Ball](https://drive.google.com/file/d/1XmKv13D0MgC9fPaA91535wCe9ztoeaHV/view?usp=sharing).
  * AMD: 
    * [ZenTimings](https://zentimings.protonrom.com/).
    
## Benchmarks

* [AIDA64](https://www.aida64.com/downloads) - free 30 day trial. We'll be using the cache and memory benchmark (found under tools) to see how our memory performs. You can right-click the start benchmark button and run memory tests only to skip the cache tests.
* [Intel Memory Latency Checker](https://software.intel.com/content/www/us/en/develop/articles/intelr-memory-latency-checker.html) - contains a lot of useful tests for measuring memory performance. More extensive data than AIDA64 and bandwidth numbers differ between the tests. Note that it must be run as an administrator to disable prefetching. On AMD systems, you may have to disable it in BIOS.
* [Intel MLC GUI](https://github.com/FarisR99/IMLCGui) - GUI for Intel Memory Latency Checker made by Faris.
* [xmrig](https://github.com/xmrig/xmrig) is very memory sensitive, so it's helpful to test the effects of specific timings. First, run as admin with `--bench=1M` as a command-line argument to start the benchmark. Then, use the benchmark time to compare.
* [MaxxMEM2](https://www.softpedia.com/get/System/Benchmarks/MaxxMEM2.shtml) - free alternative to AIDA64, but bandwidth tests seem to be a lot lower, so it isn't directly comparable to AIDA64.
* [Super Pi Mod v1.5 XS](https://www.techpowerup.com/download/super-pi/) - another memory-sensitive benchmark, but I haven't used it as much as AIDA64. 1M - 8M digits should be enough for a quick benchmark. You only need to look at the last (total) time, where lower is better.
* [HWBOT x265 Benchmark](https://hwbot.org/benchmark/hwbot_x265_benchmark_-_1080p/) - I've heard that this benchmark is also sensitive to memory, but I haven't really tested it myself.
* [PYPrime 2.x](https://github.com/monabuntur/PYPrime-2.x) - This benchmark is quick and scales very well with CPU core clock, cache/FCLK, memory frequency, and timings

# General RAM Info
## Frequency and Timings Relation
* RAM frequency is measured in megahertz (MHz) or million cycles per second. Higher frequency means more cycles per second, which means better performance.
* Esoteric note: People often refer to DDR4-3200 as being 3200 **MHz** however, in reality, the real frequency of the RAM is only 1600 MHz. As data is transferred on both the rising clock edge and falling clock edge in DDR (Double Data Rate) memory, the real frequency of the RAM is half of the number of transfers it makes per second. DDR4-3200 transfers 3200 million bits per second, and so, 3200 **MT/s** (MegaTransfers per second) is operating at a frequency of 1600 **MHz**.
* RAM timings are measured in clock cycles or ticks. Lower timings mean fewer cycles to perform an operation, which means better performance.
  * The exception to this is tREFI, which is the refresh interval. As its name suggests, tREFI is the time between refreshes. While the RAM is refreshing, it can't do anything, so you'd want to refresh as infrequently as possible. To do that, you'd want the time between refreshes to be as long as possible. This means you'd want tREFI as high as possible.
* While lower timings may be better, this also depends on the RAM's frequency. For example, DDR4-3000 CL15 and DDR4-3200 CL16 have the same latency, despite DDR4-3000 running at a lower absolute CL. This is because the higher frequency offsets the increase in CL.
* To calculate the actual time in nanoseconds (ns) of a given timing: `2000 * timing / ddr_freq`.
  * For example, CL15 at DDR4-3000 is `2000 * 15 / 3000 = 10 ns`.
  * Similarly, CL16 at DDR4-3200 is `2000 * 16 / 3200 = 10 ns`.

## Primary, Secondary and Tertiary Timings
* Intel

  ![](Images/intel-primary-secondary-tertiary.png)

* AMD

  ![](Images/amd-primary-secondary-tertiary.png)

* RAM timings are split into 3 categories: primary, secondary, and tertiary. These are indicated by 'P', 'S', and 'T', respectively.
  * Primary and secondary timings affect latency and bandwidth.
  * Tertiary timings affect bandwidth.
    * The exception is tREFI/tREF, which affects latency and bandwidth, though it isn't modifiable on AMD.

# Expectations/Limitations
* This section goes through 3 components that may influence your overclocking experience: ICs, motherboard, and IMC.

## Motherboard
* Motherboards with 2 DIMM slots will be able to achieve the highest frequencies.
* For motherboards with 4 DIMM slots, the number of sticks installed will affect your maximum memory frequency. 
  * On motherboards that use a daisy chain [memory trace layout](https://www.youtube.com/watch?v=3vQwGGbW1AE), 2 sticks are preferred. Using 4 sticks may significantly impact your maximum memory frequency.
  * On the other hand, motherboards that use T-topology will overclock the best with 4 sticks. Using 2 sticks won't impact your maximum memory frequency as much as using 4 sticks on a daisy chain motherboard (?).
  * There are some T-topology motherboards that overclock as well or better with just one DIMM per channel.
  * Most vendors don't advertise their memory trace layout, but you can make an educated guess based on the QVL. For example, the Z390 Aorus Master uses a T-Topology layout as its highest validated frequency is with 4 DIMMs. However, if the highest validated frequency were done with 2 DIMMs, it *probably* uses a daisy chain layout.
  * According to Buildzoid, Daisy Chain vs. T-Topology only matters above DDR4-4000. Following Buildzoid's logic, if you're on Ryzen 3000 or 5000, this doesn't matter as DDR4-3800 is the typical max memory frequency when running MCLK:FCLK 1:1.
* Lower end motherboards may not overclock as well, possibly due to the lower PCB quality and the number of layers (?).
  
## Integrated Circuits (ICs)
* Knowing what ICs (sometimes referred to as "dies") are in your RAM will give you an idea of what to expect. Even if you don't know them, you can still overclock your RAM.

### Shorthand Notation

To make it easier to type ICs quickly, a shorthand notation will be used.

XYZ where:
* X is the first letter of the manufacturer (S for Samsung, H for Hynix, M for Micron, N for Nanya, etc.).
* Y is the density (8 for 8 Gb, 16 for 16 Gb).
* Z is the die revision.

For example, the shorthand for Samsung 8 Gb B-die is S8B.

### Label on Sticks

Using the label on the sticks is the most accurate way to identify the IC. However, currently only Corsair, G.Skill, and Kingston labels have been decoded.

See [HardwareLuxx](https://www.hardwareluxx.de/community/threads/ryzen-ram-oc-m%C3%B6gliche-limitierungen.1216557/) for a neat infographic that summarises the following information.

* [SpecTek](https://www.micron.com/support/spectek-support) ICs are lower binned Micron ICs.
* Esoteric note: Many people have started calling this Micron E-die or E-die. The former is fine, but the latter can cause confusion as letter-die is typically used for Samsung ICs, i.e., Samsung 4 Gb E-die. Samsung is implied when you say E-die, but as people are calling Micron Rev. E E-die, it'd probably be a good idea to prefix the manufacturer.

#### Corsair Version Number
* Corsair has a 3 digit version number on the sticks' label, indicating what ICs are on the stick.
* The first digit is the manufacturer.
  * 3 = Micron
  * 4 = Samsung
  * 5 = Hynix
  * 8 = Nanya
* The second digit is the density.
  * 1 = 2 Gb
  * 2 = 4 Gb
  * 3 = 8 Gb
  * 4 = 16 Gb
* The last digit is the revision.
* See the [r/overclocking wiki](https://www.reddit.com/r/overclocking/wiki/ram/ddr4#wiki_corsair) for a full list.
#### G.Skill 042 Code
* Similar to Corsair, G.Skill uses a 042 code to indicate the ICs.
* Example: 04213X**8**8**1**0B
  * The first bolded character is the density. 4 for 4 Gb, 8 for 8 Gb, and S for 16 Gb.
  * The second bolded number is the manufacturer. 1 for Samsung, 2 for Hynix, 3 for Micron, 4 for PSC (powerchip), 5 for Nanya, and 9 for JHICC.
  * The last character is the revision.
  * This is the code for Samsung 8 Gb B-die.
* See the [r/overclocking wiki](https://www.reddit.com/r/overclocking/wiki/ram/ddr4#wiki_g.skill_sn_table) for a full list.
#### Kingston Code
* Example: DPM**M**16A1823
  * The bolded letter indicates the manufacturer. H for Hynix, M for Micron, and S for Samsung.
  * The next 2 digits indicate ranks. 08 = single rank and 16 = dual rank.
  * The following letter indicates the production month. 1-9, A, B, C.
  * The next 2 digits indicate the production year.
  * This is the code for dual-rank Micron produced in October 2018.
* [Source](http://www.xtremesystems.org/forums/showthread.php?285750-Interesting-memory-deals-thread&p=5230258&viewfull=1#post5230258)

### A Note on Logical Ranks and Density
* Single rank sticks usually clock higher than dual-rank sticks, but depending on the benchmark, the performance gain from rank interleaving<sup>1</sup> can be significant enough to outperform faster single-rank sticks. [This can be observed in synthetics and games](https://kingfaris.co.uk/ram).
   * On recent platforms (Comet Lake and Zen3), BIOS and memory controller support for dual-rank has significantly improved. On many Z490 boards, dual rank Samsung 8 Gb B-die (2x16 GB) will clock just as high as single-rank B-die, meaning you have all the performance gains of rank interleaving with little to no downsides.
   * <sup>1</sup>Rank interleaving allows the memory controller to parallelize memory requests, for example writing on one rank while the other is refreshing. The impact of this is easily observed in AIDA64 copy bandwidth. From the eyes of the memory controller, it doesn't matter whether the second rank is on the same DIMM (two ranks on one DIMM) or a different DIMM (two DIMM in one channel). It does, however, matter from an overclocking perspective when you consider memory trace layouts and BIOS support.
   * Having a second rank of the same IC also means twice as many bank groups are available. That means that short timings - such as RRD_S rather than RRD_L - can be used more often, as it's more likely for there to be a fresh bank group available. The long timing (L) is required when operating on the same bank group twice in a row, and when there are 7 other bank groups instead of 3 you have a lot more choices to avoid doing that.
   * It also means that there are twice as many banks, and thus twice as many memory rows can be open at any given time. As a result, it's more likely that the row you need will be open.
You won't have to close row A, open row B, and then close B to open A again as often.
You're held up by operations like RAS/RC/RCD (when waiting for a row to open because it was closed) and RP (when waiting for a row to close to open another one) less often.
   * x16 configurations will have half as many banks and bank groups as the traditional x8 configurations, which means less performance. See [buildzoid's video](https://www.youtube.com/watch?v=k6SIdxq2yxE) for more information.
* Density matters when determining how far your ICs can go. For example, 4 Gb AFR and 8 Gb AFR will not overclock the same despite sharing the same name. The same can be said for Micron Rev. B, which exists as 8 Gb and 16 Gb. The 16 Gb ICs overclock better and are sold in 16 GB and 8 GB capacities despite the DIMMs using 8 chips. The 8 GB sticks have their SPD modified and can be found in higher-end Crucial kits (BLM2K8G51C19U4B).
* As the total count of ranks in a system increases, so does the load on the memory controller. This usually means that more memory ranks will require higher voltage, especially VCCSA on Intel and SOC voltage on AMD.

### Voltage Scaling
* Voltage scaling simply means how the IC responds to voltage.
* On many ICs, tCL scales with voltage, meaning giving it more voltage can allow you to drop tCL. Conversely, tRCD and/or tRP typically do not scale with voltage on many ICs, meaning no matter how much voltage you pump into it, it will not budge.  
As far as I know, tCL, tRCD, tRP, and possibly tRFC can (or can not) see voltage scaling.
* Similarly, if a timing scales with voltage, you can increase the voltage to run the same timing at a higher frequency.
![CL11 Voltage Scaling](Images/cl-voltage-scaling.png)
  * You can see that tCL scales almost linearly up to DDR4-2533 with voltage on H8C.
  * tCL on S8B has perfect linear scaling with voltage.
  * tCL on M8E also has perfect linear scaling with voltage.
  * I've adapted this data into a [calculator](https://www.desmos.com/calculator/psisrpx3oh). Change the *f* and *v* sliders to the frequency and voltage you want, and it will output the frequencies and voltages achievable for a given CL (assuming that CL scales linearly up to 1.50 V). For example, DDR4-3200 CL14 at 1.35 V should do ~DDR4-3333 CL14 at 1.40 V, ~DDR4-3533 CL14 at 1.45 V, and DDR4-3733 CL14 at 1.50 V.

* B-die tRFC Voltage Scaling
![B-die tRFC Voltage Scaling](Images/b-die-trfc-voltage-scaling.png)
  * Here you can see that tRFC scales pretty well on B-die.

* Some older Micron ICs (before M8E) are known to scale negatively with voltage. That is, they become unstable at the same frequency and timings just by increasing the voltage (usually above 1.35 V).
* Here is a table of ICs I have tested and if the timing scales with voltage:

  | IC  | tCL | tRCD | tRP | tRFC |
  | :-: | :-: | :--: | :-: | :--: |
  | S8B | Y | Y | Y | Y |
  | H8C, H8D | Y | N | N | Y |
  | H8A | Y | N | N | ? |
  | M8B, M8E, M16B, N8B, S4E, S8D | Y | N | N | N |
  * The timings that don't scale with voltage usually need to be increased as you increase frequency.
  
### Expected Max Frequency
* Below are the expected max frequency for some of the common ICs:

  | IC  | Expected Max Effective Speed (MT/s) |
  | :-: | :-: |
  | H8D, M8E, M16B, S8B, S8D | 5000+ |
  | N8B, S4E | 4000+ |
  | H8C | 4133<sup>1</sup> |
  | H8A, M8B | 3600 |
  * <sup>1</sup>H8C is a bit inconsistent in my testing. I've tested 3 RipJaws V 3600 CL19 8 GB sticks. One was stuck at DDR4-3600, another at DDR4-3800, but the last could do DDR4-4000, all at CL16 with 1.45 V.
  * Don't expect lower binned ICs to overclock nearly as well as higher binned ICs. This is especially true for [B-die](https://www.youtube.com/watch?v=rmrap-Jrfww).
  * These values are simply referring to the IC's average capabilities; however, other factors, such as the motherboard and CPU, substantially impact whether or not said values are attainable.
  
### Binning
* Binning is basically grading components based on their performance characteristics.  
  Manufacturers would separate ICs into different containers/bins depending on their frequency. Hence the term binning.
* G.Skill is one manufacturer known for extensive binning and categorization. Several SKUs of G.Skill memory will often belong to the same factory bin (i.e., DDR4-3600 16-16-16-36 1.35 V bin of B-Die being binned the same as DDR4-3200 14-14-14-34 1.35 V B-Die).
* B-die binned for 2400 15-15-15 is significantly worse than good B-die binned for DDR4-3200 14-14-14 or even DDR4-3000 14-14-14. So don't expect it to have the same voltage scaling characteristics as good B-Die.
* To figure out which frequency and timings are a better (tighter) bin within the same IC at the same voltage, find out which timing doesn't scale from voltage.  
  Simply divide the frequency by that timing, and the higher value is the tighter bin.
  * For example, Crucial Ballistix DDR4-3000 15-16-16 and DDR4-3200 16-18-18 both use Micron Rev. E ICs. Simply dividing the frequency by tCL gives us the same value (200), so does that mean they're the same bin?  
  No.  
  tRCD doesn't scale with voltage, which means it needs to be increased as you increase frequency.  
  `3000 / 16 = 187.5` but `3200 / 18 = 177.78`.  
  As you can see, DDR4-3000 15-16-16 is a tighter bin than DDR4-3200 16-18-18. This means that a kit rated for DDR4-3000 15-16-16 will probably be able to do DDR4-3200 16-18-18, but a kit rated for DDR4-3200 16-18-18 might not be able to do DDR4-3000 15-16-16. However, the frequency and timings difference is pretty slight, so they'll probably overclock similarly.
  
### Maximum Recommended Daily Voltage
* [JEDEC JESD79-4B (p.174)](http://www.softnology.biz/pdf/JESD79-4B.pdf) specifies that the absolute maximum is 1.50 V.
  > Stresses greater than those listed under “Absolute Maximum Ratings” may cause permanent damage to the device. This is a stress rating only, and functional operation of the device at these or any other conditions above those indicated in the operational sections of this specification is not implied. Exposure to absolute maximum rating conditions for extended periods may affect reliability.
* This value is the official maximum of the DDR4 Spec for which all DDR4 are rated. However, numerous ICs cannot remain safe at such high sustained voltages. [S8C](https://www.hardwareluxx.de/community/f13/samsung-8gbit-ddr4-c-die-k4a8g045wc-overclocking-ergebnisse-im-startbeitrag-1198323.html) can degrade with voltages as low as 1.35 V under the right thermal and power delivery conditions. Furthermore, other ICs, such as H8D or S8B, have been observed dailying voltages well over 1.55 V. Do your research on what voltages are safe on your IC, or stick to 1.35 V or similar if this value is unknown. Due to random chance and silicon variance, YMMV compared to other people, so be safe.
* One common limiting factor for the maximum safe voltage you can operate is your CPU's architecture. According to [JEDEC](https://www.jedec.org/standards-documents/dictionary/terms/output-stage-drain-power-voltage-vddq), VDDQ, the voltage of data output, is tied to VDD, colloquially referred to as VDIMM or DRAM Voltage. This voltage interacts with the PHY or Physical Layer present on the CPU and may lead to long-term degradation of the IMC if set too high. As a result, daily use of VDIMM voltages above 1.60 V on Ryzen 3000 and 5000 and 1.65 V on Intel Consumer Lake-series Processors is not advisable as CPU degradation of the PHY is difficult to measure or notice until the issue becomes serious.
* It may be safe to daily 1.60 V as there are kits on the [B550 Unify-X QVL](https://www.msi.com/Motherboard/support/MEG-B550-UNIFY-X#support-mem-20) rated for 1.60 V. H8D, M8E, M16B and S8B *should* be fine at running 1.60 V daily, though it's recommended to have active airflow. Higher voltages lead to higher temperatures, and high temperatures can lower the threshold for what voltages are considered safe.
* Here is a list of common ICs and commonly used voltages for them:
   
   | IC  | Daily Voltage (V) | Extreme Voltage (V) |
   | :-: | :---------------: | :-----------------: |
   | H8D, H16A, M8E, M16B, S4D, S4E, S8B | Up to 1.55 | Above 1.55 |
   | H4A, H8A, H8C<sup>1</sup> , H16C, N8B | Up to 1.45 | Above 1.45 |
   | S8C | Up to 1.35 | N/A<sup>2</sup>        |
* The voltages marked as *Daily Voltage* are voltages that are known to be safe for the corresponding IC, provided temperatures are kept in check.
* The voltages marked as *Extreme Voltage* will likely not degrade but should be used cautiously. A RAM fan is recommended for these voltages.
* <sup>1</sup>Above 1.45 V has been reported to degrade on H8C. Use with caution.
* <sup>2</sup>S8C is known to scale negatively with voltage. It's recommended to stay at or below the maximum daily voltage.
  
### Ranking
* Below is how the most common ICs rank in terms of frequency and timings.
  | Tier | ICs | Description |
  | :-:  | :-: | :--:        |
  | S | S8B | Best DDR4 IC for all-around performance |
  | A | H8D, M8E<sup>1</sup>, M16B | Top Performing ICs. Known not to clock wall and generally scale with voltage. |
  | B | H8C, N8B, S4E | High-end ICs with the ability to run high frequencies with good timings. |
  | C | H8J, H16M, H16C, M16E, S8D | Decent ICs with good performance and decent frequency scaling. |
  | D | H8A, M8B, S8C, S4D | Low-end ICs commonly found in average cheap kits. Most are EOL and no longer relevant. |
  | F | H8M, M4A, S4S, N8C | Terrible ICs unable to reliably attain even the highest standard of the base JEDEC Specification. |
  * Partially based on [Buildzoid's older ranking](https://www.reddit.com/r/overclocking/comments/8cjla5/the_best_manufacturerdie_of_ddr_ram_in_order/dxfgd4x/). Some ICs are not included in this list due to the age of the post.
  * <sup>1</sup>Revisions of M8E mainly differ in the minimum tRCD achievable and how high they can clock without modification of VTT while maintaining stability. Generally, newer revisions of M8E (C9BKV, C9BLL, etc.) do tighter tRCD and clock higher without modification of VTT.
 
### Temperatures and Its Effect on Stability
* Generally, the hotter your RAM is, the less stability it will have at higher frequencies and/or tighter timings.
* The tRFC timings are very dependent on temperatures, as they are related to capacitor leakage, which is affected by temperature. Therefore, higher temperatures will need higher tRFC values. tRFC2 and tRFC4 are timings that activate when the operating temperature of DRAM hits 85 °C. Below these temperatures, these timings don't do anything.
* Generally speaking, RAM is temperature sensitive and its ideal range is ~30-40 °C. However, some ICs may be able to withstand higher temperatures, so YMMV.
* M8E, on the other hand, doesn't seem to be as strongly temperature sensitive, demonstrated by [buildzoid](https://www.youtube.com/watch?v=OeHEtULQg3Q).
* You might find that you're stable when running a memory test yet crash while gaming. This is because your CPU and/or GPU dump heat in the case, raising the RAM temperatures in the process. Thus, it is good to stress test your GPU while running a memory test to simulate stability while gaming.
 
## Integrated Memory Controller (IMC)
### Intel IMC
* Intel's Skylake IMC is pretty strong, so it shouldn't be the bottleneck when overclocking.  
  What would you expect from 14+++++?
* The Rocket Lake IMC, aside from the limitations regarding Gear 1 and Gear 2 memory support, has the strongest memory controller of all Intel consumer CPUs by a fair margin.
* Gear 1 is preferred because the memory controller clock is synced with the DRAM clock speed. Desync incurs a latency penalty.
* Non-K Alder Lake CPUs have locked VCCSA and may not work at higher frequencies at gear 1. You can expect 3200 - 3466 at gear 1.
* There are 2 voltages you need to change if overclocking RAM: system agent (VCCSA) and IO (VCCIO).  
  **DO NOT** leave these on auto, as they can pump dangerous voltage levels into your IMC, potentially degrading or even killing it. Most of the time, you can keep VCCSA and VCCIO the same, but sometimes too much can harm stability (credits: Silent_Scone).
  
  ![](Images/vccsa-vccio-sweet-spot.png)

  Below are my suggested VCCSA and VCCIO for 2 single rank DIMMs:

  | Effective Speed (MT/s) | VCCSA/VCCIO (V) |
  | :-------------: | :-------------: |
  | 3000 - 3600 | 1.15 - 1.20 |
  | 3600 - 4000 | 1.20 - 1.25 |
  | 4000 - 4200 | 1.25 - 1.30 |
  | 4200 - 4400 | 1.30 - 1.35 |
  * VCCIO should generally be 50 mV lower than VCCSA, and running 1.4 V VCCSA + 1.35 V VCCIO is acceptable as an upper limit.
  * Safe voltages on Alder Lake are not known because it is relatively new. 1.25-1.35 V VCCSA and VDDQ has not been proven to show considerable degradation.
    * For more information see [Information](#information).
  * With more DIMMs and/or dual-rank DIMMs, you may need higher VCCSA and VCCIO than suggested.
* On Skylake to Rocket Lake (inclusive) CPUs, tRCD and tRP are linked, meaning if you set tRCD 16 but tRP 17, both will run at the higher timing (17). This limitation is why many ICs don't do as well on Intel and why B-die is a good match for Intel.
  * On Asrock and EVGA UEFIs, they're combined into tRCDtRP. On ASUS UEFIs, tRP is hidden. On MSI and Gigabyte UEFIs, tRCD and tRP are visible but setting them to different values just sets both to the higher value.
* On Alder Lake CPUs, tRCD and tRP are no longer linked
* Expected memory latency range: 40 ns - 50 ns.
   * Expected memory latency range for B-Die: 35 ns - 45 ns.
   * Overall, latency varies between generations due to a difference in die size (ring bus). As a result, a 9900K will have a slightly lower latency than a 10700K at the same settings since the 10700K has the same die as a 10900K.
   * Latency is affected by the RTLs and IOLs. Generally speaking, higher quality boards and overclocking oriented boards will be more direct in routing the memory traces and will likely have lower RTLs and IOLs. On some motherboards, changing RTLs and IOLs have no effect.
  
### AMD IMC
Some terminology:
* MCLK: Real memory clock (half of the effective RAM speed). For example, for DDR4-3200, the MCLK is 1600 MHz.
* FCLK: Infinity Fabric clock.
* UCLK: Unified memory controller clock. Half of MCLK when MCLK and FCLK are not equal (desynchronized, 2:1 mode).
* On Zen and Zen+, MCLK == FCLK == UCLK. However, on Zen2 and Zen3, you can specify FCLK. If MCLK is 1600 MHz (DDR4-3200) and you set FCLK to 1600 MHz, UCLK will also be 1600 MHz unless you set MCLK:UCLK ratio to 2:1 (also known as UCLK DIV mode, etc.). However, if you set FCLK to 1800 MHz or higher, UCLK will run at 800 MHz (desynchronized).

* Ryzen 1000 and 2000's IMC can be finicky when overclocking and can't hit as high frequencies as Intel can. On the other hand, Ryzen 3000 and 5000's IMCs are much better and are more or less on par with Intel's newer Skylake-based CPUs, i.e., 9th and 10th gen.
* SOC voltage is the voltage to the IMC, and like with Intel, it's not recommended to leave it on auto. Typical ranges for this value range around 1.00V and 1.125V. Higher values are generally acceptable and may be necessary for stabilizing higher capacity memory and may aid in attaining FCLK stability.
* By contrast, memory instability can occur when SOC voltage is too high. This negative scaling typically occurs between 1.15 V and 1.25 V on most Ryzen CPUs.
  > There are clear differences in how the memory controller behaves on the different CPU specimens. The majority of the CPUs will do 3466MHz or higher at 1.050V SoC voltage, however the difference lies in how the different specimens react to the voltage. Some of the specimens seem scale with the increased SoC voltage, while the others simply refuse to scale at all or in some cases even illustrate negative scaling. All of the tested samples illustrated negative scaling (i.e. more errors or failures to train) when higher than 1.150V SoC was used. In all cases the maximum memory frequency was achieved at =< 1.100V SoC voltage.

  [~ The Stilt](https://forums.anandtech.com/threads/ryzen-strictly-technical.2500572/page-72#post-39391302)
* On Ryzen 3000, there's also CLDO_VDDG (commonly abbreviated to VDDG, not to be confused with CLDO_VDD**P**), which is the voltage to the Infinity Fabric. SOC voltage should be at least 40 mV above CLDO_VDDG as CLDO_VDDG is derived from SOC voltage.
  > Most cLDO voltages are regulated from the two main power rails of the CPU. In case of cLDO_VDDG and cLDO_VDDP, they are regulated from the VDDCR_SoC plane.  
  > Because of this, there are couple rules. For example, if you set the VDDG to 1.10 V, while your actual SoC voltage under load is 1.05 V the VDDG will stay roughly at 1.01V max.  
  > Likewise if you have VDDG set to 1.100 V and start increasing the SoC voltage, your VDDG will raise as well.  
  > I don't have the exact figure, but you can assume that the minimum drop-out voltage (Vin-Vout) is around 40 mV.  
  > Meaning you ACTUAL SoC voltage has to be at least by this much higher, than the requested VDDG for it to take effect as it is requested.  
  > 
  > Adjusting the SoC voltage alone, unlike on previous gen. parts doesn't do much if anything at all.  
  > The default value is fixed 1.100 V and AMD recommends keeping it at that level. Increasing the VDDG helps with the fabric overclocking in certain scenarios, but not always.  
  > 1800 MHz FCLK should be doable at the default 0.950 V value and for pushing the limits it might be beneficial to increase it to =< 1.05 V (1.100 - 1.125 V SoC, depending on the load-line).  

  [~ The Stilt](https://www.overclock.net/threads/strictly-technical-matisse-not-really.1728758/page-2#post-28031966)
  * On AGESA 1.0.0.4 or newer, VDDG is separated into VDDG IOD and VDDG CCD for the I/O die and the chiplets parts, respectively.

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
    > DDR4-3400 – 12.5 % of the samples   
    > DDR4-3466 – 25.0 % of the samples  
    > DDR4-3533 – 62.5 % of the samples
    
    [~ The Stilt](https://forums.anandtech.com/threads/ryzen-strictly-technical.2500572/page-72#post-39391302)
  * 2 CCD Ryzen 3000 CPUs (3900X and 3950X) seem to prefer 4 single rank sticks over 2 dual rank sticks.
    > For 2 CCD SKUs, 2 DPC SR configuration seems to be the way to go.  
    > Both the 3600 and 3700X did 1800 MHz UCLK on 1 DPC DR config, but most likely due to the discrepancy of the two CCDs in 3900X, it barely does 1733 MHz on those DIMMs.  
    > Meanwhile, with the 2 DPC SR config, there is no issue in reaching 1866 MHz FCLK/UCLK. 

    [~ The Stilt](https://www.overclock.net/forum/10-amd-cpus/1728758-strictly-technical-matisse-not-really-26.html#post28052342)
* tRCD is split into tRCDRD (read) and tRCDWR (write). Usually, tRCDWR can go lower than tRCDRD, but I haven't noticed any performance improvements from lowering tRCDWR. It's best to keep them the same.
* Geardown mode (GDM) is automatically enabled above DDR4-2666, forcing even tCL, tCWL, tRTP, tWR, and CR 1T. If you want to run odd tCL, disable GDM. If you're unstable, try running CR 2T, but that may negate the performance gain from dropping tCL and may even be less stable than GDM enabled.
  * For example, if you try to run DDR4-3000 CL15 with GDM enabled, CL will be rounded up to 16.
  * In terms of performance: GDM disabled CR 1T > GDM enabled CR 1T > GDM disabled CR 2T.
* On single CCD Ryzen 3000 CPUs below 3900X, write bandwidth is halved.
  > In memory bandwidth, we see something odd, the write speed of AMD's 3700X, and that's because of the CDD to IOD connection, where the writes are 16 B/cycle on the 3700X, but it's double that on the 3900X. AMD said this let them conserve power, which accounts for part of the lower TDP AMD aimed for. AMD says applications rarely do pure writes, but it did hurt the 3700X's performance in one of our benchmarks on the next page.  
  
  [~ TweakTown](https://www.tweaktown.com/reviews/9051/amd-ryzen-3900x-3700x-zen2-review/index3.html)
* Expected memory latency range:

  | Ryzen | Latency (ns) |
  | :---: | :----------: |
  | 1000 | 65 - 75 |
  | 2000 | 60 - 70 |
  | 3000 | 65 - 75 (1:1 MCLK:FCLK) <br/> 75+ (2:1 MCLK:FCLK) |
  | 4000/5000G | 55 - 65 |
  | 5000 | 60 - 70 (1:1 MCLK:FCLK) <br/> 70+ (desynchronised FCLK) |
* On Ryzen 3000 and 5000, high enough FCLK can overcome the penalties from desynchronising MCLK and FCLK, provided that you can lock your UCLK to MCLK.
  
  ![Chart](Images/optimal-fclk-vs-mclk.png) 
  * (Credits: [Buildzoid](https://www.youtube.com/watch?v=10pYf9wqFFY))
  
# Overclocking
* **Disclaimer**: The silicon lottery will affect your overclocking potential, so there may be some deviation from my suggestions.
* **Warning**: Data corruption is possible when overclocking RAM. It's advised to run `sfc /scannow` every so often to ensure any corrupted system files are fixed.
* The overclocking process is pretty simple and boils down to 3 steps:
  * Set very loose (high) timings.
  * Increase DRAM frequency until unstable.
  * Tighten (lower) timings.

## Miscellaneous Tips
* Usually, a 200 MHz increase in effective DRAM frequency negates the latency penalty of loosening tCL, tRCD, and tRP by 1 but has the benefit of higher bandwidth.  
  For example, DDR4-3000 15-17-17 has the same latency as DDR4-3200 16-18-18, but DDR4-3200 16-18-18 has higher bandwidth. This is typically after initial tuning has been completed and not at XMP.
* Generally speaking, frequency should be prioritized over tighter timings, as long as performance is not negatively impacted by FCLK sync, Command Rate, or Memory Gear mode.
* Secondary and tertiary timings (except for tRFC) don't change much, if at all, across the frequency range. If you have stable secondary and tertiary timings at DDR4-3200, you could probably run them at DDR4-3600, even DDR4-4000, provided your ICs, IMC, and motherboard are capable.

### Intel
* Loosening tCCDL to 8 may help with stability, especially above DDR4-3600. This does not bring a significant latency penalty but may considerably affect memory read and write bandwidth.
* Higher cache (aka uncore, ring) frequency can increase bandwidth and reduce latency.
* For Asus Maximus boards:
   * Play around with the Maximus Tweak Modes; sometimes, one will post where the other does not.
   * You can enable Round Trip Latency under Memory Training Algorithms to let the board attempt to train RTL and IOL values.
   * If you can't boot, you can try tweaking the skew control values.  
     More info [here](https://rog.asus.com/forum/showthread.php?47670-Maximus-7-Gene-The-road-to-overclocking-memory-without-increasing-voltage) (images broken).
* tXP (and subsequently PPD) has a major impact on AIDA64 memory latency.
* RTT Wr, Park, and Nom can have a massive impact on overclocking. The ideal values may depend on your board, memory IC and density. The "optimal" values will let you clock higher with less memory controller voltage. Some boards reveal the auto values (MSI) while others don't (Asus). Finding the perfect combination is time-consuming but very helpful for advanced tuning.
* On some motherboards, enabling XMP can allow for better overclocking.
  * Thanks to Bored and Muren for finding and verifying this on their Asrock motherboards.

### AMD
* Try playing around with ProcODT if you can't boot. This setting determines the processor's on-die termination impedance. According to [Micron](https://www.micron.com/support/~/media/D546161C2C6140BCB0BAEE954AA53433.pdf), higher settings of ProcODT can lead to more stable RAM, but the trade-off is potentially needing higher voltages. On Ryzen 1000 and 2000, you should try values between 40Ω and 68.6Ω due to the considerably weaker memory controller. 
On Ryzen 3000 and 5000, [1usmus](https://www.overclock.net/threads/new-dram-calculator-for-ryzen%E2%84%A2-1-7-3-overclocking-dram-on-am4-membench-0-8-dram-bench.1640919/page-240#post-28049664) suggests 28Ω - 40Ω. Lower settings may be harder to run but potentially helps with voltage requirements. Higher values may aid with stability, according to [Micron](https://media-www.micron.com/-/media/client/global/documents/products/technical-note/dram/tn4040_ddr4_point_to_point_design_guide.pdf?la=en&rev=d58bc222192d411aae066b2577a12677), values of ODT above 60Ω are only suitable for extremely weak memory controllers and lower power solutions.
This seems to line up with [The Stilt's](https://www.overclock.net/forum/10-amd-cpus/1728758-strictly-technical-matisse-not-really-26.html) settings.
  > Phy at AGESA defaults, except ProcODT of 40.0Ohm, an ASUS auto-rule for Optimem III.
* Lower SOC voltage and/or VDDG IOD may help with stability.
* On Ryzen 3000 and 5000, higher CLDO_VDDP can help with stability above DDR4-3600.
  > Increasing cLDO_VDDP seems beneficial > 3600MHz MEMCLKs, as increasing it seems to improve the margins and help with potential training issues. 
  
  Source: [The Stilt](https://www.overclock.net/forum/10-amd-cpus/1728758-strictly-technical-matisse-not-really-26.html).
 
  > Small changes to VDDP can have a big effect, and VDDP cannot not be set to a value greater than VDIMM-0.1V (**not to exceed 1.05V**).
  
  Source: [AMD](https://web.archive.org/web/20210520115124/https://community.amd.com/t5/blogs/community-update-4-let-s-talk-dram/ba-p/415902)
* When pushing FCLK around 1800 MHz, intermittent RAM training errors may be alleviated or eliminated by increasing VDDG CCD.

## Finding a Baseline
1. * Ensure your sticks are in the recommended DIMM slots (usually 2 and 4).

   * Make sure your CPU overclock is disabled when tuning RAM, as an unstable CPU can lead to memory errors. Likewise, when pushing high frequency with tight timings, your CPU may become unstable and may need to be re-done.

   * Make sure your UEFI/BIOS is up to date.

2. On Intel, set command rate (CR) to 2T if it isn't already and set tCCDL to 8

   On AMD, set Gear Down Mode to Enabled if it isn't already.

4. On Intel, start with 1.2 V VCCSA and 1.15 V VCCIO. For ADL, VCCIO does not exist. Note that if you have an Alder Lake non-K SKU, VCCSA will be locked and your overclock potential will be limited.

   On AMD, start with 1.10 V SOC, 0.95 V VDDP, 0.95 V VDDG CCD, and 1.05 V VDDG IOD.
   * If you are unable to boot or encounter errors while raising frequency or tightening timings, then these voltages may need to be increased. Refer to [Integrated Memory Controller (IMC)](#integrated-memory-controller-imc) for maximum recommended voltages and more information. Be careful not to raise them too much as negative scaling can occur. VCCSA/VCCIO or SOC voltage are the ones likely required to be raised, they can be incremented in steps of 25-50 mV.
   * SOC voltage might be named differently depending on the manufacturer.
     * Asrock: CPU VDDCR_SOC Voltage. If you can't find that, you can use SOC Overclock VID hidden in the AMD CBS menu.
       * [VID values](https://www.reddit.com/r/Amd/comments/842ehb/asrock_ab350_pro4_guide_bios_overclocking_raven/).
     * Asus: VDDCR SOC.
     * Gigabyte: (Dynamic<sup>1</sup>) Vcore SOC.
       * <sup>1</sup>Dynamic Vcore SOC is found on certain Gigabyte motherboards and is an offset voltage. Therefore, the base voltage can change automatically when increasing DRAM frequency. For example, +0.100 V at DDR4-3000 might result in 1.10 V actual, but +0.100V at DDR4-3400 might result in 1.20v actual.
     * MSI: CPU NB/SOC.
6. To find what voltage to use for your IC, refer to the [maximum recommended daily voltage section](#maximum-recommended-daily-voltage).
   * "Roll over" means that the IC becomes more unstable as you increase the voltage, sometimes to the point of not even POSTing.
   * ICs that are known to roll over above 1.35 V include but are not limited to: 8 Gb Samsung C-die and older Micron/SpecTek ICs (before M8E).

7. Set loose primary timings. See the table below.

   |Frequency|tCL|tRCD|tRP|tRAS|
   |---|---|---|---|---|
   |<=3200|16|20|20|40|
   |3201-3600|18|22|22|44|
   |3601-4000|20|24|24|48|
   |4001-4400|22|26|26|52|
   |4400+|24|28|28|56|

   Source: Eden from [Overclocking Discord](discord.gg/overclock)

   * Some ICs may not boot with very loose primary timings to begin with. It is recommended to loosen timings as the frequency is increases with the suggestions in the table above.
   * Some boards have auto rules that can cause issues, such as tCWL = tCL - 1, which can lead to tCWL being an odd number. If so, try setting tCWL to 1 value lower.
     * tCWL higher than 18 or 20 may not work, though it is not necessary to set such high values of tCWL.
   * See [this post](https://redd.it/ahs5a2) for more information on these timings.
  
8. Increase the DRAM frequency until it doesn't boot into Windows anymore. Keep in mind the expectations detailed above including the timings for each frequency range.
   * Ryzen 3000/5000:
     * Desynchronising MCLK and FCLK can incur a massive latency penalty, so you're better off tightening timings to keep your MCLK:FCLK 1:1. See [AMD - AM4](#amd-imc) for more information.
   * If you're on Intel, a quick way of knowing if you're unstable is to examine the RTLs and IOLs. Each group of RTLs and IOLs correspond to a channel. Within each group, 2 values correspond to each DIMM.

   RTLs and IOLs in Asrock Timing Configurator:
   
   ![](Images/intel-rtl-iol-difference-stable.png)

   As I have my sticks installed in channel A slot 2 and channel B slot 2, I need to look at D1 within each group of RTLs and IOLs.  
   RTLs should be no more than 2 apart, and IOLs should be no more than 1 apart.  
   In my case, RTLs are 53 and 55, which are exactly 2 apart, and IOLs are both 7.
   Note that having RTLs and IOLs within those ranges doesn't mean you're stable.
   * If you're on Ryzen 3000 or 5000, ensure that the Infinity Fabric frequency (FCLK) is set to half your effective DRAM frequency. Confirm this in ZenTimings by ensuring that FCLK matches UCLK and MCLK.
9. Run a memory tester of your choice.
   * Windows will use ~2000 MB, so make sure to account for that when entering the amount of RAM to test if the test has manual input. For example, I have 16 GB of RAM and usually test 14000 MB.
   * Minimum recommended coverage/runtime:
     * **For AMD, run Prime95 Large FFTs and OCCT VRAM with max utilization simultaneously to stress the FCLK and ensure FCLK stability. This should be run after any frequency/FCLK change.**
     * MemTestHelper (HCI MemTest): 20 % per thread.
     * Karhu RAMTest: 5000 %.
       * In the advanced tab, make sure CPU cache is set to enabled. This will speed up testing by ~20 %.
       * Testing for 6400 % coverage and a 1 hour duration has an error cover rate of 99,41 % and 98,43 %, respectively ([Source - FAQ section](https://www.karhusoftware.com/ramtest/)).
     * TM5 anta777 Extreme: 3 cycles.
       * Runtime varies with density. For 16 GB RAM, it usually takes between 1.5-2 hours. If you run 32 GB RAM, you can set the 12th row of the config (Time (%)) to half, and you'll get roughly the same runtime as 16 GB.
     * OCCT Memory: 30 minutes each for SSE and AVX.
     * **You can run more tests like other TM5 configs to ensure stability. It is recommended to run various tests for maximum error coverage.**
10. If you crash/freeze/BSOD or get an error, drop the DRAM frequency by a notch and test again.
11. Save your overclock profile in your UEFI.
12.  From this point on, you can either: try to go for a higher frequency or work on tightening the timings.
   * Keep in mind the expectations detailed above. If you're at the limit of your ICs and/or IMC, it's best to tighten the timings.
   
## Tightening Timings
* Make sure to run a memory test and benchmark after each change to ensure performance is improving.
  * I would recommend benchmarking 3 to 5 times and averaging the results, as memory benchmarks can have a bit of variance.
  * Theoretical maximum bandwidth (MB/s) = `Transfers per clock * Actual Clock * Channel Count * Bus Width * Bit to Byte ratio`.
       * Transfers per clock refers to the number of data transfers in one full memory clock cycle. This occurs twice per cycle on DDR RAM, on the rising and falling clock edges.
       * Actual Clock is the real clock of the memory, simply measured in MHz. This is generally shown as the real memory frequency by programs such as CPU-Z.
       * Channel Count is the number of memory channels active on your CPU.
       * Bus Width is the width of each memory channel, measured in bits. Since DDR1, this is 64 bits.
       * Bit to Byte ratio is a constant 1/8, or 0.125.
 
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
    
    * Your read and write bandwidth should be 90 % - 98 % of the theoretical maximum bandwidth.
      * On single CCD Ryzen 3000-5000 CPUs, write bandwidth should be 90 % - 98 % of half of the theoretical maximum bandwidth.  
        It is possible to hit half of the theoretical maximum write bandwidth. See [here](https://redd.it/cgc9bh).
      * Percentage of theoretically max bandwidth is inversely proportional to most memory timings. Generally speaking, as RAM timings are tightened, this value will increase.

1. I would recommend tightening some of the secondary timings first, as they can speed up memory testing. First is tRRDS, tRRDL and tFAW.
   
   | Timing | Safe | Tight | Extreme |
   | ------ | ---- | ----- | ------- |
   | tRRDS tRRDL tFAW | 6 6 24 | 4 6 16 | 4 4 16 |

   * The minimum value for which lowering tFAW will affect the performance of RAM is `tRRDS * 4` or `tRRDL * 4`, whichever is lower.
   * You don't have to run all of the timings at one preset. For example, you might only be able to run tRRDS tRRDL tFAW at the tight preset, but you may be able to run tWR at the extreme preset.
     
2. Next is tRFC. Default for 8 Gb ICs is 350 **ns** (note the units).
   * Note: Tightening tRFC too much can result in system freezes/lock-ups.
   * tRFC is the number of cycles for which the DRAM capacitors are "recharged" or refreshed. Because capacitor charge loss is proportional to temperature, RAM operating at higher temperatures may need substantially higher tRFC values.
   * To convert to ns: `2000 * timing / ddr_freq`.  
   For example, tRFC 250 at DDR4-3200 is `2000 * 250 / 3200 = 156.25 ns`.
   * To convert from ns (this is what you would type in your UEFI): `ns * ddr_freq / 2000`.  
   For example, 180 ns at DDR4-3600 is `180 * 3600 / 2000 = 324`, so you would type 324 in your UEFI.
   * Below are the typical tRFC in ns for the common ICs:
   
     | IC | tRFC (ns) |
     | :-: | :-------: |
     | S8B | 120 - 180 |
     | N8B | 150 - 170 |
     | H8D | 240 - 260 |
     | H8A, H8C | 260 - 280 |
     | M8E, M16B | 280 - 310 |
     | S8C | 300 - 340 |
     
   * For all other ICs, I would recommend doing a binary search to find the lowest stable tRFC.  
   For example, say your tRFC is 630. The next tRFC you should try is half of that (315). If that is unstable, you know that your lowest tRFC is between 315 and 630, so you try the midpoint (`(315 + 630) / 2 = 472.5`, round down to 472). If that is stable, you know that your lowest tRFC is between 315 and 472, so you try the midpoint and so on.
   * [tRFC table by Reous](https://www.hardwareluxx.de/community/threads/hynix-8gbit-ddr4-cjr-c-die-h5an8g8ncjr-djr-2020-update.1206340/) (bottom of page).
3. Here are my suggestions for the rest of the secondaries:

   | Timing | Safe | Tight | Extreme |
   | :----: | :--: | :---: | :-----: |
   | tCWL<sup>1</sup> | tCL | tCL - 1 | tCL - 2 |
   | tWR tRTP<sup>1</sup> | 20 10 | 16 8 | 12 6 |
   | tWTRS tWTRL | 4 12 | 4 10 | 4 8 |
   
   * On AMD, if GDM is enabled, tCWL, tWR and tRTP get rounded so drop them by 2 or keep them even.

   * On some Intel motherboards, tWR in the UEFI does nothing and instead needs to be controlled through tWRPRE (sometimes tWRPDEN). Dropping tWRPRE by 1 will drop tWR by 1, following the rule tWR = tWRPRE - tCWL - 4.
   * <sup>1</sup>tWR is 2*tRTP as per the Micron DDR4 datasheet. This relationship is also stated in the JEDEC DDR4 datasheet. 
   ![tWR tRTP relationship](Images/tWR-tRTP-relationship.png)  
   Thanks to [junkmann](https://github.com/integralfx/MemTestHelper/issues/55) for pointing this out.

   * On Intel, tWTRS/L should be left on auto and controlled with tWRRD_dg/sg, respectively. Dropping tWRRD_dg by 1 will drop tWTRS by 1. Likewise, with tWRRD_sg. Once they're as low as you can go, manually set tWTRS/L.
   * On Intel, changing tCWL will affect tWRRD_dg/sg and thus tWTR_S/L. If you lower tCWL by 1, you need to lower tWRRD_dg/sg by 1 to keep the same tWTR values. Note that this might also affect tWR per the relationship described earlier.
   * <sup>1</sup>Some motherboards don't play nice with odd tCWL. For example, I'm stable at 4000 15-19-19 tCWL 14, yet tCWL 15 doesn't even POST. Another user has had similar experiences. Some motherboards may seem fine but have issues with it at higher frequencies (Asus). Manually setting tCWL equal to tCL if tCL is even or one below if tCL is uneven should alleviate this (eg. if tCL = 18 try tCWL = 18 or 16, if tCL = 17 try tCWL = 16).
   * The extreme preset is not the minimum floor in this case. tRTP can go as low as 5 (6 with Gear Down Mode on), while tWTRS/L can go as low as 1/6. Some boards are fine doing tCWL as low as tCL - 6. Keep in mind that this *will* increase the load on your memory controller.
   * On AMD, tCWL can often be set to tCL - 2 but is known to require higher tWRRD.
   
4. Now for the tertiaries:
    * If you're on AMD, refer to [this post](https://redd.it/ahs5a2).  
      My suggestion:
  
       | Timing | Safe | Tight | Extreme |
       | ------ | ---- | ----- | ------- |
       | tRDRDSCL tWRWRSCL | 4 4 | 3 3 | 2 2 |
     
        * A lot of ICs are known to have issues with low SCLs. Values such as 2 are extremely difficult for all but ICs such as Samsung 8 Gb B-Die. These values are not necessarily linked, and values such as 5 are acceptable. Mixing and matching is possible, and more often than not, tRDRDSCL will be the one that needs to be run 1 or even 2 values higher. Values above 5 greatly hurt bandwidth, so their use is not advised.
     
    * If you're on Intel, tune the tertiaries one group at a time.  
      My suggestions:
      
      | Timing | Safe | Tight | Extreme |
      | ------ | ---- | ----- | ------- |
      | tRDRD_sg/dg/dr/dd | 8/4/8/8 | 7/4/7/7 | 6/4/6/6 |
      | tWRWR_sg/dg/dr/dd | 8/4/8/8 | 7/4/7/7 | 6/4/6/6 |
      * For tWRRD_sg/dg, see step 3. For tWRRD_dr/dd, drop them both by 1 until you get instability or performance degrades.
      * For tRDWR_sg/dg/dr/dd, drop them all by 1 until you get instability or performance degrades. You can usually run them all the same, e.g., 9/9/9/9.
        * Setting these too tight can cause system freezes.
      * Note that dr only affects dual rank sticks, so you can ignore this timing if you have single rank sticks. In the same way, dd only needs to be considered when you run two DIMMs per channel. You can also set them to 0 or 1 if you really want to.  

      * For dual rank setups (see [notes on ranks](#a-note-on-logical-ranks-and-density)):
         * tRDRD_dr/dd can be lowered a step further to 5 for a large bump in read bandwidth.
         * tWRWR_sg 6 can cause write bandwidth regression over tWRWR_sg 7, despite being stable.
    
5. Drop tCL by 1 until it's unstable.
   * On AMD, if GDM is enabled, tCL gets rounded so drop tCL by 2 or keep it even.   
 
6. On Intel, drop tRCD and tRP by 1 until unstable.  

   On AMD, drop tRCD by 1 until unstable. Repeat with tRP.
   * Note: More IMC voltage may be necessary to stabilize tighter tRCD.
   
7. Set `tRAS = tRCD(RD) + tRTP`. Increase if unstable.
   * This is the absolute minimum tRAS can be.  
   ![tRAS](Images/tras-datasheet-diagram.png)
   Here, tRAS is the time between ACT and PRE commands.
     * ACT to READ = tRCD
     * READ to PRE = tRTP
     * Hence, tRAS = tRCD + tRTP.


8. Set `tRC = tRP + tRAS`. Increase if unstable.
   * tRC is only available on AMD and some Intel UEFIs.
   * On Intel UEFIs, tRC does seem to be affected by tRP and tRAS, even if it is hidden.
     * (1) [tRP 19 tRAS 42](Images/tRC-tRP19-tRAS42.png) - fully stable.
     * (2) [tRP 19 tRAS 36](Images/tRC-tRP19-tRAS36.png) - instant error.
     * (3) [tRP 25 tRAS 36](Images/tRC-tRP25-tRAS42.png) - stable up to 500 %.
     * In (1) and (3), tRC is 61 and isn't completely unstable. However, in (2) tRC is 55 and RAMTest finds an error instantly. This indicates that my RAM can do low tRAS, but not low tRC. Since tRC is hidden, I need higher tRAS to get higher tRC to ensure stability.

9. Increase tREFI until it's unstable. The binary search method for finding the lowest tRFC can also be applied here.  
   Otherwise, here are my suggestions:
   | Timing | Safe | Tight | Extreme |
   | ------ | ---- | ----- | ------- |
   | tREFI | 32768 | 40000 | Max (65535 or 65534) |
   * It's typically not good to increase tREFI too much as ambient temperature changes (e.g., winter to summer) can be enough to cause instability.
   * Keep in mind that running max tREFI can corrupt files, so tread with caution.

10. Finally, onto command rate.

    AMD:
    * Getting GDM disabled and CR 1 stable can be pretty difficult, but it's worth a shot if you've come this far down the rabbit hole.
    * If you can get GDM disabled and CR 1 stable without touching anything, skip this section.
    * CR 1 becomes significantly harder to run as the frequency increases. Often, running CR 2 can help with achieving higher frequencies.
    * On AMD, Gear Down Mode will override Command Rate. For this reason, disabling Gear Down Mode to set CR 2 may be beneficial to overall stability.
    
    1. One possibility is to set the drive strengths to 60-20-20-24 and setup times to 63-63-63.
       * Drive strengths are ClkDrvStr, AddrCmdDrvStr, CsOdtDrvStr and CkeDrvStr.
       * Setup times are AddrCmdSetup, CsOdtSetup and CkeSetup.
    2. If you can't POST, adjust the setup times until you can (you should adjust them all together).
    3. Run a memory test.
    4. Adjust setup times, then drive strengths if unstable.
    * My stable GDM off CR 1 settings

      ![](Images/gdm-off-cr-1t-stable.png)

    1. Often, a drive strength above 24 ohms may hurt stability. Furthermore, running non-zero setup times is rarely needed; however, it may aid in the stabilization of CR 1.
   
    Intel:
    * If below DDR4-4400, try setting CR to 1T. If that doesn't work, leave CR on 2T.
    * On Asus Maximus boards, enabling Trace Centering can help greatly with pushing CR 1T to higher frequencies.

11. On Intel, you can increase IOL offsets to reduce IOLs. Make sure to run a memory test after. More info [here](https://hwbot.org/newsflash/3058_advanced_skylake_overclocking_tune_ddr4_memory_rtlio_on_maximus_viii_with_alexaros_guide).
  * In general, RTL and IOL values impact memory performance. Therefore, lowering them will increase bandwidth and decrease latency quite significantly.
  
    ![](Images/rtl-iol-aida-impact.png)

  * Lower values will, in some cases, also help with stability and lower memory controller voltage requirements. Some boards train them very well on their own. Some boards allow for easy tuning, while other boards simply ignore any user input.
  * If all else fails, you can try manually decreasing the RTL and IOL pair.

12. You can also increase DRAM voltage to drop timings even more. Keep in mind the [voltage scaling characteristics of your ICs](#voltage-scaling) and the [maximum recommended daily voltage](#maximum-recommended-daily-voltage).

# Useful Links
## Benchmarks
* [Impact of RAM on Intel's Skylake desktop architecture by KingFaris](https://kingfaris.co.uk/blog/intel-ram-oc-impact)
* [RAM timings and their influence on games and applications (AMD) by Reous](https://www.hardwareluxx.de/community/threads/ram-timings-und-deren-einfluss-auf-spiele-und-anwendungen-amd-update-23-05-2020.1269156/)
## Information
* [r/overclocking Wiki - DDR4](https://www.reddit.com/r/overclocking/wiki/ram/ddr4)
* [Demystifying Memory Overclocking on Ryzen: OC Guidelines and Explaining Subtimings, Resistances, Voltages, and More! by varexos717](https://redd.it/ahs5a2)
* [Maximus Z690 and Alder Lake: Modern CPU’s require Modern Overclocking Solutions](https://rog.asus.com/forum/showthread.php?126369-Maximus-Z690-and-Alder-Lake-Modern-CPU%92s-require-Modern-Overclocking-Solutions)
* [12th Gen Intel Memory Overclocking Voltages - buildzoid](http://buildzoid.blogspot.com/2022/03/12th-gen-intel-memory-overclocking.html)
* [HardwareLUXX Ryzen RAM OC Thread](https://www.hardwareluxx.de/community/f13/ryzen-ram-oc-thread-moegliche-limitierungen-1216557.html)
* [Ryzen 3000 Memory / Fabric (X370/X470/X570) by elmor](https://www.overclock.net/forum/13-amd-general/1728878-ryzen-3000-memory-fabric-x370-x470-x570.html)
* [Intel Memory Overclocking Quick Reference by sdch](https://www.overclock.net/threads/official-intel-ddr4-24-7-memory-stability-thread.1569364/page-392#post-27784556)
* [The road to overclocking memory without increasing voltage by Raja@ASUS](https://rog.asus.com/forum/showthread.php?47670-Maximus-7-Gene-The-road-to-overclocking-memory-without-increasing-voltage) (images broken)
* [Advanced Skylake Overclocking: Tune DDR4 Memory RTL/IO on Maximus VIII with Alex@ro's Guide](https://hwbot.org/newsflash/3058_advanced_skylake_overclocking_tune_ddr4_memory_rtlio_on_maximus_viii_with_alexaros_guide)
* [BSOD codes when OC'ing and possible actions](https://www.reddit.com/r/overclocking/comments/atwtt5/psa_bsod_codes_when_ocing_and_possible_actions/)
