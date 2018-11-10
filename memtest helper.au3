#include <EditConstants.au3>
#include <GUIConstantsEx.au3>
#include <MsgBoxConstants.au3>
#include <StaticConstants.au3>
#include <WinAPISys.au3>
#include <WindowsConstants.au3>

Global Const $MEMTEST_EXE = "memtest_6.0_no_nag.exe"
Global Const $MAX_THREADS = EnvGet("NUMBER_OF_PROCESSORS")
Global Const $MEMTEST_WIDTH = 221
Global Const $MEMTEST_HEIGHT = 253
Global Const $UPDATE_INTERVAL = 100         ; how often to update coverage info (in ms)
Global Const $MEMTEST_BTN_START = "Button1"
Global Const $MEMTEST_BTN_STOP = "Button2"
Global Const $MEMTEST_EDT_RAM = "Edit1"

Global Const $CBS_DROPDOWNLIST = 0x3        ; for some reason constant isn't defined

If Not FileExists($MEMTEST_EXE) Then
    MsgBox($MB_OK, "Error", "MemTest can not be located")
    Exit
EndIf

Global $memtest_hwnds[$MAX_THREADS]         
Global $is_finished[$MAX_THREADS]           ; index tells whether memtest has reached specified coverage %
Global $hwnd_gui                            
Global $edt_total                           ; edit for total RAM to test (in MB)
Global $cbo_threads                         ; combo for number of threads
Global $cbo_rows                            ; combo for number of rows
Global $ipt_x_offset                        ; input for positioning memtest windows
Global $ipt_y_offset                        ; input for positioning memtest windows
Global $btn_run
Global $btn_stop
Global $lst_coverage_items[$MAX_THREADS]
Global $chk_stop_at                         ; to stop at a specified coverage %
Global $edt_stop_at
create_gui()

; --- EVENT HANDLING ---

While 1
    Sleep(1)
WEnd

Func close()
    Exit
EndFunc

Func run_memtest()
    If Not validate_input() Then
        Return
    EndIf

    GUICtrlSetState($edt_total, $GUI_DISABLE)
    GUICtrlSetState($cbo_threads, $GUI_DISABLE)
    GUICtrlSetState($cbo_rows, $GUI_DISABLE)
    GUICtrlSetState($btn_run, $GUI_DISABLE)
    GUICtrlSetState($btn_stop, $GUI_ENABLE)
    GUICtrlSetState($chk_stop_at, $GUI_DISABLE)
    GUICtrlSetState($edt_stop_at, $GUI_DISABLE)

    For $i = 0 To GuiCtrlRead($cbo_threads) - 1
        $is_finished[$i] = False
    Next

    start()
    
    AdlibRegister("update_coverage_info", $UPDATE_INTERVAL)
    
    WinActivate($hwnd_gui)
EndFunc

Func stop_memtest()   
    ; click stop
    For $i = 0 To GuiCtrlRead($cbo_threads) - 1
        If Not $is_finished[$i] Then
            ControlClick($memtest_hwnds[$i], "", $MEMTEST_BTN_STOP)
        EndIf
    Next
    
    AdlibUnRegister("update_coverage_info")
    ; the user may have pressed stop while the coverage
    ; info was updating which causes some info to be missing
    Sleep(100)
    update_coverage()
    
    GUICtrlSetState($edt_total, $GUI_ENABLE)
    GUICtrlSetState($cbo_threads, $GUI_ENABLE)
    GUICtrlSetState($cbo_rows, $GUI_ENABLE)
    GUICtrlSetState($btn_run, $GUI_ENABLE)
    GUICtrlSetState($btn_stop, $GUI_DISABLE)
    GUICtrlSetState($chk_stop_at, $GUI_ENABLE)
    GUICtrlSetState($edt_stop_at, $GUI_ENABLE)
    
    WinActivate($hwnd_gui)
EndFunc

Func offset_changed()
    move_memtests()
