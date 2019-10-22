using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuSolver.Solver.Implementations.Internal
{
    internal class SudokuStateTree : IDisposable
    {
        private const int _MAX_GUESS_COUNT = 30;

        // Initial state this class was constructed with
        public SudokuState RootState { get; set; }
        // Current working set of children, all of the same generation
        public List<SudokuState> WorkingSet { get; private set; }
        
        // Ctor
        public SudokuStateTree(SudokuState state)
        {
            RootState = state;
        }

        /// <summary>
        /// Solves the puzzle based on the passed in initial state, if possible.
        /// </summary>
        /// <returns>SudokuState solution</returns>
        public SudokuState Solve()
        {
            // Create a working set to contain items of the currently considered generation
            WorkingSet = new List<SudokuState>(new[] { RootState });

            // Loop up until maximum depth
            for (int i = 0; i < _MAX_GUESS_COUNT; i++)
            {
                // Create a new working set, for the next generation
                var newWorkingSet = new ConcurrentBag<SudokuState>();

                // For each state in the current generation
                // foreach (var state in WorkingSet)
                Parallel.ForEach(WorkingSet, state =>
                {
                    // Get the keystone
                    var keystone = state.GetKeystone();

                    // Skip if it is unsolvable
                    if (!keystone.HasValue) return;

                    // Deconstruct
                    var (row, col, vals) = keystone.Value;

                    // Loop over possible keystone values
                    foreach (var value in vals)
                    {
                        // Create a child as a clone of the parent
                        var child = state.Clone();

                        // Set the value for the keystone on the new shild
                        child.SetCellValue(row, col, value);

                        // Attempt a simple solution, in case this enable a bunch of assignments
                        child.SimpleSolve();

                        // Skip keeping this child if it becomers unsolvable
                        if (!child.IsUnsolvable) 
                            // Add this child to the next generation's buffer list
                            newWorkingSet.Add(child);
                    }
                });

                var solution = newWorkingSet.FirstOrDefault(state => state.IsSolved);
                if (solution != null) return solution;

                // Set working set to be the next generation
                WorkingSet = newWorkingSet.ToList(); 
            }

            // Null if truly unsolvable?
            return null;
        }

        // I don't know why, but these prevent errors.
        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Managed resources
            }
        }
    }
}
