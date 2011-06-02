﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoMac.OpenGL;

namespace Microsoft.Xna.Framework.Graphics
{
	public class EffectParameter
	{
		EffectParameterType paramType;
		EffectParameterClass paramClass;
		int rowCount;
		string name;
		int colCount;
		EffectParameterCollection elements;
		string semantic;
		EffectParameterCollection structMembers;
		object _cachedValue = null;
		internal int internalIndex;  // used by opengl processes.
		int internalLength;
		Effect _parentEffect;
		
		internal EffectParameter(Effect parent, string paramName, int paramIndex, string paramSType, int paramLength)
		{
			_parentEffect = parent;
			name = paramName;
			internalIndex = paramIndex;
			internalLength = paramLength;
			
			switch (paramSType ){
			case "FloatVec2":
				paramType = EffectParameterType.Single;
				paramClass = EffectParameterClass.Vector;
				rowCount = 1;
				colCount = 2;
				_cachedValue = MonoMac.OpenGL.Vector2.Zero;
				break;
			case "Sampler2D":
				paramType = EffectParameterType.Texture2D;
				paramClass = EffectParameterClass.Object;
				rowCount = 0;
				colCount = 0;
				break;				
				
			}
				
			
			
		}
		public int ColumnCount {
			get { return colCount; }
		}

		public EffectParameterCollection Elements {
			get { return elements; }
		}

		public string Name {
			get { return name; }
		}

		public EffectParameterClass ParameterClass { 
			get { return paramClass;} 
		}

		public EffectParameterType ParameterType { 
			get { return paramType;} 
		}

		public int RowCount {
			get { return rowCount; }
		}

		public string Semantic {
			get { return semantic; }
		}

		EffectParameterCollection StructureMembers {
			get { return structMembers; }
		}

		public void SetValue (object value)
		{
		}

		public bool GetValueBoolean ()
		{
			return true;
		}

		public bool[] GetValueBooleanArray ()
		{
			return new bool[0];
		}

		public int GetValueInt32 ()
		{
			return 0;
		}

		public int[] GetValueInt32Array ()
		{
			return new int[0];
		}

		public Matrix GetValueMatrix ()
		{
			return new Matrix ();
		}

		public Matrix[] GetValueMatrixArray ()
		{
			return new Matrix[0];
		}

		public Quaternion GetValueQuaternion ()
		{
			return new Quaternion ();
		}

		public Quaternion[] GetValueQuaternionArray ()
		{
			return new Quaternion[0];
		}

		public Single GetValueSingle ()
		{
			return new Single ();
		}

		public Single[] GetValueSingleArray ()
		{
			return new Single[0];
		}

		public string GetValueString ()
		{
			return String.Empty;
		}

		public Texture2D GetValueTexture2D ()
		{
			return null; //new Texture2D ();
		}

		//		public Texture3D GetValueTexture3D ()
		//		{
		//			return new Texture3D ();
		//		}

		//		public TextureCube GetValueTextureCube ()
		//		{
		//			return null; //new TextureCube ();
		//		}

		public Vector2 GetValueVector2 ()
		{
			return Vector2.One;
		}

		public Vector2[] GetValueVector2Array ()
		{
			return new Vector2[0];
		}

		public Vector3 GetValueVector3 ()
		{
			return Vector3.One;
		}

		public Vector3[] GetValueVector3Array ()
		{
			return new Vector3[0];
		}

		public Vector4 GetValueVector4 ()
		{
			return Vector4.One;
		}

		public Vector4[] GetValueVector4Array ()
		{
			return new Vector4[0];
		}		

		public void SetValue (bool value)
		{
		}

		public void SetValue (bool[] value)
		{
		}

		public void SetValue (int value)
		{
		}

		public void SetValue (int[] value)
		{
		}

		public void SetValue (Matrix value)
		{
		}

		public void SetValue (Matrix[] value)
		{
		}

		public void SetValue (Quaternion value)
		{
		}

		public void SetValue (Quaternion[] value)
		{
		}

		public void SetValue (Single value)
		{
		}

		public void SetValue (Single[] value)
		{
		}

		public void SetValue (string value)
		{
		}

		public void SetValue (Texture value)
		{
			GL.UseProgram(_parentEffect.CurrentTechnique.Passes[0].shaderProgram);
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D,value._textureId);
			GL.Enable(EnableCap.Texture2D);
			GL.Uniform1(internalIndex, value._textureId);
			_cachedValue = value._textureId;
			GL.UseProgram(0);
		}

		public void SetValue (Vector2 value)
		{
			GL.UseProgram(_parentEffect.CurrentTechnique.Passes[0].shaderProgram);			
			MonoMac.OpenGL.Vector2 vect2 = new MonoMac.OpenGL.Vector2(value.X, value.Y);
			_cachedValue = vect2;
			GL.Uniform2(internalIndex,vect2);
			GL.UseProgram(0);
		}

		public void SetValue (Vector2[] value)
		{
		}

		public void SetValue (Vector3 value)
		{
			GL.UseProgram(_parentEffect.CurrentTechnique.Passes[0].shaderProgram);			
			MonoMac.OpenGL.Vector3 vect3 = new MonoMac.OpenGL.Vector3(value.X, value.Y, value.Z);
			_cachedValue = vect3;
			GL.Uniform3(internalIndex,vect3);
			GL.UseProgram(0);			
		}

		public void SetValue (Vector3[] value)
		{
		}

		public void SetValue (Vector4 value)
		{
			GL.UseProgram(_parentEffect.CurrentTechnique.Passes[0].shaderProgram);			
			MonoMac.OpenGL.Vector4 vect4 = new MonoMac.OpenGL.Vector4(value.X, value.Y, value.Z, value.W);
			_cachedValue = vect4;
			GL.Uniform4(internalIndex,vect4);
			GL.UseProgram(0);	
		}

		public void SetValue (Vector4[] value)
		{
		}

		internal void ApplyEffectValue() 
		{
			
			switch (ParameterClass) {
			case EffectParameterClass.Vector:
				ApplyVectorValue();
				break;
			}
		}
		
		private void ApplyVectorValue() 
		{
			switch (rowCount) {
			case 2:
				GL.Uniform2(internalIndex,(MonoMac.OpenGL.Vector2)_cachedValue);
				break;
			}
		}
	}
}
