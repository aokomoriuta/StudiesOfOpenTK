using OpenTK;
using OpenTK.Graphics;
namespace LWisteria.StudiesOfOpenTK.MultiBuffer
{
	/// <summary>
	/// 立方体
	/// </summary>
	struct Cube
	{
		/// <summary>
		/// Cubeのデータサイズ
		/// </summary>
		public static readonly int SizeInByte = System.Runtime.InteropServices.Marshal.SizeOf(default(Cube));

		/// <summary>
		/// 立方体の中心座標
		/// </summary>
		readonly Vector3 position;

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
		/// <param name="_position">中心座標</param>
		/// <param name="_size">1辺の長さ</param>
		/// <param name="_color">表示色</param>
		public Cube(Vector3 _position, float _size, Color4 _color)
		{
			// 各パラメーターを設定
			this.position = _position;
			this.size = _size;
			this.color = _color;
		}
		
	}
}