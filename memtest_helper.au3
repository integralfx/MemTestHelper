#include <ColorConstants.au3>
#include <ComboConstants.au3>
#include <Date.au3>
#include <EditConstants.au3>
#include <GuiComboBox.au3>
#include <GUIConstantsEx.au3>
#include <GuiListView.au3>
#include <MemoryConstants.au3>
#include <MsgBoxConstants.au3>
#include <StaticConstants.au3>
#include <WinAPISys.au3>
#include <WindowsConstants.au3>

Global Const $MEMTEST_EXE = "memtest_6.0_no_nag.exe"
Global Const $NUM_THREADS = EnvGet("NUMBER_OF_PROCESSORS")
Global Const $MAX_THREADS = $NUM_THREADS * 4
Global Const $MEMTEST_WIDTH = 217
Global Const $MEMTEST_HEIGHT = 247
Global Const $UPDATE_INTERVAL = 100         ; how often to update coverage info (in ms)
Global Const $MEMTEST_BTN_START = "Button1"
Global Const $MEMTEST_BTN_STOP = "Button2"
Global Const $MEMTEST_EDT_RAM = "Edit1"
Global Const $MEMTEST_MAX_RAM = 2048        ; max RAM for each instance

Global Const $GUI_WIDTH = 230
Global Const $GUI_HEIGHT = 350

If Not FileExists($MEMTEST_EXE) Then
    MsgBox($MB_OK, "Error", "MemTest can not be located")
    Exit
EndIf

Global $is_running = False
Global $memtest_hwnds[$MAX_THREADS]         
Global $is_finished[$MAX_THREADS]           ; index tells whether memtest has reached specified coverage %
Global $hwnd_gui                            
Global $btn_auto_ram                        ; button to automatically input available RAM
Global $edt_ram                             ; edit for RAM to test (in MB)
Global $cbo_threads                         ; combo for number of threads
Global $h_timer
Global $lbl_elapsed_time
Global $lbl_estimated_time
Global $lbl_speed
Global $cbo_rows                            ; combo for number of rows
Global $ipt_x_offset                        ; input for positioning memtest windows
Global $ipt_y_offset                        ; input for positioning memtest windows
Global $ipt_x_spacing
Global $ipt_y_spacing
Global $btn_run
Global $btn_stop
Global $btn_show                            ; button for showing MemTest windows
Global $lst_coverage_items[$MAX_THREADS + 1]; index 0 is the total coverage % and errors
Global $chk_stop_at                         ; to stop at a specified coverage %
Global $chk_stop_at_total                   ; stop at total coverage %
Global $ipt_stop_at
Global $chk_stop_at_err
Global $chk_stop_at_err_total
Global $ipt_stop_at_err
Global $ipt_update                          ; update interval
create_gui()

; --- EVENT HANDLING ---

While 1
    Sleep(1)
WEnd

Func close()
    If $is_running Then
        stop_memtest()
    EndIf
    
    close_all_memtests()
    
    Exit
EndFunc

Func run_memtest()
    If Not validate_input() Then
        Return
    EndIf

    $h_timer = TimerInit()

    GUICtrlSetState($btn_auto_ram, $GUI_DISABLE)
    GUICtrlSetState($edt_ram, $GUI_DISABLE)
    GUICtrlSetState($cbo_threads, $GUI_DISABLE)
    GUICtrlSetState($cbo_rows, $GUI_DISABLE)
    GUICtrlSetState($btn_run, $GUI_DISABLE)
    GUICtrlSetState($btn_stop, $GUI_ENABLE)
    GUICtrlSetState($btn_show, $GUI_ENABLE)
    GUICtrlSetState($chk_stop_at, $GUI_DISABLE)
    GUICtrlSetState($ipt_stop_at, $GUI_DISABLE)
    GUICtrlSetState($chk_stop_at_total, $GUI_DISABLE)
    GUICtrlSetState($chk_stop_at_err, $GUI_DISABLE)
    GUICtrlSetState($ipt_stop_at_err, $GUI_DISABLE)
    GUICtrlSetState($chk_stop_at_err_total, $GUI_DISABLE)
    GUICtrlSetState($ipt_update, $GUI_DISABLE)

    For $i = 0 To GuiCtrlRead($cbo_threads) - 1
        $is_finished[$i] = False
    Next

    $is_running = True
    start()
    
    AdlibRegister("update_coverage_info", GUICtrlRead($ipt_update))
    AdlibRegister("update_time", 500)
    
    WinActivate($hwnd_gui)
