using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectExtensions
{
    public static Rect Padding(this Rect rect, float top, float bottom, float left, float right)
    {
        return new Rect(rect.x + left, rect.y + top, rect.width - left - right, rect.height - top - bottom);
    }

    public static Rect Padding(this Rect rect, float vertical, float horizontal) =>
        rect.Padding(vertical, vertical, horizontal, horizontal);

    public static Rect Padding(this Rect rect, float padding) => rect.Padding(padding, padding);

    public static Rect Margin(this Rect rect, float top, float bottom, float left, float right)
    {
        return rect.Padding(-top, -bottom, -left, -right);
    }
    
    public static Rect Margin(this Rect rect, float vertical, float horizontal) =>
        rect.Margin(vertical, vertical, horizontal, horizontal);

    public static Rect Margin(this Rect rect, float margin) => rect.Margin(margin, margin);

    public static Rect[] SplitX(this Rect rect, params float[] factors)
    {
        var rects = new Rect[factors.Length];
        var current = rect;
        
        for (int i = 0; i < factors.Length; i++)
        {
            current.width = rect.width * factors[i];
            rects[i] = current;
            current.x = current.xMax;
        }

        return rects;
    }
    
    public static Rect[] SplitY(this Rect rect, params float[] factors)
    {
        var rects = new Rect[factors.Length];
        var current = rect;
        
        for (int i = 0; i < factors.Length; i++)
        {
            current.height = rect.height * factors[i];
            rects[i] = current;
            current.y = current.yMax;
        }

        return rects;
    }

    public static Rect[] AbsSplitX(this Rect rect, float splitDistance)
    {
        var d = splitDistance >= 0 ? splitDistance : rect.width + splitDistance;
        
        return new []
        {
            new Rect(rect.x, rect.y, d, rect.height),
            new Rect(rect.x + d, rect.y, rect.width - d, rect.height)
        };
    }
    
    public static Rect[] AbsSplitY(this Rect rect, float splitDistance)
    {
        var d = splitDistance >= 0 ? splitDistance : rect.height + splitDistance;
        
        return new []
        {
            new Rect(rect.x, rect.y, rect.width, d),
            new Rect(rect.x, rect.y + d, rect.width, rect.height - d)
        };
    }
    
    public static Rect With(this Rect rect, float? x = null, float? y = null, float? w = null, float? h = null)
    {
        return new Rect(x ?? rect.x, y ?? rect.y, w ?? rect.width, h ?? rect.height);
    }
}
