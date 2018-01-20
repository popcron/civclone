using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path<Node> : IEnumerable<Node>
{
    public Node LastStep { get; private set; }
    public Path<Node> PreviousSteps { get; private set; }
    public int TotalCost { get; private set; }

    private Path(Node lastStep, Path<Node> previousSteps, int totalCost)
    {
        LastStep = lastStep;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
    }

    public Path(Node start) : this(start, null, 0) { }

    public Path<Node> AddStep(Node step, int stepCost)
    {
        return new Path<Node>(step, this, TotalCost + stepCost);
    }

    public IEnumerator<Node> GetEnumerator()
    {
        for (var p = this; p != null; p = p.PreviousSteps)
            yield return p.LastStep;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

class PriorityQueue<P, V>
{
    private readonly SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();

    public void Enqueue(P priority, V value)
    {
        Queue<V> q;
        if (!list.TryGetValue(priority, out q))
        {
            q = new Queue<V>();
            list.Add(priority, q);
        }
        q.Enqueue(value);
    }

    public V Dequeue()
    {
        // will throw if there isn’t any first element!
        var pair = list.First();
        var v = pair.Value.Dequeue();
        if (pair.Value.Count == 0) // nothing left of the top priority.
            list.Remove(pair.Key);
        return v;
    }

    public bool IsEmpty
    {
        get { return !list.Any(); }
    }
}

public static class WorldPathfinding
{
    public static Path<TileObject> FindPath(TileObject start, TileObject destination)
    {
        var closed = new HashSet<TileObject>();
        var queue = new PriorityQueue<double, Path<TileObject>>();
        queue.Enqueue(0, new Path<TileObject>(start));

        while (!queue.IsEmpty)
        {
            var path = queue.Dequeue();

            if (closed.Contains(path.LastStep))
                continue;
            if (path.LastStep.Equals(destination))
                return path;

            closed.Add(path.LastStep);

            foreach (TileObject n in path.LastStep.Neighbours)
            {
                if(n.passable)
                {
                    int d = Distance(path.LastStep, n);
                    var newPath = path.AddStep(n, d);
                    queue.Enqueue(newPath.TotalCost + Estimate(n, destination),
                        newPath);
                }
            }
        }

        return null;
    }

    private static int Distance(TileObject a, TileObject b)
    {
        return 1;
    }

    private static int Estimate(TileObject tile, TileObject destTile)
    {
        float dx = Mathf.Abs(destTile.coordinate.X - tile.coordinate.X);
        float dy = Mathf.Abs(destTile.coordinate.Y - tile.coordinate.Y);
        int z1 = -(tile.coordinate.X + tile.coordinate.Y);
        int z2 = -(destTile.coordinate.X + destTile.coordinate.Y);
        float dz = Mathf.Abs(z2 - z1);

        return (int)Mathf.Max(dx, dy, dz);
    }
}