EndFunc

Func stop_memtest()   
    ; click stop
    For $i = 0 To GuiCtrlRead($cbo_threads) - 1
        If Not $is_finished[$i] Then
            ControlClick($memtest_hwnds[$i], "", $MEMTEST_BTN_STOP)
        EndIf
    Next
    
    AdlibUnRegister("update_time")
    AdlibUnRegister("update_coverage_info")
    ; the user may have pressed stop while the coverage
    ; info was updating which causes some info to be missing
    Sleep(100)
    update_coverage()
    
    GUICtrlSetState($btn_auto_ram, $GUI_ENABLE)
    GUICtrlSetState($edt_ram, $GUI_ENABLE)
    GUICtrlSetState($cbo_threads, $GUI_ENABLE)
    GUICtrlSetState($cbo_rows, $GUI_ENABLE)
    GUICtrlSetState($btn_run, $GUI_ENABLE)
    GUICtrlSetState($btn_stop, $GUI_DISABLE)
    GUICtrlSetState($btn_show, $GUI_DISABLE)
    GUICtrlSetState($chk_stop_at, $GUI_ENABLE)
    If BitAND(GuiCtrlRead($chk_stop_at), $GUI_CHECKED) Then
        GUICtrlSetState($ipt_stop_at, $GUI_ENABLE)
        GUICtrlSetState($chk_stop_at_total, $GUI_ENABLE)
    EndIf
    GUICtrlSetState($chk_stop_at_err, $GUI_ENABLE)
    If BitAND(GuiCtrlRead($chk_stop_at_err), $GUI_CHECKED) Then
        GUICtrlSetState($ipt_stop_at_err, $GUI_ENABLE)
        GUICtrlSetState($chk_stop_at_err_total, $GUI_ENABLE)
    EndIf
    GUICtrlSetState($ipt_update, $GUI_ENABLE)
    
    $is_running = False
    
    WinActivate($hwnd_gui)
    MsgBox($MB_OK, "", "MemTest finished")
EndFunc

Func show_memtest()
    If $is_running Then
        For $i = 0 To GUICtrlRead($cbo_threads) - 1
            WinActivate($memtest_hwnds[$i])
        Next
        
        ; GUISetState() doesn't work for some reason :/
        WinActivate($hwnd_gui)
    EndIf
EndFunc

Func offset_changed()
    move_memtests()
EndFunc

Func spacing_changed()
    move_memtests()
EndFunc

Func center_memtests()
    center_xy_offsets()
    
    move_memtests()
EndFunc

Func btn_auto_ram_clicked()
    Local $mem = MemGetStats()
    Local $free = Floor($mem[$MEM_AVAILPHYSRAM] / 1024)
    GUICtrlSetData($edt_ram, $free)
EndFunc

; checked/unchecked stop at (%) checkbox
Func chk_stop_at_checked()   
    If BitAND(GuiCtrlRead($chk_stop_at), $GUI_CHECKED) Then
        GUICtrlSetState($ipt_stop_at, $GUI_ENABLE)
        GUICtrlSetState($chk_stop_at_total, $GUI_ENABLE)
    Else
        GUICtrlSetState($ipt_stop_at, $GUI_DISABLE)
        GUICtrlSetState($chk_stop_at_total, $GUI_DISABLE)
    EndIf
EndFunc

Func chk_stop_at_err_checked()
    If BitAND(GuiCtrlRead($chk_stop_at_err), $GUI_CHECKED) Then
        GUICtrlSetState($ipt_stop_at_err, $GUI_ENABLE)
        GUICtrlSetState($chk_stop_at_err_total, $GUI_ENABLE)
    Else
        GUICtrlSetState($ipt_stop_at_err, $GUI_DISABLE)
        GUICtrlSetState($chk_stop_at_err_total, $GUI_DISABLE)
    EndIf
EndFunc

