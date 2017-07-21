using UnityEngine;
using System;


[Serializable]
public class SimpleGPUInstancingComponent
{
	public Renderer ObjectRenderer;
	public Color ObjectColor;
}

public class SimpleGPUInstancingExample : MonoBehaviour
{
	public SimpleGPUInstancingComponent[] Objects;

	void Start()
	{
		MaterialPropertyBlock matpropertyBlock = new MaterialPropertyBlock();
		if ( Objects != null && Objects.Length > 0 )
		{
			for ( int i = 0; i < Objects.Length; i++ )
			{
				matpropertyBlock.SetColor( "_Color", Objects[ i ].ObjectColor );
				Objects[ i ].ObjectRenderer.SetPropertyBlock( matpropertyBlock );
			}
		}
	}
}
