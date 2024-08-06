using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<int> GetCells()
        {
            List<int> cells = new List<int>();
            for (int i = 1; i <= 81; i++)
            {
                cells.Add(Decimal.ToInt32((Controls.Find("nud" + i, true)[0] as NumericUpDown).Value));
            }
            return cells;
        }

        private void SetCells(List<int> cells)
        {
            for (int i = 1; i <= 81; i++)
            {
                (Controls.Find("nud" + i, true)[0] as NumericUpDown).Value = cells[i - 1];
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            SetCells(Enumerable.Repeat(0, 81).ToList());
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    List<int> cells = JsonConvert.DeserializeObject<List<int>>(json);
                    if (cells != null && cells.Count == 81)
                    {
                        SetCells(cells);
                    }
                }
                catch
                {
                    MessageBox.Show("Opening file failed");
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(GetCells());
                    File.WriteAllText(saveFileDialog.FileName, json);
                }
                catch
                {
                    MessageBox.Show("Saving file failed");
                }
            }
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            List<int> cells = GetCells();
            Solver solver = new Solver(cells);
            if (solver.Validate())
            {
                List<int> solution = solver.Solve();
                SetCells(solution);
            }
            else
            {
                MessageBox.Show("Invalid game data");
            }
        }
    }
}
