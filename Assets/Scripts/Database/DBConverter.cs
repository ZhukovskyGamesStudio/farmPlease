using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class DBConverter
{
    public static string FieldToString(Dictionary<Vector2Int, SmartTile> tilesDict)
    {
        string res = "";

        Vector2Int curCoord = new Vector2Int(0, 0);
        //Debug.Log("A is " + Convert.ToInt32('A'));

        /*  
            Debug.Log("Z is " + Convert.ToInt32('Z'));
            Debug.Log("a is " + Convert.ToInt32('a'));
            Debug.Log("z is " + Convert.ToInt32('z'));
        */
      

        int circle = 0;
        int step = 0;
        int i = 0;

        while (i < tilesDict.Count)
        {
            int index;
            if (tilesDict.ContainsKey(curCoord))
            {
                index = (int)tilesDict[curCoord].type;
            }
            else
                index = -10;

            res += ToUTF(index);

            curCoord = Next(curCoord, circle, step);

            step++;
            if (step == circle * 6 || circle == 0)
            {
                circle++;
                step = 0;
            }
            i++;
            if (i > 10000)
            {
                Debug.LogError("spent too much in while. Instant Break");
                break;
            }
        }
        return res;
    }

    



    public static string ToUTF(int index)
    {
        int val1 = index / 52 + Convert.ToInt32('A');
        int val2 = index % 52 + Convert.ToInt32('A');
        string res = Convert.ToChar(val1).ToString() + "" + Convert.ToChar(val2).ToString();
        //Debug.Log( index + " res: " +  res);
        return res;
    }


    public static int UTFToInt(string str)
    {
        int val1 = (Convert.ToInt32(str[0]) - Convert.ToInt32('A')) * 52;
        int val2 = Convert.ToInt32(str[1]) - Convert.ToInt32('A');
        return val1 + val2;
    }
    /*
   static char GetChar(int index)
   {

       return Encoding.UTF8.GetString(index.ToString());
   }  */

    public static Vector2Int Next(Vector2Int now, int circle, int step)
    {
        if (circle == 0)
            return GetHexNeighbor(now, 0);

        if (step == circle * 6 - 1)
            return GetHexNeighbor(GetHexNeighbor(now, 5), 0);

        step /= circle;

        switch (step)
        {
            case 0: return GetHexNeighbor(now, 4);
            case 1: return GetHexNeighbor(now, 3);
            case 2: return GetHexNeighbor(now, 2);
            case 3: return GetHexNeighbor(now, 1);
            case 4: return GetHexNeighbor(now, 0);
            case 5: return GetHexNeighbor(now, 5);
        }

        Debug.LogError("Not meant to be here");

        return now + new Vector2Int(100, 100);
    }

    static Vector2Int GetHexNeighbor(Vector2Int center, int index)
    {
        Vector2Int[] neighbors = new Vector2Int[6];

        if (center.y % 2 == 0)
        {
            neighbors[0] = center + new Vector2Int(0, 1);
            neighbors[1] = center + new Vector2Int(1, 0);
            neighbors[2] = center + new Vector2Int(0, -1);
            neighbors[3] = center + new Vector2Int(-1, -1);
            neighbors[4] = center + new Vector2Int(-1, 0);
            neighbors[5] = center + new Vector2Int(-1, 1);
        }
        else
        {
            neighbors[0] = center + new Vector2Int(1, 1);
            neighbors[1] = center + new Vector2Int(1, 0);
            neighbors[2] = center + new Vector2Int(1, -1);
            neighbors[3] = center + new Vector2Int(0, -1);
            neighbors[4] = center + new Vector2Int(-1, 0);
            neighbors[5] = center + new Vector2Int(0, 1);
        }

        return neighbors[index];
    }


}
