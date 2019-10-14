using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Solver
{
    public static class Extensions
    {
        public static T[] GetRow<T>(this T[,] array, int row)
        {
            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            for (int i = 0; i < cols; i++)
                result[i] = array[row, i];

            return result;
        }

        public static T[] GetColumn<T>(this T[,] array, int column)
        {
            int rows = array.GetUpperBound(0) + 1;
            T[] result = new T[rows];

            for (int i = 0; i < rows; i++)
                result[i] = array[i, column];

            return result;
        }

        public static T[] GetSquare<T>(this T[,] array, (int tlX, int tlY, int brX, int brY) tpl)
             => array.GetSquare(tpl.tlX, tpl.tlY, tpl.brX, tpl.brY);

        public static T[] GetSquare<T>(this T[,] array, int tlX, int tlY, int brX, int brY)
        {
            (int h, int w) = ((brY - tlY), (brX - tlX));
            T[] result = new T[(w + 1) * (h + 1)];

            for (int i = 0; i < h + 1; i++)
                for (int j = 0; j < w + 1; j++)
                    result[i * (w + 1) + j] = array[tlY + i, tlX + j];

            return result;
        }

        public static IEnumerable<(T, U)> Cross<T, U>(this IEnumerable<T> src, IEnumerable<U> other)
        {
            foreach (var t in src)
                foreach (var u in other)
                    yield return (t, u);
        }
    }
}
