Public Class IMCleanOBD

    Public Structure srtDeviceData

        Public DeviceID As String
        Public DeviceHardwareIDs As String
        Public DeviceManufacturer As String
        Public DeviceDescription As String
        Public DeviceFirmwareVersion As String

        Public DeviceType As Integer
        Public FirmwareVersion As String
        Public DeviceVoltage As Single
        Public DeviceVoltageDLC As Single

    End Structure
    Public DeviceData As srtDeviceData

    Public Structure srtInspectionData

        Public OBDdata_VIN As String
        Public OBDdata_VINtxt As String
        Public OBDdata_MIL As String
        Public OBDdata_MILtxt As String
        Public OBDdata_DTC As String
        Public OBDdata_DTCtxt As String

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

        InspectionData.OBDdata_VIN = OBDdata_VIN
        InspectionData.OBDdata_VINtxt = OBDdata_VINtxt
        InspectionData.OBDdata_MIL = OBDdata_MIL
        InspectionData.OBDdata_MILtxt = OBDdata_MILtxt
        InspectionData.OBDdata_DTC = OBDdata_DTC
        InspectionData.OBDdata_DTCtxt = OBDdata_DTCtxt

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

        InspectionData.OBDdata_DTCtxt = OBDdata_DTCtxt

    End Sub

    Public Function set_OpusKeyDevice(ByVal pKeyDevice As String)

        Dim lStatus As String = Nothing

        xOpus_KeyDevice = pKeyDevice

        If xOpus_KeyDevice = "OPUS1234" Then
            xOpus_KeyDevicePass = True
            lStatus = "Pass:set_OpusKeyDevice"
        Else
            xOpus_KeyDevicePass = False
            lStatus = "Err:set_OpusKeyDevice"
        End If

        Applog(lStatus)
        Return lStatus
    End Function

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

        lStatus = lFindUSBDevices()

        If Mid(lStatus, 1, 4) = "Pass" Then

            DeviceData.DeviceID = strDeviceInfo.DeviceID
            DeviceData.DeviceHardwareIDs = strDeviceInfo.DeviceHardwareIDs
            DeviceData.DeviceManufacturer = strDeviceInfo.DeviceManufacturer
            DeviceData.DeviceDescription = strDeviceInfo.DeviceDescription
            lStatus = "Pass:ReviewDevicePlug"

        Else

            DeviceData.DeviceID = "Null"
            DeviceData.DeviceHardwareIDs = "Null"
            DeviceData.DeviceManufacturer = "Null"
            DeviceData.DeviceDescription = "Null"
            lStatus = "Err:ReviewDevicePlug"

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

            End If
        Else
            lStatus = "Pass"
        End If

        If Mid(lStatus, 1, 4) = "Pass" Then

            Call lInitIMCleanDevice()
            DeviceData.DeviceType = sDeviceType
            DeviceData.FirmwareVersion = sFirmwareVersion
            DeviceData.DeviceVoltage = sDeviceVoltage
            DeviceData.DeviceVoltageDLC = sDeviceVoltageDLC

            lStatus = "Pass:InitIMCleanDevice"
        Else

            lStatus = "Err:InitIMCleanDevice"
            DeviceData.DeviceType = 0
            DeviceData.FirmwareVersion = "Null"
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


End Class
