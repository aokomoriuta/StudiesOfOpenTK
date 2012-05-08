using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace LWisteria.StudiesOfOpenTK.GeometryShader
{
	/// <summary>
	/// 最上位ウインドウ
	/// </summary>
	partial class MainWindow : System.Windows.Window
	{
		/// <summary>
		/// 最上位ウインドウを作成
		/// </summary>
		public MainWindow()
		{
			// コンポーネント初期化
			InitializeComponent();


			// プログラムを初期化
			int program = 0;

			// バッファーオブジェクトを初期化
			int vbo = 0;
			int vao = 0;
			int ebo = 0;

			// 頂点数はゼロに初期化
			int indexCount = 0;

			// ウインドウが読みこまれたら
			this.Loaded += (sender, e) =>
			{
				// シェーダーとプログラムの作成
				{
					// コントロールを有効化
					glControl.MakeCurrent();

					// シェーダーの状態
					int vertexShaderStatus;
					int geometryShaderStatus;
					int flagmentShaderStatus;

					// 頂点・ジオメトリ・フラグメントリシェーダーを作成
					int vertexShader = GL.CreateShader(ShaderType.VertexShader);
					int geometryShader = GL.CreateShader(ShaderType.GeometryShaderExt);
					int flagmentShader = GL.CreateShader(ShaderType.FragmentShader);

					// ソースから読み込み
					GL.ShaderSource(vertexShader, Properties.Resources.vertex);
					GL.ShaderSource(geometryShader, Properties.Resources.geometry);
					GL.ShaderSource(flagmentShader, Properties.Resources.fragment);

					// コンパイル
					GL.CompileShader(vertexShader);
					GL.CompileShader(geometryShader);
					GL.CompileShader(flagmentShader);

					// シェーダーを読み込み
					GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out vertexShaderStatus);
					GL.GetShader(geometryShader, ShaderParameter.CompileStatus, out geometryShaderStatus);
					GL.GetShader(flagmentShader, ShaderParameter.CompileStatus, out flagmentShaderStatus);

					// ログを取得
					string vertexLog = GL.GetShaderInfoLog(vertexShader);
					string geometryLog = GL.GetShaderInfoLog(geometryShader);
					string flagmentLog = GL.GetShaderInfoLog(flagmentShader);

					// コンパイルに失敗していたら
					if(vertexLog + geometryLog + flagmentLog != "")
					{
						// 例外
						throw new ApplicationException(String.Format(
							"Vertex:{0}" + System.Environment.NewLine +
							"Geometry:{1}" + System.Environment.NewLine +
							"Flagment:{2}" + System.Environment.NewLine,
							vertexLog, geometryLog, flagmentLog));
					}

					// プログラムを作成して、シェーダーを設定
					program = GL.CreateProgram();
					GL.AttachShader(program, vertexShader);
					GL.AttachShader(program, geometryShader);
					GL.AttachShader(program, flagmentShader);
					
					// 入出力値のタイプを設定
					GL.Ext.ProgramParameter(program, ExtGeometryShader4.GeometryInputTypeExt, (int)All.Triangles);
					GL.Ext.ProgramParameter(program, ExtGeometryShader4.GeometryOutputTypeExt, (int)All.Triangles);

					// 最大頂点数を指定
					int maxVerticesCount;
					GL.GetInteger((GetPName)ExtGeometryShader4.MaxGeometryOutputVerticesExt, out maxVerticesCount);
					GL.Ext.ProgramParameter(program, ExtGeometryShader4.GeometryVerticesOutExt, maxVerticesCount);

					// プログラムをリンク
					GL.LinkProgram(program);

					// 不要になったシェーダーを削除
					GL.DeleteShader(vertexShader);
					GL.DeleteShader(geometryShader);
					GL.DeleteShader(flagmentShader);

					// 使用するプログラムの指定
					GL.UseProgram(program);
				}

				// 頂点・頂点インデックスの作成
				{
					// 頂点データの作成
					var vertices = new Vector3[]
					{
						new Vector3(0, 0, 0),
						new Vector3(1, 0, 0),
						new Vector3(0, 1, 0),
					};

					// 頂点インデックスデータの作成
					var indices = new uint[]
					{
						0,1,2,
					};

					// 頂点インデックス数設定
					indexCount = indices.Length;

					// 頂点バッファー(VBO)を作成して割り当て
					GL.GenBuffers(1, out vbo);
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

					// 頂点インデックスバッファー(EBO)を作成して割り当て
					GL.GenBuffers(1, out ebo);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

					// 頂点と頂点インデックスを書き込み
					GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
						new IntPtr(Vector3.SizeInBytes * vertices.Length),
						vertices,
						BufferUsageHint.StaticDraw);
					GL.BufferData<uint>(
						BufferTarget.ElementArrayBuffer,
						new IntPtr(sizeof(uint) * indices.Length),
						indices,
						BufferUsageHint.StaticDraw);
				}

				// 頂点データとシェーダーの関連付け
				{
					// 頂点配列バッファー(VAO)を作成して割り当て
					GL.GenVertexArrays(1, out vao);
					GL.BindVertexArray(vao);

					// シェーダー内の位置の場所を取得
					int positionLocation = GL.GetAttribLocation(program, "position");

					// 位置を有効化
					GL.EnableVertexAttribArray(positionLocation);

					// 位置のサイズとオフセットを設定
					GL.VertexAttribPointer(
						positionLocation,
						3,
						VertexAttribPointerType.Float,
						false,
						Vector3.SizeInBytes,
						0);

					// 作成した頂点および頂点配列を設定
					GL.BindVertexArray(vao);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
				}

				// カメラや光源などその他の設定
				{
					// 深度を有効化
					GL.Enable(EnableCap.DepthTest);

					// シェーダー側で点の大きさを変更可能に
					GL.Enable(EnableCap.ProgramPointSize);

					// 画面全体を表示
					GL.Viewport(0, 0, glControl.Width, glControl.Height);

					// ビュー（カメラ）を作成して割り当て
					Matrix4 view =
						Matrix4.LookAt(
							new Vector3(1, 3, 2),
							new Vector3(0, 0, 0),
							new Vector3(0, 0, 1)
						);
					GL.UniformMatrix4(GL.GetUniformLocation(program, "view"), false, ref view);

					// 投影法を作成して割り当て
					Matrix4 projection = Matrix4.CreateOrthographic(
						(float)glControl.Width/100,
						(float)glControl.Height/100,
						-20, 20);
					GL.UniformMatrix4(GL.GetUniformLocation(program, "projection"), false, ref projection);
				}
			};

			// コントロールの描画時には
			this.glControl.Paint += (sender, e) =>
			{
				// コントロールを有効化
				glControl.MakeCurrent();

				// 画面をクリア
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				// 描画
				GL.DrawElements(BeginMode.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);

				// バッファーを交換
				glControl.SwapBuffers();
			};

			// 終了時に
			this.Closed += (sender, e) =>
			{
				// バッファー等を削除
				GL.DeleteBuffers(1, ref vbo);
				GL.DeleteBuffers(1, ref ebo);
				GL.DeleteVertexArrays(1, ref vao);

				// プログラムを削除
				GL.DeleteProgram(program);
			};
		}
	}
}