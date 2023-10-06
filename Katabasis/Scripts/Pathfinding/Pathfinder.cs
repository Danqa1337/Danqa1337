using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Debug = System.Diagnostics.Debug;


public struct FindPathJob : IJob
{
    [ReadOnly] public Entity creature;
	[ReadOnly] public int tryLimit;
	[ReadOnly] public int2 startPosition;
	[ReadOnly] public int2 targetPosition;
    public NativeList<int2> path;
	[ReadOnly] public NativeArray<TileData> tileDataArray;
	[ReadOnly] public NativeArray<int2> neiborOffsets;

	public void Execute()
    {
		
		NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(4096, Allocator.Temp);
		for (int i = 0; i < 4096; i++)
		{
			int2 position = i.ToMapPosition();
			int gCost = int.MaxValue;
			int hCost = GetHCost(position, targetPosition);
			pathNodeArray[i] = new PathNode()
			{
				X = position.x,
				Y = position.y,
				index = i,

				gCost = gCost,
				hCost = hCost,
				fCost = gCost + hCost,

				parentIndex = -1,

			};
		}

		int endNodeIndex = targetPosition.ToMapIndex();
		PathNode startNode = pathNodeArray[startPosition.ToMapIndex()];
		startNode.gCost = 0;
		startNode.calculateFCost();
		pathNodeArray[startNode.index] = startNode;



		NativeList<int> openList = new NativeList<int>(Allocator.Temp);
		NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

		openList.Add(startNode.index);



		int t = 0;

		//if (creature == null || creature == Player.i) tLimit = 10000;

		while (openList.Length > 0 && t < tryLimit)
		{
			t++;
			int currentIndex = GetLowestFCostIndex(openList, pathNodeArray);
			PathNode currentNode = pathNodeArray[currentIndex];


			if (currentIndex == endNodeIndex)
			{
				//Path Completed!
				break;

			}
			else
			{
				//Remove current from openlist
				for (int i = 0; i < openList.Length; i++)
				{
					if (openList[i] == currentIndex)
					{
						openList.RemoveAtSwapBack(i);
						break;
					}
				}

				closedList.Add(currentIndex);
				for (int i = 0; i < neiborOffsets.Length; i++)
				{
					int2 offset = neiborOffsets[i];
					int2 position = new int2(currentNode.X + offset.x, currentNode.Y + offset.y);


					int neiborIndex = position.ToMapIndex();
					

					//check valid position
					if (neiborIndex == -1) continue;
					if (closedList.Contains(neiborIndex)) continue;

					PathNode neiborNode = pathNodeArray[neiborIndex];

					if (!tileDataArray[neiborNode.index].isWalkable(creature)
                        && !(position.Equals(startPosition) || position.Equals(targetPosition))) continue;

					int tentativeGCost = currentNode.gCost + GetHCost(currentNode, neiborNode);
					if (tentativeGCost < neiborNode.gCost)
					{
						neiborNode.parentIndex = currentNode.index;
						neiborNode.gCost = tentativeGCost;
						neiborNode.calculateFCost();
						pathNodeArray[neiborNode.index] = neiborNode;

						if (!openList.Contains(neiborNode.index))
						{
							openList.Add(neiborNode.index);
						}
					}
				}
			}
		}

		PathNode endNode = pathNodeArray[endNodeIndex];

		if (endNode.parentIndex == -1)
		{
            UnityEngine.Debug.Log("can not complete path from " + startPosition + " to " + targetPosition);
            path.CopyFrom(new int2[0]);
        }
		else
        {
            var retraced = RetracePath(pathNodeArray, endNode);
			path.CopyFrom(retraced);
            retraced.Dispose();
        }

		pathNodeArray.Dispose();
		openList.Dispose();
		closedList.Dispose();
	}
	private static NativeList<int2> RetracePath(NativeArray<PathNode> pathnodeArray, PathNode endNode)
	{
		if (endNode.parentIndex == -1)
		{
			//Path Not Found

			return new NativeList<int2>(Allocator.Temp);
		}
		else
		{
			//Path Found
			NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
			NativeList<int2> reversedPath = new NativeList<int2>(Allocator.Temp);

			reversedPath.Add(new int2(endNode.X, endNode.Y));
			PathNode currentNode = endNode;
			
			while (currentNode.parentIndex != -1)
			{
				PathNode parent = pathnodeArray[currentNode.parentIndex];
				reversedPath.Add(new int2(currentNode.X, currentNode.Y));
				currentNode = parent;

			}
			
            for (int i = reversedPath.Length-1; i >= 0; i--)
            {
				path.Add(reversedPath[i]);
            }

            reversedPath.Dispose();
			return path;
		}
	}

	private static int GetLowestFCostIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
	{
		PathNode lowestFNode = pathNodeArray[openList[0]];
		for (int i = 0; i < openList.Length; i++)
		{
			PathNode testNode = pathNodeArray[openList[i]];
			if (testNode.fCost < lowestFNode.fCost)
			{
				lowestFNode = testNode;
			}
		}
		return lowestFNode.index;
	}
	static int GetHCost(PathNode A, PathNode B)
	{
		return GetHCost(A.X, A.Y, B.X, B.Y);
	}
	static int GetHCost(int2 A, int2 B)
	{
		return GetHCost(A.x, A.y, B.x, B.y);
	}

