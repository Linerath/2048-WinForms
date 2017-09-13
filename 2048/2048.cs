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
    public delegate void NewTileEventHandler(Int32 x, Int32 y);

    public sealed class _2048
    {
        public event EndGameEventHandler EndGameEvent;
        public event ScoreOverflowEventHandler ScoreOverflowEvent;
        public event NewTileEventHandler NewTileEvent;

        private Int32 _minValue;             // минимально выпадающее значение
        private Int32 _score;                // очки
        private Int32[,] _matrix;
        private LimitedStack<Int32[,]> _history;     // ограниченный стек действий
        private LimitedStack<Int32> _scoreHistory;   // ограниченный стек очков
        private Int32 _historyMaxLength = 5; // количество разрешенных отмен операций

        public _2048(Int32 rows, Int32 cells, Int32 minValue = 2)
        {
            if (rows < 2 || cells < 2)
                throw new ArgumentException("Размер матрицы не может быть меньше 2х2.");

            _minValue = minValue;
            New(rows, cells);
        }

        public Boolean TrySaveGame(String file)
        {
            if (file == null || !File.Exists(file)) return false;
            try
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream(file, FileMode.Truncate)))
                {
                    bw.Write(_minValue);
                    bw.Write(_score);
                    bw.Write(_matrix.GetLength(0));
                    bw.Write(_matrix.GetLength(1));
                    foreach (Int32 i in _matrix)
                        bw.Write(i);
                    bw.Write(_historyMaxLength);

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

        public Boolean TryLoadGame(String file)
        {
            if (file == null || !File.Exists(file)) return false;
            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream(file, FileMode.OpenOrCreate)))
                {
                    _minValue = br.ReadInt32();
                    _score = br.ReadInt32();
                    _matrix = new Int32[br.ReadInt32(), br.ReadInt32()];
                    for (Int32 i =0; i < _matrix.GetLength(0); i++)
                    {
                        for (Int32 j = 0; j < _matrix.GetLength(1); j++)
                        {
                            _matrix[i, j] = br.ReadInt32();
                        }
                    }
                    _historyMaxLength = br.ReadInt32();
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
        public void New(Int32 rows, Int32 cells)
        {
            Random random = new Random();
            _matrix = new Int32[rows, cells];
            Int32 count = random.Next(1, 5);

            while (count != 0)
            {
                NewElement();
                count--;
            }
            _history = new LimitedStack<Int32[,]>(_historyMaxLength);
            _scoreHistory = new LimitedStack<Int32>(_historyMaxLength);
        }
        // Генерация нового числа.
        private void NewElement()
        {
            Random random = new Random();
            Int32 size = _matrix.Length;
            Int32 rows = _matrix.GetLength(0);
            Int32 cells = _matrix.GetLength(1);

            Int32 x = random.Next(0, rows);
            Int32 y = random.Next(0, cells);

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

        public Int32[,] GetMatrix()
        {
            Int32[,] temporary = new Int32[_matrix.GetLength(0), _matrix.GetLength(1)];
            Array.Copy(_matrix, temporary, _matrix.Length);
            return temporary;
        }
        public Int32 Score => _score;

        public Boolean TryMoveDown()
        {
            return Move(down: true);
        }
        public Boolean TryMoveUp()
        {
            return Move(up: true);
        }
        public Boolean TryMoveLeft()
        {
            return Move(left: true);
        }
        public Boolean TryMoveRight()
        {
            return Move(right: true);
        }
        // *В РАЗРАБОТКЕ*
        private Boolean Move(Boolean down = false, Boolean up = false, Boolean left = false, Boolean right = false)
        {
            if (!down && !up && !left && !right) throw new ArgumentException("1 argument must be true.");

            Int32 rows = _matrix.GetLength(0);
            Int32 cells = _matrix.GetLength(1);
            Int32 moveScore = 0;
            Boolean oneMoved = false;

            Int32 originalScore = _score;
            Int32[,] original_matrix = new Int32[rows, cells];
            Array.Copy(_matrix, original_matrix, _matrix.Length);
            Int32 untill;
            if (down || up) untill = cells;
            else untill = rows;
            for (Int32 i = 0; i < untill; i++)
            {
                Int32[] line = null;
                Int32 lineScore = 0;
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
                    ScoreOverflowEvent?.Invoke();
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
                    ScoreOverflowEvent?.Invoke();
                }
                _history.Push(original_matrix);
                _scoreHistory.Push(originalScore);
                NewElement();
                return true;
            }
            return false;
        }

        public Boolean UndoAllowed()
        {
            return _history.Count == 0 ? false : true;
        }
        public void Undo()
        {
            if (_history.Count == 0)
                return;

            _matrix = _history.Pop();
            _score = _scoreHistory.Pop();
        }
    }

    /*Некоторые операции с матрицей*/
    public static class _matrixOperations
    {
        // Выделение строки матрицы по заданным координатам.
        // *В РАЗРАБОТКЕ*
        public static Int32[] SelectRow(Int32[,] _matrix, Int32 x1, Int32 y1, Int32 x2, Int32 y2)
        {
            if (_matrix == null) return null;

            if (x1 == x2)
            {
                Int32[] row = new Int32[Math.Abs(y2 - y1) + 1];
                if (y1 > y2)
                {
                    for (Int32 i = y1, j = 0; i >= y2; i--, j++)
                    {
                        row[j] = _matrix[x1, i];
                    }
                }
                else
                {
                    for (Int32 i = y1, j = 0; i <= y2; i++, j++)
                    {
                        row[j] = _matrix[x1, i];
                    }
                }
                return row;
            }
            else if (y1 == y2)
            {
                Int32[] row = new Int32[Math.Abs(x2 - x1) + 1];
                if (x1 > x2)
                {
                    for (Int32 i = x1, j = 0; i >= x2; i--, j++)
                    {
                        row[j] = _matrix[i, y1];
                    }
                }
                else
                {
                    for (Int32 i = x1, j = 0; i <= x2; i++, j++)
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
        public static void SetLine(Int32[,] _matrix, Int32[] line, Int32 x1, Int32 y1, Int32 x2, Int32 y2)
        {
            if (line == null) return;

            if (x1 == x2)
            {
                if (y1 > y2)
                {
                    for (Int32 i = y1, j = 0; i >= y2; i--, j++)
                    {
                        _matrix[x1, i] = line[j];
                    }
                }
                else
                {
                    for (Int32 i = y1, j = 0; i <= y2; i++, j++)
                    {
                        _matrix[x1, i] = line[j];
                    }
                }
            }
            else if (y1 == y2)
            {
                if (x1 > x2)
                {
                    for (Int32 i = x1, j = 0; i >= x2; i--, j++)
                    {
                        _matrix[i, y1] = line[j];
                    }
                }
                else
                {
                    for (Int32 i = x1, j = 0; i <= x2; i++, j++)
                    {
                        _matrix[i, y1] = line[j];
                    }
                }
            }
            else
                throw new ArgumentException("Передаваемые координаты должны образовывать строку.");
        }

        // Сдвиг одной линии матрицы по правилам игры.
        public static Int32 LineShiftAttempt(Int32[] line)
        {
            if (line == null || line.Length < 2)
                throw new ArgumentException("Aray length must be more than 1");

            Boolean[] connected = new Boolean[line.Length];
            Boolean moved = false;
            Int32 score = 0;

            for (Int32 i = 0; i < line.Length; i++)
            {
                for (Int32 j = line.Length - 1; j > 0; j--)
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
        public static Boolean CanBeMoved(Int32[,] _matrix)
        {
            if (_matrix == null) return false;

            for (Int32 i = 0; i < _matrix.GetLength(0); i++)
            {
                for (Int32 j = 0; j < _matrix.GetLength(1); j++)
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
        private Int32 _maxLength;

        public LimitedStack(Int32 maxLength)
        {
            _maxLength = maxLength;
        }

        new public void Push(T element)
        {
            if (this.Count == _maxLength)
            {
                List<T> tempList = this.ToList<T>();
                for (Int32 i = tempList.Count - 1; i > 0; i--)
                {
                    tempList[i] = tempList[i - 1];
                }
                for (Int32 i = this.Count - 1; i > 0; i--)
                {
                    tempList[i] = tempList[i - 1];
                }
                tempList[0] = element;
                this.Clear();
                for (Int32 i = tempList.Count - 1; i >= 0; i--)
                {
                    base.Push(tempList[i]);
                }

                //T[] newStack = this.ToArray();
                //for (Int32 i = this.Count - 1; i > 0; i--)
                //{
                //    newStack[i] = newStack[i - 1];
                //}
                //newStack[0] = element;
                //this.Clear();
                //for (Int32 i = newStack.Length - 1; i >= 0; i--)
                //{
                //    base.Push(newStack[i]);
                //}
            }
            else
                base.Push(element);
        }
    }
}