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

    [Serializable]
    public sealed class _2048
    {
        public event EndGameEventHandler EndGameEvent;
        public event ScoreOverflowEventHandler ScoreOverflowEvent;
        public event NewTileEventHandler NewTileEvent;

        private Int32 minValue;                   // Минимально выпадающий блок
        private Int32 score;                      // Очки
        private Int32[,] matrix;                  // Сама матрица, содержащая структуру игры
        private LimitedStack<Int32[,]> history;   // Ограниченный стек действий
        private LimitedStack<Int32> scoreHistory; // Ограниченный стек очков
        private Int32 _historyMaxLength = 5;       // Количество разрешенных отмен операций

        /// <param name="rows">Количество строк.</param>
        /// <param name="cells">Количество столбцов.</param>
        /// <param name="minValue">Минимально выпадающий блок.</param>
        public _2048(Int32 rows, Int32 cells, Int32 minValue = 2)
        {
            if (rows < 2 || cells < 2)
                throw new ArgumentException("Размер матрицы не может быть меньше 2х2.");

            this.minValue = minValue;
            New(rows, cells);
        }

        public Boolean TrySaveGame(String file)
        {
            if (file == null || !File.Exists(file)) return false;
            try
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream(file, FileMode.Truncate)))
                {
                    bw.Write(minValue);
                    bw.Write(score);
                    bw.Write(matrix.GetLength(0));
                    bw.Write(matrix.GetLength(1));
                    foreach (Int32 i in matrix)
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
                    minValue = br.ReadInt32();
                    score = br.ReadInt32();
                    matrix = new Int32[br.ReadInt32(), br.ReadInt32()];
                    for (Int32 i = 0; i < matrix.GetLength(0); i++)
                    {
                        for (Int32 j = 0; j < matrix.GetLength(1); j++)
                        {
                            matrix[i, j] = br.ReadInt32();
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

        /// <summary>
        /// Генерация нового поля.
        /// </summary>
        /// <param name="rows">Количество строк.</param>
        /// <param name="cells">Количество столбцов.</param>
        private void New(Int32 rows, Int32 cells)
        {
            Random random = new Random();
            matrix = new Int32[rows, cells];
            Int32 count = rows > cells ? random.Next(1, cells) : random.Next(1, rows);

            while (count != 0)
            {
                NewElement();
                count--;
            }
            history = new LimitedStack<Int32[,]>(_historyMaxLength);
            scoreHistory = new LimitedStack<Int32>(_historyMaxLength);
        }
        /// <summary>
        /// Генерация нового блока
        /// </summary>
        private void NewElement()
        {
            Random random = new Random();
            Int32 size = matrix.Length;
            Int32 rows = matrix.GetLength(0);
            Int32 cells = matrix.GetLength(1);

            Int32 x = random.Next(0, rows);
            Int32 y = random.Next(0, cells);

            while (matrix[x, y] != 0)
            {
                x = random.Next(0, rows);
                y = random.Next(0, cells);
            }
            // Шанс выпадения удвоенного блока = 1/10
            if (random.Next(0, 10) == 1)
                matrix[x, y] = minValue + minValue;
            else
                matrix[x, y] = minValue;

            NewTileEvent?.Invoke(x, y);

            if (!_matrixOperations.CanBeMoved(matrix))
                EndGameEvent();
        }

        /// <summary>
        /// Получение копии матрицы игры.
        /// </summary>
        /// <returns>Матрица игры.</returns>
        public Int32[,] GetMatrix()
        {
            Int32[,] temporary = new Int32[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, temporary, matrix.Length);
            return temporary;
        }

        /// <summary>
        /// Сдвиг блоков вниз.
        /// </summary>
        /// <returns>Возвращает true, если как минимум 1 блок был сдвинут, иначе false.</returns>
        public Boolean TryMoveDown()
        {
            return Move(down: true);
        }
        /// <summary>
        ///  Сдвиг блоков вверх.
        /// </summary>
        /// <returns>Возвращает true, если как минимум 1 блок был сдвинут, иначе false.</returns>
        public Boolean TryMoveUp()
        {
            return Move(up: true);
        /// <returns>Возвращает true, если как минимум 1 блок был сдвинут, иначе false.</returns>
        }
        /// <summary>
        ///  Сдвиг блоков влево.
        /// </summary>
        /// <returns>Возвращает true, если как минимум 1 блок был сдвинут, иначе false.</returns>
        public Boolean TryMoveLeft()
        {
            return Move(left: true);
        }
        /// <summary>
        /// Сдвиг блоков вправо.
        /// </summary>
        /// <returns>Возвращает true, если как минимум 1 блок был сдвинут, иначе false.</returns>
        public Boolean TryMoveRight()
        {
            return Move(right: true);
        }

        // *В ДОРАБОТКЕ*
        private Boolean Move(Boolean down = false, Boolean up = false, Boolean left = false, Boolean right = false)
        {
            if (!down && !up && !left && !right) throw new ArgumentException("1 argument must be true.");

            Int32 rows = matrix.GetLength(0);
            Int32 cells = matrix.GetLength(1);
            Int32 moveScore = 0;
            Boolean oneMoved = false;

            Int32 originalScore = score;
            Int32[,] original_matrix = new Int32[rows, cells];
            Array.Copy(matrix, original_matrix, matrix.Length);
            Int32 untill;
            if (down || up) untill = cells;
            else untill = rows;
            for (Int32 i = 0; i < untill; i++)
            {
                Int32[] line = null;
                Int32 lineScore = 0;
                if (down)
                    line = _matrixOperations.SelectRow(matrix, 0, i, rows - 1, i);
                else if (up)
                    line = _matrixOperations.SelectRow(matrix, rows - 1, i, 0, i);
                else if (left)
                    line = _matrixOperations.SelectRow(matrix, i, cells - 1, i, 0);
                else if (right)
                    line = _matrixOperations.SelectRow(matrix, i, 0, i, cells - 1);
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
                        _matrixOperations.SetLine(matrix, line, 0, i, rows - 1, i);
                    else if (up)
                        _matrixOperations.SetLine(matrix, line, rows - 1, i, 0, i);
                    else if (left)
                        _matrixOperations.SetLine(matrix, line, i, cells - 1, i, 0);
                    else if (right)
                        _matrixOperations.SetLine(matrix, line, i, 0, i, cells - 1);
                    moveScore += lineScore;
                }
            }
            if (oneMoved)
            {
                try
                {
                    score += moveScore;
                }
                catch (OverflowException)
                {
                    ScoreOverflowEvent?.Invoke();
                }
                history.Push(original_matrix);
                scoreHistory.Push(originalScore);
                NewElement();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Возвращает true, если разрешено отменить ход.
        /// </summary>
        /// <returns>Возвращает true, если разрешено отменить ход, иначе false.</returns>
        public Boolean UndoAllowed()
        {
            return history.Count == 0 ? false : true;
        }
        /// <summary>
        /// Отмена одного хода.
        /// </summary>
        public void Undo()
        {
            if (history.Count == 0)
                return;

            matrix = history.Pop();
            score = scoreHistory.Pop();
        }

        public Int32 Score => score;
        public Int32 RowCount => matrix.GetLength(0);
        public Int32 ColumnCount => matrix.GetLength(1);
    
        // "Админка". Для тестирования.
        private void ForTesting()
        {
            Random random = new Random();
            Int32 size = matrix.Length;
            Int32 rows = matrix.GetLength(0);
            Int32 cells = matrix.GetLength(1);

            Int32 x = random.Next(0, rows);
            Int32 y = random.Next(0, cells);

            while (matrix[x, y] != 0)
            {
                x = random.Next(0, rows);
                y = random.Next(0, cells);
            }

            Int32 max = matrix[0,0];
            foreach (Int32 v in matrix)
            {
                if (max < v)
                    max = v;
            }
            matrix[x, y] = max+max;

            NewTileEvent?.Invoke(x, y);

            if (!_matrixOperations.CanBeMoved(matrix))
                EndGameEvent();
        }
    }

    /*Некоторые операции с матрицей*/
    public static class _matrixOperations
    {
        // Выделение строки матрицы по заданным координатам.
        // *В ДОРАБОТКЕ*
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
        // *В ДОРАБОТКЕ*
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
        // *В ДОРАБОТКЕ*
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
        private Int32 maxLength;

        public LimitedStack(Int32 maxLength)
        {
            this.maxLength = maxLength;
        }

        new public void Push(T element)
        {
            if (Count == maxLength)
            {
                List<T> tempList = this.ToList<T>();
                for (Int32 i = tempList.Count - 1; i > 0; i--)
                {
                    tempList[i] = tempList[i - 1];
                }
                for (Int32 i = Count - 1; i > 0; i--)
                {
                    tempList[i] = tempList[i - 1];
                }
                tempList[0] = element;
                Clear();
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