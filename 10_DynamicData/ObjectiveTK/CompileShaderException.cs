namespace LWisteria.StudiesOfOpenTK.ObjectiveTK
{
	/// <summary>
	/// シェーダーのコンパイルに失敗した時に発生する例外
	/// </summary>
	class CompileShaderException : System.ApplicationException
	{
		/// <summary>
		/// シェーダー
		/// </summary>
		public readonly int Shader;

		/// <summary>
		/// シェーダーの種類
		/// </summary>
		public readonly OpenTK.Graphics.OpenGL.ShaderType Type;

		/// <summary>
		/// コンパイルログ情報
		/// </summary>
		public readonly string InfoLog;

		/// <summary>
		/// シェーダーとログを指定して作成する
		/// </summary>
		/// <param name="shader">シェーダー</param>
		/// <param name="type">シェーダーの種類</param>
		/// <param name="infoLog">ログ</param>
		public CompileShaderException(int shader, OpenTK.Graphics.OpenGL.ShaderType type, string infoLog)
			: base()
		{
			// シェーダーとログを設定
			this.Shader = shader;
			this.Type = type;
			this.InfoLog = infoLog;
		}

		/// <summary>
		/// 文字列に変換する
		/// </summary>
		/// <returns>シェーダーとログ</returns>
		public override string ToString()
		{
			// シェーダーとログを文字列にして返す
			return string.Format("シェーダー：{0}({1})\nログ：{1}", this.Shader, this.Type, this.InfoLog);
		}
	}
}