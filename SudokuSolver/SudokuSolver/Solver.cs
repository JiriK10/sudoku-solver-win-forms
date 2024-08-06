using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    internal class Solver
    {
        /// <summary>
        /// Cells with values of Sudoku
        /// </summary>
        private List<int> _cells;

        /// <summary>
        /// Indexes of cells that are empty -> needs solving
        /// </summary>
        private List<int> _cellsToSolve;

        public Solver(List<int> cells)
        {
            _cells = cells;
        }

        private int GetRowIdx(int cellIdx) =>  (int)Math.Floor(cellIdx / 9d);
        private int GetColumnIdx(int cellIdx) => cellIdx % 9;
        private int GetBoxIdx(int cellIdx) => (int)Math.Floor(cellIdx / 27d) * 3 + (int)Math.Floor((cellIdx % 9) / 3d);

        private bool ValidateRow(int rowIdx)
        {
            return _cells
                .Skip(rowIdx * 9)
                .Take(9)
                .Where(c => c > 0)
                .GroupBy(c => c)
                .Where(g => g.Count() > 1)
                .Count() == 0;
        }

        private bool ValidateColumn(int colIdx)
        {
            return _cells
                .Where((cell, idx) => cell > 0 && GetColumnIdx(idx) == colIdx)
                .GroupBy(c => c)
                .Where(g => g.Count() > 1)              
                .Count() == 0;
        }

        private bool ValidateBox(int boxIdx)
        {
            return _cells
                .Where((cell, idx) => cell > 0 && GetBoxIdx(idx) == boxIdx)
                .GroupBy(c => c)
                .Where(g => g.Count() > 1)
                .Count() == 0;
        }

        public bool Validate()
        {
            for (int i = 0; i < 9; i++)
            {
                if (!ValidateRow(i) || !ValidateColumn(i) || !ValidateBox(i))
                    return false;
            }
            return true;
        }

        public List<int> Solve()
        {
            // Find cells to solve
            _cellsToSolve = new List<int>();
            for (int i = 0; i < _cells.Count; i++)
            {
                if (_cells[i] == 0)
                {
                    _cellsToSolve.Add(i);
                }
            }

            // Start solving
            if (_cellsToSolve.Count > 0)
            {
                SolveCell(0);
            }

            return _cells;
        }

        private bool SolveCell(int cellToSolveIdx)
        {
            int cellIdx = _cellsToSolve[cellToSolveIdx];
            int rowIdx = GetRowIdx(cellIdx);
            int colIdx = GetColumnIdx(cellIdx);
            int boxIdx = GetBoxIdx(cellIdx);

            // Find available values for cell (currently aren't in same row, column or 3x3 box)
            List<int> usedValues = _cells
                .Where((cell, idx) => cell > 0 && (GetColumnIdx(idx) == colIdx || GetRowIdx(idx) == rowIdx || GetBoxIdx(idx) == boxIdx))
                .Distinct()
                .ToList();
            List<int> availableValues = Enumerable.Range(1, 9)
                .Except(usedValues)
                .ToList();

            // Solve for available values
            foreach (int value in availableValues)
            {
                _cells[cellIdx] = value;
                if (cellToSolveIdx + 1 >= _cellsToSolve.Count || SolveCell(cellToSolveIdx + 1))
                {
                    return true;
                }
            }

            // Wrong "solving path"
            _cells[cellIdx] = 0;
            return false;
        }
    }
}
