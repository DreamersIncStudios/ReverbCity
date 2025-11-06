using System;
using UnityEngine;

namespace DreamersStudio.CameraControlSystem
{

	public class CameraSettings : MonoBehaviour
	{
		//todo add event system for value change;
		[SerializeField] float[] distances = new float[32];
		void Start()
		{

		//	Camera.main.layerCullDistances = distances;
		//	Camera.main.layerCullSpherical = true;
		}

		private void OnValidate()
		{

		}
	}
}