EndFunc

Func center_memtests()
    center_xy_offsets()
    
    move_memtests()
EndFunc

; checked/unchecked stop at (%) checkbox
Func chk_stop_at_checked()   
    If BitAND(GuiCtrlRead($chk_stop_at), $GUI_CHECKED) Then
        GUICtrlSetState($edt_stop_at, $GUI_ENABLE)
    Else
        GUICtrlSetState($edt_stop_at, $GUI_DISABLE)
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
        MsgBox($MB_OK, "", "MemTest finished")
    EndIf
EndFunc

Func update_coverage()
    Local $threads = GUICtrlRead($cbo_threads)
    
    For $i = 0 To $threads - 1
        Local $item = $lst_coverage_items[$i]
        Local $hwnd = $memtest_hwnds[$i]
        
        Local $info = get_coverage_info($hwnd)
        Local $coverage = $info[0]
        Local $errors = $info[1]
        GUICtrlSetData($item, $coverage & "|" & $errors)
        
        ; check coverage %
        If BitAND(GuiCtrlRead($chk_stop_at), $GUI_CHECKED) Then
            Local $stop_at = Number(GUICtrlRead($edt_stop_at), $NUMBER_DOUBLE)
            If Number($coverage, $NUMBER_DOUBLE) > $stop_at Then
                If Not $is_finished[$i] Then
                    ; click stop button
                    ControlClick($hwnd, "", $MEMTEST_BTN_STOP)
                    $is_finished[$i] = True
                EndIf
            EndIf
        EndIf
    Next
    
    ; set rest to nothing
    For $i = $threads To $MAX_THREADS - 1
        GUICtrlSetData($lst_coverage_items[$i], "-|-")
    Next
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
    $hwnd_gui = GUICreate("MemTest Helper", 200, 300, -1, -1)
    GUISetOnEvent($GUI_EVENT_CLOSE, "close")
    
    GUICtrlCreateTab(0, 0, 200, 300)
    
    ; Main tab
    GUICtrlCreateTabItem("Main")
    GUICtrlCreateLabel("Total RAM to test (MB):", 10, 25)
    $edt_total = GUICtrlCreateEdit("", 135, 20, 50, Default, 0)
    
    GUICtrlCreateLabel("Number of threads:", 10, 50)
    $cbo_threads = GUICtrlCreateCombo("", 135, 45, 50, Default, $CBS_DROPDOWNLIST)
    GUICtrlSetData($cbo_threads, get_cbo_threads(), $MAX_THREADS)
    GUICtrlSetOnEvent($cbo_threads, "cbo_threads_selected")
    
    $btn_run = GUICtrlCreateButton("Run", 10, 70, 80, 30)
    GUICtrlSetOnEvent($btn_run, "run_memtest")
    
    $btn_stop = GUICtrlCreateButton("Stop", 110, 70, 80, 30)
    GUICtrlSetState($btn_stop, $GUI_DISABLE)
    GUICtrlSetOnEvent($btn_stop, "stop_memtest")
    
    Local $lst_coverage = GUICtrlCreateListView("Coverage (%)|Errors", 10, 110, 180, 180)
    For $i = 0 To $MAX_THREADS - 1
        $lst_coverage_items[$i] = GUICtrlCreateListViewItem("-|-", $lst_coverage)
    Next
    
    ; Settings tab
    GUICtrlCreateTabItem("Settings")
    Local $rows = 2
    Local $cols = $MAX_THREADS / $rows
    Local $x_offset = (_WinAPI_GetSystemMetrics(0) - $MEMTEST_WIDTH * $cols) / 2
    Local $y_offset = (_WinAPI_GetSystemMetrics(1) - $MEMTEST_HEIGHT * $rows) / 2
    
    GUICtrlCreateLabel("X offset:", 10, 30)
    $ipt_x_offset = GUICtrlCreateInput($x_offset, 55, 25, 50, Default, 0)
    GUICtrlCreateUpdown($ipt_x_offset)
    GUICtrlSetOnEvent($ipt_x_offset, "offset_changed")
    
    GUICtrlCreateLabel("Y offset:", 10, 55)
    $ipt_y_offset = GUICtrlCreateInput($y_offset, 55, 50, 50, Default, 0)
    GUICtrlCreateUpdown($ipt_y_offset)
    GUICtrlSetOnEvent($ipt_y_offset, "offset_changed")
    
    Local $btn_center = GUICtrlCreateButton("Center", 110, 35, 80)
    GUICtrlSetOnEvent($btn_center, "center_memtests")
    
    $chk_stop_at = GUICtrlCreateCheckbox("Stop at (%):", 10, 75)
    GUICtrlSetOnEvent($chk_stop_at, "chk_stop_at_checked")
    $edt_stop_at = GUICtrlCreateEdit("", 90, 75, 50, Default, 0)
    GUICtrlSetState($edt_stop_at, $GUI_DISABLE)
    
    GUICtrlCreateLabel("Number of rows:", 10, 105)
    $cbo_rows = GUICtrlCreateCombo("", 90, 100, 50, Default, $CBS_DROPDOWNLIST)
    GUICtrlSetData($cbo_rows, get_cbo_rows(), "2")
    GUICtrlSetOnEvent($cbo_rows, "cbo_rows_selected")
    
    GUICtrlCreateTabItem("")
    
    ; show GUI
    GUISetState()
