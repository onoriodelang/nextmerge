using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lipsar.Editor
{
    public enum PropertyAction { None, Hide, Disable }

    public class ShaderPropertyInfo
    {
        public string name;
        public string[] keywords;
        public PropertyAction action;
        public int indentLevel;

        public ShaderPropertyInfo(string name, string[] keywords, int indentLevel = 0, PropertyAction action = PropertyAction.None)
        {
            this.name = name;
            this.keywords = keywords;
            this.action = action;
            this.indentLevel = indentLevel;
        }
    }

    public class ShaderEditorStyles
    {
        public static GUIStyle logo;
        public static GUIStyle logoTitle;
        public static Texture2D logoImage;

        public static GUIStyle sectionTitle;

        public static void InitImages(string path)
        {

            if (logoImage == null)
                logoImage = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "logo.png");
        }

        public static void Init()
        {
            if (logo == null)
            {
                // Logo Title
                logoTitle = new GUIStyle(GUI.skin.GetStyle("Label"));
                logoTitle.fontSize = 25;

                //logo
                logo = new GUIStyle(GUI.skin.GetStyle("Label"));
                logo.alignment = TextAnchor.UpperRight;
                logo.stretchWidth = false;
                logo.stretchHeight = false;
                logo.normal.background = logoImage;

                //sectionTitle
                sectionTitle = new GUIStyle(GUI.skin.GetStyle("Label"));
                sectionTitle.fontStyle = FontStyle.Bold;
                sectionTitle.fontSize = 14;
            }
        }
    }

    public class BaseShaderEditor : ShaderGUI
    {

        protected string title = "Lipsar";

        virtual public Dictionary<string, ShaderPropertyInfo> GetShaderPropertyInfos()
        {
            return null;
        }

        public void DrawHeader(string shaderName = default)
        {
            ShaderEditorStyles.InitImages("Assets/Shaders/Lipsar/Logo/");
            ShaderEditorStyles.Init();

            /////////////////////////////////////////////////////////////////////////////////
            //// Logo
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();//IAPEditorStyles.logoBackground
            EditorGUILayout.LabelField(title, ShaderEditorStyles.logoTitle, GUILayout.Height(40));
            EditorGUILayout.LabelField("", ShaderEditorStyles.logo, GUILayout.Width(64), GUILayout.Height(64));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(shaderName.Substring(title.Length + 1), ShaderEditorStyles.sectionTitle, GUILayout.Height(20));
            GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.Space();
            //// Logo
            /// /////////////////////////////////////////////////////////////////////////////////
        }

        protected void DrawHorizontalLine()
        {
            GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(1));
        }

        protected void DrawSectionHeader(string title)
        {
            EditorGUILayout.LabelField(title, ShaderEditorStyles.sectionTitle, GUILayout.Height(20));
            //GUILayout.Box(GUIContent.none,GUILayout.ExpandWidth(true),GUILayout.Height(1));
            DrawHorizontalLine();
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Raises the GU event.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        /// <param name="properties">Properties.</param>
        override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material targetMat = materialEditor.target as Material;
            DrawHeader(targetMat.shader.name);
            materialEditor.SetDefaultGUIWidths();

            // get the current keywords from the material
            foreach (MaterialProperty p in properties)
            {
                if (GetShaderPropertyInfos().ContainsKey(p.name))
                {
                    ShaderPropertyInfo info = GetShaderPropertyInfos()[p.name];

                    if (info.action == PropertyAction.None)
                    {
                        materialEditor.ShaderProperty(p, p.displayName, info.indentLevel);
                    }
                    else
                    {
                        if (info.keywords != null)
                        {
                            //Loop for keywords
                            foreach (string keyword in info.keywords)
                            {
                                if (targetMat.IsKeywordEnabled(keyword))
                                {
                                    materialEditor.ShaderProperty(p, p.displayName, info.indentLevel);
                                }
                                else
                                {
                                    if (info.action == PropertyAction.Hide)
                                    {

                                    }
                                    else if (info.action == PropertyAction.Disable)
                                    {

                                        EditorGUI.BeginDisabledGroup(true);
                                        materialEditor.ShaderProperty(p, p.displayName, info.indentLevel);
                                        EditorGUI.EndDisabledGroup();
                                    }
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    //Debug.Log("pp " + p.displayName);
                    materialEditor.ShaderProperty(p, p.displayName, 0);
                }
            }

            EditorGUILayout.Space();
            GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.Space();
            materialEditor.RenderQueueField();
            materialEditor.EnableInstancingField();
            materialEditor.DoubleSidedGIField();
        }
    } // BaseShaderEditor

    [InitializeOnLoad]
    public class CustomShaderEditor : BaseShaderEditor
    {
        /// <summary>
		/// The shader property infos.
		/// </summary>
		public static Dictionary<string, ShaderPropertyInfo> _shaderPropertyInfos;

        override public Dictionary<string, ShaderPropertyInfo> GetShaderPropertyInfos()
        {
            return _shaderPropertyInfos;
        }

        static CustomShaderEditor()
        {
            _shaderPropertyInfos = new Dictionary<string, ShaderPropertyInfo>
            {
                { "_Cutoff", new ShaderPropertyInfo("_Cutoff", new string[] { "_CUTOUT_ON" }, 1, PropertyAction.Disable) },

                // ---- TRIPLANAR MAPPING ----
                { "_Sharpness", new ShaderPropertyInfo("_Sharpness", new string[] { "_TRIPLANARMAPPING_ON" }, 1, PropertyAction.Hide) },

                // ---- WORLD SPACE GRADIENT ----
                { "_ColorTopGradient", new ShaderPropertyInfo("_ColorTopGradient", new string[] { "_GLOBALGRADIENT_ON" }, 1, PropertyAction.Hide) },
                { "_ColorBottomGradient", new ShaderPropertyInfo("_ColorBottomGradient", new string[] { "_GLOBALGRADIENT_ON" }, 1, PropertyAction.Hide) },
                { "_GradientCenterX", new ShaderPropertyInfo("_GradientCenterX", new string[] { "_GLOBALGRADIENT_ON" }, 1, PropertyAction.Hide) },
                { "_GradientCenterY", new ShaderPropertyInfo("_GradientCenterY", new string[] { "_GLOBALGRADIENT_ON" }, 1, PropertyAction.Hide) },
                { "_GradientSize", new ShaderPropertyInfo("_GradientSize", new string[] { "_GLOBALGRADIENT_ON" }, 1, PropertyAction.Hide) },
                { "_GradientAngle", new ShaderPropertyInfo("_GradientAngle", new string[] { "_GLOBALGRADIENT_ON" }, 1, PropertyAction.Hide) },

                // ---- LOCAL SPACE GRADIENT ----
                { "_ColorLocalTopGradient", new ShaderPropertyInfo("_ColorLocalTopGradient", new string[] { "_LOCALGRADIENT_ON" }, 1, PropertyAction.Hide) },
                { "_ColorLocalBottomGradient", new ShaderPropertyInfo("_ColorLocalBottomGradient", new string[] { "_LOCALGRADIENT_ON" }, 1, PropertyAction.Hide) },
                { "_ColorLocalGradienMixDelta", new ShaderPropertyInfo("_ColorLocalGradienMixDelta", new string[] { "_LOCALGRADIENT_ON" }, 1, PropertyAction.Hide) },

                // ---- CUBEMAP ----
                { "_Cube", new ShaderPropertyInfo("_Cube", new string[] { "_CUBEMAP_ON" }, 1, PropertyAction.Hide) },
                { "_MixPower", new ShaderPropertyInfo("_MixPower", new string[] { "_CUBEMAP_ON" }, 1, PropertyAction.Hide) },

                { "_SpecularColor", new ShaderPropertyInfo("_SpecularColor", new string[] { "_SPECULAR_ON" }, 1, PropertyAction.Hide) },
                { "_Gloss", new ShaderPropertyInfo("_Gloss", new string[] { "_SPECULAR_ON" }, 1, PropertyAction.Hide) }
            };
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);
        }
    }
}