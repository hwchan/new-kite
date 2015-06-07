using UnityEngine;
using System.Collections;

public static class AnimationScript {

	/// <summary>
	/// Animates the sprite.
	/// </summary>
	/// <param name='render'>
	/// The mesh renderer to play the animation on.
	/// </param>
	/// <param name='colSize'>
	/// # of total columns in the sprite sheet.
	/// </param>
	/// <param name='rowSize'>
	/// # of total rows in the sprite sheet.
	/// </param>
	/// <param name='rowFrameStart'>
	/// Indicates which frame # to start in the x axis of the sprite sheet - (STARTS AT 0).
	/// </param>
	/// <param name='colFrameStart'>
	/// Indicates which frame # to start in the y axis of the sprite sheet (STARTS AT 0).
	/// </param>
	/// <param name='totalFrames'>
	/// The total number of frames in the animation along the x-axis.
	/// </param>
	/// <param name='fps'>
	/// The Framerate (frames per second).
	/// </param>
	public static void AnimateSprite(Renderer render, int colSize, int rowSize, int rowFrameStart, int colFrameStart, int totalFrames, int fps){
		if(render != null){
			// Calculate the index as a factor off of time
			int index = (int) (Time.time * fps);
			index = index % totalFrames;
			
			// Calculate individual sprite offsets & u/v coordinates
			Vector2 size = new Vector2 ( 1.0f / colSize, 1.0f/rowSize);
			float u = index % colSize;
			float v = index / colSize;
			
			// Use the above information to set the offset vector
			Vector2 offset = new Vector2 ( (u+rowFrameStart) * size.x, (1.0f - size.y) - (v + colFrameStart)*size.y);
			
			//Debug.Log("frame: " + index + " u: " + u + " v: " + v + " offsetX: " + offset.x + " offsetY: " + offset.y);
			
			// Apply the size and offset vectors to the material texture (ie. Sprite sheet)
			render.material.mainTextureOffset = offset;
			render.material.mainTextureScale  = size;
			
			render.material.SetTextureOffset("_MainTex", offset);
			render.material.SetTextureScale("_MainTex", size);
		}
			
	}
	
	/// <summary>
	/// Animates the sprite. Use this when animations off of the same material (sprite sheet) must run independently.
	/// </summary>
	/// <param name='creationTime'>
	/// The time when the object was first created
	/// </param>
	/// <param name='render'>
	/// The mesh renderer to play the animation on.
	/// </param>
	/// <param name='colSize'>
	/// # of total columns in the sprite sheet.
	/// </param>
	/// <param name='rowSize'>
	/// # of total rows in the sprite sheet.
	/// </param>
	/// <param name='rowFrameStart'>
	/// Indicates which frame # to start in the x axis of the sprite sheet - (STARTS AT 0).
	/// </param>
	/// <param name='colFrameStart'>
	/// Indicates which frame # to start in the y axis of the sprite sheet (STARTS AT 0).
	/// </param>
	/// <param name='totalFrames'>
	/// The total number of frames in the animation along the x-axis.
	/// </param>
	/// <param name='fps'>
	/// The Framerate (frames per second).
	/// </param>	
	public static void AnimateSprite(float creationTime, Renderer render, int colSize, int rowSize, int rowFrameStart, int colFrameStart, int totalFrames, int fps){
		if(render != null){
			// Calculate the index as a factor off of time
			int index = (int) ((Time.time - creationTime) * fps);
			index = index % totalFrames;
			
			// Calculate individual sprite offsets & u/v coordinates
			Vector2 size = new Vector2 ( 1.0f / colSize, 1.0f/rowSize);
			float u = index % colSize;
			float v = index / colSize;
			
			// Use the above information to set the offset vector
			Vector2 offset = new Vector2 ( (u+rowFrameStart) * size.x, (1.0f - size.y) - (v + colFrameStart)*size.y);
			
			//Debug.Log("frame: " + index + " u: " + u + " v: " + v + " offsetX: " + offset.x + " offsetY: " + offset.y);
			
			// Apply the size and offset vectors to the material texture (ie. Sprite sheet)
			render.material.mainTextureOffset = offset;
			render.material.mainTextureScale  = size;
			
			render.material.SetTextureOffset("_MainTex", offset);
			render.material.SetTextureScale("_MainTex", size);
		}
	}
}
