﻿//
//  GameRenderer.cs
//
//  Author:
//       Noah Ablaseau <nablaseau@hotmail.com>
//
//  Copyright (c) 2017 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using linerider.Tools;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace linerider.Drawing
{
    public static class GameRenderer
    {
        #region Fields

        internal static OpenGLCanvas canvas;
        public static GLWindow Game;

        #endregion Fields

        #region Methods
        public static void BeginDrawingGameCoords()
        {

        }

        public static void DrawIteration(float opacity, Rider fullrider, int iteration, bool momentumvectors = false, bool drawcontactpoints = false)
        {
            var rider = fullrider.iterations[iteration];
            DrawScarf(rider.ScarfLines, opacity);
            var points = rider.ModelAnchors;

            DrawGraphic(Models.Leg, points[4].Position, points[9].Position, opacity);

            DrawGraphic(Models.Arm, points[5].Position, points[7].Position, opacity);
            if (!rider.Crashed)
                RenderRoundedLine(points[7].Position, points[3].Position, Color.Black, 0.1f);

            if (rider.SledBroken)
            {
                DrawGraphic(Models.BrokenSled, points[0].Position, points[3].Position, opacity);
            }
            else
            {
                DrawGraphic(Models.Sled, points[0].Position, points[3].Position, opacity);
            }

            DrawGraphic(Models.Leg, points[4].Position, points[8].Position, opacity);
            if (!rider.Crashed)
            {
                DrawGraphic(Models.Bosh, points[4].Position, points[5].Position, opacity);
            }
            else
            {
                DrawGraphic(Models.BoshDead, points[4].Position, points[5].Position, opacity);
            }
            if (!rider.Crashed)
                RenderRoundedLine(points[6].Position, points[3].Position, Color.Black, 0.1f);

            DrawGraphic(Models.Arm, points[5].Position, points[6].Position, opacity);
            if (momentumvectors)
            {
                foreach (var anchor in rider.ModelAnchors)
                {
                    var vec1 = anchor.Position;
                    var vec2 = vec1 + (anchor.Position - anchor.Prev + anchor.Gravity);
                    RenderRoundedLine(vec1, vec2, Color.Red, 1f / 2, false);
                }
            }
            if (drawcontactpoints)
            {
                using (new GLEnableCap(EnableCap.Blend))
                {
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    var brokenpoints = Game.Track.DiagnoseIteration(fullrider, iteration);
                    for (var i = 0; i < rider.ModelLines.Length; i++)
                    {
                        var c = Color.FromArgb(255, Color.FromArgb(0xCC72B7));
                        if (rider.ModelLines[i] is BindLine)
                        {
                            continue;
                        }
                        else if (rider.ModelLines[i] is RepelLine)
                        {
                            c = Color.CornflowerBlue;
                            RenderRoundedLine(rider.ModelLines[i].Position, rider.ModelLines[i].Position2, c, 1f / 4, false);
                        }
                        else if (i <= 3)
                        {
                            RenderRoundedLine(rider.ModelLines[i].Position, rider.ModelLines[i].Position2, c, 1f / 4, false);
                        }
                    }
                    for (var i = 0; i < rider.ModelLines.Length; i++)
                    {
                        if (rider.ModelLines[i] is BindLine)
                        {
                            if (rider.Crashed)
                                continue;
                            if (brokenpoints.Contains(i))
                                RenderRoundedLine(rider.ModelLines[i].Position, rider.ModelLines[i].Position2, Color.DarkOrange, 1f / 4, false);
                        }
                    }
                    for (int i = 0; i < rider.ModelAnchors.Length; i++)
                    {
                        Color c = Color.Cyan;
                        if (((i == 0 || i == 1) && brokenpoints.Contains(-1)) || ((i == 4 || i == 5) && brokenpoints.Contains(-2)))
                        {
                            c = Color.Blue;
                        }
                        RenderRoundedLine(rider.ModelAnchors[i].Position, rider.ModelAnchors[i].Position, c, 1f / 4, false);

                    }
                }
            }
        }
        public static void DrawRider(float opacity, Rider rider, bool scarf = false, bool drawcontactpoints = false, bool momentumvectors = false)
        {
            if (scarf)
                DrawScarf(rider.ScarfLines, opacity);
            var points = rider.ModelAnchors;

            DrawGraphic(Models.Leg, points[4].Position, points[9].Position, opacity);

            DrawGraphic(Models.Arm, points[5].Position, points[7].Position, opacity);
            if (!rider.Crashed)
                RenderRoundedLine(points[7].Position, points[3].Position, Color.Black, 0.1f);

            if (rider.SledBroken)
            {
                DrawGraphic(Models.BrokenSled, points[0].Position, points[3].Position, opacity);
            }
            else
            {
                DrawGraphic(Models.Sled, points[0].Position, points[3].Position, opacity);
            }

            DrawGraphic(Models.Leg, points[4].Position, points[8].Position, opacity);
            if (!rider.Crashed)
            {
                DrawGraphic(Models.Bosh, points[4].Position, points[5].Position, opacity);
            }
            else
            {
                DrawGraphic(Models.BoshDead, points[4].Position, points[5].Position, opacity);
            }
            if (!rider.Crashed)
                RenderRoundedLine(points[6].Position, points[3].Position, Color.Black, 0.1f);

            DrawGraphic(Models.Arm, points[5].Position, points[6].Position, opacity);
            List<Vertex> vertices = new List<Vertex>(300);
            if (momentumvectors)
            {
                foreach (var anchor in rider.ModelAnchors)
                {
                    var vec1 = anchor.Position;
                    var vec2 = vec1 + (anchor.Momentum);
                    vertices.AddRange(GenRoundedLine(vec1, vec2, Color.Red, 1f / 2, false));
                }
            }
            if (drawcontactpoints)
            {
                var brokenpoints = Game.Track.Diagnose(rider);
                for (var i = 0; i < rider.ModelLines.Length; i++)
                {
                    var c = Color.FromArgb(255, Color.FromArgb(0xCC72B7));
                    if (rider.ModelLines[i] is BindLine)
                    {
                        continue;
                    }
                    else if (rider.ModelLines[i] is RepelLine)
                    {
                        c = Color.CornflowerBlue;
                        vertices.AddRange(GenRoundedLine(rider.ModelLines[i].Position, rider.ModelLines[i].Position2, c, 1f / 4, false));
                    }
                    else if (i <= 3)
                    {
                        vertices.AddRange(GenRoundedLine(rider.ModelLines[i].Position, rider.ModelLines[i].Position2, c, 1f / 4, false));
                    }
                }
                for (var i = 0; i < rider.ModelLines.Length; i++)
                {
                    if (rider.ModelLines[i] is BindLine)
                    {
                        if (rider.Crashed)
                            continue;
                        if (brokenpoints.Contains(i))
                            vertices.AddRange(GenRoundedLine(rider.ModelLines[i].Position, rider.ModelLines[i].Position2, Color.DarkOrange, 1f / 4, false));
                    }
                }
                for (int i = 0; i < rider.ModelAnchors.Length; i++)
                {
                    Color c = Color.Cyan;
                    if (((i == 0 || i == 1) && brokenpoints.Contains(-1)) || ((i == 4 || i == 5) && brokenpoints.Contains(-2)))
                    {
                        c = Color.Blue;
                    }
                    vertices.AddRange(GenRoundedLine(rider.ModelAnchors[i].Position, rider.ModelAnchors[i].Position, c, 1f / 4, false));
                }
            }
            if (vertices.Count != 0)
            {
                VAO vao = new VAO(false, false);
                vao.Texture = StaticRenderer.CircleTex;
                vao.AddVerticies(vertices);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                vao.Draw(PrimitiveType.Triangles);
            }
        }

        public static void DrawScarf(DynamicLine[] lines, float opacity)
        {
            GLEnableCap blend = null;
            if (opacity < 1)
            {
                blend = new GLEnableCap(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            }

            GameDrawingMatrix.Enter();
            VAO scarf = new VAO(false, true);//VAO does not need disposing, it does not allocate a buffer
            bool t = false;
            for (int i = 0; i < lines.Length; i++)
            {
                Color c = Color.FromArgb((byte)(255 * opacity), 209, 1, 1);
                var alt = Color.FromArgb((byte)(255 * opacity), 255, 100, 100);
                var diff = lines[i].Position2 - lines[i].Position;
                var dangle = MathHelper.RadiansToDegrees(Math.Atan2(diff.Y, diff.X)) + 90;
                Vector2d q1;
                Vector2d q2;
                Vector2d q3;
                Vector2d q4;
                float size = 2;
                if (i == 0)
                {
                    var diff2 = lines[i + 1].Position2 - lines[i + 1].Position;
                    var dangle2 = MathHelper.RadiansToDegrees(Math.Atan2(diff2.Y, diff2.X)) + 90;
                    Turtle turtle = new Turtle(lines[i].Position);
                    turtle.Move(dangle2, -(size / 2));
                    q1 = turtle.Point;
                    turtle.Move(0, size);
                    q2 = turtle.Point;
                    turtle.Point = lines[i].Position2;
                    turtle.Move(dangle, size / 2);
                    q3 = turtle.Point;
                    turtle.Move(0, -size);
                    q4 = turtle.Point;
                }
                else
                {
                    var diff2 = lines[i - 1].Position2 - lines[i - 1].Position;
                    var dangle2 = MathHelper.RadiansToDegrees(Math.Atan2(diff2.Y, diff2.X)) + 90;
                    Turtle turtle = new Turtle(lines[i].Position);
                    turtle.Move(dangle2, -(size / 2));
                    q1 = turtle.Point;
                    turtle.Move(0, size);
                    q2 = turtle.Point;
                    turtle.Point = lines[i].Position2;
                    turtle.Move(dangle, size / 2);
                    q3 = turtle.Point;
                    turtle.Move(0, -size);
                    q4 = turtle.Point;
                }
                scarf.AddVertex(new Vertex((Vector2)q1, t ? c : alt));
                scarf.AddVertex(new Vertex((Vector2)q2, t ? c : alt));
                scarf.AddVertex(new Vertex((Vector2)q3, t ? c : alt));
                scarf.AddVertex(new Vertex((Vector2)q4, t ? c : alt));
                t = !t;
            }
            scarf.Draw(PrimitiveType.Quads);
            GameDrawingMatrix.Exit();

            if (blend != null)
                blend.Dispose();
        }

        public static void DrawTrackLine(StandardLine l, Color color, bool drawwell, bool drawcolor, bool drawknobs, bool redknobs = false)
        {
            color = Color.FromArgb(255, color);
            var thickness = 2;
            Color color2;
            var type = l.GetLineType();
            switch (type)
            {
                case LineType.Blue:
                    color2 = Color.FromArgb(0, 0x66, 0xFF);
                    break;

                case LineType.Red:
                    color2 = Color.FromArgb(0xCC, 0, 0);
                    break;

                default:
                    throw new Exception("Rendering Invalid Line");
            }
            if (drawcolor)
            {
                var loc3 = l.Perpendicular.X > 0 ? (Math.Ceiling(l.Perpendicular.X)) : (Math.Floor(l.Perpendicular.X));
                var loc4 = l.Perpendicular.Y > 0 ? (Math.Ceiling(l.Perpendicular.Y)) : (Math.Floor(l.Perpendicular.Y));
                if (type == LineType.Red)
                {
                    var redline = l as RedLine;
                    GameDrawingMatrix.Enter();
                    GL.Color3(color2);
                    GL.Begin(PrimitiveType.Triangles);
                    var basepos = l.Position2;
                    for (int ix = 0; ix < redline.Multiplier; ix++)
                    {
                        var angle = MathHelper.RadiansToDegrees(Math.Atan2(l.diff.Y, l.diff.X));
                        Turtle t = new Turtle(l.Position2);
                        var basex = 8 + (ix * 2);
                        t.Move(angle, -basex);
                        GL.Vertex2(new Vector2((float)t.X, (float)t.Y));
                        t.Move(90, l.inv ? -8 : 8);
                        GL.Vertex2(new Vector2((float)t.X, (float)t.Y));
                        t.Point = l.Position2;
                        t.Move(angle, -(ix * 2));
                        GL.Vertex2(new Vector2((float)t.X, (float)t.Y));
                    }
                    GL.End();
                    GameDrawingMatrix.Exit();
                }
                RenderRoundedLine(new Vector2d(l.Position.X + loc3, l.Position.Y + loc4),
                    new Vector2d(l.Position2.X + loc3, l.Position2.Y + loc4), color2, thickness);
            }
            RenderRoundedLine(l.Position, l.Position2, color, thickness, drawknobs, redknobs);
            if (drawwell)
            {
                using (new GLEnableCap(EnableCap.Blend))
                {
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    GameDrawingMatrix.Enter();
                    GL.Begin(PrimitiveType.Quads);
                    GL.Color4(new Color4(150, 150, 150, 150));
                    var rect = StaticRenderer.GenerateThickLine((Vector2)l.Position, (Vector2)l.Position2, (float)(StandardLine.Zone * 2));

                    GL.Vertex2(l.Position);
                    GL.Vertex2(l.Position2);
                    GL.Vertex2(rect[l.inv ? 2 : 1]);
                    GL.Vertex2(rect[l.inv ? 3 : 0]);
                    GL.End();
                    GL.PopMatrix();
                }
            }
        }
        private static readonly VAO _roundlinevao = new VAO(false, true);

        public static void RenderRoundedLine(Vector2d position, Vector2d position2, Color color, float thickness, bool knobs = false, bool redknobs = false)
        {
            using (new GLEnableCap(EnableCap.Blend))
            {
                using (new GLEnableCap(EnableCap.Texture2D))
                {
                    var vertices = GenRoundedLine(position, position2, color, thickness, knobs, redknobs);
                    if (vertices.Count != 0)
                    {
                        _roundlinevao.Texture = StaticRenderer.CircleTex;
                        _roundlinevao.Clear();
                        foreach (var v in vertices)
                            _roundlinevao.AddVertex(v);
                        _roundlinevao.SetOpacity(color.A / 255f);
                        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                        _roundlinevao.Draw(PrimitiveType.Triangles);
                        _roundlinevao.Texture = 0;
                    }
                }
            }
        }
        public static List<Vertex> GenRoundedLine(Vector2d position, Vector2d position2, Color color, float thickness, bool knobs = false, bool redknobs = false)
        {
            List<Vertex> vertices = new List<Vertex>(6 * 5);
            var end1 = (position + Game.ScreenTranslation) * Game.Track.Zoom;
            var end2 = (position2 + Game.ScreenTranslation) * Game.Track.Zoom;
            var line = StaticRenderer.GenerateThickLine((Vector2)end1, (Vector2)end2, thickness * Game.Track.Zoom);

            vertices.Add(new Vertex(line[0], color));
            vertices.Add(new Vertex(line[1], color));
            vertices.Add(new Vertex(line[2], color));
            vertices.Add(new Vertex(line[0], color));
            vertices.Add(new Vertex(line[3], color));
            vertices.Add(new Vertex(line[2], color));
            vertices.AddRange(StaticRenderer.FastCircle((Vector2)(end1), Game.Track.Zoom * (thickness / 2), color));
            vertices.AddRange(StaticRenderer.FastCircle((Vector2)(end2), Game.Track.Zoom * (thickness / 2), color));
            if (knobs)
            {
                vertices.AddRange(StaticRenderer.FastCircle((Vector2)(end1), Game.Track.Zoom * (thickness / 3), redknobs ? Color.Red : Color.White));
                vertices.AddRange(StaticRenderer.FastCircle((Vector2)(end2), Game.Track.Zoom * (thickness / 3), redknobs ? Color.Red : Color.White));
            }
            return vertices;
        }

        private static void DrawGraphic(VBO graphic, Vector2d p1, Vector2d rotationAnchor, float opacity)
        {
            var angle = Angle.FromLine(p1, rotationAnchor);
            var offset = -(Game.ScreenPosition - p1);
            GL.PushMatrix();
            GL.Scale(Game.Track.Zoom, Game.Track.Zoom, 0);
            GL.Translate(offset.X, offset.Y, 0);
            GL.Rotate(angle.Degrees, 0, 0, 1);
            GL.Scale(0.5, 0.5, 0);
            graphic.SetOpacity(opacity);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            graphic.Draw(PrimitiveType.Triangles);
            GL.PopMatrix();
        }

        #endregion Methods
    }
}