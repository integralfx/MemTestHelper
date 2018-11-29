using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MemTestHelper
{
    public partial class FormLayout : Form
    {
        public FormLayout()
        {
            InitializeComponent();
        }

        private void FormLayout_Load(object sender, EventArgs e)
        {
            parent = (Form1)Owner;

            init_grid();
        }

        private void FormLayout_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                // re-create layout grid if necessary
                int curr_rows = is_selected.GetLength(0),
                    curr_threads = is_selected.GetLength(1),
                    new_rows = parent.get_selected_num_rows(),
                    new_threads = parent.get_selected_num_threads();

                if (curr_rows != new_rows || curr_threads != new_threads)
                {
                    dgv_layout.Rows.Clear();
                    dgv_layout.Columns.Clear();
                    init_grid();
                }
            }
        }

        // move with parent form
        private void FormLayout_Move(object sender, EventArgs e)
        {
            if (parent != null)
                parent.Location = new Point(Location.X - parent.Width, Location.Y);
        }

        private void FormLayout_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!should_close)
            {
                Hide();
                Parent = null;
                e.Cancel = true;
            }
        }

        private void dgv_layout_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var cell = (DataGridViewCheckBoxCell)dgv_layout.Rows[e.RowIndex].Cells[e.ColumnIndex];
                Boolean selected = is_selected[e.RowIndex, e.ColumnIndex];

                if (selected)
                {
                    cell.Style.BackColor = Control.DefaultBackColor;
                    is_selected[e.RowIndex, e.ColumnIndex] = false;
                }
                else
                {
                    cell.Style.BackColor = Color.Green;
                    is_selected[e.RowIndex, e.ColumnIndex] = true;
                }
            }
        }

        // fix double click issue: https://stackoverflow.com/a/15011844
        private void dgv_layout_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
                dgv_layout.EndEdit();
        }

        private void init_grid()
        {
            int threads = parent.get_selected_num_threads(),
                rows = parent.get_selected_num_rows();

            for (int i = 0; i < threads; i++)
            {
                DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn()
                {
                    Name = (i + 1).ToString(),
                    HeaderText = (i + 1).ToString(),
                    ReadOnly = false,
                    Width = 30,
                    TrueValue = true,
                    FalseValue = false
                };
                dgv_layout.Columns.Add(col);
            }

            for (int i = 0; i < rows; i++)
            {
                dgv_layout.Rows.Add();
                dgv_layout.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }

            is_selected = new Boolean[rows, threads];
        }

        private Form1 parent;
        public Boolean should_close = false;
        private Boolean[,] is_selected;
    }
}
