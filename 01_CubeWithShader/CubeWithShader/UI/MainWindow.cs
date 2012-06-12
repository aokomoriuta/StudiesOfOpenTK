using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace LWisteria.StudiesOfOpenTK.CubeWithShader
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
					int flagmentShaderStatus;

					// 頂点シェーダー・フラグメントシェーダーを作成
					int vertexShader = GL.CreateShader(ShaderType.VertexShader);
					int flagmentShader = GL.CreateShader(ShaderType.FragmentShader);

					// ソースから読み込み
					GL.ShaderSource(vertexShader, Properties.Resources.vertex);
					GL.ShaderSource(flagmentShader, Properties.Resources.fragment);

					// コンパイル
					GL.CompileShader(vertexShader);
					GL.CompileShader(flagmentShader);

					// シェーダーを読み込み
					GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out vertexShaderStatus);
					GL.GetShader(flagmentShader, ShaderParameter.CompileStatus, out flagmentShaderStatus);

					// コンパイルに失敗していたら
					if(vertexShaderStatus != 1)
					{
						// 例外
						throw new CompileShaderException(vertexShader, GL.GetShaderInfoLog(vertexShader));
					}
					else if(flagmentShaderStatus != 1)
					{
						// 例外
						throw new CompileShaderException(flagmentShader, GL.GetShaderInfoLog(flagmentShader));
					}

					// プログラムを作成して、シェーダーを設定
					program = GL.CreateProgram();
					GL.AttachShader(program, vertexShader);
					GL.AttachShader(program, flagmentShader);

					// プログラムをリンク
					GL.LinkProgram(program);

					// 不要になったシェーダーを削除
					GL.DeleteShader(vertexShader);
					GL.DeleteShader(flagmentShader);

					// 使用するプログラムの指定
					GL.UseProgram(program);
				}

				// 頂点・頂点インデックスの作成
				{
					// 頂点データの作成
					var vertices = new Vertex[]
					{
						// -x面
						new Vertex(new Vector3(0, 0, 0), new Vector3(-1, 0, 0)),
						new Vertex(new Vector3(0, 1, 0), new Vector3(-1, 0, 0)),
						new Vertex(new Vector3(0, 1, 1), new Vector3(-1, 0, 0)),
						new Vertex(new Vector3(0, 0, 1), new Vector3(-1, 0, 0)),

						// +x面
						new Vertex(new Vector3(1, 0, 0), new Vector3(1, 0, 0)),
						new Vertex(new Vector3(1, 1, 0), new Vector3(1, 0, 0)),
						new Vertex(new Vector3(1, 1, 1), new Vector3(1, 0, 0)),
						new Vertex(new Vector3(1, 0, 1), new Vector3(1, 0, 0)),

						// -y面
						new Vertex(new Vector3(0, 0, 0), new Vector3(0, -1, 0)),
						new Vertex(new Vector3(1, 0, 0), new Vector3(0, -1, 0)),
						new Vertex(new Vector3(1, 0, 1), new Vector3(0, -1, 0)),
						new Vertex(new Vector3(0, 0, 1), new Vector3(0, -1, 0)),

						// +y面
						new Vertex(new Vector3(0, 1, 0), new Vector3(0, 1, 0)),
						new Vertex(new Vector3(1, 1, 0), new Vector3(0, 1, 0)),
						new Vertex(new Vector3(1, 1, 1), new Vector3(0, 1, 0)),
						new Vertex(new Vector3(0, 1, 1), new Vector3(0, 1, 0)),

						// -z面
						new Vertex(new Vector3(0, 0, 0), new Vector3(0, 0, -1)),
						new Vertex(new Vector3(1, 0, 0), new Vector3(0, 0, -1)),
						new Vertex(new Vector3(1, 1, 0), new Vector3(0, 0, -1)),
						new Vertex(new Vector3(0, 1, 0), new Vector3(0, 0, -1)),

						// +z面
						new Vertex(new Vector3(0, 0, 1), new Vector3(0, 0, 1)),
						new Vertex(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
						new Vertex(new Vector3(1, 1, 1), new Vector3(0, 0, 1)),
						new Vertex(new Vector3(0, 1, 1), new Vector3(0, 0, 1)),
					};

					// 頂点インデックスデータの作成
					var indices = new uint[]
					{
						// -x面
						0,1,2, 0,2,3,

						// +x面
						4,6,5, 4,7,6,

						// -y面
						8,10,9, 8,11,10,

						// +y面
						12,13,14, 12,14,15,

						// -z面
						16,17,18, 16,18,19,

						// +z面
						20,22,21, 20,23,22
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
					GL.BufferData<Vertex>(BufferTarget.ArrayBuffer,
						new IntPtr(vertices.Length * Vertex.Stride),
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

					// シェーダー内の位置と法線の場所を取得
					int positionLocation = GL.GetAttribLocation(program, "position");
					int normalLocation = GL.GetAttribLocation(program, "normal");

					// 位置と法線を有効化
					GL.EnableVertexAttribArray(positionLocation);
					GL.EnableVertexAttribArray(normalLocation);

					// 位置および法線のサイズとオフセットを設定
					GL.VertexAttribPointer(
						positionLocation,
						3,
						VertexAttribPointerType.Float,
						false,
						Vertex.Stride,
						0);
					GL.VertexAttribPointer(
						normalLocation,
						3,
						VertexAttribPointerType.Float,
						true,
						Vertex.Stride,
						Vector3.SizeInBytes);
				}

				// カメラや光源などその他の設定
				{
					// 深度を有効化
					GL.Enable(EnableCap.DepthTest);

					// 表面は時計回り（左回り）に設定
					GL.FrontFace(FrontFaceDirection.Cw);

					// 表面のみ表示
					GL.Enable(EnableCap.CullFace);
					GL.CullFace(CullFaceMode.Back);

					// 画面全体を表示
					GL.Viewport(0, 0, glControl.Width, glControl.Height);

					// ビュー（カメラ）を作成して割り当て
					Matrix4 view =
						Matrix4.LookAt(
							new Vector3(5, 6, 4),
							new Vector3(0, 0, 0),
							new Vector3(0, 0, 1)
						);
					GL.UniformMatrix4(GL.GetUniformLocation(program, "view"), false, ref view);

					// 投影法を作成して割り当て
					Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
						(float)System.Math.PI / 4,
						(float)glControl.Width / (float)glControl.Height,
						0.1f, 10);
					GL.UniformMatrix4(GL.GetUniformLocation(program, "projection"), false, ref projection);

					// 光源の方向を作成して割り当て
					Vector3 lightDir = new Vector3(-5, -6, -4);
					lightDir.Normalize();
					GL.Uniform3(GL.GetUniformLocation(program, "lightDirection"), lightDir);
				}
			};

			// コントロールの描画時には
			this.glControl.Paint += (sender, e) =>
			{
				// コントロールを有効化
				glControl.MakeCurrent();

				// 画面をクリア
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				// 作成した頂点および頂点配列を設定
				GL.BindVertexArray(vao);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

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