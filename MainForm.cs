using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class MainForm : Form
    {
        Sudoku sudoku;
        byte[,] data;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add(9);
            //data = new byte[,] { { 0, 0, 3, 6, 0, 0, 8, 0, 0 }, { 0, 2, 4, 0, 0, 0, 0, 0, 7 }, { 0, 0, 8, 0, 5, 9, 0, 0, 4 }, { 0, 0, 0, 0, 9, 0, 0, 0, 0 }, { 0, 0, 7, 0, 1, 4, 0, 0, 0 }, { 1, 0, 0, 0, 0, 3, 0, 7, 0 }, { 6, 0, 0, 0, 0, 0, 0, 3, 0 }, { 0, 7, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 3, 0, 5, 6, 0, 8 } };
            //data = new byte[,] { { 0, 0, 0, 8, 0, 0, 0, 0, 4 }, { 9, 0, 3, 0, 1, 0, 7, 0, 6 }, { 0, 0, 0, 0, 0, 0, 0, 0, 8 }, { 0, 5, 8, 0, 9, 0, 0, 0, 0 }, { 1, 6, 0, 2, 0, 0, 0, 0, 0 }, { 0, 2, 0, 0, 0, 0, 3, 0, 0 }, { 4, 0, 0, 5, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 3, 0, 0, 0 }, { 0, 0, 1, 0, 0, 4, 9, 0, 0 } };
            //sudoku = new Sudoku(data);
            sudoku = new Sudoku();
            //sudoku.Show(dataGridView1);
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value.ToString().Length > 1) return;
            Sudoku t = new Sudoku(sudoku);
            try
            {
                byte value = Convert.ToByte(((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value);
                sudoku[(byte)e.RowIndex, (byte)e.ColumnIndex] = value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                sudoku = t;
                ((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value = "";
            }
            //sudoku.Show((DataGridView)sender);
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            Sudoku.DFS(sudoku);
            sudoku.Show(dataGridView1);
            if (sudoku.isFinished()) MessageBox.Show("finished");
            else MessageBox.Show("failed");
        }

        private void button_Reset_Click(object sender, EventArgs e)
        {
            sudoku.Reset();
            for (byte row = 0; row < 9; row++)
                for (byte column = 0; column < 9; column++)
                {
                    dataGridView1[column, row].Value = "";
                    dataGridView1[column, row].Style.BackColor = Color.White;
                }
        }
    }
}