EndFunc

Func validate_input()
    Local $amount = GUICtrlRead($edt_total)
    
    If $amount = "" Then
        MsgBox($MB_OK, "Error", "Please enter amount of RAM")
        Return False
    EndIf

    If Not StringIsDigit($amount) Then
        MsgBox($MB_OK, "Error", "Amount of RAM must be an integer")
        Return False
    EndIf

    If Number($amount) < $MAX_THREADS Then
        MsgBox($MB_OK, "Error", "Amount of RAM must be greater than " & $MAX_THREADS)
        Return False
    EndIf
    
    Return True
EndFunc

Func start()
    Local $threads = GUICtrlRead($cbo_threads)
    Local $ram_amount = GUICtrlRead($edt_total) / $threads
    
    close_all_memtests()
    
    For $i = 0 To $threads - 1
        Local $pid = Run($MEMTEST_EXE)
        Sleep(100)
        $memtest_hwnds[$i] = get_hwnd_for_pid($pid)
    Next
    
    move_memtests()
    
    Local $rows = GUICtrlRead($cbo_rows)
    Local $cols = $threads / $rows
    
    For $row = 0 To $rows - 1
        For $col = 0 To $cols - 1
            Local $index = $row * $cols + $col
            Local $hwnd = $memtest_hwnds[$index]
                    
            ; input amount of RAM
            ControlSend($hwnd, "", $MEMTEST_EDT_RAM, $ram_amount)
            
            ; click start
            ControlClick($hwnd, "", $MEMTEST_BTN_START)
        Next
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
    ; memtest not started
    If BitAND(GUICtrlGetState($btn_run), $GUI_ENABLE) Then
        Return
    EndIf
    
    Local $x_offset = Number(GUICtrlRead($ipt_x_offset), $NUMBER_DOUBLE)
    Local $y_offset = Number(GUICtrlRead($ipt_y_offset), $NUMBER_DOUBLE)
    Local $rows = GUICtrlRead($cbo_rows)
    Local $cols = GUICtrlRead($cbo_threads) / $rows
    
    For $row = 0 To $rows - 1
        For $col = 0 To $cols - 1
            Local $index = $row * $cols + $col
            Local $hwnd = $memtest_hwnds[$index]
            
            WinMove($hwnd, "", _ 
                    $col * $MEMTEST_WIDTH + $x_offset, _
                    $row * $MEMTEST_HEIGHT + $y_offset)
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

; --- HELPER FUNCTIONS ---
