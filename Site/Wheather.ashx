<%@ WebHandler Language="C#" Class="Wheather" %>

using System;
using System.Web;
using System.Drawing;

public class Wheather : IHttpHandler
{
    int Width = 0;
    int Height = 0;
    public void ProcessRequest(System.Web.HttpContext context)
    {
        //if (context.Request.Params["height"] != null)
        //{


        //    try
        //    {
        //        Height = int.Parse(context.Request.Params["height"]);
        //    }
        //    catch
        //    {
        //        Height = 0;
        //    }

        //    if (context.Request.Params["width"] != null)
        //    {
        //        //try
        //        //{
        //        //    Width = int.Parse(context.Request.Params["width"]);
        //        //}
        //        //catch
        //        //{
        //        //    Width = 0;
        //        //}

        //        //context.Response.Clear();
        //        //context.Response.ContentType = "image/png";
        //        //byte[] buffer = null;
        //        //if (Width > 0 && Height > 0)
        //        //{
        //        //    Size sz = new Size(Width, Height);
        //        //    buffer = GadgetWheatherProcessor.GetWheatherImageStream(sz);
        //        //}
        //        //else
        //        //    buffer = GadgetWheatherProcessor.GetWheatherImageStream();
        //        //char[] chars = System.Text.Encoding.Default.GetChars(buffer);
        //        //context.Response.Write(chars, 0, chars.Length);
        //        context.Response.End();
        //    }
        //}
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}

