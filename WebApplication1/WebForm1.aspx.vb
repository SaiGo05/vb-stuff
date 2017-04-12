Imports System.Data.OleDb
Imports System.Web
Imports System.Net.Mail
Imports System.IO
Imports System.Data.SqlClient
Public Class WebForm1
    Inherits System.Web.UI.Page
    Dim strUser As String = ""
    Dim ChangeLast As DateTime = Now
    Dim strAccess As String = ""
    Dim strSQL As String = ""
    Dim watchTI As String = ""
    Dim favRoleCarry As String = ""
    Dim favRoleMid As String = ""
    Dim favRoleOff As String = ""
    Dim subNewsletter As String = ""
    Dim favTI As String = ""
    Dim steamID As String = ""
    Dim strEmailSubject As String = "Test – CSC470"
    Dim strPath As String = System.AppDomain.CurrentDomain.BaseDirectory()
    Dim strUserName As String = Environment.UserName
    Dim path As String = ""
    Dim strLine As String = String.Empty
    Dim strFileName As String = "tickets.txt"
    Dim strTicketNumber As String = ""
    Dim strVehicleType As String = ""
    Dim strVehicleMake As String = ""
    Dim strLicensePlate As String = ""
    Dim strLicenseState As String = ""
    Dim strSlashZero As String = ""
    Dim strBlockNumber As String = ""
    Dim strStreet As String = ""
    Dim strTicketDate As String = ""
    Dim strTicketTime As String = ""
    Dim strMeterLocation As String = ""
    Dim strIssuedBy As String = ""
    Dim strUnknown1 As String = ""
    Dim strOriginalFine As String = ""
    Dim strFourteenDayFine As String = ""
    Dim strTicketDay As String = ""
    Dim strUnknown2 As String = ""
    Dim strViolationCode As String = ""
    Dim strViolationType As String = ""
    Dim strUnknown3 As String = ""
    Dim strUnknown4 As String = ""
    Dim strUnknown5 As String = ""
    Dim strUnknown6 As String = ""
    Dim strUnknown7 As String = ""
    Dim strUnknown8 As String = ""
    Dim strPostedLimit As String = ""
    Dim strRestOfString As String = ""
    Dim dblOriginalFineSum As Double = 0.0
    Dim dblOriginalFineSum2 As Double = 0.0
    Dim myReader As SqlDataReader
    Dim myAdapter As SqlDataAdapter
    Dim myDataset As DataSet


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strUser As String = String.Empty
        strUser = Environment.UserName
        Session("UserName") = strUser
        If strUser.Length() < 1 Then
            Response.Redirect("Error.aspx")
        End If

        If Not Page.IsPostBack Then
            ObtainEvents()
            ObtainValues()
        End If
        ReadTextFile()
        ReadTextSplit()


        'Creating a table on pageload:
        'CreateTable("Tickets")

        'Inserting into the created table on pageload:
        'InsertSQL()

        'Dropping the table created
        'DropTable("Tickets")

        If Session("gridViewTickets") = False Then
            ticketsGV.Visible = False
        Else
            ticketsGV.Visible = True
            ticketsGridBind()
        End If

    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        'Radio button 1
        If RadioButtonList1.SelectedValue = "Yes" Then
            watchTI = "Yes"
        ElseIf RadioButtonList1.SelectedValue = "No" Then
            watchTI = "No"
        Else
            watchTI = "Unselected"
        End If

        'Radio button 2
        If RadioButtonList2.SelectedValue = "Yes" Then
            subNewsletter = "Yes"
        ElseIf RadioButtonList2.SelectedValue = "No" Then
            subNewsletter = "No"
        Else
            subNewsletter = "Unselected"
        End If

        'Checkbox
        If CheckBoxList1.Items(0).Selected Then
            favRoleCarry = "Yes"
        Else
            favRoleCarry = "No"
        End If
        If CheckBoxList1.Items(1).Selected Then
            favRoleMid = "Yes"
        Else
            favRoleMid = "No"
        End If
        If CheckBoxList1.Items(2).Selected Then
            favRoleOff = "Yes"
        Else
            favRoleOff = "No"
        End If

        'Textbox
        steamID = TextBox1.Text

        'Dropdown
        'favTI = DropDownList1.SelectedItem.Text
        If DropDownList1.SelectedItem.Text = "Select" Then
            favTI = "Unselected"
        Else
            favTI = DropDownList1.SelectedItem.Text
        End If

        'Connecting to access
        Dim strPath As String = System.AppDomain.CurrentDomain.BaseDirectory()

        Session("path") = strPath
        strPath = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + strPath + "Database1.accdb"
        Session("access") = strPath

        'Piping and stuff
        Dim myConnection As OleDbConnection
        Dim myCommand As OleDbCommand
        Dim strAccess As String = ""
        Dim strSQL As String = ""
        strUser = Environment.UserName
        strAccess = Session("access")
        myConnection = New OleDbConnection(strAccess)
        strSQL = "insert into UserValues(watchInternational, favRoleCarry, favRoleMid, favRoleOff, subNewsletter, favTi, steamID, UserName, DateTimeChangeLast)"
        strSQL += "values('" + watchTI + "','" + favRoleCarry + "','" + favRoleMid + "','" + favRoleOff + "','" + subNewsletter + "','" + favTI + "','" + steamID + "','" + strUser + "','" + ChangeLast + "')"
        myCommand = New OleDbCommand(strSQL, myConnection)

        'email
        strEmailSubject = "Insert statement"
        CreateEmail()

        myCommand.Connection.Open()
        myCommand.ExecuteNonQuery()
        myCommand.Dispose()
        myConnection.Dispose()
    End Sub

    Public Sub ObtainEvents()

        Dim myConnection As OleDbConnection
        Dim myCommand As OleDbCommand
        Dim myReader As OleDbDataReader

        Dim strEventName As String = ""

        Dim strPath As String = System.AppDomain.CurrentDomain.BaseDirectory()

        Session("path") = strPath
        strPath = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + strPath + "Database1.accdb"
        Session("access") = strPath
        strAccess = Session("access")
        myConnection = New OleDbConnection(strAccess)
        strSQL = "select * from Events order by EventName asc"

        myCommand = New OleDbCommand(strSQL, myConnection)
        myCommand.Connection.Open()

        myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)

        DropDownList1.Items.Clear()
        DropDownList1.Items.Add("Select")
        While (myReader.Read())
            strEventName = Trim("" + myReader("EventName"))
            DropDownList1.Items.Add(strEventName)
        End While

        myReader.Close()
        myCommand.Dispose()
        myConnection.Dispose()
    End Sub

    Public Sub ObtainValues()

        Dim myConnection As OleDbConnection
        Dim myCommand As OleDbCommand
        Dim myReader As OleDbDataReader

        Dim strUserName As String = ""
        strUserName = Session("userName")

        Dim strPath As String = System.AppDomain.CurrentDomain.BaseDirectory()

        Session("path") = strPath
        strPath = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + strPath + "Database1.accdb"
        Session("access") = strPath
        strAccess = Session("access")
        myConnection = New OleDbConnection(strAccess)
        strSQL = "select top 1 * from UserValues where UserName = '" + strUserName + "' order by DateTimeChangeLast desc"

        myCommand = New OleDbCommand()
        myCommand.CommandText = strSQL
        myCommand.CommandType = CommandType.Text
        myCommand.Connection = myConnection
        myCommand.Connection.Open()
        myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection)



        While (myReader.Read())
            watchTI = Trim("" + myReader("watchInternational"))
            favRoleCarry = Trim("" + myReader("favRoleCarry"))
            favRoleMid = Trim("" + myReader("favRoleMid"))
            favRoleOff = Trim("" + myReader("favRoleOff"))
            subNewsletter = Trim("" + myReader("subNewsletter"))
            favTI = Trim("" + myReader("favTi"))
            steamID = Trim("" + myReader("steamID"))

        End While

        myReader.Close()
        myCommand.Dispose()
        myConnection.Dispose()

        If watchTI = "Yes" Then
            RadioButtonList1.SelectedValue = "Yes"

        ElseIf watchTI = "No" Then
            RadioButtonList1.SelectedValue = "No"

        End If

        If favRoleCarry = "Yes" Then
            CheckBoxList1.Items(0).Selected = True
        ElseIf CheckBoxList1.Items(0).Selected = False Then
        End If

        If favRoleMid = "Yes" Then
            CheckBoxList1.Items(1).Selected = True
        ElseIf CheckBoxList1.Items(1).Selected = False Then
        End If

        If favRoleOff = "Yes" Then
            CheckBoxList1.Items(2).Selected = True
        ElseIf CheckBoxList1.Items(2).Selected = False Then
        End If

        If subNewsletter = "Yes" Then
            RadioButtonList2.SelectedValue = "Yes"

        ElseIf subNewsletter = "No" Then
            RadioButtonList2.SelectedValue = "No"

        End If

        Dim lstItems As Integer = 0
        lstItems = DropDownList1.Items.Count - 1

        While lstItems >= 1

            If favTI = DropDownList1.Items(lstItems).Text Then
                DropDownList1.Items(lstItems).Selected = True
            End If
            lstItems = lstItems - 1

        End While
        'DropDownList1.SelectedValue = favTI

        If steamID.ToString = "Not Entered" Then

            TextBox1.Text = ""

        Else TextBox1.Text = steamID

        End If


    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        DropDownList1.ClearSelection()
        CheckBoxList1.ClearSelection()
        'CheckBoxList1.Items(1).Selected = False
        'CheckBoxList1.Items(2).Selected = False
        TextBox1.Text = ""
        RadioButtonList1.ClearSelection()
        RadioButtonList2.ClearSelection()
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        'Radio button 1
        If RadioButtonList1.SelectedValue = "Yes" Then
            watchTI = "Yes"
        ElseIf RadioButtonList1.SelectedValue = "No" Then
            watchTI = "No"
        Else
            watchTI = "Unselected"
        End If

        'Radio button 2
        If RadioButtonList2.SelectedValue = "Yes" Then
            subNewsletter = "Yes"
        ElseIf RadioButtonList2.SelectedValue = "No" Then
            subNewsletter = "No"
        Else
            subNewsletter = "Unselected"
        End If

        'Checkbox
        If CheckBoxList1.Items(0).Selected Then
            favRoleCarry = "Yes"
        Else
            favRoleCarry = "No"
        End If
        If CheckBoxList1.Items(1).Selected Then
            favRoleMid = "Yes"
        Else
            favRoleMid = "No"
        End If
        If CheckBoxList1.Items(2).Selected Then
            favRoleOff = "Yes"
        Else
            favRoleOff = "No"
        End If

        'Textbox
        steamID = TextBox1.Text

        'Dropdown
        'favTI = DropDownList1.SelectedItem.Text
        If DropDownList1.SelectedItem.Text = "Select" Then
            favTI = "Unselected"
        Else
            favTI = DropDownList1.SelectedItem.Text
        End If

        'Connecting to access
        Dim strPath As String = System.AppDomain.CurrentDomain.BaseDirectory()

        Session("path") = strPath
        strPath = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + strPath + "Database1.accdb"
        Session("access") = strPath

        'Piping and stuff
        Dim myConnection As OleDbConnection
        Dim myCommand As OleDbCommand
        Dim strAccess As String = ""
        Dim strSQL As String = ""
        strUser = Environment.UserName
        strAccess = Session("access")
        myConnection = New OleDbConnection(strAccess)

        strSQL = "update UserValues "
        strSQL += "watchInternational = '" + watchTI + "', "
        strSQL += "favRoleCarry = '" + favRoleCarry + "', "
        strSQL += "favRoleMid = '" + favRoleMid + "', "
        strSQL += "favRoleOff = '" + favRoleOff + "', "
        strSQL += "subNewsletter = '" + subNewsletter + "', "
        strSQL += "favTi = '" + favTI + "', "
        strSQL += "steamID = '" + steamID + "', "
        strSQL += "DateTimeChangeLast =  #" + ChangeLast + "#"
        strSQL += " where UserName = '" + strUser + "' "

        strEmailSubject = "Update statement"
        CreateEmail()
        ' = "update UserValues(watchInternational, favRoleCarry, favRoleMid, favRoleOff, subNewsletter, favTi, steamID, UserName, DateTimeChangeLast)"
        'strSQL += "values('" + watchTI + "','" + favRoleCarry + "','" + favRoleMid + "','" + favRoleOff + "','" + subNewsletter + "','" + favTI + "','" + steamID + "','" + strUser + "','" + ChangeLast + "')"
        myCommand = New OleDbCommand(strSQL, myConnection)
        myCommand.Connection.Open()
        myCommand.ExecuteNonQuery()
        myCommand.Dispose()
        myConnection.Dispose()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        'Connecting to access
        Dim strPath As String = System.AppDomain.CurrentDomain.BaseDirectory()

        Session("path") = strPath
        strPath = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + strPath + "Database1.accdb"
        Session("access") = strPath

        'Piping and stuff
        Dim myConnection As OleDbConnection
        Dim myCommand As OleDbCommand
        Dim strAccess As String = ""
        Dim strSQL As String = ""
        strUser = Environment.UserName
        strAccess = Session("access")
        myConnection = New OleDbConnection(strAccess)

        strSQL = "delete from UserValues "
        'strSQL += "watchInternational = '" + watchTI + "', "
        'strSQL += "favRoleCarry = '" + favRoleCarry + "', "
        'strSQL += "favRoleMid = '" + favRoleMid + "', "
        'strSQL += "favRoleOff = '" + favRoleOff + "', "
        'strSQL += "subNewsletter = '" + subNewsletter + "', "
        'strSQL += "favTi = '" + favTI + "', "
        'strSQL += "steamID = '" + steamID + "', "
        'strSQL += "DateTimeChangeLast =  #" + ChangeLast + "#"
        strSQL += " where UserName = '" + strUser + "' "
        strEmailSubject = "Delete statement"
        CreateEmailDel()
        ' = "update UserValues(watchInternational, favRoleCarry, favRoleMid, favRoleOff, subNewsletter, favTi, steamID, UserName, DateTimeChangeLast)"
        'strSQL += "values('" + watchTI + "','" + favRoleCarry + "','" + favRoleMid + "','" + favRoleOff + "','" + subNewsletter + "','" + favTI + "','" + steamID + "','" + strUser + "','" + ChangeLast + "')"
        myCommand = New OleDbCommand(strSQL, myConnection)
        myCommand.Connection.Open()
        myCommand.ExecuteNonQuery()
        myCommand.Dispose()
        myConnection.Dispose()

        btnClear_Click(Nothing, Nothing)
        'CreateEmail()
    End Sub


    Public Sub CreateEmail()
        Dim strMailFrom As String = "sregu2@uis.edu"
        Dim strMailTo As String = "rjack01s@uis.edu"
        Dim strEmailName As String = "rjack01s@uis.edu"

        Dim mail As New MailMessage(strMailFrom, strMailTo)
        'mail.To.Add(strEmailName)

        mail.Subject = strEmailSubject
        mail.IsBodyHtml = True

        mail.Body = ""
        mail.Body += strEmailSubject
        mail.Body += "<br /><br />"

        mail.Body += "<span style='font-family:Verdana; font-size: 24px'>"
        mail.Body += "Did you watch a TI?: "
        mail.Body += "<span style='color: Red;'>"
        If RadioButtonList1.SelectedValue = "Yes" Then
            mail.Body += "Yes"
        ElseIf RadioButtonList1.SelectedValue = "No" Then
            mail.Body += "No"
        Else
            mail.Body += "No button was selected"
        End If
        mail.Body += "</span>"
        mail.Body += "<br /><br />"

        '
        mail.Body += "<span style='font-family:Verdana; font-size: 24px'>"
        mail.Body += "Sub to the Newsletter?: "
        mail.Body += "<span style='color: Red;'>"
        If RadioButtonList2.SelectedValue = "Yes" Then
            mail.Body += "Yes"
        ElseIf RadioButtonList2.SelectedValue = "No" Then
            mail.Body += "No"
        Else
            mail.Body += "No button was selected"
        End If
        mail.Body += "</span>"
        mail.Body += "<br /><br />"

        '
        mail.Body += "<span style='font-family:Verdana; font-size: 24px'>"
        mail.Body += "What roles do you play?: "
        mail.Body += "<span style='color: Blue;'>"
        If CheckBoxList1.Items(0).Selected Then
            mail.Body += " Carry "
        End If
        If CheckBoxList1.Items(1).Selected Then
            mail.Body += " Mid "
        End If
        If CheckBoxList1.Items(2).Selected Then
            mail.Body += "Offlane"
        End If
        mail.Body += "</span>"
        mail.Body += "<br /><br />"

        mail.Body += "<span style='font-family:Verdana; font-size: 24px'>"
        mail.Body += "What is your favourite TI?: "
        mail.Body += "<span style='color: Green;'>"
        If DropDownList1.SelectedItem.Text = "Select" Then
            mail.Body += "Not Selected"
        Else
            mail.Body += DropDownList1.SelectedItem.Text
        End If
        mail.Body += "</span>"
        mail.Body += "<br /><br />"
        '

        mail.Body += "<span style='font-family:Verdana; font-size: 24px'>"
        mail.Body += "What is your Steam ID?: "
        mail.Body += "<span style='color: Green;'>"
        If steamID.Length = 0 Then
            mail.Body += "Not entered"
        Else
            mail.Body += steamID
        End If
        mail.Body += "</span>"
        mail.Body += "<br /><br />"


        'testLbl.Text = mail.ToString

        Dim mySmtp As New SmtpClient
        mySmtp.Host = "webmail.uis.edu"
        mySmtp.Send(mail)
    End Sub

    Public Sub ReadTextFile()

        Session(path) = strPath


        Try
            If File.Exists(strPath + strFileName) Then
                ' the file exists
            Else
                ' the file does not exist
                Exit Sub
            End If
        Catch ex As Exception
            Dim e As String = ex.ToString
        End Try

        Dim sr As StreamReader = New StreamReader(strPath + strFileName)
        'Dim strLine As String = String.Empty
        Do
            strLine = sr.ReadLine()
            'Loop Until strLine Is Nothing
            If strLine Is Nothing Then Exit Do
            ' close the streamreader
            'sr.Close()

            Dim found As Integer = 0

            found = InStr(strLine, ";")


            'If found > 0 Then
            '    strTicketNumber = Left(strLine, found - 1)
            'End If

            strRestOfString = strLine
            Dim variableCounter As Integer = 1
            If strRestOfString Is Nothing Then
                strRestOfString = ""
            End If
            Do While strRestOfString.Length > 0
                found = InStr(strRestOfString, ";")
                If found > 0 Then

                    Select Case variableCounter
                        Case 1
                            strTicketNumber = Left(strRestOfString, found - 1)
                        Case 2
                            strVehicleType = Left(strRestOfString, found - 1)
                        Case 3
                            strVehicleMake = Left(strRestOfString, found - 1)
                        Case 4
                            strLicensePlate = Left(strRestOfString, found - 1)
                        Case 5
                            strLicenseState = Left(strRestOfString, found - 1)
                        Case 6
                            strSlashZero = Left(strRestOfString, found - 1)
                        Case 7
                            strBlockNumber = Left(strRestOfString, found - 1)
                        Case 8
                            strStreet = Left(strRestOfString, found - 1)
                        Case 9
                            strTicketDate = Left(strRestOfString, found - 1)
                        Case 10
                            strTicketTime = Left(strRestOfString, found - 1)
                        Case 11
                            strMeterLocation = Left(strRestOfString, found - 1)
                        Case 12
                            strIssuedBy = Left(strRestOfString, found - 1)
                        Case 13
                            strUnknown1 = Left(strRestOfString, found - 1)
                        Case 14
                            strOriginalFine = Left(strRestOfString, found - 1)
                        Case 15
                            strFourteenDayFine = Left(strRestOfString, found - 1)
                        Case 16
                            strTicketDay = Left(strRestOfString, found - 1)
                        Case 17
                            strUnknown2 = Left(strRestOfString, found - 1)
                        Case 18
                            strViolationCode = Left(strRestOfString, found - 1)
                        Case 19
                            strViolationType = Left(strRestOfString, found - 1)
                        Case 20
                            strUnknown3 = Left(strRestOfString, found - 1)
                        Case 21
                            strUnknown4 = Left(strRestOfString, found - 1)
                        Case 22
                            strUnknown5 = Left(strRestOfString, found - 1)
                        Case 23
                            strUnknown6 = Left(strRestOfString, found - 1)
                        Case 24
                            strUnknown7 = Left(strRestOfString, found - 1)
                        Case 25
                            strUnknown8 = Left(strRestOfString, found - 1)
                        Case 26
                            strPostedLimit = Left(strRestOfString, found - 1)
                    End Select

                    strRestOfString = Mid(strRestOfString, found + 1, Len(strLine) - found)
                    variableCounter += 1
                End If
            Loop
            dblOriginalFineSum += Convert.ToDouble(strOriginalFine)
            WriteToFile(strTicketNumber + " " + strOriginalFine + " " + dblOriginalFineSum.ToString())

            'WriteToFile(strTicketNumber + " " + strVehicleType + " " + strVehicleMake + " " + strLicensePlate + " " + strLicenseState + " " + strTicketDate + " " + strTicketTime + " " + strMeterLocation + " " + strIssuedBy + " " + strPostedLimit + " ")

        Loop Until strLine Is Nothing
        sr.Close()

    End Sub

    Public Sub WriteToFile(ByVal strOutputLine As String)
        Dim strPath As String = Session("path")
        Using sw As StreamWriter = New StreamWriter(strPath + "output.txt", True)
            sw.WriteLine(strOutputLine)
        End Using
    End Sub

    Public Sub WriteToFile2(ByVal strOutputLine As String)
        Dim strPath As String = Session("path")
        Using sw As StreamWriter = New StreamWriter(strPath + "outputsplit.txt", True)
            sw.WriteLine(strOutputLine)
        End Using
    End Sub

    Public Sub CreateEmailDel()
        Dim strMailFrom As String = "sregu2@uis.edu"
        Dim strMailTo As String = "rjack01s@uis.edu"
        Dim strEmailName As String = "rjack01s@uis.edu"

        Dim mail As New MailMessage(strMailFrom, strMailTo)
        'mail.To.Add(strEmailName)

        mail.Subject = strEmailSubject
        mail.IsBodyHtml = True

        mail.Body = ""
        mail.Body += strEmailSubject
        mail.Body += "<br /><br />"

        mail.Body += "<span style='font-family:Verdana; font-size: 24px'>"
        mail.Body += "Deleted record "
        mail.Body += "<br /><br />"

        Dim mySmtp As New SmtpClient
        mySmtp.Host = "webmail.uis.edu"
        mySmtp.Send(mail)
    End Sub

    Public Sub ReadTextSplit()
        Session(path) = strPath
        Dim splitString(25) As String

        Dim sr As StreamReader = New StreamReader(strPath + strFileName)
        Dim strLine As String = String.Empty
        Do
            strLine = sr.ReadLine()

            If strLine Is Nothing Then Exit Do


            Dim found As Integer = 0
            strRestOfString = strLine
            splitString = strRestOfString.Split(";")
            Dim variableCounter As Integer = 1
            If variableCounter < splitString.Count Then

                Select Case variableCounter
                    Case 1
                        strTicketNumber = splitString(0)
                    Case 2
                        strVehicleType = splitString(1)
                    Case 3
                        strVehicleMake = splitString(2)
                    Case 4
                        strLicensePlate = splitString(3)
                    Case 5
                        strLicenseState = splitString(4)
                    Case 6
                        strSlashZero = splitString(5)
                    Case 7
                        strBlockNumber = splitString(6)
                    Case 8
                        strStreet = splitString(7)
                    Case 9
                        strTicketDate = splitString(8)
                    Case 10
                        strTicketTime = splitString(9)
                    Case 11
                        strMeterLocation = splitString(10)
                    Case 12
                        strIssuedBy = splitString(11)
                    Case 13
                        strUnknown1 = splitString(12)
                    Case 14
                        strOriginalFine = splitString(13)
                    Case 15
                        strFourteenDayFine = splitString(14)
                    Case 16
                        strTicketDay = splitString(15)
                    Case 17
                        strUnknown2 = splitString(16)
                    Case 18
                        strViolationCode = splitString(17)
                    Case 19
                        strViolationType = splitString(18)
                    Case 20
                        strUnknown3 = splitString(19)
                    Case 21
                        strUnknown4 = splitString(20)
                    Case 22
                        strUnknown5 = splitString(21)
                    Case 23
                        strUnknown6 = splitString(22)
                    Case 24
                        strUnknown7 = splitString(23)
                    Case 25
                        strUnknown8 = splitString(24)
                    Case 26
                        strPostedLimit = splitString(25)
                        variableCounter += 1
                End Select
                dblOriginalFineSum2 += Convert.ToDouble(strOriginalFine)
                WriteToFile2(strTicketNumber + " " + strOriginalFine + " " + dblOriginalFineSum2.ToString())
            End If
        Loop
    End Sub

    Public Sub CreateTable(ByVal strTableName As String)
        Dim myConnection As SqlConnection = New SqlConnection
        Dim myCommand As SqlCommand = New SqlCommand
        Dim strSql As String
        myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConn").ConnectionString
        strSql = String.Empty
        strSql += "CREATE TABLE " + strTableName + "("
        strSql += "TicketNumber varchar(50) not null, "
        strSql += "VehicleType varchar(50) not null, "
        strSql += "VehicleMake varchar(50) null, "
        strSql += "LicensePlate varchar(50) null, "
        strSql += "LicenseState varchar(50) null, "
        strSql += "SlashZero varchar(50) null, "
        strSql += "BlockNumber varchar(50) null, "
        strSql += "Street varchar(50) null, "
        strSql += "TicketDate varchar(50) null, "
        strSql += "TicketTime varchar(50) null, "
        strSql += "MeterLocation varchar(50) null, "
        strSql += "IssuedBy varchar(50) null, "
        strSql += "Unknown1 varchar(50) null, "
        strSql += "OriginalFine varchar(50) null, "
        strSql += "FourteenDayFine varchar(50) null, "
        strSql += "TicketDay varchar(50) null, "
        strSql += "Unknown2 varchar(50) null, "
        strSql += "ViolationCode varchar(50) null, "
        strSql += "ViolationType varchar(50) null, "
        strSql += "Unknown3 varchar(50) null, "
        strSql += "Unknown4 varchar(50) null, "
        strSql += "Unknown5 varchar(50) null, "
        strSql += "Unknown6 varchar(50) null, "
        strSql += "Unknown7 varchar(50) null, "
        strSql += "Unknown8 varchar(50) null, "
        strSql += "PostedLimit varchar(50) null, "
        strSql += "primary key(TicketNumber))"
        myCommand.CommandText = strSql
            myCommand.CommandType = CommandType.Text
            myCommand.Connection = myConnection
            myConnection.Open()
            myCommand.ExecuteNonQuery()
            myConnection.Close()
            myCommand.Dispose()
            myConnection.Dispose()

    End Sub

    Public Sub DropTable(ByVal strTableName As String)
        Dim myConnection As SqlConnection = New SqlConnection
        Dim myCommand As SqlCommand = New SqlCommand
        Dim strSql As String = String.Empty
        myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConn").ConnectionString
        strSql = "drop table " + strTableName
            myCommand.CommandText = strSql
            myCommand.CommandType = CommandType.Text
            myCommand.Connection = myConnection
            myConnection.Open()
            myCommand.ExecuteNonQuery()
            myConnection.Close()
            myCommand.Dispose()
        myConnection.Dispose()
    End Sub

    Public Sub InsertSQL()
        Dim myConnection As SqlConnection = New SqlConnection
        Dim myCommand As SqlCommand = New SqlCommand
        Dim strSql As String = String.Empty
        Dim strFileName As String = "Tickets.txt"
        Dim filePath As String = System.AppDomain.CurrentDomain.BaseDirectory()
        Dim SplitFile(25) As String
        Dim I As Integer
        Dim fileParse As StreamReader = New StreamReader(filePath + strFileName)
        Do
            strLine = fileParse.ReadLine()
            strRestOfString = strLine
            If strRestOfString <> Nothing Then
                SplitFile = strRestOfString.Split(";")
                For I = 0 To SplitFile.Count Step 1
                    Select Case I
                        Case 0
                            strTicketNumber = SplitFile(0)
                        Case 1
                            strVehicleType = SplitFile(1)
                        Case 2
                            strVehicleMake = SplitFile(2)
                        Case 3
                            strLicensePlate = SplitFile(3)
                        Case 4
                            strLicenseState = SplitFile(4)
                        Case 5
                            strSlashZero = SplitFile(5)
                        Case 6
                            strBlockNumber = SplitFile(6)
                        Case 7
                            strStreet = SplitFile(7)
                        Case 8
                            strTicketDate = SplitFile(8)
                        Case 9
                            strTicketTime = SplitFile(9)
                        Case 10
                            strMeterLocation = SplitFile(10)
                        Case 11
                            strIssuedBy = SplitFile(11)
                        Case 12
                            strUnknown1 = SplitFile(12)
                        Case 13
                            strOriginalFine = SplitFile(13)
                        Case 14
                            strFourteenDayFine = SplitFile(14)
                        Case 15
                            strTicketDay = SplitFile(15)
                        Case 16
                            strUnknown2 = SplitFile(16)
                        Case 17
                            strViolationCode = SplitFile(17)
                        Case 18
                            strViolationType = SplitFile(18)
                        Case 19
                            strUnknown3 = SplitFile(19)
                        Case 20
                            strUnknown4 = SplitFile(20)
                        Case 21
                            strUnknown5 = SplitFile(21)
                        Case 22
                            strUnknown6 = SplitFile(22)
                        Case 23
                            strUnknown7 = SplitFile(23)
                        Case 24
                            strUnknown8 = SplitFile(24)
                        Case 25
                            strPostedLimit = SplitFile(25)
                    End Select
                Next
                strSql = "insert into Tickets(TicketNumber,VehicleType,VehicleMake,LicensePlate,LicenseState,"
                strSql += "SlashZero,BlockNumber,Street,TicketDate,TicketTime,MeterLocation,IssuedBy,"
                strSql += "Unknown1,OriginalFine,FourteenDayFine,TicketDay,Unknown2,ViolationCode,"
                strSql += "ViolationType,Unknown3,Unknown4,Unknown5,Unknown6,Unknown7,Unknown8,PostedLimit)"
                strSql += "values('" + strTicketNumber + "',"
                strSql += " '" + strVehicleType + "',"
                strSql += "'" + strVehicleMake + "',"
                strSql += "'" + strLicensePlate + "',"
                strSql += "'" + strLicenseState + "',"
                strSql += "'" + strSlashZero + "',"
                strSql += "'" + strBlockNumber + "',"
                strSql += "'" + strStreet + "',"
                strSql += "'" + strTicketDate + "',"
                strSql += "'" + strTicketTime + "',"
                strSql += "'" + strMeterLocation + "',"
                strSql += "'" + strIssuedBy + "',"
                strSql += "'" + strUnknown1 + "',"
                strSql += "'" + strOriginalFine + "',"
                strSql += "'" + strFourteenDayFine + "',"
                strSql += "'" + strTicketDay + "',"
                strSql += "'" + strUnknown2 + "',"
                strSql += "'" + strViolationCode + "',"
                strSql += "'" + strViolationType + "',"
                strSql += "'" + strUnknown3 + "',"
                strSql += "'" + strUnknown4 + "',"
                strSql += "'" + strUnknown5 + "',"
                strSql += "'" + strUnknown6 + "',"
                strSql += "'" + strUnknown7 + "',"
                strSql += "'" + strUnknown8 + "',"
                strSql += "'" + strPostedLimit + "')"
                myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConn").ConnectionString
                myCommand.CommandText = strSql
                myCommand.CommandType = CommandType.Text
                myCommand.Connection = myConnection
                myConnection.Open()
                myCommand.ExecuteNonQuery()
                myConnection.Close()
                myCommand.Dispose()
                myConnection.Dispose()
            End If
        Loop Until strLine Is Nothing
        fileParse.Close()
    End Sub


    Protected Sub ticketsGV_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        ticketsGV.PageIndex = e.NewPageIndex
        Me.ticketsGridBind()
    End Sub

    Public Sub ticketsGridBind()
        Dim myConnection As SqlConnection = New SqlConnection
        Dim myCommand As SqlCommand = New SqlCommand

        myConnection = New SqlConnection()
        myCommand = New SqlCommand()
        myConnection.ConnectionString = ConfigurationManager.ConnectionStrings("mySQLConn").ConnectionString

        strSQL = "select * from Tickets"

        myCommand.CommandText = strSQL
        myCommand.CommandType = CommandType.Text
        myCommand.Connection = myConnection
        myConnection.Open()

        myAdapter = New SqlDataAdapter(strSQL, myConnection)
        myDataset = New DataSet
        myAdapter.Fill(myDataset)

        If myDataset.Tables(0).Rows.Count > 0 Then
            ticketsGV.DataSource = myDataset
            Dim dvTicket As DataView = myDataset.Tables(0).DefaultView
            Dim stringss As String = ViewState("SortByTicketNumber")
            If ViewState("SortByTicketNumber") <> Nothing Then
                dvTicket.Sort = ViewState("SortByTicketNumber").ToString()
            End If
            ticketsGV.DataSource = dvTicket
            ticketsGV.DataBind()
        End If

        myConnection.Close()
        myCommand.Dispose()
        myConnection.Dispose()

    End Sub

    Protected Sub ticketsGV_Sorting(sender As Object, e As GridViewSortEventArgs)

        Dim stringss As String = ViewState("SortByTicketNumber")
        If ViewState("SortByTicketNumber") = Nothing Then
            ViewState("SortByTicketNumber") = "LicenseState ASC"
        End If
        Dim strSortExpression As String() = ViewState("SortByTicketNumber").ToString().Split(" "c)

        If strSortExpression(0) = e.SortExpression Then
            If strSortExpression(1) = "ASC" Then
                ViewState("SortByTicketNumber") = Convert.ToString(e.SortExpression) & " " & "DESC"
            Else
                ViewState("SortByTicketNumber") = Convert.ToString(e.SortExpression) & " " & "ASC"
            End If
        Else
            ViewState("SortByTicketNumber") = Convert.ToString(e.SortExpression) & " " & "ASC"
        End If
        ticketsGridBind()
    End Sub
    Protected Sub ticketsGV_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim strSelectedRows As String = String.Empty
        Dim row As GridViewRow = ticketsGV.SelectedRow()
        strSelectedRows += "Ticket Number = " + row.Cells(1).Text
        'Mailtext.Text = strSelectedRows
        ticketsGV.Visible = False
        Session("gridViewTickets") = "False"
    End Sub

End Class