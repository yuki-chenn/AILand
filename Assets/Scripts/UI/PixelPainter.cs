using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using AILand.Utils;

[RequireComponent(typeof(RawImage))]
public class PixelPainter : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Canvas Settings")]
    public int width  = 256;
    public int height = 256;
    public Color backgroundColor = Color.clear;
    public bool canPaint = true;

    [Header("Brush Settings")]
    public int   brushSize  = 4;
    public bool  isCirleBrush = true;
    public Color brushColor = Color.black;

    private Texture2D m_texPaint;
    private RawImage  m_rawImgPaint;
    private bool      m_isPainting;

    // 记录上一次绘制的像素位置
    private Vector2Int? m_lastPixel;

    void Awake()
    {
        m_rawImgPaint = GetComponent<RawImage>();
        InitCanvas();
    }

    private void InitCanvas()
    {
        m_texPaint = new Texture2D(width, height, TextureFormat.RGBA32, false);
        m_texPaint.filterMode = FilterMode.Point;
        ClearCanvas();
        m_rawImgPaint.texture = m_texPaint;
    }

    public void ClearCanvas()
    {
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = backgroundColor;
        }
        m_texPaint.SetPixels(colors);
        m_texPaint.Apply();
        
        m_lastPixel = null;
    }
    
    public void SetTexture(Texture2D tex)
    {
        if(tex == null)
        {
            Debug.LogError("Texture cannot be null");
            return;
        }
        
        width = tex.width;
        height = tex.height;
        
        m_texPaint = new Texture2D(tex.width, tex.height, tex.format, false);
        m_texPaint.filterMode = FilterMode.Point;
        m_texPaint = tex;
        m_texPaint.Apply();
        m_rawImgPaint.texture = m_texPaint;
    }
    
    public float[,] GetPaintMap()
    {
        return Util.GrayTexture2Array(m_texPaint);
    }

    public void OnPointerDown(PointerEventData e)
    {
        m_isPainting    = true;
        m_lastPixel   = null;
        OnPaint(e);
    }

    public void OnDrag(PointerEventData e)
    {
        if (m_isPainting)
        {
            OnPaint(e);
        }
            
    }

    public void OnPointerUp(PointerEventData e)
    {
        m_isPainting    = false;
        m_lastPixel   = null;
    }

    private void OnPaint(PointerEventData e)
    {
        if(!canPaint) return;
        
        // 屏幕坐标->画布坐标
        RectTransform rt = m_rawImgPaint.rectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt, e.position, e.pressEventCamera, out Vector2 local)) return;
        
        float nx = Mathf.InverseLerp(-rt.rect.width * 0.5f, rt.rect.width * 0.5f, local.x);
        float ny = Mathf.InverseLerp(-rt.rect.height * 0.5f, rt.rect.height * 0.5f, local.y);
        
        // 左下 (0,0)
        int px = Mathf.FloorToInt(nx * width);
        int py = Mathf.FloorToInt(ny * height);

        bool isErase = e.button == PointerEventData.InputButton.Right;
        var current = new Vector2Int(px, py);

        if (m_lastPixel.HasValue)
        {
            DrawLine(m_lastPixel.Value, current, isErase);
        }
        else
        {
            DrawPoint(px, py, isErase);
        }
        
        m_lastPixel = current;
        m_texPaint.Apply();
    }

    // 绘制点
    private void DrawPoint(int x, int y, bool isErase)
    {
        Color color = isErase ? backgroundColor : brushColor;
        int r2 = brushSize * brushSize;
        for (int dx = -brushSize; dx <= brushSize; dx++)
        {
            for (int dy = -brushSize; dy <= brushSize; dy++)
            {
                int sx = x + dx, sy = y + dy;
                if (sx < 0 || sx >= width || sy < 0 || sy >= height) continue;
                if (isCirleBrush && dx*dx + dy*dy > r2) continue;
                m_texPaint.SetPixel(sx, sy, color);
            }
        }
        
    }

    // Bresenham 绘制线
    private void DrawLine(Vector2Int a, Vector2Int b, bool isErase)
    {
        int x0 = a.x, y0 = a.y;
        int x1 = b.x, y1 = b.y;
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            DrawPoint(x0, y0, isErase);
            if (x0 == x1 && y0 == y1) break;
            int e2 = err * 2;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx)  { err += dx; y0 += sy; }
        }
    }
    
    
    
}