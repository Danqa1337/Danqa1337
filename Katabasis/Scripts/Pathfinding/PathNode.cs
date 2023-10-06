using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct PathNode
{

	

	public int X;
	public int Y;
    public int2 position => new int2(X, Y);

	public int gCost;
	public int hCost;
	public int fCost;
	public void calculateFCost()
    {
		fCost = gCost + hCost;
    }
	////public int fCost
	//{
	//	get
	//	{
	//		return gCost + hCost;
	//	}
	//}
	public int parentIndex;
	int heapIndex;




	public int index
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}

    public int CompareTo(PathNode nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}

    public static bool operator ==(PathNode A, PathNode B)
    {
		if (A.X == B.X && A.Y == B.Y) return true;
		return false;
	}
	public static bool operator !=(PathNode A, PathNode B)
	{
		if (A.X != B.X || A.Y != B.Y) return true;
		return false;
	}

}

