using UnityEngine;

public class SphericalCursorModule : MonoBehaviour {
	// This is a sensitivity parameter that should adjust how sensitive the mouse control is.
	public float Sensitivity;

	// This is a scale factor that determines how much to scale down the cursor based on its collision distance.
	public float DistanceScaleFactor;
	
	// This is the layer mask to use when performing the ray cast for the objects.
	// The furniture in the room is in layer 8, everything else is not.
	private const int ColliderMask = (1 << 8);

	// This is the Cursor game object. Your job is to update its transform on each frame.
	private GameObject Cursor;

	// This is the Cursor mesh. (The sphere.)
	private MeshRenderer CursorMeshRenderer;

	// This is the scale to set the cursor to if no ray hit is found.
	private Vector3 DefaultCursorScale = new Vector3(10.0f, 10.0f, 10.0f);

	// Maximum distance to ray cast.
	private const float MaxDistance = 100.0f;

	// Sphere radius to project cursor onto if no raycast hit.
	private const float SphereRadius = 1000.0f;



    void Awake() 
	{
		Cursor = transform.Find("Cursor").gameObject;
		CursorMeshRenderer = Cursor.transform.GetComponentInChildren<MeshRenderer>();
        CursorMeshRenderer.GetComponent<Renderer>().material.color = new Color(0.0f, 0.8f, 1.0f);
	}	


	// for having Cursor pop when he goes to a new object:
	public AnimationCurve popAnimation;
	private float popT;
	private Collider lastCollider;

	// for blinking highlighted object:
	private float selectionT;
	public Material highlightMaterial;
	public Color colorA, colorB;


	void Update()
	{
		// Cursor Position
		Vector2 halfScreen = new Vector2 (Screen.width, Screen.height) * 0.5f;
		Vector2 mouseLocal = (Vector2)Input.mousePosition - halfScreen;
		float halfScreenYDenom = 1.0f / halfScreen.y;
		mouseLocal = new Vector2 (mouseLocal.x * halfScreenYDenom, mouseLocal.y * halfScreenYDenom) * Sensitivity * 100.0f;
		float screenRadians = this.GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad;

		float cosX = Mathf.Cos (screenRadians * mouseLocal.x);
		float sinX = Mathf.Sin (screenRadians * mouseLocal.x);
		float cosY = Mathf.Cos (screenRadians * mouseLocal.y);
		float sinY = Mathf.Sin (screenRadians * mouseLocal.y);
		Vector3 cursorDirection = new Vector3 (sinX, sinY, cosX * cosY);


		
		// Update Cursor Position and Scale based on Raycast
		var cursorHit = new RaycastHit();
		if (Physics.Raycast(this.transform.position, cursorDirection, out cursorHit, Mathf.Infinity, ColliderMask))
		{
			if (cursorHit.collider != lastCollider)
			{
				popT = 0.0f;
				lastCollider = cursorHit.collider;
			}

			Cursor.transform.position = cursorHit.point;
			popT += Time.deltaTime;
			float scale = (cursorHit.distance * DistanceScaleFactor + 0.2f) * 0.5f + popAnimation.Evaluate(popT*2.0f)*0.4f;
			Cursor.transform.localScale = new Vector3(scale, scale, scale);
		}
		else 
		{
			Cursor.transform.localPosition = cursorDirection * SphereRadius;
			Cursor.transform.localScale = DefaultCursorScale;
			lastCollider = null;
		}




		// Update highlighted object based upon the raycast.
		if (cursorHit.collider != null)
		{
			Selectable.CurrentSelection = cursorHit.collider.gameObject;

			selectionT += Time.deltaTime;
			float blend = (Mathf.Sin(selectionT*5.0f) + 1.0f) * 0.5f;
			highlightMaterial.color = colorA * blend + colorB * (1.0f-blend);
		}
		else
		{
			Selectable.CurrentSelection = null;
		}
	}
}