; selected number of threads
Func cbo_threads_selected()
    Local $threads = GUICtrlRead($cbo_threads)
    GUICtrlSetData($cbo_rows, get_cbo_rows(), Mod($threads, 2) = 0 ? "2" : "1")
    
    center_xy_offsets()
EndFunc

Func cbo_rows_selected()
    center_xy_offsets()
EndFunc

; --- EVENT HANDLING ---


; --- THREADS ---

Func update_coverage_info()
    update_coverage()
    
    If is_all_finished() Then
        stop_memtest()
    EndIf
EndFunc

Func update_coverage()
    Local $threads = GUICtrlRead($cbo_threads)
    Local $stop_at = Number(GUICtrlRead($ipt_stop_at))
    Local $stop_at_checked = BitAND(GUICtrlRead($chk_stop_at), $GUI_CHECKED)
    Local $stop_at_total_checked = BitAND(GUICtrlRead($chk_stop_at_total), $GUI_CHECKED)
    Local $stop_at_err = Number(GUICtrlRead($ipt_stop_at_err))
    Local $stop_at_err_checked = BitAND(GUICtrlRead($chk_stop_at_err), $GUI_CHECKED)
    Local $stop_at_err_total_checked = BitAND(GUICtrlRead($chk_stop_at_err_total), $GUI_CHECKED)
    Local $total_coverage = 0
    Local $total_errors = 0
    
    ; 0 is the total coverage and errors
    For $i = 1 To $threads
        Local $item = $lst_coverage_items[$i]
        Local $hwnd = $memtest_hwnds[$i - 1]
        
        Local $info = get_coverage_info($hwnd)
        Local $coverage = Number($info[0], $NUMBER_DOUBLE)
        Local $errors = Number($info[1])
        GUICtrlSetData($item, $i & "|" & $coverage & "|" & $errors)
        
        If $errors > 0 Then
            GUICtrlSetColor($item, $COLOR_RED)
        EndIf
        
        ; check coverage %
        If $stop_at_checked And Not $stop_at_total_checked Then
            If $coverage > $stop_at Then
                If Not $is_finished[$i - 1] Then
                    ; click stop button
                    ControlClick($hwnd, "", $MEMTEST_BTN_STOP)
                    $is_finished[$i - 1] = True
                EndIf
            EndIf
        EndIf
        
        ; check error count
        If $stop_at_err_checked And Not $stop_at_err_total_checked Then
            If $errors > $stop_at_err Then
                If Not $is_finished[$i - 1] Then
                    ; click stop button
                    ControlClick($hwnd, "", $MEMTEST_BTN_STOP)
                    $is_finished[$i - 1] = True
                EndIf
            EndIf
        EndIf
        
        $total_coverage += $coverage
        $total_errors += $errors
    Next
    
    GUICtrlSetData($lst_coverage_items[0], "T|" & $total_coverage & "|" & $total_errors)
    
    If $total_errors > 0 Then
        GUICtrlSetColor($lst_coverage_items[0], $COLOR_RED)
    EndIf
    
    ; check total coverage
    If $stop_at_checked And $stop_at_total_checked Then
        If $total_coverage > $stop_at Then
            stop_memtests()
        EndIf
        
        Local $diff = TimerDiff($h_timer)
        Local $est = ($diff / $total_coverage * $stop_at) - $diff
        Local $h, $m, $s
        _TicksToTime($est, $h, $m, $s)
        GUICtrlSetData($lbl_estimated_time, StringFormat("%02dh%02dm%02ds to %d%%", $h, $m, $s, $stop_at))
    EndIf
    
    ; check total errors
    If $stop_at_err_checked And $stop_at_err_total_checked Then
        If $total_errors > $stop_at_err Then
            stop_memtests()
        EndIf
    EndIf
    
    ; set rest to nothing
    For $i = $threads + 1 To $MAX_THREADS
        GUICtrlSetData($lst_coverage_items[$i], $i & "|-|-")
    Next
EndFunc

