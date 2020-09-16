using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise
{
    class Graph
    {
        int[,] adj = new int[6, 6]
        {
            { -1, 15, -1, 35, -1, -1},
            { 15, 0, 05, 10, -1, -1},
            { -1, 05, -1, -1, -1, -1},
            { 35, 10, -1, -1, 05, -1},
            { -1, -1, -1, 05, -1, 05},
            { -1, -1, -1, -1, 05, -1},
        };

        //List<int>[] adj2 = new List<int>[]
        //{
        //    new List<int>() { 1, 3 },
        //    new List<int>() { 0, 2, 3},
        //    new List<int>() { 1 },
        //    new List<int>() { 0, 1, 4},
        //    new List<int>() { 3, 5},
        //    new List<int>() { 4},
        //};

        #region DFS
        /*
        bool[] visited = new bool[6];

        // 1) 우선 now 부터 방문하고.
        // 2) now와 연결된 정점들을 하나씩 확인해서 방문한다.
        public void DFS(int now)
        {
            Console.WriteLine(now);
            visited[now] = true;

            for(int next = 0; next < 6; ++next)
            {
                // 연결되어 있지 않으면 스킵.
                if (adj[now, next] == 0)
                    continue;

                // 이미 방문했다면 스킵
                if (visited[next])
                    continue;

                DFS(next);
            }
        }

        public void DFS2(int now)
        {
            Console.WriteLine(now);
            visited[now] = true;

            foreach(int next in adj2[now])
            {
                // 이미 방문했다면 스킵
                if (visited[next])
                    continue;

                DFS2(next);
            }
        }
        
        public void SeachAll()
        {
            visited = new bool[6];
            for(int now = 0; now < 6; ++now)
            {
                if (visited[now] == false)
                    DFS(now);
            }
        }
        */
        #endregion

        #region BFS
        /*
        public void BFS(int start)
        {
            bool[] found = new bool[6];
            int[] parent = new int[6];
            int[] distance = new int[6];

            Queue<int> q = new Queue<int>();

            q.Enqueue(start);
            found[start] = true;
            parent[start] = start;
            distance[start] = 0;

            while(q.Count > 0)
            {
                int now = q.Dequeue();
                Console.WriteLine(now);

                for(int next = 0; next < 6; ++next)
                {
                    if (adj[now, next] == 0)
                        continue;

                    if (found[next])
                        continue;

                    q.Enqueue(next);
                    found[next] = true;
                    parent[next] = now;
                    distance[next] = distance[next] + 1;
                }
            }
        }
        */

        #endregion

        #region Dijkstra
        public void Dijkstra(int start)
        {
            bool[] visited = new bool[6];
            int[] distance = new int[6];
            int[] parent = new int[6];

            for(int i = 0; i<distance.Length; ++i)
                distance[i] = Int32.MaxValue;
            
            distance[start] = 0;
            parent[start] = start;

            while (true)
            {
                // 제일 좋은 후보를 찾는다. (가장 가까이에 있는)

                //  가장 유력한 후보의 거리의 번호를 저장한다.
                int closet = Int32.MaxValue;
                int now = -1;

                for(int i = 0; i< 6; ++i)
                {
                    // 이미 방문한 정점은 스킵
                    if (visited[i])
                        continue;

                    // 아직 발견(예약)된 적이 없거나, 기존 후보보다 멀리 있으면 스킵
                    if (distance[i] == Int32.MaxValue ||
                        distance[i] > closet)
                        continue;

                    closet = distance[i];
                    now = i;
                }

                // 다음 후보가 하나도 없다 -> 종료
                if (now == -1)
                    break;

                // 제일 좋은 후보를 찾았으니 방문한다.
                visited[now] = true;


                // 방문한 정점과 인접한 정점들을 조사해서
                // 상황에 따라 발견한 최단거리를 갱신한다.
                for(int next = 0; next < 6; ++next)
                {
                    // 연결되지 않는 정점 스킵
                    if (adj[now, next] == -1)
                        continue;

                    if (visited[next])
                        continue;

                    // 새로 조사된 정점의 최단거리를 계산한다.
                    int nextDist = distance[now] + adj[now, next];

                    // 만약에 기존에 발견한 최단거리가 새로 조사된 최단거리보다 크면, 정보를 갱신
                    if (nextDist < distance[next])
                    {
                        distance[next] = nextDist;
                        parent[next] = now;
                    }
                }
            }
        }
        #endregion
    }

    class TreeNode<T>
    {
        public T Data { get; set; }
        public List<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();
    }
    
    class PriorityQueue<T> where T : IComparable<T>
    {
        List<T> _heap = new List<T>();

        public void Push(T data)
        {
            // 힙의 맨 끝에 새로운 데이터를 삽입한다.
            _heap.Add(data);

            int now = _heap.Count - 1;

            // 도장깨기 시작
            while(now > 0)
            {
                int next = (now - 1) / 2;
                if (_heap[now].CompareTo(_heap[next]) < 0)
                    break;

                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                // 검사 위치를 이동한다.
                now = next;
            }
        }

        public T Pop()
        {
            // 반환할 데이터를 따로 저장
            T ret = _heap[0];

            int lastindex = _heap.Count - 1;
            _heap[0] = _heap[lastindex];
            _heap.RemoveAt(lastindex);
            --lastindex;

            // 역 도장깨기

            int now = 0;
            while(true)
            {
                int left = (2 * now) + 1;
                int right = (2 * now) + 2;

                int next = now;
                // 왼쪽보다 현재 값이 크면, 왼쪽으로 이동
                if (left <= lastindex && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left;

                // 오른쪽 값이 현재 값보다 크면, 오른쪽으로 이동
                if (right <= lastindex && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right;

                // 왼쪽, 오른쪽 모두 현재값보다 작으면 종료
                if (next == now)
                    break;

                // 두 값 교체
                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                // 검사 위치 이동
                now = next;
            }

            return ret;
        }

        public int Count()
        {
            return _heap.Count;
        }
    }

    class Knight : IComparable<Knight>
    {
        public int ID { get; set; }

        public int CompareTo(Knight other)
        {
            if (ID == other.ID)
                return 0;

            return ID > other.ID ? 1 : -1;
        }
    }


    class Program
    {
        static TreeNode<string> MakeTree()
        {
            TreeNode<string> root = new TreeNode<string>() { Data = "R1 개발실" };
            {
                TreeNode<string> node = new TreeNode<string>() { Data = "디자인팀" };
                node.Children.Add(new TreeNode<string>() { Data = "전투" });
                node.Children.Add(new TreeNode<string>() { Data = "경재" });
                node.Children.Add(new TreeNode<string>() { Data = "스토리" });
                root.Children.Add(node);
            }

            {
                TreeNode<string> node = new TreeNode<string>() { Data = "프로그래밍팀" };
                node.Children.Add(new TreeNode<string>() { Data = "서버" });
                node.Children.Add(new TreeNode<string>() { Data = "클라" });
                node.Children.Add(new TreeNode<string>() { Data = "엔진" });
                root.Children.Add(node);
            }

            {
                TreeNode<string> node = new TreeNode<string>() { Data = "아트팀" };
                node.Children.Add(new TreeNode<string>() { Data = "배경" });
                node.Children.Add(new TreeNode<string>() { Data = "캐릭터" });
                node.Children.Add(new TreeNode<string>() { Data = "원화" });
                root.Children.Add(node);
            }

            return root;
        }

        static void PrintTree(TreeNode<string> root)
        {
            Console.WriteLine(root.Data.ToString());

            foreach(TreeNode<string> child in root.Children)
            {
                PrintTree(child);
            }
        }

        static int GetHeight(TreeNode<string> root)
        {
            int height = 0;

            foreach(TreeNode<string> child in root.Children)
            {
                int newHeight = GetHeight(child) + 1;
                if (height < newHeight)
                    height = newHeight;
            }

            return height;
        }

        static void Main(string[] args)
        {
            //Graph graph = new Graph();
            //graph.BFS(0);
            //graph.Dijkstra(0);

            //TreeNode<string> root = MakeTree();
            //PrintTree(root);
            //Console.WriteLine(GetHeight(root));
            PriorityQueue<Knight> q = new PriorityQueue<Knight>();
            q.Push(new Knight() { ID = 20 });
            q.Push(new Knight() { ID = 30 });
            q.Push(new Knight() { ID = 40 });
            q.Push(new Knight() { ID = 10 });
            q.Push(new Knight() { ID = 5 });

            while (q.Count() > 0)
            {
                Console.WriteLine(q.Pop().ID);
            }
        }
    }
}
