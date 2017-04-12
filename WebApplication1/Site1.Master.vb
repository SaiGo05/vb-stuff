Public Class Site1
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub lbtnGridView_Click(sender As Object, e As EventArgs)
        If Session("gridViewTickets") = False Then
            Session("gridViewTickets") = True
        Else
            Session("gridViewTickets") = False
        End If
        Response.Redirect("WebForm1.aspx")

    End Sub

End Class