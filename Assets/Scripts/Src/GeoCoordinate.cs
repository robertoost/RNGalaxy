using System;
using UnityEngine;

namespace RNGalaxy
{
    public struct GeoCoordinate : IEquatable<GeoCoordinate>
    {
        // Readonly members to prevent unwanted alterations.
        private readonly float _latitude;
        private readonly float _longitude;

        public float latitude { get { return _latitude; } }
        public float longitude { get { return _longitude; } }

        public GeoCoordinate(float latitude, float longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }
        public GeoCoordinate(Vector3 pointOnUnitSphere) : this()
        {
            _latitude = Mathf.Asin(pointOnUnitSphere.y);
            _longitude = Mathf.Atan2(pointOnUnitSphere.x, -pointOnUnitSphere.z);
        }

        public Vector3 ToVector3()
        {
            float r = Mathf.Cos(latitude);
            float x = Mathf.Sin(longitude) * r;
            float y = Mathf.Sin(latitude);
            float z = -Mathf.Cos(longitude) * r;

            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", latitude, longitude);
        }

        public override bool Equals(System.Object other)
        {
            return other is GeoCoordinate && Equals((GeoCoordinate)other);
        }

        /// <summary>
        /// Compares the current coordinate to another, using approximate float equality to prevent floating point errors.
        /// </summary>
        /// <param name="other">The other lat long coordinates.</param>
        /// <returns>True when latitude and longitude for both coordinates are equal.</returns>
        public bool Equals(GeoCoordinate other)
        {
            bool latEq = UnityEngine.Mathf.Approximately(latitude, other.latitude);
            bool longEq = UnityEngine.Mathf.Approximately(longitude, other.longitude);
            return latEq && longEq;
        }

        public override int GetHashCode()
        {
            return latitude.GetHashCode() ^ longitude.GetHashCode();
        }


    }
}