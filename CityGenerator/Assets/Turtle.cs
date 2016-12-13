using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum RoadType {Highway, PrimaryStreet, Street}

// Highway: primary routes for traveling throughout the country, from one city to another, over long distances
// PrimaryStreet: Medium-low traffic densities 
// Street: Any road for public travel which does not meet the criteria for any other type
// https://wiki.waze.com/wiki/Road_types/USA#Streets

public struct TurtleState{

	public Vector2 previousPosition;
	public Vector2 position;
	public float angle;
}

public class RoadSegment{

	public Vector2 start;
	public Vector2 end;
	public int branchDepth;
	public RoadType roadType;
	public bool connected = true;
	public int id;
	public int stackID;

	public Vector3[] getLine(){

		return new Vector3[]{start, end};
	}
}

public class Turtle{

	public Vector2 currentPosition {get; private set;}
	public Vector2 previousPosition {get; private set;}

	public List<Vector3> vertices {get; private set;}
	public List<RoadSegment> segments {get; private set;}
	public Stack<TurtleState> states = new Stack<TurtleState>();
	public float distance = 10;
	private float angle = 0;
	public int stackDepth {get; private set;}
	private int id = 0;
	public int stackID = 0;


	//public List<RoadSegment> segments = new List<RoadSegment>();

	public void removeSegment(RoadSegment segment){
		segments.Remove(segment);
	}

	public void initialize(float angle, float distance){
		currentPosition = new Vector2(0f, 0f);
		previousPosition = currentPosition;
		vertices = new List<Vector3>();
		segments = new List<RoadSegment>();

		this.angle = angle;
		this.distance = distance;
		vertices.Add(currentPosition);
		stackDepth = 0;
	}

	public void forward(){


		var angleRadians = angle * Mathf.PI / 180;
		var newX = currentPosition.x + (float)(distance * Mathf.Sin(angleRadians));
		var newY = currentPosition.y + (float)(distance * Mathf.Cos(angleRadians));
		currentPosition = new Vector2(newX, newY);

		foreach(RoadSegment seg in segments){
			if ((almostEqual(seg.start.x, previousPosition.x, 0.01f) && almostEqual(seg.start.y, previousPosition.y, 0.01f) &&
				almostEqual(seg.end.x, currentPosition.x, 0.01f) && almostEqual(seg.end.y, currentPosition.y, 0.01f)) || 
			
				(almostEqual(seg.start.x, currentPosition.x, 0.01f) && almostEqual(seg.start.y, currentPosition.y, 0.01f) &&
					almostEqual(seg.end.x, previousPosition.x, 0.01f) && almostEqual(seg.end.y, previousPosition.y, 0.01f))) {
				previousPosition = currentPosition;
				return;
			}
		}

		RoadSegment segment = new RoadSegment();
		segment.start = previousPosition;
		segment.end = currentPosition;
		segment.branchDepth = stackDepth;
		segment.id = id;
		segment.stackID = stackID;

		segments.Add(segment);
		vertices.Add(currentPosition);
		//Debug.Log(currentPosition);
		previousPosition = currentPosition;
		id++;
	}


	bool almostEqual(float a, float b, float epsilon){

		if (Mathf.Abs(a - b) < epsilon){
			return true;
		}
		return false;

	}

	// NOTICE: excludes the last dead end at branching depth 0
	public List<RoadSegment> getDeadEnds(){ 
		List<RoadSegment> deadEnds = new List<RoadSegment>();
		foreach (RoadSegment segment in segments){
			if (segment.connected == false){
				deadEnds.Add(segment);
			}
		}
		return deadEnds;
	}

	public void push(){
		TurtleState state = new TurtleState();
		state.position = currentPosition;
		state.previousPosition = previousPosition;
		state.angle = angle;
		states.Push(state);
		stackDepth++;
		stackID ++;
	}

	public void pop(){
		currentPosition = states.Peek().position;
		previousPosition = states.Peek().previousPosition;

		angle = states.Peek().angle;
		states.Pop();
		vertices.Add(currentPosition);
		stackDepth--;
		segments[segments.Count-1].connected = false;
	}

	public void rotate(float angleDelta){
		angle += angleDelta;
	}
}