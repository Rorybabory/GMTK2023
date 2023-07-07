using System;
using System.Collections.Generic;
using UnityEngine;


//This is a utility namespace made by Blake Rubadue
namespace GGUtil
{
    [Serializable]
    public struct ClampFloat : IEquatable<ClampFloat>
    {
        public float Min;
        public float Max;

        [SerializeField] float _Value;

        public ClampFloat(float _Min = 0, float _Max = 1, float _val = 0) //constructor
        {
            Min = _Min;
            Max = _Max;
            _Value = Mathf.Clamp(_val, Min, Max);
        }

        public float Value //modifier and accessor for the value. stored as private for displaying 
        {
            get { return _Value; }
            set { _Value = Mathf.Clamp(value, Min, Max); }
        }

        public bool Equals(ClampFloat other)
        {
            return (Min == other.Min && Max == other.Max && Value == other.Value);
        }
    }
    
    [Serializable]
    public struct ClampFloat01 : IEquatable<ClampFloat01>
    {
        [SerializeField] float _Value;

        public ClampFloat01(float _val = 0) //constructor
        {
            _Value = Mathf.Clamp01(_val);
        }

        public float Value //modifier and accessor for the value. stored as private for displaying 
        {
            get { return _Value; }
            set { _Value = Mathf.Clamp01(value); }
        }

        public bool Equals(ClampFloat01 other)
        {
            return (Value == other.Value);
        }
    }

    [Serializable]
    public struct RangeF
    {
        public float Min;
        public float Max;

        public RangeF(float _min = 0, float _max = 1)
        {
            Min = _min;
            Max = _max;
        }
    }

    [Serializable]
    public struct GGRangeInt
    {
        public int start;

        public int length;

        public int end => start + length;

        public GGRangeInt(int start, int length)
        {
            this.start = start;
            this.length = length;
        }
    }

    public class BlerpSpring
    {
        public double Target;
        public double Acceleration;
        public double Drag;
        public float Clamp;

        public double Current;
        public double Velocity;
        
        public BlerpSpring(double value = 0, double target = 0, double initialVelocity = 0, double acceleration = 500, double drag = 5E-9f, float max = -1)
        {
            Current = value;
            Velocity = initialVelocity;
            Target = target;
            Acceleration = acceleration;
            Drag = drag;
            Clamp = max;
        }

        public void Update(float dt)
        {
            GGMath.Blerp(ref Current, ref Velocity, Target, Clamp, Acceleration, Drag, dt);
        }

        public void Reset(double _Value = 0, double _Velocity = 0)
        {
            Current = _Value;
            Velocity = _Velocity;
        }
    }

    public static class GGMath
    {
        //------------------------------------------- BLERP -------------------------------------------------------//
        #region Blerp
        /// <summary>
        /// Bouncy Interpolation <br></br>
        /// </summary>
        /// https://www.desmos.com/calculator/jwrtshsuqn Desmos Graph Link
        public static Vector3 Blerp(ref Vector3 Current, ref Vector3 Velocity, Vector3 Target, float MaxDistance = -1, float acceleration = 500f, double drag = 5E-9f, float dt = 1 / 60)
        {
            Velocity += acceleration * dt * (Target - Current);
            Velocity = new Vector3((float)(Velocity.x * Math.Pow(drag, dt)), 
                (float)(Velocity.y * Math.Pow(drag, dt)), 
                (float)(Velocity.z * Math.Pow(drag, dt))); //Multiply by a double and THEN cast, to minimize loss
            var dv = (Math.Pow(drag, dt * dt) - 1) / (dt * Math.Log(drag));
            Current += Velocity * (float)dv;
            if (MaxDistance >= 0) Current = Vector3.Distance(Current, Target) < MaxDistance ? Target : (Current - Target).normalized * MaxDistance + Target; //clamp to within MaxDistance of the target
            return Current;
        }

        /// <summary>
        /// Bouncy Interpolation
        /// </summary>
        public static double Blerp(ref double Current, ref double Velocity, double Target, float MaxDistance = -1, double acceleration = 500, double drag = 5E-9f, float dt = 1 / 60)
        {
            Velocity += (Target - Current) * acceleration * dt;
            Velocity *= Math.Pow(drag, dt);
            Current += Velocity * (Math.Pow(drag, dt * dt) - 1) / (dt * Math.Log(drag));
            if (MaxDistance >= 0) if (MaxDistance >= 0) Current = Math.Abs(Current - Target) < MaxDistance ? Target : Math.Sign(Current - Target) * MaxDistance + Target; //clamp to within MaxDistance of the target
            return Current;
        }

