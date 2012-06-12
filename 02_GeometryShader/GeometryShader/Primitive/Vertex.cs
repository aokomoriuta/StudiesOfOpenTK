using OpenTK;
namespace LWisteria.StudiesOfOpenTK.CubeWithShader
{
	/// <summary>
	/// 頂点要素
	/// </summary>
	struct Vertex
	{
		/// <summary>
		/// 頂点要素のサイズ
		/// </summary>
		public static readonly int Stride = System.Runtime.InteropServices.Marshal.SizeOf(default(Vertex));

		/// <summary>
		/// 頂点の位置
		/// </summary>
		public readonly Vector3 Position;

		/// <summary>
		/// 頂点の法線方向
		/// </summary>
		public readonly Vector3 Normal;

		/// <summary>
		/// 頂点要素を作成する
		/// </summary>
		/// <param name="position">頂点位置</param>
		/// <param name="normal">頂点の法線方向ベクトル</param>
		public Vertex(Vector3 position, Vector3 normal)
			:this()
		{
			// 位置と法線方向ベクトルを設定
			this.Position = position;
			this.Normal = normal;

			// 法線方向ベクトルを正規化
			this.Normal.Normalize();
		}
	}
}