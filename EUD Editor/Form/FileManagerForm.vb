﻿Public Class FileManagerForm
    Dim TAB_INDEX As Byte

    Dim grpwire As New GRP
    Dim tranwire As New GRP
    Dim wirefram As New GRP

    Private Sub ColorReset()
        ThemeSetForm.SetControlColor(Me)
    End Sub

    Private loadcmp As Boolean = False
    Private Sub FileManagerForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadcmp = False
        Lan.SetLanguage(Me)
        Lan.SetMenu(Me, MenuStrip1)
        Lan.SetMenu(Me, ListMenu)


        'Data덤퍼에서 지정된게 있다면 4, 없다면 옵션에서 참조
        If dataDumper_stat_txt_f = 0 Then
            ComboBox3.SelectedIndex = statlang
        Else
            ComboBox3.SelectedIndex = 3
        End If



        DataGridView1.Columns(1).HeaderText = Lan.GetText(Me.Name, "OldText")
        DataGridView1.Columns(2).HeaderText = Lan.GetText(Me.Name, "NewText")
        DataGridView1.Columns(3).HeaderText = Lan.GetText(Me.Name, "Edit")


        Dim mpq As New SFMpq

        LoadFileimportable()


        'PasteRefresh()
        'ListView1.LargeImageList = DatEditForm.ICONILIST

        'For i = 0 To DatEditForm.ICONILIST.Images.Count - 1
        '    ListView1.Items.Add("")
        '    ListView1.Items(i).ImageIndex = i
        'Next

        'My.Computer.Clipboard.SetText(ImageToGRP("C:\Users\skslj\Desktop\LightBlock.bmp"))


        grpwire.Reset()
        tranwire.Reset()
        wirefram.Reset()

        grpwire.LoadPalette(14)
        tranwire.LoadPalette(14)
        wirefram.LoadPalette(14)
        If dataDumper_grpwire_f <> 0 Then
            grpwire.LoadGRP(dataDumper_grpwire)
        Else
            grpwire.LoadGRP(mpq.ReaddatFile("unit\wirefram\grpwire.grp"))
        End If

        If dataDumper_wirefram_f <> 0 Then
            wirefram.LoadGRP(dataDumper_wirefram)
        Else
            wirefram.LoadGRP(mpq.ReaddatFile("unit\wirefram\wirefram.grp"))
        End If

        If dataDumper_tranwire_f <> 0 Then
            tranwire.LoadGRP(dataDumper_tranwire)
        Else
            tranwire.LoadGRP(mpq.ReaddatFile("unit\wirefram\tranwire.grp"))
        End If

        LoadData()
        LoadList()
        PaletDraw()
        ColorReset()
        loadcmp = True
    End Sub

    Private Sub ListBox1_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ListBox1.MouseUp
        If e.Button = MouseButtons.Right Then

            Dim n As Integer = ListBox1.IndexFromPoint(e.X, e.Y)
            If n <> ListBox.NoMatches Then
                ListBox1.SelectedIndex = n
            End If

            ListMenuShow()
        End If
    End Sub

    Private Sub ListMenuShow()
        Dim cliptext As String = My.Computer.Clipboard.GetText()


        Try
            붙여넣기ToolStripMenuItem.Enabled = False
            Select Case TAB_INDEX
                Case 1
                    Dim values() As String = cliptext.Split(",")

                    If values.Count = 3 Then
                        For i = 0 To values.Count - 2
                            Try
                                Dim temp As Integer = CInt(values(i))
                                붙여넣기ToolStripMenuItem.Enabled = True
                            Catch ex As Exception
                                붙여넣기ToolStripMenuItem.Enabled = False
                                Exit For
                            End Try
                        Next
                    Else
                        붙여넣기ToolStripMenuItem.Enabled = False
                    End If

            End Select
        Catch ex As Exception
            붙여넣기ToolStripMenuItem.Enabled = False
        End Try

        ListMenu.Show()
        ListMenu.Location = MousePosition
    End Sub


    Private Sub ListBox1_DrawItem(ByVal sender As Object,
ByVal e As System.Windows.Forms.DrawItemEventArgs) Handles ListBox1.DrawItem

        If (e.Index < 0) Then Exit Sub

        Dim myBrush = New SolidBrush(ProgramSet.colorFieldText)

        If ListBox1.Items(e.Index)(LITEM.ischange) = True Then
            myBrush = Brushes.IndianRed
        End If



        If (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
            e = New DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State Xor DrawItemState.Selected, e.ForeColor,
        Color.DarkRed)
        End If


        e.DrawBackground()


        e.Graphics.DrawString(ListBox1.Items(e.Index)(LITEM.Name),
        e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)


        e.DrawFocusRectangle()

    End Sub

    Private Sub PaletDraw()
        ListView1.BeginUpdate()
        ListView1.Items.Clear()
        Dim flingyNum, SpriteNum, ImageNum As Integer
        Dim size As Integer = ListBox1.Items.Count - 1
        For i = 0 To size
            Dim index As Integer = ListBox1.Items(i)(LITEM.index)

            ListView1.Items.Add("")
            Dim itemindex As Integer = ListView1.Items.Count - 1
            flingyNum = DatEditDATA(DTYPE.units).ReadValue("Graphics", index)
            SpriteNum = DatEditDATA(DTYPE.flingy).ReadValue("Sprite", flingyNum)
            ImageNum = DatEditDATA(DTYPE.sprites).ReadValue("Image File", SpriteNum)

            ListView1.LargeImageList = DatEditForm.IMAGELIST
            ListView1.Items(itemindex).ImageIndex = ImageNum
            ListView1.Items(itemindex).Tag = index
        Next
        ListView1.EndUpdate()


        'ListView1.Clear()
        'ListView1.Items.Add(New ListView.ListViewItemCollection())
    End Sub

    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        LoadList()
        PaletDraw()
    End Sub

    Private LastSize As Integer
    Private Sub PaletteBtn(sender As Object, e As EventArgs) Handles Button5.Click
        If SplitContainer1.SplitterDistance = 24 Then
            SplitContainer1.Panel1MinSize = 93
            SplitContainer1.IsSplitterFixed = False
            SplitContainer1.SplitterDistance = LastSize '244
            Button5.Text = Lan.GetText(Me.Name, "Fold")
        Else
            LastSize = SplitContainer1.SplitterDistance
            SplitContainer1.Panel1MinSize = 24
            SplitContainer1.IsSplitterFixed = True
            SplitContainer1.SplitterDistance = 24
            Button5.Text = Lan.GetText(Me.Name, "UnFold")
        End If
    End Sub
    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.KeyUp
        LISTFILTER = TextBox2.Text

        LoadList()
        PaletDraw()
    End Sub


    Private Sub LoadListData()
        DataGridView1.SuspendLayout()
        DataGridView1.Rows.Clear()
        For i = 0 To stat_txt.Count - 1
            If TextBox1.Text <> "" Then
                If Not stat_txt(i).Contains(TextBox1.Text) Then
                    Continue For
                End If
            End If
            If TextBox3.Text <> "" Then
                If stattextdic.ContainsKey(i) Then
                    If Not stattextdic(i).Contains(TextBox3.Text) Then
                        Continue For
                    End If
                Else
                    Continue For
                End If
            End If
            If stattextdic.ContainsKey(i) Then
                DataGridView1.Rows.Add(i + 1, stat_txt(i), stattextdic(i), Lan.GetText(Me.Name, "Edit"))
                DataGridView1.Rows(DataGridView1.Rows.Count - 1).Tag = i
            Else
                DataGridView1.Rows.Add(i + 1, stat_txt(i), "", Lan.GetText(Me.Name, "Edit"))
                DataGridView1.Rows(DataGridView1.Rows.Count - 1).Tag = i
            End If
        Next
        DataGridView1.ResumeLayout()
    End Sub


    Dim LoadStatus As Boolean
    Private Sub LoadData()
        LoadStatus = True
        Select Case TAB_INDEX
            Case 0
                LoadListData()

            Case 1
                NumericUpDown1.Maximum = wirefram.framecount - 1
                NumericUpDown2.Maximum = grpwire.framecount - 1
                NumericUpDown3.Maximum = tranwire.framecount - 1

                '12 > 11
                If wirefram.framecount > _OBJECTNUM Then
                    wirefram.DrawToPictureBox(PictureBox1, wireframData(_OBJECTNUM))
                    NumericUpDown1.Value = wireframData(_OBJECTNUM)

                    If wireframData(_OBJECTNUM) = _OBJECTNUM Then
                        NumericUpDown1.BackColor = ProgramSet.colorFieldBackground
                    Else
                        NumericUpDown1.BackColor = ProgramSet.colorChangedBackground
                    End If
                    NumericUpDown1.Visible = True
                Else
                    wirefram.DrawToPictureBox(PictureBox1, 0)
                    NumericUpDown1.Visible = False
                End If
                If grpwire.framecount > _OBJECTNUM Then
                    grpwire.DrawToPictureBox(PictureBox2, grpwireData(_OBJECTNUM))
                    NumericUpDown2.Value = grpwireData(_OBJECTNUM)

                    If grpwireData(_OBJECTNUM) = _OBJECTNUM Then
                        NumericUpDown2.BackColor = ProgramSet.colorFieldBackground
                    Else
                        NumericUpDown2.BackColor = ProgramSet.colorChangedBackground
                    End If
                    NumericUpDown2.Visible = True
                Else
                    grpwire.DrawToPictureBox(PictureBox2, 0)
                    NumericUpDown2.Visible = False
                End If

                If tranwire.framecount > _OBJECTNUM Then
                    tranwire.DrawToPictureBox(PictureBox3, tranwireData(_OBJECTNUM))
                    NumericUpDown3.Value = tranwireData(_OBJECTNUM)

                    If tranwireData(_OBJECTNUM) = _OBJECTNUM Then
                        NumericUpDown3.BackColor = ProgramSet.colorFieldBackground
                    Else
                        NumericUpDown3.BackColor = ProgramSet.colorChangedBackground
                    End If
                    NumericUpDown3.Visible = True
                Else
                    tranwire.DrawToPictureBox(PictureBox3, 0)
                    NumericUpDown3.Visible = False
                End If
        End Select
        LoadStatus = False
    End Sub

    Private Sub FileManagerForm_Close(sender As Object, e As EventArgs) Handles MyBase.Closing
        DataGridView1.EndEdit()
    End Sub


    Private Sub SELECTLIST(index As Integer)
        ListBox1.SelectedIndex = -1

        For i = 0 To ListBox1.Items.Count - 1
            If ListBox1.Items(i)(LITEM.index) = index Then
                ListBox1.SelectedIndex = i
                _OBJECTNUM = index
                Exit Sub
            End If
        Next


        If ListBox1.SelectedIndex = -1 Then
            If ListBox1.Items.Count <> 0 Then
                ListBox1.SelectedIndex = 0
                _OBJECTNUM = ListBox1.Items(0)(LITEM.index)
                Exit Sub
            End If
        End If

        _OBJECTNUM = 0
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedIndex <> -1 Then
            _OBJECTNUM = ListBox1.SelectedItem(LITEM.index)

            LoadData()
        End If
    End Sub
    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.Click, ListView1.ItemSelectionChanged
        Try
            SELECTLIST(ListView1.SelectedItems(0).Tag)
        Catch ex As Exception
        End Try
    End Sub

    Enum LITEM
        ischange = 0
        index = 1
        Name = 2
    End Enum

    Dim LISTFILTER As String
    Dim _OBJECTNUM As Integer
    Private Sub LoadList()
        Dim lastSELECT As Integer = _OBJECTNUM


        ListBox1.BeginUpdate()

        ListBox1.Items.Clear()



        For i = 0 To CODE(DTYPE.units).Count - 1
            Dim list(2) As String

            Dim temp As String = CODE(DTYPE.units)(i)
            If DatEditDATA(DTYPE.units).ReadValue("Unit Map String", i) = 0 Then
                list(LITEM.Name) = temp
            Else
                Try
                    list(LITEM.Name) = ProjectSet.CHKSTRING(-1 + DatEditDATA(DTYPE.units).ReadValue("Unit Map String", i)) & " (" & temp & ")" 'ProjectSet.UNITSTR(index)
                Catch ex As Exception
                    list(LITEM.Name) = temp
                End Try

            End If
            list(LITEM.index) = i
            list(LITEM.ischange) = False
            list(LITEM.Name) = "[" & Format(i, "000") & "]- " & list(LITEM.Name)




            If wireframData(i) <> i Or grpwireData(i) <> i Or tranwireData(i) <> i Then
                list(LITEM.ischange) = True
            End If




            Dim stra, strb As String
            stra = list(LITEM.Name).ToLower
            If LISTFILTER <> "" Then
                strb = LISTFILTER.ToLower
            Else
                strb = ""
            End If


            If CheckBox5.Checked = True Then
                If list(LITEM.ischange) = True And InStr(stra, strb) <> 0 Then
                    ListBox1.Items.Add(list)
                End If
            Else
                If InStr(stra, strb) <> 0 Then
                    ListBox1.Items.Add(list)
                End If
            End If
        Next


        SELECTLIST(lastSELECT)


        ListBox1.EndUpdate()
    End Sub


    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        StatTextAdd(DataGridView1.Rows(e.RowIndex).Tag, DataGridView1.Item(e.ColumnIndex, e.RowIndex).Value)
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = 3 Then
            Dim dialog As DialogResult
            StatTextForm.stringNum = DataGridView1.Rows(e.RowIndex).Tag
            dialog = StatTextForm.ShowDialog()
            If dialog = DialogResult.OK Then

                StatTextAdd(DataGridView1.Rows(e.RowIndex).Tag, StatTextForm.RawText)
                DataGridView1.Item(2, e.RowIndex).Value = StatTextForm.RawText

                'ComboBox32.Items(TextBox58.Text) = StatTextForm.RawText
            End If
        End If
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        TAB_INDEX = TabControl1.SelectedIndex
        LoadData()
    End Sub

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        If LoadStatus = False Then
            wireframData(_OBJECTNUM) = NumericUpDown1.Value
            LoadData()
            If ListBox1.SelectedIndex <> -1 Then
                If wireframData(_OBJECTNUM) <> _OBJECTNUM Then
                    ListBox1.SelectedItem(LITEM.ischange) = True
                Else
                    ListBox1.SelectedItem(LITEM.ischange) = False
                End If
            End If
        End If
    End Sub

    Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged
        If LoadStatus = False Then
            grpwireData(_OBJECTNUM) = NumericUpDown2.Value
            LoadData()
            If ListBox1.SelectedIndex <> -1 Then
                If grpwireData(_OBJECTNUM) <> _OBJECTNUM Then
                    ListBox1.SelectedItem(LITEM.ischange) = True
                Else
                    ListBox1.SelectedItem(LITEM.ischange) = False
                End If
            End If
        End If
    End Sub

    Private Sub NumericUpDown3_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown3.ValueChanged
        If LoadStatus = False Then
            tranwireData(_OBJECTNUM) = NumericUpDown3.Value
            LoadData()
            If ListBox1.SelectedIndex <> -1 Then
                If tranwireData(_OBJECTNUM) <> _OBJECTNUM Then
                    ListBox1.SelectedItem(LITEM.ischange) = True
                Else
                    ListBox1.SelectedItem(LITEM.ischange) = False
                End If
            End If
        End If
    End Sub

    Private Sub 초기화ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 초기화ToolStripMenuItem.Click
        wireframData(_OBJECTNUM) = _OBJECTNUM
        grpwireData(_OBJECTNUM) = _OBJECTNUM
        tranwireData(_OBJECTNUM) = _OBJECTNUM
        If ListBox1.SelectedIndex <> -1 Then
            ListBox1.SelectedItem(LITEM.ischange) = False
        End If
        LoadData()
    End Sub

    Private Sub 복사ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 복사ToolStripMenuItem.Click
        Dim str As String = ""
        Select Case TAB_INDEX
            Case 1
                str = wireframData(_OBJECTNUM) & "," & grpwireData(_OBJECTNUM) & "," & tranwireData(_OBJECTNUM)
        End Select

        PasteRefresh()
        Try
            My.Computer.Clipboard.SetText(str)
        Catch ex As Exception
            My.Computer.Clipboard.Clear()
        End Try
    End Sub

    Private Sub 붙여넣기ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 붙여넣기ToolStripMenuItem.Click
        Dim cliptext As String = My.Computer.Clipboard.GetText

        Select Case TAB_INDEX
            Case 1
                Dim codes() As String = cliptext.Split(",")

                wireframData(_OBJECTNUM) = codes(0)
                grpwireData(_OBJECTNUM) = codes(1)
                tranwireData(_OBJECTNUM) = codes(2)

                If ListBox1.SelectedIndex <> -1 Then
                    If wireframData(_OBJECTNUM) = _OBJECTNUM And grpwireData(_OBJECTNUM) = _OBJECTNUM And tranwireData(_OBJECTNUM) = _OBJECTNUM Then
                        ListBox1.SelectedItem(LITEM.ischange) = False
                    Else
                        ListBox1.SelectedItem(LITEM.ischange) = True
                    End If
                End If

                LoadData()
        End Select
    End Sub

    Private Sub PasteRefresh()
        Dim cliptext As String = My.Computer.Clipboard.GetText()


        Try
            오브젝트붙여넣기ToolStripMenuItem.Enabled = False
            Select Case TAB_INDEX
                Case 1
                    Dim values() As String = cliptext.Split(",")

                    If values.Count = 3 Then
                        For i = 0 To values.Count - 2
                            Try
                                Dim temp As Integer = CInt(values(i))
                                오브젝트붙여넣기ToolStripMenuItem.Enabled = True
                            Catch ex As Exception
                                오브젝트붙여넣기ToolStripMenuItem.Enabled = False
                                Exit For
                            End Try
                        Next
                    Else
                        오브젝트붙여넣기ToolStripMenuItem.Enabled = False
                    End If

            End Select
        Catch ex As Exception
            오브젝트붙여넣기ToolStripMenuItem.Enabled = False
        End Try
    End Sub
    Private Sub 편집ToolStripMenuItem_DropDownOpening(sender As Object, e As EventArgs) Handles 편집ToolStripMenuItem.DropDownOpening
        PasteRefresh()
    End Sub
    Private Sub 편집ToolStripMenuItem_DropDownClosed(sender As Object, e As EventArgs) Handles 편집ToolStripMenuItem.DropDownClosed
        오브젝트붙여넣기ToolStripMenuItem.Enabled = True
    End Sub

    Private Sub 오브젝트초기화ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 오브젝트초기화ToolStripMenuItem.Click
        wireframData(_OBJECTNUM) = _OBJECTNUM
        grpwireData(_OBJECTNUM) = _OBJECTNUM
        tranwireData(_OBJECTNUM) = _OBJECTNUM
        If ListBox1.SelectedIndex <> -1 Then
            ListBox1.SelectedItem(LITEM.ischange) = False
        End If
        LoadData()
    End Sub

    Private Sub 오브젝트복사ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 오브젝트복사ToolStripMenuItem.Click
        Dim str As String = ""
        Select Case TAB_INDEX
            Case 1
                str = wireframData(_OBJECTNUM) & "," & grpwireData(_OBJECTNUM) & "," & tranwireData(_OBJECTNUM)
        End Select

        Try
            My.Computer.Clipboard.SetText(str)
        Catch ex As Exception
            My.Computer.Clipboard.Clear()
        End Try
    End Sub

    Private Sub 오브젝트붙여넣기ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 오브젝트붙여넣기ToolStripMenuItem.Click
        Dim cliptext As String = My.Computer.Clipboard.GetText()


        Try
            Select Case TAB_INDEX
                Case 1
                    Dim values() As String = cliptext.Split(",")

                    If values.Count = 3 Then
                        For i = 0 To values.Count - 2
                            Try

                            Catch ex As Exception
                                Exit Sub
                            End Try
                        Next
                    Else
                        Exit Sub
                    End If

            End Select
        Catch ex As Exception
            Exit Sub
        End Try


        Select Case TAB_INDEX
            Case 1
                Dim codes() As String = cliptext.Split(",")

                wireframData(_OBJECTNUM) = codes(0)
                grpwireData(_OBJECTNUM) = codes(1)
                tranwireData(_OBJECTNUM) = codes(2)

                If ListBox1.SelectedIndex <> -1 Then
                    If wireframData(_OBJECTNUM) = _OBJECTNUM And grpwireData(_OBJECTNUM) = _OBJECTNUM And tranwireData(_OBJECTNUM) = _OBJECTNUM Then
                        ListBox1.SelectedItem(LITEM.ischange) = False
                    Else
                        ListBox1.SelectedItem(LITEM.ischange) = True
                    End If
                End If

                LoadData()
        End Select
    End Sub

    Private Sub 테마설정TToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 테마설정TToolStripMenuItem.Click
        ThemeSetForm.ShowDialog()
        ColorReset()
        LoadData()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        LoadListData()
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
        If loadcmp Then
            statlang = ComboBox3.SelectedIndex
            If statlang <> 3 Then
                dataDumper_stat_txt_f = 0
                dataDumper_stat_txt = ""
            Else
                ComboBox3.SelectedIndex = 0
                Exit Sub
            End If
            stat_txt = Readstat_txtfile(True)
            LoadListData()
        End If
    End Sub
End Class