        /// <summary>
        /// Bouncy Interpolation
        /// </summary>
        public static float Blerp(ref float Current, ref float Velocity, float Target, float MaxDistance = -1, float acceleration = 500, float drag = 5E-9f, float dt = 1 / 60)
        {
            Velocity += (Target - Current) * acceleration * dt;
            Velocity *= Mathf.Pow(drag, dt);
            Current += Velocity * (Mathf.Pow(drag, dt * dt) - 1) / (dt * Mathf.Log(drag));
            if (MaxDistance >= 0) if (MaxDistance >= 0) Current = Mathf.Abs(Current - Target) < MaxDistance ? Target : Mathf.Sign(Current - Target) * MaxDistance + Target; //clamp to within MaxDistance of the target
            return Current;
        }

        // - - - - - - - - BLERP EXTENSIONS - - - - - - - - //

        /// <summary>
        /// Bouncy interpolation
        /// </summary>
        public static Vector3 Blerp(this Vector3 Current, ref Vector3 Velocity, Vector3 Target, float MaxDistance = -1, float acceleration = 500f, float drag = 20f, float dt = 1 / 60)
        {
            Velocity += acceleration * dt * (Target - Current);
            Velocity = new Vector3((float)(Velocity.x * Math.Pow(drag, dt)),
                (float)(Velocity.y * Math.Pow(drag, dt)),
                (float)(Velocity.z * Math.Pow(drag, dt))); //Multiply by a double and THEN cast, to minimize loss
            var dv = (Math.Pow(drag, dt * dt) - 1) / (dt * Math.Log(drag));
            Current += Velocity * (float)dv;
            if (MaxDistance >= 0) Current = Vector3.Distance(Current, Target) < MaxDistance ? Target : (Current - Target).normalized * MaxDistance + Target; //clamp to within MaxDistance of the target
            return Current;
        }
        #endregion

        //------------------------------------------- REMAP & CLAMP -------------------------------------------------------//
        #region Remap
        ///<summary>
        /// Returns a value with it's domain remapped from (fromMin, fromMax) to (toMin, to Max)
        ///</summary>
        public static float Remap(this float value, float min1, float max1, float min2, float max2)
        {
            return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
        }
        #endregion

        #region Clamp
        public static Vector3 ClampVector(this Vector3 value, Vector3 min, Vector3 max)
        {
            Vector3 result = new()
            {
                x = Mathf.Clamp(value.x, min.x, max.x),
                y = Mathf.Clamp(value.y, min.y, max.y),
                z = Mathf.Clamp(value.z, min.z, max.z)
            };

            return result;
        }
        #endregion

        //------------------------------------------- SPACE CONVERSION(S) -------------------------------------------------------//
        /// <summary>
        /// Convert from a worldspace point to a canvas space point
        /// </summary>
        public static Vector2 WorldToCanvasPoint(Vector3 WorldPoint, RectTransform CanvasRect, Camera cam)
        {
            Vector2 ViewportPos = cam.WorldToViewportPoint(WorldPoint);
            Vector2 WorldObjectScreenPos = new(
                ((ViewportPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x / 2)),
                ((ViewportPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y / 2)));

            return WorldObjectScreenPos;
        }

        //------------------------------------------- UNITYENGINE.PHYSICS EXTENTIONS -------------------------------------------------------//
        #region Casts
        /// <summary>
        /// Physics cast in a cone shape, similar to SphereCastAll()<br></br>
        /// Credit to walterellisfun on github<br></br>
        /// Link: https://github.com/walterellisfun/ConeCast/blob/master/ConeCastExtension.cs
        /// </summary>
        public static RaycastHit[] ConeCastAll(Vector3 origin, float maxRadius, Vector3 direction, float maxDistance, float coneAngle, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin - new Vector3(0, 0, maxRadius), maxRadius, direction, maxDistance, layerMask, queryTriggerInteraction);
            List<RaycastHit> coneCastHitList = new();

            if (sphereCastHits.Length > 0)
            {
                for (int i = 0; i < sphereCastHits.Length; i++)
                {
                    Vector3 hitPoint = sphereCastHits[i].point;
                    Vector3 directionToHit = hitPoint - origin;
                    float angleToHit = Vector3.Angle(direction, directionToHit);

                    if (angleToHit < coneAngle)
                    {
                        coneCastHitList.Add(sphereCastHits[i]);
                    }
                }
            }

            RaycastHit[] coneCastHits = coneCastHitList.ToArray();

            return coneCastHits;
        }

