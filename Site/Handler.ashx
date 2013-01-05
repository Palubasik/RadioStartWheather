<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        int Width = 0;
        int Height = 0;
        context.Response.ContentType = "image/png";
        if (context.Request.Params["height"] != null)
        {


            try
            {
                Height = int.Parse(context.Request.Params["height"]);
            }
            catch
            {
                Height = 0;
            }
        }
        if (context.Request.Params["width"] != null)
        {
            try
            {
                Width = int.Parse(context.Request.Params["width"]);
            }
            catch
            {
                Width = 0;
            }
        }

        //        //context.Response.Clear();
        //        //context.Response.ContentType = "image/png";
        byte[] buffer = null;
        if (Width > 0 && Height > 0)
        {
            System.Drawing.Size sz = new System.Drawing.Size(Width, Height);
            buffer = RadioStart.WheatherGadgetProcess.GadgetWheatherProcessor.GetWheatherImageStream(sz);
        }
        else
            buffer = RadioStart.WheatherGadgetProcess.GadgetWheatherProcessor.GetWheatherImageStream();

        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        
        //context.Response.Write("Hello World");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}