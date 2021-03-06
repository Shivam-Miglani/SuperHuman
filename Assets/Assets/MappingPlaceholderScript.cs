﻿using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class MappingPlaceholderScript : MonoBehaviour
{

	// public Text TextObject;
	public GameObject g;
	public GameObject holocamera;
	public GameObject lava;
	private SpatialUnderstandingDllTopology.TopologyResult[] resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[512];


	int frameCount;
	IntPtr statsPtr;
	public static bool scanning = true;


	private bool draw = true;


	// Use this for initialization
	void Start()
	{
		scanning = true;
	}

	public void ScannedEnough()
	{
		if (scanning)
		{
			Debug.Log(resultsTopology);
			SpatialUnderstanding.Instance.RequestFinishScan();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (scanning)
		{
			this.getSpatialStatistics();
		}
		else
		{
			if (draw)
			{
				this.findPositionsOnFloor();
			}

		}
	}

	void getSpatialStatistics()
	{
		if (this.statsPtr == IntPtr.Zero)
		{
			this.statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
		}

		if ((++this.frameCount % 5 == 0) && SpatialUnderstandingDll.Imports.QueryPlayspaceStats(this.statsPtr) != 0)
		{
			var statistics = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
			//Camera.main.transform.position
			//this.TextObject.text = string.Format("Total m2: {0:G2} | Horizontal m2 {1:G2}", statistics.TotalSurfaceArea, statistics.HorizSurfaceArea);
			if (statistics.HorizSurfaceArea >= 8)
			{
				ScannedEnough();
				scanning = false;
			}
		}



	}

	public void findPositionsOnFloor()
	{
		// Setup
		float minWidthOfWallSpace = 1.0f;
		float minHeightAboveFloor = 1.0f;

		// Only if we're enabled
		if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
		{
			return;
		}

		// Query
		IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
		int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnFloor(
			minWidthOfWallSpace, minHeightAboveFloor,
			resultsTopology.Length, resultsTopologyPtr);


		// Output
		if ((++this.frameCount % 5 == 0) && SpatialUnderstandingDll.Imports.QueryPlayspaceStats(this.statsPtr) != 0)
		{
			//this.TextObject.text = string.Format(" FloorCount {0:G2}", locationCount);

			if (locationCount != 0)
			{
				var rects = resultsTopology.OrderByDescending(r => r.width * r.length).Take(Math.Max(10, resultsTopology.Length));
				//this.TextObject.text = string.Format(" FloorCount {0:G2} | Ordered Rects {1:G2}", locationCount, rects.Count());
				//TODO: remove this for loop
				foreach (var rect in rects)
				{

					//this.TextObject.text = string.Format(" Rect: {0:G2}, {1:G2}", rect.position.x, rect.position.y);
					//g = GameObject.CreatePrimitive(PrimitiveType.Plane);
					g.transform.position = new Vector3(holocamera.transform.position.x, rect.position.y + 0.02f, holocamera.transform.position.z);
					lava.transform.position = new Vector3(holocamera.transform.position.x, rect.position.y - 1.0f, holocamera.transform.position.z);

					//g.transform.
					break;

				}
				draw = false;
				Energy.MapPlaced = true;
			}

		}
	}


}