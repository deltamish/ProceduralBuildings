using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Face
{
	// fields
	
	private readonly Building _parent;
	private Vector3 _normal;
	private Vector3 _right;
	private float _width;
	private bool _is_free;
	private List<Vector3> _boundaries = new List<Vector3>();
	private List<FaceComponent> _face_components = new List<FaceComponent>();

	
	// properties

	/// <summary>
	/// Gets the parent mesh.
	/// </summary>
	/// <value>
	/// The parent mesh.
	/// </value>
	public Building Parent
	{
		get { return _parent; }
	}
	
	/// <summary>
	/// Gets the normal of this face.
	/// </summary>
	/// <value>
	/// The normal.
	/// </value>
	public Vector3 Normal
	{
		get { return _normal; }
	}
	
	public Vector3 Right
	{
		get { return _right; }
	}
	
	/// <summary>
	/// Gets the width of this face.
	/// </summary>
	/// <value>
	/// The width.
	/// </value>
	public float Width
	{
		get { return _width; }
	}

	
	public ReadOnlyCollection<Vector3> Boundaries
	{
		get { return _boundaries.AsReadOnly(); }
	}
	
	
	// constructors
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Face"/> class,
	/// from the given points in clockwise order.
	/// </summary>
	/// <param name='parent'>
	/// The parent mesh of this face.
	/// </param>
	/// <param name='dr'>
	/// Down-right point of the face.
	/// </param>
	/// <param name='dl'>
	/// Down-left point of the face.
	/// </param>
	public Face (Building parent, Vector3 dr, Vector3 dl)
	{
		_parent = parent;
		
		_boundaries.Add(dr);
		_boundaries.Add(dl);
		_boundaries.Add(new Vector3(dl.x, dl.y + _parent.Height, dl.z));
		_boundaries.Add(new Vector3(dr.x, dr.y + _parent.Height, dr.z));
		
		this.CalculateNormal();
		this.CalculateWidth();
	}
	
	
	// methods
	
	/// <summary>
	/// Calculates the normal.
	/// </summary>
	private void CalculateNormal ()
	{
		_right = new Vector3(_boundaries[0].x - _boundaries[1].x,
												 _boundaries[0].y - _boundaries[1].y,
												 _boundaries[0].z - _boundaries[1].z);
		
//		Vector3 edge2 = new Vector3(_boundaries[0].x - _boundaries[3].x,
//									_boundaries[0].y - _boundaries[3].y,
//									_boundaries[0].z - _boundaries[3].z);
		
		_normal = Vector3.Cross(Vector3.up, _right);
		_normal.Normalize();
		_right.Normalize();
	}
	
	/// <summary>
	/// Calculates the width.
	/// </summary>
	private void CalculateWidth ()
	{
		Vector3 edge = new Vector3(_boundaries[0].x - _boundaries[1].x,
														   0f,
														   _boundaries[0].z - _boundaries[1].z);
		_width = edge.magnitude;
	}


	public void ConstructFaceComponents (float component_width, float inbetween_space)
	{
		int components_no = Mathf.CeilToInt(_width / (component_width + inbetween_space));
		float fixed_space = (_width - components_no * component_width) / (components_no + 1);

		for (int floor = 0; floor < _parent.FloorNumber; ++floor)
		{
			float offset = fixed_space;
			for (int i = 0; i < components_no; ++i)
			{
				Vector3 dr = _boundaries[0] - _right * offset + (new Vector3(0f, floor * _parent.FloorHeight, 0f));
				Vector3 dl = dr - _right * component_width;
				offset += component_width;
				_face_components.Add(new FaceComponent(this, component_width, dr, dl, 3f / 5f));
				offset += fixed_space;
			}
		}
	}
	
	/// <summary>
	/// Draw the face.
	/// </summary>
	public void Draw ()
	{
//		GL.PushMatrix();
//		GL.Begin(GL.QUADS);
//		foreach (Vector3 v in _boundaries)
//			GL.Vertex(v);
//		GL.End();
//		GL.PopMatrix();
		foreach (FaceComponent fc in _face_components)
			fc.Draw();
	}
}