Func update_time()
    Local $diff = TimerDiff($h_timer), $h, $m, $s
    _TicksToTime($diff, $h, $m, $s)
    GUICtrlSetData($lbl_elapsed_time, StringFormat("%02dh%02dm%02ds", $h, $m, $s))
    
    ; estimate time for coverage
    Local $threads = GUICtrlRead($cbo_threads)
    ; get total coverage
    Local $total_coverage = 0
    For $i = 1 To $threads
        Local $item = $lst_coverage_items[$i]
        Local $hwnd = $memtest_hwnds[$i - 1]
        
        Local $info = get_coverage_info($hwnd)
        Local $coverage = Number($info[0], $NUMBER_DOUBLE)
        
        $total_coverage += $coverage
    Next
    
    Local $stop_at = Number(GUICtrlRead($ipt_stop_at))
    Local $stop_at_checked = BitAND(GUICtrlRead($chk_stop_at), $GUI_CHECKED)
    Local $stop_at_total_checked = BitAND(GUICtrlRead($chk_stop_at_total), $GUI_CHECKED)
    Local $est = 0, $cov = $stop_at
    ; use user input coverage %
    If $stop_at_checked Then
        If $stop_at_total_checked Then
            $est = ($diff / $total_coverage * $stop_at) - $diff
        Else
            ; calculate average coverage and use that to estimate
            Local $avg = $total_coverage / $threads
            $est = ($diff / $avg * $stop_at) - $diff
        EndIf
    Else
        ; calculate average coverage and use that to estimate
        Local $avg = $total_coverage / $threads
        ; round up to next multiple of 100
        $cov = (Int($avg / 100) + 1) * 100
        $est = ($diff / $avg * $cov) - $diff
    EndIf
    
    _TicksToTime($est, $h, $m, $s)
    GUICtrlSetData($lbl_estimated_time, StringFormat("%02dh%02dm%02ds to %d%%", $h, $m, $s, $cov))
    
    Local $ram = Number(GUICtrlRead($edt_ram))
    Local $speed = ($total_coverage / 100) * $ram / ($diff / 1000)
    GUICtrlSetData($lbl_speed, StringFormat("%.2fMB/s", $speed))
EndFunc

; --- THREADS ---


; --- HELPER FUNCTIONS ---

Func get_cbo_rows()
    Local $threads = GUICtrlRead($cbo_threads)
    
    Local $items = ""
    For $i = 1 To $threads
        If Mod($threads, $i) = 0 Then
            $items &= "|" & $i
        EndIf
    Next

    Return $items
EndFunc

Func get_cbo_threads()
    Local $items = ""
    For $i = 1 to $MAX_THREADS
        $items &= "|" & $i
    Next
    
    Return $items
EndFunc

Func center_xy_offsets()
    Local $rows = GUICtrlRead($cbo_rows)
    Local $cols = GUICtrlRead($cbo_threads) / $rows
    Local $x_offset = Floor((_WinAPI_GetSystemMetrics(0) - $MEMTEST_WIDTH * $cols) / 2)
    Local $y_offset = Floor((_WinAPI_GetSystemMetrics(1) - $MEMTEST_HEIGHT * $rows) / 2)
    
    GUICtrlSetData($ipt_x_offset, $x_offset)
    GUICtrlSetData($ipt_y_offset, $y_offset)
EndFunc

Func create_gui()
    ; use events
    Opt("GUIOnEventMode", 1)
    
    ; create GUI
    $hwnd_gui = GUICreate("MemTest Helper", $GUI_WIDTH, $GUI_HEIGHT, -1, -1)
    GUISetOnEvent($GUI_EVENT_CLOSE, "close")
    
    ; create tabs
    GUICtrlCreateTab(0, 0, $GUI_WIDTH, $GUI_HEIGHT)
    create_main_tab()
    create_settings_tab()
    create_about_tab()
    GUICtrlCreateTabItem("")
    
    ; show GUI
    GUISetState()
EndFunc

