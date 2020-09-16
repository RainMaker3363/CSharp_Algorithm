using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part02
{
    class MyList<T>
    {
        const int DEFAULTSize = 1;
        public int Count = 0;       // 실제로 사용중인 데이터 개수
        public int Capacity    // 예약된 데이터 개수
        {
            get
            {
                return _data.Length;
            }
        }

        private T[] _data = new T[DEFAULTSize];

        public void Add(T item)
        {
            // 1. 공간이 충분히 남아 있는지 확인한다.
            if(Count >= Capacity)
            {
                // 공간을 다시 눌러서 확보한다
                T[] newArray = new T[Count + 2];
                for(int i = 0; i< Count; ++i)
                {
                    newArray[i] = _data[i];
                }

                _data = newArray;
            }

            // 2. 공간에다가 데이터를 넣어준다.
            _data[Count] = item;
            ++Count;
        }

        public T this[int index]
        {
            get
            {
                return _data[index];
            }

            set
            {
                _data[index] = value;
            }
        }

        public void RemoveAt(int index)
        {
            for (int i = index; i < Count - 1; ++i)
                _data[i] = _data[i + 1];

            _data[Count - 1] = default(T);

            --Count;
        }
    }

    class MyLinkedListNode<T>
    {
        public T Data;
        public MyLinkedListNode<T> Next;
        public MyLinkedListNode<T> Prev;
    }

    class MyLinkedList<T>
    {
        public MyLinkedListNode<T> Head = null;    // 첫번째
        public MyLinkedListNode<T> Tail = null;    // 마지막

        public int Count = 0;

        public MyLinkedListNode<T> AddLast(T data)
        {
            MyLinkedListNode<T> newRoom = new MyLinkedListNode<T>();
            newRoom.Data = data;

            // 만약 아직 방이 아예 없었다면, 새로 추가된 첫번째 방이 곧 Head이다
            if(Head == null)
            {
                Head = newRoom;
            }

            // 기존에 마지막 방과 새로 추가되는 방을 연결해준다.
            if(Tail != null)
            {
                Tail.Next = newRoom;
                newRoom.Prev = Tail;
            }

            Tail = newRoom;
            ++Count;

            return newRoom;
        }

        public void Remove(MyLinkedListNode<T> room)
        {
            // 기존의 첫번째 방 다음 방을 첫번째 방으로 인정한다
            if (Head == room)
                Head = Head.Next;

            if(Tail == room)
                Tail = Tail.Prev;

            if (room.Prev != null)
            {
                room.Prev.Next = room.Next;
            }

            if(room.Next != null)
            {
                room.Next.Prev = room.Prev;
            }

            --Count;
        }
    }

    class Board
    {
        const char CIRCLE = '\u25cf';

        public TileType[,] Tile { get; private set; }
        public int Size { get; private set; }

        public int DestY { get; private set; }
        public int DestX { get; private set; }
        

        Player _player;

        public enum TileType
        {
            Empty,
            Wall,
        }

        public void Initialize(int size, Player player)
        {
            if (size % 2 == 0)
                return;

            Tile = new TileType[size, size];
            Size = size;
            _player = player;

            DestY = Size - 2;
            DestX = Size - 2;

            //GenerateBinaryTreeMaze();
            GenerateSideWinderMaze();
        }

        private void GenerateBinaryTreeMaze()
        {
            #region Binary Tree Algorithm

            // 일단 길을 다 막아버리는 작업
            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    if (x % 2 == 0 || y % 2 == 0)
                    {
                        Tile[y, x] = TileType.Wall;
                    }
                    else
                    {
                        Tile[y, x] = TileType.Empty;
                    }
                }
            }

            // 랜덤으로 우측 혹은 아래로 길을 뚫는 작업
            Random rand = new Random();
            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    if (x % 2 == 0 || y % 2 == 0)
                        continue;

                    if (y == Size - 2 && x == Size - 2)
                    {
                        continue;
                    }

                    if (y == Size - 2)
                    {
                        Tile[y, x + 1] = TileType.Empty;
                        continue;
                    }

                    if (x == Size - 2)
                    {
                        Tile[y + 1, x] = TileType.Empty;
                        continue;
                    }

                    if (rand.Next(0, 2) == 0)
                    {
                        Tile[y, x + 1] = TileType.Empty;
                    }
                    else
                    {
                        Tile[y + 1, x] = TileType.Empty;
                    }
                }
            }

            #endregion
        }

        private void GenerateSideWinderMaze()
        {
            // 일단 길을 다 막아버리는 작업
            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    if (x % 2 == 0 || y % 2 == 0)
                    {
                        Tile[y, x] = TileType.Wall;
                    }
                    else
                    {
                        Tile[y, x] = TileType.Empty;
                    }
                }
            }

            Random rand = new Random();
            for (int y = 0; y < Size; ++y)
            {
                int count = 1;
                for (int x = 0; x < Size; ++x)
                {
                    if (x % 2 == 0 || y % 2 == 0)
                        continue;

                    if (y == Size - 2 && x == Size - 2)
                    {
                        continue;
                    }

                    if (y == Size - 2)
                    {
                        Tile[y, x + 1] = TileType.Empty;
                        continue;
                    }

                    if (x == Size - 2)
                    {
                        Tile[y + 1, x] = TileType.Empty;
                        continue;
                    }

                    if (rand.Next(0, 2) == 0)
                    {
                        Tile[y, x + 1] = TileType.Empty;
                        ++count;
                    }
                    else
                    {
                        int randomIndex = rand.Next(0, count);
                        Tile[y + 1, x - (randomIndex * 2)] = TileType.Empty;
                        count = 1;
                    }
                }
            }
        }

        public void Render()
        {
            ConsoleColor prevColor = Console.ForegroundColor;
            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    // 플레이어 좌표를 갖고 와서, 그 좌표랑 현재 y, x 가 일치하면 플레이어 전용 색상으로 표시
                    if(y == _player.PosY && x == _player.PosX)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    else if(y == DestY && x == DestX)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else
                    {
                        Console.ForegroundColor = GetTileColor(Tile[y, x]);
                    }
                    
                    Console.Write(CIRCLE);
                }

                Console.WriteLine();
            }

            prevColor = Console.ForegroundColor;
        }

        private ConsoleColor GetTileColor(TileType type)
        {
            switch(type)
            {
                case TileType.Empty:
                    return ConsoleColor.Green;
                case TileType.Wall:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.Green;
            }
        }
    }
}
 