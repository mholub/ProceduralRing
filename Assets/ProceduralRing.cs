using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class ProceduralRing : MonoBehaviour {
	public enum SubdivionType
	{
		Constant,
		AngleRatio
	}

	public float InnerRadius = 0.4f;
	public float OuterRadius = 0.5f;

	public SubdivionType SubdivionMode;
	[Range(0, 360)]
	public float Angle = 360;
	public int Subdivisions = 36;

	public bool UpdateMeshCollider;

	private Mesh mesh;
	private MeshCollider meshCollider;

	private Vector3[] vertices;
	private int[] indices;

	public void UpdateMesh() {
		updateMesh();
	}

	void OnValidate () {
		updateMesh ();
	}

	void updateMesh() {
		if (mesh == null) {
			mesh = new Mesh ();
			mesh.MarkDynamic ();
			mesh.name = "ProceduralRing";
			updateMesh ();

			GetComponent<MeshFilter> ().mesh = mesh;
		}

		int subdivisions = (SubdivionMode == SubdivionType.AngleRatio) ? Mathf.CeilToInt (Angle / 360.0f * Subdivisions) : Subdivisions;

		int vertexCount = (Mathf.CeilToInt(subdivisions + 1)) * 2;
		if (vertices == null || vertices.Length != vertexCount) {
			vertices = new Vector3[vertexCount];
		}

		float dAlpha = 2 * Mathf.PI / 360 * Angle / subdivisions;
		for (int i = 0; i < subdivisions + 1; ++i) {
			float angle = dAlpha * i;
			float nx = Mathf.Cos (angle);
			float ny = Mathf.Sin (angle);

			vertices [2 * i] = InnerRadius * new Vector3 (nx, ny, 0);
			vertices [2 * i + 1] = OuterRadius * new Vector3 (nx, ny, 0);
		}

		mesh.Clear ();
		mesh.vertices = vertices;

		int indexCount = 3 * subdivisions * 2; 
		if (indices == null || indices.Length != indexCount) {
			indices = new int[3 * subdivisions * 2];
		}

		for (int i = 0; i < subdivisions; ++i) {
			indices [6 * i] = 2 * i + 1;
			indices [6 * i + 1] = 2 * i;
			indices [6 * i + 2] = 2 * i + 2;
			indices [6 * i + 3] = 2 * i + 3;
			indices [6 * i + 4] = 2 * i + 1;
			indices [6 * i + 5] = 2 * i + 2;
		}

		mesh.SetIndices (indices, MeshTopology.Triangles, 0);

		if (UpdateMeshCollider) {
			if (meshCollider == null) {
				meshCollider = GetComponent<MeshCollider> ();
			}
			meshCollider.sharedMesh = mesh;
		}
	}
}
