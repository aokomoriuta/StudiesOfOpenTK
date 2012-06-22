namespace LWisteria.StudiesOfOpenTK.SimpleCloo
{
	/// <summary>
	/// OpenCLソースのコンパイルに失敗した時に発生する例外
	/// </summary>
	class BuildCLException : System.ApplicationException
	{
		/// <summary>
		/// シェーダー
		/// </summary>
		public readonly string SourceString;

		/// <summary>
		/// コンパイルログ情報
		/// </summary>
		public readonly string Log;

		/// <summary>
		/// ソースとログを指定して作成する
		/// </summary>
		/// <param name="source">シェーダー</param>
		/// <param name="log">ログ</param>
		public BuildCLException(string source, string log)
			: base()
		{
			// ソースとログを設定
			this.SourceString = source;
			this.Log = log;
		}
	}
}