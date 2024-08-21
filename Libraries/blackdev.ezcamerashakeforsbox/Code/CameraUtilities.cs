using Sandbox;
using System;

namespace EZCameraShake
{
    public class CameraUtilities
    {
		/// <summary>
		/// Smoothes a Vector3 that represents euler angles.
		/// </summary>
		/// <param name="current">The current Vector3 value.</param>
		/// <param name="target">The target Vector3 value.</param>
		/// <param name="velocity">A refernce Vector3 used internally.</param>
		/// <param name="smoothTime">The time to smooth, in seconds.</param>
		/// <returns>The smoothed Vector3 value.</returns>
		public static Vector3 SmoothDampEuler( Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime, float deltaTime )
		{
			float velocityX = velocity.x;
			float velocityY = velocity.y;
			float velocityZ = velocity.z;

			float x = MathExtensions.SmoothDampAngle( current.x, target.x, ref velocityX, smoothTime, deltaTime );
			float y = MathExtensions.SmoothDampAngle( current.y, target.y, ref velocityY, smoothTime, deltaTime );
			float z = MathExtensions.SmoothDampAngle( current.z, target.z, ref velocityZ, smoothTime, deltaTime );

			velocity = new Vector3( velocityX, velocityY, velocityZ );

			return new Vector3( x, y, z );
		}


		/// <summary>
		/// Multiplies each element in Vector3 v by the corresponding element of w.
		/// </summary>
		public static Vector3 MultiplyVectors(Vector3 v, Vector3 w)
        {
            v.x *= w.x;
            v.y *= w.y;
            v.z *= w.z;

            return v;
        }
    }
}

public static class MathExtensions
{
	public static float SmoothDampAngle( float current, float target, ref float currentVelocity, float smoothTime, float deltaTime, float maxSpeed = float.PositiveInfinity )
	{
		// Ensure smoothTime is never too small
		smoothTime = Math.Max( 0.0001f, smoothTime );

		// Calculate the difference between the current and target angles
		float deltaAngle = MathfDeltaAngle( current, target );

		// Calculate the desired target position by adding deltaAngle to the current angle
		target = current + deltaAngle;

		// SmoothDamp logic for interpolating between angles
		float result = SmoothDamp( current, target, ref currentVelocity, smoothTime, deltaTime, maxSpeed );

		return result;
	}

	// SmoothDamp function: interpolates between two values using velocity and smooth time
	public static float SmoothDamp( float current, float target, ref float currentVelocity, float smoothTime, float deltaTime, float maxSpeed = float.PositiveInfinity )
	{
		// Similar to Mathf.SmoothDamp logic in Unity
		float num = 2f / smoothTime;
		float num2 = num * deltaTime;
		float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
		float num4 = current - target;
		float num5 = target;
		float maxDelta = maxSpeed * smoothTime;
		num4 = Clamp( num4, -maxDelta, maxDelta );
		target = current - num4;
		float num7 = (currentVelocity + num * num4) * deltaTime;
		currentVelocity = (currentVelocity - num * num7) * num3;
		float num8 = target + (num4 + num7) * num3;

		// Prevent overshooting
		if ( num5 - current > 0f == num8 > num5 )
		{
			num8 = num5;
			currentVelocity = (num8 - num5) / deltaTime;
		}
		return num8;
	}

	// Function to calculate the shortest difference between two angles, handling wrap-around
	public static float MathfDeltaAngle( float current, float target )
	{
		float delta = target - current;
		while ( delta < -180f ) delta += 360f;
		while ( delta > 180f ) delta -= 360f;
		return delta;
	}

	// Clamp function to restrict values between min and max
	public static float Clamp( float value, float min, float max )
	{
		if ( value < min ) return min;
		if ( value > max ) return max;
		return value;
	}
}
