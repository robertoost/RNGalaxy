using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace RNGalaxy
{
    class PlanetGeneratorUI : MonoBehaviour
    {
		public GUISkin guiSkin;


		Rect windowRect = new Rect(0, 0, 200, 385);

		float[] formCache;
		bool formChanged = false;

		float changeDelay = 0.1f;
		float timePassed = 0;


		private string randomSeedText = "42";
		public int randomSeed = 42;

		float numPointsSliderValue = 2000f;
		public int numPoints = 2000;

		float numPlatesSliderValue = 30f;
		public int numPlates = 30;

		float radiusSliderValue = 7f;
		public float radius = 7f;

		float tileJitterSliderValue = 0f;
		public float tileJitter = 0f;

		float mountainElevationSliderValue = 0.5f;
		public float mountainElevation = 0.5f;

		float roughnessSliderValue = 1f;
		public float roughness = 1f;

		float baseLandHeightSliderValue = 0.5f;
		public float baseLandHeight = 0.5f;

		float rotationSpeed = 45f;

		const int FIELD_WIDTH = 110;
		const int LABEL_WIDTH = 160;

		public PlanetGenerator planetGenerator;
		public Rotator rotator;

		void Start()
		{
			UpdatePlanet();
			windowRect.x = (Screen.width / 3 - windowRect.width) / 2;
			windowRect.y = (Screen.height - windowRect.height) / 2;
		}


		void OnGUI()
		{
			GUI.skin = guiSkin;
			windowRect = GUI.Window(0, windowRect, DoMyWindow, "RNGalaxy Planet Gen.");
		}

		private void Update()
		{
			rotator.speed = rotationSpeed;

			// Compare this cache to the previous one.
			float[] formValues = new float[] { randomSeed, numPoints, numPlates, radius, tileJitter, mountainElevation, roughness, baseLandHeightSliderValue};

			if (formCache == null)
            {
				formCache = formValues;
            }

			// If the form was changed and the changedelay has passed, send the event to the planet generator.
			if (formChanged)
            {
				timePassed += Time.deltaTime;
				if (timePassed > changeDelay)
                {
					formChanged = false;
					timePassed = 0;
					formCache = formValues;
					UpdatePlanet();
					return;
                }
			}

			// Check for changes.
			bool changeFound = false;
			for (int i = 0; i < formValues.Length; i++)
            {
				if (Mathf.Approximately(formValues[i], formCache[i]) == false)
                {
					changeFound = true;
					break;
                }
            }

			// If we keep changing the values, the planet won't update.
			if (changeFound)
            {
				timePassed = 0;
				formCache = formValues;
				formChanged = true;
            }
        }

		void UpdatePlanet()
        {
			planetGenerator.UpdatePlanet(randomSeed, numPoints, numPlates, radius, tileJitter, mountainElevation, roughness, baseLandHeight);
        }

        void DoMyWindow(int windowID)
		{

			GUI.Label(new Rect(20, 40, LABEL_WIDTH, 20), "Random seed.");

			randomSeedText = GUI.TextField(new Rect(15, 65, FIELD_WIDTH, 20), randomSeedText, 25);

            randomSeedText = Regex.Replace(randomSeedText, @"[^a-zA-Z0-9 ]", "");
            randomSeed = int.Parse(randomSeedText);

			numPoints = Slider(ref numPointsSliderValue, "Nr. of points", 90, 200, 10000);

			numPlates = Slider(ref numPlatesSliderValue, "Nr. of plates", 125, 1, 200);

			radius = Slider(ref radiusSliderValue, "Radius", 160, 6f, 8f);

			tileJitter = Slider(ref tileJitterSliderValue, "Tile jitter", 195, 0f, 1f);

			mountainElevation = Slider(ref mountainElevationSliderValue, "Mountain elevation", 230, 0f, 1f);

			roughness = Slider(ref roughnessSliderValue, "Terrain roughness", 265, 0f, 1f);

			baseLandHeight = Slider(ref baseLandHeightSliderValue, "Base land height", 300, 0.1f, 2f);

			rotationSpeed = Slider(ref rotationSpeed, "RotationSpeed", 335, 0f, 500);

			GUI.DragWindow(new Rect(0, 0, 10000, 10000));
		}

		int Slider(ref float sliderValue, string label, float y, int min, int max)
        {
			GUI.Label(new Rect(20, y, LABEL_WIDTH, 20), label);
			sliderValue = GUI.HorizontalSlider(new Rect(15, y + 20, FIELD_WIDTH, 30), sliderValue, min, max);
			sliderValue = Mathf.Round(sliderValue);
			int convertedValue = (int)sliderValue;
			GUI.Label(new Rect(140, y + 20, 60, 30), $"{convertedValue}");

			return convertedValue;
		}

        float Slider(ref float sliderValue, string label, float y, float min, float max)
        {
			GUI.Label(new Rect(20, y, LABEL_WIDTH, 20), label);
			sliderValue = GUI.HorizontalSlider(new Rect(15, y + 20, FIELD_WIDTH, 30), sliderValue, min, max);

			// Easter egg :)
			string valueLabel = $"{Mathf.Round(sliderValue * 1000) / 1000}";
			if (label == "RotationSpeed" && Mathf.Approximately(sliderValue, 500f))
            {
				valueLabel = "AAAA!!!";
            }

			GUI.Label(new Rect(140, y + 20, 70, 30), valueLabel);

			return sliderValue;
		}

	}
}
