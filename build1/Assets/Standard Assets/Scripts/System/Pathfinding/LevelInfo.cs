using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// LevelInfo is a (programming) object that contains 2D information about the
/// actual game level you see in while playing (refered to as "3D"/"world" from now on)
/// 
/// Issue: shiftX and shiftY SHOULD be determined during construction by passing
/// an object instead
/// </summary>
/// <exception cref='ArgumentException'>
/// Is thrown when the boundaries are invalid (max > min)
/// </exception>
public class LevelInfo
{
	// aka "epsilon" for float rounding errors
	private static float SLACK = 0.001f;
	
	// contains static info only: walls, obstacles, etc
	// true if occupied (impassible), false otherwise
	// static objects that stay in place
	private bool[,] staticGrid;
	
	// dynamic objects that move
	// this might be better as int something else
	// in case we want to store better information of WHAT dynamically resides there
	private int[,] dynamicGrid; 
	
	// some boundaries
	private float minX = 0.0f;
	private float maxX = 0.0f;
	private float minY = 0.0f;
	private float maxY = 0.0f;
	
	// shift (aka "transformation" in linear algebra) values to
	// go from 3D world -> 2D data and vice versa
	private float shiftX = 0.0f;
	private float shiftY = 0.0f;
	private bool determinedShiftFlag = false;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LevelInfo"/> class.
	/// Please ensure that the all objects are within the boundaries.
	/// </summary>
	/// <param name='minX'>
	/// Minimum x.
	/// </param>
	/// <param name='maxX'>
	/// Max x.
	/// </param>
	/// <param name='minY'>
	/// Minimum y.
	/// </param>
	/// <param name='maxY'>
	/// Max y.
	/// </param>
	/// <exception cref='ArgumentException'>
	/// Is thrown when the boundaries are invalid (max > min)
	/// </exception>
	public LevelInfo (float minX, float maxX, float minY, float maxY)
	{
		if (maxY < minY || maxX < minX) {
			throw new ArgumentException("Level size: Max cannot be smaller than min");
		}
		this.maxX = maxX;
		this.minX = minX;
		this.minY = minY;
		this.maxY = maxY;
		
		staticGrid = new bool[getWidth(), getHeight()];
		resetDynamic();
	}
	
	/// <summary>
	/// Clears the dynamic information stored in the object
	/// </summary>
	public void resetDynamic() {
		dynamicGrid = new int[getWidth(), getHeight()];
	}
	
