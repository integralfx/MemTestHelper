namespace MemTestHelper
{
    partial class FormLayout
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv_layout = new System.Windows.Forms.DataGridView();
            this.btn_up = new System.Windows.Forms.Button();
            this.btn_left = new System.Windows.Forms.Button();
            this.btn_down = new System.Windows.Forms.Button();
            this.btn_right = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_layout)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_layout
            // 
            this.dgv_layout.AllowUserToAddRows = false;
            this.dgv_layout.AllowUserToDeleteRows = false;
            this.dgv_layout.AllowUserToResizeColumns = false;
            this.dgv_layout.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_layout.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_layout.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_layout.Location = new System.Drawing.Point(12, 12);
            this.dgv_layout.MultiSelect = false;
            this.dgv_layout.Name = "dgv_layout";
            this.dgv_layout.RowHeadersWidth = 50;
            this.dgv_layout.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_layout.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgv_layout.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgv_layout.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_layout.ShowCellErrors = false;
            this.dgv_layout.ShowCellToolTips = false;
            this.dgv_layout.ShowEditingIcon = false;
            this.dgv_layout.ShowRowErrors = false;
            this.dgv_layout.Size = new System.Drawing.Size(266, 150);
            this.dgv_layout.TabIndex = 0;
            this.dgv_layout.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgv_layout_CellMouseUp);
            this.dgv_layout.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_layout_CellValueChanged);
            // 
            // btn_up
            // 
            this.btn_up.Location = new System.Drawing.Point(107, 168);
            this.btn_up.Name = "btn_up";
            this.btn_up.Size = new System.Drawing.Size(75, 23);
            this.btn_up.TabIndex = 1;
            this.btn_up.Text = "Up";
            this.btn_up.UseVisualStyleBackColor = true;
            // 
            // btn_left
            // 
            this.btn_left.Location = new System.Drawing.Point(12, 201);
            this.btn_left.Name = "btn_left";
            this.btn_left.Size = new System.Drawing.Size(75, 23);
            this.btn_left.TabIndex = 2;
            this.btn_left.Text = "Left";
            this.btn_left.UseVisualStyleBackColor = true;
            // 
            // btn_down
            // 
            this.btn_down.Location = new System.Drawing.Point(107, 236);
            this.btn_down.Name = "btn_down";
            this.btn_down.Size = new System.Drawing.Size(75, 23);
            this.btn_down.TabIndex = 3;
            this.btn_down.Text = "Down";
            this.btn_down.UseVisualStyleBackColor = true;
            // 
            // btn_right
            // 
            this.btn_right.Location = new System.Drawing.Point(203, 201);
            this.btn_right.Name = "btn_right";
            this.btn_right.Size = new System.Drawing.Size(75, 23);
            this.btn_right.TabIndex = 4;
            this.btn_right.Text = "Right";
            this.btn_right.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(107, 201);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 5;
            this.button5.Text = "Center";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // FormLayout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 271);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.btn_right);
            this.Controls.Add(this.btn_down);
            this.Controls.Add(this.btn_left);
            this.Controls.Add(this.btn_up);
            this.Controls.Add(this.dgv_layout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLayout";
            this.Text = "Layout";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLayout_FormClosing);
            this.Load += new System.EventHandler(this.FormLayout_Load);
            this.VisibleChanged += new System.EventHandler(this.FormLayout_VisibleChanged);
            this.Move += new System.EventHandler(this.FormLayout_Move);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_layout)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_layout;
        private System.Windows.Forms.Button btn_up;
        private System.Windows.Forms.Button btn_left;
        private System.Windows.Forms.Button btn_down;
        private System.Windows.Forms.Button btn_right;
        private System.Windows.Forms.Button button5;
    }
}