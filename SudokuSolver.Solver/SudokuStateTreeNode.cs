using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Solver
{
    internal class SudokuStateTreeNode
    {
        private static readonly int _MAX_RECURSION_DEPTH = 14;

        public SudokuState State { get; set; }
        public SudokuStateTreeNode ParentNode { get; set; }
        public List<SudokuStateTreeNode> ChildNodes { get; set; } = new List<SudokuStateTreeNode>();

        public bool IsRoot => ParentNode is null;

        public SudokuStateTreeNode(SudokuState state)
        {
            State = state;
        }

        protected SudokuStateTreeNode(SudokuState state, SudokuStateTreeNode parentNode = null, SudokuStateTreeNode[] children = null)
        {
            State = state;
            ParentNode = parentNode;
            ChildNodes = children?.ToList() ?? new List<SudokuStateTreeNode>();
        }

        public SudokuState TryGetSolution()
        {
            var recursionDepth = 1;
            return TryGetSolution_Internal(recursionDepth);
        }

        private SudokuState TryGetSolution_Internal(int recursionDepth)
        {
            if (recursionDepth > _MAX_RECURSION_DEPTH)
                return null;

            SudokuState solution;

            if (ChildNodes.Count == 0)
            {
                var keystone = State.GetKeystone();

                if (State.IsUnsolvable || keystone == null)
                    return null;

                foreach (var value in keystone.Value.vals)
                {
                    var newState = State.Clone();
                    newState.SetCellValue(keystone.Value.row, keystone.Value.col, value);
                    newState.SimpleSolve();

                    if (newState.IsSolved) return newState;
                    if (!newState.IsUnsolvable)
                        ChildNodes.Add(new SudokuStateTreeNode(newState, this));
                }
            }

            foreach (var child in ChildNodes.OrderBy(node => node.State.UnsolvedCellCount).ThenBy(node => node.State.Uncertainty))
                if ((solution = child.TryGetSolution_Internal(++recursionDepth)) != null)
                    return solution;

            return null;
        }
    }

    public class SudokuStateTree : IDisposable
    {
        private static readonly int _MAX_RECURSION_DEPTH = 20;

        public SudokuState RootState { get; set; }
        public List<SudokuState> WorkingSet { get; private set; }
        
        public SudokuStateTree(SudokuState state)
        {
            RootState = state;
        }

        public SudokuState Solve()
        {
            WorkingSet = new List<SudokuState>(new[] { RootState });

            for (int i = 0; i < _MAX_RECURSION_DEPTH; i++)
            {
                var newWorkingSet = new List<SudokuState>();

                foreach (var state in WorkingSet)
                {
                    var keystone = state.GetKeystone();
                    if (!keystone.HasValue) continue;
                    var (row, col, vals) = keystone.Value;

                    foreach (var value in vals)
                    {
                        var child = state.Clone();
                        child.SetCellValue(row, col, value);
                        child.SimpleSolve();
                        if (child.IsSolved) return child;
                        if (child.IsUnsolvable) continue;
                        newWorkingSet.Add(child);
                    }
                }

                WorkingSet = newWorkingSet; 
            }

            Console.WriteLine();

            return null;
        }

        public void Dispose() { }
    }
}
