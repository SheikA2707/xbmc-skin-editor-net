﻿Imports System.Text.RegularExpressions
Public Class frmMain
#Region "Declaration"
    Dim xbmccommunicator As New XBMC.XBMC_Communicator
    Const debugex As Integer = 0
    Dim currentskin As skin
    Dim currentskin_file As skin_file
    Dim skindir As IO.DirectoryInfo
    Dim control_startline, control_endline As Integer
    Dim control_tabulation As String
    Dim changedavalue As Boolean
    Dim texturearray As New ArrayList
    Dim modified_font As System.Drawing.Font
    Dim modified_line As New List(Of String)
#End Region
#Region "menus"
    'open skin
    Private Sub OpenSkinToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenSkinToolStripMenuItem.Click
        frmOpenSkin.ShowDialog()
    End Sub
    'create skin
    Private Sub CreateSkinToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CreateSkinToolStripMenuItem.Click
        frmCreateSkin.ShowDialog()
    End Sub

    Private Sub cmdTestWindow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdTestWindow.Click

        If SaveFile() Then
            Dim strmwriter As IO.StreamWriter = currentskin_file.Xml_File.CreateText()
            For Each lne As String In txtWindow.Lines
                strmwriter.WriteLine(lne)
            Next
            strmwriter.Close()
            xbmccommunicator.Status.Refresh()
            If xbmccommunicator.Status.IsConnected() = True Then
                xbmccommunicator.Controls.ActivateWindow(currentskin_file.windowid.ToString)
            Else
                MsgBox("xbmc is not open or the settings to control xbmc are not correct")
            End If

        End If
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsToolStripMenuItem.Click
        frmSettings.Show()
    End Sub

    Private Sub cmdSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click
        SaveFile()
    End Sub

    Private Sub cmdReload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReload.Click
        txtWindow.Text = IO.File.ReadAllText(currentskin_file.Xml_File.FullName)
    End Sub
