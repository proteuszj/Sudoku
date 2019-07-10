using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Sudoku
{
    public enum ElementValue { ONE = 1, TWO = 2, THREE = 4, FOUR = 8, FIVE = 16, SIX = 32, SEVEN = 64, EIGHT = 128, NINE = 256 };

    public static class ElementMask
    {
        public static readonly UInt16[] Value ={ 0, (UInt16)ElementValue.ONE, (UInt16)ElementValue.TWO, (UInt16)ElementValue.THREE, (UInt16)ElementValue.FOUR, (UInt16)ElementValue.FIVE, (UInt16)ElementValue.SIX, (UInt16)ElementValue.SEVEN, (UInt16)ElementValue.EIGHT, (UInt16)ElementValue.NINE };
    }

    class SudokuPosition
    {
        public readonly byte RowIndex;
        public readonly byte ColumnIndex;
        public SudokuPosition(byte row, byte column)
        {
            RowIndex = row;
            ColumnIndex = column;
        }
    }

    class StackElement
    {
        public readonly Sudoku Data;
        public readonly SudokuPosition Position;
        public readonly byte Number;
        public StackElement(Sudoku source, byte row, byte column, byte number)
        {
            Data = new Sudoku(source);
            Position = new SudokuPosition(row, column);
            Number = number;
        }
    }

    class Sudoku
    {
        private ushort[,] data;
        private bool[,] isChecked;

        public Sudoku()
        {
            data = new ushort[9, 9];
            isChecked = new bool[9, 9];
            Reset();
        }

        public Sudoku(byte[,] Value)
            : this()
        {
            for (byte row = 0; row < 9; row++)
                for (byte column = 0; column < 9; column++)
                    if (Value[row, column] != 0) this[row, column] = Value[row, column];
        }

        public Sudoku(Sudoku source)
            : this()
        {
            Copy(source);
        }

        public void Copy(Sudoku source)
        {
            for (byte row = 0; row < 9; row++)
                for (byte column = 0; column < 9; column++)
                {
                    data[row, column] = source.data[row, column];
                    isChecked[row, column] = source.isChecked[row, column];
                }
        }

        public void Reset()
        {
            for (byte row = 0; row < 9; row++)
                for (byte column = 0; column < 9; column++)
                {
                    data[row, column] = 0x1ff;
                    isChecked[row, column] = false;
                }
        }

        public static UInt16 Number2Code(byte Number)
        {
            if (Number > 0 && Number < 10) return (UInt16)(1 << (Number - 1));
            else return 0xFFFF;
        }

        public static Byte Code2Number(UInt16 Code, byte startNumber)
        {
            for (byte i = (byte)(startNumber + 1); i < 10; i++)
                if ((Number2Code(i) & Code) > 0) return i;
            return 0;
        }

        public static Byte Code2Number(UInt16 Code)
        {
            return Code2Number(Code, 0);
        }

        public static string GetNumbers(UInt16 Code)
        {
            string s = "";
            for (byte i = 1; i < 10; i++)
                if ((Number2Code(i) & Code) > 0) s += i.ToString();
            return s;
        }
        
        public byte this[byte row, byte column]
        {
            get
            {
                return Code2Number(data[row, column]);
            }
            set
            {
                SetNumber(row, column, value);
            }
        }

        public bool RemoveNumber(byte row, byte column, byte Number)
        {
            if ((data[row, column] & Number2Code(Number)) > 0)
            {
                data[row, column] -= Number2Code(Number);
                return true;
            }
            else
                return false;
        }

        private void SetNumber(byte row, byte column, byte Number)
        {
            Queue<SudokuPosition> q = new Queue<SudokuPosition>();
            data[row, column] = Number2Code(Number);
            q.Enqueue(new SudokuPosition(row, column));
            isChecked[row, column] = true;
            while (q.Count > 0)
            {
                SudokuPosition sp = q.Dequeue();
                for (byte i = 0; i < 9; i++)
                    if (i != sp.RowIndex)
                    {
                        RemoveNumber(i, sp.ColumnIndex, Code2Number(data[sp.RowIndex, sp.ColumnIndex]));
                        if (GetNumbers(data[i, sp.ColumnIndex]).Length == 0) throw new Exception("Try another number");
                        if (GetNumbers(data[i, sp.ColumnIndex]).Length == 1 && !isChecked[i, sp.ColumnIndex])
                        {
                            isChecked[i, sp.ColumnIndex] = true;
                            q.Enqueue(new SudokuPosition(i, sp.ColumnIndex));
                        }
                    }
                for (byte j = 0; j < 9; j++)
                    if (j != sp.ColumnIndex)
                    {
                        RemoveNumber(sp.RowIndex, j, Code2Number(data[sp.RowIndex, sp.ColumnIndex]));
                        if (GetNumbers(data[sp.RowIndex, j]).Length == 0) throw new Exception("Try another number");
                        if (GetNumbers(data[sp.RowIndex, j]).Length == 1 && !isChecked[sp.RowIndex, j])
                        {
                            isChecked[sp.RowIndex, j] = true;
                            q.Enqueue(new SudokuPosition(sp.RowIndex, j));
                        }
                    }
                for (byte i = (byte)(sp.RowIndex / 3 * 3); i < sp.RowIndex / 3 * 3 + 3; i++)
                    for (byte j = (byte)(sp.ColumnIndex / 3 * 3); j < sp.ColumnIndex / 3 * 3 + 3; j++)
                        if (i != sp.RowIndex || j != sp.ColumnIndex)
                        {
                            RemoveNumber(i, j, Code2Number(data[sp.RowIndex, sp.ColumnIndex]));
                            if (GetNumbers(data[i, j]).Length == 0) throw new Exception("Try another number");
                            if (GetNumbers(data[i, j]).Length == 1 && !isChecked[i, j])
                            {
                                isChecked[i, j] = true;
                                q.Enqueue(new SudokuPosition(i, j));
                            }
                        }
            }
        }

        public void Check()
        {
            UInt16[] AndEx = new ushort[9];
            UInt16 CodeToBeSet;
            bool flag = true;
            while (flag)
            {
                flag = false;
                for (byte i = 0; i < 9; i++)
                {
                    for (byte k = 0; k < 9; k++) AndEx[k] = 0;
                    for (byte j = 0; j < 9; j++)
                        for (byte k = 0; k < 9; k++) if (j != k) AndEx[k] |= data[i, j];
                    for (byte j = 0; j < 9; j++)
                    {
                        CodeToBeSet = (UInt16)(data[i, j] - (data[i, j] & AndEx[j]));
                        if (CodeToBeSet > 0 && !isChecked[i, j])
                        {
                            this[i, j] = Code2Number(CodeToBeSet);
                            flag = true;
                        }
                    }
                }
                for (byte j = 0; j < 9; j++)
                {
                    for (byte k = 0; k < 9; k++) AndEx[k] = 0;
                    for (byte i = 0; i < 9; i++)
                        for (byte k = 0; k < 9; k++) if (i != k) AndEx[k] |= data[i, j];
                    for (byte i = 0; i < 9; i++)
                    {
                        CodeToBeSet = (UInt16)(data[i, j] - (data[i, j] & AndEx[i]));
                        if (CodeToBeSet > 0 && !isChecked[i, j])
                        {
                            this[i, j] = Code2Number(CodeToBeSet);
                            flag = true;
                        }
                    }
                }
                for (byte i1 = 0; i1 < 3; i1++)
                    for (byte j1 = 0; j1 < 3; j1++)
                    {
                        for (byte k = 0; k < 9; k++) AndEx[k] = 0;
                        for (byte i2 = 0; i2 < 3; i2++)
                            for (byte j2 = 0; j2 < 3; j2++)
                                for (byte k = 0; k < 9; k++) if (i2 * 3 + j2 != k) AndEx[k] |= data[i1 * 3 + i2, j1 * 3 + j2];
                        for (byte i2 = 0; i2 < 3; i2++)
                            for (byte j2 = 0; j2 < 3; j2++)
                            {
                                CodeToBeSet = (UInt16)(data[i1 * 3 + i2, j1 * 3 + j2] - (data[i1 * 3 + i2, j1 * 3 + j2] & AndEx[i2 * 3 + j2]));
                                if (CodeToBeSet > 0 && !isChecked[i1 * 3 + i2, j1 * 3 + j2])
                                {
                                    this[(byte)(i1 * 3 + i2), (byte)(j1 * 3 + j2)] = Code2Number(CodeToBeSet);
                                    flag = true;
                                }
                            }
                    }
            }
        }

        public static void DFS(Sudoku start)
        {
            start.Check();
            Stack<StackElement> stack = new Stack<StackElement>();
            StackElement s;
            for (byte row = 0; row < 9; row++)
                for (byte column = 0; column < 9; column++)
                {
                    while (!start.isChecked[row, column])
                    {
                        stack.Push(new StackElement(start, row, column, start[row, column]));
                        //
                        try
                        {
                            start[row, column] = start[row, column];
                        }
                        catch (Exception)
                        {
                            do
                            {
                                s = stack.Pop();
                                row = s.Position.RowIndex;
                                column = s.Position.ColumnIndex;
                                start.Copy(s.Data);
                                start.RemoveNumber(row, column, start[row, column]);
                            } while (start[row, column] == 0);
                        }
                    }
                }
        }

        public void Show(DataGridView dataGridView)
        {
            for (byte row = 0; row < 9; row++)
                for (byte column = 0; column < 9; column++)
                {
                    dataGridView[column, row].Value = GetNumbers(data[row, column]);
                    dataGridView[column, row].Style.BackColor = (GetNumbers(data[row, column]).Length==1)?System.Drawing.Color.Red:System.Drawing.Color.Green;
                }
        }

        public bool isFinished()
        {
            //for (byte row = 0; row < 9; row++)
                //for (byte column = 0; column < 9; column++)
                    //if (GetNumbers(data[row, column]).Length > 1) return false;
            //return true;
            bool flag = true;
            for (byte row = 0; row < 9; row++)
                for (byte column = 0; column < 9; column++)
                    flag &= isChecked[row, column];
            return flag;
        }
    }
}