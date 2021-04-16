﻿Imports System.Web.Optimization
Imports System.Web.Routing

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Fires when the application is started
        RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles(BundleTable.Bundles)
        DevExpress.XtraScheduler.SchedulerCompatibility.Base64XmlObjectSerialization = False
    End Sub
    Sub Application_BeginRequest(sender As Object, e As EventArgs)
        Dim uriObject As String = HttpContext.Current.Request.Url.OriginalString
        'app.Context.Request.Url.OriginalString
        If uriObject.ToString.Contains("/learnsco") Or uriObject.ToString.Contains("/sco") Or uriObject.ToString.Contains("/CMSContent") Or uriObject.ToString.Contains("/eitslm") Or uriObject.ToString.Contains("/finalise") Or uriObject.ToString.Contains("/errorAccess") Then
            Me.Response.Headers("X-FRAME-OPTIONS") = "ALLOWALL"
        Else
            Me.Response.Headers("X-FRAME-OPTIONS") = "ALLOW-FROM https://localhost:44367/"
            Me.Response.Headers("X-FRAME-OPTIONS") = "ALLOW-FROM https://localhost:44363/"
            Me.Response.Headers("X-FRAME-OPTIONS") = "ALLOW-FROM https://hee-dls-test.softwire.com/"
        End If
    End Sub
    Private Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        Response.Cookies("ASP.NET_SessionId").SameSite = SameSiteMode.Lax
        If Request.IsSecureConnection Then Response.Cookies("ASP.NET_SessionId").Secure = True
    End Sub
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        Try
            '
            ' Send an error email first as that has a good chance of working
            '
            Dim exp As System.Exception = Server.GetLastError
            '
            ' Check if the exception has been packaged in another one
            '
            If TypeOf HttpContext.Current.Error Is HttpUnhandledException Then
                exp = exp.GetBaseException      ' grab the original exception
            End If
            '
            ' Throw away any "remote host closed the connection" errors.
            ' We're not concerned about those.
            '
            If exp.Message().Contains("0x800704CD") Or exp.Message().Contains("0x80070057") Then
                Return
            End If
            '
            ' Send the error email
            '
            CCommon.LogErrorToEmail(exp)
            '
            ' Also log to the database. This might not work if the database isn't functioning,
            ' so do it second.
            '
            CCommon.LogErrorToDatabase(2, 0, exp.ToString, HttpContext.Current.Request)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Application_PreRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)
        DevExpress.Web.ASPxWebControl.GlobalAccessibilityCompliant = True
    End Sub
End Class