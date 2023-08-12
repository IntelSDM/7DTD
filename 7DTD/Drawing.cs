using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cheat
{
    class Drawing
    {
        static Drawing()
        {
            drawMaterial = new Material(Shader.Find("Hidden/Internal-Colored"))
            {
                hideFlags = (HideFlags)61
            };

            drawMaterial.SetInt("_SrcBlend", 5);
            drawMaterial.SetInt("_DstBlend", 10);
            drawMaterial.SetInt("_Cull", 0);
            drawMaterial.SetInt("_ZWrite", 0);
        }
        private static Material drawMaterial;

        public static void DrawHealthBar(EntityAlive entity, float height, float x, float y)
        {
            float maxHealth = entity.Stats.Health.Max;
            float curHealth = entity.Stats.Health.Value;
            float percentage = curHealth / (float)maxHealth;
            float barHeight = height * percentage;
            Color32 barColour = GetHealthColour(entity);
            DrawFilledBox(x - 5f, y, 4f, height, new Color32(0, 0, 0, 180)); // draw health bar background
            DrawFilledBox(x - 4f, y + height - barHeight - 1f, 2f, barHeight, barColour); // draw healthbar
        }
        private static Color32 GetHealthColour(EntityAlive entity)
        {
            float maxhp = entity.Stats.Health.Max;
            float hp = entity.Stats.Health.Value;
            float percent2 = (hp / maxhp) * 100;
            Color32 barcol = new Color32();
            if (percent2 <= 100 && percent2 >= 86)
            {
                barcol = new Color32(15, 212, 10, 255);
            }
            if (percent2 <= 85 && percent2 >= 66)
            {
                barcol = new Color32(253, 219, 9, 200);

            }
            if (percent2 <= 65 && percent2 >= 35)
            {
                barcol = new Color32(249, 108, 24, 200);
            }
            if (percent2 <= 34 && percent2 >= 0)
            {
                barcol = new Color32(249, 3, 3, 255);
            }
            return barcol;
        }
        public static void DrawCircle(Color Col, Vector2 Center, float Radius)
        {
            GL.PushMatrix();

            if (!drawMaterial.SetPass(0))
            {
                GL.PopMatrix();
                return;
            }

            GL.Begin(1);
            GL.Color(Col);

            for (float num = 0f; num < 6.28318548f; num += 0.05f)
            {
                GL.Vertex(new Vector3(Mathf.Cos(num) * Radius + Center.x, Mathf.Sin(num) * Radius + Center.y));
                GL.Vertex(new Vector3(Mathf.Cos(num + 0.05f) * Radius + Center.x, Mathf.Sin(num + 0.05f) * Radius + Center.y));
            }

            GL.End();
            GL.PopMatrix();
        }


        public static void RectFilled(float x, float y, float width, float height, Color color)
        {
            if (color != textureColor)
            {
                textureColor = color;
                texture.SetPixel(0, 0, color);
                texture.Apply();
            }
            GUI.DrawTexture(new Rect(x, y, width, height), texture);
        }

        public static void RectOutlined(float x, float y, float width, float height, Color color, float thickness = 1f)
        {
            RectFilled(x, y, thickness, height, color);
            RectFilled(x + width - thickness, y, thickness, height, color);
            RectFilled(x + thickness, y, width - thickness * 2f, thickness, color);
            RectFilled(x + thickness, y + height - thickness, width - thickness * 2f, thickness, color);
        }

        public static void DrawShadowString(Vector2 pos, string text, string shadowText, Color color, bool center = true, int size = 12)
        {
            style.fontSize = size;
            style.richText = true;
            style.font = tahoma;
            style.normal.textColor = color;
            style.fontStyle = FontStyle.Bold;
            outlineStyle.fontSize = size;
            outlineStyle.richText = true;
            outlineStyle.font = tahoma;
            outlineStyle.normal.textColor = new Color(0f, 0f, 0f, 1f);
            outlineStyle.fontStyle = FontStyle.Bold;
            GUIContent content = new GUIContent(text);
            GUIContent content2 = new GUIContent(shadowText);
            if (center)
            {
                pos.x -= style.CalcSize(content).x / 2f;
            }
            GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 300f, 25f), content2, outlineStyle);
            GUI.Label(new Rect(pos.x, pos.y, 300f, 25f), content, style);
        }

        public static void BoxRect(Rect rect, Color color)
        {
            if (color != textureColor)
            {
                texture.SetPixel(0, 0, color);
                texture.Apply();
                textureColor = color;
            }
            GUI.DrawTexture(rect, texture);
        }
        public static void DrawLine(Vector2 startPos, Vector2 endPos, Color color, float thickness)
        {
            if (texture != null)
            {
                texture.SetPixel(0, 0, color);
                texture.wrapMode = TextureWrapMode.Repeat;
                texture.Apply();
            }
            DrawLineStretched(startPos, endPos, texture, thickness);
        }

        public static void DrawLineStretched(Vector2 lineStart, Vector2 lineEnd, Texture2D texture, float thickness)
        {
            Vector2 vector = lineEnd - lineStart;
            float num = 57.29578f * Mathf.Atan(vector.y / vector.x);
            if (vector.x < 0f)
            {
                num += 180f;
            }
            if (thickness < 1f)
            {
                thickness = 1f;
            }
            int num2 = (int)Mathf.Ceil(thickness / 2f);
            GUIUtility.RotateAroundPivot(num, lineStart);
            GUI.DrawTexture(new Rect(lineStart.x, lineStart.y - (float)num2, vector.magnitude, thickness), texture);
            GUIUtility.RotateAroundPivot(-num, lineStart);
        }

        public static void DrawString(Vector2 pos, string text, Color color, bool center = true, int size = 12, FontStyle fontStyle = FontStyle.Bold, int depth = 1)
        {
            style.fontSize = size;
            style.richText = true;
            style.font = tahoma;
            style.normal.textColor = color;
            style.fontStyle = fontStyle;
            outlineStyle.fontSize = size;
            outlineStyle.richText = true;
            outlineStyle.font = tahoma;
            outlineStyle.normal.textColor = new Color(0f, 0f, 0f, 1f);
            outlineStyle.fontStyle = fontStyle;
            GUIContent content = new GUIContent(text);
            GUIContent content2 = new GUIContent(text);
            if (center)
            {
                pos.x -= style.CalcSize(content).x / 2f;
            }
            switch (depth)
            {
                case 0:
                    GUI.Label(new Rect(pos.x, pos.y, 3000f, 25f), content, style);
                    return;
                case 1:
                    GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 3000f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y, 3000f, 25f), content, style);
                    return;
                case 2:
                    GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 3000f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x - 1f, pos.y - 1f, 3000f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y, 3000f, 25f), content, style);
                    return;
                case 3:
                    GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 3000f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x - 1f, pos.y - 1f, 3000f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y - 1f, 3000f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y + 1f, 3000f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y, 3000f, 25f), content, style);
                    return;
                default:
                    return;
            }
        }
        public static void DrawStringMenu(Vector2 pos, string text, Color color, bool center = true, int size = 12, FontStyle fontStyle = FontStyle.Bold, int depth = 1, int index = 0, int parent = 0)
        {
            style.fontSize = size;
            style.richText = true;
            style.font = tahoma;
            style.normal.textColor = color;
            style.fontStyle = fontStyle;
            outlineStyle.fontSize = size;
            outlineStyle.richText = true;
            outlineStyle.font = tahoma;
            outlineStyle.normal.textColor = new Color(0f, 0f, 0f, 1f);
            outlineStyle.fontStyle = fontStyle;
            GUIContent content = new GUIContent(text);
            GUIContent content2 = new GUIContent(text);
            int sizeplus = size + 2;
            if (parent == index)
            {
                style.normal.textColor = Color.cyan;
                style.fontSize = 16;
                string instring = text + " ->";
                content = new GUIContent(instring);
                content2 = new GUIContent(instring);
                depth = 0;
            }

            if (center)
            {
                pos.x -= style.CalcSize(content).x / 2f;
            }
            switch (depth)
            {
                case 0:
                    GUI.Label(new Rect(pos.x, pos.y, 300f, 25f), content, style);
                    return;
                case 1:
                    GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y, 300f, 25f), content, style);
                    return;
                case 2:
                    GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x - 1f, pos.y - 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y, 300f, 25f), content, style);
                    return;
                case 3:
                    GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x - 1f, pos.y - 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y - 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y + 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y, 300f, 25f), content, style);
                    return;
                default:
                    return;
            }
        }
        public static void DrawStringMenuTog(Vector2 pos, string text, Color color, bool center = true, int size = 12, FontStyle fontStyle = FontStyle.Bold, int depth = 1, int index = 0, int parent = 0)
        {
            style.fontSize = size;
            style.richText = true;
            style.font = tahoma;
            style.normal.textColor = color;
            style.fontStyle = fontStyle;
            outlineStyle.fontSize = size;
            outlineStyle.richText = true;
            outlineStyle.font = tahoma;
            outlineStyle.normal.textColor = new Color(0f, 0f, 0f, 1f);
            outlineStyle.fontStyle = fontStyle;
            GUIContent content = new GUIContent(text);
            GUIContent content2 = new GUIContent(text);
            int sizeplus = size + 2;
            if (parent == index)
            {
                style.normal.textColor = Color.cyan;
                style.fontSize = 16;
                string instring = text;
                content = new GUIContent(instring);
                content2 = new GUIContent(instring);
                depth = 0;
            }

            if (center)
            {
                pos.x -= style.CalcSize(content).x / 2f;
            }
            switch (depth)
            {
                case 0:
                    GUI.Label(new Rect(pos.x, pos.y, 300f, 25f), content, style);
                    return;
                case 1:
                    GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y, 300f, 25f), content, style);
                    return;
                case 2:
                    GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x - 1f, pos.y - 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y, 300f, 25f), content, style);
                    return;
                case 3:
                    GUI.Label(new Rect(pos.x + 1f, pos.y + 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x - 1f, pos.y - 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y - 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y + 1f, 300f, 25f), content2, outlineStyle);
                    GUI.Label(new Rect(pos.x, pos.y, 300f, 25f), content, style);
                    return;
                default:
                    return;
            }
        }
        public static void PlayerCornerBox(Vector2 Head, float Width, float Height, float thickness, int distance, Color color)
        {
            int num = (int)(Width / 4f);
            int num2 = num;
            RectFilled(Head.x - Width / 2f - 1f, Head.y - 1f, (float)(num + 2), 3f, Color.black);

            RectFilled(Head.x - Width / 2f - 1f, Head.y - 1f, 3f, (float)(num2 + 2), Color.black);

            RectFilled(Head.x + Width / 2f - (float)num - 1f, Head.y - 1f, (float)(num + 2), 3f, Color.black);
            RectFilled(Head.x + Width / 2f - 1f, Head.y - 1f, 3f, (float)(num2 + 2), Color.black);
            RectFilled(Head.x - Width / 2f - 1f, Head.y + Height - 4f, (float)(num + 2), 3f, Color.black);

            RectFilled(Head.x - Width / 2f - 1f, Head.y + Height - (float)num2 - 4f, 3f, (float)(num2 + 2), Color.black);

            RectFilled(Head.x + Width / 2f - (float)num - 1f, Head.y + Height - 4f, (float)(num + 2), 3f, Color.black);
            RectFilled(Head.x + Width / 2f - 1f, Head.y + Height - (float)num2 - 4f, 3f, (float)(num2 + 3), Color.black);
            RectFilled(Head.x - Width / 2f, Head.y, (float)num, 1f, color);

            RectFilled(Head.x - Width / 2f, Head.y, 1f, (float)num2, color);

            RectFilled(Head.x + Width / 2f - (float)num, Head.y, (float)num, 1f, color);
            RectFilled(Head.x + Width / 2f, Head.y, 1f, (float)num2, color);
            RectFilled(Head.x - Width / 2f, Head.y + Height - 3f, (float)num, 1f, color);

            RectFilled(Head.x - Width / 2f, Head.y + Height - (float)num2 - 3f, 1f, (float)num2, color);

            RectFilled(Head.x + Width / 2f - (float)num, Head.y + Height - 3f, (float)num, 1f, color);
            RectFilled(Head.x + Width / 2f, Head.y + Height - (float)num2 - 3f, 1f, (float)(num2 + 1), color);
        }
        public static void DrawFilledBox(float x, float y, float width, float height, Color color)
        {
            bool flag = _coloredBoxTexture == null;
            if (flag)
            {
                _coloredBoxTexture = new Texture2D(1, 1);
            }
            bool flag2 = _coloredBoxColor != color;
            if (flag2)
            {
                _coloredBoxColor = color;
                _coloredBoxTexture.SetPixel(0, 0, _coloredBoxColor);
                _coloredBoxTexture.wrapMode = TextureWrapMode.Repeat;
                _coloredBoxTexture.Apply();
            }
            GUI.DrawTexture(new Rect(x, y, width, height), _coloredBoxTexture);
        }
        private static Color _coloredBoxColor;
        private static Texture2D _coloredBoxTexture;




        public static Material DrawMaterial;

        private static Color textureColor;

        private static Color outlineColor = new Color(0f, 0f, 0f, 1f);

        private static Texture2D texture = new Texture2D(1, 1);

        private static GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12
        };

        private static GUIStyle outlineStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12
        };

        public static Font tahoma = Font.CreateDynamicFontFromOSFont("Segoe UI", 12);

        private static Font segoeUI = Font.CreateDynamicFontFromOSFont("Segoe UI", 12);

    }
}
