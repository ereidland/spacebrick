using UnityEngine;
using Spacebrick;

public class TreeTest : MonoBehaviour
{
    public int NumIterations = 2000;
    public int NumDeletions = 1000;

    public int MinRange = 1;
    public int MaxRange = 1000;

    private void Start()
    {
        KeyValueTree<int, string> tree = new KeyValueTree<int, string>();

        var watch = new System.Diagnostics.Stopwatch();

        watch.Start();
        for (int i = 0; i < NumIterations; i++)
        {
            int key = Random.Range(MinRange, MaxRange);
            string value = "Hi.";

            if (NumDeletions > 0 && Random.value > 0.5f)
            {
                NumDeletions--;
                tree.Delete(key);
            }
            else
            {
                tree.Set(key, value);
            }
        }

        watch.Stop();

        foreach (var leaf in tree.RawLeaves)
        {
            if (leaf.Key != 0)
                Debug.Log(leaf.Key + " - " + leaf.Value);
        }

        Debug.Log(string.Format("{0} iterations took {1} milliseconds, at {2} iterations/millisecond.", NumIterations, watch.ElapsedMilliseconds, NumIterations/(float)watch.ElapsedMilliseconds));
    }
}
