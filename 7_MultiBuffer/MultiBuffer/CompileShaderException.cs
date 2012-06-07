namespace LWisteria.StudiesOfOpenTK.MultiBuffer
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
		/// コンパイルログ情報
		/// </summary>
		public readonly string InfoLog;

		/// <summary>
		/// シェーダーとログを指定して作成する
		/// </summary>
		/// <param name="shader">シェーダー</param>
		/// <param name="infoLog">ログ</param>
		public CompileShaderException(int shader, string infoLog)
			:base()
		{
			// シェーダーとログをせってい
			this.Shader = shader;
			this.InfoLog = infoLog;
		}

		/// <summary>
		/// 文字列に変換する
		/// </summary>
		/// <returns>シェーダーとログ</returns>
		public override string ToString()
		{
			// シェーダーとログを文字列にして返す
			return string.Format("シェーダー：{0}\nログ：{1}", this.Shader, this.InfoLog);
		}
	}
}