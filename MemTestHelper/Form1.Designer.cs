namespace MemTestHelper
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tab_main = new System.Windows.Forms.TabPage();
            this.btn_hide = new System.Windows.Forms.Button();
            this.lstCoverage = new System.Windows.Forms.ListView();
            this.hdr_no = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdr_coverage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdr_errors = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblSpeedValue = new System.Windows.Forms.Label();
            this.lbl_speed = new System.Windows.Forms.Label();
            this.lblEstimatedTime = new System.Windows.Forms.Label();
            this.lbl_estimated = new System.Windows.Forms.Label();
            this.lblElapsedTime = new System.Windows.Forms.Label();
            this.lbl_elapsed = new System.Windows.Forms.Label();
            this.btn_show = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.cboThreads = new System.Windows.Forms.ComboBox();
            this.lbl_num_threads = new System.Windows.Forms.Label();
            this.txtRAM = new System.Windows.Forms.TextBox();
            this.btnAutoRAM = new System.Windows.Forms.Button();
            this.tab_settings = new System.Windows.Forms.TabPage();
            this.chkStartMin = new System.Windows.Forms.CheckBox();
            this.chkStopAtTotal = new System.Windows.Forms.CheckBox();
            this.chkStopOnError = new System.Windows.Forms.CheckBox();
            this.txtStopAt = new System.Windows.Forms.TextBox();
            this.chkStopAt = new System.Windows.Forms.CheckBox();
            this.cboRows = new System.Windows.Forms.ComboBox();
            this.btn_center = new System.Windows.Forms.Button();
            this.udWinHeight = new System.Windows.Forms.NumericUpDown();
            this.udYOffset = new System.Windows.Forms.NumericUpDown();
            this.lbl_win_height = new System.Windows.Forms.Label();
            this.lbl_y_offset = new System.Windows.Forms.Label();
            this.udYSpacing = new System.Windows.Forms.NumericUpDown();
            this.lbl_rows = new System.Windows.Forms.Label();
            this.lbl_y_spacing = new System.Windows.Forms.Label();
            this.udXSpacing = new System.Windows.Forms.NumericUpDown();
            this.lbl_x_spacing = new System.Windows.Forms.Label();
            this.udXOffset = new System.Windows.Forms.NumericUpDown();
            this.lbl_x_offset = new System.Windows.Forms.Label();
            this.tab_about = new System.Windows.Forms.TabPage();
            this.txt_discord = new System.Windows.Forms.TextBox();
            this.lbl_discord = new System.Windows.Forms.Label();
            this.lbl_version = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tab_main.SuspendLayout();
            this.tab_settings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udWinHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udYOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udYSpacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udXSpacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udXOffset)).BeginInit();
            this.tab_about.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab_control
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.tabControl.Controls.Add(this.tab_main);
            this.tabControl.Controls.Add(this.tab_settings);
            this.tabControl.Controls.Add(this.tab_about);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl.Name = "tab_control";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(227, 382);
            this.tabControl.TabIndex = 0;
            // 
            // tab_main
            // 
            this.tab_main.Controls.Add(this.btn_hide);
            this.tab_main.Controls.Add(this.lstCoverage);
            this.tab_main.Controls.Add(this.lblSpeedValue);
            this.tab_main.Controls.Add(this.lbl_speed);
            this.tab_main.Controls.Add(this.lblEstimatedTime);
            this.tab_main.Controls.Add(this.lbl_estimated);
            this.tab_main.Controls.Add(this.lblElapsedTime);
            this.tab_main.Controls.Add(this.lbl_elapsed);
            this.tab_main.Controls.Add(this.btn_show);
            this.tab_main.Controls.Add(this.btnStop);
            this.tab_main.Controls.Add(this.btnRun);
            this.tab_main.Controls.Add(this.cboThreads);
            this.tab_main.Controls.Add(this.lbl_num_threads);
            this.tab_main.Controls.Add(this.txtRAM);
            this.tab_main.Controls.Add(this.btnAutoRAM);
            this.tab_main.Location = new System.Drawing.Point(4, 22);
            this.tab_main.Margin = new System.Windows.Forms.Padding(0);
            this.tab_main.Name = "tab_main";
            this.tab_main.Size = new System.Drawing.Size(219, 356);
            this.tab_main.TabIndex = 0;
            this.tab_main.Text = "Main";
            this.tab_main.UseVisualStyleBackColor = true;
            // 
            // btn_hide
            // 
            this.btn_hide.Location = new System.Drawing.Point(112, 83);
            this.btn_hide.Name = "btn_hide";
            this.btn_hide.Size = new System.Drawing.Size(85, 20);
            this.btn_hide.TabIndex = 8;
            this.btn_hide.Text = "Hide";
            this.btn_hide.UseVisualStyleBackColor = true;
            this.btn_hide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // lst_coverage
            // 
            this.lstCoverage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lstCoverage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hdr_no,
            this.hdr_coverage,
            this.hdr_errors});
            this.lstCoverage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstCoverage.Location = new System.Drawing.Point(4, 154);
            this.lstCoverage.Name = "lst_coverage";
            this.lstCoverage.Size = new System.Drawing.Size(210, 199);
            this.lstCoverage.TabIndex = 7;
            this.lstCoverage.UseCompatibleStateImageBehavior = false;
            this.lstCoverage.View = System.Windows.Forms.View.Details;
            // 
            // hdr_no
            // 
            this.hdr_no.Text = "No.";
            this.hdr_no.Width = 31;
            // 
            // hdr_coverage
            // 
            this.hdr_coverage.Text = "Coverage (%)";
            this.hdr_coverage.Width = 86;
            // 
            // hdr_errors
            // 
            this.hdr_errors.Text = "Errors";
            this.hdr_errors.Width = 70;
            // 
            // lbl_speed_value
            // 
            this.lblSpeedValue.AutoSize = true;
            this.lblSpeedValue.Location = new System.Drawing.Point(81, 137);
            this.lblSpeedValue.Name = "lbl_speed_value";
            this.lblSpeedValue.Size = new System.Drawing.Size(54, 13);
            this.lblSpeedValue.TabIndex = 6;
            this.lblSpeedValue.Text = "0.00MB/s";
            // 
            // lbl_speed
            // 
            this.lbl_speed.AutoSize = true;
            this.lbl_speed.Location = new System.Drawing.Point(21, 137);
            this.lbl_speed.Name = "lbl_speed";
            this.lbl_speed.Size = new System.Drawing.Size(41, 13);
            this.lbl_speed.TabIndex = 6;
            this.lbl_speed.Text = "Speed:";
            // 
            // lbl_estimated_time
            // 
            this.lblEstimatedTime.AutoSize = true;
            this.lblEstimatedTime.Location = new System.Drawing.Point(81, 122);
            this.lblEstimatedTime.Name = "lbl_estimated_time";
            this.lblEstimatedTime.Size = new System.Drawing.Size(62, 13);
            this.lblEstimatedTime.TabIndex = 6;
            this.lblEstimatedTime.Text = "00h00m00s";
            // 
            // lbl_estimated
            // 
            this.lbl_estimated.AutoSize = true;
            this.lbl_estimated.Location = new System.Drawing.Point(21, 122);
            this.lbl_estimated.Name = "lbl_estimated";
            this.lbl_estimated.Size = new System.Drawing.Size(56, 13);
            this.lbl_estimated.TabIndex = 6;
            this.lbl_estimated.Text = "Estimated:";
            // 
            // lbl_elapsed_time
            // 
            this.lblElapsedTime.AutoSize = true;
            this.lblElapsedTime.Location = new System.Drawing.Point(81, 107);
            this.lblElapsedTime.Name = "lbl_elapsed_time";
            this.lblElapsedTime.Size = new System.Drawing.Size(62, 13);
            this.lblElapsedTime.TabIndex = 6;
            this.lblElapsedTime.Text = "00h00m00s";
            // 
            // lbl_elapsed
            // 
            this.lbl_elapsed.AutoSize = true;
            this.lbl_elapsed.Location = new System.Drawing.Point(21, 107);
            this.lbl_elapsed.Name = "lbl_elapsed";
            this.lbl_elapsed.Size = new System.Drawing.Size(48, 13);
            this.lbl_elapsed.TabIndex = 6;
            this.lbl_elapsed.Text = "Elapsed:";
            // 
            // btn_show
            // 
            this.btn_show.Location = new System.Drawing.Point(21, 83);
            this.btn_show.Name = "btn_show";
            this.btn_show.Size = new System.Drawing.Size(85, 20);
            this.btn_show.TabIndex = 5;
            this.btn_show.Text = "Show";
            this.btn_show.UseVisualStyleBackColor = true;
            this.btn_show.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // btn_stop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(112, 53);
            this.btnStop.Name = "btn_stop";
            this.btnStop.Size = new System.Drawing.Size(85, 25);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btn_run
            // 
            this.btnRun.Location = new System.Drawing.Point(21, 53);
            this.btnRun.Name = "btn_run";
            this.btnRun.Size = new System.Drawing.Size(85, 25);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // cbo_threads
            // 
            this.cboThreads.DropDownHeight = 100;
            this.cboThreads.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboThreads.FormattingEnabled = true;
            this.cboThreads.IntegralHeight = false;
            this.cboThreads.Location = new System.Drawing.Point(146, 28);
            this.cboThreads.Name = "cbo_threads";
            this.cboThreads.Size = new System.Drawing.Size(50, 21);
            this.cboThreads.TabIndex = 4;
            this.cboThreads.SelectionChangeCommitted += new System.EventHandler(this.cboThreads_SelectionChangeCommitted);
            // 
            // lbl_num_threads
            // 
            this.lbl_num_threads.AutoSize = true;
            this.lbl_num_threads.Location = new System.Drawing.Point(21, 33);
            this.lbl_num_threads.Name = "lbl_num_threads";
            this.lbl_num_threads.Size = new System.Drawing.Size(97, 13);
            this.lbl_num_threads.TabIndex = 3;
            this.lbl_num_threads.Text = "Number of threads:";
            // 
            // txt_ram
            // 
            this.txtRAM.Location = new System.Drawing.Point(146, 3);
            this.txtRAM.Name = "txt_ram";
            this.txtRAM.Size = new System.Drawing.Size(50, 20);
            this.txtRAM.TabIndex = 2;
            // 
            // btn_auto_ram
            // 
            this.btnAutoRAM.Location = new System.Drawing.Point(21, 3);
            this.btnAutoRAM.Margin = new System.Windows.Forms.Padding(0);
            this.btnAutoRAM.Name = "btn_auto_ram";
            this.btnAutoRAM.Size = new System.Drawing.Size(100, 20);
            this.btnAutoRAM.TabIndex = 1;
            this.btnAutoRAM.Text = "RAM to test (MB):";
            this.btnAutoRAM.UseVisualStyleBackColor = true;
            this.btnAutoRAM.Click += new System.EventHandler(this.btnAutoRam_Click);
            // 
            // tab_settings
            // 
            this.tab_settings.Controls.Add(this.chkStartMin);
            this.tab_settings.Controls.Add(this.chkStopAtTotal);
            this.tab_settings.Controls.Add(this.chkStopOnError);
            this.tab_settings.Controls.Add(this.txtStopAt);
            this.tab_settings.Controls.Add(this.chkStopAt);
            this.tab_settings.Controls.Add(this.cboRows);
            this.tab_settings.Controls.Add(this.btn_center);
            this.tab_settings.Controls.Add(this.udWinHeight);
            this.tab_settings.Controls.Add(this.udYOffset);
            this.tab_settings.Controls.Add(this.lbl_win_height);
            this.tab_settings.Controls.Add(this.lbl_y_offset);
            this.tab_settings.Controls.Add(this.udYSpacing);
            this.tab_settings.Controls.Add(this.lbl_rows);
            this.tab_settings.Controls.Add(this.lbl_y_spacing);
            this.tab_settings.Controls.Add(this.udXSpacing);
            this.tab_settings.Controls.Add(this.lbl_x_spacing);
            this.tab_settings.Controls.Add(this.udXOffset);
            this.tab_settings.Controls.Add(this.lbl_x_offset);
            this.tab_settings.Location = new System.Drawing.Point(4, 22);
            this.tab_settings.Margin = new System.Windows.Forms.Padding(0);
            this.tab_settings.Name = "tab_settings";
            this.tab_settings.Size = new System.Drawing.Size(219, 356);
            this.tab_settings.TabIndex = 1;
            this.tab_settings.Text = "Settings";
            this.tab_settings.UseVisualStyleBackColor = true;
            // 
            // chk_start_min
            // 
            this.chkStartMin.AutoSize = true;
            this.chkStartMin.Checked = true;
            this.chkStartMin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStartMin.Location = new System.Drawing.Point(21, 181);
            this.chkStartMin.Name = "chk_start_min";
            this.chkStartMin.Size = new System.Drawing.Size(96, 17);
            this.chkStartMin.TabIndex = 8;
            this.chkStartMin.Text = "Start minimised";
            this.chkStartMin.UseVisualStyleBackColor = true;
            // 
            // chk_stop_at_total
            // 
            this.chkStopAtTotal.AutoSize = true;
            this.chkStopAtTotal.Enabled = false;
            this.chkStopAtTotal.Location = new System.Drawing.Point(156, 134);
            this.chkStopAtTotal.Name = "chk_stop_at_total";
            this.chkStopAtTotal.Size = new System.Drawing.Size(50, 17);
            this.chkStopAtTotal.TabIndex = 6;
            this.chkStopAtTotal.Text = "Total";
            this.chkStopAtTotal.UseVisualStyleBackColor = true;
            // 
            // chk_stop_on_err
            // 
            this.chkStopOnError.AutoSize = true;
            this.chkStopOnError.Checked = true;
            this.chkStopOnError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStopOnError.Location = new System.Drawing.Point(21, 157);
            this.chkStopOnError.Name = "chk_stop_on_err";
            this.chkStopOnError.Size = new System.Drawing.Size(87, 17);
            this.chkStopOnError.TabIndex = 4;
            this.chkStopOnError.Text = "Stop on error";
            this.chkStopOnError.UseVisualStyleBackColor = true;
            // 
            // txt_stop_at
            // 
            this.txtStopAt.Enabled = false;
            this.txtStopAt.Location = new System.Drawing.Point(100, 131);
            this.txtStopAt.Name = "txt_stop_at";
            this.txtStopAt.Size = new System.Drawing.Size(50, 20);
            this.txtStopAt.TabIndex = 5;
            // 
            // chk_stop_at
            // 
            this.chkStopAt.AutoSize = true;
            this.chkStopAt.Location = new System.Drawing.Point(21, 134);
            this.chkStopAt.Name = "chk_stop_at";
            this.chkStopAt.Size = new System.Drawing.Size(80, 17);
            this.chkStopAt.TabIndex = 4;
            this.chkStopAt.Text = "Stop at (%):";
            this.chkStopAt.UseVisualStyleBackColor = true;
            this.chkStopAt.CheckedChanged += new System.EventHandler(this.chkStopAt_CheckedChanged);
            // 
            // cbo_rows
            // 
            this.cboRows.DropDownHeight = 100;
            this.cboRows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRows.FormattingEnabled = true;
            this.cboRows.IntegralHeight = false;
            this.cboRows.Location = new System.Drawing.Point(106, 104);
            this.cboRows.Name = "cbo_rows";
            this.cboRows.Size = new System.Drawing.Size(50, 21);
            this.cboRows.TabIndex = 3;
            this.cboRows.SelectionChangeCommitted += new System.EventHandler(this.cboRows_SelectionChangeCommitted);
            // 
            // btn_center
            // 
            this.btn_center.Location = new System.Drawing.Point(121, 13);
            this.btn_center.Name = "btn_center";
            this.btn_center.Size = new System.Drawing.Size(80, 25);
            this.btn_center.TabIndex = 2;
            this.btn_center.Text = "Center";
            this.btn_center.UseVisualStyleBackColor = true;
            this.btn_center.Click += new System.EventHandler(this.btnCenter_Click);
            // 
            // ud_win_height
            // 
            this.udWinHeight.Location = new System.Drawing.Point(100, 202);
            this.udWinHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udWinHeight.Minimum = new decimal(new int[] {
            420,
            0,
            0,
            0});
            this.udWinHeight.Name = "ud_win_height";
            this.udWinHeight.Size = new System.Drawing.Size(50, 20);
            this.udWinHeight.TabIndex = 1;
            this.udWinHeight.Value = new decimal(new int[] {
            420,
            0,
            0,
            0});
            this.udWinHeight.ValueChanged += new System.EventHandler(this.udWinHeight_ValueChanged);
            // 
            // ud_y_offset
            // 
            this.udYOffset.Location = new System.Drawing.Point(66, 30);
            this.udYOffset.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udYOffset.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.udYOffset.Name = "ud_y_offset";
            this.udYOffset.Size = new System.Drawing.Size(50, 20);
            this.udYOffset.TabIndex = 1;
            this.udYOffset.ValueChanged += new System.EventHandler(this.offset_Changed);
            // 
            // lbl_win_height
            // 
            this.lbl_win_height.AutoSize = true;
            this.lbl_win_height.Location = new System.Drawing.Point(18, 204);
            this.lbl_win_height.Name = "lbl_win_height";
            this.lbl_win_height.Size = new System.Drawing.Size(81, 13);
            this.lbl_win_height.TabIndex = 0;
            this.lbl_win_height.Text = "Window height:";
            // 
            // lbl_y_offset
            // 
            this.lbl_y_offset.AutoSize = true;
            this.lbl_y_offset.Location = new System.Drawing.Point(21, 33);
            this.lbl_y_offset.Name = "lbl_y_offset";
            this.lbl_y_offset.Size = new System.Drawing.Size(46, 13);
            this.lbl_y_offset.TabIndex = 0;
            this.lbl_y_offset.Text = "Y offset:";
            // 
            // ud_y_spacing
            // 
            this.udYSpacing.Location = new System.Drawing.Point(76, 81);
            this.udYSpacing.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udYSpacing.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.udYSpacing.Name = "ud_y_spacing";
            this.udYSpacing.Size = new System.Drawing.Size(50, 20);
            this.udYSpacing.TabIndex = 1;
            this.udYSpacing.ValueChanged += new System.EventHandler(this.offset_Changed);
            // 
            // lbl_rows
            // 
            this.lbl_rows.AutoSize = true;
            this.lbl_rows.Location = new System.Drawing.Point(21, 109);
            this.lbl_rows.Name = "lbl_rows";
            this.lbl_rows.Size = new System.Drawing.Size(84, 13);
            this.lbl_rows.TabIndex = 0;
            this.lbl_rows.Text = "Number of rows:";
            // 
            // lbl_y_spacing
            // 
            this.lbl_y_spacing.AutoSize = true;
            this.lbl_y_spacing.Location = new System.Drawing.Point(21, 84);
            this.lbl_y_spacing.Name = "lbl_y_spacing";
            this.lbl_y_spacing.Size = new System.Drawing.Size(57, 13);
            this.lbl_y_spacing.TabIndex = 0;
            this.lbl_y_spacing.Text = "Y spacing:";
            // 
            // ud_x_spacing
            // 
            this.udXSpacing.Location = new System.Drawing.Point(76, 56);
            this.udXSpacing.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udXSpacing.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.udXSpacing.Name = "ud_x_spacing";
            this.udXSpacing.Size = new System.Drawing.Size(50, 20);
            this.udXSpacing.TabIndex = 1;
            this.udXSpacing.ValueChanged += new System.EventHandler(this.offset_Changed);
            // 
            // lbl_x_spacing
            // 
            this.lbl_x_spacing.AutoSize = true;
            this.lbl_x_spacing.Location = new System.Drawing.Point(21, 59);
            this.lbl_x_spacing.Name = "lbl_x_spacing";
            this.lbl_x_spacing.Size = new System.Drawing.Size(57, 13);
            this.lbl_x_spacing.TabIndex = 0;
            this.lbl_x_spacing.Text = "X spacing:";
            // 
            // ud_x_offset
            // 
            this.udXOffset.Location = new System.Drawing.Point(66, 5);
            this.udXOffset.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udXOffset.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.udXOffset.Name = "ud_x_offset";
            this.udXOffset.Size = new System.Drawing.Size(50, 20);
            this.udXOffset.TabIndex = 1;
            this.udXOffset.ValueChanged += new System.EventHandler(this.offset_Changed);
            // 
            // lbl_x_offset
            // 
            this.lbl_x_offset.AutoSize = true;
            this.lbl_x_offset.Location = new System.Drawing.Point(21, 8);
            this.lbl_x_offset.Name = "lbl_x_offset";
            this.lbl_x_offset.Size = new System.Drawing.Size(46, 13);
            this.lbl_x_offset.TabIndex = 0;
            this.lbl_x_offset.Text = "X offset:";
            // 
            // tab_about
            // 
            this.tab_about.Controls.Add(this.txt_discord);
            this.tab_about.Controls.Add(this.lbl_discord);
            this.tab_about.Controls.Add(this.lbl_version);
            this.tab_about.Location = new System.Drawing.Point(4, 22);
            this.tab_about.Margin = new System.Windows.Forms.Padding(0);
            this.tab_about.Name = "tab_about";
            this.tab_about.Size = new System.Drawing.Size(219, 356);
            this.tab_about.TabIndex = 2;
            this.tab_about.Text = "About";
            this.tab_about.UseVisualStyleBackColor = true;
            // 
            // txt_discord
            // 
            this.txt_discord.Location = new System.Drawing.Point(100, 145);
            this.txt_discord.Name = "txt_discord";
            this.txt_discord.ReadOnly = true;
            this.txt_discord.Size = new System.Drawing.Size(80, 20);
            this.txt_discord.TabIndex = 1;
            this.txt_discord.Text = "∫ntegral#7834";
            // 
            // lbl_discord
            // 
            this.lbl_discord.AutoSize = true;
            this.lbl_discord.Location = new System.Drawing.Point(50, 150);
            this.lbl_discord.Name = "lbl_discord";
            this.lbl_discord.Size = new System.Drawing.Size(46, 13);
            this.lbl_discord.TabIndex = 0;
            this.lbl_discord.Text = "Discord:";
            // 
            // lbl_version
            // 
            this.lbl_version.AutoSize = true;
            this.lbl_version.Location = new System.Drawing.Point(84, 120);
            this.lbl_version.Name = "lbl_version";
            this.lbl_version.Size = new System.Drawing.Size(69, 13);
            this.lbl_version.TabIndex = 0;
            this.lbl_version.Text = "Version 1.9.5";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 381);
            this.Controls.Add(this.tabControl);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(242, 1000);
            this.MinimumSize = new System.Drawing.Size(242, 420);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MemTestHelper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.tabControl.ResumeLayout(false);
            this.tab_main.ResumeLayout(false);
            this.tab_main.PerformLayout();
            this.tab_settings.ResumeLayout(false);
            this.tab_settings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udWinHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udYOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udYSpacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udXSpacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udXOffset)).EndInit();
            this.tab_about.ResumeLayout(false);
            this.tab_about.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tab_main;
        private System.Windows.Forms.TabPage tab_settings;
        private System.Windows.Forms.TabPage tab_about;
        private System.Windows.Forms.Button btnAutoRAM;
        private System.Windows.Forms.TextBox txtRAM;
        private System.Windows.Forms.Label lbl_num_threads;
        private System.Windows.Forms.ComboBox cboThreads;
        private System.Windows.Forms.Button btn_show;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblSpeedValue;
        private System.Windows.Forms.Label lbl_speed;
        private System.Windows.Forms.Label lblEstimatedTime;
        private System.Windows.Forms.Label lbl_estimated;
        private System.Windows.Forms.Label lblElapsedTime;
        private System.Windows.Forms.Label lbl_elapsed;
        private System.Windows.Forms.ListView lstCoverage;
        private System.Windows.Forms.ColumnHeader hdr_no;
        private System.Windows.Forms.ColumnHeader hdr_coverage;
        private System.Windows.Forms.ColumnHeader hdr_errors;
        private System.Windows.Forms.Label lbl_x_offset;
        private System.Windows.Forms.NumericUpDown udXOffset;
        private System.Windows.Forms.NumericUpDown udYOffset;
        private System.Windows.Forms.Label lbl_y_offset;
        private System.Windows.Forms.NumericUpDown udYSpacing;
        private System.Windows.Forms.Label lbl_y_spacing;
        private System.Windows.Forms.NumericUpDown udXSpacing;
        private System.Windows.Forms.Label lbl_x_spacing;
        private System.Windows.Forms.Button btn_center;
        private System.Windows.Forms.ComboBox cboRows;
        private System.Windows.Forms.Label lbl_rows;
        private System.Windows.Forms.CheckBox chkStopAtTotal;
        private System.Windows.Forms.TextBox txtStopAt;
        private System.Windows.Forms.CheckBox chkStopAt;
        private System.Windows.Forms.CheckBox chkStopOnError;
        private System.Windows.Forms.TextBox txt_discord;
        private System.Windows.Forms.Label lbl_discord;
        private System.Windows.Forms.Label lbl_version;
        private System.Windows.Forms.CheckBox chkStartMin;
        private System.Windows.Forms.Button btn_hide;
        private System.Windows.Forms.NumericUpDown udWinHeight;
        private System.Windows.Forms.Label lbl_win_height;
    }
}

