using System;
using System.Collections.Generic;
using System.Text;

namespace geoc
{
    public class PositionStabilisation
    {
        bool b_HasInitialized = false;
        int i_RoundCount = 0;
        int i_ArrayPosition = 0;

        NavigationCalculation.grad_minute_second[] ac_PositionArray;
        NavigationCalculation.grad_minute_second c_AveragePosition;
        //NavigationCalculation.grad_minute_second c_LastAveragePosition;

        public PositionStabilisation(int i_InitRoundCount)
        {
            i_RoundCount = i_InitRoundCount;
            
            //int i;
            //for (i = 0; i < i_RoundCount; i++)
            //{
            ac_PositionArray = new NavigationCalculation.grad_minute_second[i_RoundCount];
            //}

        }

        public void addPosition(NavigationCalculation.grad_minute_second c_NewPosition)
        {
            if (b_HasInitialized == false)
            {
                b_HasInitialized = true;

                int i;
                for (i = 0; i < i_RoundCount; i++)
                {
                    ac_PositionArray[i] = c_NewPosition;
                }
                c_AveragePosition = c_NewPosition;
                c_AveragePosition.Mul(i_RoundCount);
            }
            else
            {
                c_AveragePosition.Sub(ac_PositionArray[i_ArrayPosition]);
                c_AveragePosition.Add(c_NewPosition);
                ac_PositionArray[i_ArrayPosition] = c_NewPosition;
                i_ArrayPosition = (i_ArrayPosition + 1) % i_RoundCount;
            }
        }

        public NavigationCalculation.grad_minute_second getAveragePosition()
        {
            NavigationCalculation.grad_minute_second c_dividedAverage = c_AveragePosition;
            c_dividedAverage.Div(i_RoundCount);
            return (c_dividedAverage);
        }

        //public NavigationCalculation.grad_minute_second getLastAveragePosition()
        //{

        //}
    }
}
