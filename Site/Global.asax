<%@ Application Language="C#" %>

<script runat="server">
    
    RadioStart.WheatherGadgetProcess.WheatherGadgetData gadgetData = null;
    RadioStart.WheatherGadgetProcess.GadgetWheatherProcessor processor = null;
    void Application_Start(object sender, EventArgs e) 
    {
       // System.Diagnostics.Debugger.Break();

        gadgetData = RadioStart.WheatherGadgetProcess.ConfigSerializer.Deserialize<RadioStart.WheatherGadgetProcess.WheatherGadgetData>(
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"config.xml"));
        processor = new RadioStart.WheatherGadgetProcess.GadgetWheatherProcessor(gadgetData);
        processor.StartProcess();
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        processor.StopProcess();
        //  Code that runs on application shutdown
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
