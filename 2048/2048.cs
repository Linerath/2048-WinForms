using System;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Game_2048
{
    public delegate void EndGameEventHandler();
    public delegate void ScoreOverflowEventHandler();
    public delegate void NewTileEventHandler(int x, int y);

    public sealed class _2048
    {
        public event EndGameEventHandler EndGameEvent;
        public event ScoreOverflowEventHandler ScoreOverflowEvent;
        public event NewTileEventHandler NewTileEvent;

        private readonly int _minValue;   // минимально выпадающее значение
        private int _score;                // очки
        private int[,] _matrix;
        LimitedStack<int[,]> history;     // ограниченный стек действий
        LimitedStack<int> scoreHistory;   // ограниченный стек очков
        private int historyMaxLength = 5; // количество разрешенных отмен операций

        public _2048(int rows, int cells, int minValue = 2)
        {
            if (rows < 2 || cells < 2)
                throw new ArgumentException("Размер матрицы не может быть меньше 2х2");

            _minValue = minValue;
            New(rows, cells);
        }

        public bool TrySaveGame(string file)
        {
            if (file == null || !File.Exists(file)) return false;
            try
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream(file, FileMode.Create)))
                {
                    bw.Write(_minValue);
                    bw.Write(_score);
                    bw.Write(_matrix.GetLength(0));
                    bw.Write(_matrix.GetLength(1));
                    foreach (int i in _matrix)
                        bw.Write(i);
                    bw.Write(historyMaxLength);

                    return true;
                }
            }
            catch (IOException)
            {

                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool TryLoadGame(string file)
        {
            if (file == null || !File.Exists(file)) return false;
            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream(file, FileMode.OpenOrCreate)))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Генерация нового поля.
        public void New(int rows, int cells)
        {
            Random random = new Random();
            _matrix = new int[rows, cells];
            int count = random.Next(1, 5);

            while (count != 0)
            {
                NewElement();
                count--;
            }
            history = new LimitedStack<int[,]>(historyMaxLength);
            scoreHistory = new LimitedStack<int>(historyMaxLength);
        }
        // Генерация нового числа.
        private void NewElement()
        {
            Random random = new Random();
            int size = _matrix.Length;
            int rows = _matrix.GetLength(0);
            int cells = _matrix.GetLength(1);

            int x = random.Next(0, rows);
            int y = random.Next(0, cells);

            while (_matrix[x, y] != 0)
            {
                x = random.Next(0, rows);
                y = random.Next(0, cells);
            }
            if (random.Next(0, 10) == 1)
                _matrix[x, y] = _minValue + _minValue;
            else
                _matrix[x, y] = _minValue;

            if (NewTileEvent != null)
                NewTileEvent(x, y);

            if (!_matrixOperations.CanBeMoved(_matrix))
                EndGameEvent();
        }

        public int[,] GetMatrix()
        {
            int[,] temporary = new int[_matrix.GetLength(0), _matrix.GetLength(1)];
            Array.Copy(_matrix, temporary, _matrix.Length);
            return temporary;
        }
        public int GetScore()
        {
            return _score; 
        }

        public bool TryMoveDown()
        {
            return Move(down: true);
        }
        public bool TryMoveUp()
        {
            return Move(up: true);
        }
        public bool TryMoveLeft()
        {
            return Move(left: true);
        }
        public bool TryMoveRight()
        {
            return Move(right: true);
        }
        // *В РАЗРАБОТКЕ*
        private bool Move(bool down = false, bool up = false, bool left = false, bool right = false)
        {
            if (!down && !up && !left && !right) throw new ArgumentException("1 argument must be true.");

            int rows = _matrix.GetLength(0);
            int cells = _matrix.GetLength(1);
            int moveScore = 0;
            bool oneMoved = false;

            int originalScore = _score;
            int[,] original_matrix = new int[rows, cells];
            Array.Copy(_matrix, original_matrix, _matrix.Length);
            int untill;
            if (down || up) untill = cells;
            else untill = rows;
            for (int i = 0; i < untill; i++)
            {
                int[] line = null;
                int lineScore = 0;
                if (down)
                    line = _matrixOperations.SelectRow(_matrix, 0, i, rows - 1, i);
                else if (up)
                    line = _matrixOperations.SelectRow(_matrix, rows - 1, i, 0, i);
                else if (left)
                    line = _matrixOperations.SelectRow(_matrix, i, cells - 1, i, 0);
                else if (right)
                    line = _matrixOperations.SelectRow(_matrix, i, 0, i, cells - 1);
                try
                {
                    lineScore = _matrixOperations.LineShiftAttempt(line);
                }
                catch (OverflowException)
                {
                    if (ScoreOverflowEvent != null)
                        ScoreOverflowEvent();
                }
                if (lineScore >= 0)
                {
                    oneMoved = true;
                    if (down)
                        _matrixOperations.SetLine(_matrix, line, 0, i, rows - 1, i);
                    else if (up)
                        _matrixOperations.SetLine(_matrix, line, rows - 1, i, 0, i);
                    else if (left)
                        _matrixOperations.SetLine(_matrix, line, i, cells - 1, i, 0);
                    else if (right)
                        _matrixOperations.SetLine(_matrix, line, i, 0, i, cells - 1);
                    moveScore += lineScore;
                }
            }
            if (oneMoved)
            {
                try
                {
                    _score += moveScore;
                }
                catch (OverflowException)
                {
                    if (ScoreOverflowEvent != null)
                        ScoreOverflowEvent();
                }
                history.Push(original_matrix);
                scoreHistory.Push(originalScore);
                NewElement();
                return true;
            }
            else return false;
        }

        public bool UndoAllowed()
        {
            return history.Count == 0 ? false : true;
        }
        public void Undo()
        {
            if (history.Count == 0)
                return;

            _matrix = history.Pop();
            _score= scoreHistory.Pop();
        }
    }

    /*Некоторые операции с матрицей*/
    public static class _matrixOperations
    {
        // Выделение строки матрицы по заданным координатам.
        // *В РАЗРАБОТКЕ*
        public static int[] SelectRow(int[,] _matrix, int x1, int y1, int x2, int y2)
        {
            if (_matrix == null) return null;

            if (x1 == x2)
            {
                int[] row = new int[Math.Abs(y2 - y1) + 1];
                if (y1 > y2)
                {
                    for (int i = y1, j = 0; i >= y2; i--, j++)
                    {
                        row[j] = _matrix[x1, i];
                    }
                }
                else
                {
                    for (int i = y1, j = 0; i <= y2; i++, j++)
                    {
                        row[j] = _matrix[x1, i];
                    }
                }
                return row;
            }
            else if (y1 == y2)
            {
                int[] row = new int[Math.Abs(x2 - x1) + 1];
                if (x1 > x2)
                {
                    for (int i = x1, j = 0; i >= x2; i--, j++)
                    {
                        row[j] = _matrix[i, y1];
                    }
                }
                else
                {
                    for (int i = x1, j = 0; i <= x2; i++, j++)
                    {
                        row[j] = _matrix[i, y1];
                    }
                }
                return row;
            }
            else
                throw new ArgumentException("Передаваемые координаты должны образовывать строку.");
        }

        // Установка строки матрицы.
        // *В РАЗРАБОТКЕ*
        public static void SetLine(int[,] _matrix, int[] line, int x1, int y1, int x2, int y2)
        {
            if (line == null) return;

            if (x1 == x2)
            {
                if (y1 > y2)
                {
                    for (int i = y1, j = 0; i >= y2; i--, j++)
                    {
                        _matrix[x1, i] = line[j];
                    }
                }
                else
                {
                    for (int i = y1, j = 0; i <= y2; i++, j++)
                    {
                        _matrix[x1, i] = line[j];
                    }
                }
            }
            else if (y1 == y2)
            {
                if (x1 > x2)
                {
                    for (int i = x1, j = 0; i >= x2; i--, j++)
                    {
                        _matrix[i, y1] = line[j];
                    }
                }
                else
                {
                    for (int i = x1, j = 0; i <= x2; i++, j++)
                    {
                        _matrix[i, y1] = line[j];
                    }
                }
            }
            else
                throw new ArgumentException("Передаваемые координаты должны образовывать строку.");
        }

        // Сдвиг одной линии матрицы по правилам игры.
        public static int LineShiftAttempt(int[] line)
        {
            if (line == null || line.Length < 2)
                throw new ArgumentException("Aray length must be more than 1");

            bool[] connected = new bool[line.Length];
            bool moved = false;
            int score = 0;

            for (int i = 0; i < line.Length; i++)
            {
                for (int j = line.Length - 1; j > 0; j--)
                {
                    if (line[j - 1] != 0 && line[j - 1] == line[j] && !connected[j] && !connected[j - 1])
                    {
                        line[j] = checked(line[j] + line[j]);
                        line[j - 1] = 0;
                        score += line[j];
                        connected[j] = true;
                        moved = true;
                    }
                    else if (line[j - 1] != 0 && line[j] == 0)
                    {
                        line[j] = line[j - 1];
                        line[j - 1] = 0;
                        moved = true;
                    }
                }
            }
            return moved ? score : -1;
        }

        // Можно ли сдвинуть матрицу в какую-либо сторону по правилам игры.
        // *В РАЗРАБОТКЕ*
        public static bool CanBeMoved(int[,] _matrix)
        {
            if (_matrix == null) return false;

            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    if ((i > 0 && ((_matrix[i - 1, j] == _matrix[i, j]) || _matrix[i - 1, j] == 0)) ||
                        (i < _matrix.GetLength(0) - 1 && ((_matrix[i + 1, j] == _matrix[i, j]) || _matrix[i + 1, j] == 0)) ||
                        (j > 0 && ((_matrix[i, j - 1] == _matrix[i, j]) || _matrix[i, j - 1] == 0)) ||
                        (j < _matrix.GetLength(1) - 1 && ((_matrix[i, j + 1] == _matrix[i, j]) || _matrix[i, j + 1] == 0)))
                        return true;
                }
            }
            return false;
        }
    }

    /*Медленная реализация ограниченного стека*/
    public class LimitedStack<T> : Stack<T>
    {
        private int _maxLength;

        public LimitedStack(int maxLength)
        {
            _maxLength = maxLength;
        }

        new public void Push(T element)
        {
            if (this.Count == _maxLength)
            {
                List<T> tempList = this.ToList<T>();
                for (int i = tempList.Count - 1; i > 0; i--)
                {
                    tempList[i] = tempList[i - 1];
                }
                for (int i = this.Count - 1; i > 0; i--)
                {
                    tempList[i] = tempList[i - 1];
                }
                tempList[0] = element;
                this.Clear();
                for (int i = tempList.Count - 1; i >= 0; i--)
                {
                    base.Push(tempList[i]);
                }

                //T[] newStack = this.ToArray();
                //for (int i = this.Count - 1; i > 0; i--)
                //{
                //    newStack[i] = newStack[i - 1];
                //}
                //newStack[0] = element;
                //this.Clear();
                //for (int i = newStack.Length - 1; i >= 0; i--)
                //{
                //    base.Push(newStack[i]);
                //}
            }
            else
                base.Push(element);
        }
    }
}