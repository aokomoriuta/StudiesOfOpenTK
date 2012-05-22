using OpenTK;
namespace LWisteria.StudiesOfOpenTK.SizeVariableCube
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
		/// 立方体を作成する
		/// </summary>
		/// <param name="position">中心座標</param>
		/// <param name="size">1辺の長さ</param>
		public Cube(Vector3 _position, float _size)
		{
			// 位置と大きさを設定
			this.position = _position;
			this.size = _size;
		}
		
	}
}