        /// <summary>
        /// Creates a sphere cast that grows in size
        /// </summary>
        public static RaycastHit GrowthSphereCast(Vector3 position, float maxRadius, Vector3 direction, int stepCount = 5, float range = Mathf.Infinity, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            if (!Physics.SphereCast(position, maxRadius, direction, out RaycastHit _hit, range, layerMask, queryTriggerInteraction)) return _hit; //will never grow to hit anything
            
            //the initial raycast is step zero so set step to 1
            int step = 1;

            float radius = maxRadius;

            RaycastHit closestHit = _hit;
            while (step <= stepCount)
            {
                radius += (_hit.collider ? -1 : 1) * maxRadius / Mathf.Pow(2, step); //increase or decrease radius based on whether a hit was found.
                if (Physics.SphereCast(position, radius, direction, out _hit, range, layerMask, queryTriggerInteraction)) closestHit = _hit; //cast and update closest hit
                step++; //increment step#
            }
            return closestHit;
        }

        /// <summary>
        /// Casts a ray from origin in direction with maxDistance length. Returns the exit point of the raycast. <br></br>
        /// <b>ONLY WORKS ON CONCAVE SHAPES</b>
        /// </summary>
        /// <param name="origin">Start of the raycast</param>
        /// <param name="direction">Direction of the raycast</param>
        /// <param name="enterHitInfo">Hit info for the normal raycast</param>
        /// <param name="maxDistance">Max distance to find an object in the initial raycast</param>
        /// <param name="layerMask"></param>
        /// <param name="queryTriggerInteraction"></param>
        /// <returns>The exit point of the raycast</returns>
        public static bool RaycastExit(Vector3 origin, Vector3 direction, out RaycastHit enterHitInfo, out RaycastHit exitHitInfo, float maxDistance, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            if (!Physics.Raycast(origin, direction, out enterHitInfo, maxDistance, layerMask, queryTriggerInteraction))
            {
                exitHitInfo = new RaycastHit(); //So just return the default
                return false; //Initial raycast to get the object. If no hit is found, return default raycasthit.
            }

            var collider = enterHitInfo.collider;
            var ReverseCastDistance = collider.bounds.size.magnitude + Mathf.Epsilon; //get distance that the reverse raycast will be casted from the initial hit
            // ^ by getting the magnitude of the size, it ensures that the raycast can only ever be casted further than the max depth of the object away.
            var rOrigin = enterHitInfo.point + direction.normalized * ReverseCastDistance; //get the origin of the new raycast
            RaycastHit[] hits = Physics.RaycastAll(rOrigin, -direction, ReverseCastDistance, layerMask, queryTriggerInteraction); //cast a ray in the opposite direction

            RaycastHit result = Array.Find(hits, hit => hit.collider == collider); //filter out the desired hit
            exitHitInfo = result;

            return true;
        }

        #endregion

        #region Vectors
        public static void LookAt2D(this Transform transform, Vector2 position, float offset = 0)
        {
            var diff = position - (Vector2)transform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z + offset);
        }

        public static void PivotLookAt2D(this Transform transform, Vector2 pivotPoint, Vector2 target, float offset = 0)
        {
            var diff = target - pivotPoint;
            diff.Normalize();

            float targetAngle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg; //angle from the pivot position to the target position

            var pivotDiff = (pivotPoint - (Vector2)transform.position); //angle of the transform around the pivot
            float pivotAngle = Mathf.Atan2(pivotDiff.normalized.y, pivotDiff.normalized.x) * Mathf.Rad2Deg; //angle from the pivot to the transform position

            float deltaAngle = targetAngle - pivotAngle + offset;
            transform.position = Quaternion.Euler(Vector3.forward * deltaAngle) * pivotDiff + (Vector3)pivotPoint;
            transform.LookAt2D(target, offset);
        }

        public static Quaternion LookRotation2D(Vector2 position, Vector2 target, float offset = 0)
        {
            var diff = target - position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, rot_z + offset);
        }

        public static Vector2 PivotLookPosition2D(Vector2 position, Vector2 pivotPoint, Vector2 target, float offset = 0)
        {
            var diff = target - pivotPoint;
            diff.Normalize();

            float targetAngle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg; //angle from the pivot position to the target position

            var pivotDiff = (pivotPoint - position); //angle of the transform around the pivot
            float pivotAngle = Mathf.Atan2(pivotDiff.normalized.y, pivotDiff.normalized.x) * Mathf.Rad2Deg; //angle from the pivot to the transform position

            float deltaAngle = targetAngle - pivotAngle + offset;
            return Quaternion.Euler(Vector3.forward * deltaAngle) * pivotDiff + (Vector3)pivotPoint;
        }

        #endregion
    }

    public static class GGPhysics
    {
        public static Vector2 ComputeTotalImpulse(Collision2D collision)
        {
            Vector2 impulse = Vector2.zero;

            int contactCount = collision.contactCount;
            for (int i = 0; i < contactCount; i++)
            {
                var contact = collision.GetContact(i);
                impulse += contact.normal * contact.normalImpulse;
                impulse.x += contact.tangentImpulse * contact.normal.y;
                impulse.y -= contact.tangentImpulse * contact.normal.x;
            }

            return impulse;
        }
    }
}