Func create_main_tab()
    GUICtrlCreateTabItem("Main")
    
    Local $x = 25
    Local $y = 25
    
    $btn_auto_ram = GUICtrlCreateButton("RAM to test (MB):", $x, $y, 100, 20)
    GUICtrlSetOnEvent($btn_auto_ram, "btn_auto_ram_clicked")
    GUICtrlSetTip($btn_auto_ram, "Automatically input free RAM")
    $edt_ram = GUICtrlCreateEdit("", $x + 125, $y, 50, 20, 0)
    
    $y += 30
    GUICtrlCreateLabel("Number of threads:", $x, $y)
    $cbo_threads = GUICtrlCreateCombo("", $x + 125, $y - 5, 50, 100, BitOR($CBS_DROPDOWNLIST, $WS_VSCROLL, $CBS_NOINTEGRALHEIGHT))
    GUICtrlSetData($cbo_threads, get_cbo_threads(), $NUM_THREADS)
    GUICtrlSetOnEvent($cbo_threads, "cbo_threads_selected")
    
    $y += 20
    $btn_run = GUICtrlCreateButton("Run", $x, $y, 55, 30)
    GUICtrlSetOnEvent($btn_run, "run_memtest")
    
    $btn_stop = GUICtrlCreateButton("Stop", $x + 60, $y, 55, 30)
    GUICtrlSetState($btn_stop, $GUI_DISABLE)
    GUICtrlSetOnEvent($btn_stop, "stop_memtest")
    
    $btn_show = GUICtrlCreateButton("Show", $x + 120, $y, 55, 30)
    GUICtrlSetState($btn_show, $GUI_DISABLE)
    GUICtrlSetOnEvent($btn_show, "show_memtest")
    
    $y += 35
    GUICtrlCreateLabel("Elapsed:", $x, $y)
    $lbl_elapsed_time = GUICtrlCreateLabel("00h00m00s", $x + 60, $y, 120)
    
    GUICtrlCreateLabel("Estimated:", $x, $y + 15, 120)
    $lbl_estimated_time = GUICtrlCreateLabel("00h00m00s", $x + 60, $y + 15, 120)
    
    GUICtrlCreateLabel("Speed:", $x, $y + 30)
    $lbl_speed = GUICtrlCreateLabel("0.00MB/s", $x + 60, $y + 30, 120)
    
    $x -= 15
    $y += 50
    Local $lst_coverage = GUICtrlCreateListView("No.|Coverage (%)|Errors", $x, $y, 210, 180)
    ; total coverage and errors
    $lst_coverage_items[0] = GUICtrlCreateListViewItem("T|-|-", $lst_coverage)
    For $i = 1 To $MAX_THREADS
        $lst_coverage_items[$i] = GUICtrlCreateListViewItem($i & "|-|-", $lst_coverage)
    Next
    For $i = 0 To 2
        _GUICtrlListView_SetColumnWidth($lst_coverage, $i, $LVSCW_AUTOSIZE_USEHEADER)
    Next
EndFunc

