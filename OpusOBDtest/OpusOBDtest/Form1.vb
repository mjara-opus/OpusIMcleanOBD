
Imports System.Net.NetworkInformation
Imports System.Runtime.Serialization.Formatters
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1

    Public Hilo01 As Thread
    Public Hilo02 As Thread
    Public tmrReloj As Integer
    Private Sub frmOBDtest_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim lStatus As String = Nothing

        Me.Location = New Drawing.Point(5, 5)

        UsrBallTimer.wTime = 150
        UsrBallTimer.Visible = False

        Call rntMensajeusuario("Inicialice el IMClean OBD.")

        Call RntCargaTablas()

        ListBox1.HorizontalScrollbar = True

        tmrReloj = 0
        Timer1.Enabled = True

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        lblFechaHora.Text = Format(Now, "dd-MMM-yyyy - HH:mm:ss")

        If cOpusIMCleanOBDdrv.DeviceData.DeviceLinkOn And b_initDeviceOBD Then

            b_initDeviceOBD = False
            PicOBD.Visible = True
            UsrBallTimer.Visible = False
            BtnInitDevice.Enabled = False
            BtnOBDtest.Enabled = True
            txtPlaca.Enabled = True
            btnSalvar.Enabled = True
            txtPlaca.Select()

        Else

            If b_initDeviceOBD Then

                UsrBallTimer.Visible = True
                tmrReloj += 1
                If tmrReloj < 150 Then Call Me.UsrBallTimer.tick_tock()

            End If

        End If

        'If b_VehTest Then
        'UsrBallTimer.Visible = True
        'tmrReloj += 1
        'If tmrReloj < tmrVehTest Then Call Me.UsrBallTimer.tick_tock()
        'End If


    End Sub

    Private Sub BtnInitDevice_Click(sender As Object, e As EventArgs) Handles BtnInitDevice.Click

        ListBox1.Items.Clear()

        tmrReloj = 0
        UsrBallTimer.wTime = 150
        UsrBallTimer.Refresh()
        UsrBallTimer.Visible = True

        Call ComunicaDeviceOBD()

    End Sub

    Private Sub ComunicaDeviceOBD()

        Dim lStatus As String = "Iniciando conexión con el IMClean OBD..." & Format(Now, "HH:mm:ss")

        Call rntMensajeusuario(lStatus)
        Applog(lStatus)

        Do

            lStatus = cOpusIMCleanOBDdrv.ReviewDevicePlug()

            If Mid(lStatus, 1, 4) = "Key?" Then '-- requerimiento de licencia

                Call rntMensajeusuario("!:Opus IMClean OBD requiere licencia de software. " & lStatus) ' Opus IMClean OBD requiere licencia usuario.")
                Applog("Opus IMClean OBD requiere licencia de software.") '"Opus IMClean OBD licencia no valida o expirada.")
                Applog(lStatus)

                lStatus = InputBox(Mid(lStatus, 5, 50), "Ingrese Licencia de software.", lStatus)

                lStatus = cOpusIMCleanOBDdrv.set_OpusKeyDevice(lStatus)

                If Mid(lStatus, 1, 4) = "Err:" Then Exit Do

            Else

                Exit Do

            End If

        Loop

        If Mid(lStatus, 1, 4) = "Pass" Then

            lblDeviceDescription.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceDescription
            lblIdDevice.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceID

            Applog("DeviceData ")
            Applog("-------------------------------------------------------")
            Applog("initDeviceOBD: " & lStatus)

            If Mid(lStatus, 1, 4) = "Pass" Then

                Applog("Licencia: " & Year(Now) & Chr(174) & " Ok")
                Applog("DeviceDescription: " & cOpusIMCleanOBDdrv.DeviceData.DeviceDescription)
                Applog("DeviceID: " & cOpusIMCleanOBDdrv.DeviceData.DeviceID)
                Applog(" ")
                Applog("IMClean OBD instalado en el  CPU, Iniciando dispositivo ...")

                Call rntMensajeusuario("IMClean OBD instalado en el  CPU, Iniciando dispositivo ...")

                Control.CheckForIllegalCrossThreadCalls = False
                Threading.Thread.CurrentThread.ApartmentState = Threading.ApartmentState.STA

                b_initDeviceOBD = True
                Hilo01 = New Thread(AddressOf initDeviceOBD)
                Hilo01.Start()

            End If

        End If

        'Applog(lStatus)
        Call rntMensajeusuario(lStatus)

    End Sub


    Private Sub initDeviceOBD()

        Dim lStatus As String = Nothing

        lStatus = cOpusIMCleanOBDdrv.InitIMCleanDevice()

        Applog("initDeviceOBD: " & lStatus)

        If Mid(lStatus, 1, 4) = "Pass" Then

            lblVoltaje.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceVoltage
            lblVoltajeDLC.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceVoltageDLC
            lblFirmWare.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceFirmwareVersion

            Applog("DeviceManufacturer: " & cOpusIMCleanOBDdrv.DeviceData.DeviceManufacturer)
            Applog("DeviceType: " & cOpusIMCleanOBDdrv.DeviceData.DeviceType)
            Applog("DeviceVoltage: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltage)
            Applog("DeviceVoltageDLC: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltageDLC)

            lStatus = "Opus IMClean OBD en línea. Esperando comunicación con el ECU del vehículo."

            Applog(lStatus)
            Call rntMensajeusuario(lStatus)
            System.Threading.Thread.Sleep(250)

            Hilo01.Abort()

        End If

        b_initDeviceOBD = False

    End Sub

    Private Sub limpiaCampos()

        ListBox1.Items.Clear()

        lblProtocolo.Text = " "
        LblVIN.Text = " "
        lblDTC.Text = " "

        LblOBD_mil.BackColor = Drawing.Color.Gray

        LblOBD_MSI_D.BackColor = Drawing.Color.Gray
        LblOBD_MSI_C.BackColor = Drawing.Color.Gray
        LblOBD_MSI.BackColor = Drawing.Color.Gray

        LblOBD_CCM_D.BackColor = Drawing.Color.Gray
        LblOBD_CCM_C.BackColor = Drawing.Color.Gray
        LblOBD_CCM.BackColor = Drawing.Color.Gray

        LblOBD_CMB_D.BackColor = Drawing.Color.Gray
        LblOBD_CMB_C.BackColor = Drawing.Color.Gray
        LblOBD_CMB.BackColor = Drawing.Color.Gray

        LblOBD_O2S_D.BackColor = Drawing.Color.Gray
        LblOBD_O2S_C.BackColor = Drawing.Color.Gray
        LblOBD_O2S.BackColor = Drawing.Color.Gray

        LblOBD_CAT_D.BackColor = Drawing.Color.Gray
        LblOBD_CAT_C.BackColor = Drawing.Color.Gray
        LblOBD_CAT.BackColor = Drawing.Color.Gray

        Me.Refresh()

    End Sub



    Private Sub BtnOBDtest_Click(sender As Object, e As EventArgs) Handles BtnOBDtest.Click

        Dim lStatus As String = "!:IMClean OBD comunicando con el ECU del vehículo, espere un momento..."

        ldbFechaHora = Format(Now, "dd/MM/yyyy -  HH:mm:ss")

        Call limpiaCampos()

        If Len(ldbPlaca) < 3 Then

            Call rntMensajeusuario("!:Captura la placa / matricula del vehículo.")
            txtPlaca.Select()

        Else

            Applog(lStatus)
            Call rntMensajeusuario(lStatus)

            'Control.CheckForIllegalCrossThreadCalls = False
            'Threading.Thread.CurrentThread.ApartmentState = Threading.ApartmentState.STA

            'UsrBallTimer.wTime = tmrVehTest
            'UsrBallTimer.resetAvance()
            'UsrBallTimer.Visible = True
            'tmrReloj = 0
            'b_VehTest = True

            'Hilo02 = New Thread(AddressOf VehTest)
            'Hilo02.Start()
            Call VehTest()

        End If

    End Sub

    Private Sub VehTest()

        Dim lStatus As String = cOpusIMCleanOBDdrv.VehiculoLink()

        b_VehTest = False
        UsrBallTimer.Visible = False

        If Mid(lStatus, 1, 4) = "Pass" Then

            Call ShowTestResult()

        Else

            lblTerminalDatos.BackColor = Color.Gray '-- OBD-ECU 
            Applog("Err:IMClean OBD fallo comunicanción con el ECU del vehículo.")
            Call rntMensajeusuario("Err:IMClean OBD fallo comunicanción con el ECU del vehículo.")

        End If

        'Hilo02.Abort()

    End Sub

    Private Sub ShowTestResult()

        lblVoltaje.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceVoltage
        lblVoltajeDLC.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceVoltageDLC

        LblVIN.Text = cOpusIMCleanOBDdrv.InspectionData.OBDdata_VIN

        Select Case cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL
            Case "1" : LblOBD_mil.BackColor = Drawing.Color.Orange '-- Luz encendida = alerta
            Case "0" : LblOBD_mil.BackColor = Drawing.Color.Green '-- 0 / 9 = ok.
            Case Else : LblOBD_mil.BackColor = Drawing.Color.Gray
        End Select

        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_MSI, 1, 1) = "1" Then LblOBD_MSI_D.BackColor = Drawing.Color.Green '-- Disponible
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_MSI, 2, 1) = "1" Then LblOBD_MSI_C.BackColor = Drawing.Color.Green '-- Completado
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_MSI, 3, 1) = "1" Then LblOBD_MSI.BackColor = Drawing.Color.Red
        If cOpusIMCleanOBDdrv.InspectionData.OBD_MSI = "110" Then LblOBD_MSI.BackColor = Drawing.Color.Green '-- Sin DTC

        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCM, 1, 1) = "1" Then LblOBD_CCM_D.BackColor = Drawing.Color.Green '-- Disponible
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCM, 2, 1) = "1" Then LblOBD_CCM_C.BackColor = Drawing.Color.Green '-- Completado
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCM, 3, 1) = "1" Then LblOBD_CCM.BackColor = Drawing.Color.Red
        If cOpusIMCleanOBDdrv.InspectionData.OBD_CCM = "110" Then LblOBD_CCM.BackColor = Drawing.Color.Green '-- Sin DTC

        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CMB, 1, 1) = "1" Then LblOBD_CMB_D.BackColor = Drawing.Color.Green '-- Disponible
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CMB, 2, 1) = "1" Then LblOBD_CMB_C.BackColor = Drawing.Color.Green '-- Completado
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CMB, 3, 1) = "1" Then LblOBD_CMB.BackColor = Drawing.Color.Red
        If cOpusIMCleanOBDdrv.InspectionData.OBD_CMB = "110" Then LblOBD_CMB.BackColor = Drawing.Color.Green '-- Sin DTC

        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 1, 1) = "1" Then LblOBD_O2S_D.BackColor = Drawing.Color.Green '-- Disponible
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 2, 1) = "1" Then LblOBD_O2S_C.BackColor = Drawing.Color.Green '-- Completado
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 3, 1) = "1" Then LblOBD_O2S.BackColor = Drawing.Color.Red
        If cOpusIMCleanOBDdrv.InspectionData.OBD_O2S = "110" Then LblOBD_O2S.BackColor = Drawing.Color.Green '-- Sin DTC

        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 1, 1) = "1" Then LblOBD_CAT_D.BackColor = Drawing.Color.Green '-- Disponible
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 2, 1) = "1" Then LblOBD_CAT_C.BackColor = Drawing.Color.Green '-- Completado
        If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 3, 1) = "1" Then LblOBD_CAT.BackColor = Drawing.Color.Red
        If cOpusIMCleanOBDdrv.InspectionData.OBD_CAT = "110" Then LblOBD_CAT.BackColor = Drawing.Color.Green '-- Sin DTC

        lblProtocolo.Text = cOpusIMCleanOBDdrv.InspectionData.OBDdata_PROTOCOLO
        lblOBDECU.Text = cOpusIMCleanOBDdrv.InspectionData.OBD_ECU

        lblDTC.Text = cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTC

        Applog("DeviceVoltage: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltage)
        Applog("DeviceVoltageDLC: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltageDLC)
        Applog(" ")
        Applog("Placa: " & ldbPlaca & "           Fecha: " & ldbFechaHora)
        Applog("Marca: " & ldbMARCA)
        Applog("SubMarca: " & ldbSubMARCA & "Modelo: " & ldbModelo)
        Applog("-------------------------------------------------------")
        Applog("OBD_ECU: " & cOpusIMCleanOBDdrv.InspectionData.OBD_ECU)
        Applog("OBDdata_PROTOCOLO: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_PROTOCOLO)
        Applog("OBDdata_VIN: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_VIN)
        Applog("OBDdata_MIL: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL)
        Applog("MSI: " & cOpusIMCleanOBDdrv.InspectionData.OBD_MSI)
        Applog("CCM: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CCM)
        Applog("CMB: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CMB)
        Applog("OS2: " & cOpusIMCleanOBDdrv.InspectionData.OBD_O2S)
        Applog("CAT: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CAT)
        Applog("CCC: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CCC)
        Applog("EVS: " & cOpusIMCleanOBDdrv.InspectionData.OBD_EVS)
        Applog("SAS: " & cOpusIMCleanOBDdrv.InspectionData.OBD_SAS)
        Applog("FAA: " & cOpusIMCleanOBDdrv.InspectionData.OBD_FAA)
        Applog("O2C: " & cOpusIMCleanOBDdrv.InspectionData.OBD_O2C)
        Applog("DTC: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTC)
        Applog(" ")
        Applog("Pid0101 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0101) '-- Monitores MIL
        Applog("Pid0300 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0300) '-- DTC
        Applog("Pid0121 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0121) '-- Distancia MIL on
        Applog("Pid0131 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0131) '-- Distancia MIL borrado
        Applog("Pid0133 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0133) '-- Presion Barometrica Kpa 
        Applog("Pid011F : " & cOpusIMCleanOBDdrv.InspectionData.Pid011F) '-- Tiempo de encendido motor
        Applog("Pid017F : " & cOpusIMCleanOBDdrv.InspectionData.Pid017F) '-- Tiempo de marcha motor
        Applog("Pid014D : " & cOpusIMCleanOBDdrv.InspectionData.Pid014D) '-- Tiempo MIL on
        Applog("Pid0951 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0951) '-- Tipo combustible
        Applog("Pid0902 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0902) '-- VIN
        Applog("Pid0904 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0904) '-- Cal ID
        Applog("Pid010C : " & cOpusIMCleanOBDdrv.InspectionData.Pid010C) '-- RPM
        Applog(" ")
        Applog("Inpección de monitores y codigos DTC en el ECU del vehículo terminada.")

        Call rntMensajeusuario("Inpección de monitores y codigos DTC en el ECU del vehículo terminada.")

        If Not cOpusIMCleanOBDdrv.InspectionData.OBD_ECU_onLine Then

            lblTerminalDatos.BackColor = Color.Gray '-- OBD-ECU gris
            Applog("Err: El OBD no esta conectado al vehículo.")
            Call rntMensajeusuario("Err: El OBD no esta conectado al vehículo.")

        Else

            lblTerminalDatos.BackColor = Color.Green '-- OBD-ECU verde

            If cOpusIMCleanOBDdrv.OBD_SimulationWarning Then

                Applog("Err: Posible simulación de lecturas del ECU detectada.")
                Call rntMensajeusuario("Err: Posible simulación de lecturas del ECU detectada.")

            End If

        End If

    End Sub

    Private Sub BtnContinuar_Click(sender As Object, e As EventArgs) Handles BtnContinuar.Click

        Me.Close()

    End Sub

    Private Sub rntMensajeusuario(ByVal pMensaje As String)
        If Mid(pMensaje, 1, 4) = "Err:" Or Mid(pMensaje, 1, 4) = "Fail" Then

            lblMensajeUsuario.Text = Mid(pMensaje, 5, Len(pMensaje))
            lblMensajeUsuario.BackColor = Drawing.Color.Red

        Else

            If Mid(pMensaje, 1, 2) = "@:" Then

                lblMensajeUsuario.Text = Mid(pMensaje, 3, Len(pMensaje))
                lblMensajeUsuario.BackColor = xMsgUsuColor

            Else

                If Mid(pMensaje, 1, 2) = "!:" Then

                    lblMensajeUsuario.Text = Mid(pMensaje, 3, Len(pMensaje))
                    lblMensajeUsuario.BackColor = Drawing.Color.Orange

                Else

                    If Mid(pMensaje, 1, 2) = "$:" Then

                        lblMensajeUsuario.Text = Mid(pMensaje, 3, Len(pMensaje))
                        lblMensajeUsuario.BackColor = Drawing.Color.Green

                    Else

                        lblMensajeUsuario.Text = pMensaje
                        lblMensajeUsuario.BackColor = Drawing.Color.Transparent

                    End If

                End If

            End If

        End If

        'Applog("MsgUsu: " & pMensaje & " | sts: " & xStatus)
        lblMensajeUsuario.Refresh()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim lStatus As String = Nothing
        'lStatus = "-  DAD_DLL_VERSION_21.10.25.0
        '-  4,TX MSG,ICAN11bt500,00 00 07 DF 01 01 
        '-  6,RX QRY,ICAN11bt500,Delay: 100
        '-  108,RX MSG,ICAN11bt500,00 00 07 DF 
        '-  110,RX MSG,ICAN11bt500,00 00 07 E8 41 01 00 07 E1 00 
        '-  110,RX MSG,ICAN11bt500,00 00 07 EA 41 01 00 04 00 00 
        '-  110,RX QRY,ICAN11bt500,Delay: 100
        '-  220,RX QRY,ICAN11bt500,Delay: 100
        '-  333,RX QRY,ICAN11bt500,Delay: 100
        '-  445,RX QRY,ICAN11bt500,Delay: 100
        '-  556,RX QRY,ICAN11bt500,Delay: 100"
        'Call cOpusIMCleanOBDdrv.tmpDECODE_Bus(lStatus)
        'ListBox1.Items.Add("mil: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL & " | FIN")
        'Call cOpusIMCleanOBDdrv.tmpDECODE_MIL(cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL)

        'lStatus = cOpusIMCleanOBDdrv.tmpSet_DataMySQL()
        'MsgBox(lStatus)

        'Timer1.Enabled = False


        'Call UsrBallTimerAlta.tick_tock()

        'Call CorreHilo()


    End Sub


    Private Sub PicESPLogo_Click(sender As Object, e As EventArgs) Handles PicESPLogo.Click

        If Button1.Visible = True Then
            Button1.Visible = False
        Else
            Button1.Visible = True
        End If

    End Sub

    Private Sub btnIniSQL_Click(sender As Object, e As EventArgs) Handles btnIniSQL.Click

        Dim lStatus As String = Nothing

        MySQLConnectionString = txtMySQLConnectionString.Text

        If Len(MySQLConnectionString) > 10 Then '-- Not IsNothing(MySQLConnectionString)

            lStatus = IniSQL()
            rntMensajeusuario(lStatus)
            If Mid(lStatus, 1, 4) = "Pass" Then
                lblTerminalDatos.BackColor = Color.Green
            Else
                lblTerminalDatos.BackColor = Color.Red
            End If

        Else
            lblTerminalDatos.BackColor = Color.Gray
        End If

    End Sub


    Public Function IniSQL() As String

        Dim lStatus As String = Nothing

        Try

            lStatus = cOpusIMCleanOBDdrv.set_ConnectionString(MySQLConnectionString)

        Catch ex As Exception

            lStatus = "Err:IniSQL | " & ex.Message

        End Try

        Applog(lStatus)
        Return lStatus

    End Function

    Private Sub btnSalvar_Click(sender As Object, e As EventArgs) Handles btnSalvar.Click

        wrReporte("-------------------------------------------------------")
        wrReporte("Placa: " & ldbPlaca)
        wrReporte("Marca: " & ldbMARCA)
        wrReporte("SubMarca: " & ldbSubMARCA)
        wrReporte("Modelo: " & ldbModelo)
        wrReporte("Fecha-Hora: " & ldbFechaHora)
        wrReporte("ECU_onLine: " & cOpusIMCleanOBDdrv.InspectionData.OBD_ECU_onLine)
        wrReporte("Posible Simulación: " & cOpusIMCleanOBDdrv.OBD_SimulationWarning)
        wrReporte("-------------------------------------------------------")
        wrReporte("OBD_ECU: " & cOpusIMCleanOBDdrv.InspectionData.OBD_ECU)
        wrReporte("OBDdata_PROTOCOLO: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_PROTOCOLO)
        wrReporte("OBDdata_VIN: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_VIN)
        wrReporte("OBDdata_MIL: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL)
        wrReporte("MSI: " & cOpusIMCleanOBDdrv.InspectionData.OBD_MSI)
        wrReporte("CCM: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CCM)
        wrReporte("CMB: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CMB)
        wrReporte("OS2: " & cOpusIMCleanOBDdrv.InspectionData.OBD_O2S)
        wrReporte("CAT: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CAT)
        wrReporte("CCC: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CCC)
        wrReporte("EVS: " & cOpusIMCleanOBDdrv.InspectionData.OBD_EVS)
        wrReporte("SAS: " & cOpusIMCleanOBDdrv.InspectionData.OBD_SAS)
        wrReporte("FAA: " & cOpusIMCleanOBDdrv.InspectionData.OBD_FAA)
        wrReporte("O2C: " & cOpusIMCleanOBDdrv.InspectionData.OBD_O2C)
        wrReporte("DTC: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTC)
        wrReporte(" ")
        wrReporte("Pid0101 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0101) '-- Monitores MIL
        wrReporte("Pid0300 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0300) '-- DTC
        wrReporte("Pid0121 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0121) '-- Distancia MIL on
        wrReporte("Pid0131 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0131) '-- Distancia MIL borrado
        wrReporte("Pid0133 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0133) '-- Presion Barometrica Kpa 
        wrReporte("Pid011F : " & cOpusIMCleanOBDdrv.InspectionData.Pid011F) '-- Tiempo de encendido motor
        wrReporte("Pid017F : " & cOpusIMCleanOBDdrv.InspectionData.Pid017F) '-- Tiempo de marcha motor
        wrReporte("Pid014D : " & cOpusIMCleanOBDdrv.InspectionData.Pid014D) '-- Tiempo MIL on
        wrReporte("Pid0951 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0951) '-- Tipo combustible
        wrReporte("Pid0902 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0902) '-- VIN
        wrReporte("Pid0904 : " & cOpusIMCleanOBDdrv.InspectionData.Pid0904) '-- Cal ID
        wrReporte("Pid010C : " & cOpusIMCleanOBDdrv.InspectionData.Pid010C) '-- RPM
        wrReporte("=======================================================")
        wrReporte(" ")

        Dim lRecData As String = ldbFechaHora & "," &
                            ldbPlaca & "," &
                            Trim(ldbMARCA) & "," &
                            Trim(ldbSubMARCA) & "," &
                            Trim(ldbModelo) & "," &
                            cOpusIMCleanOBDdrv.InspectionData.OBD_ECU & "," &
                            cOpusIMCleanOBDdrv.DeviceData.DeviceVoltageDLC & "," &
                            cOpusIMCleanOBDdrv.InspectionData.OBDdata_PROTOCOLO & "," &
                            cOpusIMCleanOBDdrv.InspectionData.OBDdata_VIN & "," &
                            cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_MSI, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_MSI, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_MSI, 3, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCM, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCM, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCM, 3, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CMB, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CMB, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CMB, 3, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 3, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 3, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCC, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCC, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCC, 3, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_EVS, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_EVS, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_EVS, 3, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_SAS, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_SAS, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_SAS, 3, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_FAA, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_FAA, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_FAA, 3, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2C, 1, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2C, 2, 1) & "," &
                            Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2C, 3, 1) & "," &
                            cOpusIMCleanOBDdrv.OBD_SimulationWarning & "," &
                            cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTC

        Call wrDataCSV(lRecData)

        Call limpiaCampos()

        lblTerminalDatos.BackColor = Color.Gray '-- OBD-ECU verde

        Call rtnBarraProceso()

        txtPlaca.Text = " "
        txtPlaca.Select()

    End Sub

    Private Sub rtnBarraProceso()

        PrgBrSalval.Visible = True
        PrgBrSalval.BackColor = Drawing.Color.Red
        PrgBrSalval.Maximum = 100
        Dim IxPrgBrSalval As Integer
        For IxPrgBrSalval = 1 To 100
            System.Threading.Thread.Sleep(10)
            PrgBrSalval.Value = IxPrgBrSalval

        Next
        System.Threading.Thread.Sleep(250)
        PrgBrSalval.Value = 0
        PrgBrSalval.Visible = False

    End Sub

    Private Sub BtnInitDevice_GotFocus(sender As Object, e As EventArgs) Handles BtnInitDevice.GotFocus
        BtnInitDevice.BackColor = xFocusColor
    End Sub

    Private Sub BtnInitDevice_LostFocus(sender As Object, e As EventArgs) Handles BtnInitDevice.LostFocus
        BtnInitDevice.BackColor = xBtnNeutro
    End Sub

    Private Sub BtnOBDtest_GotFocus(sender As Object, e As EventArgs) Handles BtnOBDtest.GotFocus
        BtnOBDtest.BackColor = xFocusColor
    End Sub

    Private Sub BtnOBDtest_LostFocus(sender As Object, e As EventArgs) Handles BtnOBDtest.LostFocus
        BtnOBDtest.BackColor = xBtnNeutro
    End Sub

    Private Sub btnSalvar_GotFocus(sender As Object, e As EventArgs) Handles btnSalvar.GotFocus
        btnSalvar.BackColor = xFocusColor
    End Sub

    Private Sub btnSalvar_LostFocus(sender As Object, e As EventArgs) Handles btnSalvar.LostFocus
        btnSalvar.BackColor = xBtnNeutro
    End Sub

    Private Sub txtPlaca_GotFocus(sender As Object, e As EventArgs) Handles txtPlaca.GotFocus
        Call rntMensajeusuario("Capture la placa del vehículo a probar.")
        txtPlaca.BackColor = xbackColorAmarillo
    End Sub

    Private Sub txtPlaca_LostFocus(sender As Object, e As EventArgs) Handles txtPlaca.LostFocus
        txtPlaca.BackColor = xCampoTexto
        ldbPlaca = txtPlaca.Text

    End Sub

    Private Sub RntCargaTablas()

        Try

            Call CargarMarca()

            Call CargarSubMarca()

        Catch ex As Exception

            Call rntMensajeusuario("Err:Falla al cargar las tablas del CATALOGO VEHICULAR (" & ex.Message & ")")
            Applog("Err: RntCargaTablas | Falla al cargar las tablas del CATALOGO VEHICULAR (" & ex.Message & ")")

        End Try

    End Sub

    Private Sub CargarMarca()

        Try

            Dim Ztx, Vtx As Integer
            Dim Ttx, Ftx As String

            Dim xFileTXT As String = "c:\opus_prog\tablas\tb_marca.dat"
            Dim Apunt As String = FileSystem.FreeFile()

            'Dim ldbMARCA As Integer '= cDBS.dbVEHICULO.dbMARCA

            cbxMarca.DisplayMember = "Text"
            cbxMarca.ValueMember = "Value"
            Dim tbMarca As New DataTable
            tbMarca.Columns.Add("Text", GetType(String))
            tbMarca.Columns.Add("Value", GetType(Integer))

            FileSystem.FileOpen(Apunt, xFileTXT, OpenMode.Input, OpenAccess.Read)
            Do While Not EOF(Apunt)
                Ftx = FileSystem.LineInput(Apunt)
                Ztx = Len(Ftx)
                Vtx = Val(Mid(Ftx, 1, 4))
                Ttx = Mid(Ftx, 6, Ztx)

                If Vtx < 200 Then tbMarca.Rows.Add(Ttx, Vtx)

            Loop
            FileSystem.FileClose(Apunt)
            cbxMarca.DataSource = tbMarca

            cbxMarca.Refresh()

        Catch ex As Exception
            MsgBox("Err:CargarSubMarca | " & ex.Message)
        End Try

    End Sub

    Private Sub CargarSubMarca()

        Try

            Dim Ztx, V1tx, V2tx As Integer
            Dim Ttx, Ftx As String

            'Dim ldbSubMARCA As Integer '= cDBS.dbVEHICULO.dbSUBMARCA

            Dim xFileTXT As String = "c:\opus_prog\tablas\tb_SubMarca.dat"
            Dim Apunt As String = FileSystem.FreeFile()

            cbxSubMarca.DisplayMember = "Text"
            cbxSubMarca.ValueMember = "Value"
            Dim tbSubMarca As New DataTable
            tbSubMarca.Columns.Add("Text", GetType(String))
            tbSubMarca.Columns.Add("Value", GetType(Integer))

            FileSystem.FileOpen(Apunt, xFileTXT, OpenMode.Input, OpenAccess.Read)

            Do While Not EOF(Apunt)
                Ftx = FileSystem.LineInput(Apunt)
                Ztx = Len(Ftx)
                V1tx = Val(Mid(Ftx, 1, 4))
                V2tx = Val(Mid(Ftx, 6, 4))
                Ttx = Mid(Ftx, 11, Ztx)

                If V1tx = Val(Mid(ldbMARCA, 1, 4)) Then tbSubMarca.Rows.Add(Ttx, V2tx)

            Loop
            FileSystem.FileClose(Apunt)

            cbxSubMarca.DataSource = tbSubMarca

            cbxSubMarca.Refresh()

        Catch ex As Exception
            MsgBox("Err:CargarSubMarca | " & ex.Message)
        End Try

    End Sub

    Private Sub cbxMarca_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxMarca.SelectedIndexChanged
        '-- nada
    End Sub

    Private Sub cbxMarca_GotFocus(sender As Object, e As EventArgs) Handles cbxMarca.GotFocus
        cbxMarca.BackColor = xFocusColor
        Call rntMensajeusuario("Seleccione la marca del vehículo.")

    End Sub

    Private Sub cbxMarca_LostFocus(sender As Object, e As EventArgs) Handles cbxMarca.LostFocus
        cbxMarca.BackColor = xCampoTexto
        ldbMARCA = Format(cbxMarca.SelectedValue, "000") & " " & cbxMarca.Text

        Call CargarSubMarca()

    End Sub

    Private Sub cbxMarca_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbxMarca.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Return) Or e.KeyChar = Convert.ToChar(Keys.Tab) Then
            cbxSubMarca.Select()
        End If
    End Sub

    Private Sub cbxSubMarca_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbxSubMarca.SelectedIndexChanged
        '-- nada
    End Sub

    Private Sub cbxSubMarca_GotFocus(sender As Object, e As EventArgs) Handles cbxSubMarca.GotFocus
        cbxSubMarca.BackColor = xFocusColor
        Call rntMensajeusuario("Seleccione la sub-marca del vehículo.")

    End Sub

    Private Sub cbxSubMarca_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbxSubMarca.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Return) Then
            txtModelo.Select()

        End If

    End Sub

    Private Sub cbxSubMarca_LostFocus(sender As Object, e As EventArgs) Handles cbxSubMarca.LostFocus
        cbxSubMarca.BackColor = xCampoTexto
        ldbSubMARCA = Format(cbxSubMarca.SelectedValue, "000") & " " & cbxSubMarca.Text

    End Sub

    Private Sub txtPlaca_TextChanged(sender As Object, e As EventArgs) Handles txtPlaca.TextChanged
        '-- nada
    End Sub

    Private Sub txtPlaca_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPlaca.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Return) Then
            cbxMarca.Select()

        End If
    End Sub

    Private Sub txtModelo_TextChanged(sender As Object, e As EventArgs) Handles txtModelo.TextChanged
        '-- nada
    End Sub

    Private Sub txtModelo_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtModelo.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Return) Then
            BtnOBDtest.Select()

        End If
    End Sub

    Private Sub txtModelo_LostFocus(sender As Object, e As EventArgs) Handles txtModelo.LostFocus
        txtModelo.BackColor = xCampoTexto
        ldbModelo = txtModelo.Text
    End Sub

    Private Sub txtModelo_GotFocus(sender As Object, e As EventArgs) Handles txtModelo.GotFocus
        txtModelo.BackColor = xFocusColor
    End Sub

End Class
