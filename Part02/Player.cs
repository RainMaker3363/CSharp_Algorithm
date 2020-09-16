using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part02
{
    class Pos
    {
        public int Y;
        public int X;
        public Pos(int _y, int _x)
        {
            this.Y = _y;
            this.X = _x;
        }
    }

    class Player
    {
        const int MOVE_TICK = 10;
        UInt64 _sumTick = 0;

        public int PosX { get; private set; }
        public int PosY { get; private set; }

        private Board _board;
        private Random _random = new Random();


        enum eDir
        {
            Up = 0,
            Left,
            Down,
            Right,
        }

        struct PQNode : IComparable<PQNode>
        {
            public int F;
            public int G;
            public int Y;
            public int X;

            public int CompareTo(PQNode other)
            {
                if (F == other.F)
                    return 0;

                return F < other.F ? 1 : -1;
            }
        }

        int _dir = (int)eDir.Up;
        List<Pos> _points = new List<Pos>();
        int _listIndex = 0;

        public void Initilize(int y, int x, Board board)
        {
            PosX = x;
            PosY = y;
            _board = board;

            //RightHand();
            //BFS();
            AStar();
        }



        private void AStar()
        {
            // U L D R UL DL DR UR
            int[] deltaY = new int[] { -1, 0, 1, 0 };
            int[] deltaX = new int[] { 0, -1, 0, 1};
            int[] cost = new int[] { 10, 10, 10, 10};

            // 점수 매기기
            // F = G + H
            // F = 최종 점수( 적을 수록 좋음, 경로에 따라 달라짐)
            // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용( 작을수록 좋음)
            // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

            // (Y, X) 이미 방문했는지 여부 기록 (방문 = Closed 상태)
            bool[,] closesd = new bool[_board.Size, _board.Size];

            // (Y, X) 가는 길을 한 번이라도 발견했는지
            // 발견 X => MaxValue;
            // 발견 O => F = G + H
            int[,] open = new int[_board.Size, _board.Size];
            for(int y = 0; y <_board.Size; ++y)
            {
                for (int x = 0; x < _board.Size; ++x)
                {
                    open[y, x] = Int32.MaxValue;
                }
            }

            Pos[,] parent = new Pos[_board.Size, _board.Size];

            // 오픈 리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            // 시작점 발견 (에약 진행)
            open[PosY, PosX] = 10 * (Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX));
            pq.Push(new PQNode() { F = 10 * (Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX)), G = 0, Y = PosY, X = PosX});
            parent[PosY, PosX] = new Pos(PosY, PosX);

            while (pq.Count > 0)
            {
                // 제일 좋은 후보를 찾는다
                PQNode node = pq.Pop();

                // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문한 경우 스킵
                if (closesd[node.Y, node.X])
                    continue;

                // 방문한다
                closesd[node.Y, node.X] = true;

                // 목적지 도착했으면 바로 종료
                if (node.Y == _board.DestY && node.X == _board.DestX)
                    break;

                // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(Open)한다
                for(int i = 0; i<deltaY.Length; ++i)
                {
                    int nextY = node.Y + deltaY[i];
                    int nextX = node.X + deltaX[i];

                    // 유효 범위가 넘어가면 스킵
                    if (nextX < 0 || nextX >= _board.Size ||
                        nextY < 0 || nextY >= _board.Size)
                        continue;

                    // 벽으로 막혀있으면 스킵
                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall)
                        continue;

                    // 이미 방문한 곳이면 스킵
                    if (closesd[nextY, nextX])
                        continue;


                    // 비용 계산
                    int g = node.G + cost[i];
                    int h = 10 * (Math.Abs(_board.DestY - nextY) + Math.Abs(_board.DestX - nextX));

                    // 다른 경로에서 더 빠른 길을 이미 찾았으면 스킵
                    if (open[nextY, nextX] < g + h)
                        continue;

                    // 예약 진행
                    open[nextY, nextX] = g + h;
                    pq.Push(new PQNode() { F = g + h, G = g, Y = nextY, X = nextX });
                    parent[nextY, nextX] = new Pos(node.Y, node.X);
                }
            }

            CalcPathFromParent(parent);
        }

        // 넓이 우선 탐색
        private void BFS()
        {
            int[] deltaY = new int[] {-1, 0, 1, 0 };
            int[] deltaX = new int[] { 0, -1, 0, 1};

            bool[,] found = new bool[_board.Size, _board.Size];
            Pos[,] parent = new Pos[_board.Size, _board.Size];


            Queue<Pos> q = new Queue<Pos>();
            q.Enqueue(new Pos(PosY, PosX));
            found[PosY, PosX] = true;
            parent[PosY, PosX] = new Pos(PosY, PosX);

            while (q.Count > 0)
            {
                Pos pos = q.Dequeue();
                int nowY = pos.Y;
                int nowX = pos.X;

                for(int i = 0; i<4; ++i)
                {
                    int nextY = nowY + deltaY[i];
                    int nextX = nowX + deltaX[i];

                    if (nextX < 0 || nextX >= _board.Size ||
                        nextY < 0 || nextY >= _board.Size)
                        continue;

                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall)
                        continue;

                    if (found[nextY, nextX])
                        continue;

                    q.Enqueue(new Pos(nextY, nextX));
                    found[nextY, nextX] = true;
                    parent[nextY, nextX] = new Pos(nowY, nowX);
                }
            }

            CalcPathFromParent(parent);
        }

        private void CalcPathFromParent(Pos[,] parent)
        {
            int y = _board.DestY;
            int x = _board.DestX;
            while (parent[y, x].Y != y || parent[y, x].X != x)
            {
                _points.Add(new Pos(y, x));
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            _points.Add(new Pos(y, x));
            _points.Reverse();
        }

        // 우수법 탐색
        private void RightHand()
        {

            // 현재 바라보고 있는 방향을 기준으로, 좌표 변화를 나타낸다.
            int[] frontY = new int[] { -1, 0, 1, 0 };
            int[] frontX = new int[] { 0, -1, 0, 1 };

            int[] rightY = new int[] { 0, -1, 0, 1 };
            int[] rightX = new int[] { 1, 0, -1, 0 };

            _points.Add(new Pos(PosY, PosX));

            // 목적지 도착하기 전에는 계속 실행
            while (PosY != _board.DestY || PosX != _board.DestX)
            {
                // 1. 현재 바라보는 방향을 기준으로 오른쪽으로 갈 수 있는지 확인.
                if (_board.Tile[PosY + rightY[_dir], PosX + rightX[_dir]] == Board.TileType.Empty)
                {
                    // 오른쪽 방향으로 90도 회전
                    _dir = (_dir - 1 + 4) % 4;

                    // 앞으로 한 보 전진
                    PosX = PosX + frontX[_dir];
                    PosY = PosY + frontY[_dir];

                    _points.Add(new Pos(PosY, PosX));
                }
                // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인
                else if (_board.Tile[PosY + frontY[_dir], PosX + frontX[_dir]] == Board.TileType.Empty)
                {
                    // 앞으로 한 보 전진
                    PosX = PosX + frontX[_dir];
                    PosY = PosY + frontY[_dir];

                    _points.Add(new Pos(PosY, PosX));
                }
                else
                {
                    // 왼쪽 방향으로 90도 회전
                    _dir = (_dir + 1 + 4) % 4;
                }
            }
        }

        public void Update(UInt64 deltaTick)
        {
            if(_listIndex >= _points.Count)
            {
                _listIndex = 0;
                _points.Clear();
                _board.Initialize(_board.Size, this);
                Initilize(1, 1, _board);
            }

            _sumTick += deltaTick;
            if(_sumTick >= MOVE_TICK)
            {
                if (_listIndex >= _points.Count)
                    return;

                _sumTick = 0;

                PosY = _points[_listIndex].Y;
                PosX = _points[_listIndex].X;
                ++_listIndex;
                // 0.1초마다 실행될 로직을 넣어준다.
                //int randValue = _random.Next(0, 4);
                //switch(randValue)
                //{
                //    case 0:         // 상
                //        {
                //            if (PosY - 1 >= 0 &&
                //                _board.Tile[PosY - 1, PosX] == Board.TileType.Empty)
                //                PosY = PosY - 1;
                //        }
                //        break;
                //    case 1:         // 하
                //        {
                //            if (PosY + 1 < _board.Size &&
                //                _board.Tile[PosY + 1, PosX] == Board.TileType.Empty)
                //                PosY = PosY + 1;
                //        }
                //        break;
                //    case 2:         // 좌
                //        {
                //            if (PosX - 1 >= 0 &&
                //                _board.Tile[PosY, PosX - 1] == Board.TileType.Empty)
                //                PosX = PosX - 1;
                //        }
                //        break;
                //    case 3:         // 우
                //        {
                //            if (PosX + 1 < _board.Size &&
                //                _board.Tile[PosY, PosX + 1] == Board.TileType.Empty)
                //                PosX = PosX + 1;
                //        }
                //        break;
                //}
            }
        }
    }
}
