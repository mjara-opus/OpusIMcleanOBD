Public Class IMCleanOBD

    Public Opus_KeyDevicePass As Boolean

    Public Structure srtDeviceData

        Public DeviceID As String
        Public DeviceHardwareIDs As String
        Public DeviceManufacturer As String
        Public DeviceDescription As String
        Public DeviceFirmwareVersion As String

        Public DeviceType As Integer
        Public DeviceVoltage As Single
        Public DeviceVoltageDLC As Single

    End Structure
    Public DeviceData As srtDeviceData

    Public Structure srtInspectionData

        Public OBDdata_VINhx As String
        Public OBDdata_VIN As String
        Public OBDdata_MILhx As String
        Public OBDdata_MIL As String
        Public OBDdata_DTChx As String
        Public OBDdata_DTC As String

        Public OBDdata_PROTOCOLO As String

        Public OBD_MSI As String '-- Sistema de detección de condiciones inadecuadas de ignición de cilindros ' LrdOBD_cilin
        Public OBD_CCM As String '-- Sistema de eficiencia del convertidor catalitico 
        Public OBD_CMB As String '-- Sistema de combustible 
        Public OBD_O2S As String '-- Sistema de sensores de oxigeno 
        Public OBD_CAT As String '-- Sistema de componentes integrales 

        Public OBD_CCC As String '-- Sistema de calentamiento del convertidor catalitico
        Public OBD_EVS As String '-- Sistema evaporativo
        Public OBD_SAS As String '-- Sistema secundario de aire
        Public OBD_FAA As String '-- Sistema de fugas de aire acondicionado
        Public OBD_O2C As String '-- Sistema de calentamiento del sensor de oxigeno

    End Structure
    Public InspectionData As srtInspectionData

    Private Sub SetData()

        InspectionData.OBDdata_VINhx = OBDdata_VIN
        InspectionData.OBDdata_VIN = OBDdata_VINtxt
        InspectionData.OBDdata_MILhx = OBDdata_MIL
        InspectionData.OBDdata_MIL = OBDdata_MILtxt
        InspectionData.OBDdata_DTChx = OBDdata_DTC
        InspectionData.OBDdata_DTC = OBDdata_DTCtxt

        InspectionData.OBD_MSI = LrdOBD_MSI
        InspectionData.OBD_CCM = LrdOBD_CCM
        InspectionData.OBD_CMB = LrdOBD_CMB
        InspectionData.OBD_O2S = LrdOBD_O2S
        InspectionData.OBD_CAT = LrdOBD_CAT

        InspectionData.OBD_CCC = LrdOBD_CCC
        InspectionData.OBD_EVS = LrdOBD_EVS
        InspectionData.OBD_SAS = LrdOBD_SAS
        InspectionData.OBD_FAA = LrdOBD_FAA
        InspectionData.OBD_O2C = LrdOBD_O2C

        InspectionData.OBDdata_PROTOCOLO = OBDdata_PROTOCOLO

    End Sub



    Public Function set_ConnectionString(ByVal pVIDB_ConnectionString As String)

        Dim lStatus As String = Nothing

        xVIDB_ConnectionString = pVIDB_ConnectionString

        If xVIDB_ConnectionString = "OPUS1234" Then
            xOpus_VIDBLinkOnline = True
            lStatus = "Pass:set_ConnectionString"
        Else
            xOpus_VIDBLinkOnline = False
            lStatus = "Err:set_ConnectionString"
        End If

        Applog(lStatus)
        Return lStatus
    End Function


    Public Function ReviewDevicePlug() As String
        Dim lStatus As String = Nothing

        DeviceData.DeviceID = "Null"
        DeviceData.DeviceHardwareIDs = "Null"
        DeviceData.DeviceManufacturer = "Null"
        DeviceData.DeviceDescription = "Null"
        Opus_KeyDevicePass = False

        lStatus = lFindUSBDevices()

        If Mid(lStatus, 1, 4) = "Pass" Then

            If System.IO.File.Exists(xLocalKEYfile) Then

                If Mid(getOpusKeyFile(), 1, 4) = "Pass" Then '-- Se valida la licencia 

                    Opus_KeyDevicePass = True
                    DeviceData.DeviceID = strDeviceInfo.DeviceID
                    DeviceData.DeviceHardwareIDs = strDeviceInfo.DeviceHardwareIDs
                    DeviceData.DeviceManufacturer = strDeviceInfo.DeviceManufacturer
                    DeviceData.DeviceDescription = strDeviceInfo.DeviceDescription
                    DeviceData.DeviceFirmwareVersion = sFirmwareVersion
                    lStatus = "Pass:IMClean OBD conectado."

                Else

                    lStatus = "Key? " & strDeviceInfo.DeviceID & "-" & getMacAddress(0)

                End If

            Else

                lStatus = "Key? " & strDeviceInfo.DeviceID & "-" & getMacAddress(0)

            End If

        Else

            lStatus = "Err:IMClean OBD desconectado."

        End If

        Return lStatus

    End Function


    Public Function InitIMCleanDevice() As String
        Dim lStatus As String = Nothing

        If Len(strDeviceInfo.DeviceID) < 5 Then

            lStatus = lFindUSBDevices()

            If Mid(lStatus, 1, 4) = "Pass" Then
                DeviceData.DeviceID = strDeviceInfo.DeviceID
                DeviceData.DeviceHardwareIDs = strDeviceInfo.DeviceHardwareIDs
                DeviceData.DeviceManufacturer = strDeviceInfo.DeviceManufacturer
                DeviceData.DeviceDescription = strDeviceInfo.DeviceDescription
                DeviceData.DeviceFirmwareVersion = sFirmwareVersion
                DeviceData.DeviceType = sDeviceType
                DeviceData.DeviceVoltage = sDeviceVoltage
                DeviceData.DeviceVoltageDLC = sDeviceVoltageDLC

            End If
        Else
            lStatus = "Pass"
        End If

        If Mid(lStatus, 1, 4) = "Pass" Then

            Call lInitIMCleanDevice()
            DeviceData.DeviceType = sDeviceType
            DeviceData.DeviceFirmwareVersion = sFirmwareVersion
            DeviceData.DeviceVoltage = sDeviceVoltage
            DeviceData.DeviceVoltageDLC = sDeviceVoltageDLC

            lStatus = "Pass:InitIMCleanDevice | IMClean OBD online"
        Else

            lStatus = "Err:InitIMCleanDevice | IMClean OBD line off"
            DeviceData.DeviceType = 0
            DeviceData.DeviceFirmwareVersion = "Null"
            DeviceData.DeviceVoltage = 0
            DeviceData.DeviceVoltageDLC = 0

        End If

        Return lStatus
    End Function

    Public Function VehiculoLink() As String

        Dim lStatus As String = Nothing

        lStatus = lVehiculoLink()
        Call SetData()

        Return lStatus
    End Function

    Public Sub tmpDECODE_Bus(ByVal pDato As String)

        Call DECODE_Bus(pDato)
        Call SetData()

    End Sub


    Public Sub tmpDECODE_MIL(ByVal pDato As String)

        Call DECODE_MIL(pDato)
        Call SetData()

    End Sub

    Public Function set_OpusKeyDevice(ByVal pOpusKey As String) As String

        Return lset_OpusKeyDevice(pOpusKey)

    End Function

    Public Function DriverVersion() As String
        '-- Leemos el número de versión de la aplicación 

        Dim x1 As String = My.Application.Info.Version.Major
        Dim x2 As String = My.Application.Info.Version.Minor
        Dim x3 As String = My.Application.Info.Version.Build
        'Dim x4 As String = My.Application.Info.AssemblyName

        Return "v." & x1 & "." & x2 & "." & x3 '& "." & x4

    End Function


End Class
