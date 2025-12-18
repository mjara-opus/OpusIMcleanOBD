Imports System.IO
Imports System.Net.Mime.MediaTypeNames
Imports System.Xml
Imports System.Threading

Imports OpusIMCleanOBDdrv


Module Module1

    Public b_initDeviceOBD As Boolean
    Public b_VehTest As Boolean
    Public cOpusIMCleanOBDdrv As New OpusIMCleanOBDdrv.IMCleanOBD

    Public xPlaca As String
    Public xFechaHora As String

    Public tmrVehTest As Integer = 25

    Public MySQLConnectionString As String = "server=localhost;uid=opus1234;pwd=1234opus;database=OpusOBDtest;Integrated Security=True"

    '//------------------------------- COLORES
    Public xFormColor As Object = Drawing.Color.FromArgb(0, 128, 128) '-- Verde Opus
    Public xMsgUsuColor As Object = Drawing.Color.FromArgb(65, 58, 102) '--#0a3a66
    Public xPanelMsgUsuColor As Object = Drawing.Color.FromArgb(10, 58, 102) '--#051d33
    Public xMsgUsuColorOscuro As Object = Drawing.Color.FromArgb(5, 29, 51) '--#051d33
    Public xFocusColor As Object = Drawing.Color.FromArgb(250, 173, 30) '--#1890ff
    Public xBtnNeutro As Object = Drawing.Color.FromArgb(223, 221, 217) '--#dfddd9
    Public xbackColorAmarillo As Object = Drawing.Color.FromArgb(250, 173, 20) '--#faad14
    Public xbackMarron As Object = Drawing.Color.FromArgb(150, 104, 12) '--#96680c
    Public xCampoTexto As Object = Drawing.Color.FromArgb(188, 183, 152) '--#96680c

    '||-------------------------------

    Public Sub Applog(ByVal lParametro As String)

        Dim logFile As String = "C:\OPUS_PROG\OpusIMCleanOBD\OpusIMCleanOBD_test.log"

        Try
            Dim lDato As String

            Form1.ListBox1.Items.Add(lParametro)
            Form1.ListBox1.Refresh()

            lDato = Format(Now, "hh:mm:ss| ") & lParametro

            Dim sw As New System.IO.StreamWriter(logFile, True)
            sw.WriteLine(lDato)
            sw.Close()

        Catch ex As Exception
            'MsgBox("AppLog:" + ex.Message)
        End Try

    End Sub

    Public Sub wrReporte(ByVal pDato As String)

        Dim txFecha As String = Format(Now, "ddMMyyyy")
        Dim logFile As String = "C:\OPUS_PROG\Reportes\OBD_IMClean_Rep_" & txFecha & ".txt"

        Try

            Dim sw As New System.IO.StreamWriter(logFile, True)
            sw.WriteLine(pDato)
            sw.Close()

        Catch ex As Exception
            'MsgBox("AppLog:" + ex.Message)
        End Try

    End Sub

End Module
