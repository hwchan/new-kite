using UnityEngine;
using System.Collections;

public class AStar {
	
	
	// limit on the number of steps to look in the algorithm (may be useful for efficieny)
	private static int STEP_LIMIT = 999; 
	// true to not allow diagonal movements
	private static bool NO_DIAGONAL_MOVEMENT = true;
	// straight line if possible instead of grid-like movements
	private static bool DONT_MOVE_GRID_BASED = true;
	
	private static int DIAG_COST = 14; // ~sqrt(2) * 10
	private static int LINEAR_COST = 10; // 1 * 10

	private AStar(){}
	
	/// <summary>
	/// Find the vector telling next location to go to.
	/// if fails to find a solution, it returns the target by default
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='mapInfo'>
	/// LevelInfo containing info on the level.
	/// </param>
	public static Vector2 Find(Vector2 position, Vector2 target, LevelInfo mapInfo) {
		if (canGoStraightLine(position, target, mapInfo)) return target;
		
		Vector2 defaultRtrnLoc = target; // by default tell to go straight to target
		
		if (!mapInfo.inBounds(position)) { 
			// this SHOULDN'T happen, you should not be out of bounds of the level
			return defaultRtrnLoc;
		}
		
		int xPos = (int)position.x;
		int yPos = (int)position.y;
		int xTarget = (int)target.x;
		int yTarget = (int)target.y;
		
		// use 3th dimension to store c(p) in f(p) = h(p) + c(p)
		PriorityQueue<Vector3> frontier = new PriorityQueue<Vector3>();
		
		// add start position to frontier
		int cp = 0;
		int fp = getHP(position, target) + cp;
		frontier.enqueue(fp, new Vector3(xPos, yPos, cp));
		
		// backtracking (cpsc320 anyone?)
		// might not be the optimal method, but it works
		// Vector3, x, y map to coords, z is used for cost
		// z = -1 if visited, non-neg integer if not visited = cost to reach location
		Vector3[,] backtrack = new Vector3[(int)mapInfo.getSize().x, (int)mapInfo.getSize().y];
		// set the backtrack for position to -1,-1
		backtrack[xPos, yPos] = new Vector3(-1, -1, 1);
		
		for (int i = 0; i < STEP_LIMIT; i++) {
			if (frontier.isEmpty()) {
				return defaultRtrnLoc; // can't reach it...
			}
			
			// select lowest f(p) = h(p) + c(p)
			Vector3 nextDequeue = frontier.dequeue();
			Vector2 nextLocation = new Vector2(nextDequeue.x, nextDequeue.y);
			
			// double check that this location has not been visited
			if (backtrack[(int)nextLocation.x, (int)nextLocation.y].z == -1) {
				continue;
			}
			
			// check if goal
			if ((int)nextLocation.x == xTarget && (int)nextLocation.y == yTarget) {
				
				// backtrack from this position....
				// find first step to move
				return getBacktrack(backtrack, target, position, mapInfo);
			}
			// mark location as accessed
			backtrack[(int)nextLocation.x, (int)nextLocation.y].z = -1;
			
			bool verify_diag = false;
			// otherwise add neighbour of those to frontier that:
			for (int deltaX = -1; deltaX <= 1; deltaX++) {
				for (int deltaY = -1; deltaY <=1; deltaY++) {
					verify_diag = false;
					if (NO_DIAGONAL_MOVEMENT && (deltaX * deltaY != 0)) { 
						continue; 
					} else {
						if (deltaX * deltaY != 0) {
						// verify that we can actually move diagonally	
							verify_diag = true;
						}
					}
					
					Vector2 neighbour = nextLocation + new Vector2(deltaX, deltaY);
					
					int thisCP = (int)nextDequeue.z + Mathf.Abs(deltaX * deltaY) * DIAG_COST 
									+ LINEAR_COST;
					int thisFP = getHP(neighbour, target) + thisCP;
					
					// a) is within boundaries
					if (!mapInfo.inBounds(neighbour)) { continue; }
					
					// b) have not been visited or it's cheaper to go this way
					Vector3 locInfo = backtrack[(int)neighbour.x, (int)neighbour.y];
					if (locInfo != Vector3.zero && locInfo.z < thisCP) { continue; }
					
					// c) are accessible from this location (walkable as specified by mapInfo)
					if (mapInfo.isOccupiedBy(neighbour, MapScript.ENEMY_GROUP)) { continue; }
					if (verify_diag) {
						if (mapInfo.isOccupiedBy(nextLocation + new Vector2(0, deltaY),
							MapScript.ENEMY_GROUP)) { continue; }
						if (mapInfo.isOccupiedBy(nextLocation + new Vector2(deltaX, 0),
							MapScript.ENEMY_GROUP)) { continue; }
					}
					
					
					// all criteria satisfied
					// add to frontier
					frontier.enqueue(thisFP, new Vector3(neighbour.x, neighbour.y, thisCP));
					
					// add to allow backtrack
					backtrack[(int)neighbour.x, (int)neighbour.y] = 
						new Vector3(nextLocation.x, nextLocation.y, thisCP);
				}
			}
		}
		// outta steps, return the closest one found so far
		if (frontier.isEmpty()) {
			return defaultRtrnLoc; // can't reach it...don't move?
		} else {
			Vector3 nextDequeue = frontier.dequeue();
			Vector2 nextLocation = new Vector2(nextDequeue.x, nextDequeue.y);
			return getBacktrack(backtrack, nextLocation, position, mapInfo);
		}
	}
	
