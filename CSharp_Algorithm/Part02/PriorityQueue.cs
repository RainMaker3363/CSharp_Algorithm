using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part02
{
    class PriorityQueue<T> where T : IComparable<T>
    {
        List<T> _heap = new List<T>();

        public void Push(T data)
        {
            // 힙의 맨 끝에 새로운 데이터를 삽입한다.
            _heap.Add(data);

            int now = _heap.Count - 1;

            // 도장깨기 시작
            while (now > 0)
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
            while (true)
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

        public int Count
        {
            get
            {
                return _heap.Count;
            }
        }
    }
}
