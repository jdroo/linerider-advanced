﻿using System;

namespace NGraphics
{
	public class FilterCanvas : ICanvas
	{
		public readonly ICanvas NextCanvas;

		public FilterCanvas (ICanvas nextCanvas)
		{
			if (nextCanvas == null)
				throw new ArgumentNullException ("nextCanvas");
			NextCanvas = nextCanvas;
		}

		public virtual Pen GetPen (Pen pen)
		{
			return pen;
		}

		public virtual Brush GetBrush (Brush brush)
		{
			return brush;
		}

		#region ICanvas implementation

		public virtual void SaveState ()
		{
			NextCanvas.SaveState ();
		}

		public virtual void Transform (Transform transform)
		{
			NextCanvas.Transform (transform);
		}

		public virtual void RestoreState ()
		{
			NextCanvas.RestoreState ();
		}

		public virtual Size MeasureText(string text, Font font)
		{
			return NextCanvas.MeasureText(text, font);
		}

		public virtual void DrawText (string text, Rect frame, Font font, TextAlignment alignment = TextAlignment.Left, Pen pen = null, Brush brush = null)
		{
			NextCanvas.DrawText (text, frame, font, alignment, GetPen (pen), GetBrush (brush));
		}
        public void DrawPath(Path path)
        {
            NextCanvas.DrawPath(path);
        }

        public virtual void DrawRectangle (Rect frame, Pen pen = null, Brush brush = null)
		{
			NextCanvas.DrawRectangle (frame, GetPen (pen), GetBrush (brush));
		}

		public virtual void DrawEllipse (Rect frame, Pen pen = null, Brush brush = null)
		{
			NextCanvas.DrawEllipse (frame, GetPen (pen), GetBrush (brush));
		}

		public virtual void DrawImage (IImage image, Rect frame, double alpha = 1.0)
		{
			NextCanvas.DrawImage (image, frame, alpha);
		}

		#endregion
	}
}

