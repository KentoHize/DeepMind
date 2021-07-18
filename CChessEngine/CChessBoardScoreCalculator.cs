using System;
using System.Collections.Generic;
using System.Text;

namespace CChessEngine
{
    public static class CChessBoardScoreCalculator
    {

        //NoDisk專門使用
        //一般運算
        //更強大
        //車600分 馬250分 炮300分 非過河兵100分 象100分 士100分
        //兵進一+20分
        //再進一+30分
        //之後離主將越近+10分
        //走至底線-100分
        //車離開原地 +30分
        //車過河 +60分
        //炮過河 +30分
        //有士+40分
        //有象+20分
        //馬離主將越近 + 10分
        //上象+30分
        //邊象-20分
        //上士+30分
        public static long MeasureScorePrecision(CChessBoard board)
        {
            long result = 0;            
            int sign = 1;
            int[] canonQuantity = new int[2];
            bool[] hasAdvisor = new bool[2];
            bool[] hasBishop = new bool[2];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (Char.IsUpper(board[i, j]))
                        sign = 1;
                    else
                        sign = -1;
                    switch (board[i, j])
                    {
                        case 'p':
                        case 'P':
                            result += 100 * sign;
                            //持有100分
                            if (sign == 1 && j == 4)
                                result += 20;
                            if (sign == -1 && j == 5)
                                result -= 20;
                            //進一+20分
                            if (sign == 1 && j >= 5)
                                result += 30 + (5 - j) * 10 + Math.Abs(i - 5) * -10 + 40; 
                            if (sign == -1 && j <= 4)
                                result -= 30 + (j - 4) * 10 + Math.Abs(i - 5) * -10 + 40;
                            //過河+30分 ，每靠近主將一步+10分
                            if (sign == 1 && j == 9)
                                result -= 100;
                            if (sign == -1 && j == 0)
                                result += 100;
                            //底兵-100分
                            break;                        
                        case 'c':
                        case 'C':
                            result += 300 * sign;
                            if(sign == 1)
                                canonQuantity[0] += 1;
                            else
                                canonQuantity[1] += 1;
                            //持有300分
                            if (sign == 1 && j >= 5)
                                result += 30;
                            if (sign == -1 && j <= 4)
                                result -= 30;
                            //過河+30分                            
                            break;
                        case 'n':
                        case 'N':
                            result += 250 * sign;
                            //持有250分
                            if (sign == 1)
                                result += j * 10 + Math.Abs(i - 5) * -10 + 40;
                            if (sign == -1)
                                result -= (9 - j) * 10 + Math.Abs(i - 5) * -10 + 40;
                            //離主將越近 + 10分
                            if (sign == 1 && j == 9)
                                result -= 10;
                            if (sign == -1 && j == 0)
                                result += 10;
                            //底線-10分
                            break;
                        case 'r':
                        case 'R':                            
                            result += 600 * sign;
                            //持有600分
                            if (sign == 1 && j >= 5)
                                result += 60;
                            if (sign == -1 && j <= 4)
                                result -= 60;
                            //過河+60分
                            if (sign == 1 && ((i != 0 && j != 0) || (i != 8 && j != 0)))
                                result += 30;
                            if (sign == -1 && ((i != 0 && j != 9) || (i != 8 && j != 9)))
                                result -= 30;
                            //離開原地+30分
                            break;
                        case 'b':
                        case 'B':
                            result += 100 * sign;
                            if (sign == 1)
                                hasBishop[0] = true;
                            else
                                hasBishop[1] = true;
                            //持有100分
                            if (sign == 1 && i == 4 && j == 2)
                                result += 30;
                            if (sign == -1 && i == 4 && j == 7)
                                result -= 30;
                            //上象+30分
                            if (sign == 1 && (i == 0 || i == 8) && j == 2)
                                result -= 20;
                            if (sign == -1 && (i == 0 || i == 8) && j == 7)
                                result += 20;
                            //邊象-20分                            
                            break;
                        case 'A':
                        case 'a':                            
                            result += 100 * sign;
                            if (sign == 1)
                                hasAdvisor[0] = true;
                            else
                                hasAdvisor[1] = true;
                            //持有100分
                            if (sign == 1 && i == 4 && j == 1)
                                result += 30;
                            if (sign == -1 && i == 4 && j == 8)
                                result -= 30;
                            //上士+30分                            
                            break;
                        default:
                            break;
                    }
                }
            }
            if (hasBishop[0])
                result += canonQuantity[0] * 20;
            if (hasBishop[1])
                result -= canonQuantity[1] * 20;
            //炮有象+20分
            if (hasAdvisor[0])
                result += canonQuantity[0] * 40;
            if (hasAdvisor[1])
                result -= canonQuantity[1] * 40;
            //炮有士+40分

            //結算
            return CChessBoardNode.DefaultScore + result;
        }

        //NoDisk專門使用
        //一般運算
        public static long MeasureScore(CChessBoard board)
        {
            //車6x分 馬3x分 炮3x分 過河兵2x分 兵x分 象x分 士x分
            //最少32 + 5 + 2 + 2 = 39分
            //最多32 + 10 + 2 + 2 = 44分
            //最小-44分
            long result = 44;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    switch (board[i, j])
                    {
                        case 'p':
                            result -= 1;
                            break;
                        case 'P':
                            result += 1;
                            break;
                        case 'c':
                            result -= 3;
                            break;
                        case 'C':
                            result += 3;
                            break;
                        case 'n':
                            result -= 3;
                            break;
                        case 'N':
                            result += 3;
                            break;
                        case 'r':
                            result -= 6;
                            break;
                        case 'R':
                            result += 6;
                            break;
                        case 'b':
                            result -= 1;
                            break;
                        case 'B':
                            result += 1;
                            break;
                        case 'a':
                            result -= 1;
                            break;
                        case 'A':
                            result += 1;
                            break;
                        default:
                            break;
                    }
                }
            }
            return result * (CChessBoardNode.MaxScore / 90);
        }
    }
}
