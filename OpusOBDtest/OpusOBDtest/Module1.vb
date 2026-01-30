Imports System.IO
Imports System.Net.Mime.MediaTypeNames
Imports System.Xml
Imports System.Threading

Imports OpusIMCleanOBDdrv


Module Module1

    Public b_initDeviceOBD As Boolean
    Public b_VehTest As Boolean
    Public cOpusIMCleanOBDdrv As New OpusIMCleanOBDdrv.IMCleanOBD

    Public tmrVehTest As Integer = 25

    Public MySQLConnectionString As String = "server=localhost;uid=opus1234;pwd=1234opus;database=OpusOBDtest;Integrated Security=True"

    Public ldbPlaca As String
    Public ldbFechaHora As String
    Public ldbMARCA As String
    Public ldbSubMARCA As String
    Public ldbModelo As String

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

    Public Sub wrDataCSV(ByVal pDato As String)

        Dim logFile As String = "C:\OPUS_PROG\DATABASES\OBD_IMClean_Data.csv"

        Try

            If Not System.IO.File.Exists(logFile) Then

                Dim TitFile As String = "Fecha-Hora," &
                                            "Placa," &
                                            "Marca," &
                                            "SubMarca, " &
                                            "Modelo," &
                                            "OBD_ECU, " &
                                            "Voltaje, " &
                                            "Protocolo," &
                                            "VIN, " &
                                            "MIL," &
                                            "MSI-d, " &
                                            "MSI-c, " &
                                            "MSI-e, " &
                                            "CCM-d," &
                                            "CCM-c," &
                                            "CCM-e," &
                                            "CMB-d, " &
                                            "CMB-c, " &
                                            "CMB-e, " &
                                            "O2S-d," &
                                            "O2S-c," &
                                            "O2S-e," &
                                            "CAT-d, " &
                                            "CAT-c, " &
                                            "CAT-e, " &
                                            "CCC-d," &
                                            "CCC-c," &
                                            "CCC-e," &
                                            "EVS-d, " &
                                            "EVS-c, " &
                                            "EVS-e, " &
                                            "SAS-d," &
                                            "SAS-c," &
                                            "SAS-e," &
                                            "FAA-d, " &
                                            "FAA-c, " &
                                            "FAA-e, " &
                                            "O2C-d," &
                                            "O2C-c," &
                                            "O2C-e," &
                                            "Simulation, " &
                                            "DTC"

                Dim sw As New System.IO.StreamWriter(logFile, True)

                sw.WriteLine(TitFile)
                sw.WriteLine(pDato)
                sw.Close()

            Else

                Dim sw As New System.IO.StreamWriter(logFile, True)
                sw.WriteLine(pDato)
                sw.Close()

            End If

        Catch ex As Exception
            'MsgBox("AppLog:" + ex.Message)
        End Try

    End Sub




End Module
