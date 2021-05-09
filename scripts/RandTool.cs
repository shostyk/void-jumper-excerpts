using UnityEngine;

namespace OlegShostyk
{
    public class RandTool
    {
        public static bool IsChance(float eventChance)
        {
            return Random.Range(0f, 1f) < eventChance;
        }

        public static int RandomSign()
        {
            int r = Random.Range(0, 2);
            return r == 0 ? -1 : 1;
        }
    }
}
