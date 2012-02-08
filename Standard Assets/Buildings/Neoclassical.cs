using UnityEngine;

using Exception = System.Exception;

public sealed class Neoclassical : Building
{
	// fields
	private const float _component_width_min = 1.5f;
	private const float _component_width_max = 1.75f;
	
	private const float _component_space_min = 2f;
	private const float _component_space_max = 2.25f;
	
	
	// constructors
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Neoclassical"/> class.
	/// The boundaries of the base of this building must be given in 
	/// clockwise order.
	/// </summary>
	/// <param name='p1'>
	/// A point in space.
	/// </param>
	/// <param name='p2'>
	/// A point in space.
	/// </param>
	/// <param name='p3'>
	/// A point in space.
	/// </param>
	/// <param name='p4'>
	/// A point in space.
	/// </param>
	public Neoclassical (Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
	: base(p1, p2, p3, p4, BuildingType.Neoclassical)
	{
		this.FloorHeight = Random.Range(4f, 5f);
		this.FloorNumber = Util.RollDice(new float[] {0.15f, 0.7f, 0.15f});
	}
	
	
	// methods
	
	/// <summary>
	/// Constructs the faces.
	/// </summary>
	public override void ConstructFaces ()
	{
		base.ConstructFaces();
	}
	
	public override void ConstructFaceComponents ()
	{
		if (Faces.Count == 0) throw new Exception("There are no faces to construct the components.");
		
		float component_width = Random.Range(_component_width_min, _component_width_max);
		float inbetween_space = Random.Range(_component_space_min, _component_space_max);
		
		foreach (Face face in Faces)
		{
			int components_no = GetComponentsNumber(face.Width, component_width, inbetween_space);
			float effective_width = components_no * component_width;
			float fixed_space = (face.Width - effective_width) / (components_no + 1);

			for (int floor = 0; floor < this.FloorNumber; ++floor)
			{
				float offset = fixed_space;
				for (int i = 0; i < components_no; ++i)
				{
					Vector3 dr = face.Boundaries[0] - face.Right * offset + (new Vector3(0f, floor * this.FloorHeight, 0f));
          Vector3 dl = dr - face.Right * component_width;
					offset += component_width;
					face.AddFaceComponent(new FaceComponent(face, component_width, FloorHeight * 3 / 5, dr, dl));
					offset += fixed_space;
				}
			}
		}
	}
	
	/// <summary>
	/// Calculates the number of components that fit in a face.
	/// </summary>
	/// <returns>
	/// The number of components.
	/// </returns>
	/// <param name='face_width'>
	/// The width of the face.
	/// </param>
	/// <param name='component_width'>
	/// The width of the face components.
	/// </param>
	/// <param name='space'>
	/// The wanted space between each face component, approximately.
	/// </param>
	private int GetComponentsNumber (float face_width, float component_width, float space)
	{
		return Mathf.CeilToInt(face_width / (component_width + space));
	}
}