	// takes in pos and target as 3D world coordinates
	public static Vector3 Find(Vector3 position, Vector3 target, LevelInfo mapInfo) {
		// optional todo: first perform a raycast from position to target
		// if there is nothing blocking
		// simply return target
		// Note: to do this properly, we'd need a layerMask as to not
		// "collide" with the player (meaning it'd always return false)
		
		// delegate/reduce to 2D data version
		Vector2 gridVersion = Find(mapInfo.LevelToGrid(position), mapInfo.LevelToGrid(target), mapInfo);
		return mapInfo.GridToLevel(gridVersion);
	}
	
	// f(p) = h(p) + c(p)
	// get h(p) based on target and start
//	static private int getHP(int xPos, int yPos, int xTarget, int yTarget) {
//		return (Mathf.Abs(xTarget - xPos) + Mathf.Abs(yTarget - yPos)) * LINEAR_COST;
//	}
	// unused & incomplete
	private static int getHP(Vector2 pos, Vector2 target) {
		return (int)(getDistance(pos, target) * LINEAR_COST);
	}
	
	
	/***
	 * Flee A* implementation
	 * - what this does is similiar to A*. It searches and returns the location with the highest "distance"
	 *   from the player. It has a limit to the distance it is willing to search (o), and it has a distance
	 *   that it (P) refuses to come close to (x) the target (T)
	 * 
     *   Think of it as a donut shape centered around the target. The width of the donut depends on the distance
     *   between the position and the target
	 *    --------------------------
	 *    |  ooooooooo              |
	 *    | oooooxooooo             |
	 *    |ooPooxxxooooo            |
	 *    |ooooxxTxxooooo           |
	 *    |oooooxxxooooo            |
	 *    | oooooxooooo             |
	 *    |  ooooooooo              |
	 *     -------------------------
	 * 
	 * */
	