	static int GetHCost(int AX, int AY, int BX, int BY)
	{
		int dstX = math.abs(AX - BX);
		int dstY = math.abs(AY - BY);
		int remaining = math.abs(dstX - dstY);

		return 14 * Mathf.Min(dstY, dstX) + 10 * remaining;
	}
}
public class PathFinderQuery
{
	public TileData startTile;
	public TileData targetTile;
	public Entity creature;
	public bool canMoveDiagonaly = true;
	public int tryLimit = 100;
	public NativeList<int2> result;
	public JobHandle jobHandle;
	public PathFinderPath Path;

    public PathFinderQuery(TileData startTile, TileData targetTile, Entity creature, bool canMoveDiagonaly = true, int tryLimit = 100)
    {
        this.startTile = startTile;
        this.targetTile = targetTile;
        this.creature = creature;
        this.canMoveDiagonaly = canMoveDiagonaly;
        this.tryLimit = tryLimit;

		
    }

    public void Complete()
    {
		
		jobHandle.Complete();

		Path.tiles = new List<TileData>();
		if (result.IsCreated && result.Length > 0)
		{
			foreach (var item in result)
			{
				Path.tiles.Add(item.ToTileData());
			}
			result.Dispose();
		}
		else
		{
			Path = PathFinderPath.NUll;
		}

		
		//Pathfinder.activeQueries.Remove(this);
	}
}




public class Pathfinder
{
	//Node[,] grid;

//	public static List<PathFinderQuery> activeQueries = new List<PathFinderQuery>();
	private static float startTime;

 
    public static PathFinderPath FindPath(TileData startTile, TileData targetTile, Entity creature, int tryLimit = 100)
    {
		var query = new PathFinderQuery(startTile, targetTile, creature, true, tryLimit);
        FindPath(query);
		query.Complete();
		return query.Path;

    }


    public static PathFinderPath FindPath(TileData startTile, TileData targetTile, 
        bool canMoveDiagonaly = true,
        int tryLimit = 100)
    {

		var query = new PathFinderQuery(startTile, targetTile, Entity.Null, true, tryLimit);
		FindPath(query);
		query.Complete();
		return query.Path;
	}



	public static void FindPath(PathFinderQuery query)
    {
		BaseMethodsClass.StartTest();
        if (query.startTile == query.targetTile)
        {
            throw new NullReferenceException("start is target");
        }
        
		startTime = Time.realtimeSinceStartup;
		


		Debug.Print("!!!!");
        var offsets = TileUpdater.NeiborsOffsetsArray8;
		if(!query.canMoveDiagonaly) offsets = TileUpdater.NeiborsOffsetsArray4;
		query.result = new NativeList<int2>(2, Allocator.TempJob);
		var job = new FindPathJob()
		{
			tryLimit = query.tryLimit,
			startPosition = query.startTile.position,
			targetPosition = query.targetTile.position,
            path = query.result,
			neiborOffsets = offsets,
			tileDataArray = TileUpdater.current,
			creature = query.creature,
		};

		query.jobHandle = job.Schedule();

		//jobs.Add(handle);
		BaseMethodsClass.EndTest("pathfinding took ");
	
	}
	
	

	public static void Complete()
	{
		//foreach (var item in activeQueries)
		//{
		//	item.Complete();
			
		//}
	}
	
}



[BurstCompile]
public struct PathFinderPath
{ 
	public  static bool operator ==(PathFinderPath path1, PathFinderPath path2)
    {
        return path1.Equals(path2);
    }
    public  static bool operator !=(PathFinderPath path1, PathFinderPath path2)
    {
        return !path1.Equals(path2);
    }

    public static PathFinderPath NUll;
	public int length
	{
        get
		{
			if (tiles != null) return tiles.Count;
			else return 0;
		}
		

	}
	public List<TileData> tiles;
	public TileData startTile => tiles[0];

    public TileData targetTile
    {
        get
        {
            if(tiles.Count > 0) return tiles[tiles.Count - 1];
            return  TileData.Null;
        }
    }

    public HashSet<TileData> waySide
    {
		get
        {

			HashSet<TileData> side = new HashSet<TileData>();
            foreach (var tile in tiles)
            {
                foreach (var neibor in tile.GetNeibors(true))
                {
					if (neibor != TileData.Null && !tiles.Contains(neibor))  side.Add(neibor);
                } 
            }
            
			return side;
        }
    }

	public PathFinderPath(List<int2> vectors)
    {
		tiles = new List<TileData>();
        foreach (var item in vectors)
        {
			tiles.Add(item.ToTileData());
        }

    }

	public PathFinderPath(List<TileData> tiles)
    {
        this.tiles = tiles;
		
    }

	public PathFinderPath Shorten(TileData tile)
    {
		tiles.Remove(tile);
        return new PathFinderPath(tiles);
    }

}


