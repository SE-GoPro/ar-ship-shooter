using UnityEngine;

public class TiltWindow : MonoBehaviour
{
	public Vector2 range = new Vector2(5f, 3f);

	Transform mTrans;
	Quaternion mStart;
	Vector2 mRot = Vector2.zero;
	float halfWidth = Screen.width * 0.5f;
	float halfHeight = Screen.height * 0.5f;

	void Start ()
	{
		// GYRO
		Logger.Log("TiltWindow: GYRO support - " + SystemInfo.supportsGyroscope);
		if (SystemInfo.supportsGyroscope)
        {
			if (!Input.gyro.enabled)
            {
				Input.gyro.enabled = true;
			}
        }
		// Input
		mTrans = transform;
		mStart = mTrans.localRotation;
	}

	void Update ()
	{
		// Tilt by GYRO
		if (SystemInfo.supportsGyroscope)
        {
			mTrans.localRotation = mStart * Quaternion.Euler(
				-Input.gyro.attitude.y * range.x * 3,
				Input.gyro.attitude.x * range.y * 8,
				0f
			);
		}
		else
		// Tilt by mouse
        {
			Vector3 pos = Input.mousePosition;

			float x = Mathf.Clamp((pos.x - halfWidth) / halfWidth, -1f, 1f);
			float y = Mathf.Clamp((pos.y - halfHeight) / halfHeight, -1f, 1f);
			mRot = Vector2.Lerp(mRot, new Vector2(x, y), Time.deltaTime * 5f);

			mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * range.y, mRot.x * range.x, 0f);
		}
	}
}
