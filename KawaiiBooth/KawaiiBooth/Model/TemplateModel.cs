using System;
public class TemplateModel
{
    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }
    public int PhotoCount { get; set; }
    public string BackgroundBase64 { get; set; }
    public List<LayerModel> Layers { get; set; }
    public List<PhotoSlotModel> PhotoSlots { get; set; }
}

public class LayerModel
{
    public string Base64Image { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

public class PhotoSlotModel
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}