	public static Vector2 Flee(Vector2 position, Vector2 target, LevelInfo mapInfo) {
		float minDistancePercentage = 50.0f; //%
		float maxDistancePercentage = 350.0f; //%
		// if position and target are "r" units apart
		// will find cheapest path to farthest location that does not get within r*50% of
		// target, and no farther than r*350% of target
		// (of course, if this is repeatedly called, as you get farther from target
		// your search range increases
		
		// Alternatively, think of it as how "panic'd" the AI is.
		// if a AI running away from the target is really close
		// it will "panic" and simply tries to get slightly farther away
		// as it reaches farther away, it "relaxes" and makes a better analysis of the
		// possible paths.
		
		float distance = getDistance(position, target);
		int minDistance = (int)(distance * minDistancePercentage / 100.0f) - 1;
		int maxDistance = (int)(distance * maxDistancePercentage / 100.0f) + 1; 
		
		Vector2 defaultRtrnLoc = position;//position + (position - target); // by default tell to go away
		
		if (!mapInfo.inBounds(position)) { 
			// this SHOULDN'T happen, you should not be out of bounds of the level
			return defaultRtrnLoc;
		}
		
		int xPos = (int)position.x;
		int yPos = (int)position.y;
		
		
		// never go farther than twice the distance (computational resource reasons)
		
		// use 3rd dimension to store c(p) in f(p) = h(p) + c(p)
		PriorityQueue<Vector3> frontier = new PriorityQueue<Vector3>();
		
		// farthest location so far
		Vector2 farthestPoint = position;
		int farthestPointFP = int.MaxValue;
		
		// add start position to frontier
		int cp = 0;
		int fp = getFleeHP(position, target) + cp;
		frontier.enqueue(fp, new Vector3(xPos, yPos, cp));
		
		// backtracking (cpsc320 anyone?)
		// might not be the optimal method, but it works
		// Vector3, x, y map to coords, z is used for cost
		// z = -1 if visited, non-neg integer if not visited = cost to reach location
		Vector3[,] backtrack = new Vector3[(int)mapInfo.getSize().x, (int)mapInfo.getSize().y];
		// set the backtrack for position to -1,-1
		backtrack[xPos, yPos] = new Vector3(-1, -1, 1);
		
		for (int i = 0; i < STEP_LIMIT; i++) {
			if (frontier.isEmpty()) {
				//return defaultRtrnLoc; // can't reach it...
				return getBacktrack(backtrack, farthestPoint);
			}
			
			// select lowest f(p) = h(p) + c(p)
			Vector3 nextDequeue = frontier.dequeue();
			Vector2 nextLocation = new Vector2(nextDequeue.x, nextDequeue.y);
			
			// double check that this location has not been visited
			if (backtrack[(int)nextLocation.x, (int)nextLocation.y].z == -1) {
				continue;
			}
			
			// mark location as accessed
			backtrack[(int)nextLocation.x, (int)nextLocation.y].z = -1;
			
			bool verify_diag = false;
			// otherwise add neighbour of those to frontier that:
			for (int deltaX = -1; deltaX <= 1; deltaX++) {
				for (int deltaY = -1; deltaY <=1; deltaY++) {
					verify_diag = false;
					if (NO_DIAGONAL_MOVEMENT && (deltaX * deltaY != 0)) { 
						continue; 
					} else {
						if (deltaX * deltaY != 0) {
						// verify that we can actually move diagonally	
							verify_diag = true;
						}
					}
					
					Vector2 neighbour = nextLocation + new Vector2(deltaX, deltaY);
					float neighbourDistance = getDistance(neighbour, target);
					
					int thisCP = (int)nextDequeue.z + Mathf.Abs(deltaX * deltaY) * DIAG_COST
									+ LINEAR_COST;
					// increase the cost if it means we have to go CLOSER to the target (than originally)
//					if (neighbourDistance > distance && neighbourDistance > 3) {
//						thisCP += (int)(neighbourDistance - distance);
//					}
					int thisFP = getFleeHP(neighbour, target) + thisCP;
					
					// a) is within boundaries
					if (!mapInfo.inBounds(neighbour)) { continue; }
					
					// b) have not been visited or it's cheaper to go this way
					Vector3 locInfo = backtrack[(int)neighbour.x, (int)neighbour.y];
					if (locInfo != Vector3.zero && locInfo.z < thisCP) { continue; }
					
					// c) are accessible from this location (walkable as specified by mapInfo)
					if (mapInfo.isOccupiedBy(neighbour, MapScript.ENEMY_GROUP)) { continue; }
					if (verify_diag) {
						if (mapInfo.isOccupiedBy(nextLocation + new Vector2(0, deltaY),
							MapScript.ENEMY_GROUP)) { continue; }
						if (mapInfo.isOccupiedBy(nextLocation + new Vector2(deltaX, 0),
							MapScript.ENEMY_GROUP)) { continue; }
					}
					
					
					// d) within our search bounds
					if (neighbourDistance < minDistance || neighbourDistance > maxDistance) {
						continue;
					}
					
					// UPDATE farthestPOINT!
					if (thisFP < farthestPointFP) { 
						farthestPoint = neighbour;
						farthestPointFP = thisFP;
					}
					
					// all criteria satisfied
					// add to frontier
					frontier.enqueue(thisFP, new Vector3(neighbour.x, neighbour.y, thisCP));
					
					// add to allow backtrack
					backtrack[(int)neighbour.x, (int)neighbour.y] = 
						new Vector3(nextLocation.x, nextLocation.y, thisCP);
				}
			}
		}
		// outta steps, return the closest one found so far
		if (frontier.isEmpty()) {
			return getBacktrack(backtrack, farthestPoint); // can't reach it, return farthest found so far
		} else {
			Vector3 nextDequeue = frontier.dequeue();
			Vector2 nextLocation = new Vector2(nextDequeue.x, nextDequeue.y);
			return getBacktrack(backtrack, nextLocation);
		}
	}
	

	
	// takes in pos and target as 3D world coordinates
	public static Vector3 Flee(Vector3 position, Vector3 target, LevelInfo mapInfo) {
		// delegate/reduce to 2D data version
		Vector2 gridVersion = Flee(mapInfo.LevelToGrid(position), mapInfo.LevelToGrid(target), mapInfo);
		return mapInfo.GridToLevel(gridVersion);
	}
	
	
	// f(p) = h(p) + c(p)
	// get h(p) based on target and start
//	static private int getFleeHP(int xPos, int yPos, int xTarget, int yTarget) {
//		return int.MaxValue / 2 - getHP(xPos, yPos, xTarget, yTarget) * 3;
//	}	
	private static int getFleeHP(Vector2 position, Vector2 target) {
		return int.MaxValue / 2 - getHP(position, target) * 3;
	}
	
	
	private static float getDistance(Vector2 a, Vector2 b) {
		if (NO_DIAGONAL_MOVEMENT) {
			return manhattenDist(a, b);					
		} else {
			return euclideanDist(a, b);
		}
	}
	
