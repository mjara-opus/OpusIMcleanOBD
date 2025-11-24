
Imports System.Threading

Public Class Form1

    Public Hilo01 As Thread
    Public tmrReloj As Integer
    Private Sub frmOBDtest_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim lStatus As String = Nothing

        Me.Location = New Drawing.Point(5, 5)

        UsrBallTimer.wTime = 150

        ListBox1.HorizontalScrollbar = True

        tmrReloj = 0
        Timer1.Enabled = True

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        lblFechaHora.Text = Format(Now, "dd-MMM-yyyy - HH:mm:ss")

        If cOpusIMCleanOBDdrv.DeviceData.DeviceLinkOn Then

            PicOBD.Visible = True
            UsrBallTimer.Visible = False
            BtnInitDevice.Enabled = False
            BtnOBDtest.Enabled = True

        Else

            If b_initDeviceOBD Then

                UsrBallTimer.Visible = True
                tmrReloj += 1
                If tmrReloj < 150 Then Call Me.UsrBallTimer.tick_tock()

            End If

        End If

    End Sub

    Private Sub BtnInitDevice_Click(sender As Object, e As EventArgs) Handles BtnInitDevice.Click

        ListBox1.Items.Clear()

        tmrReloj = 0
        Call ComunicaDeviceOBD()

    End Sub

    Private Sub ComunicaDeviceOBD()

        Dim lStatus As String = "Iniciando conexión con el IMClean OBD..." & Format(Now, "HH:mm:ss")

        Call rntMensajeusuario(lStatus)
        Applog(lStatus)

        lStatus = cOpusIMCleanOBDdrv.ReviewDevicePlug()

        If Mid(lStatus, 1, 4) = "Key?" Then '-- requerimiento de licencia

            Call rntMensajeusuario("!:Opus IMClean OBD requiere licencia usuario." & lStatus) ' Opus IMClean OBD requiere licencia usuario.")
            Applog("Opus IMClean OBD requiere licencia usuario.") '"Opus IMClean OBD licencia no valida o expirada.")
            Applog(lStatus)

            lStatus = InputBox(Mid(lStatus, 5, 50), "Ingrese Licencia usuario.")

            lStatus = cOpusIMCleanOBDdrv.set_OpusKeyDevice(lStatus)

            MsgBox(lStatus)

        Else

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
        'Call rntMensajeusuario(lStatus)

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

    Private Sub BtnOBDtest_Click(sender As Object, e As EventArgs) Handles BtnOBDtest.Click

        ListBox1.Items.Clear()
        lblProtocolo.Text = " "
        LblVIN.Text = " "
        lblDTC.Text = " "


        Dim lStatus As String = "IMClean OBD comunicando con el ECU del vehículo, espere un momento..."

        '//-- 009 - Valor nulo = Monitor no disponible, no completado, sin DTC.  
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
        '||-- 009 - Valor nulo = Monitor no disponible, no completado, sin DTC.  

        Me.Refresh()

        Applog(lStatus)
        Call rntMensajeusuario(lStatus)

        lStatus = cOpusIMCleanOBDdrv.VehiculoLink()

        If Mid(lStatus, 1, 4) = "Pass" Then

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

            lblDTC.Text = cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTC

            Applog("DeviceVoltage: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltage)
            Applog("DeviceVoltageDLC: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltageDLC)
            Applog(" ")
            Applog("InspectionData")
            Applog("-------------------------------------------------------")
            Applog("OBDdata_PROTOCOLO: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_PROTOCOLO)
            Applog("OBDdata_VINhx: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_VINhx)
            Applog("OBDdata_VIN: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_VIN)
            Applog("OBDdata_MILhx: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_MILhx)
            Applog("OBDdata_MIL: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL)
            Applog("MSI: " & cOpusIMCleanOBDdrv.InspectionData.OBD_MSI)
            Applog("CCM: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CCM)
            Applog("CMB: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CMB)
            Applog("OS2: " & cOpusIMCleanOBDdrv.InspectionData.OBD_O2S)
            Applog("CAT: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CAT)
            Applog("DTChx: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTChx)
            Applog("DTC: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTC)
            Applog(" ")
            Applog("Inpección de monitores y codigos DTC en el ECU del vehículo terminada.")

            Call rntMensajeusuario("Inpección de monitores y codigos DTC en el ECU del vehículo terminada.")

            If cOpusIMCleanOBDdrv.OBD_SimulationWarning Then

                Applog("Err: Posible simulación de lecturas del ECU detectada.")
                Call rntMensajeusuario("Err: Posible simulación de lecturas del ECU detectada.")

            End If

        Else

            Applog("Err:IMClean OBD fallo comunicanción con el ECU del vehículo.")
            Call rntMensajeusuario("Err:IMClean OBD fallo comunicanción con el ECU del vehículo.")

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


End Class