Func create_settings_tab()
    GUICtrlCreateTabItem("Settings")
    Local $rows = 2
    Local $cols = $NUM_THREADS / $rows
    Local $x_offset = (_WinAPI_GetSystemMetrics(0) - $MEMTEST_WIDTH * $cols) / 2
    Local $y_offset = (_WinAPI_GetSystemMetrics(1) - $MEMTEST_HEIGHT * $rows) / 2
    
    Local $x = 25
    Local $y = 30
    GUICtrlCreateLabel("X offset:", $x, $y)
    $ipt_x_offset = GUICtrlCreateInput($x_offset, $x + 45, $y - 5, 50, Default, 0)
    GUICtrlCreateUpdown($ipt_x_offset)
    GUICtrlSetOnEvent($ipt_x_offset, "offset_changed")
    
    GUICtrlCreateLabel("Y offset:", $x, $y + 25)
    $ipt_y_offset = GUICtrlCreateInput($y_offset, $x + 45, $y + 20, 50, Default, 0)
    GUICtrlCreateUpdown($ipt_y_offset)
    GUICtrlSetOnEvent($ipt_y_offset, "offset_changed")
    
    Local $btn_center = GUICtrlCreateButton("Center", $x + 100, $y + 5, 80)
    GUICtrlSetOnEvent($btn_center, "center_memtests")
    
    $y += 50
    GUICtrlCreateLabel("X spacing:", $x, $y)
    $ipt_x_spacing = GUICtrlCreateInput(0, $x + 55, $y - 5, 50, Default, 0)
    GUICtrlCreateUpdown($ipt_x_spacing)
    GUICtrlSetOnEvent($ipt_x_spacing, "spacing_changed")
    
    GUICtrlCreateLabel("Y spacing:", $x, $y + 25)
    $ipt_y_spacing = GUICtrlCreateInput(0, $x + 55, $y + 20, 50, Default, 0)
    GUICtrlCreateUpdown($ipt_y_spacing)
    GUICtrlSetOnEvent($ipt_y_spacing, "spacing_changed")
    
    $y += 50
    GUICtrlCreateLabel("Number of rows:", $x, $y)
    $cbo_rows = GUICtrlCreateCombo("", $x + 85, $y - 5, 50, 100, BitOR($CBS_DROPDOWNLIST, $WS_VSCROLL, $CBS_NOINTEGRALHEIGHT))
    GUICtrlSetData($cbo_rows, get_cbo_rows(), $rows)
    GUICtrlSetOnEvent($cbo_rows, "cbo_rows_selected")
    
    $y += 25
    $chk_stop_at = GUICtrlCreateCheckbox("Stop at (%):", $x, $y)
    GUICtrlSetOnEvent($chk_stop_at, "chk_stop_at_checked")
    $ipt_stop_at = GUICtrlCreateInput("", $x + 95, $y, 35, Default, 0)
    GUICtrlSetState($ipt_stop_at, $GUI_DISABLE)
    $chk_stop_at_total = GUICtrlCreateCheckbox("Total", $x + 135, $y)
    GUICtrlSetState($chk_stop_at_total, $GUI_DISABLE)
    GUICtrlSetTip($chk_stop_at_total, "Stop at total coverage")

    $chk_stop_at_err = GUICtrlCreateCheckbox("Stop at (errors):", $x, $y + 25)
    GUICtrlSetOnEvent($chk_stop_at_err, "chk_stop_at_err_checked")
    $ipt_stop_at_err = GUICtrlCreateInput("", $x + 95, $y + 25, 35, Default, 0)
    GUICtrlSetState($ipt_stop_at_err, $GUI_DISABLE)
    $chk_stop_at_err_total = GUICtrlCreateCheckbox("Total", $x + 135, $y + 25)
    GUICtrlSetState($chk_stop_at_err_total, $GUI_DISABLE)
    GUICtrlSetTip($chk_stop_at_err_total, "Stop at total errors")
    
    $y += 55
    GUICtrlCreateLabel("Update interval (ms):", $x, $y)
    $ipt_update = GUICtrlCreateInput($UPDATE_INTERVAL, $x + 100, $y - 5, 40, 20, 0)
EndFunc

Func create_about_tab()
    GUICtrlCreateTabItem("About")
    
    GUICtrlCreateLabel("Version 1.6", 84, 120)
    
    GUICtrlCreateLabel("Discord:", 50, 150)
    GUICtrlCreateInput("∫ntegral#7834", 100, 145, 80, Default, $ES_READONLY)
EndFunc