#End Region
#Region "Skin function and sub"
    Function GetControlAttribute(ByVal attrib As List(Of control_attributes), ByVal value As String) As String
        Dim type As String = ""
        For Each ctr As control_attributes In attrib
            If ctr.name.Equals(value) Then
                Return ctr.value
            End If
        Next
        Return type
    End Function

    Private Sub SetControl(ByVal ctrl As xml_control)
        Dim index As Integer = 0
        Dim type As String = GetControlAttribute(ctrl.attributes, "type")

        Dim choicearray As New ArrayList()
        Dim includearray As New ArrayList()
        Dim textarray As New ArrayList()
        propGridControl.ItemSet.Clear()
        propGridControl.Refresh()
        'propGridControl.PropertySort = PropertySort.Categorized
        For Each ctr As control_attributes In ctrl.attributes
            If ctr.show_in_property = True Then
                index = propGridControl.Item.Add(ctr.name, ctr.value, False, type, ctr.description, True)
                If ctr.choices > -1 Then
                    Select Case ctr.choices
                        Case ChoiceType.ControlId
                            If choicearray.Count = 0 Then
                                choicearray = currentskin_file.ListControlIds
                            End If
                            propGridControl.Item(index).Choices = New PropertyGridEx.CustomChoices(choicearray, True)
                        Case ChoiceType.Include
                            If includearray.Count = 0 Then
                                includearray.AddRange(currentskin.GetIncludes().ToArray())
                            End If
                            propGridControl.Item(index).Choices = New PropertyGridEx.CustomChoices(includearray, True)
                        Case ChoiceType.Texture
                            If textarray.count = 0 Then
                                textarray.AddRange(texturearray.ToArray())
                            End If
                            propGridControl.Item(index).Choices = New PropertyGridEx.CustomChoices(textarray, True)

                    End Select

                End If
            End If
        Next
        propGridControl.Refresh()
    End Sub

    Function ConvertListToString(ByVal list As List(Of String)) As String
        Dim retstring As String = ""
        For Each line As String In list

            retstring += line + vbNewLine
        Next
        If retstring.Length > 0 Then
            retstring = retstring.Substring(0, retstring.Length() - 1)
        End If
        Return retstring
    End Function

    Private Sub LoadTexture(ByVal dir As IO.DirectoryInfo)
        texturearray.Clear()
        skindir = dir
        Dim mediadir As IO.DirectoryInfo
        Dim texture As String
        If (IO.Directory.Exists(dir.FullName + "\media")) Then
            mediadir = New IO.DirectoryInfo(dir.FullName + "\media")
            For Each fle As IO.FileInfo In mediadir.GetFiles("*.png", System.IO.SearchOption.AllDirectories)

                texture = fle.FullName.Replace(mediadir.FullName + "\", "")
                texturearray.Add(texture)
            Next


        End If


    End Sub
    Public Sub LoadSkin(ByVal skinpath As String)
        cmbWindow.Items.Clear()
        currentskin = New skin(skinpath)
        For Each fle As String In currentskin.GetWindows()

            cmbWindow.Items.Add(fle)

        Next
        Dim dir As New IO.DirectoryInfo(skinpath)
        Me.Text = "Xbmc Skin editor Current skin is : " + dir.Name
        cmbWindow.SelectedIndex = 0
        control_startline = 0
        control_endline = 0
        control_tabulation = ""
        changedavalue = False
        LoadTexture(dir)
    End Sub

    Function SaveFile() As Boolean
        If currentskin_file.windowid.ToString.Length > 0 Then
            Dim strmwriter As IO.StreamWriter = currentskin_file.Xml_File.CreateText()
            For Each lne As String In txtWindow.Lines
                strmwriter.WriteLine(lne)
            Next
            strmwriter.Close()
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub LoadWindowNamed(ByVal window As String)
        currentskin_file = currentskin.GetWindow(window)

    End Sub

    Public Sub SetXbmcCommunicator()
        Dim ip As String = My.Settings.ip
        If My.Settings.port.Length > 0 Then
            ip = ip + ":" + My.Settings.port
        End If

        xbmccommunicator.SetIp(ip)

        xbmccommunicator.SetConnectionTimeout(3000)
        xbmccommunicator.SetCredentials(My.Settings.username, My.Settings.password)


    End Sub
    Private Sub ExHandling(ByVal msg As String)
        If debugex = 0 Then
            Debug.Print(msg)
        Else
            MsgBox(msg)
        End If

    End Sub
#End Region
#Region "Forms handles"
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If My.Settings.currentskin.Length = 0 Then
            brwFolder.Description = "Select the directory of the skin you want to edit"
            Dim appData As String = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)
            brwFolder.SelectedPath = appData
            If IO.Directory.Exists(appData + "\xbmc") Then
                brwFolder.SelectedPath = appData + "\xbmc"
            End If
            If IO.Directory.Exists(brwFolder.SelectedPath + "\addons") Then
                brwFolder.SelectedPath = brwFolder.SelectedPath + "\addons"
                Dim dir As New IO.DirectoryInfo(brwFolder.SelectedPath)
                For Each dr As IO.DirectoryInfo In dir.GetDirectories()
                    If dr.Name.Contains("skin.") Then
                        brwFolder.SelectedPath = dr.FullName
                        Exit For
                    End If
                Next
            End If
            If brwFolder.ShowDialog() = Windows.Forms.DialogResult.OK Then
                My.Settings.currentskin = brwFolder.SelectedPath
                My.Settings.Save()
            Else
                Me.Close()
            End If
        End If
        LoadSkin(My.Settings.currentskin)

        SetXbmcCommunicator()
        Dim sze As Single = txtWindow.Font.Size
        modified_font = New System.Drawing.Font("Microsoft Sans Serif", sze, System.Drawing.FontStyle.Bold)
    End Sub
#End Region

