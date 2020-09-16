using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Part02
{
    class Program
    {
        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        static void Main(string[] args)
        {
            Player player = new Player();
            Board board = new Board();
            board.Initialize(25, player);
            player.Initilize(1, 1, board);
            

            Console.CursorVisible = false;
            const UInt64 WAIT_TICK = 1000 / 30;


            UInt64 lastTick = 0;


            while(true)
            {
                #region 프레임 관리
                UInt64 currentTick = GetTickCount64();// System.Environment.TickCount;
                UInt64 elapsedTick = currentTick - lastTick;

                
                // 만약에 경과한 시간이 1 / 30 초보다 작다면
                if (elapsedTick < WAIT_TICK)
                    continue;

                UInt64 deltaTick = currentTick - lastTick;
                lastTick = currentTick;

                #endregion

                // 입력

                // 로직
                player.Update(deltaTick);

                // 랜더링
                Console.SetCursorPosition(0, 0);
                board.Render();
            }
            
        }
    }
}
