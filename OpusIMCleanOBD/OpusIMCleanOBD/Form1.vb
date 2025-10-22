
Imports DrewTech.IIMClean
Imports System.Threading

Public Class Form1


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Dim sendThread As New Thread(AddressOf SendLoop)
        'sendThread.Start()
        PrgBrComm.BackColor = Drawing.Color.Red
        PrgBrComm.Maximum = 100

        ListBox1.HorizontalScrollbar = True

    End Sub

    Private Sub SendLoop()

        'PrgBrComm.BackColor = Drawing.Color.Red
        'PrgBrComm.Maximum = 100
        Dim IxPrgBrSalval As Integer
        For IxPrgBrSalval = 1 To 100
            System.Threading.Thread.Sleep(2000)
            PrgBrComm.Value = IxPrgBrSalval

        Next

    End Sub


    Private Sub LimpiaPantalla()

        ListBox1.Items.Clear()

        Me.TextBox1.Text = ""
        Me.TextBox2.Text = ""
        Me.TextBox3.Text = ""
        Me.TextBox4.Text = ""
        Me.TextBox5.Text = ""
        Me.TextBox6.Text = ""
        Me.TextBox7.Text = ""
        Me.TextBox8.Text = ""

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Call LimpiaPantalla() 'ListBox1.Items.Clear()
        TextBoxTxt.Text = ">>> Checa Device ..."
        Applog(">>> Checa Device ...................................................")

        Dim lReturn() As String = CheckForUSBDevices()

        If (IsNothing(lReturn) = False) Then

            If lReturn.Count > 0 Then

                Me.TextBox1.Text = "DeviceID: " & strDeviceInfo.DeviceID
                Me.TextBox2.Text = "HardwareIDs: " & strDeviceInfo.DeviceHardwareIDs
                Me.TextBox3.Text = "PortName: " & strDeviceInfo.DevicePortName
                Me.TextBox4.Text = "Manufacturer: " & strDeviceInfo.DeviceManufacturer
                Me.TextBox5.Text = "Description: " & strDeviceInfo.DeviceDescription

                Applog("*** DeviceID: " & strDeviceInfo.DeviceID)
                Applog("*** HardwareIDs: " & strDeviceInfo.DeviceHardwareIDs)
                Applog("*** PortName: " & strDeviceInfo.DevicePortName)
                Applog("*** Manufacturer: " & strDeviceInfo.DeviceManufacturer)
                Applog("*** Description: " & strDeviceInfo.DeviceDescription)

                TextBoxTxt.Text = "*** Checa Device .. ok "

            End If
        Else

            TextBoxTxt.Text = "*** Err:Dispositivo IMClean no detectado. "

        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click


        If System.IO.File.Exists(logFile) Then System.IO.File.Delete(logFile)

        Try

            Call LimpiaPantalla() 'ListBox1.Items.Clear()
            TextBoxTxt.Text = ">>> Conecta OBD ..."
            Applog(">>> Conecta OBD ...................................................")

            Call OpenDevice()

            TextBoxTxt.Text = "*** Conecta OBD .. ok"

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        'System.Diagnostics.Process.Start("notepad.exe", logFile)

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        Call LimpiaPantalla() 'ListBox1.Items.Clear()
        TextBoxTxt.Text = ">>> OBD - ECU ..."
        Applog(">>> OBD - ECU ...................................................")
        TextBoxTxt.Refresh()

        MyDADSemaphore = New System.Threading.Semaphore(1, 1)
        MyDoDADSemaphore = New System.Threading.Semaphore(1, 1)

        Call Connect_OBD()

        TextBoxTxt.Text = "*** OBD - ECU .. ok "

        'System.Diagnostics.Process.Start("notepad.exe", logFile)

    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed

        If (IsNothing(MyDad) = False) Then MyDad.Close()

    End Sub

End Class
