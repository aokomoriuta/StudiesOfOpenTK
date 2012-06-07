using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LWisteria.StudiesOfOpenTK.ObjectiveOpenTK
{
	/// <summary>
	/// 頂点属性値
	/// </summary>
	public struct VertexAttribution
	{
		/// <summary>
		/// 属性の変数名
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// 属性の数
		/// </summary>
		public readonly int Size;

		/// <summary>
		/// 属性の種類
		/// </summary>
		public readonly VertexAttribPointerType Type;

		/// <summary>
		/// 正規化されているかどうか
		/// </summary>
		public readonly bool Normalized;

		/// <summary>
		/// 属性の位置
		/// </summary>
		public readonly int Offset;

		/// <summary>
		/// 頂点属性値を作成する
		/// </summary>
		/// <param name="name">変数名</param>
		/// <param name="type">属性の種類</param>
		/// <param name="size">属性の数</param>
		/// <param name="offset">属性の位置</param>
		/// <param name="normalized">正規化されているかどうか</param>
		public VertexAttribution(string name, VertexAttribPointerType type, int size, int offset, bool normalized = false)
		{
			// 各パラメーターを設定
			this.Name = name;
			this.Type = type;
			this.Size = size;
			this.Offset = offset;
			this.Normalized = normalized;
		}
	}
}