Func validate_input()
    Local $amount = GUICtrlRead($edt_ram)
    
    If $amount = "" Then
        MsgBox($MB_OK, "Error", "Please enter amount of RAM")
        Return False
    EndIf

    If Not StringIsDigit($amount) Then
        MsgBox($MB_OK, "Error", "Amount of RAM must be an integer")
        Return False
    EndIf
    
    Local $threads = GUICtrlRead($cbo_threads)
    $amount = Number($amount)
    If $amount < $threads Then
        MsgBox($MB_OK, "Error", "Amount of RAM must be greater than " & $threads)
        Return False
    EndIf
    
    If $amount > $MEMTEST_MAX_RAM * $threads Then
        MsgBox($MB_OK, "Error", "Amount of RAM must be at most " & $MEMTEST_MAX_RAM * $threads)
        Return False
    EndIf
    
    Local $mem = MemGetStats()
    Local $mem_total = Floor($mem[$MEM_TOTALPHYSRAM] / 1024)
    If $amount > $mem_total Then
        MsgBox($MB_OK, "Error", "Amount of RAM exceeds total RAM (" & $mem_total & ")")
        Return False
    EndIf
    
    Local $mem_avail = Floor($mem[$MEM_AVAILPHYSRAM] / 1024)
    If $amount > $mem_avail Then
        Local $ret = MsgBox($MB_YESNO, "Warning", _
                            "Amount of RAM exceeds available RAM (" & $mem_avail & ")" & @CRLF & _
                            "This will cause RAM to be paged to your storage," & @CRLF & _
                            "making MemTest really slow." & @CRLF & _
                            "Continue?")
        If $ret = $IDNO Then
            Return False
        EndIf
    EndIf
    
    ; validate X and Y offsets
    Local $x_offset = GUICtrlRead($ipt_x_offset)
    Local $y_offset = GUICtrlRead($ipt_y_offset)
    
    If $x_offset = "" Then
        MsgBox($MB_OK, "Error", "Please enter X offset")
        Return False
    EndIf
    If $y_offset = "" Then
        MsgBox($MB_OK, "Error", "Please enter Y offset")
        Return False
    EndIf
    
    If Not StringIsDigit($x_offset) Then
        MsgBox($MB_OK, "Error", "X offset must be an integer")
        Return False
    EndIf
    If Not StringIsDigit($y_offset) Then
        MsgBox($MB_OK, "Error", "Y offset must be an integer")
        Return False
    EndIf
    
    ; validate X and Y spacing
    Local $x_spacing = GUICtrlRead($ipt_x_spacing)
    Local $y_spacing = GUICtrlRead($ipt_y_spacing)
    
    If $x_spacing = "" Then
        MsgBox($MB_OK, "Error", "Please enter X spacing")
        Return False
    EndIf
    If $y_spacing = "" Then
        MsgBox($MB_OK, "Error", "Please enter Y spacing")
        Return False
    EndIf
    
    If Not StringIsDigit($x_spacing) Then
        MsgBox($MB_OK, "Error", "X spacing must be an integer")
        Return False
    EndIf
    If Not StringIsDigit($y_spacing) Then
        MsgBox($MB_OK, "Error", "Y spacing must be an integer")
        Return False
    EndIf
    
    ; validate stop at %
    If BitAND(GUICtrlRead($chk_stop_at), $GUI_CHECKED) Then
        Local $stop_at = GUICtrlRead($ipt_stop_at)
        
        If $stop_at = "" Then
            MsgBox($MB_OK, "Error", "Please enter stop at (%)")
            Return False
        EndIf
        
        If Not StringIsDigit($stop_at) Then
            MsgBox($MB_OK, "Error", "Stop at (%) must be an integer")
            Return False
        EndIf
        
        $stop_at = Number($stop_at)
        
        If $stop_at <= 0 Then
            MsgBox($MB_OK, "Error", "Stop at (%) must be greater than 0")
            Return False
        EndIf
    EndIf
    
    ; validate stop at error count
    If BitAND(GUICtrlRead($chk_stop_at_err), $GUI_CHECKED) Then
        Local $stop_at = GUICtrlRead($ipt_stop_at_err)
        
        If $stop_at = "" Then
            MsgBox($MB_OK, "Error", "Please enter stop at error count")
            Return False
        EndIf
        
        If Not StringIsDigit($stop_at) Then
            MsgBox($MB_OK, "Error", "Stop at error count must be an integer")
            Return False
        EndIf
        
        $stop_at = Number($stop_at)
        
        If $stop_at <= 1 Then
            MsgBox($MB_OK, "Error", "Stop at error count must be greater than 1")
            Return False
        EndIf
    EndIf
    
    ; validate update interval
    Local $update = GUICtrlRead($ipt_update)
    
    If $update = "" Then
        MsgBox($MB_OK, "Error", "Please enter update interval")
        Return False
    EndIf
    
    If Not StringIsDigit($update) Then
        MsgBox($MB_OK, "Error", "Update interval must be an integer")
        Return False
    EndIf
    
    $update = Number($update)
    ; update interval of 1ms doesn't let you stop
    If $update < 2 Then
        MsgBox($MB_OK, "Error", "Update interval must be at least 2")
        Return False
    EndIf
    
    Return True
EndFunc

