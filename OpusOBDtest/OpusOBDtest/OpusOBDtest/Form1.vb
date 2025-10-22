Public Class Form1

    Private Sub frmOBDtest_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        Timer1.Enabled = True

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        lblFechaHora.Text = Format(Now, "dd-MMM-yyyy - HH:mm:ss")

    End Sub

    Private Sub BtnInitDevice_Click(sender As Object, e As EventArgs) Handles BtnInitDevice.Click

        Call initDeviceOBD()

    End Sub

    Private Sub initDeviceOBD()

        Dim lStatus As String = Nothing

        Call rntMensajeusuario("Iniciando conexión con el IMClean OBD...")

        lStatus = cOpusIMCleanOBDdrv.ReviewDevicePlug()

        lblDeviceDescription.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceDescription
        lblIdDevice.Text = cOpusIMCleanOBDdrv.DeviceData.DeviceID

        'Call rntMensajeusuario(lStatus)
        If Mid(lStatus, 1, 4) = "Pass" Then

            Call rntMensajeusuario("IMClean OBD en línea, Iniciando dispositivo ...")
            lStatus = cOpusIMCleanOBDdrv.InitIMCleanDevice()

            If Mid(lStatus, 1, 4) = "Pass" Then
                PicOBD.Visible = True
                lStatus = "Opus IMClean OBD en línea."
            Else

            End If
            Call rntMensajeusuario(lStatus)

        End If


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

            LblVIN.Text = cOpusIMCleanOBDdrv.InspectionData.OBDdata_VINtxt

            If cOpusIMCleanOBDdrv.InspectionData.OBDdata_MIL = "1" Then
                LblOBD_mil.BackColor = Drawing.Color.Orange '-- Luz encendida = alerta
            Else
                LblOBD_mil.BackColor = Drawing.Color.Green '-- 0 / 9 = ok.
            End If

            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_MSI, 1, 1) = "1" Then LblOBD_MSI_D.BackColor = Drawing.Color.Green '-- Disponible
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_MSI, 2, 1) = "1" Then LblOBD_MSI_C.BackColor = Drawing.Color.Green '-- Completado
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_MSI, 3, 1) = "0" Then LblOBD_MSI.BackColor = Drawing.Color.Green '-- Sin DTC

            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCM, 1, 1) = "1" Then LblOBD_CCM_D.BackColor = Drawing.Color.Green '-- Disponible
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCM, 2, 1) = "1" Then LblOBD_CCM_C.BackColor = Drawing.Color.Green '-- Completado
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CCM, 3, 1) = "0" Then LblOBD_CCM.BackColor = Drawing.Color.Green '-- Sin DTC

            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CMB, 1, 1) = "1" Then LblOBD_CMB_D.BackColor = Drawing.Color.Green '-- Disponible
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CMB, 2, 1) = "1" Then LblOBD_CMB_C.BackColor = Drawing.Color.Green '-- Completado
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CMB, 3, 1) = "0" Then LblOBD_CMB.BackColor = Drawing.Color.Green '-- Sin DTC

            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 1, 1) = "1" Then LblOBD_O2S_D.BackColor = Drawing.Color.Green '-- Disponible
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 2, 1) = "1" Then LblOBD_O2S_C.BackColor = Drawing.Color.Green '-- Completado
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_O2S, 3, 1) = "0" Then LblOBD_O2S.BackColor = Drawing.Color.Green '-- Sin DTC

            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 1, 1) = "1" Then LblOBD_CAT_D.BackColor = Drawing.Color.Green '-- Disponible
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 2, 1) = "1" Then LblOBD_CAT_C.BackColor = Drawing.Color.Green '-- Completado
            If Mid(cOpusIMCleanOBDdrv.InspectionData.OBD_CAT, 3, 1) = "0" Then LblOBD_CAT.BackColor = Drawing.Color.Green '-- Sin DTC

            txtDTC.Text = cOpusIMCleanOBDdrv.InspectionData.OBDdata_DTCtxt

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



End Class