	// Manhatten distance;
	private static int manhattenDist(Vector2 a, Vector2 b) {
		return (int)(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y));
	}
	
	// euclidean distance;
	private static float euclideanDist(Vector2 a, Vector2 b) {
		return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
	}

	/****
	 * Helper methods
	 * */
	
	// helper method for backtracking
	static private Vector2 getBacktrack(Vector3[,] backtrack, Vector2 target) {
		
		int pathX = (int)target.x;
		int pathY = (int)target.y;
		
		if ((int)backtrack[pathX, pathY].x == -1) return target; // on the same square

		 while(pathX != -1) {
			// get the previous location
			Vector3 prevLoc = backtrack[pathX, pathY];
			
			Vector3 back2Loc = backtrack[(int)prevLoc.x, (int)prevLoc.y];
			if ((int)back2Loc.x == -1) return (new Vector2(pathX, pathY));
			pathX = (int)prevLoc.x;
			pathY = (int)prevLoc.y;
		} 
		// this should theoretically never be returned if backtracking worked
		return target;
	}
	
	static private Vector2 getBacktrack(Vector3[,] backtrack, Vector2 target, Vector2 source, LevelInfo mapInfo) {
		
		int pathX = (int)target.x;
		int pathY = (int)target.y;
		
		if ((int)backtrack[pathX, pathY].x == -1) return target; // on the same square

		 while(pathX != -1) {
			// get the previous location
			Vector3 prevLoc = backtrack[pathX, pathY];
			
			Vector3 back2Loc = backtrack[(int)prevLoc.x, (int)prevLoc.y];
			if ((int)back2Loc.x == -1) return (new Vector2(pathX, pathY));
			pathX = (int)prevLoc.x;
			pathY = (int)prevLoc.y;
			
			if (canGoStraightLine(source, new Vector2(pathX, pathY), mapInfo)) return new Vector2(pathX, pathY);
		} 
		// this should theoretically never be returned if backtracking worked
		return target;
	}
	
	static private bool canGoStraightLine(Vector2 src, Vector2 dest, LevelInfo mapInfo) {
		if (DONT_MOVE_GRID_BASED == false) return false;
		
		int minX = (int)Mathf.Min(src.x, dest.x);
		int maxX = (int)Mathf.Max(src.x, dest.x);
		int minY = (int)Mathf.Min(src.y, dest.y);
		int maxY = (int)Mathf.Max(src.y, dest.y);

		for( int i = minX; i <= maxX; i++) {
			for (int j = minY; j <= maxY; j++) {
				if (i == (int)src.x && j == (int)src.y) continue;
				if (i == (int)dest.x && j == (int)dest.y) continue;

				if (mapInfo.isOccupied(new Vector2(i, j))) return false;
			}
		}
		
		return true;
	}
	
}
