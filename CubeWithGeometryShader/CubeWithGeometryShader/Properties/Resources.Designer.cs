﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.261
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace LWisteria.StudiesOfOpenTK.CubeWithGeometryShader.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LWisteria.StudiesOfOpenTK.CubeWithGeometryShader.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   #version 330
        ///
        /////! output: color
        ///out vec4 outColor;
        ///
        ///
        /////! entry point
        ///void main()
        ///{
        ///	//! set diffusing color
        ///	outColor = vec4(1, 0, 0, 1);
        ///} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string fragment {
            get {
                return ResourceManager.GetString("fragment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   #version 330
        ///#extension GL_EXT_geometry_shader4 : enable
        ///
        ///layout(points) in; 
        ///layout(triangle_strip, max_vertices=4) out;
        ///
        /////! input: view (camera)
        ///uniform mat4 view;
        ///
        /////! input: projection matrix
        ///uniform mat4 projection;
        ///
        /////! entry point
        ///void main(void)
        ///{
        ///	// calculate projection and model view matrix
        ///	mat4 projectionView = projection*view;
        ///
        ///	// original point
        ///	gl_Position = projectionView* gl_in[0].gl_Position;
        ///	EmitVertex();
        ///
        ///	// other points1
        ///	gl_Position = projectionView* (gl_in[ [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string geometry {
            get {
                return ResourceManager.GetString("geometry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   #version 330
        ///
        /////! input: position of vertex
        ///in vec3 position;
        ///
        /////! entry point
        ///void main()
        ///{
        ///	//! set position of vertex
        ///	gl_Position = vec4(position, 1.0);
        ///} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string vertex {
            get {
                return ResourceManager.GetString("vertex", resourceCulture);
            }
        }
    }
}
