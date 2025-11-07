Public Class Form1

    Private Sub frmOBDtest_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Location = New Drawing.Point(50, 50)

        ListBox1.HorizontalScrollbar = True
        Timer1.Enabled = True

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        lblFechaHora.Text = Format(Now, "dd-MMM-yyyy - HH:mm:ss")

    End Sub

    Private Sub BtnInitDevice_Click(sender As Object, e As EventArgs) Handles BtnInitDevice.Click

        ListBox1.Items.Clear()

        Call initDeviceOBD()

        ListBox1.Items.Add("DeviceData ")
        ListBox1.Items.Add("-------------------------------------------------------")
        ListBox1.Items.Add("DeviceDescription: " & cOpusIMCleanOBDdrv.DeviceData.DeviceDescription)
        ListBox1.Items.Add("DeviceID: " & cOpusIMCleanOBDdrv.DeviceData.DeviceID)
        ListBox1.Items.Add("DeviceManufacturer: " & cOpusIMCleanOBDdrv.DeviceData.DeviceManufacturer)
        ListBox1.Items.Add("DeviceType: " & cOpusIMCleanOBDdrv.DeviceData.DeviceType)
        ListBox1.Items.Add("DeviceVoltage: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltage)
        ListBox1.Items.Add("DeviceVoltageDLC: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltageDLC)

    End Sub

    Private Sub initDeviceOBD()

        Dim lStatus As String = Nothing

        Call rntMensajeusuario("Iniciando conexión con el IMClean OBD...")
        ListBox1.Items.Add("Iniciando conexión con el IMClean OBD...")

        lStatus = cOpusIMCleanOBDdrv.ReviewDevicePlug()

        If Mid(lStatus, 1, 4) = "Key?" Then '-- requerimiento de licencia

            Call rntMensajeusuario("!:Opus IMClean OBD requiere licencia usuario." & lStatus) ' Opus IMClean OBD requiere licencia usuario.")
            ListBox1.Items.Add("Opus IMClean OBD requiere licencia usuario.") '"Opus IMClean OBD licencia no valida o expirada.")
            ListBox1.Items.Add(lStatus)

            lStatus = InputBox(Mid(lStatus, 5, 50), "Ingrese Licencia usuario.")

            lStatus = cOpusIMCleanOBDdrv.set_OpusKeyDevice(lStatus)

            MsgBox(lStatus)

        Else

            ListBox1.Items.Add("initDeviceOBD: " & lStatus)

            lblDeviceDescription.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceDescription
            lblIdDevice.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceID

            If Mid(lStatus, 1, 4) = "Pass" Then

                ListBox1.Items.Add("Licencia: " & Year(Now) & Chr(174) & " Ok")
                ListBox1.Items.Add("DeviceDescription: " & cOpusIMCleanOBDdrv.DeviceData.DeviceDescription)
                ListBox1.Items.Add("DeviceID: " & cOpusIMCleanOBDdrv.DeviceData.DeviceID)
                ListBox1.Items.Add(" ")
                ListBox1.Items.Add("IMClean OBD en línea, Iniciando dispositivo ...")

                Call rntMensajeusuario("IMClean OBD en línea, Iniciando dispositivo ...")
                lStatus = cOpusIMCleanOBDdrv.InitIMCleanDevice()

                If Mid(lStatus, 1, 4) = "Pass" Then

                    PicOBD.Visible = True
                    BtnOBDtest.Enabled = True
                    lStatus = "Opus IMClean OBD en línea."

                    lblVoltaje.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceVoltage
                    lblVoltajeDLC.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceVoltageDLC
                    lblFirmWare.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceFirmwareVersion

                End If

            End If

        End If

        ListBox1.Items.Add(lStatus)
        Call rntMensajeusuario(lStatus)

    End Sub

    Private Sub BtnOBDtest_Click(sender As Object, e As EventArgs) Handles BtnOBDtest.Click

        Dim lStatus As String = Nothing

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

        Call rntMensajeusuario("IMClean OBD comunicando con el ECU del vehículo, espere un momento...")

        'Me.Refresh()

        lStatus = cOpusIMCleanOBDdrv.VehiculoLink()

        If Mid(lStatus, 1, 4) = "Pass" Then

            LblVIN.Text = cOpusIMCleanOBDdrv.InspectionData.OBDdata_VIN

            If cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL = "1" Then
                LblOBD_mil.BackColor = Drawing.Color.Orange '-- Luz encendida = alerta
            Else
                LblOBD_mil.BackColor = Drawing.Color.Green '-- 0 / 9 = ok.
            End If

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
            If cOpusIMCleanOBDdrv.InspectionData.OBD_CMB = "110" Then LblOBD_MSI.BackColor = Drawing.Color.Green '-- Sin DTC

            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 1, 1) = "1" Then LblOBD_O2S_D.BackColor = Drawing.Color.Green '-- Disponible
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 2, 1) = "1" Then LblOBD_O2S_C.BackColor = Drawing.Color.Green '-- Completado
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 3, 1) = "1" Then LblOBD_O2S.BackColor = Drawing.Color.Red
            If cOpusIMCleanOBDdrv.InspectionData.OBD_O2S = "110" Then LblOBD_MSI.BackColor = Drawing.Color.Green '-- Sin DTC

            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 1, 1) = "1" Then LblOBD_CAT_D.BackColor = Drawing.Color.Green '-- Disponible
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 2, 1) = "1" Then LblOBD_CAT_C.BackColor = Drawing.Color.Green '-- Completado
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 3, 1) = "1" Then LblOBD_CAT.BackColor = Drawing.Color.Red
            If cOpusIMCleanOBDdrv.InspectionData.OBD_CAT = "110" Then LblOBD_CAT.BackColor = Drawing.Color.Green '-- Sin DTC

            lblProtocolo.Text = cOpusIMCleanOBDdrv.InspectionData.OBDdata_PROTOCOLO

            lblDTC.Text = cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTC

            ListBox1.Items.Add("DeviceVoltage: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltage)
            ListBox1.Items.Add("DeviceVoltageDLC: " & cOpusIMCleanOBDdrv.DeviceData.DeviceVoltageDLC)
            ListBox1.Items.Add(" ")
            ListBox1.Items.Add("InspectionData")
            ListBox1.Items.Add("-------------------------------------------------------")
            ListBox1.Items.Add("OBDdata_PROTOCOLO: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_PROTOCOLO)
            ListBox1.Items.Add("OBDdata_VINhx: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_VINhx)
            ListBox1.Items.Add("OBDdata_VIN: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_VIN)
            ListBox1.Items.Add("OBDdata_MILhx: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_MILhx)
            ListBox1.Items.Add("OBDdata_MIL: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL)
            ListBox1.Items.Add("MSI: " & cOpusIMCleanOBDdrv.InspectionData.OBD_MSI)
            ListBox1.Items.Add("CCM: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CCM)
            ListBox1.Items.Add("CMB: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CMB)
            ListBox1.Items.Add("OS2: " & cOpusIMCleanOBDdrv.InspectionData.OBD_O2S)
            ListBox1.Items.Add("CAT: " & cOpusIMCleanOBDdrv.InspectionData.OBD_CAT)
            ListBox1.Items.Add("DTChx: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTChx)
            ListBox1.Items.Add("DTC: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTC)

            Call rntMensajeusuario("Inpección de monitores y codigos DTC en el ECU del vehículo terminada.")

        Else

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

        Dim lStatus As String

        lStatus = "-  DAD_DLL_VERSION_21.10.25.0
                    -  4,TX MSG,ICAN11bt500,00 00 07 DF 01 01 
                    -  6,RX QRY,ICAN11bt500,Delay: 100
                    -  108,RX MSG,ICAN11bt500,00 00 07 DF 
                    -  110,RX MSG,ICAN11bt500,00 00 07 E8 41 01 00 07 E1 00 
                    -  110,RX MSG,ICAN11bt500,00 00 07 EA 41 01 00 04 00 00 
                    -  110,RX QRY,ICAN11bt500,Delay: 100
                    -  220,RX QRY,ICAN11bt500,Delay: 100
                    -  333,RX QRY,ICAN11bt500,Delay: 100
                    -  445,RX QRY,ICAN11bt500,Delay: 100
                    -  556,RX QRY,ICAN11bt500,Delay: 100"

        Call cOpusIMCleanOBDdrv.tmpDECODE_Bus(lStatus)

        ListBox1.Items.Add("mil: " & cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL & " | FIN")
        'Call cOpusIMCleanOBDdrv.tmpDECODE_MIL(cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL)


    End Sub

    Private Sub PicESPLogo_Click(sender As Object, e As EventArgs) Handles PicESPLogo.Click

        If Button1.Visible = True Then
            Button1.Visible = False
        Else
            Button1.Visible = True
        End If
    End Sub

End Class
