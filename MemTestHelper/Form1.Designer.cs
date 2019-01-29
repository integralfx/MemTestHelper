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
            this.tab_control = new System.Windows.Forms.TabControl();
            this.tab_main = new System.Windows.Forms.TabPage();
            this.btn_hide = new System.Windows.Forms.Button();
            this.lst_coverage = new System.Windows.Forms.ListView();
            this.hdr_no = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdr_coverage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hdr_errors = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbl_speed_value = new System.Windows.Forms.Label();
            this.lbl_speed = new System.Windows.Forms.Label();
            this.lbl_estimated_time = new System.Windows.Forms.Label();
            this.lbl_estimated = new System.Windows.Forms.Label();
            this.lbl_elapsed_time = new System.Windows.Forms.Label();
            this.lbl_elapsed = new System.Windows.Forms.Label();
            this.btn_show = new System.Windows.Forms.Button();
            this.btn_stop = new System.Windows.Forms.Button();
            this.btn_run = new System.Windows.Forms.Button();
            this.cbo_threads = new System.Windows.Forms.ComboBox();
            this.lbl_num_threads = new System.Windows.Forms.Label();
            this.txt_ram = new System.Windows.Forms.TextBox();
            this.btn_auto_ram = new System.Windows.Forms.Button();
            this.tab_settings = new System.Windows.Forms.TabPage();
            this.chk_start_min = new System.Windows.Forms.CheckBox();
            this.chk_stop_at_total = new System.Windows.Forms.CheckBox();
            this.chk_stop_at_err = new System.Windows.Forms.CheckBox();
            this.txt_stop_at = new System.Windows.Forms.TextBox();
            this.chk_stop_at = new System.Windows.Forms.CheckBox();
            this.cbo_rows = new System.Windows.Forms.ComboBox();
            this.btn_center = new System.Windows.Forms.Button();
            this.ud_y_offset = new System.Windows.Forms.NumericUpDown();
            this.lbl_y_offset = new System.Windows.Forms.Label();
            this.ud_y_spacing = new System.Windows.Forms.NumericUpDown();
            this.lbl_rows = new System.Windows.Forms.Label();
            this.lbl_y_spacing = new System.Windows.Forms.Label();
            this.ud_x_spacing = new System.Windows.Forms.NumericUpDown();
            this.lbl_x_spacing = new System.Windows.Forms.Label();
            this.ud_x_offset = new System.Windows.Forms.NumericUpDown();
            this.lbl_x_offset = new System.Windows.Forms.Label();
            this.tab_about = new System.Windows.Forms.TabPage();
            this.txt_discord = new System.Windows.Forms.TextBox();
            this.lbl_discord = new System.Windows.Forms.Label();
            this.lbl_version = new System.Windows.Forms.Label();
            this.tab_control.SuspendLayout();
            this.tab_main.SuspendLayout();
            this.tab_settings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ud_y_offset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_y_spacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_x_spacing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_x_offset)).BeginInit();
            this.tab_about.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab_control
            // 
            this.tab_control.Controls.Add(this.tab_main);
            this.tab_control.Controls.Add(this.tab_settings);
            this.tab_control.Controls.Add(this.tab_about);
            this.tab_control.Location = new System.Drawing.Point(0, 0);
            this.tab_control.Margin = new System.Windows.Forms.Padding(0);
            this.tab_control.Name = "tab_control";
            this.tab_control.SelectedIndex = 0;
            this.tab_control.Size = new System.Drawing.Size(227, 382);
            this.tab_control.TabIndex = 0;
            // 
            // tab_main
            // 
            this.tab_main.Controls.Add(this.btn_hide);
            this.tab_main.Controls.Add(this.lst_coverage);
            this.tab_main.Controls.Add(this.lbl_speed_value);
            this.tab_main.Controls.Add(this.lbl_speed);
            this.tab_main.Controls.Add(this.lbl_estimated_time);
            this.tab_main.Controls.Add(this.lbl_estimated);
            this.tab_main.Controls.Add(this.lbl_elapsed_time);
            this.tab_main.Controls.Add(this.lbl_elapsed);
            this.tab_main.Controls.Add(this.btn_show);
            this.tab_main.Controls.Add(this.btn_stop);
            this.tab_main.Controls.Add(this.btn_run);
            this.tab_main.Controls.Add(this.cbo_threads);
            this.tab_main.Controls.Add(this.lbl_num_threads);
            this.tab_main.Controls.Add(this.txt_ram);
            this.tab_main.Controls.Add(this.btn_auto_ram);
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
            this.btn_hide.Click += new System.EventHandler(this.btn_hide_Click);
            // 
            // lst_coverage
            // 
            this.lst_coverage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hdr_no,
            this.hdr_coverage,
            this.hdr_errors});
            this.lst_coverage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lst_coverage.Location = new System.Drawing.Point(4, 154);
            this.lst_coverage.Name = "lst_coverage";
            this.lst_coverage.Size = new System.Drawing.Size(210, 199);
            this.lst_coverage.TabIndex = 7;
            this.lst_coverage.UseCompatibleStateImageBehavior = false;
            this.lst_coverage.View = System.Windows.Forms.View.Details;
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
            this.lbl_speed_value.AutoSize = true;
            this.lbl_speed_value.Location = new System.Drawing.Point(81, 137);
            this.lbl_speed_value.Name = "lbl_speed_value";
            this.lbl_speed_value.Size = new System.Drawing.Size(54, 13);
            this.lbl_speed_value.TabIndex = 6;
            this.lbl_speed_value.Text = "0.00MB/s";
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
            this.lbl_estimated_time.AutoSize = true;
            this.lbl_estimated_time.Location = new System.Drawing.Point(81, 122);
            this.lbl_estimated_time.Name = "lbl_estimated_time";
            this.lbl_estimated_time.Size = new System.Drawing.Size(62, 13);
            this.lbl_estimated_time.TabIndex = 6;
            this.lbl_estimated_time.Text = "00h00m00s";
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
            this.lbl_elapsed_time.AutoSize = true;
            this.lbl_elapsed_time.Location = new System.Drawing.Point(81, 107);
            this.lbl_elapsed_time.Name = "lbl_elapsed_time";
            this.lbl_elapsed_time.Size = new System.Drawing.Size(62, 13);
            this.lbl_elapsed_time.TabIndex = 6;
            this.lbl_elapsed_time.Text = "00h00m00s";
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
            this.btn_show.Click += new System.EventHandler(this.btn_show_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.Enabled = false;
            this.btn_stop.Location = new System.Drawing.Point(112, 53);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(85, 25);
            this.btn_stop.TabIndex = 5;
            this.btn_stop.Text = "Stop";
            this.btn_stop.UseVisualStyleBackColor = true;
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // btn_run
            // 
            this.btn_run.Location = new System.Drawing.Point(21, 53);
            this.btn_run.Name = "btn_run";
            this.btn_run.Size = new System.Drawing.Size(85, 25);
            this.btn_run.TabIndex = 5;
            this.btn_run.Text = "Run";
            this.btn_run.UseVisualStyleBackColor = true;
            this.btn_run.Click += new System.EventHandler(this.btn_run_Click);
            // 
            // cbo_threads
            // 
            this.cbo_threads.DropDownHeight = 100;
            this.cbo_threads.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_threads.FormattingEnabled = true;
            this.cbo_threads.IntegralHeight = false;
            this.cbo_threads.Location = new System.Drawing.Point(146, 28);
            this.cbo_threads.Name = "cbo_threads";
            this.cbo_threads.Size = new System.Drawing.Size(50, 21);
            this.cbo_threads.TabIndex = 4;
            this.cbo_threads.SelectionChangeCommitted += new System.EventHandler(this.cbo_threads_SelectionChangeCommitted);
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
            this.txt_ram.Location = new System.Drawing.Point(146, 3);
            this.txt_ram.Name = "txt_ram";
            this.txt_ram.Size = new System.Drawing.Size(50, 20);
            this.txt_ram.TabIndex = 2;
            // 
            // btn_auto_ram
            // 
            this.btn_auto_ram.Location = new System.Drawing.Point(21, 3);
            this.btn_auto_ram.Margin = new System.Windows.Forms.Padding(0);
            this.btn_auto_ram.Name = "btn_auto_ram";
            this.btn_auto_ram.Size = new System.Drawing.Size(100, 20);
            this.btn_auto_ram.TabIndex = 1;
            this.btn_auto_ram.Text = "RAM to test (MB):";
            this.btn_auto_ram.UseVisualStyleBackColor = true;
            this.btn_auto_ram.Click += new System.EventHandler(this.btn_auto_ram_Click);
            // 
            // tab_settings
            // 
            this.tab_settings.Controls.Add(this.chk_start_min);
            this.tab_settings.Controls.Add(this.chk_stop_at_total);
            this.tab_settings.Controls.Add(this.chk_stop_at_err);
            this.tab_settings.Controls.Add(this.txt_stop_at);
            this.tab_settings.Controls.Add(this.chk_stop_at);
            this.tab_settings.Controls.Add(this.cbo_rows);
            this.tab_settings.Controls.Add(this.btn_center);
            this.tab_settings.Controls.Add(this.ud_y_offset);
            this.tab_settings.Controls.Add(this.lbl_y_offset);
            this.tab_settings.Controls.Add(this.ud_y_spacing);
            this.tab_settings.Controls.Add(this.lbl_rows);
            this.tab_settings.Controls.Add(this.lbl_y_spacing);
            this.tab_settings.Controls.Add(this.ud_x_spacing);
            this.tab_settings.Controls.Add(this.lbl_x_spacing);
            this.tab_settings.Controls.Add(this.ud_x_offset);
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
            this.chk_start_min.AutoSize = true;
            this.chk_start_min.Checked = true;
            this.chk_start_min.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_start_min.Location = new System.Drawing.Point(21, 181);
            this.chk_start_min.Name = "chk_start_min";
            this.chk_start_min.Size = new System.Drawing.Size(96, 17);
            this.chk_start_min.TabIndex = 8;
            this.chk_start_min.Text = "Start minimised";
            this.chk_start_min.UseVisualStyleBackColor = true;
            // 
            // chk_stop_at_total
            // 
            this.chk_stop_at_total.AutoSize = true;
            this.chk_stop_at_total.Enabled = false;
            this.chk_stop_at_total.Location = new System.Drawing.Point(156, 134);
            this.chk_stop_at_total.Name = "chk_stop_at_total";
            this.chk_stop_at_total.Size = new System.Drawing.Size(50, 17);
            this.chk_stop_at_total.TabIndex = 6;
            this.chk_stop_at_total.Text = "Total";
            this.chk_stop_at_total.UseVisualStyleBackColor = true;
            // 
            // chk_stop_at_err
            // 
            this.chk_stop_at_err.AutoSize = true;
            this.chk_stop_at_err.Checked = true;
            this.chk_stop_at_err.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_stop_at_err.Location = new System.Drawing.Point(21, 157);
            this.chk_stop_at_err.Name = "chk_stop_at_err";
            this.chk_stop_at_err.Size = new System.Drawing.Size(87, 17);
            this.chk_stop_at_err.TabIndex = 4;
            this.chk_stop_at_err.Text = "Stop on error";
            this.chk_stop_at_err.UseVisualStyleBackColor = true;
            // 
            // txt_stop_at
            // 
            this.txt_stop_at.Enabled = false;
            this.txt_stop_at.Location = new System.Drawing.Point(116, 131);
            this.txt_stop_at.Name = "txt_stop_at";
            this.txt_stop_at.Size = new System.Drawing.Size(35, 20);
            this.txt_stop_at.TabIndex = 5;
            // 
            // chk_stop_at
            // 
            this.chk_stop_at.AutoSize = true;
            this.chk_stop_at.Location = new System.Drawing.Point(21, 134);
            this.chk_stop_at.Name = "chk_stop_at";
            this.chk_stop_at.Size = new System.Drawing.Size(80, 17);
            this.chk_stop_at.TabIndex = 4;
            this.chk_stop_at.Text = "Stop at (%):";
            this.chk_stop_at.UseVisualStyleBackColor = true;
            this.chk_stop_at.CheckedChanged += new System.EventHandler(this.chk_stop_at_CheckedChanged);
            // 
            // cbo_rows
            // 
            this.cbo_rows.DropDownHeight = 100;
            this.cbo_rows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_rows.FormattingEnabled = true;
            this.cbo_rows.IntegralHeight = false;
            this.cbo_rows.Location = new System.Drawing.Point(106, 104);
            this.cbo_rows.Name = "cbo_rows";
            this.cbo_rows.Size = new System.Drawing.Size(50, 21);
            this.cbo_rows.TabIndex = 3;
            this.cbo_rows.SelectionChangeCommitted += new System.EventHandler(this.cbo_rows_SelectionChangeCommitted);
            // 
            // btn_center
            // 
            this.btn_center.Location = new System.Drawing.Point(121, 13);
            this.btn_center.Name = "btn_center";
            this.btn_center.Size = new System.Drawing.Size(80, 25);
            this.btn_center.TabIndex = 2;
            this.btn_center.Text = "Center";
            this.btn_center.UseVisualStyleBackColor = true;
            this.btn_center.Click += new System.EventHandler(this.btn_center_Click);
            // 
            // ud_y_offset
            // 
            this.ud_y_offset.Location = new System.Drawing.Point(66, 30);
            this.ud_y_offset.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ud_y_offset.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.ud_y_offset.Name = "ud_y_offset";
            this.ud_y_offset.Size = new System.Drawing.Size(50, 20);
            this.ud_y_offset.TabIndex = 1;
            this.ud_y_offset.ValueChanged += new System.EventHandler(this.offset_changed);
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
            this.ud_y_spacing.Location = new System.Drawing.Point(76, 81);
            this.ud_y_spacing.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ud_y_spacing.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.ud_y_spacing.Name = "ud_y_spacing";
            this.ud_y_spacing.Size = new System.Drawing.Size(50, 20);
            this.ud_y_spacing.TabIndex = 1;
            this.ud_y_spacing.ValueChanged += new System.EventHandler(this.offset_changed);
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
            this.ud_x_spacing.Location = new System.Drawing.Point(76, 56);
            this.ud_x_spacing.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ud_x_spacing.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.ud_x_spacing.Name = "ud_x_spacing";
            this.ud_x_spacing.Size = new System.Drawing.Size(50, 20);
            this.ud_x_spacing.TabIndex = 1;
            this.ud_x_spacing.ValueChanged += new System.EventHandler(this.offset_changed);
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
            this.ud_x_offset.Location = new System.Drawing.Point(66, 5);
            this.ud_x_offset.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ud_x_offset.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.ud_x_offset.Name = "ud_x_offset";
            this.ud_x_offset.Size = new System.Drawing.Size(50, 20);
            this.ud_x_offset.TabIndex = 1;
            this.ud_x_offset.ValueChanged += new System.EventHandler(this.offset_changed);
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
            this.lbl_version.Size = new System.Drawing.Size(60, 13);
            this.lbl_version.TabIndex = 0;
            this.lbl_version.Text = "Version 1.9";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 381);
            this.Controls.Add(this.tab_control);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MemTestHelper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.tab_control.ResumeLayout(false);
            this.tab_main.ResumeLayout(false);
            this.tab_main.PerformLayout();
            this.tab_settings.ResumeLayout(false);
            this.tab_settings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ud_y_offset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_y_spacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_x_spacing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_x_offset)).EndInit();
            this.tab_about.ResumeLayout(false);
            this.tab_about.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tab_control;
        private System.Windows.Forms.TabPage tab_main;
        private System.Windows.Forms.TabPage tab_settings;
        private System.Windows.Forms.TabPage tab_about;
        private System.Windows.Forms.Button btn_auto_ram;
        private System.Windows.Forms.TextBox txt_ram;
        private System.Windows.Forms.Label lbl_num_threads;
        private System.Windows.Forms.ComboBox cbo_threads;
        private System.Windows.Forms.Button btn_show;
        private System.Windows.Forms.Button btn_stop;
        private System.Windows.Forms.Button btn_run;
        private System.Windows.Forms.Label lbl_speed_value;
        private System.Windows.Forms.Label lbl_speed;
        private System.Windows.Forms.Label lbl_estimated_time;
        private System.Windows.Forms.Label lbl_estimated;
        private System.Windows.Forms.Label lbl_elapsed_time;
        private System.Windows.Forms.Label lbl_elapsed;
        private System.Windows.Forms.ListView lst_coverage;
        private System.Windows.Forms.ColumnHeader hdr_no;
        private System.Windows.Forms.ColumnHeader hdr_coverage;
        private System.Windows.Forms.ColumnHeader hdr_errors;
        private System.Windows.Forms.Label lbl_x_offset;
        private System.Windows.Forms.NumericUpDown ud_x_offset;
        private System.Windows.Forms.NumericUpDown ud_y_offset;
        private System.Windows.Forms.Label lbl_y_offset;
        private System.Windows.Forms.NumericUpDown ud_y_spacing;
        private System.Windows.Forms.Label lbl_y_spacing;
        private System.Windows.Forms.NumericUpDown ud_x_spacing;
        private System.Windows.Forms.Label lbl_x_spacing;
        private System.Windows.Forms.Button btn_center;
        private System.Windows.Forms.ComboBox cbo_rows;
        private System.Windows.Forms.Label lbl_rows;
        private System.Windows.Forms.CheckBox chk_stop_at_total;
        private System.Windows.Forms.TextBox txt_stop_at;
        private System.Windows.Forms.CheckBox chk_stop_at;
        private System.Windows.Forms.CheckBox chk_stop_at_err;
        private System.Windows.Forms.TextBox txt_discord;
        private System.Windows.Forms.Label lbl_discord;
        private System.Windows.Forms.Label lbl_version;
        private System.Windows.Forms.CheckBox chk_start_min;
        private System.Windows.Forms.Button btn_hide;
    }
}

