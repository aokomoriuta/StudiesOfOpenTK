using OpenTK;
using OpenTK.Graphics;
namespace LWisteria.StudiesOfOpenTK.DynamicData
{
	/// <summary>
	/// 立方体
	/// </summary>
	public struct Cube
	{
		/// <summary>
		/// 立方体の中心座標
		/// </summary>
		Vector3 position;

		/// <summary>
		/// 立方体の1辺の長さ
		/// </summary>
		readonly float size;

		/// <summary>
		/// 立方体の表示色
		/// </summary>
		readonly Color4 color;

		/// <summary>
		/// 立方体を作成する
		/// </summary>
		/// <param name="_size">1辺の長さ</param>
		/// <param name="_color">表示色</param>
		public Cube(float _size, Color4 _color)
		{
			// 各パラメーターを設定
			this.position = new Vector3();
			this.size = _size;
			this.color = _color;
		}

		public double PositionX
		{
			get
			{
				return this.position.X;
			}
			set
			{
				this.position.X = (float)value;
			}
		}

		public double PositionY
		{
			get
			{
				return this.position.Y;
			}
			set
			{
				this.position.Y = (float)value;
			}
		}

		public double PositionZ
		{
			get
			{
				return this.position.Z;
			}
			set
			{
				this.position.Z = (float)value;
			}
		}
		
	}
}