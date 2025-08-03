using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AILand.Utils;

public enum PaintMode
{
    Draw,
    GrayMaskDraw, // 在已有遮罩的基础上绘制
}

[RequireComponent(typeof(RawImage))]
public class PixelPainter : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("PaintMode")]
    public PaintMode paintMode = PaintMode.Draw;

    [Header("Optional UI Bindings")]
    public Slider sliderBrush;       
    public Toggle toggleCircleBrush; 

    [Header("Cursor")]
    public Texture2D cursorTex;      
    public Vector2 cursorHotspot = new(16, 16);

    
    [Header("Canvas Settings")]
    public int width = 200;
    public int height = 200;
    public Color backgroundColor = Color.black;
    public bool canPaint = true;

    [Header("Brush Settings")]
    [Min(1)]
    public int brushSize = 4;
    public bool isCirleBrush = true;
    public Color brushColor = Color.white;

    [Header("GrayMaskDraw")]
    public Texture2D texGrayMask;


    
    Texture2D m_texPaint;
    RawImage m_rawImgPaint;
    bool m_isPainting;
    Vector2Int? m_lastPixel;

    public float[,] GetPaintMap() => Util.GrayTexture2Array(m_texPaint);

    
    void Awake()
    {
        m_rawImgPaint = GetComponent<RawImage>();
        InitCanvas();
        BindUI();
    }

    
    private void InitCanvas()
    {
        m_texPaint = new Texture2D(width, height, TextureFormat.RGBA32, false){ filterMode = FilterMode.Point };
        m_rawImgPaint.texture = m_texPaint;

        if (paintMode == PaintMode.GrayMaskDraw)
        {
            texGrayMask = texGrayMask ?? new Texture2D(width, height, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        }
        ClearCanvas();
    }


    private void BindUI()
    {
        if (sliderBrush)
        {
            sliderBrush.SetValueWithoutNotify(brushSize);
            sliderBrush.onValueChanged.AddListener(v =>
                brushSize = Mathf.Clamp(Mathf.RoundToInt(v),
                                        (int)sliderBrush.minValue,
                                        (int)sliderBrush.maxValue));
        }

        if (toggleCircleBrush)
        {
            toggleCircleBrush.SetIsOnWithoutNotify(isCirleBrush);
            toggleCircleBrush.onValueChanged.AddListener(v => isCirleBrush = v);
        }
    }

    public void ClearCanvas()
    {
        if (paintMode == PaintMode.GrayMaskDraw)
        {
            SetTexture(texGrayMask);
            m_lastPixel = null;
        }
        else
        {
            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++) colors[i] = backgroundColor;
            m_texPaint.SetPixels(colors);
            m_texPaint.Apply();
            m_lastPixel = null;
        }
        
    }

    public void SetTexture(Texture2D tex)
    {
        if (!tex) { Debug.LogError("Texture cannot be null"); return; }
        width = tex.width;
        height = tex.height;
        m_texPaint = new Texture2D(width, height, tex.format, false)
        { filterMode = FilterMode.Point };
        m_texPaint.SetPixels(tex.GetPixels());
        m_texPaint.Apply();
        m_rawImgPaint.texture = m_texPaint;
    }

    public void SetGrayMask(Texture2D tex)
    {
        if(paintMode != PaintMode.GrayMaskDraw)
        {
            Debug.LogError("Cannot set gray mask when paint mode is not GrayMaskDraw");
            return;
        }
        if (!tex) { Debug.LogError("Gray mask texture cannot be null"); return; }
        if (tex.width != width || tex.height != height)
        {
            Debug.LogError("Gray mask texture size must match the paint texture size");
            return;
        }
        texGrayMask = tex;
        ClearCanvas();
    }



    #region 接口实现
    public void OnPointerEnter(PointerEventData _)
    {
        if (cursorTex) Cursor.SetCursor(cursorTex, cursorHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData _)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerDown(PointerEventData e)
    {
        m_isPainting = true;
        m_lastPixel = null;
        OnPaint(e);
    }

    public void OnDrag(PointerEventData e)
    {
        if (m_isPainting) OnPaint(e);
    }

    public void OnPointerUp(PointerEventData _)
    {
        m_isPainting = false;
        m_lastPixel = null;
    }
    #endregion
    
    void OnPaint(PointerEventData e)
    {
        if (!canPaint) return;

        RectTransform rt = m_rawImgPaint.rectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt, e.position, e.pressEventCamera, out Vector2 local)) return;

        float nx = Mathf.InverseLerp(-rt.rect.width * 0.5f, rt.rect.width * 0.5f, local.x);
        float ny = Mathf.InverseLerp(-rt.rect.height * 0.5f, rt.rect.height * 0.5f, local.y);
        int px = Mathf.FloorToInt(nx * width);
        int py = Mathf.FloorToInt(ny * height);

        bool isErase = e.button == PointerEventData.InputButton.Right;
        Vector2Int curr = new(px, py);

        if (m_lastPixel.HasValue) DrawLine(m_lastPixel.Value, curr, isErase);
        else DrawPoint(px, py, isErase);

        m_lastPixel = curr;
        m_texPaint.Apply();
    }

    void DrawPoint(int x, int y, bool isErase)
    {
        Color color = isErase ? backgroundColor : brushColor;
        int r2 = brushSize * brushSize;
        for (int dx = -brushSize; dx <= brushSize; dx++)
        {
            for (int dy = -brushSize; dy <= brushSize; dy++)
            {
                int sx = x + dx, sy = y + dy;
                if (sx < 0 || sx >= width || sy < 0 || sy >= height) continue;
                if (isCirleBrush && dx * dx + dy * dy > r2) continue;
                if (paintMode == PaintMode.GrayMaskDraw)
                {
                    float gray = texGrayMask.GetPixel(sx, sy).grayscale;
                    Color outColor;
                    if (isErase)
                    {
                        outColor = texGrayMask.GetPixel(sx, sy);
                    }
                    else
                    {
                        Color brush = color;              
                        Color.RGBToHSV(brush, out float h, out float s, out float v);
                        outColor = Color.HSVToRGB(h, s, gray);
                        outColor.a = brush.a;                  
                    }
                    m_texPaint.SetPixel(sx, sy, outColor);
                }
                else
                {
                    m_texPaint.SetPixel(sx, sy, color);
                }
            }
        }
    }

    void DrawLine(Vector2Int a, Vector2Int b, bool isErase)
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
            int e2 = err << 1;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (sliderBrush)
        {
            brushSize = Mathf.Clamp(brushSize,
                                    (int)sliderBrush.minValue,
                                    (int)sliderBrush.maxValue);
            sliderBrush.SetValueWithoutNotify(brushSize);
        }

        if (toggleCircleBrush)
        {
            toggleCircleBrush.SetIsOnWithoutNotify(isCirleBrush);
        }
    }
#endif
}