	/// <summary>
	/// Adds the static object.
	/// </summary>
	/// <param name='position'>
	/// Position of object.
	/// </param>
	/// <param name='size'>
	/// Size of object.
	/// </param>
	public void addStaticObject(Vector3 position, Vector3 size) {
		float width = size.x;
		float height = size.z;
		Vector3 BLposition = new Vector3(-width/2.0f,0,-height/2.0f) + position;
		if (determinedShiftFlag == false) {
			determinedShiftFlag = true;
			float tempX = BLposition.x;
			float tempY = BLposition.z;
			shiftX = tempX - Mathf.Floor(tempX - minX) - SLACK;
			shiftY = tempY - Mathf.Floor(tempY - minY) - SLACK;
			
		}
		Vector2 gridPosition = LevelToGrid(BLposition);
		
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				int gridX = (int)(gridPosition.x + i);
				int gridY = (int)(gridPosition.y + j);
				if (inBounds(new Vector2(gridX, gridY))) {
					// mark those positions as true
					staticGrid[gridX, gridY] = true;
				}

			}
		}
	}
	
	public void removeStaticObject(Vector3 position, Vector3 size) {
		float width = size.x;
		float height = size.z;
		Vector3 BLposition = new Vector3(-width/2.0f,0,-height/2.0f) + position;
		Vector2 gridPosition = LevelToGrid(BLposition);
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				int gridX = (int)(gridPosition.x + i);
				int gridY = (int)(gridPosition.y + j);
				if (inBounds(new Vector2(gridX, gridY))) {
					// mark those positions as false
					staticGrid[gridX, gridY] = false;
				}

			}
		}
	}

	/// <summary>
	/// Adds the dynamic object.
	/// Just pass it transform.position & bounds.size
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	/// <param name='size'>
	/// Size.
	/// </param>
	/// <param name='g'>
	/// A value to "group" this object into
	/// Use values >= 1.
	/// A value of 0 will essentially clear anything added
	/// </param>
	public void addDynamicObject(Vector3 position, Vector3 size, int g) {
		float width = size.x;
		float height = size.z;
		
		Vector3 BLposition = new Vector3(-width/2.0f,0,-height/2.0f) + position;
	
		if (g == MapScript.PLAYER_GROUP) {
			width = 1;
			height = 1;
		}
		
		if (width <= 1 && height <= 1) {
			// just take the center if it's a really small object
			BLposition = position;
		}
	
		Vector2 gridPosition = LevelToGrid(BLposition);
		
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				int gridX = (int)(gridPosition.x + i);
				int gridY = (int)(gridPosition.y + j);
				if (inBounds(new Vector2(gridX, gridY))) {
					// mark those positions as true
					dynamicGrid[gridX, gridY] = g;
				}
			}
		}
	}
	
	public void addDynamicObject(Vector3 position, Vector3 size)
	{
		addDynamicObject(position, size, 1);
	}
	
	/// <summary>
	/// transform 3D world coordinate to 2D data.
	/// Note: does not verify that the coordinates are in bounds.
	/// </summary>
	/// <returns>
	/// The point on the grid.
	/// </returns>
	/// <param name='coord'>
	/// Coordinate in 3D world.
	/// </param>
	public Vector2 LevelToGrid (Vector3 coord) {
		return new Vector2(Mathf.Floor(coord.x-shiftX), Mathf.Floor(coord.z-shiftY));
	}
	
	/// <summary>
	/// Transform 2D data back into 3D world coordinate.
	/// Note: does not verify that the coordinates are in bounds.
	/// </summary>
	/// <returns>
	/// The coordinate in 3D world.
	/// </returns>
	/// <param name='coord'>
	/// The point on the grid.
	/// </param>
	public Vector3 GridToLevel (Vector2 coord) {
		return new Vector3(coord.x+shiftX+0.5f, 0, coord.y+shiftY+0.5f);
	}
	
	/******
	 * One thing to particularly note is array-layout vs coordinate/level-layout:
	 * Sample Map/Level: [x,y]
	 * [0,7].........[10,7]
	 *   .             
	 *   .
	 *   <----- x ---->
	 * [0,0] ....... [10,0]
	 * 
	 * 2D - Array: [x,y]
	 * [0,0] ....... [0,10]
	 * 	.<----- x ---->
	 *  .
	 * 	.
	 *  .
	 *  .
	 * [0,7] .......[10,7]
	 * *****/
	/// <summary>
	/// GET ASCII string of level layout (visual), mainly for debugging purposes
	/// </summary>
	/// <returns>
	/// The ASCII representation.
	/// </returns>
	public string ToASCII() {
		string ascii = "";
		for (int y = getHeight() -1; y >= 0; y--) {
			for (int x = 0; x < getWidth(); x++) {
				if (dynamicGrid[x, y] != 0) {
					ascii +="4"; // just something that will fit monospace
				} else 
				if (staticGrid[x, y] == true) {
					ascii += "X";
				} else {
					ascii += "_";
				}
			}
			ascii += "\n";
		}
		return ascii;
	}
	
	/// <summary>
	/// Gets an image of the map with a width no larger than specified
	/// Image will be scaled to the maximum size possible.
	/// 
	/// Note: this function is very slow.
	/// </summary>
	/// <returns>
	/// The map image.
	/// </returns>
	/// <param name='maxWidth'>
	/// Max width of the image.
	/// </param>
	public Texture2D getMapImage(int maxWidth) {
		int scale = maxWidth / getWidth();
		return getScaledMapImage(scale);
	}
	
	/// <summary>
	/// Gets an image of the map scaled to the specified scale.
	/// 
	/// Note: this function is very slow.
	/// </summary>
	/// <returns>
	/// The map image.
	/// </returns>
	/// <param name='scale'>
	/// The scaling factor. Values < 1 will be treated as 1
	/// </param>
	public Texture2D getScaledMapImage(int scale) {
		if (scale < 1) scale = 1;
		Texture2D map = new Texture2D(getWidth() * scale, getHeight() * scale);
		
		for (int y = getHeight()*scale-1; y >= 0; y--) {
			for (int x = 0; x < getWidth() * scale; x++) {
				if (dynamicGrid[x/scale, y/scale] != 0) {
					map.SetPixel(x, y, groupNumberToColor(dynamicGrid[x/scale, y/scale]));
				} else 
				if (staticGrid[x/scale, y/scale] == true) {
					map.SetPixel(x, y, Color.black);
				} else {
					map.SetPixel(x, y, Color.gray);
				}
			}
		}
		map.Apply();
		return map;
	}
	
	private Color groupNumberToColor(int number) {
		if (number == MapScript.ENEMY_GROUP) {
			return Color.red;
		} 
//		else if (number == MapScript.TRAP_CACHE_GROUP) {
//			return Color.green;
//		} 
//		else if (number == MapScript.POWERUP_GROUP) {
//			return Color.yellow;
//		} 
//		else if (number == MapScript.BASIC_NET_GROUP) {
//			return Color.black;
//		} 
		else if (number == MapScript.PLAYER_GROUP) {
			return Color.yellow;
		} 
		else {
			return Color.white;
		}
	}
	
	/// <summary>
	/// Gets the dimensions of grid (x, y)
	/// </summary>
	/// <returns>
	/// The size. Sadly, vector2 are floats, while these are always ints.
	/// (cast them)
	/// </returns>
	public Vector2 getSize() {
		return new Vector2(getWidth(), getHeight());
	}
	
	/// <summary>
	/// Gets the width of the level
	/// </summary>
	/// <returns>
	/// The width.
	/// </returns>
	public int getWidth() {
		return (int)Mathf.Ceil(maxX - minX);
	}
	
	/// <summary>
	/// Gets the height of the level
	/// </summary>
	/// <returns>
	/// The height.
	/// </returns>
	public int getHeight() {
		return (int)Mathf.Ceil(maxY - minY);
	}
	
	/// <summary>
	/// Checks if the 2D data point fits in the grid
	/// </summary>
	/// <returns>
	/// True if fits into grid, false if out of bounds
	/// </returns>
	/// <param name='coord'>
	/// The 2D data point.
	/// </param>
	public bool inBounds(Vector2 coord) {
		return (coord.x >= 0 &&
		        coord.y >= 0 &&
		        coord.x < getWidth() &&
		        coord.y < getHeight());
	}
	
	/// <summary>
	/// Checks if the 3D world coordinate fits in the grid
	/// </summary>
	/// <returns>
	/// True if fits into grid, false if out of bounds
	/// </returns>
	/// <param name='coord'>
	/// The 3D world coordinate.
	/// </param>
	public bool inBounds(Vector3 coord) {
		return inBounds(LevelToGrid(coord));
	}
	
	/// <summary>
	/// Checks if the 2D data point contains something.
	/// NOTE: will return true if it is outside of bounds
	/// </summary>
	/// <returns>
	/// True if occupied either statically or dynamically
	/// </returns>
	/// <param name='coord'>
	/// The 2D data point.
	/// </param>
	public bool isOccupied(Vector2 coord) {
		if (!inBounds(coord)) return true;
		if (staticGrid[(int)coord.x, (int)coord.y] == true) { return true; }
		if (dynamicGrid[(int)coord.x, (int)coord.y] != 0) { return true; }
		return false;
	}
	
	/// <summary>
	/// Checks if the 3D world coordinate contains something.
	/// NOTE: will return true if it is outside of bounds
	/// </summary>
	/// <returns>
	/// True if occupied either statically or dynamically
	/// </returns>
	/// <param name='coord'>
	/// The 3D world coordinate.
	/// </param>
	public bool isOccupied(Vector3 coord) {
		return isOccupied(LevelToGrid(coord));
	}
	
	/// <summary>
	/// Checks if the 2D data point contains a dynamic object with group
	/// 
	/// NOTE: will return true if it is outside of bounds or statically occupied
	/// </summary>
	/// <returns>
	/// The occupied by.
	/// </returns>
	/// <param name='coord'>
	/// The 2D data point.
	/// </param>
	/// <param name='dynamicGroup'>
	/// The group id when dynamically added objects.
	/// A group id of 0 will return true if nothing dynamically occupies it.
	/// </param>
	public bool isOccupiedBy(Vector2 coord, int dynamicGroup) {
		if (!inBounds(coord)) return true;
		if (staticGrid[(int)coord.x, (int)coord.y] == true) { return true; }
		if (dynamicGrid[(int)coord.x, (int)coord.y] == dynamicGroup) { return true; }
		return false;
	}
	
	/// <summary>
	/// Checks if the 3D world coordinate contains a dynamic object with group
	/// 
	/// NOTE: will return true if it is outside of bounds or statically occupied
	/// </summary>
	/// <returns>
	/// The occupied by.
	/// </returns>
	/// <param name='coord'>
	/// The 3D world coordinate.
	/// </param>
	/// <param name='dynamicGroup'>
	/// The group id when dynamically added objects.
	/// </param>
	public bool isOccupiedBy(Vector3 coord, int dynamicGroup) {
		return isOccupiedBy(LevelToGrid(coord), dynamicGroup);
	}
	
	
	/// <summary>
	/// Randomly determines a point on the 3D world not occupied by static objects
	/// (walls/obstacles)
	/// </summary>
	/// <returns>
	/// The statically unoccupied grid point.
	/// </returns>
	public Vector3 randomStaticallyUnoccupiedWorldPoint() {
		return GridToLevel(randomStaticallyUnoccupiedGridPoint());
	}
	
	/// <summary>
	/// Randomly determines a point on the 3D world not occupied by anything
	/// </summary>
	/// <returns>
	/// The dynamically unoccupied world point.
	/// </returns>
	public Vector3 randomDynamicallyUnoccupiedWorldPoint() {
		return GridToLevel(randomDynamicallyUnoccupiedGridPoint());
	}
	
	/// <summary>
	/// Randomly determines a point on the 2D grid not occupied by static objects
	/// (walls/obstacles)
	/// </summary>
	/// <returns>
	/// The statically unoccupied grid point.
	/// </returns>
	public Vector2 randomStaticallyUnoccupiedGridPoint() {
		Vector2 point = randomGridPoint();
		
		while( isOccupied(point) && isOccupiedBy(point, -99) ){ 
			// check that it's occupied by something (static/dynamic), and not occupied by static
			point = randomGridPoint();
		}
		return point;
	}
	
	/// <summary>
	/// Randomly determines a point on the 2D grid not occupied by anything
	/// </summary>
	/// <returns>
	/// The dynamically unoccupied grid point.
	/// </returns>
	public Vector2 randomDynamicallyUnoccupiedGridPoint() {
		Vector2 point = randomGridPoint();
		
		while( isOccupied(point) ){
			point = randomGridPoint();
		}
		return point;
	}
	
	/// <summary>
	/// Private: generates a random point on the grid
	/// </summary>
	/// <returns>
	/// The grid point.
	/// </returns>
	private Vector2 randomGridPoint() {
		return new Vector2( (int) UnityEngine.Random.Range(0, getWidth()),
							(int) UnityEngine.Random.Range(0, getHeight()));
	}
	
	/// <summary>
	/// Generates a grid point with a distance from the position at a random angle.
	/// </summary>
	/// <returns>
	/// The grid point.
	/// </returns>
	/// <param name='pos'>
	/// Position of the base point.
	/// </param>
	/// <param name='angle'>
	/// Polar angle of which the returned point can rest.
	/// </param>
	/// <param name='distance'>
	/// Distance from the base point and the returned point.
	/// </param>
	/*public Vector3 randomPolarGridPoint(Vector3 pos, float angle, float distance) {
		Vector3 offset = new Vector3(distance,0,0);
		offset = Quaternion.Euler(0,angle * UnityEngine.Random.Range(0,360/angle),0) * offset;
		Vector3 temp = pos + offset;
		temp = new Vector3 ((int) temp.x, 0, (int) temp.z);
		return temp;
	}*/
}