Func start()
    Local $threads = GUICtrlRead($cbo_threads)
    Local $ram_amount = Round(GUICtrlRead($edt_ram) / $threads, 2)
    
    close_all_memtests()
    
    For $i = 0 To $threads - 1
        Local $pid = Run($MEMTEST_EXE)
        Sleep(100)
        $memtest_hwnds[$i] = get_hwnd_for_pid($pid)
    Next
    
    move_memtests()
    
    For $i = 0 To $threads - 1
        Local $hwnd = $memtest_hwnds[$i]
        
        ; input amount of RAM
        ControlSend($hwnd, "", $MEMTEST_EDT_RAM, $ram_amount)
        
        ; click start
        ControlClick($hwnd, "", $MEMTEST_BTN_START)
    Next
EndFunc

Func get_hwnd_for_pid($pid)
    Local $list = WinList()
    For $i = 1 To $list[0][0]
        If $pid = WinGetProcess($list[$i][1]) Then
            Return $list[$i][1]
        EndIf
    Next

    Return -1
EndFunc

Func ConsoleWriteLn($msg = "")
    Return ConsoleWrite($msg & @CRLF)
EndFunc

; returns the index of the first digit in $str
Func find_first_digit($str)
    If $str = "" Then
        Return -1
    EndIf
    
    $str = StringSplit($str, "")
    For $i = 1 To $str[0]
        If StringIsDigit($str[$i]) Then
            Return $i - 1
        EndIf
    Next
    
    Return -1
EndFunc

; returns [coverage, errors]
Func get_coverage_info($hwnd)
    Local $info[2] = ["", ""]
    
    Local $str = ControlGetText($hwnd, "", "[CLASS:Static; INSTANCE:1]")
    If $str = "" Then
        Return $info
    EndIf
    
    ; get the start of coverage %
    Local $start = find_first_digit($str)
    If $start = -1 Then
        Return $info
    EndIf
    $str = StringRight($str, StringLen($str) - $start)
    
    Local $coverage = StringSplit($str, "%")[1]
    ; find start of error count
    Local $start = StringInStr($str, "Coverage, ") - 1 + StringLen("Coverage, ")
    ; Test over. 47.3% Coverage, 0 Errors
    ;                            ^^^^^^^^
    Local $errors = StringRight($str, StringLen($str) - $start)
    ; 0 Errors
    ; ^
    $errors = StringSplit($errors, " ")[1]
    
    $info[0] = $coverage
    $info[1] = $errors
    Return $info
EndFunc

Func move_memtests()
    If Not $is_running Then
        Return
    EndIf
    
    Local $x_offset = Number(GUICtrlRead($ipt_x_offset), $NUMBER_DOUBLE)
    Local $y_offset = Number(GUICtrlRead($ipt_y_offset), $NUMBER_DOUBLE)
    Local $x_spacing = Number(GUICtrlRead($ipt_x_spacing))
    Local $y_spacing = NUmber(GUICtrlRead($ipt_y_spacing))
    Local $rows = GUICtrlRead($cbo_rows)
    Local $cols = GUICtrlRead($cbo_threads) / $rows
    
    For $row = 0 To $rows - 1
        For $col = 0 To $cols - 1
            Local $index = $row * $cols + $col
            Local $hwnd = $memtest_hwnds[$index]
            Local $x = $col * $MEMTEST_WIDTH + $x_offset + $col * $x_spacing
            Local $y = $row * $MEMTEST_HEIGHT + $y_offset + $row * $y_spacing
            
            WinMove($hwnd, "", $x, $y)
        Next
    Next
EndFunc

; checks if all elements of $is_finished are True
Func is_all_finished()   
    For $i = 0 To GUICtrlRead($cbo_threads) - 1
        If Not $is_finished[$i] Then
            Return False
        EndIf
    Next
    
    Return True
EndFunc

Func close_all_memtests()   
    Local $list = ProcessList($MEMTEST_EXE)
    For $i = 1 To $list[0][0]
        ProcessClose($list[$i][1])
    Next
EndFunc

Func stop_memtests()
    For $i = 0 To GUICtrlRead($cbo_threads) - 1
        If Not $is_finished[$i] Then
            Local $hwnd = $memtest_hwnds[$i]
            ControlClick($hwnd, "", $MEMTEST_BTN_STOP)
            $is_finished[$i] = True
        EndIf
    Next
EndFunc

; --- HELPER FUNCTIONS ---