#Region "Control Handles"
    Private Sub cmbWindow_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbWindow.SelectedIndexChanged
        LoadWindowNamed(cmbWindow.Text)
        txtWindow.Text = IO.File.ReadAllText(currentskin_file.Xml_File.FullName)
        For Each mtch As Match In StringHelper.RegEx.GetGroupsWithoutMultiLine("control .+?id=""([^""]+)""", txtWindow.Text)
            currentskin_file.ListControlIds.Add(mtch.Groups(1).Value)
        Next

    End Sub



    Private Sub txtWindow_CursorPositionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtWindow.CursorPositionChanged
        If changedavalue = True Then
            changedavalue = False
            Exit Sub
        End If
        Dim ResultString As String = ""
        Dim SubjectLines As New List(Of String)
        Dim lsttab As New List(Of Char)
        propGridControl.Item.Clear()
        control_startline = 0
        control_endline = 0
        control_tabulation = ""
        For x As Integer = txtWindow.CurrentLine To 0 Step -1
            If (txtWindow.Lines(x).Contains("control") And txtWindow.Lines(x).Contains("type")) Then
                control_startline = x
                For xx As Integer = x To txtWindow.Lines.Length() - 1
                    SubjectLines.Add(txtWindow.Lines(xx))
                    If txtWindow.Lines(xx).Contains("control") = False And control_tabulation.Length = 0 Then
                        control_tabulation = txtWindow.Lines(xx).Substring(0, txtWindow.Lines(xx).IndexOf("<"))

                    End If
                    If (txtWindow.Lines(xx).Contains("</control>")) Then
                        control_endline = xx
                        Exit For
                    End If

                Next

                ResultString = ConvertListToString(SubjectLines)

            End If
            If control_endline > 0 Then Exit For

        Next
        'got a result
        If ResultString.Length > 0 Then

            Dim ctrl As New xml_control(ResultString)
            SetControl(ctrl)

            'index = propGridControl.Item.Add(ctr.name, ctr.value, False, ctrl.type, "value blablabla", True)
        End If
    End Sub

    Private Sub propGridControl_PropertyValueChanged(ByVal s As Object, ByVal e As System.Windows.Forms.PropertyValueChangedEventArgs) Handles propGridControl.PropertyValueChanged
        Debug.Print("changing value of" + e.ChangedItem.Label)
        Dim newText As New List(Of String)
        Dim lineindex As Integer = 0
        Dim addedline As String = ""
        Dim insertLine As Boolean = True

        For Each lne As String In txtWindow.Lines
            addedline = lne
            If control_startline <= lineindex And control_endline > lineindex Then
                If lne.Contains("""" + e.ChangedItem.Label + """") Or lne.Contains("<" + e.ChangedItem.Label) Then
                    addedline = lne.Replace(e.OldValue, e.ChangedItem.Value)
                    modified_line.Add(addedline)
                    insertLine = False
                End If
            End If
            newText.Add(addedline)
            lineindex += 1
        Next

        If insertLine Then
            If e.ChangedItem.Label.Equals("id") Then
                newText(control_startline) = newText(control_startline).Replace(">", " id=""" + e.ChangedItem.Value + """>")
                modified_line.Add(newText(control_startline))
            Else
                newText.Insert(control_endline, control_tabulation + "<" + e.ChangedItem.Label + ">" + e.ChangedItem.Value.ToString + "</" + e.ChangedItem.Label + ">")
                modified_line.Add(newText(control_endline))
                control_endline += 1
            End If
        End If

        changedavalue = True
        txtWindow.Text = ConvertListToString(newText)
        For Each lne As String In modified_line
            Dim res As Integer = txtWindow.Find(lne)
            If res > -1 Then
                txtWindow.SelectionStart = res
                txtWindow.SelectionLength = lne.Length
                txtWindow.SelectionFont = modified_font
            End If
        Next


        'load image
        If frmPreview.Visible = True And e.ChangedItem.Label.Contains("texture") Then
            frmPreview.LoadImage(skindir.FullName + "\media\" + e.ChangedItem.Value)
        End If

    End Sub
#End Region

    
    Private Sub MediaPreviewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MediaPreviewToolStripMenuItem.Click
        frmPreview.Show()
    End Sub

    Private Sub propGridControl_SelectedGridItemChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.SelectedGridItemChangedEventArgs) Handles propGridControl.SelectedGridItemChanged
        If e.NewSelection.Label.Contains("texture") = False Then Exit Sub
        If e.NewSelection.Value.ToString().Length = 0 Then Exit Sub
        If frmPreview.Visible Then

            frmPreview.LoadImage(skindir.FullName + "\media\" + e.NewSelection.Value)

        End If

    End Sub
End Class
