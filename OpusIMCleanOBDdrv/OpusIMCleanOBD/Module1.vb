Imports System.IO
Imports System.Xml

Imports DT
Imports MySql.Data.MySqlClient
Imports Microsoft.SqlServer.Server
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

'---------------------------------------------------------------------------------------------------------------------
'-- 06.11.25  Se agregan chkSumCADD(), CodeDD(), DecodeDD(), genera Chksum-licencia de versión del DLL, vigencia anual
'-- 07.11.25  Se agregan calcCRC(), calcCRCNS(), facNS, facFH para generar CHKSUM por registros Hx de la prueba 
'---------------------------------------------------------------------------------------------------------------------

Module Module1

    Public MyDad As DT.DAD.IMClean = Nothing

    Public MyLastCommResult As DrewTech.IIMClean.DT_Com_Result = DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect
    Public MyLastConnect As DateTime = DateTime.MinValue
    Public MyLastReConnect As DateTime = DateTime.MinValue
    Public MyDeviceLogSaveStatus As String = "?"
    Public MyLastExtendedStatus As OBDLinkExtendedStatus = OBDLinkExtendedStatus.Unknown

    Public sDeviceType As Integer
    Public sFirmwareVersion As String
    Public sDeviceVoltage As Single
    Public sDeviceVoltageDLC As Single

    Public MyIMCleanDriverDLLVersion As String = ""

    Public MyLastFunctionSeconds As Double = -1
    Public MyCurrentSerialDataLog As String = ""
    Public MyCurrentSerialDataLogSize As Long = 0
    Public MyCurrentSerialDataLogIsCommandLog As Boolean = False

    Public MyCurrentVendorDataLog As String = ""
    Public MyCurrentVendorDataLogSize As Long = 0

    Public MyDeviceLogClear As Boolean = True
    Public MyDeviceLogClearStatus As String = "?"
    Public MyDeviceLogStatusDateFormat As String = "yyyy-MM-dd_HH:mm:ss"
    Public MyLastClearLogs As DateTime = DateTime.MinValue
    Public MyDeviceLogSave As Boolean = False

    Const OBDLINK_FIRMWARE_CONFIG As String = "ObdLink"
    Const OBDLINK_FIRMWARE_KEY_BASE As String = "Firmware"

    Public Const OBD_PROTOCOL_NAME_NONE As String = "None"
    Public Const OBD_PROTOCOL_NAME_NOTLINKED As String = "NotLinked"
    Public Const OBD_PROTOCOL_NAME_PWM As String = "PWM"
    Public Const OBD_PROTOCOL_NAME_VPW As String = "VPW"
    Public Const OBD_PROTOCOL_NAME_ISO9141 As String = "ISO"
    Public Const OBD_PROTOCOL_NAME_KWP As String = "KWD 2000"
    Public Const OBD_PROTOCOL_NAME_KWP_SLOW As String = "KWD 2000-S"
    Public Const OBD_PROTOCOL_NAME_KWP_FAST As String = "KWD 2000-F"
    Public Const OBD_PROTOCOL_NAME_CAN As String = "CAN"
    Public Const OBD_PROTOCOL_NAME_CAN11 As String = "CAN11"
    Public Const OBD_PROTOCOL_NAME_CAN29 As String = "CAN29"
    Public Const OBD_PROTOCOL_NAME_CAN11_250 As String = "CAN11-250"
    Public Const OBD_PROTOCOL_NAME_CAN29_250 As String = "CAN29-250"

    Public Const OBD_VOLTAGE_FAILURE As Single = -3.0
    Public Const OBD_VOLTAGE_BUSY As Single = -8.0
    Public Const OBD_VOLTAGE_UPDATE_REQUIRED As Single = -9.0

    Public Const OBDLINK_TYPE_9020WIRED As Integer = 0
    Public Const OBDLINK_TYPE_9020WIRELESS As Integer = 1
    Public Const OBDLINK_TYPE_DREWDADWIRED_GEN As Integer = 2
    Public Const OBDLINK_TYPE_DREWDADWIRELESS_GEN As Integer = 3
    Public Const OBDLINK_TYPE_DREWDADWIRED_CA As Integer = 4
    Public Const OBDLINK_TYPE_DREWDADWIRELESS_CA As Integer = 5
    Public Const OBDLINK_TYPE_DREWDADWIRED_GA As Integer = 6
    Public Const OBDLINK_TYPE_DREWDADWIRELESS_GA As Integer = 7
    Public Const OBDLINK_TYPE_SIMULATOR As Integer = 9
    Public Const OBDLINK_TYPE_DREWDADWIRED_GEN4 As Integer = 12
    Public Const OBDLINK_TYPE_DREWDADWIRELESS_GEN4 As Integer = 13
    Public Const OBDLINK_TYPE_DREWDADWIRED_GDP3 As Integer = 14
    Public Const OBDLINK_TYPE_DREWDADWIRELESS_GDP3 As Integer = 15
    Public Const OBDLINK_TYPE_UNKNOWN As Integer = -1

    Public Const OBDLINK_NAME_9020WIRED As String = "9020-Wired"
    Public Const OBDLINK_NAME_9020WIRELESS As String = "9020-Wireless"
    Public Const OBDLINK_NAME_DREWDADWIRED_GEN As String = "DrewDAD_GEN-Wired"
    Public Const OBDLINK_NAME_DREWDADWIRELESS_GEN As String = "DrewDAD_GEN-Wireless"
    Public Const OBDLINK_NAME_DREWDADWIRED_GEN4 As String = "DrewDAD_GEN4-Wired"
    Public Const OBDLINK_NAME_DREWDADWIRELESS_GEN4 As String = "DrewDAD_GEN4-Wireless"
    Public Const OBDLINK_NAME_DREWDADWIRED_GDP3 As String = "DrewDAD_GDP3-Wired"
    Public Const OBDLINK_NAME_DREWDADWIRELESS_GDP3 As String = "DrewDAD_GDP3-Wireless"
    Public Const OBDLINK_NAME_DREWDADWIRED_CA As String = "DrewDAD_CA-Wired"
    Public Const OBDLINK_NAME_DREWDADWIRELESS_CA As String = "DrewDAD_CA-Wireless"
    Public Const OBDLINK_NAME_DREWDADWIRED_GA As String = "DrewDAD_GA-Wired"
    Public Const OBDLINK_NAME_DREWDADWIRELESS_GA As String = "DrewDAD_GA-Wireless"

    Public lvalue As Integer
    Public OBDLinkType As Integer
    Public lDeviceLinkOn As Boolean

    Public Connect_Succeeded As Boolean
    Public Connect_MyEngineId As Byte
    Public Connect_MyObdProtocol As String
    Public Connect_LinkProtocol As String
    Public Connect_ConnectModuleHashtable As Hashtable
    Public Connect_TotalPIDCountDCount As Integer
    Public Connect_ConnectVIN As String = ""

    Public OBDLinkInfo_Port = ""
    Public OBDLinkInfo_PortWired = ""
    Public OBDLinkInfo_PortWireless = ""
    Public OBDLinkInfo_InitializationStatus = "?"
    Public OBDLinkInfo_InitializationStatusDateTime = DateTime.MinValue
    Public OBDLinkInfo_DLLVersion = "?"
    Public OBDLinkInfo_DriverVersion = "?"
    Public OBDLinkInfo_FirmwareVersion = "?"
    Public OBDLinkInfo_SerialNumber = "?"
    Public OBDLinkInfo_LatestFirmwareVersion = "?"
    Public OBDLinkInfo_LastDeviceExtendedStatus = "?"
    Public OBDLinkInfo_LastDeviceExtendedStatusDateTime = DateTime.MinValue
    Public OBDLinkInfo_VehicleIsLinked = "?"
    '
    Public OBDLinkInfo_BatteryInformationAvailable = False
    Public OBDLinkInfo_BatteryInformationUpdateDateTime = DateTime.Now.AddHours(-10)
    Public OBDLinkInfo_BatteryCycleCount = 0.0
    Public OBDLinkInfo_BatteryStateOfCharge = 0.0
    Public OBDLinkInfo_BatteryHealth = 0.0
    Public OBDLinkInfo_BatteryTemperature = 0.0
    Public OBDLinkInfo_BatteryCurrent = 0.0
    '
    Public OBDLinkInfo_DeviceInformationAvailable = False
    Public OBDLinkInfo_DeviceInformationUpdateDateTime = DateTime.Now.AddHours(-10)
    Public OBDLinkInfo_DeviceSupportList = Nothing
    Public OBDLinkInfo_DeviceCurrentList = Nothing
    Public OBDLinkInfo_DeviceConnection = ""

    Public MyLastConnectVIN As String = ""
    Public MyLastConnectSpecialControl As String = ""
    Public MyLastConnectCallingProcedure As String = ""

    Public MyDADAccessCount As Long = 0
    Public MyDADLastReleaseLocation As String = ""
    Public MyDADLastGetLocation As String = ""
    Public MyDadOpen As Boolean = False

    Public MyDadOpenTime As DateTime = DateTime.MinValue
    Public MyDAD_SpecialControl As String = ""
    Public MyDADPort As String = ""
    Public MyDAD_AllowUpdateRequiredContinue As Boolean = False
    Public MyDAD_FullOpenClose As Boolean = True
    Public MyDAD_DevicesTicks As Int32 = System.Environment.TickCount
    Public MyDAD_InitializeStatusDateTime As DateTime = DateTime.MinValue
    Public MyIMCleanDriverVersion As Integer = 0
    Public MyDAD_SupportedProducts() As String = Nothing
    Public MyDAD_DeviceConnection As String = Nothing
    Public MyDeviceType As Integer = 0
    Public MyDAD_InitialMaxWaitTime As Integer = 600
    Public MyDAD_InitializeStatus As String = ""
    Public MyDAD_Devices() As String = Nothing
    Public MyDAD_DevicesCount As Integer = 0

    Public MyDADSemaphore As System.Threading.Semaphore = Nothing
    Public MyDoDADSemaphore As System.Threading.Semaphore = Nothing

    Public InitializeInterfaceAbort As Boolean = False

    Public ConnectModuleHashtable As Hashtable = Nothing

    Public MyObdProtocolCAN As Boolean = False

    Public MyObdProtocol As String

    Public MyOBDLinkActiveCount As Integer = 0
    Public MyDAD_ConnectCount As Integer = 0
    Public MyEngineId As Byte = 0

    Public PidSupportDictionary As New Dictionary(Of Byte, BitArray)     ' Mode 1 PID support per ECU
    Public PIDCountDictionary As New Dictionary(Of Byte, Integer)        ' Mode 1 PID support count per ECU
    Public SortedPIDCountDictionary As New Dictionary(Of Byte, Integer)  ' Mode 1 PID support count per ECU sorted by count
    Public PidSupportOverall As BitArray = Nothing                       ' Mode 1 PID support overall (all ECUs)
    Public Mode1ECUCount As Integer = 0                                  ' Mode 6 count of responding ECUs
    Public TotalPIDCount As Integer = 0                                  ' Mode 1 total PID count

    Public Mode6PidSupportDictionary As New Dictionary(Of Byte, BitArray)     ' Mode 6 PID support per ECU
    Public Mode6PIDCountDictionary As New Dictionary(Of Byte, Integer)        ' Mode 6 PID support count per ECU
    Public Mode6SortedPIDCountDictionary As New Dictionary(Of Byte, Integer)  ' Mode 6 PID support count per ECU sorted by count
    Public Mode6PidSupportOverall As BitArray = Nothing                       ' Mode 6 PID support overall (all ECUs)
    Public Mode6ECUCount As Integer = 0                                       ' Mode 6 count of responding ECUs
    Public Mode6TotalPIDCount As Integer = 0                                  ' Mode 6 total PID count

    Public Mode9PidSupportDictionary As New Dictionary(Of Byte, BitArray)     ' Mode 9 PID support per ECU
    Public Mode9PIDCountDictionary As New Dictionary(Of Byte, Integer)        ' Mode 9 PID support count per ECU
    Public Mode9SortedPIDCountDictionary As New Dictionary(Of Byte, Integer)  ' Mode 9 PID support count per ECU sorted by count
    Public Mode9PidSupportOverall As BitArray = Nothing                       ' Mode 9 PID support overall (all ECUs)
    Public Mode9ECUCount As Integer = 0                                       ' Mode 9 count of responding ECUs
    Public Mode9TotalPIDCount As Integer = 0

    Public Save_VehicleIsLinked As String = ""

    Public MySerialDataLogMaxBytes As Double = 160000
    Public MyVendorDataLogMaxBytes As Double = 120000

    Public ECUArray(100, 2) As String  ' Only for mode 1
    Public CVNArray(100, 2) As String
    Public CALIDArray(100, 3) As String

    '//--

    Public LrdOBD_STATUS_Cadd As String
    Public OBDdataBus As String()

    Public OBDdata_PROTOCOLO As String
    'Public OBDdata_VIN As String
    Public OBDdata_VINtxt As String
    'Public OBDdata_MIL As String
    Public OBDdata_MILtxt As String
    'Public OBDdata_DTC As String
    Public OBDdata_DTCtxt As String
    'Public OBDdata_RPMhx As String
    Public OBDdata_RPM As Integer
    Public OBDdata_RPMmat(3) As Integer

    'Public xOBDcadtxt As String
    Public xOBDHeadtxt As String

    Public LrdOBD_EDO_MIL As String

    Public LrdOBD_MSI As String '-- Sistema de detección de condiciones inadecuadas de ignición de cilindros ' LrdOBD_cilin
    Public LrdOBD_CCM As String '-- Sistema de eficiencia del convertidor catalitico ' LrdOBD_catal
    Public LrdOBD_CMB As String '-- Sistema de combustible ' LrdOBD_combu
    Public LrdOBD_O2S As String '-- Sistema de sensores de oxigeno ' LrdOBD_oxige
    Public LrdOBD_CAT As String '-- Sistema de componentes integrales ' LrdOBD_integ 

    Public LrdOBD_CCC As String '-- Sistema de calentamiento del convertidor catalitico
    Public LrdOBD_EVS As String '-- Sistema evaporativo
    Public LrdOBD_SAS As String '-- Sistema secundario de aire
    Public LrdOBD_FAA As String '-- Sistema de fugas de aire acondicionado
    Public LrdOBD_O2C As String '-- Sistema de calentamiento del sensor de oxigeno

    Public LrdOBD_Fallas As String
    Public xTablaFallas(20) As String

    Public xDTC_MSI As Boolean
    Public xDTC_CCM As Boolean
    Public xDTC_CMB As Boolean
    Public xDTC_O2S As Boolean
    Public xDTC_CAT As Boolean
    Public xDTC_CCC As Boolean
    Public xDTC_EVS As Boolean
    Public xDTC_SAS As Boolean
    Public xDTC_FAA As Boolean
    Public xDTC_O2C As Boolean

    Public xPid0101 As String = "Null" '-- Monitores MIL
    Public xPid0300 As String = "Null" '-- DTC
    Public xPid0121 As String = "Null" '-- Distancia MIL on
    Public xPid0131 As String = "Null" '-- Distancia MIL borrado
    Public xPid0133 As String = "Null" '-- Presion Barometrica Kpa 
    Public xPid011F As String = "Null" '-- Tiempo de encendido motor
    Public xPid017F As String = "Null" '-- Tiempo de marcha motor
    Public xPid014D As String = "Null" '-- Tiempo MIL on
    Public xPid0951 As String = "Null" '-- Tipo combustible
    Public xPid0902 As String = "Null" '-- VIN
    Public xPid0904 As String = "Null" '-- Cal ID
    Public xPid010C As String = "Null" '-- RPM

    Public facNS As Int32
    Public facFH As Int32

    '||--

    Public conn As New MySql.Data.MySqlClient.MySqlConnection

    Public xOpus_KeyDevice As String
    Public xOpus_KeyDevicePass As Boolean

    Public xVIDB_ConnectionString As String '= "server=localhost;uid=opus1234;pwd=1234opus;database=OpusOBDtest;Integrated Security=True"
    Public xVIDBLinkOnline As Boolean

    Public xLocalKEYfile As String = My.Computer.FileSystem.CurrentDirectory & "\OpusKeyLicense.key"

    Public xMacAddress As String

    Public xOBD_ECU_onLine As Boolean
    Public xOBD_SimulationWarning As Boolean
    Public xMinVoltage As Integer = 8 '-- Minimo voltaje que puede registrar un motor.

    Public Structure srtOBDBatteryInformation
        Public BatteryCycleCountAvailable As Boolean
        Public BatteryCycleCountUpdateDateTime As DateTime
        Public BatteryCycleCount As Single
        '
        Public BatteryStateOfChargeAvailable As Boolean
        Public BatteryStateOfChargeUpdateDateTime As DateTime
        Public BatteryStateOfCharge As Single
        '
        Public BatteryHealthAvailable As Boolean
        Public BatteryHealthUpdateDateTime As DateTime
        Public BatteryHealth As Single
        '
        Public BatteryTemperatureAvailable As Boolean
        Public BatteryTemperatureUpdateDateTime As DateTime
        Public BatteryTemperature As Single
        '
        Public BatteryCurrentAvailable As Boolean
        Public BatteryCurrentUpdateDateTime As DateTime
        Public BatteryCurrent As Single
    End Structure
    Public OBDBatteryInformation As srtOBDBatteryInformation

    Public Enum OBDLinkExtendedStatus
        Unknown
        Success
        Failure
        Timeout
        Busy
        Unsupported
        NotConnected
        UpdateRequired
        Exception
    End Enum

    Public Structure DeviceInfoStruct
        Public DeviceName As String
        Public DeviceCaption As String
        Public DeviceDescription As String
        Public DeviceID As String
        Public DevicePortName As String
        Public DeviceDriverName As String
        Public DeviceManufacturer As String
        Public DeviceHardwareIDs As String
        Public DeviceFirmwareVersion As String
        Public UserChoice() As String
    End Structure
    Public strDeviceInfo As DeviceInfoStruct

    Public Structure OBDWorkParams
        Dim sCommand As String
        Dim sLocation As String
        Dim oInParameters() As Object
        Dim oOutParameters() As Object
    End Structure

    Public Sub InitializeStringArrayData()
        InitStringArray(ECUArray)
        InitStringArray(CVNArray)
        InitStringArray(CALIDArray)
    End Sub


    Public Sub InitializePIDData()
        PidSupportDictionary = Nothing
        PIDCountDictionary = Nothing
        SortedPIDCountDictionary = Nothing
        PidSupportDictionary = New Dictionary(Of Byte, BitArray)
        PIDCountDictionary = New Dictionary(Of Byte, Integer)
        SortedPIDCountDictionary = New Dictionary(Of Byte, Integer)
        PidSupportOverall = Nothing
        Mode1ECUCount = 0
        TotalPIDCount = 0
        '
        Mode6PidSupportDictionary = Nothing
        Mode6PIDCountDictionary = Nothing
        Mode6SortedPIDCountDictionary = Nothing
        Mode6PidSupportDictionary = New Dictionary(Of Byte, BitArray)
        Mode6PIDCountDictionary = New Dictionary(Of Byte, Integer)
        Mode6SortedPIDCountDictionary = New Dictionary(Of Byte, Integer)
        Mode6PidSupportOverall = Nothing
        Mode6ECUCount = 0
        Mode6TotalPIDCount = 0
        '
        Mode9PidSupportDictionary = Nothing
        Mode9PIDCountDictionary = Nothing
        Mode9SortedPIDCountDictionary = Nothing
        Mode9PidSupportDictionary = New Dictionary(Of Byte, BitArray)
        Mode9PIDCountDictionary = New Dictionary(Of Byte, Integer)
        Mode9SortedPIDCountDictionary = New Dictionary(Of Byte, Integer)
        Mode9PidSupportOverall = Nothing
        Mode9ECUCount = 0
        Mode9TotalPIDCount = 0
    End Sub



    Function GetCallingProcedure(Optional ByVal CallDepth As Integer = 2,
                                     Optional ByVal NameOnly As Boolean = False) As String

        Dim sReturn As String = "?"
        Dim StackTrace As System.Diagnostics.StackTrace = Nothing
        Dim StackFrame As System.Diagnostics.StackFrame = Nothing
        Dim iFrameCount As Integer = 0
        Try
            StackTrace = New Diagnostics.StackTrace(False)
            iFrameCount = StackTrace.FrameCount
            If (iFrameCount > 0) Then
                If (CallDepth > iFrameCount) Then CallDepth = iFrameCount - 1
                If (CallDepth < 0) Then CallDepth = 2
                ' Go two levels back to get who call my caller (usually)
                StackFrame = StackTrace.GetFrame(CallDepth)
                sReturn = StackFrame.GetMethod.Name.ToString()
                If (NameOnly = False) Then
                    sReturn = StackFrame.GetMethod.DeclaringType.FullName.ToString() & ":" & sReturn
                End If
            End If
        Catch ex As Exception
            Applog("Err:GetCallingProcedure | " & ex.Message)
        End Try
        StackFrame = Nothing
        StackTrace = Nothing

        Applog("... GetCallingProcedure | " & sReturn)
        Return (sReturn)
    End Function

    Public Function CReplace(ByVal original As String,
                              ByVal pattern As String,
                              ByVal replacement As String,
                              Optional ByVal comparisonType As System.StringComparison = StringComparison.OrdinalIgnoreCase,
                              Optional ByVal stringBuilderInitialSize As Integer = -1) As String

        Dim sReturn As String = Nothing
        If (IsNothing(original) = True) Then
            sReturn = Nothing
        ElseIf ((IsNothing(pattern) = True) OrElse
                (original.Length <= 0) OrElse
                (pattern.Length <= 0)) Then
            sReturn = ""
        Else
            Dim posCurrent As Integer = 0
            Dim lenPattern As Integer = pattern.Length
            Dim idxNext As Integer
            Dim result As System.Text.StringBuilder = Nothing
            If (stringBuilderInitialSize < 0) Then
                result = New System.Text.StringBuilder(original.Length)
            Else
                result = New System.Text.StringBuilder(stringBuilderInitialSize)
            End If
            idxNext = original.IndexOf(pattern, comparisonType)
            While (idxNext >= 0)
                result.Append(original, posCurrent, idxNext - posCurrent)
                result.Append(replacement)
                posCurrent = idxNext + lenPattern
                idxNext = original.IndexOf(pattern, posCurrent, comparisonType)
            End While
            result.Append(original, posCurrent, original.Length - posCurrent)
            sReturn = result.ToString
        End If

        'Applog("... CReplace | " & sReturn)
        Return (sReturn)

    End Function



    Public Function LocalGetCallingProcedure(Optional ByVal CallDepth As Integer = 2,
                                             Optional ByVal NameOnly As Boolean = False) As String

        Dim sReturn As String = ""
        Dim sLevel As String = ""
        Dim idx As Integer = 1
        Dim iEnd As Integer = 3
        Dim iFrameDepth As Integer = 3
        If (NameOnly = True) Then iFrameDepth = 0
        Do While (idx <= iEnd)
            sLevel = GetCallingProcedure(CallDepth + idx, iFrameDepth) 'Sti.Utilities.LogFuncs.GetCallingProcedure(CallDepth + idx, iFrameDepth)
            sLevel = CReplace(sLevel, "Sti.Peripherals.ObdLinks.", "")
            sLevel = CReplace(sLevel, "Sti.Peripherals.", "")
            If (sLevel.ToUpper.Contains("Internal".ToUpper) = True) Then iEnd = iEnd + 1
            If (sReturn.Length > 0) Then sReturn = sReturn & ","
            sReturn = sReturn & CReplace(sLevel, "Sti.", "")
            idx = idx + 1
        Loop

        'Applog("... LocalGetCallingProcedure | " & sReturn)
        Return (sReturn)
    End Function


    Public Function IncrementActiveCount() As Integer

        MyOBDLinkActiveCount = MyOBDLinkActiveCount + 1
        If (MyOBDLinkActiveCount > 9999) Then MyOBDLinkActiveCount = 999

        Applog("... IncrementActiveCount | " & MyOBDLinkActiveCount)
        Return (MyOBDLinkActiveCount)
    End Function


    Public Function LocalIncrementActiveCount(ByVal sLogInfo As String) As Integer

        Static LastCallingProcedure As String = ""
        Static LastsLogInfo As String = ""
        Dim iReturn As Integer
        iReturn = IncrementActiveCount()
        Dim CallingProcedure As String = LocalGetCallingProcedure(, True)
        If (iReturn <> 1) Then
            Applog("IAC | Warning ... Active_count:" & iReturn.ToString("0") &
                               "  (" & CallingProcedure & ", " & sLogInfo & ")" &
                               " L:(" & LastCallingProcedure & ", " & LastsLogInfo & ")")
        End If
        LastCallingProcedure = CallingProcedure
        LastsLogInfo = sLogInfo

        'Applog("... LocalIncrementActiveCount | " & iReturn)
        Return (iReturn)
    End Function

    Public Function MyDADStatus() As String

        Dim RetValue As String = "DAD:Nothing"
        Try
            If (IsNothing(MyDad) = False) Then
                RetValue = "LCR:" & MyLastCommResult.ToString & "," & MyLastExtendedStatus.ToString
            End If
        Catch ex As Exception

        End Try

        Applog("... MyDADStatus | " & RetValue)
        Return (RetValue)

    End Function


    Public Sub SetupVehicleIsLinked(ByVal strValue As String)
        Applog("... SetupVehicleIsLinked")

        Dim CallingProcedure As String = LocalGetCallingProcedure()
        'If (IsNothing(OBDLinkInfo) = False) Then
        Save_VehicleIsLinked = OBDLinkInfo_VehicleIsLinked
        OBDLinkInfo_VehicleIsLinked = strValue
        If (Save_VehicleIsLinked <> OBDLinkInfo_VehicleIsLinked) Then
            Applog("SVLI | VehicleIsLinked:'" & Save_VehicleIsLinked & "'->'" & OBDLinkInfo_VehicleIsLinked & "'  (" & CallingProcedure & ")")
        End If
        'End If
    End Sub



    Public Function SetupMyLastCommResult(ByVal CommResult As DrewTech.IIMClean.DT_Com_Result,
                                           ByVal VehicleCommFlag As Boolean,
                                           Optional ByVal NoDeviceCountCheck As Boolean = False) As DrewTech.IIMClean.DT_Com_Result

        If (IsNothing(CommResult) = True) Then
            MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect
            SetupVehicleIsLinked("?")
        Else
            MyLastCommResult = CommResult
            If ((VehicleCommFlag = True) And
                (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success)) Then
                SetupVehicleIsLinked("True")
            End If
            If ((MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.DADNotConnected) Or
                (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.DADTimedOut) Or
                (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.VehicleNotConnected) Or
                (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.VehicleTimedOut) Or
                (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.VehicleError) Or
                (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.J2534_USB_UPDATE_REQUIRED)) Then
                SetupVehicleIsLinked("False")
            End If
        End If

        Applog("... SetupMyLastCommResult: " & MyLastCommResult.ToString)

        Return (MyLastCommResult)

    End Function


    Private Function GetDAD(Optional ByVal sLocation As String = "") As Boolean
        Static lCount As Long = 0
        Dim bReturn As Boolean = False
        Dim sMessage As String = "?"
        lCount = lCount + 1
        If (lCount > 99999) Then lCount = 9
        Applog("GetDAD | Starting ... Cnt:" & lCount.ToString("00000") & "," & MyDADAccessCount.ToString("00") &
                              " Opn:" & Microsoft.VisualBasic.Left(MyDadOpen.ToString, 1) & " (" & sLocation & ")")
        MyDADLastGetLocation = sLocation
        Try
            If (MyDADSemaphore.WaitOne(200) = True) Then
                MyDADAccessCount = MyDADAccessCount + 1
                If (MyDADAccessCount > 99) Then MyDADAccessCount = 99
                sMessage = "Success ..."
                bReturn = True
            Else
                sMessage = "Failure ..."
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                bReturn = False
            End If
        Catch ex As Exception
            sMessage = "ex:'" & ex.Message & "'"
            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
            bReturn = False
        End Try
        Dim iTemp As Integer = 5
        If (bReturn = False) Then iTemp = 1
        Applog("GetDAD | " & sMessage & " Cnt:" & lCount.ToString("00000") & "," & MyDADAccessCount.ToString("00") &
                                  " Opn:" & Microsoft.VisualBasic.Left(MyDadOpen.ToString, 1) &
                                  " (" & sLocation & ", " & MyDADLastReleaseLocation & ")")
        GetDAD = bReturn
    End Function


    Public Function IsCommResultSuccess(ByVal CommResult As DrewTech.IIMClean.DT_Com_Result) As Boolean

        Dim bReturn As Boolean = False
        If (IsNothing(CommResult) = True) Then
        Else
            If ((CommResult = DrewTech.IIMClean.DT_Com_Result.Success) OrElse
                ((MyDAD_AllowUpdateRequiredContinue = True) And
                 (CommResult = DrewTech.IIMClean.DT_Com_Result.J2534_USB_UPDATE_REQUIRED))) Then
                bReturn = True
            End If
        End If

        Applog("... IsCommResultSuccess | " & bReturn)
        Return (bReturn)
    End Function


    Public Function DeltaTimeTicks(ByVal StartTicks As Int32, Optional ByVal sControl As String = "Seconds") As Double
        'Applog("... DeltaTimeTicks")

        Dim dReturn As Double = -1.0
        Try
            Dim EndingTicks As Int32 = System.Environment.TickCount
            If (EndingTicks >= StartTicks) Then
                Dim DeltaTicks As Int32
                Dim DeltaMilliSeconds As Double
                Dim DeltaSeconds As Double
                DeltaTicks = EndingTicks - StartTicks
                DeltaMilliSeconds = DeltaTicks / 1.0
                DeltaSeconds = DeltaMilliSeconds / 1000.0
                If ((sControl.Length > 0) AndAlso (sControl.Substring(0, 1).ToUpper = "M".ToUpper)) Then
                    'sReturn = DeltaMilliSeconds.ToString("000")
                    dReturn = DeltaMilliSeconds
                Else
                    'sReturn = DeltaSeconds.ToString("0.00")
                    dReturn = DeltaSeconds
                End If
            End If
        Catch ex As Exception
            dReturn = -2.0
        End Try
        Return (dReturn)
    End Function


    Public Function FunctionSeconds(ByVal StartTicks As Int32) As String

        Dim sReturn As String = "-1"
        Try
            MyLastFunctionSeconds = DeltaTimeTicks(StartTicks, "Seconds")
            If (MyLastFunctionSeconds >= 0) Then
                sReturn = MyLastFunctionSeconds.ToString("0.00")
            Else
                sReturn = "-2"
            End If
        Catch ex As Exception
            sReturn = "-2"
        End Try

        'Applog("... FunctionSeconds: " & sReturn)
        Return (sReturn)
    End Function


    Public Function OpenDAD_If_Needed(Optional ByVal sTag As String = "") As Boolean

        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim tmpTime As DateTime
        Dim sError As String = "(?"
        Dim bLogFlag As Boolean = False
        If (IsNothing(MyDad) = False) Then
            If (MyDadOpen = False) Then
                bLogFlag = True
                Applog("OpenDAD_If_Needed | Attempting to open DAD ... " & sTag)
                tmpTime = DateTime.Now
                Dim cres As DrewTech.IIMClean.DT_Com_Result = DrewTech.IIMClean.DT_Com_Result.InvalidData
                cres = MyDad.Open()
                SetupMyLastCommResult(cres, False)
                cres = Nothing
                'If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                If (IsCommResultSuccess(MyLastCommResult) = True) Then
                    MyDadOpen = True
                    MyDadOpenTime = tmpTime
                    sError = "(Success"
                    If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.J2534_USB_UPDATE_REQUIRED) Then
                        sError = sError & "UR"
                    End If
                Else
                    sError = "(" & MyLastCommResult.ToString
                End If
            Else
                sError = "(Open"
            End If
        Else
            sError = "(No DAD"
            If (MyDAD_FullOpenClose = True) Then MyDadOpen = False ' No DAD cannot be open
        End If
        If (bLogFlag = True) OrElse (MyDadOpen = False) Then
            Applog("OpenDAD_If_Needed | DAD_Open:" & MyDadOpen.ToString &
                                             " ... " &
                                             sError & " | " & FunctionSeconds(StartTicks) & ") " &
                                             sTag)
        End If

        Applog("... OpenDAD_If_Needed | " & MyDadOpen)
        Return (MyDadOpen)

    End Function


    Public Sub DoDad(ByVal a As Action,
                      ByVal Tag As String,
                      Optional ByVal SpecialLogControl As String = "",
                      Optional ByVal sControl As String = "")

        Applog("... DoDad | Action: " & a.ToString & " | tag: " & Tag)

        Dim iCount As Integer = 0
        Dim LastSerialDataLogLength As Long = 0
        Dim CallingProcedure As String = LocalGetCallingProcedure(3)
        Dim bSerialDataLogCleared As Boolean = False
        Dim sDLLInfo As String = ""
        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim ElapsedSeconds As Double
        Dim bIncludeVoltage As Boolean = False
        Dim bConnected As Boolean = True
        Dim bReadVoltage As Boolean = False
        Dim idx As Integer
        Dim sTemp As String
        Dim LogText As String = ""

        iCount = iCount + 1
        If (iCount > 99) Then iCount = 99
        sTemp = ""
        Try
            Dim dTemp As Double
            dTemp = DateTime.Now.Subtract(MyDadOpenTime).TotalSeconds
            If (dTemp > 999.99) Then dTemp = 999.99
            If (MyDadOpen = True) Then
                sTemp = "," & dTemp.ToString("000.00")
            End If
        Catch ex As Exception
        End Try

        Applog("DoDad-" & iCount.ToString("00") & " | " & Tag & " (Open:" & MyDadOpen.ToString & sTemp & " | " & CallingProcedure & ")")

        If (iCount > 1) Then
            For idx = 0 To 30
                System.Windows.Forms.Application.DoEvents()
                System.Threading.Thread.Sleep(200)
                System.Windows.Forms.Application.DoEvents()
                If (iCount = 1) Then Exit For
            Next
        End If

        'Applog("DoDad-" & iCount.ToString("00") & " | ..00")

        If (MyDoDADSemaphore.WaitOne(2000) = True) Then

            Try
                Dim StartDateTime As DateTime = DateTime.Now
                If ((MyDadOpen = False) AndAlso (sControl.ToUpper.Contains("SupressAutoOpen".ToUpper) = False)) Then
                    Applog("DoDad-MyDoDADSemaphore-" & iCount.ToString("00") & " | " & Tag & " ... Attempting Open")
                    OpenDAD_If_Needed("DoDad-" & Tag & " (" & CallingProcedure & ")")
                End If

                'Applog("DoDad-" & iCount.ToString("00") & " | ..01")

                MyCurrentSerialDataLog = ""
                MyCurrentSerialDataLogSize = 0
                '
                If (Tag.ToUpper.Contains("Close".ToUpper) = True) Then
                    Applog("Doing close ...")
                End If

                'Applog("DoDad-" & iCount.ToString("00") & " | ..02")

                If (sControl.ToUpper.Contains("SupressConnectCheck".ToUpper) = False) Then
                    If ((bIncludeVoltage = True) AndAlso
                        (MyDadOpen = True) AndAlso
                        (MyLastCommResult <> DrewTech.IIMClean.DT_Com_Result.Success)) Then

                        'Applog("DoDad-" & iCount.ToString("00") & " | ..03")

                        Try
                            bReadVoltage = True
                            Dim dRes As DrewTech.IIMClean.DT_IGenericResponse(Of Decimal) = Nothing
                            Applog("DoDad-" & iCount.ToString("00") & "  GetVoltage ...")
                            dRes = MyDad.GetVoltage()
                            'dRes = MyDad.GetSerialNumber()
                            If (IsNothing(dRes) = False) Then
                                If ((dRes.CommResult = DrewTech.IIMClean.DT_Com_Result.DADNotConnected) OrElse
                                                (dRes.CommResult = DrewTech.IIMClean.DT_Com_Result.DADError) OrElse
                                                (dRes.CommResult = DrewTech.IIMClean.DT_Com_Result.J2534_USB_UPDATE_REQUIRED)) Then
                                    bConnected = False
                                    SetupMyLastCommResult(dRes.CommResult, False)
                                    Applog("DoDad-" & iCount.ToString("00") & " | " & Tag & "-CnctChk:'" & dRes.CommResult.ToString)
                                End If
                            End If
                            dRes = Nothing

                            'Applog("DoDad-" & iCount.ToString("00") & " | ..04")

                        Catch ex As Exception
                            Applog("Err:DoDad-" & iCount.ToString("00") & " | " & Tag & "-CnctChk-ex:'" & ex.Message)
                        End Try
                    End If
                End If
                '
                'Applog("DoDad-" & iCount.ToString("00") & " | ..05")

                If (MyDeviceLogClear = True) Then
                    MyDeviceLogClear = False
                    Try
                        If (IsNothing(MyDad) = False) Then
                            LastSerialDataLogLength = 0
                            Applog("DoDad-MyDeviceLogClear-" & iCount.ToString("00") & "  ClearLogs ...")
                            MyDad.ClearLogs()
                            bSerialDataLogCleared = True
                            MyDeviceLogClearStatus = "Completed:" & DateTime.Now.ToString(MyDeviceLogStatusDateFormat)
                            MyLastClearLogs = DateTime.Now
                        End If
                    Catch ex As Exception
                        MyDeviceLogClearStatus = "Error:" & ex.Message & "'"
                        Applog("Err:DoDad-" & iCount.ToString("00") & " | " & Tag & "-exCL:'" & ex.Message & "'")
                    End Try
                End If
                '
                'Applog("DoDad-" & iCount.ToString("00") & " | ..05")

                Try
                    If ((bConnected = True) AndAlso (IsNothing(MyDad) = False)) Then
                        Applog("DoDad-" & iCount.ToString("00") & "  ClearCommandLog ...")
                        MyDad.ClearCommandLog()
                    End If
                Catch ex As Exception
                    Applog("Err:DoDad-" & iCount.ToString("00") & " | " & Tag & "-exCC: " & ex.Message)
                End Try
                '
                'Applog("DoDad-" & iCount.ToString("00") & " | ..06")

                If ((MyDadOpen = True) OrElse (sControl.ToUpper.Contains("IgnoreOpen".ToUpper) = True)) Then
                    If ((bConnected = True) AndAlso (IsNothing(a) = False)) Then
                        Dim StartTicksInvoke As Int32 = System.Environment.TickCount
                        Applog("DoDad-" & iCount.ToString("00") & "  Invoke ... (" & Tag & ")")
                        If (Tag.ToUpper.Contains("Close".ToUpper) = True) Then
                            Applog("DoDad-Close ...")
                        End If
                        a.Invoke()
                        Applog("DoDad-" & iCount.ToString("00") & "  Invoke ... (" & Tag & ")" &
                                 "  (" & DeltaTimeTicks(StartTicksInvoke, "MS").ToString("000") & ")")
                    End If
                Else
                    Applog("DoDad-" & iCount.ToString("00") & " | " & Tag & " ... Skipped_Not_Open")
                End If
                Dim EndDateTime As DateTime = DateTime.Now

                'Applog("DoDad-" & iCount.ToString("00") & " | ..07")

                Const LogEntryPrefix As String = vbCrLf & "                    -  "
                MyCurrentSerialDataLog = ""
                MyCurrentSerialDataLogSize = 0

                'Applog("DoDad-" & iCount.ToString("00") & " | ..08")

                If (IsNothing(a) = False) Then
                    Try

                        MyCurrentSerialDataLogIsCommandLog = True
                        Applog("DoDad-" & iCount.ToString("00") & "  CommandLog.Length ...")
                        MyCurrentSerialDataLogSize = MyDad.CommandLog.Length

                        Applog("DoDad-" & iCount.ToString("00") & "  CommandLog ... (" & MyCurrentVendorDataLogSize.ToString("0") & ")")

                        MyCurrentSerialDataLog = Microsoft.VisualBasic.Left(MyDad.CommandLog, CInt(MySerialDataLogMaxBytes))

                        'Applog("MyDad.CommandLog: " & MyDad.CommandLog)

                        'Applog("DoDad-" & iCount.ToString("00") & " | ..09")

                        Dim iStart As Integer = MyCurrentSerialDataLog.IndexOf("DAD_DLL_VERSION_")
                        Dim iEnd As Integer = 0
                        Applog("DoDad-" & iCount.ToString("00") & "-CommandLog-Len:" & MyCurrentSerialDataLog.Length.ToString("0") &
                                 " Siz:" & MyCurrentSerialDataLogSize.ToString("0") & " | iStart: " & iStart)

                        If (iStart >= 0) Then
                            iStart = iStart + 16
                            iEnd = MyCurrentSerialDataLog.Substring(iStart).IndexOf(vbCrLf)
                            If (iEnd > 0) Then
                                sDLLInfo = MyCurrentSerialDataLog.Substring((iStart - 16), (iEnd + 16) - (iStart - 16))
                                OBDLinkInfo_DLLVersion = MyCurrentSerialDataLog.Substring(iStart, iEnd)
                                If (MyIMCleanDriverDLLVersion.Length > 0) Then OBDLinkInfo_DLLVersion = MyIMCleanDriverDLLVersion
                                If (MyCurrentSerialDataLogIsCommandLog = True) Then
                                    Dim iStart2 As Integer = iStart + iEnd + 2
                                    Dim iEnd2 As Integer = MyCurrentSerialDataLog.Substring(iStart2).IndexOf(vbCrLf)
                                    If (iEnd2 > 0) Then
                                        sDLLInfo = MyCurrentSerialDataLog.Substring((iStart - 16), (iEnd2 + iStart2) - (iStart - 16)).Replace(vbCrLf, " .. ").Trim

                                        OBDLinkInfo_DLLVersion = MyCurrentSerialDataLog.Substring(iStart2, iEnd2)
                                        If (MyIMCleanDriverDLLVersion.Length > 0) Then OBDLinkInfo_DLLVersion = MyIMCleanDriverDLLVersion

                                    End If
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        Applog("Err:DoDad: " & ex.Message)
                    End Try
                    '
                    'Applog("DoDad-" & iCount.ToString("00") & " | ..10")

                    'Applog("DoDad-" & iCount.ToString("00") & "  SerialDataLog.Length ...")
                    MyCurrentSerialDataLogSize = MyDad.SerialDataLog.Length
                    Applog("DoDad-" & iCount.ToString("00") & "  SerialData.Length ... (" & MyCurrentSerialDataLogSize.ToString("0") & ")")
                    MyCurrentSerialDataLog = MyDad.SerialDataLog  ' Get all bytes (for now)

                    'Applog("DoDad-" & iCount.ToString("00") & " | ..11")

                End If
                '
                If (MyCurrentSerialDataLog.Length = LastSerialDataLogLength) Then
                    ' No change since last time (probably non-vehicle command)
                    MyCurrentSerialDataLog = ""
                ElseIf (MyCurrentSerialDataLog.Length > LastSerialDataLogLength) Then
                    Dim iSave As Integer = MyCurrentSerialDataLog.Length
                    ' Get the difference from the last time
                    MyCurrentSerialDataLog = Microsoft.VisualBasic.Right(MyCurrentSerialDataLog,
                                                                         CInt(iSave - LastSerialDataLogLength)).Trim
                    LastSerialDataLogLength = iSave
                Else
                    ' Assume no vehicle related activity so no changes
                    'Debug.WriteLine("Here for debug")
                End If

                'Applog("DoDad-" & iCount.ToString("00") & " | ..12")

                MyCurrentSerialDataLog = Microsoft.VisualBasic.Right(MyCurrentSerialDataLog,
                                                                     CInt(MySerialDataLogMaxBytes)).Trim
                ' Do some cleanup to make the log pretty
                If (MyCurrentSerialDataLog.Length >= 2) Then
                    If (Right(MyCurrentSerialDataLog, 2) = vbCrLf) Then
                        MyCurrentSerialDataLog = Left(MyCurrentSerialDataLog, MyCurrentSerialDataLog.Length - 2)
                    End If
                End If

                'Applog("DoDad-" & iCount.ToString("00") & " | ..13")

                MyCurrentSerialDataLog = MyCurrentSerialDataLog.Trim
                MyCurrentSerialDataLog = MyCurrentSerialDataLog.Replace(vbCrLf & vbCrLf, vbCrLf)
                MyCurrentSerialDataLog = MyCurrentSerialDataLog.Trim
                Try
                    If (MyCurrentSerialDataLog.Length > 0) Then
                        If (Microsoft.VisualBasic.Left(MyCurrentSerialDataLog, vbCrLf.Length) = vbCrLf) Then
                            MyCurrentSerialDataLog = MyCurrentSerialDataLog.Substring(vbCrLf.Length)
                            MyCurrentSerialDataLog = MyCurrentSerialDataLog.Trim
                        End If
                        If (Microsoft.VisualBasic.Right(MyCurrentSerialDataLog, vbCrLf.Length) = vbCrLf) Then
                            MyCurrentSerialDataLog = MyCurrentSerialDataLog.Substring(0, MyCurrentSerialDataLog.Length - vbCrLf.Length)
                            MyCurrentSerialDataLog = MyCurrentSerialDataLog.Trim
                        End If
                    End If
                Catch ex As Exception
                    Applog("Err:DoDad | CSDL-ex: " & ex.Message)
                End Try

                'Applog("DoDad-" & iCount.ToString("00") & " | ..14")

                If (Tag.ToUpper.Contains("CALID".ToUpper) = True) Then
                    Applog("Tag = CALID")
                End If
                If (Tag.ToUpper.Contains("BATTERY".ToUpper) = True) Then
                    Applog("Tag = BATTERY")
                End If

                'Applog("DoDad-" & iCount.ToString("00") & " | ..15")

                sTemp = LogEntryPrefix
                If (MyCurrentSerialDataLog.Length <= 0) Then sTemp = ""
                LogText = String.Format("SLL:{0},{1}-CF:{2}-DLLInfo:'{3}'{4}{5}{6}                    ({7:0.000} s,{8}) {9}",
                                        MyCurrentSerialDataLogSize.ToString("00000"),
                                        LastSerialDataLogLength.ToString("00000"),
                                        bSerialDataLogCleared.ToString.Substring(0, 1),
                                        sDLLInfo,
                                        sTemp, MyCurrentSerialDataLog.Replace(vbCrLf, LogEntryPrefix), vbCrLf,
                                        EndDateTime.Subtract(StartDateTime).TotalSeconds,
                                        Microsoft.VisualBasic.Left(bReadVoltage.ToString, 1), Tag)

                'Applog("DoDad-" & iCount.ToString("00") & " | ..16")

                If (MyDeviceLogSave = True) Then
                    MyDeviceLogSave = False
                    MyCurrentSerialDataLog = ""
                    MyCurrentSerialDataLogSize = 0
                    Try
                        MyCurrentSerialDataLogIsCommandLog = False
                        MyCurrentSerialDataLogSize = MyDad.SerialDataLog.Length

                        MyCurrentSerialDataLog = MyDad.SerialDataLog.Trim
                        If (MyCurrentSerialDataLog.Length >= 2) Then
                            If (Right(MyCurrentSerialDataLog, 2) = vbCrLf) Then
                                MyCurrentSerialDataLog = Left(MyCurrentSerialDataLog, MyCurrentSerialDataLog.Length - 2)
                            End If
                            MyCurrentSerialDataLog = Trim(MyCurrentSerialDataLog)
                        End If
                    Catch ex As Exception

                    End Try
                    '
                    'Applog("DoDad-" & iCount.ToString("00") & " | ..17")

                    MyCurrentVendorDataLog = ""
                    MyCurrentVendorDataLogSize = 0

                    Try
                        StartTicks = System.Environment.TickCount
                        Applog("DoDad-" & iCount.ToString("00") & "  VendorLog.Length ...")
                        MyCurrentVendorDataLogSize = MyDad.VendorLog.Length
                        ElapsedSeconds = DeltaTimeTicks(StartTicks)
                        Applog("VendorLogTime1: " & ElapsedSeconds.ToString("0.000"))
                        Applog("DoDad-" & iCount.ToString("00") & "  VendorLog ... (" & MyCurrentVendorDataLogSize.ToString("0") & ")")
                        MyCurrentVendorDataLog = Microsoft.VisualBasic.Left(MyDad.VendorLog.Trim, CInt(MyVendorDataLogMaxBytes))

                        ElapsedSeconds = DeltaTimeTicks(StartTicks)
                        Applog("VendorLogTime2: " & ElapsedSeconds.ToString("0.000"))
                        If (MyCurrentVendorDataLog.Length >= 2) Then
                            If (Right(MyCurrentVendorDataLog, 2) = vbCrLf) Then
                                MyCurrentVendorDataLog = Left(MyCurrentVendorDataLog, MyCurrentVendorDataLog.Length - 2)
                            End If
                            MyCurrentVendorDataLog = Trim(MyCurrentVendorDataLog)
                        End If

                        'Applog("DoDad-" & iCount.ToString("00") & " | ..18")

                        ElapsedSeconds = DeltaTimeTicks(StartTicks)
                        Applog("VendorLogTime3: " & ElapsedSeconds.ToString("0.000"))

                        Applog("DoDad-" & iCount.ToString("00") & " | " & Tag & " ... Updating VendorLog (" &
                                        "SL:" & MyCurrentSerialDataLogSize.ToString("0") &
                                        " | " & MyCurrentSerialDataLog.Length.ToString("0") &
                                        " | " & MySerialDataLogMaxBytes.ToString("0") &
                                        " VL:" & MyCurrentVendorDataLogSize.ToString("0") &
                                        " | " & MyCurrentVendorDataLog.Length.ToString("0") &
                                        " | " & MyVendorDataLogMaxBytes.ToString("0") & ")")

                        sTemp = "?"
                        If (MyLastClearLogs > DateTime.MinValue) Then sTemp = MyLastClearLogs.ToString(MyDeviceLogStatusDateFormat)

                        sTemp = "?"
                        If (MyLastConnect > DateTime.MinValue) Then sTemp = MyLastConnect.ToString(MyDeviceLogStatusDateFormat)

                        sTemp = "?"
                        If (MyLastReConnect > DateTime.MinValue) Then sTemp = MyLastReConnect.ToString(MyDeviceLogStatusDateFormat)

                        'Applog("DoDad-" & iCount.ToString("00") & " | ..19")

                        ElapsedSeconds = DeltaTimeTicks(StartTicks)

                        ElapsedSeconds = DeltaTimeTicks(StartTicks)


                    Catch ex As Exception
                        MyDeviceLogSaveStatus = "SaveError:'" & ex.Message & "'"
                        Applog("Err:DoDad" & " | " & ex.Message)
                    End Try
                End If

                'Applog("DoDad-" & iCount.ToString("00") & " | ..20")

            Catch ex As Exception
                LogText = "ex:'" & ex.Message & "'"
            End Try
            MyDoDADSemaphore.Release(1)

            'Applog("DoDad-" & iCount.ToString("00") & " | ..21")

        Else
            LogText = Tag & "-" & "Semaphore_Timeout"
        End If
        Applog("DoDad-" & iCount.ToString("00") & " | " & Tag & "-" & LogText & " | " & SpecialLogControl)
        'DbgWrite("DoDad-" & iCount.ToString("00") & "  " & Tag & "-" & LogText.Replace(vbCrLf, " .. "))
        iCount = iCount - 1
        If (iCount < 0) Then iCount = 0


        OBDdata_PROTOCOLO = MyDad.OBDProtocol
        'Applog("DoDad-" & iCount.ToString("00") & " | ..22F | " & OBDdata_PROTOCOLO)
        '------------------------------
        'If InStr(Tag, "GetModePID(01, 40)") Then
        '
        'Applog("... CommandLog:" & MyDad.CommandLog)
        'Applog("... Get MON >>>-----------------------------------------------------------------------")
        'Call DECODE_Bus(MyDad.CommandLog)
        'Call DECODE_MIL(OBDdata_MIL)
        '
        'OBDdata_PROTOCOLO = MyDad.OBDProtocol
        'Applog(" ")
        'Applog("MyDad.OBDProtocol:" & MyDad.OBDProtocol)
        '
        'End If

        'Applog("... SelfTest:" & MyDad.SelfTest.ToString())


    End Sub


    Public Function DrewProtocol2MyProtocol(ByVal InProtocol As String) As String
        Dim sReturn As String = OBD_PROTOCOL_NAME_NONE
        Dim bCAN As Boolean = False
        sReturn = InProtocol
        bCAN = False
        If (sReturn.ToUpper.Contains("CAN".ToUpper) = True) Then bCAN = True
        Dim sTempProtocol As String = Left(sReturn.ToUpper, 5)
        Select Case sTempProtocol
            Case "ICAN1".ToUpper
                sReturn = OBD_PROTOCOL_NAME_CAN11
                If (InProtocol.ToUpper.Contains("bt250".ToUpper) = True) Then
                    sReturn = OBD_PROTOCOL_NAME_CAN11_250
                End If
                bCAN = True
            Case "ICAN2".ToUpper
                sReturn = OBD_PROTOCOL_NAME_CAN29
                If (InProtocol.ToUpper.Contains("bt250".ToUpper) = True) Then
                    sReturn = OBD_PROTOCOL_NAME_CAN29_250
                End If
                bCAN = True
            Case "AUTOI".ToUpper
                sReturn = OBD_PROTOCOL_NAME_ISO9141
                bCAN = False
            Case Else
                Select Case Left(sTempProtocol, 4)
                    Case "JPWM".ToUpper
                        sReturn = OBD_PROTOCOL_NAME_PWM
                        bCAN = False
                    Case "JVPW".ToUpper
                        sReturn = OBD_PROTOCOL_NAME_VPW
                        bCAN = False
                    Case "KWPF".ToUpper
                        sReturn = OBD_PROTOCOL_NAME_KWP_FAST
                        bCAN = False
                    Case "KWPS".ToUpper
                        sReturn = OBD_PROTOCOL_NAME_KWP_SLOW
                        bCAN = False
                    Case "I914".ToUpper
                        sReturn = OBD_PROTOCOL_NAME_ISO9141
                        bCAN = False
                    Case Else
                        If (sTempProtocol = "KWP".ToUpper) Then
                            sReturn = OBD_PROTOCOL_NAME_KWP_FAST
                            bCAN = False
                        End If
                End Select
        End Select
        MyObdProtocol = sReturn
        MyObdProtocolCAN = bCAN
        Applog("DrewProtocol2MyProtocol | In:'" & InProtocol & "'" & "  Out:'" & sReturn & "'" & " (CAN:" & bCAN.ToString & ")")
        Return (sReturn)
    End Function


    Public Function MyLastResult() As String

        Dim sReturn As String = ""
        Dim sLinked As String = ""
        sLinked = Save_VehicleIsLinked & "->" & OBDLinkInfo_VehicleIsLinked
        sReturn = MyLastCommResult.ToString & "," & MyLastExtendedStatus.ToString & "," & sLinked

        Applog("... MyLastResult | " & sReturn)
        Return (sReturn)
    End Function


    Public Function GetEcuIdFromAddress(ByVal address() As Byte) As Byte
        Applog("... GetEcuIdFromAddress")

        Dim EcuId As Byte = 0
        If (address.Length = 1) Then
            EcuId = address(0)
        Else
            If (MyObdProtocol.ToUpper.Contains("CAN11".ToUpper) = True) Then
                'EcuId = address(address.Length - 1) And CByte(&H7)
                EcuId = address(address.Length - 1)
            ElseIf (MyObdProtocol.ToUpper.Contains("CAN29".ToUpper) = True) Then
                EcuId = address(address.Length - 1)
            End If
        End If
        Return EcuId
    End Function


    Public Function ByteArrayToHexString(ByVal InArray As Byte(),
                                          Optional ByVal InStart As Integer = 0,
                                          Optional ByVal strSeperator As String = " ") As String

        Dim sReturn As String = ""
        Dim iPrintLength As Integer = 0
        Dim sbTemp As System.Text.StringBuilder = Nothing
        Try
            If ((IsNothing(InArray) = False) AndAlso (InArray.Length > 0) AndAlso (InStart < InArray.Length)) Then
                Dim iSepLen As Integer = 0
                If ((IsNothing(strSeperator) = False) AndAlso (strSeperator.Length > 0)) Then iSepLen = strSeperator.Length
                sbTemp = New System.Text.StringBuilder((InArray.Length * 2) + (InArray.Length * iSepLen) + 2)
                For idx As Integer = InStart To InArray.Length - 1
                    If ((iSepLen > 0) AndAlso (sbTemp.Length > 0)) Then sbTemp.Append(strSeperator)
                    sbTemp.Append(InArray(idx).ToString("X02"))
                Next
            End If
            If ((IsNothing(sbTemp) = False) AndAlso (sbTemp.Length > 0)) Then
                sReturn = sbTemp.ToString
            End If
        Catch ex As Exception
            Debug.WriteLine("ex:'" & ex.Message & "'")
        End Try

        Applog("... ByteArrayToHexString | " & sReturn)
        Return (sReturn)
    End Function


    Public Function DT_DataToString(ByVal InData As DrewTech.IIMClean.DT_IECUData) As String

        Dim sReturn As String = ""
        Dim sbTmp As New System.Text.StringBuilder(100)
        Try
            If (IsNothing(InData) = False) Then
                sbTmp.Append("Adr:" & ByteArrayToHexString(InData.Address, 0, ""))
                sbTmp.Append(",Mod:" & InData.Mode.ToString("X02"))
                sbTmp.Append(",PID:" & InData.PID.ToString("X02"))
                sbTmp.Append(",Dat:" & ByteArrayToHexString(InData.Data, 0, ""))
                sReturn = sbTmp.ToString()
            End If
        Catch ex As Exception
            Debug.WriteLine("DTDATA2STRING", "ex:'" & ex.Message & "'")
        End Try

        Applog("... DT_DataToString | " & sReturn)
        Return (sReturn)
    End Function


    Public Function CountBits(ByVal inByte As Byte) As Integer

        Dim iReturn As Integer = 0
        Dim bMask As Byte = 1
        For idx As Integer = 0 To 7
            If ((inByte And bMask) <> 0) Then
                iReturn = iReturn + 1
            End If
            bMask = bMask << 1
        Next

        'Applog("... CountBits | " & iReturn)
        Return (iReturn)
    End Function


    Public Sub SetSupportedBitArrayFromObdBytes(ByVal ObdBytes() As Byte, ByRef SupportedBitArray As BitArray,
                                                 ByVal StartIndex As Integer, ByVal sDebug As String)

        Dim sTemp As String = ""
        Dim iTemp As Integer = 0
        Dim EndOffset As Integer = 0
        If (ObdBytes.Length > 0) Then
            For idx As Integer = LBound(ObdBytes) To UBound(ObdBytes)
                If (sTemp <> "") Then sTemp = sTemp & ","
                sTemp = sTemp & ObdBytes(idx).ToString("X02")
                iTemp = iTemp + CountBits(ObdBytes(idx))
            Next
            sTemp = sTemp & ",BC:" & iTemp.ToString("000")
        End If
        Applog("SSBAFOB | " & sDebug & " InCnt:" & ObdBytes.Length.ToString("0") & " (" & sTemp & ")" & " SrtIdx:" & StartIndex.ToString("000"))
        sTemp = ""
        If (ObdBytes.Length >= 4) Then
            Dim BitArrayIndex As Integer = StartIndex
            EndOffset = ObdBytes.Length
            ' Many devices return a length byte before the data bytes
            ' with this in mind we usually use the last four bytes.
            ' The Ease simulator for CAN appears to send 5 bytes but
            ' the actual data is in the first 4.
            ' As the length byte should be 1 if we see 5 bytes where the first
            ' byte is not 1 and the last byte is 0 we will use the first 4 bytes.
            If ((EndOffset = 5) AndAlso (ObdBytes(0) <> 1) AndAlso (ObdBytes(EndOffset - 1) = 0) AndAlso
                (MyDAD_SpecialControl.ToUpper.Contains("EASEPIDSUP".ToUpper) = True)) Then
                EndOffset = EndOffset - 1
            End If
            If (sTemp.Length > 0) Then sTemp = sTemp & ","
            sTemp = sTemp & ObdBytes.Length & "-" & EndOffset.ToString("0")
            For ByteIndex As Integer = EndOffset - 4 To EndOffset - 1
                Dim ObdByte As Byte = ObdBytes(ByteIndex)
                For BitIndex As Integer = 1 To 8
                    If (((ObdByte And &H80) <> 0)) Then
                        SupportedBitArray(BitArrayIndex) = True
                        If (sTemp.Length > 0) Then sTemp = sTemp & ","
                        sTemp = sTemp & BitArrayIndex.ToString("X02")
                    End If
                    ObdByte = ObdByte << 1
                    BitArrayIndex += 1
                Next
            Next
        End If
        iTemp = 0
        For idx As Integer = 0 To SupportedBitArray.Length - 1
            If (SupportedBitArray(idx) = True) Then iTemp = iTemp + 1
        Next
        Applog("... SetSupportedBitArrayFromObdBytes | BitAryLen:" & SupportedBitArray.Length.ToString("000") &
                               "  BitSetCnt:" & iTemp.ToString("000") &
                               "  BAI:" & sTemp)
    End Sub


    Public Function LogBitArray(ByVal iLogLevel As Integer, ByVal sTag As String, ByVal sLogLead As String,
                                 ByVal bInArray As BitArray, ByVal iStartIdx As Integer,
                                 ByVal iStartIdxLog As Integer) As Integer

        Dim iReturn As Integer = 0
        Dim idx As Integer
        Dim BytesPerLine As Integer = 32
        Dim iTemp As Integer = 0
        Dim idxLog As Integer = iStartIdxLog
        Dim strTemp As String = ""
        Dim strTemp2 As String = ""
        Dim iTrueCnt As Integer = 0
        Dim iFalseCnt As Integer = 0
        Try
            If ((IsNothing(bInArray) = False) AndAlso (bInArray.Count > 0) AndAlso (iStartIdx < bInArray.Count)) Then
                For idx = iStartIdx To bInArray.Count - 1
                    If (strTemp = "") Then
                        iTemp = 0
                        strTemp = idxLog.ToString("000") & "-" & idxLog.ToString("X02") & "_" & idx.ToString("X02") & "-" & (idx + BytesPerLine).ToString("X3") & ":"
                    Else
                        strTemp = strTemp & ","
                    End If
                    If (iReturn = 0) Then
                        If (strTemp2 = "") Then
                            strTemp2 = "---" & "-" & "--" & "-" & "--" & "-" & "---" & "-"
                        Else
                            strTemp2 = strTemp2 & ","
                        End If
                    End If
                    'If (((idx - 1) Mod 8) = 0) Then strTemp = strTemp & " "
                    If ((iTemp Mod 8) = 0) Then
                        strTemp = strTemp & " "
                        strTemp2 = strTemp2 & " "
                    End If
                    If (bInArray(idx) = True) Then
                        strTemp = strTemp & "T"
                        iTrueCnt = iTrueCnt + 1
                    Else
                        strTemp = strTemp & "F"
                        iFalseCnt = iFalseCnt + 1
                    End If
                    If ((idx >= 0) And (idx <= 9)) Then
                        strTemp2 = strTemp2 & Chr(&H30 + idx)
                    Else
                        If ((idx >= 10) And (idx <= (10 + 25))) Then
                            strTemp2 = strTemp2 & Chr(&H41 + idx - 10)
                        Else
                            strTemp2 = strTemp2 & "?"
                        End If
                    End If
                    iTemp = iTemp + 1
                    If (iTemp >= BytesPerLine) Then
                        If (idx = bInArray.Count - 1) Then
                            strTemp = strTemp & "  (T:" & iTrueCnt.ToString("000") & " F:" & iFalseCnt.ToString("000") & ")"
                        End If
                        If (iReturn = 0) Then
                            Applog(iLogLevel & " | " & sTag & " | " & sLogLead & strTemp2)
                        End If
                        Applog(iLogLevel & " | " & sTag & " | " & sLogLead & strTemp)
                        iReturn = iReturn + 1
                        iTemp = 0
                        strTemp = ""
                    End If
                    idxLog = idxLog + 1
                Next
            End If
        Catch ex As Exception
            Applog("Err:LogBitArray | " & ex.Message & "'")
        End Try

        Applog("... LogBitArray | " & iReturn)
        Return (iReturn)
    End Function


    Public Function IncludePIDInPIDCount(ByVal ParameterID As Integer) As Boolean

        Static PIDCountIncludePID0 As Boolean = False
        Static PIDCountIncludePIDx20 As Boolean = True
        Static PIDCountIncludeUpdated As Boolean = False
        Dim bReturn As Boolean = True
        If (PIDCountIncludeUpdated = False) Then
            PIDCountIncludePID0 = True '--IncludePID0InPIDCount()
            PIDCountIncludePIDx20 = True '--IncludePIDx20InPIDCount()
        End If
        Select Case ParameterID
            Case &H0
                If (PIDCountIncludePID0 = False) Then bReturn = False
            Case &H20, &H40, &H60, &H80, &HA0, &HC0, &HE0
                If (PIDCountIncludePIDx20 = False) Then bReturn = False
        End Select

        'Applog("... IncludePIDInPIDCount | " & bReturn)
        Return (bReturn)
    End Function

    Public Function IncludeInPIDCount(ByVal ParameterID As Integer) As Boolean
        'Applog("... IncludeInPIDCount")
        Dim bReturn As Boolean = True
        bReturn = IncludePIDInPIDCount(ParameterID)
        'Select Case ParameterID
        '    Case &H00
        '        If (PidCountIncludePID0 = False) Then bReturn = False
        '    Case &H20, &H40, &H60, &H80, &HA0, &HC0, &HE0
        '        If (PidCountIncludePIDx20 = False) Then bReturn = False
        'End Select
        Return (bReturn)
    End Function


    Public Function DictValueSortDesc(ByVal SourceDictionary As System.Collections.Generic.Dictionary(Of Byte, Integer)) As System.Collections.Generic.Dictionary(Of Byte, Integer)
        Applog("... DictValueSortDesc")

        'Do a Dictionary Sort on the Value Highest to lowest
        'Dim inDict As Dictionary(Of Byte, Integer) = SourceDictionary
        Dim inDict As New System.Collections.Generic.Dictionary(Of Byte, Integer)
        Dim outDict As New System.Collections.Generic.Dictionary(Of Byte, Integer)
        Dim tmpDict As System.Collections.Generic.SortedDictionary(Of Byte, Integer) = Nothing
        Dim currentMaxValue As Integer = 0
        Dim sbTemp As System.Text.StringBuilder = New System.Text.StringBuilder(200)

        If ((IsNothing(SourceDictionary) = False) AndAlso (SourceDictionary.Count > 0)) Then
            ' Make a copy of the source
            sbTemp.Clear()

            For Each kvp As System.Collections.Generic.KeyValuePair(Of Byte, Integer) In SourceDictionary
                sbTemp.Append(kvp.Key.ToString("X02") & ":" & kvp.Value.ToString("000") & ",")
                inDict.Add(kvp.Key, kvp.Value)
            Next
            If (sbTemp(sbTemp.Length - 1) = ",") Then sbTemp.Remove(sbTemp.Length - 1, 1)
            sbTemp.Append("->")
            '
            Do While inDict.Keys.Count > 0
                currentMaxValue = 0
                tmpDict = Nothing
                tmpDict = New System.Collections.Generic.SortedDictionary(Of Byte, Integer)
                ' Find the max value
                For Each currentKVP As System.Collections.Generic.KeyValuePair(Of Byte, Integer) In inDict
                    If (currentKVP.Value >= currentMaxValue) Then
                        currentMaxValue = currentKVP.Value
                    End If
                Next
                ' Get the key(s) with max value into our temp dictionary
                For Each currentKVP As System.Collections.Generic.KeyValuePair(Of Byte, Integer) In inDict
                    If (currentKVP.Value = currentMaxValue) Then
                        tmpDict.Add(currentKVP.Key, currentKVP.Value)
                    End If
                Next
                ' Move all the temp dictionary entries (sorted by assending key)
                If (tmpDict.Count > 0) Then
                    For Each tmpKVP As System.Collections.Generic.KeyValuePair(Of Byte, Integer) In tmpDict
                        'Add the new current iteration Key/Value to the new dictionary and remove from old
                        outDict.Add(tmpKVP.Key, tmpKVP.Value)
                        'remove the old item
                        inDict.Remove(tmpKVP.Key)
                    Next
                End If
                ''Add the new current iteration Key/Value to the new dictionary and remove from old
                'outDict.Add(currentMaxKey, currentMaxValue)
                ''remove the old item
                'inDict.Remove(currentMaxKey)
            Loop
            For Each kvp As System.Collections.Generic.KeyValuePair(Of Byte, Integer) In outDict
                sbTemp.Append(kvp.Key.ToString("X02") & ":" & kvp.Value.ToString("000") & ",")
                inDict.Add(kvp.Key, kvp.Value)
            Next
            If (sbTemp(sbTemp.Length - 1) = ",") Then sbTemp.Remove(sbTemp.Length - 1, 1)
            Applog("DVSD | " & sbTemp.ToString)
            'Debug.WriteLine("DVSD-" & sbTemp.ToString)
        End If
        Return outDict
    End Function


    Public Sub GetSupportedPIDs(ByVal Mode As Byte)

        Dim CallingProcedure As String = LocalGetCallingProcedure()
        Dim sDebugString As String = "Success:"
        Dim PidCountBlock As Byte = 0
        Dim sTemp As String
        '
        Dim L_PidSupportDictionary As New Dictionary(Of Byte, BitArray)
        Dim L_PIDCountDictionary As New Dictionary(Of Byte, Integer)
        Dim L_SortedPIDCountDictionary As New Dictionary(Of Byte, Integer)
        Dim L_PidSupportOverall As BitArray = Nothing
        Dim L_ECUCount As Integer = 0
        Dim L_TotalPIDCount As Integer = 0
        Dim L_ECUArray(10, 2) As String
        Dim sDebug As String

        ReDim L_ECUArray(ECUArray.GetUpperBound(0), ECUArray.GetUpperBound(1))
        If ((Mode < 1) Or (Mode > 9)) Then Mode = 1
        '
        Select Case Mode
            Case 1
                ConnectModuleHashtable = Nothing
                ConnectModuleHashtable = New Hashtable
            Case 6
                ' Nothing here
            Case 9
                ' Nothing here
        End Select
        '
        Applog("GetSupportedPIDs | Starting ... (Mode:" & Mode.ToString("0") & ") (" & CallingProcedure & ")")
        Try
            Do
                Dim r1 As DrewTech.IIMClean.DT_IMpr = Nothing
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.InvalidData, False)

                DoDad(Sub() r1 = MyDad.GetModePID(Mode, PidCountBlock), String.Format("GetModePID({0:X2}, {1:X2}) - PID support mask", Mode, PidCountBlock), "NoEOL")

                If (IsNothing(r1) = False) Then SetupMyLastCommResult(r1.CommResult, True)
                Applog(" (Result:" & MyLastResult() & ")")
                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                    sDebugString = sDebugString & "True"
                    Applog("GetSupportedPIDs | PID" & PidCountBlock.ToString("X02") & " RspCnt:" & r1.Data.Count.ToString("0"))
                    For Each d As DrewTech.IIMClean.DT_IECUData In r1.Data
                        If (PidCountBlock = 0) Then L_ECUCount = L_ECUCount + 1
                        Dim ThisData As DrewTech.IIMClean.DT_IECUData = d
                        Dim EcuId As Byte = GetEcuIdFromAddress(d.Address)
                        Dim ba As BitArray
                        If (L_PidSupportDictionary.ContainsKey(EcuId)) Then
                            ba = L_PidSupportDictionary(EcuId)
                        Else
                            ba = New BitArray(256)
                            ba(0) = True  ' If we are here the PID 0 is supported
                            L_PidSupportDictionary.Add(EcuId, ba)
                        End If
                        Applog("GetSupportedPIDS | Data-" & DT_DataToString(ThisData))
                        sDebug = Mode.ToString("0") & "-" & EcuId.ToString("X02")
                        'LogBitArray(2, "GetSupportedPIDs", sDebug & "-DBG1 ", ba, 0, 0)
                        SetSupportedBitArrayFromObdBytes(ThisData.Data, ba, PidCountBlock + 1,
                                                         "MdPID:01," & PidCountBlock.ToString("X02") & " ECU:" & EcuId.ToString("X02") & " ... ")
                        'LogBitArray(2, "GetSupportedPIDs", sDebug & "-DBG2 ", ba, 0, 0)
                        LogBitArray(2, "GetSupportedPIDs", sDebug & "-DBG3 ", L_PidSupportDictionary(EcuId), 0, 0)
                    Next

                    Dim CheckNextBlock As Boolean = False
                    For Each ba As BitArray In L_PidSupportDictionary.Values
                        'If (ba(PidCountBlock + 31)) Then
                        If (ba(PidCountBlock + 31 + 1)) Then
                            CheckNextBlock = True
                            Exit For
                        End If
                    Next

                    If (Not CheckNextBlock) Then Exit Do
                    PidCountBlock += CByte(&H20)
                Else
                    sDebugString = sDebugString & "False"
                    Exit Do
                End If
            Loop
            Applog("GetSupportedPIDs | Mode:" & Mode.ToString("0") & " ECUCnt:" & L_ECUCount.ToString("0") & " PIDSupDicCnt:" & L_PidSupportDictionary.Count.ToString("0"))
            '
            If (L_PidSupportDictionary.Count > 0) Then
                L_PidSupportOverall = New BitArray(L_PidSupportDictionary.Values(0).Count)
                For idx As Integer = 0 To L_PidSupportOverall.Count - 1
                    L_PidSupportOverall(idx) = False
                Next
                Dim Counter As Integer = 0
                For Each kvp As KeyValuePair(Of Byte, BitArray) In L_PidSupportDictionary
                    LogBitArray(4, "GetSupportedPIDS", "Mode" & Mode.ToString("0") & "-ECU" & kvp.Key.ToString("X02") & ":", kvp.Value, 0, 0)
                    sTemp = ""
                    Dim iCount As Integer = 0
                    If (kvp.Value.Length > 0) Then
                        For idx As Integer = 0 To kvp.Value.Length - 1
                            If (kvp.Value(idx) = True) Then
                                If (IncludeInPIDCount(idx) = True) Then iCount = iCount + 1
                                L_PidSupportOverall(idx) = True
                            End If
                        Next
                    End If
                    L_PIDCountDictionary.Add(kvp.Key, iCount)
                    If (Counter < L_ECUArray.GetUpperBound(0)) Then
                        L_ECUArray(Counter, 0) = kvp.Key.ToString("X02")
                        L_ECUArray(Counter, 1) = iCount.ToString("0")
                        L_ECUArray(Counter, 2) = (Counter + 1).ToString("0")
                        Counter = Counter + 1
                    End If
                Next
                For idx As Integer = 0 To L_PidSupportOverall.Count - 1
                    If (L_PidSupportOverall(idx) = True) Then
                        If (IncludeInPIDCount(idx) = True) Then
                            'If (Mode = 1) Then L_TotalPIDCount = L_TotalPIDCount + 1
                            L_TotalPIDCount = L_TotalPIDCount + 1
                        End If
                    End If
                Next
                '
                sTemp = ""
                For Each kvp As KeyValuePair(Of Byte, Integer) In L_PIDCountDictionary
                    If (sTemp <> "") Then sTemp = sTemp & ", "
                    sTemp = sTemp & kvp.Key.ToString("X02") & ":" & kvp.Value.ToString("000")
                Next
                LogBitArray(2, "GetSupportedPIDs", Mode.ToString("0") & "-ECUxx ", L_PidSupportOverall, 0, 0)
                Applog("GetSupportedPIDs | PIDCnt-" & Mode.ToString("0") & "-" & sTemp & "  (TPC:" & L_TotalPIDCount.ToString("0") & ")")
                '
                If (L_PIDCountDictionary.Count > 0) Then
                    L_SortedPIDCountDictionary = DictValueSortDesc(L_PIDCountDictionary)
                    For Each kvp As KeyValuePair(Of Byte, Integer) In L_SortedPIDCountDictionary
                        sDebugString = sDebugString & " " & Mode.ToString("0") & "-" & kvp.Key.ToString("X02") & ":" & kvp.Value.ToString("000")
                        If (Mode = 1) Then
                            ConnectModuleHashtable.Add(kvp.Key, kvp.Value)
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            Applog("Err:GetSupportedPIDs | " & ex.Message)
        End Try
        '
        Select Case Mode
            Case 1
                PidSupportDictionary = Nothing
                PIDCountDictionary = Nothing
                SortedPIDCountDictionary = Nothing
                PidSupportOverall = Nothing
                PidSupportDictionary = L_PidSupportDictionary
                PIDCountDictionary = L_PIDCountDictionary
                SortedPIDCountDictionary = L_SortedPIDCountDictionary
                PidSupportOverall = L_PidSupportOverall
                Mode1ECUCount = L_ECUCount
                TotalPIDCount = L_TotalPIDCount
                ECUArray = L_ECUArray
            Case 6
                Mode6PidSupportDictionary = Nothing
                Mode6PIDCountDictionary = Nothing
                Mode6SortedPIDCountDictionary = Nothing
                Mode6PidSupportOverall = Nothing
                Mode6PidSupportDictionary = L_PidSupportDictionary
                Mode6PIDCountDictionary = L_PIDCountDictionary
                Mode6SortedPIDCountDictionary = L_SortedPIDCountDictionary
                Mode6PidSupportOverall = L_PidSupportOverall
                Mode6ECUCount = L_ECUCount
                Mode6TotalPIDCount = L_TotalPIDCount
            Case 9
                Mode9PidSupportDictionary = Nothing
                Mode9PIDCountDictionary = Nothing
                Mode9SortedPIDCountDictionary = Nothing
                Mode9PidSupportOverall = Nothing
                Mode9PidSupportDictionary = L_PidSupportDictionary
                Mode9PIDCountDictionary = L_PIDCountDictionary
                Mode9SortedPIDCountDictionary = L_SortedPIDCountDictionary
                Mode9PidSupportOverall = L_PidSupportOverall
                Mode9ECUCount = L_ECUCount
                Mode9TotalPIDCount = L_TotalPIDCount
        End Select
        If ((IsNothing(L_PidSupportOverall) = False) AndAlso (L_PidSupportOverall.Length > 0)) Then
            sDebug = ""
            Dim iCount As Integer = 0
            For idx As Integer = 0 To L_PidSupportOverall.Length - 1
                If (L_PidSupportOverall(idx) = True) Then
                    iCount = iCount + 1
                    sDebug = sDebug & idx.ToString("X02") & " "
                End If
            Next
            Applog("GetSupportedPIDs | Mode:" & Mode.ToString("0") & " PIDs-'" & iCount.ToString("000") & ":" & sDebug.Trim)
        End If
        '
        Applog(" GetSupportedPIDs | Ending ... " & sDebugString)
    End Sub


    Public Function GetIMCleanDriverVersion() As String

        Dim sReturn As String = "?"
        Try
            MyIMCleanDriverVersion = MyDad.IMCleanDriverVersion
            sReturn = MyIMCleanDriverVersion.ToString("0000")
        Catch ex As Exception
            Applog("Err:GetIMCleanDriverVersion | " & ex.Message)
        End Try

        Applog("... GetIMCleanDriverVersion | " & sReturn)
        Return sReturn
    End Function


    Private Function GetDLLVersionInternal() As String
        Applog("... GetDLLVersionInternal")

        Dim sReturn As String = ""
        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim CallingProcedure As String = LocalGetCallingProcedure()
        Dim sInterfaceDLLName As String = ""
        Dim fviData As System.Diagnostics.FileVersionInfo = Nothing
        Dim meProcess As System.Diagnostics.Process = Nothing
        Dim meProcessModule As System.Diagnostics.ProcessModule = Nothing
        Dim meProcessModuleCollection As System.Diagnostics.ProcessModuleCollection = Nothing
        Try
            meProcess = System.Diagnostics.Process.GetCurrentProcess()
            If (IsNothing(meProcess) = False) Then
                Debug.WriteLine("HandleCount:" & meProcess.HandleCount.ToString("000"))
                meProcessModuleCollection = meProcess.Modules
                If ((IsNothing(meProcessModuleCollection) = False) AndAlso (meProcessModuleCollection.Count > 0)) Then
                    For idx As Integer = 0 To meProcessModuleCollection.Count - 1
                        meProcessModule = meProcessModuleCollection(idx)
                        If (IsNothing(meProcessModule) = False) Then
                            'WriteLLL(1, "GetDLLVerInt", "idx:" & idx.ToString("00") & " Nam:'" & meProcessModule.FileName & "'")
                            If (meProcessModule.FileName.ToUpper.Contains("IMCleanDriver".ToUpper) = True) Then
                                Debug.WriteLine("idx:" & idx.ToString("00"))
                                sInterfaceDLLName = meProcessModule.FileName
                            End If
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            Applog("Err:GetDLLVerInt | " & ex.Message)
        End Try

        Try
            If (sInterfaceDLLName.Length < 3) Then
                'sInterfaceDLLName = "C:\PRY_NT\OpusIMclean_OBD\OpusIMCleanOBD\OpusIMCleanOBD\IMCleanDriver.dll" 'System.IO.Path.Combine(Sti.RunTime.Context.RunPath, "IMCleanDriver.dll")
                sInterfaceDLLName = "IMCleanDriver.dll" 'System.IO.Path.Combine(Sti.RunTime.Context.RunPath, "IMCleanDriver.dll")
            End If
            If (sInterfaceDLLName.Length < 3) Then
            Else
                fviData = System.Diagnostics.FileVersionInfo.GetVersionInfo(sInterfaceDLLName)
                If (IsNothing(fviData) = False) Then
                    Debug.WriteLine("Ver:'" & fviData.FileVersion & "'")
                    sReturn = fviData.FileVersion.Trim
                End If
            End If
        Catch ex As Exception
            Applog("Err:GetDLLVerInt | " & ex.Message)
        End Try
        '
        If (sReturn.Length > 0) Then MyIMCleanDriverDLLVersion = sReturn
        Applog("GetDLLVerInt | Ver: " & sReturn & " | (" & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
        Return (sReturn)
    End Function


    Public Function StringArrayToString(ByVal InArray As String()) As String

        Dim sReturn As String = ""
        Try
            If ((IsNothing(InArray) = False) AndAlso (InArray.Length > 0)) Then
                For Each sValue As String In InArray
                    If (sReturn.Length > 0) Then sReturn = sReturn & "|"
                    sReturn = sReturn & sValue
                Next
            End If
        Catch ex As Exception
            Applog("Err:StringArrayToString | " & ex.Message)
        End Try

        Applog("... StringArrayToString: " & sReturn)
        Return (sReturn)

    End Function

    Public Function GetDrewSupportedProducts() As String()
        Applog("... GetDrewSupportedProducts")

        Dim CallingProcedure As String = LocalGetCallingProcedure()
        Dim sReturn As String() = Nothing
        Dim iReturnCount As Integer = 0
        'Dim bSuccess As Boolean = False
        SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.NotSupported, False)
        Dim aRes As DrewTech.IIMClean.DT_IGenericResponse(Of DrewTech.IIMClean.DT_Product()) = Nothing
        If (IsNothing(MyDad) = False) Then
            aRes = MyDad.GetSupportedProducts
            SetupMyLastCommResult(aRes.CommResult, False)
            If ((IsNothing(aRes) = False) AndAlso
                (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success)) Then
                'bSuccess = True
                If (IsNothing(sReturn) = True) Then ReDim sReturn(0)
                sReturn(0) = ""
                If (aRes.Data.Count > 0) Then
                    For Each dtProd As DrewTech.IIMClean.DT_Product In aRes.Data
                        If ((iReturnCount = 0) OrElse (sReturn.Contains(dtProd.ToString.Trim) = False)) Then
                            ReDim Preserve sReturn(iReturnCount)
                            sReturn(iReturnCount) = dtProd.ToString.Trim
                            iReturnCount = iReturnCount + 1
                        End If
                    Next
                End If
            End If
        End If
        Dim sTemp As String
        sTemp = StringArrayToString(sReturn)
        Applog("GetDrewSupportedProducts | Sts:" & MyLastResult() & "  Cnt:" & iReturnCount.ToString("0") & "  Prd: " & sTemp & " |" & CallingProcedure)
        Return sReturn
    End Function


    Public Function GetExtendedCommStatus(ByVal CommResult As DrewTech.IIMClean.DT_Com_Result) As OBDLinkExtendedStatus
        Applog("... GetExtendedCommStatus")

        Dim osReturn As OBDLinkExtendedStatus = OBDLinkExtendedStatus.Unknown
        Try
            Select Case (CommResult)
                Case DrewTech.IIMClean.DT_Com_Result.DADError,
                     DrewTech.IIMClean.DT_Com_Result.ErrorOpeningPort
                    osReturn = OBDLinkExtendedStatus.Failure

                Case DrewTech.IIMClean.DT_Com_Result.Success
                    osReturn = OBDLinkExtendedStatus.Success

                Case DrewTech.IIMClean.DT_Com_Result.DADTimedOut
                    osReturn = OBDLinkExtendedStatus.Timeout

                Case DrewTech.IIMClean.DT_Com_Result.DADNotConnected
                    osReturn = OBDLinkExtendedStatus.NotConnected

                Case DrewTech.IIMClean.DT_Com_Result.NotSupported
                    osReturn = OBDLinkExtendedStatus.Unsupported
                Case DrewTech.IIMClean.DT_Com_Result.J2534_USB_UPDATE_REQUIRED
                    osReturn = OBDLinkExtendedStatus.UpdateRequired
            End Select
        Catch ex As Exception
            Applog("Err:GetExtendedCommStatus | " & ex.Message)
        End Try
        Return (osReturn)
    End Function


    Public Function GetDrewDevices(Optional ByVal sControl As String = "") As String()
        Applog("... GetDrewDevices")

        Dim CallingProcedure As String = LocalGetCallingProcedure()
        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim sReturn() As String = Nothing
        Dim iReturnCount As Integer = 0
        Dim sDeviceList As List(Of String) = New List(Of String)
        Dim bSuccess As Boolean = False
        Dim sLocation As String = ""
        Dim bUseLocalStatus As Boolean = False
        Dim LocalCommResult As DrewTech.IIMClean.DT_Com_Result = DrewTech.IIMClean.DT_Com_Result.NotSupported
        '
        If (sControl.ToUpper.Contains("NoLastCommResult".ToUpper) = True) Then bUseLocalStatus = True
        MyDAD_Devices = Nothing
        MyDAD_DevicesCount = 0
        '
        '#If (DREW_GENERIC) Then
        LocalCommResult = DrewTech.IIMClean.DT_Com_Result.DEVICE_NOT_SUPPORED
        If (bUseLocalStatus = False) Then SetupMyLastCommResult(LocalCommResult, False, True)
        Dim aRes As DrewTech.IIMClean.DT_IGenericResponse(Of String()) = Nothing
        'Dim ProductList As DrewTech.IIMClean.DT_Product() = {DrewTech.IIMClean.DT_Product.IMCLEAN_CA,
        '                                                     DrewTech.IIMClean.DT_Product.IMCLEAN_GEN_03}
        If (IsNothing(MyDad) = False) Then
            Try
                'For Each dProd As DrewTech.IIMClean.DT_Product in ProductList
                sLocation = "Loop-A1"
                aRes = Nothing
                LocalCommResult = DrewTech.IIMClean.DT_Com_Result.NotSupported
                If (bUseLocalStatus = False) Then SetupMyLastCommResult(LocalCommResult, False, True)
                sLocation = "Loop-A2"
                'aRes = MyDad.GetDevices(dProd)
                aRes = MyDad.GetDevices()
                MyDAD_DevicesTicks = System.Environment.TickCount
                sLocation = "Loop-A3"
                If (IsNothing(aRes) = False) Then
                    LocalCommResult = aRes.CommResult
                    If (bUseLocalStatus = False) Then SetupMyLastCommResult(LocalCommResult, False, True)
                End If
                sLocation = "Loop-A4"
                If ((IsNothing(aRes) = False) AndAlso
                    (LocalCommResult = DrewTech.IIMClean.DT_Com_Result.Success)) Then
                    bSuccess = True
                    If (aRes.Data.Count > 0) Then
                        For Each sDevice As String In aRes.Data
                            sLocation = "Loop-B1"
                            If (sDeviceList.Contains(sDevice.ToString.Trim) = False) Then
                                sDeviceList.Add(sDevice.ToString.Trim)
                            End If
                        Next
                    End If
                End If
                'Next
            Catch ex As Exception
                Applog("Err:GetDrewDevices | " & ex.Message & " | Loc:'" & sLocation)
            End Try
        Else
            'sDeviceList.Add("NoDAD")
        End If
        '
        If ((IsNothing(sDeviceList) = False) AndAlso (sDeviceList.Count > 0)) Then
            iReturnCount = sDeviceList.Count
            sReturn = sDeviceList.ToArray
            MyDAD_Devices = sReturn
            MyDAD_DevicesCount = iReturnCount
        End If
        sDeviceList = Nothing
        '
        Dim sTemp As String = ""
        Dim sTemp2 As String = ""
        If (IsNothing(sReturn) = False) Then sTemp = StringArrayToString(sReturn)
        sTemp2 = LocalCommResult.ToString & "," & GetExtendedCommStatus(LocalCommResult).ToString
        Applog("GetDrewDevices | Sts:" & sTemp2 & "  Cnt:" & iReturnCount.ToString("0") & "  Dev:'" & sTemp & "'" &
                                      "  " & FunctionSeconds(StartTicks) & "  (" & CallingProcedure & ")")
        Return sReturn
    End Function


    Public Function GetDrewDeviceConnection() As String
        Applog("... GetDrewDeviceConnection")

        Dim sReturn As String = Nothing
        Dim bSuccess As Boolean = False
        Dim sLocation As String = ""

        'SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.DEVICE_NOT_SUPPORED, False)
        Dim aRes As DrewTech.IIMClean.DT_IGenericResponse(Of DrewTech.IIMClean.DT_CommunicationMode) = Nothing
        If (IsNothing(MyDad) = False) Then
            Try
                sLocation = "Loop-A1"
                aRes = Nothing
                'SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.NotSupported, False)
                sLocation = "Loop-A2"

                Try
                    aRes = MyDad.GetCommunicationMode
                Catch exDAD As DrewTech.IIMClean.DADException
                    If ((IsNothing(exDAD) = False) AndAlso (IsNothing(exDAD.ComResult) = False)) Then
                        'SetupMyLastCommResult(exDAD.ComResult, False)
                        If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.J2534_USB_UPDATE_REQUIRED) Then
                            sReturn = "UpdateReq"
                        End If
                    End If
                End Try
                sLocation = "Loop-A3"
                If (IsNothing(aRes) = False) Then SetupMyLastCommResult(aRes.CommResult, False)
                sLocation = "Loop-A4"
                If ((IsNothing(aRes) = False) AndAlso
                    (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success)) Then
                    bSuccess = True
                    If (IsNothing(sReturn) = True) Then sReturn = ""
                    Select Case aRes.Data
                        Case DrewTech.IIMClean.DT_CommunicationMode.Unknown
                            sReturn = "Unknown"
                        Case DrewTech.IIMClean.DT_CommunicationMode.USB
                            sReturn = "USB"
                        Case DrewTech.IIMClean.DT_CommunicationMode.Wireless
                            sReturn = "Wireless"
                        Case Else
                            sReturn = "Unknown-" & aRes.Data.ToString("X4")
                    End Select
                End If
            Catch ex As Exception
                sReturn = "Exception"
                Applog("Err:GetDrewDevices | " & ex.Message & "|Loc:" & sLocation)
            End Try
        Else
            sReturn = "NoDAD"
        End If

        Dim sTemp As String = "Nothing"
        If (IsNothing(sReturn) = False) Then sTemp = sReturn
        Applog("GetDrewDevices | Sts:" & MyLastResult() & "  Dev: " & sTemp)
        Return sReturn
    End Function


    Public Function MyLastResultSuccess() As Boolean
        Applog("... MyLastResultSuccess")

        Dim bReturn As Boolean = False
        If (MyLastResult().ToUpper.Contains("Success".ToUpper) = True) Then
            bReturn = True
        End If
        Return (bReturn)
    End Function

    Public Function CheckForUSBDevices() As String()

        Dim lStatus As String = Nothing

        Dim sReturn() As String = Nothing
        Dim sDevices() As DeviceInfoStruct = Nothing
        Dim iDeviceCount As Integer = 0
        Try
            'sReturn = CheckForDevice("IMclean", "USB")
            iDeviceCount = CheckForDevice("IMclean", "USB", "", sDevices, "")
            If ((iDeviceCount > 0) AndAlso (IsNothing(sDevices) = False) AndAlso (sDevices.Length > 0)) Then
                Dim DeviceNames As List(Of String) = New List(Of String)
                For Each sDeviceInfo As DeviceInfoStruct In sDevices
                    If (DeviceNames.Contains(sDeviceInfo.DeviceName) = False) Then
                        DeviceNames.Add(sDeviceInfo.DeviceName)
                    End If
                Next
                sReturn = DeviceNames.ToArray
            End If

        Catch ex As Exception
            lStatus = "Err:CheckForUSBDevices | " & ex.Message
        End Try

        Applog("... CheckForUSBDevices | Pass")
        Return (sReturn)

    End Function



    Private Function SetupOBDDeviceType(ByVal GenericResponse As DrewTech.IIMClean.DT_IGenericResponse(Of Integer)) As Boolean '-- borra

        Dim bReturn As Boolean = False
        Dim iCurrentType As Integer
        Dim iNewType As Integer = OBDLINK_TYPE_UNKNOWN
        Dim iWiredCount As Integer = 0
        Dim iBluetoothCount As Integer = 0
        Dim bCommSuccess As Boolean = False
        iCurrentType = 4171 '--OBDLINK_TYPE_DREWDADWIRED_GEN4  |  Val(RdXML(pathDevProfile, "/Profile/ObdLink/OBDLinkType")) ' OBDLinkType '-- Profile
        Dim sWiredDevices() As String = Nothing
        sWiredDevices = CheckForUSBDevices()
        If (IsNothing(sWiredDevices) = False) Then
            iWiredCount = sWiredDevices.Count
        End If
        If ((IsNothing(GenericResponse) = False) AndAlso
            (GenericResponse.CommResult = DrewTech.IIMClean.DT_Com_Result.Success)) Then
            bCommSuccess = True
            MyDeviceType = GenericResponse.Data
        End If
        If (MyDeviceType <> 0) Then
            Select Case MyDeviceType
                Case 2534
                    If (iWiredCount > 0) Then
                        iNewType = OBDLINK_TYPE_DREWDADWIRED_CA
                    Else
                        iNewType = OBDLINK_TYPE_DREWDADWIRELESS_CA
                    End If
                    bReturn = True
                Case 4165
                    If (iWiredCount > 0) Then
                        iNewType = OBDLINK_TYPE_DREWDADWIRED_GEN
                    Else
                        iNewType = OBDLINK_TYPE_DREWDADWIRELESS_GEN
                    End If
                    bReturn = True
                Case 4171
                    If (iWiredCount > 0) Then
                        iNewType = OBDLINK_TYPE_DREWDADWIRED_GEN4
                    Else
                        iNewType = OBDLINK_TYPE_DREWDADWIRELESS_GEN4
                    End If
                    bReturn = True
                Case 4183
                    iNewType = OBDLINK_TYPE_DREWDADWIRED_GDP3
                    bReturn = True
                Case 4147
                    If (iWiredCount > 0) Then
                        iNewType = OBDLINK_TYPE_DREWDADWIRED_GA
                    Else
                        iNewType = OBDLINK_TYPE_DREWDADWIRELESS_GA
                    End If
                    bReturn = True
                Case Else
                    iNewType = OBDLINK_TYPE_UNKNOWN
            End Select
            If (iCurrentType <> iNewType) Then
                ' If we found the wired device or had communications success and did not find the
                ' wired device than force the issue otherwise defer to current type
                If ((iNewType = OBDLINK_TYPE_DREWDADWIRED_GEN) Or
                    (iNewType = OBDLINK_TYPE_DREWDADWIRED_GEN4) Or
                    (iNewType = OBDLINK_TYPE_DREWDADWIRED_GDP3) Or
                    (iNewType = OBDLINK_TYPE_DREWDADWIRED_CA) Or
                    (iNewType = OBDLINK_TYPE_DREWDADWIRED_GA)) Then
                    MyDeviceType = iNewType
                Else
                    'MsgBox("0002 iNewType: " & iNewType)
                End If
            End If
        End If
        Applog("SetupOBDDeviceType |  Ret:" & bReturn.ToString &
                                          " USBCnt:" & iWiredCount.ToString("0") &
                                          " ComSuc:" & bCommSuccess.ToString &
                                          " CrntTyp:" & iCurrentType.ToString("0") &
                                          " Typ:" & MyDeviceType.ToString("0")) '&
        '"  LnkTyp:" & Me.OBDLinkTypeAsString)

        Applog("... SetupOBDDeviceType | " & bReturn)
        SetupOBDDeviceType = bReturn
    End Function


    Public Function InitializeInterface() As String
        Applog("... InitializeInterface")

        Static FirmwareVersionLast As String = ""
        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim sReturn As String = ""
        Dim sSuccess As String = ""
        Dim sTemp As String = ""
        Dim iState As Integer = 0
        Dim cres As DrewTech.IIMClean.DT_Com_Result = DrewTech.IIMClean.DT_Com_Result.InvalidData

        Dim idr As DrewTech.IIMClean.DT_IGenericResponse(Of Boolean) = Nothing
        Dim sres As DrewTech.IIMClean.DT_IGenericResponse(Of String) = Nothing
        Dim ires As DrewTech.IIMClean.DT_IGenericResponse(Of Integer) = Nothing

        Applog("InitializeInterface | 0 | Starting ... IsNothing(MyDad):" & IsNothing(MyDad).ToString &
                                           "  FOC:" & MyDAD_FullOpenClose.ToString.Substring(0, 1))
        iState = 10
        Try
            If (IsNothing(MyDad) = False) Then
                Applog("InitializeInterface | 0..1 ")
                Try
                    MyDad.Close()
                    If (MyDAD_FullOpenClose = True) Then MyDadOpen = False
                    MyDad = Nothing
                    'MyDadInitialized = False
                    System.Threading.Thread.Sleep(750)
                Catch ex As Exception
                End Try
                Applog("InitializeInterface | 0..2 ")
            End If

            Applog("InitializeInterface | 0..3 ")
            iState = 30
            MyDad = New DT.DAD.IMClean
            'MyDadInitialized = False

            Applog("InitializeInterface | 0..4 ")

            If (IsNothing(MyDad) = True) Then
                MyDAD_InitializeStatus = "NO_IMCLEAN"
                MyDAD_InitializeStatusDateTime = DateTime.Now
            End If
            Applog("InitializeInterface | 1 | State:" & iState.ToString("000") & " ... IsNothing(MyDad):" & IsNothing(MyDad).ToString &
                                               "  (" & DeltaTimeTicks(StartTicks, "Seconds").ToString("00.00") & ")")

            Do While ((InitializeInterfaceAbort = False) AndAlso (IsNothing(MyDad) = False))
                Applog("InitializeInterface | 1..1 ")
                iState = 40
                sTemp = GetIMCleanDriverVersion()
                If ((sTemp.Length <= 0) OrElse (sTemp = "?")) Then
                    MyDAD_InitializeStatus = "DRIVER_ERROR"
                    MyDAD_InitializeStatusDateTime = DateTime.Now
                End If
                If (InitializeInterfaceAbort = True) Then Exit Do
                iState = 50
                GetDLLVersionInternal()
                If (InitializeInterfaceAbort = True) Then Exit Do
                iState = 60
                MyDAD_SupportedProducts = GetDrewSupportedProducts()
                If (InitializeInterfaceAbort = True) Then Exit Do
                MyDAD_Devices = Nothing
                MyDAD_DevicesCount = 0
                MyDAD_DevicesTicks = 0
                'If (MyDAD_SupportedProducts.Length > 0) Then
                If (IsNothing(MyDAD_SupportedProducts) = False) Then
                    iState = 70
                    GetDrewDevices()
                    OBDLinkInfo_DeviceCurrentList = MyDAD_Devices
                    If (InitializeInterfaceAbort = True) Then Exit Do
                End If

                iState = 80
                MyDad.Close()
                MyDadOpen = False
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                If (OpenDAD_If_Needed("GetFirmwareVersion-Init") = True) Then
                    iState = 90
                    sres = MyDad.GetFirmwareVersion()
                    If (InitializeInterfaceAbort = True) Then Exit Do
                End If
                If (IsNothing(sres) = False) Then
                    SetupMyLastCommResult(sres.CommResult, False)
                    'If (sres.CommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                    If (IsCommResultSuccess(sres.CommResult) = True) Then
                        OBDLinkInfo_FirmwareVersion = sres.Data
                    End If
                Else
                    If (MyLastExtendedStatus = OBDLinkExtendedStatus.NotConnected) Then
                        MyDAD_InitializeStatus = "NOT_CONNECTED"
                        MyDAD_InitializeStatusDateTime = DateTime.Now
                    End If
                    If (MyLastExtendedStatus = OBDLinkExtendedStatus.UpdateRequired) Then
                        MyDAD_InitializeStatus = "UPDATE_REQUIRED"
                        MyDAD_InitializeStatusDateTime = DateTime.Now
                    End If
                End If
                Applog("InitializeInterface | State:" & iState.ToString("000") & " ... IsNothing(MyDad):" & IsNothing(MyDad).ToString &
                                                   "  (" & DeltaTimeTicks(StartTicks, "Seconds").ToString("00.00") & ")")

                Try
                    Dim iTemp As Integer = 2
                    If ((OBDLinkInfo_FirmwareVersion.Length > 0) AndAlso (OBDLinkInfo_FirmwareVersion <> FirmwareVersionLast)) Then
                        iState = 100
                        FirmwareVersionLast = OBDLinkInfo_FirmwareVersion
                        iTemp = 1
                    End If
                    Applog(iTemp & " InitializeInterface | State:" & iState.ToString("000") &
                                                           "  Sts0:" & MyLastResult() &
                                                           "  Drv:" & MyIMCleanDriverVersion.ToString("0000") &
                                                           "  FW:'" & OBDLinkInfo_FirmwareVersion & "'" &
                                                           "  (" & DeltaTimeTicks(StartTicks).ToString("00.00") & ")")
                Catch ex As Exception
                End Try

                sSuccess = "?"
                If ((MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.J2534_USB_UPDATE_REQUIRED) AndAlso
                    (MyDAD_AllowUpdateRequiredContinue = False)) Then
                    sReturn = "UpdateRequired"
                Else
                    cres = DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect
                    SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                    iState = 200
                    If (InitializeInterfaceAbort = True) Then Exit Do
                    DoDad(Sub() cres = MyDad.Open(), "Open", "NoEOL", "SupressAutoOpen,IgnoreOpen,SupressConnectCheck")
                    If (InitializeInterfaceAbort = True) Then Exit Do
                    iState = 210
                    If (IsNothing(cres) = False) Then
                        SetupMyLastCommResult(cres, False)
                        If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then MyDadOpen = True
                        sSuccess = MyLastResult()
                    End If
                    Applog("2 Result: " & sSuccess) 'applog(2, Nothing, " (Result:" & sSuccess & ")")
                End If

                iState = 240
                MyDAD_DeviceConnection = GetDrewDeviceConnection()
                Applog("InitializeInterface | State:" & iState.ToString("000") &
                                                   "  DeviceConnection:'" & MyDAD_DeviceConnection & "'" &
                                                   "  (Result:" & MyLastResult() & ")" &
                                                   "  (" & DeltaTimeTicks(StartTicks).ToString("00.00") & ")")
                If (InitializeInterfaceAbort = True) Then Exit Do

                sSuccess = "?"
                idr = Nothing
                ' Do not use reset result as last result
                'SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                iState = 250
                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.J2534_USB_UPDATE_REQUIRED) Then
                Else
                    DoDad(Sub() idr = MyDad.Reset(), "Reset", "NoEOL", "IgnoreOpen")
                    If (InitializeInterfaceAbort = True) Then Exit Do
                    If (IsNothing(idr) = False) Then
                        'SetupMyLastCommResult(idr.CommResult, False)
                        'sSuccess = MyLastResult()
                        sSuccess = idr.CommResult.ToString & "," & MyLastExtendedStatus.ToString
                    End If
                    Applog("2 Result: " & sSuccess) 'applog(2, Nothing, " (Result:" & sSuccess & ")")
                End If

                Dim bDeviceType As Boolean = False
                If (MyLastResultSuccess() = True) Then
                    ires = Nothing
                    SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                    iState = 260
                    If (OpenDAD_If_Needed("GetDeviceType") = True) Then
                        iState = 270
                        ires = MyDad.GetDeviceType
                    End If
                    If (IsNothing(ires) = False) Then
                        bDeviceType = True
                        SetupMyLastCommResult(ires.CommResult, False)
                        'MyDeviceType = ires.Data
                        iState = 280
                        SetupOBDDeviceType(ires)
                        sReturn = "Success"
                    End If
                End If
                If (bDeviceType = False) Then
                    ' Not from Drew so default device based on what we see from Windows
                    Dim sWiredDevices() As String = Nothing
                    'sWiredDevices = CheckForDevice("IMclean", "USB")
                    sWiredDevices = CheckForUSBDevices()
                    If ((IsNothing(sWiredDevices) = False) AndAlso (sWiredDevices.Count > 0)) Then
                        MsgBox("0001 NO SE DETECTAN DISPOSITIVOS OBD ")
                    End If
                End If
                If (InitializeInterfaceAbort = True) Then Exit Do


                If (MyLastResultSuccess() = True) Then
                    If (OpenDAD_If_Needed("GetMaxWaitTime") = True) Then
                        iState = 300
                        MyDAD_InitialMaxWaitTime = MyDad.MaxWaitTime
                        Applog("                     (MaxWaitTime: " & MyDAD_InitialMaxWaitTime.ToString("0000") & ")")
                    End If
                End If

                If (InitializeInterfaceAbort = True) Then Exit Do
                'bReturn = True
                Exit Do
            Loop

            Dim sTemp1 As String = "Nothing"
            Dim sTemp2 As String = "Nothing"
            Try
                If (IsNothing(cres) = False) Then sTemp1 = cres.ToString
                If (IsNothing(idr) = False) Then sTemp2 = idr.CommResult.ToString
            Catch ex As Exception
            End Try
            Try
                Applog("InitializeInterface | Ret:'" & sReturn & "'" &
                                                   " IIA:" & InitializeInterfaceAbort.ToString.Substring(0, 1) &
                                                   "  Sts1:" & MyLastResult() &
                                                   "  Typ:" & MyDeviceType &
                                                   "  OpnSts:" & sTemp1 &
                                                   "  RstSts:" & sTemp2 &
                                                   "  (" & DeltaTimeTicks(StartTicks).ToString("00.00") & ")")
            Catch ex As Exception
            End Try
        Catch ex As Exception
            'MyDad = Nothing
            ''MyDadInitialized = False
            sTemp = "?"
            Try
                sTemp = DirectCast(ex, DrewTech.IIMClean.DADException).ComResult.ToString
            Catch ex2 As Exception
            End Try
            sReturn = "EXCEPTION"
            Applog("InitializeInterface |" & ex.Message & "'  ComReslt:'" & sTemp & "')")
            'ts.TraceEvent(TraceEventType.Critical, 0, "InitializeInterface error", ex.Message)
            Throw New ApplicationException(String.Format("ObdLinkDrewDAD::OnCreate exception {0}", ex.Message))
        End Try
        'MyDadInitialized = bReturn
        InitializeInterface = sReturn
    End Function


    Public Function ReleaseDAD(Optional ByVal sLocation As String = "") As Boolean
        'Applog("... ReleaseDAD")

        Static lCount As Long = 0
        Dim bReturn As Boolean = False
        Dim sMessage As String = "?"
        lCount = lCount + 1
        If (lCount > 99999) Then lCount = 9
        Applog("ReleaseDAD |  Starting ... Cnt:" & lCount.ToString("00000") & "," & MyDADAccessCount.ToString("00") & " (" & sLocation & ")")
        MyDADLastReleaseLocation = sLocation
        Try
            If (MyDADSemaphore.Release(1) = 0) Then
                MyDADAccessCount = MyDADAccessCount - 1
                If (MyDADAccessCount < -99) Then MyDADAccessCount = -99
                sMessage = "Success ..."
                bReturn = True
            Else
                sMessage = "Failure ..."
                bReturn = False
            End If
        Catch ex As Exception
            sMessage = "ex:'" & ex.Message & "'"
            bReturn = False
        End Try
        Dim iTemp As Integer = 5
        If (bReturn = False) Then iTemp = 1
        Applog("... ReleaseDAD | " & sMessage & " Cnt:" & lCount.ToString("00000") & "," & MyDADAccessCount.ToString("00") &
                                      " Opn:" & Microsoft.VisualBasic.Left(MyDadOpen.ToString, 1) &
                                      " (" & sLocation & ", " & MyDADLastGetLocation & ")")
        ReleaseDAD = bReturn
    End Function


    Public Function ResetConnectionInternal(ByRef oParams() As Object) As Boolean
        'Applog("... ResetConnectionInternal")

        Static iCount As Integer = 0
        Dim StartTicks As Int32 = System.Environment.TickCount
        'Dim sReturn As String = ""
        Dim CallingProcedure As String = LocalGetCallingProcedure()
        Dim bSuccess As Boolean = False
        Dim sDebugStatus As String = ""
        Dim sDoDadControl As String = ""
        Dim cres As DrewTech.IIMClean.DT_Com_Result = Nothing

        Dim rres As DrewTech.IIMClean.DT_IGenericResponse(Of Boolean) = Nothing '-- #If ((DREW_GENERIC) Or (DREW_GENERIC_CA))

        Dim bDoIt As Boolean = False
        Dim sControl As String = ""
        Dim bGetDADFlag As Boolean = False
        '
        Try
            iCount = iCount + 1
            If (iCount > 99) Then iCount = 99
            Try
                If (IsNothing(oParams) = False) Then
                    If (oParams.Length > 0) Then
                        bDoIt = CBool(oParams(0))
                    End If
                    If (oParams.Length > 1) Then
                        sControl = CStr(oParams(1))
                    End If
                End If
            Catch ex As Exception
                Debug.WriteLine("ex:'" & ex.Message & "'")
            End Try
            '
            Applog("ResetConnectionInternal " & iCount.ToString("00") & "-bDoIt:" & bDoIt.ToString.Substring(0, 1) &
                                                   "  sCtrl:'" & sControl & "'" &
                                                   "  (" & sDebugStatus & ")" & "  (" & MyLastResult() & ", (" & CallingProcedure & ")")
            '
            If (sControl.ToUpper.Contains("NOGETDAD".ToUpper) = True) Then
            Else
                If (GetDAD("ResetConnection") = False) Then
                    bDoIt = False
                Else
                    bGetDADFlag = True
                End If
            End If
            '
            If (bDoIt = True) Then
                'If (sControl.ToUpper.Contains("DoDadControl".ToUpper) = True) Then
                '    sDoDadControl = ""
                'End If
                '
                OBDLinkInfo_DLLVersion = ""
                OBDLinkInfo_DriverVersion = ""
                OBDLinkInfo_FirmwareVersion = ""
                OBDLinkInfo_LatestFirmwareVersion = ""
                OBDLinkInfo_SerialNumber = ""
                OBDLinkInfo_VehicleIsLinked = ""
                '
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.Success, False)

                '
                'If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                If (IsCommResultSuccess(MyLastCommResult) = True) Then
                    Try
                        If ((sControl.ToUpper.Contains("SIMPLERESET".ToUpper) = True) OrElse
                            (sControl.ToUpper.Contains("RESETONLY".ToUpper) = True)) Then
                            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.InvalidData, False)
                            DoDad(Sub() rres = MyDad.Reset(), "Reset", "NoEOL", "IgnoreOpen" & sDoDadControl)
                            If (IsNothing(rres) = False) Then SetupMyLastCommResult(rres.CommResult, False)
                            Applog(" (Result:" & MyLastResult() & ")")
                        Else
                            If (sControl.ToUpper.Contains("NORESET".ToUpper) = False) Then
                                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.InvalidData, False)
                                DoDad(Sub() rres = MyDad.Reset(), "Reset", "NoEOL", "IgnoreOpen" & sDoDadControl)
                                If (IsNothing(rres) = False) Then SetupMyLastCommResult(rres.CommResult, False)
                                Applog(" (Result:" & MyLastResult() & ")")
                                System.Threading.Thread.Sleep(200)
                            End If
                            '
                            If ((sControl.ToUpper.Contains("NOCLOSEOPEN".ToUpper) = False) And
                                (sControl.ToUpper.Contains("NOOPENCLOSE".ToUpper) = False)) Then
                                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.InvalidData, False)
                                DoDad(Sub() MyDad.Close(), "Close-RC", , "SupressAutoOpen" & sDoDadControl)
                                If (MyDAD_FullOpenClose = True) Then MyDadOpen = False
                                System.Threading.Thread.Sleep(400)
                                '
                                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.InvalidData, False)
                                cres = DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect
                                DoDad(Sub() cres = MyDad.Open(), "Open", "NoEOL", "SupressAutoOpen,IgnoreOpen" & sDoDadControl)
                                If (sDebugStatus.Length > 0) Then sDebugStatus = sDebugStatus & " "
                                If (IsNothing(cres) = False) Then
                                    sDebugStatus = sDebugStatus & "Open:" & cres.ToString
                                    SetupMyLastCommResult(cres, False)
                                Else
                                    sDebugStatus = sDebugStatus & "Open:" & "Nothing"
                                End If
                                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then MyDadOpen = True
                                Applog(" (Result:" & MyLastResult() & ")")
                            End If
                            '
                            'If (cres = DT_Com_Result.Success) Then
                            If (sControl.ToUpper.Contains("NORESET".ToUpper) = True) Then
                                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.Success, False)
                            Else
                                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.InvalidData, False)
                                DoDad(Sub() rres = MyDad.Reset(), "Reset", "NoEOL" & sDoDadControl, "IgnoreOpen" & sDoDadControl)
                                If (sDebugStatus.Length > 0) Then sDebugStatus = sDebugStatus & " "
                                If (IsNothing(rres) = False) Then
                                    sDebugStatus = sDebugStatus & "Reset:" & rres.CommResult.ToString
                                    SetupMyLastCommResult(rres.CommResult, False)
                                Else
                                    sDebugStatus = sDebugStatus & "Reset:" & "Nothing"
                                End If
                                Applog(" (Result:" & MyLastResult() & ")")
                                ' If reset is success then use the open result
                                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                                    If (IsNothing(cres) = False) Then SetupMyLastCommResult(cres, False)
                                End If
                            End If
                        End If
                        If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then bSuccess = True
                        'End If
                    Catch ex As Exception
                        Applog("Err:ResetConnectionInternal | " & ex.Message & "'" &
                                                               "  Cntrl:'" & sControl & "'" & " (" & MyDADStatus() & ")" & " (" & CallingProcedure & ")")
                    End Try
                End If
            End If
        Catch ex As Exception
            Applog("Err:ResetConnectionInternal | " & ex.Message & "'" &
                                                   "  Cntrl:'" & sControl & "'" & " (" & MyDADStatus() & ")" & " (" & CallingProcedure & ")")
        Finally
            If (bGetDADFlag = True) Then
                ReleaseDAD("ResetConnection")
            End If
        End Try
        '
        Dim iTemp As Integer = 3
        If (bSuccess = False) Then iTemp = 1

        Applog("... ResetConnectionInternal " & iCount.ToString("00") & "-Ret:" & bSuccess.ToString &
                                                   "  sCtrl:'" & sControl & "'" &
                                                   "  (" & sDebugStatus & ")" & "  (" & MyLastResult() & ", " & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
        'sReturn = bSuccess.ToString & ", " & sDebugStatus
        Return (bSuccess)

    End Function


    Public Function GetDrewDeviceCount(Optional ByVal sControl As String = "") As Integer
        Applog("... GetDrewDeviceCount")

        Dim CheckSeconds As Double = 4.0    ' No need to check devices too frequently / 6.0

        If (DeltaTimeTicks(MyDAD_DevicesTicks) > CheckSeconds) Then
            GetDrewDevices(sControl)
        End If
        OBDLinkInfo_DeviceCurrentList = MyDAD_Devices

        Applog("... GetDrewDeviceCount | " & MyDAD_DevicesCount)
        Return (MyDAD_DevicesCount)
    End Function



    Private Function GetBatteryData() As Boolean
        Applog("... GetBatteryData")

        Dim iReadCount As Integer = 0
        Dim iSuccessCount As Integer = 0
        Dim bSuccess As Boolean = False

        Dim iData As Integer
        Dim sres As DrewTech.IIMClean.DT_IGenericResponse(Of Integer) = Nothing

        sres = Nothing
        iData = 0

        If (GetDrewDeviceCount() > 0) Then
            iReadCount = iReadCount + 1
            bSuccess = False
            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.NotSupported, False)
            DoDad(Sub() sres = MyDad.BatteryStatus(DrewTech.IIMClean.DT_BatteryIOCtl.DT_IOCTL_BATT_CMD_CYCLE_COUNT), "BatteryCycleCount", "NoEOL")
            If (IsNothing(sres) = False) Then
                SetupMyLastCommResult(sres.CommResult, False, True)
                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                    bSuccess = True
                    iSuccessCount = iSuccessCount + 1
                    iData = sres.Data
                    OBDBatteryInformation.BatteryCycleCount = CSng(iData)
                    OBDBatteryInformation.BatteryCycleCountUpdateDateTime = DateTime.Now
                    OBDBatteryInformation.BatteryCycleCountAvailable = True
                End If
            End If
            Applog("(Cnt:" & iReadCount.ToString("0") & "-Success:" & bSuccess.ToString & ", '" & iData.ToString & "')")
        End If

        sres = Nothing
        iData = 0
        If (bSuccess = True) Then
            iReadCount = iReadCount + 1
            bSuccess = False
            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.NotSupported, False)
            DoDad(Sub() sres = MyDad.BatteryStatus(DrewTech.IIMClean.DT_BatteryIOCtl.DT_IOCTL_BATT_CMD_READ_SOC), "BatteryStateOfCharge", "NoEOL")
            If (IsNothing(sres) = False) Then
                SetupMyLastCommResult(sres.CommResult, False, True)
                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                    bSuccess = True
                    iSuccessCount = iSuccessCount + 1
                    iData = sres.Data
                    OBDBatteryInformation.BatteryStateOfCharge = CSng(iData)
                    OBDBatteryInformation.BatteryStateOfChargeUpdateDateTime = DateTime.Now
                    OBDBatteryInformation.BatteryStateOfChargeAvailable = True
                End If
            End If
            Applog("(Cnt:" & iReadCount.ToString("0") & "-Success:" & bSuccess.ToString & ", '" & iData.ToString & "')")
        End If

        sres = Nothing
        iData = 0
        If (bSuccess = True) Then
            iReadCount = iReadCount + 1
            bSuccess = False
            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.NotSupported, False)
            DoDad(Sub() sres = MyDad.BatteryStatus(DrewTech.IIMClean.DT_BatteryIOCtl.DT_IOCTL_BATT_CMD_READ_SOH), "BatteryHealth", "NoEOL")
            If (IsNothing(sres) = False) Then
                SetupMyLastCommResult(sres.CommResult, False, True)
                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                    bSuccess = True
                    iSuccessCount = iSuccessCount + 1
                    iData = sres.Data
                    OBDBatteryInformation.BatteryHealth = CSng(iData)
                    OBDBatteryInformation.BatteryHealthUpdateDateTime = DateTime.Now
                    OBDBatteryInformation.BatteryHealthAvailable = True
                End If
            End If
            Applog("(Cnt:" & iReadCount.ToString("0") & "-Success:" & bSuccess.ToString & ", '" & iData.ToString & "')")
        End If

        sres = Nothing
        iData = 0
        If (bSuccess = True) Then
            iReadCount = iReadCount + 1
            bSuccess = False
            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.NotSupported, False)
            DoDad(Sub() sres = MyDad.BatteryStatus(DrewTech.IIMClean.DT_BatteryIOCtl.DT_IOCTL_BATT_CMD_READ_TEMP), "BatteryTemp", "NoEOL")
            If (IsNothing(sres) = False) Then
                SetupMyLastCommResult(sres.CommResult, False, True)
                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                    bSuccess = True
                    iSuccessCount = iSuccessCount + 1
                    iData = sres.Data
                    OBDBatteryInformation.BatteryTemperature = CSng(iData)
                    If (OBDBatteryInformation.BatteryTemperature > 0.0) Then
                        OBDBatteryInformation.BatteryTemperature = CSng(OBDBatteryInformation.BatteryTemperature - 273.15)
                        OBDBatteryInformation.BatteryTemperatureUpdateDateTime = DateTime.Now
                        OBDBatteryInformation.BatteryTemperatureAvailable = True
                    End If
                End If
            End If
            Applog("(Cnt:" & iReadCount.ToString("0") & "-Success:" & bSuccess.ToString & ", '" & iData.ToString & "')")
        End If

        sres = Nothing
        iData = 0
        If (bSuccess = True) Then
            iReadCount = iReadCount + 1
            bSuccess = False
            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.NotSupported, False)
            DoDad(Sub() sres = MyDad.BatteryStatus(DrewTech.IIMClean.DT_BatteryIOCtl.DT_IOCTL_BATT_CMD_READ_CURRENT), "BatteryCurrent", "NoEOL")
            If (IsNothing(sres) = False) Then
                SetupMyLastCommResult(sres.CommResult, False, True)
                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                    bSuccess = True
                    iSuccessCount = iSuccessCount + 1
                    iData = sres.Data
                    OBDBatteryInformation.BatteryCurrent = CSng(iData)
                    OBDBatteryInformation.BatteryCurrent = CSng(OBDBatteryInformation.BatteryCurrent / 10.0)
                    OBDBatteryInformation.BatteryCurrentUpdateDateTime = DateTime.Now
                    OBDBatteryInformation.BatteryCurrentAvailable = True
                End If
            End If
            Applog("(Cnt:" & iReadCount.ToString("0") & "-Success:" & bSuccess.ToString & ", '" & iData.ToString & "')")
        End If

        Applog("GetBatteryData | Sts:" & MyLastResult() & "  Rcnt:" & iReadCount.ToString & " SCnt:" & iSuccessCount.ToString)
        Return True

    End Function



    Public Function GetDeviceType(ByVal sDebug As String) As String

        Dim sReturn As String = "?"
        Dim bSuccess As Boolean = False
        Try

            Dim ires As DrewTech.IIMClean.DT_IGenericResponse(Of Integer) = Nothing

            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
            DoDad(Sub() ires = MyDad.GetDeviceType, "GetDeviceType" & sDebug, "")
            If (IsNothing(ires) = False) Then
                SetupMyLastCommResult(ires.CommResult, False)
                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                    bSuccess = True
                    'MyDeviceType = ires.Data
                    SetupOBDDeviceType(ires)
                    sReturn = MyDeviceType.ToString("0000")
                End If
            End If
        Catch ex As Exception
            Applog("Err:GetDeviceType | " & ex.Message)
        End Try
        'Applog("GetDeviceType | Sts:" & MyLastResult() & "  Typ:" & MyDeviceType.ToString("0") & " (" & OBDLinkTypeAsString & ")")

        Applog("... GetDeviceType | " & sReturn)
        Return sReturn
    End Function



    Private Function GetDADPort() As String

        Dim sReturn As String = ""
        Try
            MyDADPort = MyDad.Port
            If ((IsNothing(MyLastCommResult) = True) OrElse
                (MyLastCommResult <> DrewTech.IIMClean.DT_Com_Result.Success)) Then
                MyDADPort = "Unknown"
            End If
            If (MyDADPort.ToUpper.Contains("USB".ToUpper) = True) Then
                Dim sWiredDevices() As String = Nothing
                sWiredDevices = CheckForUSBDevices()
                If ((IsNothing(sWiredDevices) = False) AndAlso (sWiredDevices.Count > 0)) Then
                Else
                    'Dim sBluetoothDevices() As String = Nothing
                    'sBluetoothDevices = CheckForBluetoothDevices()
                    'If ((IsNothing(sBluetoothDevices) = False) AndAlso (sBluetoothDevices.Count > 0)) Then
                    ' MyDADPort = "Wireless"
                    'Else
                    'End If
                End If
            End If
            sReturn = MyDADPort
        Catch ex As Exception
            Applog("Err:GetDADPort | " & ex.Message)
        End Try

        Applog("... GetDADPort | " & sReturn)
        Return sReturn
    End Function


    Public Function DecrementActiveCount() As Integer

        MyOBDLinkActiveCount = MyOBDLinkActiveCount - 1
        If (MyOBDLinkActiveCount < -999) Then MyOBDLinkActiveCount = -99

        Applog("... DecrementActiveCount | " & MyOBDLinkActiveCount)
        Return (MyOBDLinkActiveCount)
    End Function


    Private Function LocalDecrementActiveCount(ByVal sLogInfo As String) As Integer

        Static LastCallingProcedure As String = ""
        Static LastsLogInfo As String = ""
        Dim iReturn As Integer
        iReturn = DecrementActiveCount()
        Dim CallingProcedure As String = LocalGetCallingProcedure(, True)
        If (iReturn <> 0) Then
            Applog("DAC | Warning ... Active_count:" & iReturn.ToString("0") &
                               "  (" & CallingProcedure & ", " & sLogInfo & ")" &
                               " L:(" & LastCallingProcedure & ", " & LastsLogInfo & ")")
        End If
        LastCallingProcedure = CallingProcedure
        LastsLogInfo = sLogInfo

        Applog("... LocalDecrementActiveCount | " & iReturn)
        Return (iReturn)
    End Function


    Public Function GetDLLVersion() As String
        Applog("... GetDLLVersion")

        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim CallingProcedure As String = LocalGetCallingProcedure()
        Static versionLast As String = ""
        Dim version As String = ""
        If (MyIMCleanDriverDLLVersion.Length > 0) Then
            version = MyIMCleanDriverDLLVersion
            Dim iTemp As Integer = 3
            If ((version.Length > 0) AndAlso (version <> versionLast)) Then
                versionLast = version
                iTemp = 1
            End If
            Applog("GetDLLVersion | Ending ..." & " Ver:'" & version & "'" & " (" & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
        Else
            LocalIncrementActiveCount(CallingProcedure)
            Applog("GetDLLVersion | Starting ..." & " (" & MyDADStatus() & ")" & " (" & CallingProcedure & ")")
            Dim bSuccess As Boolean = False

            Dim sres As DrewTech.IIMClean.DT_IGenericResponse(Of String) = Nothing

            'If (GetDAD("GetDLLVersion") = True) Then
            Try
                DoDad(Sub() sres = MyDad.GetFirmwareVersion(), "GetDLLVersion", "NoEOL")
            Catch ex As Exception
                Applog("Err:GetDLLVersion | " & ex.Message)
            End Try
            If (IsNothing(sres) = False) Then
                SetupMyLastCommResult(sres.CommResult, False)
                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                    bSuccess = True
                    version = sres.Data
                End If
            End If
            Applog(" (Success:" & bSuccess.ToString & ", '" & version & "')")
            'ReleaseDAD("GetDLLVersion")
            'End If
            Dim iTemp As Integer = 3
            If ((version.Length > 0) AndAlso (version <> versionLast)) Then
                versionLast = version
                iTemp = 1
            End If
            Applog("GetDLLVersion | Ending ..." & " Ver:'" & version & "'" & " (" & MyLastResult() & ", " & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
            LocalDecrementActiveCount(CallingProcedure)
        End If
        Return version
    End Function


    Private Function GetFirmwareVersion() As String

        Static fvLast As String = ""
        Dim fv As String = ""
        Dim bSuccess As Boolean = False

        Dim sres As DrewTech.IIMClean.DT_IGenericResponse(Of String) = Nothing

        SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
        DoDad(Sub() sres = MyDad.GetFirmwareVersion(), "GetFirmwareVersion", "NoEOL")
        If (IsNothing(sres) = False) Then
            SetupMyLastCommResult(sres.CommResult, False)
            If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                bSuccess = True
                fv = sres.Data
            End If
        End If
        Applog(" (Success:" & bSuccess.ToString & ", '" & fv & "')")
        Dim iTemp As Integer = 3
        If ((fv.Length > 0) AndAlso (fv <> fvLast)) Then
            fvLast = fv
            iTemp = 1
        End If
        Applog("GetFirmwareVersion | Sts:" & MyLastResult() & "  Ver:'" & fv & "'")

        Applog("... GetFirmwareVersion | " & fv)
        Return fv
    End Function


    Private Function GetSerialNumber() As String

        Dim fv As String = ""
        Dim bSuccess As Boolean = False

        Dim sres As DrewTech.IIMClean.DT_IGenericResponse(Of String) = Nothing

        SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
        DoDad(Sub() sres = MyDad.GetSerialNumber(), "GetSerialNumber", "NoEOL")
        If (IsNothing(sres) = False) Then
            SetupMyLastCommResult(sres.CommResult, False)
            If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                bSuccess = True
                fv = sres.Data
            End If
        End If
        Applog(" (Success:" & bSuccess.ToString & ", '" & fv & "')")

        Applog("... GetSerialNumber | Sts:" & MyLastResult() & "  SN:'" & fv & "'")
        Return fv
    End Function


    Public Function OBDLinkTypeAsString(Optional ByVal iOBDLinkType As Integer = OBDLINK_TYPE_UNKNOWN) As String

        Dim iTemp As Integer = OBDLINK_TYPE_UNKNOWN
        If (iOBDLinkType <> OBDLINK_TYPE_UNKNOWN) Then
            iTemp = iOBDLinkType
        Else
            iTemp = OBDLinkType
        End If
        Select Case (iTemp)
            Case OBDLINK_TYPE_9020WIRED
                OBDLinkTypeAsString = OBDLINK_NAME_9020WIRED
            Case OBDLINK_TYPE_9020WIRELESS
                OBDLinkTypeAsString = OBDLINK_NAME_9020WIRELESS

            Case OBDLINK_TYPE_DREWDADWIRED_GEN
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRED_GEN
            Case OBDLINK_TYPE_DREWDADWIRELESS_GEN
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRELESS_GEN

            Case OBDLINK_TYPE_DREWDADWIRED_GEN4
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRED_GEN4
            Case OBDLINK_TYPE_DREWDADWIRELESS_GEN4
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRELESS_GEN4

            Case OBDLINK_TYPE_DREWDADWIRED_GDP3
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRED_GDP3
            Case OBDLINK_TYPE_DREWDADWIRELESS_GDP3
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRELESS_GDP3

            Case OBDLINK_TYPE_DREWDADWIRED_CA
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRED_CA
            Case OBDLINK_TYPE_DREWDADWIRELESS_CA
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRELESS_CA

            Case OBDLINK_TYPE_DREWDADWIRED_GA
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRED_GA
            Case OBDLINK_TYPE_DREWDADWIRELESS_GA
                OBDLinkTypeAsString = OBDLINK_NAME_DREWDADWIRELESS_GA

                'Case OBDLINK_TYPE_SIMULATOR
                '   OBDLinkTypeAsString = OBDLINK_NAME_SIMULATOR
            Case Else
                OBDLinkTypeAsString = "Unknown-(" & iTemp.ToString("0") & ")"
        End Select

        Applog("... OBDLinkTypeAsString | " & OBDLinkTypeAsString)

    End Function



    Public Function ValidateFirmwareVersion(ByVal sVersion As String) As Boolean

        Dim bReturn As Boolean = False
        Try
            If ((sVersion.Length >= 3) AndAlso (sVersion.Length <= 16)) Then
                Dim idx As Integer = 0
                For idx = 0 To sVersion.Length - 1
                    If ((sVersion.Substring(idx, 1) < "!") Or (sVersion.Substring(idx, 1) > "~")) Then Exit For
                Next
                If (idx >= sVersion.Length) Then
                    bReturn = True
                End If
            End If
        Catch ex As Exception
        End Try

        Applog("... ValidateFirmwareVersion: " & bReturn)
        Return (bReturn)
    End Function


    Public Function OBDLinkTargetFirmware(Optional ByVal iOBDLinkType As Integer = OBDLINK_TYPE_UNKNOWN) As String
        Applog("... OBDLinkTargetFirmware")

        Dim sReturn As String = "?"
        Dim sTemp As String
        If (iOBDLinkType <> OBDLINK_TYPE_UNKNOWN) Then
            sTemp = OBDLINK_FIRMWARE_KEY_BASE & OBDLinkTypeAsString(iOBDLinkType)
        Else
            sTemp = OBDLINK_FIRMWARE_KEY_BASE & OBDLinkTypeAsString()
        End If
        If (sTemp.ToUpper.Contains("Unknown".ToUpper) = True) Then
            sTemp = OBDLINK_FIRMWARE_KEY_BASE & "Unknown"
        End If

        'MsgBox(sTemp) '-- mja

        If (ValidateFirmwareVersion(sTemp) = True) Then sReturn = sTemp

        OBDLinkTargetFirmware = sReturn

        If (ValidateFirmwareVersion(lvalue) = True) Then
            If (iOBDLinkType <> OBDLINK_TYPE_UNKNOWN) Then
                sTemp = OBDLINK_FIRMWARE_KEY_BASE & OBDLinkTypeAsString(iOBDLinkType)
            Else
                sTemp = OBDLINK_FIRMWARE_KEY_BASE & OBDLinkTypeAsString()
            End If

            '-- Call WrXML(pathDevProfile, "/Profile/ObdLink/FirmwareDrewDAD_GEN-Wired", value)

        Else
            MsgBox("Invalid OBD link target firmware ..." & vbCrLf & "(" & lvalue & ")", MsgBoxStyle.OkOnly, "OBDLink")
        End If

    End Function



    Public Function GetOBDLinkInformationInternal(Optional ByVal sControl As String = "") As String 'OBDLinkInformation   borra
        Applog("... GetOBDLinkInformationInternal")

        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim CallingProcedure As String = LocalGetCallingProcedure()
        '
        If (sControl.ToUpper.Contains("NoUpdate".ToUpper) = True) Then
            ' No update requested so just return current (old) information
        Else
            'If (OBDLinkInfo.Port = "") Then OBDLinkInfo.Port = "?"
            OBDLinkInfo_PortWired = ""
            OBDLinkInfo_PortWireless = ""
            OBDLinkInfo_InitializationStatus = MyDAD_InitializeStatus
            OBDLinkInfo_InitializationStatusDateTime = MyDAD_InitializeStatusDateTime
            OBDLinkInfo_BatteryInformationAvailable = False
            OBDLinkInfo_DeviceInformationAvailable = False
            '
            If (GetDAD("GetOBDLinkInfoInternal") = True) Then
                GetDeviceType("-" & CallingProcedure)     ' Refresh the device type
                Try
                    'If (MyLastResultSuccess() = True) Then
                    'If ((OBDLinkInfo.Port = "") Or (OBDLinkInfo.Port = "?")) Then OBDLinkInfo.Port = GetDADPort()
                    OBDLinkInfo_Port = GetDADPort()
                    'End If

                    'If (MyLastResultSuccess() = True) Then
                    If ((OBDLinkInfo_DLLVersion = "") Or (OBDLinkInfo_DLLVersion = "?")) Then OBDLinkInfo_DLLVersion = GetDLLVersion()
                    'End If
                    If (OBDLinkInfo_DLLVersion = "") Then OBDLinkInfo_DLLVersion = "?"

                    If (MyLastResultSuccess() = True) Then
                        If ((OBDLinkInfo_FirmwareVersion = "") Or (OBDLinkInfo_FirmwareVersion = "?")) Then OBDLinkInfo_FirmwareVersion = GetFirmwareVersion()
                    End If
                    If (OBDLinkInfo_FirmwareVersion = "") Then OBDLinkInfo_FirmwareVersion = "?"

                    If (MyLastResultSuccess() = True) Then
                        If ((OBDLinkInfo_SerialNumber = "") Or (OBDLinkInfo_SerialNumber = "?")) Then OBDLinkInfo_SerialNumber = GetSerialNumber()
                    End If
                    If (OBDLinkInfo_SerialNumber = "") Then OBDLinkInfo_SerialNumber = "?"

                    If (OBDLinkInfo_FirmwareVersion.Length >= 3) Then

                        'Call WrXML(pathDevProfile, "/Profile/ObdLink/FirmwareDrewDAD_GEN-Wired", OBDLinkInfo_FirmwareVersion)

                    End If
                    OBDLinkInfo_LatestFirmwareVersion = OBDLinkTargetFirmware(OBDLinkType)

                    If ((MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.DADNotConnected) Or
                        (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.DADTimedOut) Or
                        (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.VehicleNotConnected) Or
                        (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.VehicleTimedOut) Or
                        (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.VehicleError)) Then
                        SetupVehicleIsLinked("False")
                    End If
                    '
                    If ((OBDLinkInfo_DriverVersion = "") Or (OBDLinkInfo_DriverVersion = "?")) Then OBDLinkInfo_DriverVersion = GetIMCleanDriverVersion()
                    If (OBDLinkInfo_DriverVersion = "") Then OBDLinkInfo_DriverVersion = "?"

                    If ((MyLastResultSuccess() = True) AndAlso (sControl.ToUpper.Contains("Battery".ToUpper) = True)) Then

                        If GetBatteryData() Then
                            OBDLinkInfo_BatteryStateOfCharge = OBDBatteryInformation.BatteryStateOfCharge
                            OBDLinkInfo_BatteryCycleCount = OBDBatteryInformation.BatteryCycleCount
                            OBDLinkInfo_BatteryHealth = OBDBatteryInformation.BatteryHealth
                            OBDLinkInfo_BatteryTemperature = OBDBatteryInformation.BatteryTemperature
                            OBDLinkInfo_BatteryCurrent = OBDBatteryInformation.BatteryCurrent
                            Applog("GetOBDLinkInfoInternal | BatCC:" & OBDBatteryInformation.BatteryCycleCount.ToString("0") & Chr(13) &
                                                                  " BatSOC:" & OBDBatteryInformation.BatteryStateOfCharge.ToString("0") & "%" & Chr(13) &
                                                                  " BatHlt:" & OBDBatteryInformation.BatteryHealth.ToString("0") & "%" & Chr(13) &
                                                                  " BatTmp:" & OBDBatteryInformation.BatteryTemperature.ToString("0") & "degC" & Chr(13) &
                                                                  " BatAMP:" & OBDBatteryInformation.BatteryCurrent.ToString("0") & "mA")
                            If ((OBDBatteryInformation.BatteryCycleCountAvailable = True) Or
                                (OBDBatteryInformation.BatteryStateOfChargeAvailable = True) Or
                                (OBDBatteryInformation.BatteryHealthAvailable = True) Or
                                (OBDBatteryInformation.BatteryTemperatureAvailable = True)) Then
                                OBDLinkInfo_BatteryInformationAvailable = True
                                OBDLinkInfo_BatteryInformationUpdateDateTime = OBDBatteryInformation.BatteryStateOfChargeUpdateDateTime
                            End If
                        End If

                    Else

                        OBDLinkInfo_BatteryInformationAvailable = False
                        OBDLinkInfo_BatteryStateOfCharge = 0.0
                        OBDLinkInfo_BatteryCycleCount = 0.0
                        OBDLinkInfo_BatteryHealth = 0.0
                        OBDLinkInfo_BatteryTemperature = 0.0
                        '
                        OBDLinkInfo_DeviceInformationAvailable = False
                        OBDLinkInfo_DeviceSupportList = Nothing
                        OBDLinkInfo_DeviceCurrentList = Nothing
                        OBDLinkInfo_DeviceConnection = ""
                    End If

                    If (sControl.ToUpper.Contains("DeviceInfo".ToUpper) = True) Then
                        MyDAD_SupportedProducts = GetDrewSupportedProducts()
                        OBDLinkInfo_DeviceSupportList = MyDAD_SupportedProducts
                        GetDrewDevices()
                        OBDLinkInfo_DeviceCurrentList = MyDAD_Devices
                        MyDAD_DeviceConnection = GetDrewDeviceConnection()
                        OBDLinkInfo_DeviceConnection = MyDAD_DeviceConnection
                        OBDLinkInfo_DeviceInformationAvailable = True
                        OBDLinkInfo_DeviceInformationUpdateDateTime = DateTime.Now
                    End If

                Catch ex As Exception
                    Applog("Err:GetOBDLinkInfoInternal | " & ex.Message & "'" & "  (" & MyLastResult() & ", " & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
                End Try
                ReleaseDAD("GetOBDLinkInfoInternal")
            End If
        End If
        Return "Pass" '(OBDLinkInfo) MJA
    End Function


    Public Function GetVoltageInternal(ByVal bDLC As Boolean, ByVal sControl As String) As Single

        Static lCount As Integer = 0
        Static LastCallingProcedure As String = ""
        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim CallingProcedure As String = LocalGetCallingProcedure()
        Dim rValue As Single = -1.0
        Dim iErrorCount As Integer = 0
        Dim sFunctionName As String = "GetVoltageInternal"
        If (bDLC = True) Then sFunctionName = "GetVoltageDLCInternal"
        lCount = lCount + 1
        If (lCount > 9999) Then lCount = 9
        If (lCount <> 1) Then
            Applog("GetVoltageInternal | Error-Cnt:" & lCount.ToString("00") & "(" & CallingProcedure & ", " & LastCallingProcedure & "')")
        End If
        LastCallingProcedure = CallingProcedure
        If (GetDAD(sFunctionName & "-" & CallingProcedure) = True) Then
            Try
                If (IsNothing(MyDad) = False) Then
                    Dim iRetryCount As Integer = 1

                    Dim r As DrewTech.IIMClean.DT_IGenericResponse(Of Decimal) = Nothing '-- #If ((DREW_GENERIC) Or (DREW_GENERIC_CA))

                    Do While (True)
                        OpenDAD_If_Needed(sFunctionName & " (" & CallingProcedure & ")")
                        r = Nothing
                        If (MyDadOpen = True) Then
                            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.InvalidData, False)
                            If (bDLC = True) Then
                                r = MyDad.GetVoltageDLC()
                            Else
                                r = MyDad.GetVoltage()
                            End If
                        End If
                        If (IsNothing(r) = False) Then SetupMyLastCommResult(r.CommResult, False, True)
                        'If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                        If (IsCommResultSuccess(MyLastCommResult) = True) Then
                            rValue = r.Data
                            Exit Do
                        Else

                            If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.J2534_USB_UPDATE_REQUIRED) Then
                                Applog("Err: " & MyLastResult() & "'" & " Cntrl:'" & sControl & "'")
                                'rValue = -9.0                   ' Update required
                                rValue = OBD_VOLTAGE_UPDATE_REQUIRED
                                Exit Do
                            Else
                                'rValue = -3.0                   ' Failure
                                rValue = OBD_VOLTAGE_FAILURE
                                iErrorCount = iErrorCount + 1
                                'Applog("GetVoltageInternal | Err:'" & MyLastResult() & "'" & " Cntrl:'" & sControl & "'" & "  (Rty:" & iRetryCount.ToString("0") & ",Typ:" & OBDLinkType.ToString("0") & ")")
                                If (sControl.ToUpper.Contains("NoRetry".ToUpper) = True) Then Exit Do
                                If (sControl.ToUpper.Contains("NoReset".ToUpper) = True) Then Exit Do

                                If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.VehicleError) Then
                                    iRetryCount = iRetryCount - 1
                                Else
                                    Exit Do
                                End If

                            End If
                        End If
                    Loop
                End If
            Catch ex As Exception
                iErrorCount = iErrorCount + 1
                Applog("Err:GetVoltageInternal: " & ex.Message)
            End Try
            ReleaseDAD(sFunctionName & "-" & CallingProcedure)
        Else
            rValue = -2.0
        End If
        lCount = lCount - 1
        If (lCount < 0) Then lCount = 0
        Dim iLog As Integer = 3
        If (iErrorCount > 0) Then iLog = 1
        If (sControl.ToUpper.Contains("Log".ToUpper) = True) Then iLog = 1

        Applog("... GetVoltageInternal | Ret:" & rValue.ToString("0.00") & " Cntrl:'" & sControl & "'" & "  (" & MyLastResult() & ", " & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
        Return (rValue)
    End Function


    Public Sub DoOBDWorkThread(ByVal InData As Object)
        Applog("... DoOBDWorkThread")

        Dim Params As OBDWorkParams
        If (IsNothing(InData) = False) Then
            Params = DirectCast(InData, OBDWorkParams)
        Else
            Params.sCommand = "?"
            Params.sLocation = "?"
            Params.oInParameters = Nothing
            Params.oOutParameters = Nothing
        End If
        Try
            Select Case Params.sCommand.ToUpper
                Case "Initialize".ToUpper
                    Dim sTemp As String = ""
                    sTemp = InitializeInterface()
                    If ((IsNothing(Params.oOutParameters) = False) AndAlso (Params.oOutParameters.Length > 0)) Then
                        Params.oOutParameters(0) = sTemp
                    End If
                Case "ResetConnection".ToUpper
                    Dim bTemp As Boolean = False
                    bTemp = ResetConnectionInternal(Params.oInParameters)
                    If ((IsNothing(Params.oOutParameters) = False) AndAlso (Params.oOutParameters.Length > 0)) Then
                        Params.oOutParameters(0) = bTemp
                    End If
                Case "GetOBDLinkInformation".ToUpper
                    Dim sTemp As String = ""
                    If ((IsNothing(Params.oInParameters) = False) AndAlso (Params.oInParameters.Length > 0)) Then
                        sTemp = CStr(Params.oInParameters(0))
                    End If
                    'Dim obiTemp As OBDLinkInformation
                    Dim lobiTemp As String = GetOBDLinkInformationInternal(sTemp) 'obiTemp = GetOBDLinkInformationInternal(sTemp)   MJA
                    'If ((IsNothing(Params.oOutParameters) = False) AndAlso (Params.oOutParameters.Length > 0)) Then
                    'Params.oOutParameters(0) = obiTemp
                    'End If
                Case "GetVoltage".ToUpper
                    Dim sTemp As String = ""
                    If ((IsNothing(Params.oInParameters) = False) AndAlso (Params.oInParameters.Length > 0)) Then
                        sTemp = CStr(Params.oInParameters(0))
                    End If
                    Dim sngTemp As Single
                    sngTemp = GetVoltageInternal(False, sTemp)
                    If ((IsNothing(Params.oOutParameters) = False) AndAlso (Params.oOutParameters.Length > 0)) Then
                        Params.oOutParameters(0) = sngTemp
                    End If
                Case "GetVoltageDLC".ToUpper
                    Dim sTemp As String = ""
                    If ((IsNothing(Params.oInParameters) = False) AndAlso (Params.oInParameters.Length > 0)) Then
                        sTemp = CStr(Params.oInParameters(0))
                    End If
                    Dim sngTemp As Single
                    'sngTemp = GetVoltageDLCInternal(sTemp)
                    sngTemp = GetVoltageInternal(True, sTemp)
                    If ((IsNothing(Params.oOutParameters) = False) AndAlso (Params.oOutParameters.Length > 0)) Then
                        Params.oOutParameters(0) = sngTemp
                    End If
                Case "Close".ToUpper
                    If (IsNothing(MyDad) = False) Then
                        MyDad.Close()
                        MyDadOpen = False
                    End If
            End Select
        Catch ex As Exception
            Applog("Err:DoOBDWorkThread | Cmd: " & Params.sCommand & " | " & ex.Message)
        End Try
    End Sub



    Public Function DoOBDWithThread(ByVal sCommand As String,
                                     ByVal sLocation As String,
                                     ByRef InParameters() As Object,
                                     ByRef OutParameters() As Object,
                                     Optional ByVal dTimeout As Double = 90000,
                                     Optional ByRef bAbortFlag As Boolean = Nothing) As String
        Applog("... DoOBDWorkThread (función)")

        Dim sReturn As String = ""
        Dim iReturn As Integer = 0
        Dim lWaitCount As Long = 0
        Dim CallingProcedure As String = LocalGetCallingProcedure()
        Dim sFunctionName As String = "DoOBDWithThread"
        Dim MyThread As Threading.Thread = Nothing
        Dim StartTicks As Int32 = System.Environment.TickCount

        'applog(2, sFunctionName, "'" & sCommand & "'-'" & sLocation & "'-" &
        '                           "Starting ... (" & dTimeout.ToString("0") & ", " & CallingProcedure & ")")

        Try
            Dim MyData As OBDWorkParams
            MyData.sCommand = sCommand
            MyData.sLocation = sLocation
            MyData.oInParameters = InParameters
            MyData.oOutParameters = OutParameters
            If (IsNothing(bAbortFlag) = False) Then bAbortFlag = False
            MyThread = New Threading.Thread(AddressOf DoOBDWorkThread)
            MyThread.Name = "OBDDrewDADThread"
            MyThread.IsBackground = True
            MyThread.Start(MyData)
        Catch ex As Exception
            iReturn = -1
            sReturn = "EXCEPTION01:'" & ex.Message & "'"
            'WriteLLL(1, sFunctionName, "'" & sCommand & "'-'" & sLocation & "'-" & "ex1:'" & ex.Message & "'")
            Applog("Err:DoOBDWithThread | " & ex.Message)
        End Try
        '
        If (IsNothing(MyThread) = False) Then
            Try
                ' Wait for a normal exit
                While ((MyThread.IsAlive = True) AndAlso
                       (DeltaTimeTicks(StartTicks, "MS") < dTimeout))
                    lWaitCount = lWaitCount + 1
                    If (lWaitCount > 99999) Then lWaitCount = 9
                    System.Threading.Thread.Sleep(100)
                    '? System.Windows.Forms.Application.DoEvents()
                End While
                If (MyThread.IsAlive = True) Then
                    iReturn = -3
                    sReturn = "TIMEOUT"
                    'WriteLLL(2, sFunctionName, "'" & sCommand & "'-'" & sLocation & "'-" &
                    '                         "Thread timeout ... (" & lWaitCount.ToString("00000") &
                    '                         ", " & FunctionSeconds(StartTicks) & ")")
                    If (IsNothing(bAbortFlag) = False) Then bAbortFlag = True
                    Dim WaitStart As Int32 = System.Environment.TickCount
                    While ((MyThread.IsAlive = True) AndAlso
                           (DeltaTimeTicks(WaitStart, "MS") < 10000))
                        System.Threading.Thread.Sleep(100)
                        '? System.Windows.Forms.Application.DoEvents()
                    End While
                    'If (MyThread.IsAlive) Then MyThread.Abort()
                    ' Wait a bit for the thread to end
                    WaitStart = System.Environment.TickCount
                    While ((MyThread.IsAlive = True) AndAlso
                           (DeltaTimeTicks(WaitStart, "MS") < 15000))
                        System.Threading.Thread.Sleep(100)
                        '? System.Windows.Forms.Application.DoEvents()
                    End While
                    If (MyThread.IsAlive = True) Then
                        iReturn = -4
                        sReturn = "TIMEOUT-KILL"
                        'WriteLLL(1, sFunctionName, "'" & sCommand & "'-'" & sLocation & "'-" & "Error killing MyThread ...")
                        'MsgBox("DoOBDWork ... Thread error!", MsgBoxStyle.OkOnly, "OBDRead - Thread Error")
                    End If
                End If
            Catch ex As Exception
                iReturn = -5
                sReturn = "EXCEPTION02:'" & ex.Message & "'"
                'WriteLLL(1, sFunctionName, "'" & sCommand & "'-'" & sLocation & "'-" & "ex2:'" & ex.Message & "'")
                Applog("Err:DoOBDWithThread | " & ex.Message)
            End Try
        Else
            iReturn = -2
            sReturn = "NOTHREAD"
        End If
        Try
            MyThread = Nothing
        Catch ex As Exception
        End Try
        'WriteLLL(2, sFunctionName, "'" & sCommand & "'-'" & sLocation & "'-" &
        '                         "Ret:" & iReturn.ToString("0") & "_('" & sReturn & "')" &
        '                         "  (" & FunctionSeconds(StartTicks) & ", " &
        'lWaitCount.ToString("00000") & ")")
        Return (sReturn)
    End Function


    Public Function ResetConnection(Optional ByVal sControl As String = "") As Boolean
        Static iCount As Integer = 0
        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim CallingProcedure As String = LocalGetCallingProcedure()
        Dim bReturn As Boolean = False
        Dim bDoIt As Boolean = False

        If (sControl.ToUpper.Contains("NOGETDAD".ToUpper) = True) Then
        Else
            LocalIncrementActiveCount(CallingProcedure)
        End If

        iCount = iCount + 1
        If (iCount > 99) Then iCount = 99
        Applog("ResetConnection " & iCount.ToString("00") & "-Starting ... Cntrl:'" & sControl & "'" & " (" & MyDADStatus() & ")" & " (" & CallingProcedure & ")")

        If (iCount = 1) Then bDoIt = True
        If (bDoIt = True) Then
            InitializeStringArrayData()
        End If
        Dim oParams(1) As Object
        oParams(0) = bDoIt
        oParams(1) = sControl
        If (sControl.ToUpper.Contains("UseThread".ToUpper) = True) Then
            Dim oOutParams(0) As Object
            oOutParams(0) = False
            DoOBDWithThread("ResetConnection", "ResetConnection", oParams, oOutParams)
            bReturn = CBool(oOutParams(0))
        Else
            bReturn = ResetConnectionInternal(oParams)
        End If

        Dim iTemp As Integer = 3
        If (bReturn = False) Then iTemp = 1
        Applog("ResetConnection " & iCount.ToString("00") & "-Ret:" & bReturn.ToString & "  (" & MyLastResult() & ", " & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
        iCount = iCount - 1
        If (iCount < 0) Then iCount = 0
        If (sControl.ToUpper.Contains("NOGETDAD".ToUpper) = True) Then
        Else
            LocalDecrementActiveCount(CallingProcedure)
        End If
        Return (bReturn)
    End Function


    Public Function CheckForNAK(ByVal CurrentSID As Integer, ByVal CurrentPID As Integer, ByVal DataBuffer As Byte()) As Boolean
        Dim bReturn As Boolean = False
        Dim iLength As Integer = 0
        Dim sTemp As String = ""
        If ((IsNothing(DataBuffer) = False) AndAlso (DataBuffer.Length > 0)) Then
            sTemp = ByteArrayToHexString(DataBuffer, 0, ",")
            iLength = DataBuffer.Length
            ' Check for NAK response
            If (((CurrentSID = &H3) OrElse (CurrentSID = &H7) OrElse (CurrentSID = &H9) OrElse (CurrentSID = &HA)) AndAlso
                (iLength = 3) AndAlso
                (DataBuffer(0) = &H7F) AndAlso
                (DataBuffer(1) = CByte(CurrentSID)) AndAlso
                ((MyObdProtocolCAN = True) OrElse
                 (MyObdProtocol = OBD_PROTOCOL_NAME_KWP) OrElse
                 (MyObdProtocol = OBD_PROTOCOL_NAME_KWP_FAST) OrElse
                 (MyObdProtocol = OBD_PROTOCOL_NAME_KWP_SLOW))) Then
                bReturn = True
            End If
            ' Check for NAK response
            If (((CurrentSID = &H1) OrElse (CurrentSID = &H2)) AndAlso
                (iLength = 4) AndAlso
                (DataBuffer(0) = &H7F) AndAlso
                (DataBuffer(1) = CByte(CurrentSID)) AndAlso
                (DataBuffer(2) = CByte(CurrentPID)) AndAlso
                ((DataBuffer(3) = &H10) OrElse
                 (DataBuffer(3) = &H11) OrElse
                 (DataBuffer(3) = &H12) OrElse
                 (DataBuffer(3) = &H21) OrElse
                 (DataBuffer(3) = &H22) OrElse
                 (DataBuffer(3) = &H78)) AndAlso
                ((MyObdProtocolCAN = True) OrElse
                 (MyObdProtocol = OBD_PROTOCOL_NAME_KWP) OrElse
                 (MyObdProtocol = OBD_PROTOCOL_NAME_KWP_FAST) OrElse
                 (MyObdProtocol = OBD_PROTOCOL_NAME_KWP_SLOW))) Then
                bReturn = True
            End If
        End If
        Dim iTemp As Integer = 4
        If (bReturn = True) Then iTemp = 1

        Applog("... CheckForNAK | Ret:" & bReturn.ToString &
                                 "  Len:" & iLength.ToString("0") &
                                 "  Buf:" & sTemp &
                                 "  SID:" & CurrentSID.ToString("X02") &
                                 "  PID:" & CurrentPID.ToString("X02") &
                                 "  Prot:'" & MyObdProtocol & "'")

        Return (bReturn)

    End Function


    Public Function ByteArrayToString(ByVal InArray As Byte(),
                                       ByVal InStart As Integer,
                                       ByVal strNonPrint As String,
                                       Optional ByVal bIgnoreNull As Boolean = False,
                                       Optional ByRef iPrintableLength As Integer = Nothing) As String

        Dim sReturn As String = ""
        Dim iPrintLength As Integer = 0
        Dim bByte As Byte
        Try
            If ((IsNothing(InArray) = False) AndAlso (InArray.Length > 0) AndAlso (InStart < InArray.Length)) Then
                For idx As Integer = InStart To InArray.Length - 1
                    bByte = InArray(idx)
                    If ((bByte <> 0) OrElse (bIgnoreNull = False)) Then
                        If ((bByte >= &H20) AndAlso (bByte <= &H7A)) Then
                            sReturn = sReturn & Chr(bByte)
                            iPrintLength = iPrintLength + 1
                        Else
                            sReturn = sReturn & strNonPrint
                        End If
                    End If
                Next
            End If
            If (IsNothing(iPrintableLength) = False) Then
                iPrintableLength = iPrintLength
            End If
        Catch ex As Exception
            Applog("Err:ByteArrayToString | " & ex.Message)
        End Try

        Applog("... ByteArrayToString | " & sReturn)
        Return (sReturn)
    End Function


    Public Sub DECODE_Bus(ByVal pDataBus As String)

        Applog("DECODE_Bus: " & pDataBus)

        Dim lCar As String = Nothing
        Dim lBustxt As String = Nothing
        Dim zDB As Integer = Len(pDataBus)
        Dim Ix0 As Integer = 0

        For Ix0 = 1 To zDB

            lCar = Mid(pDataBus, Ix0, 1)

            If Asc(lCar) <> 13 Then '-- chr(13)
                lBustxt &= lCar
                'Applog(".. " & lBustxt)
            Else

                If InStr(lBustxt, "RX MSG") > 0 Then DECODE_BusLinea(lBustxt)
                lBustxt = Nothing

            End If

        Next

    End Sub

    Public Sub DECODE_BusLinea(ByVal pDataTxt As String)

        Dim lCar As String = Nothing
        Dim lBustxt As String = Nothing
        Dim zDB As Integer = Len(pDataTxt)
        Dim Ix0 As Integer = InStrRev(pDataTxt, ",")
        Dim Ix1 As Integer = 0
        'ReDim OBDdataBus(100)      Chr(13) & '& vbCrLf

        Dim banMotGas As Boolean = False

        If InStr(pDataTxt, "07 E8") > 0 Then
            banMotGas = True '-- 07 E8 = ECU motor de gasolina 

        End If

        If InStr(pDataTxt, "07 E8") > 0 Or
                InStr(pDataTxt, "07 DF") > 0 Or
                    InStr(pDataTxt, "07 E0") > 0 Or
                        InStr(pDataTxt, "F1 10") > 0 Or
                            InStr(pDataTxt, "6B 1A") > 0 Or
                                InStr(pDataTxt, "48 6B") > 0 Or
                                    InStr(pDataTxt, "F1 58") > 0 Then '-- posibles ECUS no conocidos (electricos? diesel?) 


            If InStr(pDataTxt, "41 01") > 0 Then '-- OBDdata_MIL 
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid0101 = Trim(lBustxt)
                    Call DECODE_MIL(xPid0101)
                End If
            End If

            If InStr(pDataTxt, "41 0C") > 0 Then '-- OBDdata_RPM 
                If Ix0 > 0 And banMotGas Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid010C = Trim(lBustxt)
                    Call DECODE_RPM(xPid010C)
                End If
            End If

            If InStr(pDataTxt, "E8 43") > 0 Or
                    InStr(pDataTxt, "DF 43") > 0 Or
                        InStr(pDataTxt, "E0 43") > 0 Or
                            InStr(pDataTxt, "10 43") > 0 Or
                                InStr(pDataTxt, "1A 43") > 0 Or
                                       InStr(pDataTxt, "6B 43") > 0 Or
                                            InStr(pDataTxt, "58 43") > 0 Then '-- OBDdata_DTC
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid0300 = Trim(lBustxt)
                    Call DECODE_DTC(xPid0300)
                End If
            End If

            If InStr(pDataTxt, "41 21") > 0 Then '-- Distancia MIL on
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid0121 = Trim(lBustxt)
                    'Call DECODE_RPM(xPid0121)
                End If
            End If

            If InStr(pDataTxt, "41 31") > 0 Then '-- Distancia MIL borrado
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid0131 = Trim(lBustxt)
                    'Call DECODE_RPM(xPid0131)
                End If
            End If

            If InStr(pDataTxt, "41 33") > 0 Then '-- Presion Barometrica Kpa
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid0133 = Trim(lBustxt)
                    'Call DECODE_RPM(xPid0133)
                End If
            End If

            If InStr(pDataTxt, "41 1F") > 0 Then '-- Tiempo de encendido motor
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid011F = Trim(lBustxt)
                    'Call DECODE_RPM(xPid011F)
                End If
            End If

            If InStr(pDataTxt, "41 7F") > 0 Then '-- Tiempo de marcha motor
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid017F = Trim(lBustxt)
                    'Call DECODE_RPM(xPid017F)
                End If
            End If

            If InStr(pDataTxt, "41 4D") > 0 Then '-- Tiempo MIL on
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid014D = Trim(lBustxt)
                    'Call DECODE_RPM(xPid014D)
                End If
            End If

            If InStr(pDataTxt, "49 51") > 0 Then '-- Tipo combustible
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid0951 = Trim(lBustxt)
                    'Call DECODE_RPM(xPid0951)
                End If
            End If

            If InStr(pDataTxt, "49 02") > 0 Then '-- VIN
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid0902 = Trim(lBustxt)
                    'Call DECODE_RPM(xPid0902)
                End If
            End If

            If InStr(pDataTxt, "49 04") > 0 Then '-- Cal ID
                If Ix0 > 0 Then
                    lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)
                    xPid0904 = Trim(lBustxt)
                    'Call DECODE_VIN(xPid0904)
                End If
            End If

        End If

    End Sub


    Public Sub DECODE_MIL(ByVal pBufferDAT As String)

        '-- Aqui evaluamos los 2 primeros digitos (DISPONIBLE y COMPLETADO) El tercer digito se infiere aqui pero se rectifica en DECODE_FALLAS
        '-- Interpretación del codigo de MONITOR:   110 >>  1:Disponible ok,  1:Completado Ok,  0:Sin DTC Ok => PASO
        '--                                         109 >>  1:Disponible ok,  0:No Completado,  9:DTC na => FALLO 
        '--                                         111 >>  1:Disponible ok,  1:Completado ok,  1:con DTC => FALLO        
        '--                                         909 >>  9:No Disponible,  0:No Completado,  9:DTC na => FALLO       

        '-- LrdOBD_cilin / Sistema de Detección de Condiciones Inadecuadas de Ignición en Cilindros
        '-- LrdOBD_catal / Sistema de Eficiencia del Convertidor Catalítico
        '-- LrdOBD_combu / Sistema de Combustible
        '-- LrdOBD_oxige / Sistema de Sensores de Oxígeno
        '-- LrdOBD_integ / Sistema de Componentes Integrales

        Dim Ixd As Integer = 0
        Dim Isz As Integer = 0
        Dim xDato As String = ""
        Dim xA As String
        Dim xB As String
        Dim xC As String
        Dim xD As String

        Dim lOBD_MSI As String = ""
        Dim lOBD_CCM As String = ""
        Dim lOBD_CMB As String = ""
        Dim lOBD_O2S As String = ""
        Dim lOBD_CAT As String = ""

        Dim lOBD_CCC As String = ""
        Dim lOBD_EVS As String = ""
        Dim lOBD_SAS As String = ""
        Dim lOBD_FAA As String = ""
        Dim lOBD_O2C As String = ""

        '-- MIL formato: 010141010000FFFF /  010141018000FFFF / Dos ECUS = 0101410100040000410184076565 => ECU.1 410100040000 y ECU.2 410184076565 

        Try

            Applog("DECODE_MIL: " & pBufferDAT)
            xDato = zipESP(pBufferDAT)

            Isz = Len(xDato)
            Ixd = Strings.InStrRev(xDato, "4101") '-- Se localiza lectura del ECU
            xDato = Mid(xDato, Ixd + 4, Isz) '-- Tomamos bus restante

            Applog("DECODE_MIL dato: " & xDato)
            LrdOBD_EDO_MIL = xDato '--Salvamos cadena obtenida para aseguramiento de lectura por redundancia.

            If Len(xDato) >= 8 Then '-- Tamaño de dato valido, diferente la lectura no fue correcta.

                xA = fncDEC_to_BIN(CInt("&H" & Mid(xDato, 1, 2)))
                xB = fncDEC_to_BIN(CInt("&H" & Mid(xDato, 3, 2)))
                xC = fncDEC_to_BIN(CInt("&H" & Mid(xDato, 5, 2)))
                xD = fncDEC_to_BIN(CInt("&H" & Mid(xDato, 7, 2)))

                Applog("Status: " & xDato)
                Applog("A: " & Mid(xDato, 1, 2) & " | " & String.Format(CInt("&H" & Mid(xDato, 1, 2)), "000") & " | " & xA)
                Applog("B: " & Mid(xDato, 3, 2) & " | " & String.Format(CInt("&H" & Mid(xDato, 3, 2)), "000") & " | " & xB)
                Applog("C: " & Mid(xDato, 5, 2) & " | " & String.Format(CInt("&H" & Mid(xDato, 5, 2)), "000") & " | " & xC)
                Applog("D: " & Mid(xDato, 7, 2) & " | " & String.Format(CInt("&H" & Mid(xDato, 7, 2)), "000") & " | " & xD)

                '-- Bin  Dec (posiciones)
                '-- 0 -> 8
                '-- 1 -> 7 
                '-- 2 -> 6
                '-- 3 -> 5
                '-- 4 -> 4  
                '-- 5 -> 3 
                '-- 6 -> 2
                '-- 7 -> 1

                '//-- MSI | LrdOBD_MSI / Sistema de Detección de Condiciones Inadecuadas de Ignición en Cilindros ------------------------------------------
                '-- 0000000(0) B0 : 0=ND 1=Disponible | 000(0)0000 B4 : 0=Incompleta 1=Completa 
                lOBD_MSI = " | B0 : " & Mid(xB, 8, 1) & " | B4 : " & Mid(xB, 4, 1)
                If Mid(xB, 8, 1) = "1" Then '-- Disponible
                    If Mid(xB, 4, 1) = "0" Then '-- Completada 
                        LrdOBD_MSI = "110" '-- Paso
                    Else
                        LrdOBD_MSI = "109" '-- No completada / LrdOBD_cilin = "1" '-- Fallo 14.03.24
                        rntSalvaFalla("MSINC")
                    End If

                Else
                    LrdOBD_MSI = "009" '-- No disponible se considera aprobatorio PVVO 2025

                End If '||-------------------------------

                '//-- CCM | LrdOBD_CCM / Sistema de Eficiencia del Convertidor Catalítico ------------------------------------------------------------------
                '-- 1111111(1) C0 : 0=ND 1=Disponible | 1111111(1) D0 : 0=Incompleta 1=Completa
                lOBD_CCM = " | C0 : " & Mid(xC, 8, 1) & " | D0 : " & Mid(xD, 8, 1)
                If Mid(xC, 8, 1) = "1" Then '-- Disponible
                    If Mid(xD, 8, 1) = "0" Then '-- Completada 
                        LrdOBD_CCM = "110" '-- Paso
                    Else
                        LrdOBD_CCM = "109" '-- No completada / LrdOBD_catal = "1" '-- Fallo 14.03.24
                        rntSalvaFalla("CCMNC")
                    End If

                Else
                    LrdOBD_CCM = "009" '-- No disponible se considera aprobatorio PVVO 2025

                End If '||-------------------------------

                '//-- CMB | LrdOBD_CMB / Sistema de Combustible ----------------------------------------------------------------------------------------------
                '-- 000000(0)0 B1 : 0=ND 1=Disponible | 00(0)00000 B5 : 0=Incompleta 1=Completa
                lOBD_CMB = " | B1 : " & Mid(xB, 7, 1) & " | B5 : " & Mid(xB, 3, 1)
                If Mid(xB, 7, 1) = "1" Then '-- Disponible
                    If Mid(xB, 3, 1) = "0" Then '-- Completada 
                        LrdOBD_CMB = "110" '-- Paso
                    Else
                        LrdOBD_CMB = "109" '-- No completada / LrdOBD_combu = "1" '-- Fallo 14.03.24
                        rntSalvaFalla("CMBNC")
                    End If

                Else
                    LrdOBD_CMB = "009" '-- No disponible se considera aprobatorio PVVO 2025

                End If '||-------------------------------

                '//-- O2S | LrdOBD_O2S / Sistema de Sensores de Oxígeno ----------------------------------------------------------------------------------------
                '-- 11(1)11111 C5 : 0=ND 1=Disponible | 11(1)11111 D5 : 0=Incompleta 1=Completa
                lOBD_O2S = " | C5 : " & Mid(xC, 3, 1) & " | D5 : " & Mid(xD, 3, 1)
                If Mid(xC, 3, 1) = "1" Then '-- Disponible
                    If Mid(xD, 3, 1) = "0" Then '-- Completada 
                        LrdOBD_O2S = "110" '-- Paso
                    Else
                        LrdOBD_O2S = "109" '-- No completada / LrdOBD_oxige = "1" '-- Fallo 14.03.24
                        rntSalvaFalla("O2SNC")
                    End If

                Else
                    LrdOBD_O2S = "009" '-- No disponible se considera aprobatorio PVVO 2025

                End If '||-------------------------------

                '//-- CAT | LrdOBD_CAT / Sistema de Componentes Integrales ---------------------------------------------------------------------------------------
                '-- 00000(0)00 B2 : 0=ND 1=Disponible | 0(0)000000 B6 : 0=Incompleta 1=Completa
                lOBD_CAT = " | B2 : " & Mid(xB, 6, 1) & " | B6 : " & Mid(xB, 2, 1)
                If Mid(xB, 6, 1) = "1" Then '-- Disponible
                    If Mid(xB, 2, 1) = "0" Then '-- Completada 
                        LrdOBD_CAT = "110" '-- Paso
                    Else
                        LrdOBD_CAT = "109" '-- No completada / LrdOBD_integ = "1" '-- Fallo 14.03.24
                        rntSalvaFalla("CATNC")
                    End If

                Else
                    LrdOBD_CAT = "009" '-- No disponible se considera aprobatorio PVVO 2025

                End If '||-------------------------------

                '-- OTROS MONITORES, NO APLICAN PARA LA PRUEBA SOLO ESTADISTICA-----------------------------------------------------------------------------------------

                '//-- CCC | LrdOBD_CCC / Sistema de calentamiento de convertidor catalitico-------------------------------------------------------------------------------
                '-- 000000(0)0 C1 : 0=ND 1=Disponible | 000000(0)0 D1 : 0=Incompleta 1=Completa
                lOBD_CCC = " | C1 : " & Mid(xC, 7, 1) & " | D1 : " & Mid(xD, 7, 1)
                If Mid(xC, 7, 1) = "1" Then '-- Disponible
                    If Mid(xD, 7, 1) = "0" Then '-- Completada 
                        LrdOBD_CCC = "110" '-- Paso
                    Else
                        LrdOBD_CCC = "109" '-- No completada
                    End If

                Else
                    LrdOBD_CCC = "009" '-- No disponible se considera aprobatorio PVVO 2025
                End If '||-------------------------------

                '//-- EVS | LrdOBD_EVS / Sistema evaporativo-------------------------------------------------------------------------------
                '-- 00000(0)00 C2 : 0=ND 1=Disponible | 00000(0)00 D2 : 0=Incompleta 1=Completa
                lOBD_EVS = " | C2 : " & Mid(xC, 6, 1) & " | D2 : " & Mid(xD, 6, 1)
                If Mid(xC, 6, 1) = "1" Then '-- Disponible
                    If Mid(xD, 6, 1) = "0" Then '-- Completada 
                        LrdOBD_EVS = "110" '-- Paso
                    Else
                        LrdOBD_EVS = "109" '-- No completada
                    End If

                Else
                    LrdOBD_EVS = "009" '-- No disponible se considera aprobatorio PVVO 2025
                End If '||-------------------------------

                '//-- SAS | LrdOBD_SAS / Sistema secundario de aire -------------------------------------------------------------------------------
                '-- 0000(0)000 C3 : 0=ND 1=Disponible | 0000(0)000 D3 : 0=Incompleta 1=Completa
                lOBD_SAS = " | C3 : " & Mid(xC, 5, 1) & " | D3 : " & Mid(xD, 5, 1)
                If Mid(xC, 5, 1) = "1" Then '-- Disponible
                    If Mid(xD, 5, 1) = "0" Then '-- Completada 
                        LrdOBD_SAS = "110" '-- Paso
                    Else
                        LrdOBD_SAS = "109" '-- No completada
                    End If

                Else
                    LrdOBD_SAS = "009" '-- No disponible se considera aprobatorio PVVO 2025
                End If '||-------------------------------

                '//-- FAA | LrdOBD_FAA / Sistema de fugas de aire acondicionado ------------------------------------------------------------------
                '-- 000(0)0000 C4 : 0=ND 1=Disponible | 000(0)0000 D4 : 0=Incompleta 1=Completa
                lOBD_FAA = " | C4 : " & Mid(xC, 4, 1) & " | D4 : " & Mid(xD, 4, 1)
                If Mid(xC, 4, 1) = "1" Then '-- Disponible
                    If Mid(xD, 4, 1) = "0" Then '-- Completada 
                        LrdOBD_FAA = "110" '-- Paso
                    Else
                        LrdOBD_FAA = "109" '-- No completada
                    End If

                Else
                    LrdOBD_FAA = "009" '-- No disponible se considera aprobatorio PVVO 2025
                End If '||-------------------------------

                '//-- O2C | LrdOBD_O2C / Sistema de calentamiento del sensor de oxigeno-------------------------------------------------------------
                '-- 00(0)00000 C6 : 0=ND 1=Disponible | 00(0)00000 D6 : 0=Incompleta 1=Completa
                lOBD_O2C = " | C6 : " & Mid(xC, 3, 1) & " | D6 : " & Mid(xD, 3, 1)
                If Mid(xC, 3, 1) = "1" Then '-- Disponible
                    If Mid(xD, 3, 1) = "0" Then '-- Completada 
                        LrdOBD_O2C = "110" '-- Paso
                    Else
                        LrdOBD_O2C = "109" '-- No completada
                    End If

                Else
                    LrdOBD_O2C = "009" '-- No disponible se considera aprobatorio PVVO 2025
                End If '||-------------------------------

                LrdOBD_EDO_MIL = Mid(xA, 1, 1) '-- LrdOBD_EDO_MIL / A7  0 = MIL off,  1 = MIL on / (0)0000000 A7 ------------------------------------------------------- 

            Else

                LrdOBD_EDO_MIL = "0" '-- 9 = No disponible, en MIL no aplica, pasa como "0" 

            End If

            OBDdata_MILtxt = "Mil:" & LrdOBD_EDO_MIL & "| Msi:" & LrdOBD_MSI & " | Ccm:" & LrdOBD_CCM & " | Cmb:" & LrdOBD_CMB & " | O2s:" & LrdOBD_O2S & " | Cat:" & LrdOBD_CAT & " | Ccc:" &
                            LrdOBD_CCC & " | Evs:" & LrdOBD_EVS & " | Sas:" & LrdOBD_SAS & " | Faa:" & LrdOBD_FAA & " | O2c:" & LrdOBD_O2C

        Catch ex As Exception

            Applog("Err:DECODE_MIL | " & ex.Message)

        End Try

    End Sub


    Private Function fncDEC_to_BIN(ByVal pDato As Integer) As String

        Dim bin As Integer = 0
        Dim output As String = ""

        While pDato <> 0
            If pDato Mod 2 = 0 Then
                bin = 0
            Else
                bin = 1
            End If
            pDato = pDato \ 2
            output = Convert.ToString(bin) & output
        End While

        If output Is Nothing Then
            Return "00000000"
        Else
            output = Strings.Format(Val(output), "00000000")
            Return output
        End If

    End Function

    Private Sub DECODE_RPM(ByVal pBufferDAT As String)

        pBufferDAT = zipESP(pBufferDAT)
        Applog("DECODE_RPM: " & pBufferDAT)

        Dim Ix0 As Integer = 0
        Dim SizeBufferSalina As Integer = Len(pBufferDAT)

        If InStr(pBufferDAT, "NODATA") Then
            OBDdata_RPM = 0

        Else
            For Ix0 = 1 To SizeBufferSalina
                If Mid(pBufferDAT, Ix0, 4) = "410C" Then

                    Dim B1, B2 As String
                    Dim B3, B4, RdRPM As Double
                    Dim xRPM As String = Mid(pBufferDAT, Ix0 + 4, 4)

                    B1 = Mid(xRPM, 1, 2)
                    B2 = Mid(xRPM, 3, 2)
                    B3 = ("&H" & B1) 'Convert to Hex
                    B4 = ("&H" & B2)
                    RdRPM = (((B3 * 256) + B4) / 4)

                    OBDdata_RPM = Math.Round(RdRPM, 0)
                    Exit For

                End If

            Next

        End If

    End Sub


    Private Sub DECODE_DTC(ByVal pBufferDAT As String)

        Applog("... DECODE_DTC: " & pBufferDAT)

        'pBufferDAT = "000007E843092677267A26AE043204260420030203050310"

        LrdOBD_STATUS_Cadd = zipESP(pBufferDAT) 'lfncLimpCadd(pBufferDAT)
        Dim lPid As String = Nothing
        Dim Ixd As Integer = 0
        Dim zBufferDAT As Integer = Len(LrdOBD_STATUS_Cadd)
        Dim NoDTC As Integer = 0
        Dim Ix0 As Integer = 0

        Try

            Ixd = InStr(LrdOBD_STATUS_Cadd, "07E843") '-- 07E843##  | ## = numero de DTC detectados
            If Ixd > 0 Then

                NoDTC = Convert.ToInt16(Mid(LrdOBD_STATUS_Cadd, Ixd + 6, 2), 16)

                LrdOBD_STATUS_Cadd = Mid(LrdOBD_STATUS_Cadd, Ixd + 8, zBufferDAT)

                NoDTC = NoDTC * 4
                For Ix0 = 1 To NoDTC Step 4

                    'MsgBox(Ix0 & " == " & NoDTC)
                    If Len(LrdOBD_STATUS_Cadd) >= 4 Then

                        lPid = Mid(LrdOBD_STATUS_Cadd, Ix0, 4)
                        'MsgBox(lPid)
                        If DECODE_DTC_CODIGOS(lPid) Then

                            If Len(OBDdata_DTCtxt) > 1 Then
                                OBDdata_DTCtxt &= ", P" & lPid  '-- Solo si el DTC es parte del catalogo se reporta.
                            Else
                                OBDdata_DTCtxt &= "P" & lPid   '-- Solo si el DTC es parte del catalogo se reporta.
                            End If

                        End If
                    Else
                        Exit For
                    End If

                Next

            End If

        Catch ex As Exception
            'lrdError = pBufferDAT & "|" & ex.Message
            Applog("Err:DECODE_DTC | " & pBufferDAT & "| Icc: " & Ixd & " | " & ex.Message)

        End Try

        'Applog("DECODE_DTC >> " & xOBDcadtxt)

    End Sub



    Public Sub rntSalvaFalla(ByVal pOBDFalla As String)

        Dim Ix0 As Integer = 0
        Dim lOBDFalla As String = "P" & pOBDFalla

        For Ix0 = 1 To 20

            If xTablaFallas(Ix0) = lOBDFalla Then
                '-- El codigo ya fue salvado, terminamos proceso    
                Exit For

            Else

                If Len(xTablaFallas(Ix0)) = 0 Then '-- Se localiza registro vacio (ultimo), se guarda

                    xTablaFallas(Ix0) = lOBDFalla

                    Exit For

                End If

            End If

        Next

    End Sub


    Public Function lVehiculoLink() As String
        Dim lStatus As String = Nothing

        Try

            MyDADSemaphore = New System.Threading.Semaphore(1, 1)
            MyDoDADSemaphore = New System.Threading.Semaphore(1, 1)

            Call Connect_OBD()

            lStatus = "Pass: Inspección vehicular OBD concluido."

        Catch ex As Exception

            lStatus = "Err: Inspección vehicular OBD fallo | " & ex.Message

        End Try

        Return lStatus
    End Function


    Public Sub Connect_OBD(Optional ByVal sVIN As String = "",
                                      Optional ByVal sSpecialControl As String = "") ' As DrewTech.IIMClean.DT_Com_Result 'Sti.Peripherals.ObdLinks.ObdLink.ConnectResult
        Dim lStatus As String

        Dim bSuccess As Boolean = False
        Dim RetryCount As Integer = 2
        Dim MaxPidCount As Integer = -1
        Dim sTemp As String = ""
        Dim ProtocolAttemptOrderList As List(Of String) = Nothing

        Dim tVINLength, VinStart As Integer
        Dim sr As DrewTech.IIMClean.DT_IMpr = Nothing
        Dim ht As Hashtable = Nothing

        Applog(" ")
        Applog("... Connect_OBD ===================================================================================================================")
        Applog(" ")

        Try

            MyDad.ClearLogs()
            xOBD_ECU_onLine = False

            Dim StartTicks As Int32 = System.Environment.TickCount
            Dim CallingProcedure As String = LocalGetCallingProcedure()
            LocalIncrementActiveCount(CallingProcedure)
            MyDAD_ConnectCount = MyDAD_ConnectCount + 1
            If (MyDAD_ConnectCount > 9999) Then MyDAD_ConnectCount = 9
            Applog("... Connect | ---------------- Connect ---------------" &
                  " SC:'" & sSpecialControl & "'" &
                 " (" & MyDADStatus() & ")" & " (" & CallingProcedure & ")")

            InitializePIDData()
            InitializeStringArrayData()
            ProtocolAttemptOrderList = Nothing
            MyObdProtocol = OBD_PROTOCOL_NAME_NONE
            MyObdProtocolCAN = False
            MyEngineId = 0
            ConnectModuleHashtable = Nothing
            ConnectModuleHashtable = New Hashtable()
            '
            If (GetDAD("Connect") = True) Then
                Applog("... Connect | 1 | GetDAD =Connect")
                Try
                    Do While ((bSuccess = False) And (RetryCount >= 0))
                        Mode9PidSupportOverall = Nothing

                        ProtocolAttemptOrderList = Nothing

                        If ((IsNothing(sSpecialControl) = False) AndAlso (sSpecialControl.Length > 0)) Then
                            If (sSpecialControl.ToUpper.Contains("CAN-FIRST".ToUpper) = True) Then
                                ProtocolAttemptOrderList = New List(Of String)
                                ProtocolAttemptOrderList.Add("CAN11")
                                ProtocolAttemptOrderList.Add("CAN29")
                                ProtocolAttemptOrderList.Add("VPW")
                                ProtocolAttemptOrderList.Add("PWM")
                                ProtocolAttemptOrderList.Add("ISO")
                                ProtocolAttemptOrderList.Add("KWD 2000-F")
                                ProtocolAttemptOrderList.Add("KWD 2000-S")
                            End If
                        End If

                        MyObdProtocol = OBD_PROTOCOL_NAME_NONE
                        MyObdProtocolCAN = False
                        MyEngineId = 0
                        Mode1ECUCount = 0
                        ConnectModuleHashtable = Nothing
                        ConnectModuleHashtable = New Hashtable()

                        If (IsNothing(MyDad) = False) Then
                            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.InvalidData, False)
                            MyLastConnect = DateTime.Now
                            MyLastConnectVIN = sVIN
                            MyLastConnectSpecialControl = sSpecialControl
                            MyLastConnectCallingProcedure = CallingProcedure
                            Applog("... MyDad.Initialize ")
                            DoDad(Sub() SetupMyLastCommResult(MyDad.Initialize(ProtocolAttemptOrderList, sVIN), True), "Initialize")
                            Applog("Connect | Init-Rty:" & RetryCount.ToString("0") & " (" & MyLastResult() & " | " & FunctionSeconds(StartTicks) & ") | " & CallingProcedure)
                            If (MyLastCommResult = DrewTech.IIMClean.DT_Com_Result.Success) Then
                                Applog("... MyDad.Initialize > Success")
                                'Form1.TextBox4.Text = "Dad.Initialize > Success"
                                DrewProtocol2MyProtocol(MyDad.OBDProtocol)

                                GetSupportedPIDs(1)

                                For Each de As KeyValuePair(Of Byte, BitArray) In PidSupportDictionary
                                    Dim PidCount As Integer = 0
                                    Dim ba As BitArray = de.Value
                                    For idx As Integer = 0 To ba.Length - 1
                                        If ((ba(idx) = True) AndAlso (IncludeInPIDCount(idx) = True)) Then PidCount += 1
                                        If (PidCount > MaxPidCount) Then
                                            MaxPidCount = PidCount
                                            MyEngineId = de.Key
                                        End If
                                    Next
                                Next
                                bSuccess = True
                            Else
                                Applog("... MyDad.Initialize > NOT Success")
                                'Form1.TextBox4.Text = "Dad.Initialize > NOT Success"
                                If ((DeltaTimeTicks(StartTicks, "MS") < 8000) And (RetryCount > 0)) Then
                                    If (RetryCount > 1) Then
                                        If (ResetConnection("NOGETDAD,NOCLOSEOPEN") = False) Then Exit Do
                                    Else
                                        If (ResetConnection("NOGETDAD") = False) Then Exit Do
                                    End If
                                    RetryCount = RetryCount - 1
                                Else
                                    Exit Do
                                End If
                            End If
                        End If
                    Loop
                Catch ex As Exception
                    Applog("Err:Connect | " & ex.Message & " | (" & MyLastResult() & " | " & FunctionSeconds(StartTicks) & ") | " & CallingProcedure)
                End Try
                ReleaseDAD("Connect")
            Else
                Applog("... Connect | 1 | GetDAD =No Connect")

            End If

            Applog("... Connect | 2")
            Connect_Succeeded = bSuccess
            Connect_MyEngineId = MyEngineId
            Connect_MyObdProtocol = MyObdProtocol
            Connect_LinkProtocol = MyObdProtocol
            Connect_ConnectModuleHashtable = ConnectModuleHashtable
            Connect_TotalPIDCountDCount = TotalPIDCount
            Connect_ConnectVIN = sVIN

            SetupVehicleIsLinked("?")
            If (bSuccess = True) Then SetupVehicleIsLinked("True")
            sTemp = ""
            Try
                If (Connect_Succeeded = True) Then
                    Applog("... Connect | 3 | Connect_Succeeded = True")
                    For Each eEntry As DictionaryEntry In Connect_ConnectModuleHashtable 'Connect.GetModuleHashtable
                        If (sTemp.Length > 0) Then sTemp = sTemp & ","
                        sTemp = sTemp & CByte(eEntry.Key).ToString("X02")
                        If ((IsNothing(SortedPIDCountDictionary) = False) AndAlso (SortedPIDCountDictionary.Count > 0)) Then
                            For Each kvp As KeyValuePair(Of Byte, Integer) In SortedPIDCountDictionary
                                If (kvp.Key = CByte(eEntry.Key)) Then
                                    sTemp = sTemp & ":" & kvp.Value.ToString("000")
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                    If ((IsNothing(SortedPIDCountDictionary) = False) AndAlso (SortedPIDCountDictionary.Count > 0)) Then
                        If (sTemp.Length > 0) Then sTemp = sTemp & "->"
                        For Each kvp As KeyValuePair(Of Byte, Integer) In SortedPIDCountDictionary
                            sTemp = sTemp & kvp.Key.ToString("X02") & ":" & kvp.Value.ToString("000") & ","
                        Next
                        If (sTemp(sTemp.Length - 1) = ",") Then sTemp.Remove(sTemp.Length - 1, 1)
                    End If
                End If
            Catch ex As Exception
                Applog("Err:Connect_OBD | " & ex.Message)
            End Try

            Applog("Connect | Rslt:" & Connect_Succeeded.ToString &
                               " EECUID:" & Connect_MyEngineId.ToString("X02") &
                               " ECUs:" & Mode1ECUCount.ToString("0") & "-(" & sTemp & ")" &
                               " Prot:'" & Connect_MyObdProtocol & "', '" & Connect_LinkProtocol & "'" &
                               " PC:" & Connect_TotalPIDCountDCount.ToString("0") &
                               "  (" & MyLastResult() & ", " & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
            LocalDecrementActiveCount(CallingProcedure)

            '===================================

            Applog(" ")
            Applog("... Get MON >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(1, 1), "GetModePID(01, 01) - Readiness, DTC count, MIL", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Get MON: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)
                    'Call DECODE_MIL(xPid0101)

                Else
                    lStatus = "sr | Get MON: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... Get DTC >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(3, 0), "GetModePID(03, 00) - DTC count, MIL", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Get DTC: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)
                    'Call DECODE_DTC(xPid0300)

                Else
                    lStatus = "sr | Get DTC: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... (01, 1F) Tmp. Encendido >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(1, &H1F), "GetModePID(01, 1F) - Tmp. Encendido", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Tmp. Encendido: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    lStatus = "sr | Tmp. Encendido: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... (01, 21) Distancia MIL >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(1, 21), "GetModePID(01, 21) - Distancia MIL", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Distancia MIL: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    lStatus = "sr | Distancia MIL: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... (01, 31) Distancia MIL Borrada >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(1, 31), "GetModePID(01, 31) - Distancia MIL Borrada", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Distancia MIL Borrada: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    lStatus = "sr | Distancia MIL Borrada: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... (01, 33) Presion Barometrica Kpa >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(1, 33), "GetModePID(01, 33) - Presion Barometrica Kpa", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Presion Barometrica Kpa: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    lStatus = "sr | Presion Barometrica Kpa: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... (01, 4D) Tmp MIL on >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(1, &H4D), "GetModePID(01, 4D) - Tmp MIL on", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Tmp MIL on: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    lStatus = "sr | Tmp MIL on: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... (01, 51) Tipo Combustible >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(1, 51), "GetModePID(01, 51) - Tipo Combustible", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Tipo Combustible: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    lStatus = "sr | Tipo Combustible: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... (01, 7F) Tmp Marcha Motor >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(1, &H7F), "GetModePID(01, 7F) - Tmp Marcha Motor", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Tmp Marcha Motor: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    lStatus = "sr | Tmp Marcha Motor: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... (09, 04) Cal ID >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                MyDad.ClearLogs()
                SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(9, 4), "GetModePID(09, 04) - Cal ID", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Cal ID: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    lStatus = "sr | Cal ID: NULL"
                End If
                Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... Get VIN >>>-----------------------------------------------------------------------")
            Try
                sr = MyDad.GetModePID(9, 2) '-- vin

                Applog(MyDad.CommandLog)

                If (IsNothing(sr) = False) Then

                    lStatus = "SR: " & sr.CommResult & " | " & sr.Data.Count
                    Applog(lStatus)

                    For Each edata As DrewTech.IIMClean.DT_IECUData In sr.Data
                        Dim EcuId As Byte = GetEcuIdFromAddress(edata.Address)
                        Try
                            Applog("GetVIN | ECU:" & EcuId.ToString("X02") &
                                              "  DLen:" & edata.Data.Length.ToString("000") &
                                              " (" & ByteArrayToHexString(edata.Data) & ")")
                        Catch ex As Exception
                        End Try

                        OBDdata_VINtxt = ""
                        tVINLength = 0
                        If ((edata.Data.Length > 1) AndAlso (CheckForNAK(9, 2, edata.Data) = False)) Then
                            VinStart = 0
                            If (MyObdProtocolCAN = True) Then VinStart = 1
                            OBDdata_VINtxt = ByteArrayToString(edata.Data, VinStart, "?", True, tVINLength)
                            Applog("GetVIN | ECU:" & EcuId.ToString("X02") & "  VIN: " & xPid0902)
                        End If

                    Next

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    'Form1.TextBox3.Text = "VIN: NULL"
                    xPid0902 = "NULL"
                    OBDdata_VINtxt = "NULL"
                End If

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... Get VOLTAJES >>>-----------------------------------------------------------------------")
            Try

                sr = Nothing
                    MyDad.ClearLogs()
                    SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                DoDad(Sub() sr = MyDad.GetModePID(1, 42), "GetModePID(01, 42) - Voltaje", "NoEOL")
                If (IsNothing(sr) = False) Then
                    lStatus = "sr | Get Voltaje: " & sr.CommResult & " | " & sr.Data.Count

                    Call DECODE_Bus(MyDad.CommandLog)

                Else
                    lStatus = "sr | Get Voltaje: NULL"
                End If
                    Applog(lStatus)

            Catch ex As Exception

            End Try

            Applog(" ")
            Applog("... Get RPM >>>-----------------------------------------------------------------------")
            Try

                Dim Ix0 As Integer = 0
                ReDim OBDdata_RPMmat(3)

                For Ix0 = 1 To 4

                    sr = Nothing
                    MyDad.ClearLogs()
                    SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
                    DoDad(Sub() sr = MyDad.GetModePID(1, &HC), "GetModePID(01, 0C) - DTC count, MIL", "NoEOL")
                    If (IsNothing(sr) = False) Then
                        lStatus = "sr | Get RPM(" & Ix0 & "): " & sr.CommResult & " | " & sr.Data.Count

                        Call DECODE_Bus(MyDad.CommandLog)
                        'Call DECODE_RPM(xPid010C)

                        If Ix0 < 4 Then OBDdata_RPMmat(Ix0) = OBDdata_RPM

                    Else
                        lStatus = "sr | Get RPM: NULL"
                    End If
                    Applog(lStatus)

                Next

            Catch ex As Exception

            End Try

            Dim rResTT As DrewTech.IIMClean.DT_IGenericResponse(Of DrewTech.IIMClean.DT_SelfTestRes) = Nothing
            Dim rRes As DrewTech.IIMClean.DT_Com_Result = DrewTech.IIMClean.DT_Com_Result.InvalidData ' Nothing
            Dim rStr As DrewTech.IIMClean.DT_IGenericResponse(Of String) = Nothing
            Dim rInt As DrewTech.IIMClean.DT_IGenericResponse(Of Integer) = Nothing
            Dim rDec As DrewTech.IIMClean.DT_IGenericResponse(Of Decimal) = Nothing

            rRes = MyDad.Open()
            Applog(rRes.ToString())

            'rStr = MyDad.GetFirmwareVersion()
            'sFirmwareVersion = rStr.Data
            'Applog("InitIMCleanDevice | FirmwareVersion:" & sFirmwareVersion)

            'rInt = MyDad.GetDeviceType
            'sDeviceType = rInt.Data
            'Applog("InitIMCleanDevice | DeviceType:" & sDeviceType)

            rDec = MyDad.GetVoltage
            sDeviceVoltage = rDec.Data
            Applog("InitIMCleanDevice | DeviceVoltage:" & sDeviceVoltage)

            rDec = MyDad.GetVoltageDLC
            sDeviceVoltageDLC = rDec.Data
            Applog("InitIMCleanDevice | DeviceVoltageDLC:" & sDeviceVoltageDLC)

            If sDeviceVoltage > xMinVoltage And sDeviceVoltageDLC > xMinVoltage Then xOBD_ECU_onLine = True '-- validamos los voltajes para identificar que es un vehiculo.

            Applog("... >>>-----------------------------------------------------------------------")
            Applog("... >>>-----------------------------------------------------------------------")

            Applog("VIN: " & OBDdata_VINtxt)
            Applog("MON: " & OBDdata_MILtxt)
            Applog("DTC: " & OBDdata_DTCtxt)

            Applog("Voltage: " & sDeviceVoltage)
            Applog("VoltageDLC: " & sDeviceVoltageDLC)

            Applog("Pid0101 : " & xPid0101) '-- Monitores MIL
            Applog("Pid0300 : " & xPid0300) '-- DTC
            Applog("Pid0121 : " & xPid0121) '-- Distancia MIL on
            Applog("Pid0131 : " & xPid0131) '-- Distancia MIL borrado
            Applog("Pid0133 : " & xPid0133) '-- Presion Barometrica Kpa 
            Applog("Pid011F : " & xPid011F) '-- Tiempo de encendido motor
            Applog("Pid017F : " & xPid017F) '-- Tiempo de marcha motor
            Applog("Pid014D : " & xPid014D) '-- Tiempo MIL on
            Applog("Pid0951 : " & xPid0951) '-- Tipo combustible
            Applog("Pid0902 : " & xPid0902) '-- VIN
            Applog("Pid0904 : " & xPid0904) '-- Cal ID

            Applog("Pid010C: " & xPid010C) '-- RPM
            Applog("RPM: " & OBDdata_RPM & " | 1: " & OBDdata_RPMmat(1) & " | 2: " & OBDdata_RPMmat(2) & " | 3: " & OBDdata_RPMmat(3))

            If xOBD_ECU_onLine Then

                If OBDdata_RPM = OBDdata_RPMmat(1) And
                    OBDdata_RPM = OBDdata_RPMmat(2) And
                        OBDdata_RPM = OBDdata_RPMmat(3) Then
                    Applog("****** Posible Simulación de lecturas OBD detectada.")
                    xOBD_SimulationWarning = True
                Else
                    xOBD_SimulationWarning = False
                End If

            Else

                If Len(xPid0902) > 8 Then
                    xOBD_ECU_onLine = True
                    xOBD_SimulationWarning = True
                    Applog("****** Posible Simulación de lecturas OBD detectada.")
                End If
                Applog("****** OBD no conectado al vehículo.")

            End If

            Applog("*** Connect ... EOF()")

        Catch ex As Exception

            lStatus = "Err:Connect_OBD | " & ex.Message
            Applog(lStatus)
            'MsgBox(lStatus)

        End Try

    End Sub



    Public Sub lInitIMCleanDevice()

        Dim rResTT As DrewTech.IIMClean.DT_IGenericResponse(Of DrewTech.IIMClean.DT_SelfTestRes) = Nothing
        Dim rRes As DrewTech.IIMClean.DT_Com_Result = DrewTech.IIMClean.DT_Com_Result.InvalidData ' Nothing
        Dim rStr As DrewTech.IIMClean.DT_IGenericResponse(Of String) = Nothing
        Dim rInt As DrewTech.IIMClean.DT_IGenericResponse(Of Integer) = Nothing
        Dim rDec As DrewTech.IIMClean.DT_IGenericResponse(Of Decimal) = Nothing

        Try

            MyDad = New DT.DAD.IMClean

            If (IsNothing(MyDad) = False) Then

                rRes = MyDad.Open()
                Applog(rRes.ToString())

                rStr = MyDad.GetFirmwareVersion()
                sFirmwareVersion = rStr.Data
                Applog("InitIMCleanDevice | FirmwareVersion:" & sFirmwareVersion)

                rInt = MyDad.GetDeviceType
                sDeviceType = rInt.Data
                Applog("InitIMCleanDevice | DeviceType:" & sDeviceType)

                rDec = MyDad.GetVoltage
                sDeviceVoltage = rDec.Data
                Applog("InitIMCleanDevice | DeviceVoltage:" & sDeviceVoltage)

                rDec = MyDad.GetVoltageDLC
                sDeviceVoltageDLC = rDec.Data
                Applog("InitIMCleanDevice | DeviceVoltageDLC:" & sDeviceVoltageDLC)

            Else

                Applog("Err:InitIMCleanDevice | Dispositivo IMClean OBD no detectado.")

            End If

        Catch ex As Exception
            Applog("Err:InitIMCleanDevice | " & ex.Message)
        End Try

    End Sub

    Public Function GetObjectArrayAsString(ByRef objInput As Object()) As String

        Dim sReturn As String = ""
        Dim oTemp2 As Object = Nothing
        Try
            If ((IsNothing(objInput) = False) AndAlso (objInput.Length > 0)) Then
                For idx As Integer = 0 To objInput.Length - 1
                    If (IsNothing(objInput(idx)) = False) Then
                        If (sReturn.Length > 0) Then sReturn = sReturn & "|"
                        sReturn = sReturn & CStr(objInput(idx))
                    End If
                Next
                sReturn = Replace(sReturn, vbCr, "")
                sReturn = Replace(sReturn, vbLf, "")
            End If
        Catch ex As Exception
            Applog("Err:GOAAS | " & ex.Message)  'WriteLog(1, "GOAAS", "ex:'" & ex.Message & "'")
        End Try

        Applog("... GetObjectArrayAsString | " & sReturn)
        Return (sReturn)

    End Function



    Public Function GetPropertyValueAsString(ByRef info As System.Management.ManagementObject,
                                              ByVal ValueName As String) As String

        Dim sReturn As String = ""
        Dim sName As String = ""
        Try
            If ((IsNothing(info) = False) AndAlso (IsNothing(ValueName) = False)) Then
                sName = ValueName
                If (sName.Length > 0) Then
                    Dim oTemp As Object = Nothing
                    Dim oTemp2 As Object() = Nothing
                    oTemp = info.GetPropertyValue(sName)
                    If (IsNothing(oTemp) = False) Then
                        If (IsArray(oTemp) = True) Then
                            oTemp2 = CType(oTemp, Object())
                            sReturn = GetObjectArrayAsString(oTemp2)
                        Else
                            sReturn = CStr(oTemp)
                            If (IsNothing(sReturn) = True) Then sReturn = ""
                        End If
                        sReturn = Replace(sReturn, vbCr, "")
                        sReturn = Replace(sReturn, vbLf, "")
                    End If
                End If
            End If
        Catch ex As Exception
            Applog("Err:GPVAS | " & ValueName & " | " & ex.Message)
        End Try
        If (IsNothing(sReturn) = True) Then sReturn = ""

        Applog("... GetPropertyValueAsString: " & sReturn)
        Return (sReturn)

    End Function


    Public Function CheckForDevice(ByVal sDevice As String, ByVal sDeviceID As String, ByVal sHardwareID As String,
                                ByRef sDeviceInfo() As DeviceInfoStruct, Optional ByVal sControl As String = "") As Integer

        Dim iReturn As Integer = 0
        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim sDeviceList() As String = Nothing
        Dim sDeviceIDList() As String = Nothing
        Dim sHardwareIDList() As String = Nothing

        Dim info As System.Management.ManagementObject = Nothing
        Dim search As System.Management.ManagementObjectSearcher = Nothing
        Dim searchinfo As System.Management.ManagementObjectCollection = Nothing
        Dim DeviceInfoNow As DeviceInfoStruct
        Dim DeviceInfoList As System.Collections.Generic.List(Of DeviceInfoStruct) = New System.Collections.Generic.List(Of DeviceInfoStruct)
        Dim DeviceNames As System.Collections.Generic.List(Of String) = New System.Collections.Generic.List(Of String)

        Applog("CFD | Dev:'" & sDevice & "'" &
                       " ID:'" & sDeviceID & "'" &
                       " HID:'" & sHardwareID & "'" &
                       " Cntrl:'" & sControl & "' Starting ...")
        Try
            Dim sTemp As String
            Dim iDeviceCount As Integer = 0
            Dim deviceCaption As String = ""
            Dim deviceName As String = ""
            Dim deviceClass As String = ""
            Dim deviceDescription As String = ""
            Dim deviceDriverName As String = ""
            Dim deviceID As String = ""
            Dim devicePortName As String = ""
            Dim deviceManufacturer As String = ""
            Dim deviceHardwareIDs As String = ""
            Dim devicePNPID As String = ""
            Dim devicePNPClass As String = ""
            Dim sLogString As String = ""
            Dim userchoice() As String = {""}
            Dim UC_Property() As String = Nothing

            If ((IsNothing(sDevice) = False) AndAlso (sDevice.Length > 0)) Then
                sDeviceList = sDevice.Split("|"c)
            End If
            If ((IsNothing(sDeviceID) = False) AndAlso (sDeviceID.Length > 0)) Then
                sDeviceIDList = sDeviceID.Split("|"c)
            End If
            If ((IsNothing(sHardwareID) = False) AndAlso (sHardwareID.Length > 0)) Then
                sHardwareIDList = sHardwareID.Split("|"c)
            End If

            Dim sBuild As System.Text.StringBuilder = New System.Text.StringBuilder("", 300)
            Dim iCount As Integer = 0

            If ((IsNothing(sDeviceList) = False) AndAlso (sDeviceList.Length > 0)) Then
                For idx As Integer = 0 To sDeviceList.Length - 1
                    If (iCount = 0) Then
                        sBuild.Append(" WHERE (")
                    Else
                        sBuild.Append(" OR")
                    End If
                    iCount = iCount + 1
                    If (sControl.ToUpper.Contains("PrintJob".ToUpper) = True) Then
                        sBuild.Append(" DriverName Like '%")
                    Else
                        sBuild.Append(" Name Like '%")
                    End If
                    sBuild.Append(sDeviceList(idx))
                    sBuild.Append("%' OR Description Like  '%")
                    sBuild.Append(sDeviceList(idx))
                    sBuild.Append("%'")
                Next
                If (iCount > 0) Then sBuild.Append(") ")
            End If

            If ((IsNothing(sDeviceIDList) = False) AndAlso (sDeviceIDList.Length > 0)) Then
                If (iCount > 0) Then
                    sBuild.Append(" AND ( ")
                    iCount = 0
                Else
                    sBuild.Append(" WHERE ( ")
                End If
                For idx As Integer = 0 To sDeviceIDList.Length - 1
                    If (iCount = 0) Then
                        'sBuild.Append(" WHERE (")
                    Else
                        sBuild.Append(" OR")
                    End If
                    iCount = iCount + 1
                    sBuild.Append(" DeviceID Like '%")
                    sBuild.Append(sDeviceIDList(idx))
                    sBuild.Append("%'")
                Next
                If (iCount > 0) Then sBuild.Append(") ")
            End If

            If (sControl.ToUpper.Contains("Printer".ToUpper) = True) Then
                sTemp = "SELECT * FROM Win32_Printer" & sBuild.ToString
            ElseIf (sControl.ToUpper.Contains("PrintJob".ToUpper) = True) Then
                sTemp = "SELECT * FROM Win32_PrintJob" & sBuild.ToString
            Else
                sTemp = "SELECT * FROM Win32_PNPEntity" & sBuild.ToString
            End If
            sTemp = sTemp.Trim

            If (sControl.ToUpper.Contains("UCP_".ToUpper) = True) Then
                sBuild.Clear()
                Dim sArray() As String

                sArray = sControl.Split(New String() {",", "|"}, 6, StringSplitOptions.RemoveEmptyEntries)
                If ((IsNothing(sArray) = False) AndAlso (sArray.Length > 0)) Then
                    For Each sProp As String In sArray
                        sProp = sProp.Trim
                        If ((sProp.Length > 4) AndAlso (sProp.Substring(0, 4).ToUpper = "UCP_".ToUpper)) Then
                            If (sBuild.Length > 0) Then sBuild.Append("|")
                            sBuild.Append(sProp.Substring(4))
                        End If
                    Next
                    If (sBuild.Length > 0) Then
                        UC_Property = sBuild.ToString.Split("|"c)
                        ReDim userchoice(UC_Property.Length - 1)
                    End If
                End If
            End If

            search = New System.Management.ManagementObjectSearcher("\\.\ROOT\cimv2", sTemp)
            Applog("CFD | Search:'" & sTemp & " | ")

            searchinfo = search.Get()

            If (IsNothing(searchinfo) = False) Then
                Applog("CFD | Search_Result_Count:" & searchinfo.Count.ToString("0"))
                Try
                    If (searchinfo.Count > 0) Then
                        For Each info In searchinfo
                            ' Go through each device detected.
                            deviceID = GetPropertyValueAsString(info, "DeviceID")
                            Dim Ix0 As Integer = InStrRev(deviceID, "\") + 1
                            strDeviceInfo.DeviceID = Mid(deviceID, Ix0, 50)

                            deviceHardwareIDs = GetPropertyValueAsString(info, "HardwareID")
                            strDeviceInfo.DeviceHardwareIDs = deviceHardwareIDs

                            'devicePortName = GetPropertyValueAsString(info, "PortName")
                            'strDeviceInfo.DevicePortName = devicePortName

                            deviceManufacturer = GetPropertyValueAsString(info, "Manufacturer")
                            strDeviceInfo.DeviceManufacturer = deviceManufacturer

                            devicePNPID = GetPropertyValueAsString(info, "PNPDeviceID")
                            devicePNPClass = GetPropertyValueAsString(info, "PNPClass")
                            deviceCaption = GetPropertyValueAsString(info, "Caption")
                            deviceName = GetPropertyValueAsString(info, "Name")
                            deviceClass = GetPropertyValueAsString(info, "CreationClassName")

                            deviceDescription = GetPropertyValueAsString(info, "Description")
                            strDeviceInfo.DeviceDescription = deviceDescription

                            'deviceDriverName = GetPropertyValueAsString(info, "DriverName")
                            'strDeviceInfo.DeviceDriverName = deviceDriverName
                            'If ((IsNothing(UC_Property) = False) AndAlso (UC_Property.Length > 0)) Then
                            'For idx As Integer = 0 To UC_Property.Length - 1
                            'userchoice(idx) = GetPropertyValueAsString(info, UC_Property(idx))
                            'Next
                            'End If

                            iDeviceCount = iDeviceCount + 1

                            sLogString = "Dev_" & iDeviceCount.ToString("000") & Chr(13) & '& vbCrLf
                                     " Nam:'" & deviceName & Chr(13) &
                                     "  PNPID:'" & devicePNPID & Chr(13) &
                                     "  HWID:'" & deviceHardwareIDs & Chr(13) &
                                     "  ID:'" & deviceID & Chr(13) &
                                     "  Cap:'" & deviceCaption & Chr(13) &
                                     "  Cls:'" & deviceClass & Chr(13) &
                                     "  Des:'" & deviceDescription & Chr(13) &
                                     "  Drv:'" & deviceDriverName & Chr(13) &
                                     "  Man:'" & deviceManufacturer & Chr(13) &
                                     "  PN:'" & devicePortName & Chr(13)
                            Applog(sLogString)
                            If ((IsNothing(sDeviceList) = False) AndAlso (sDeviceList.Length > 0)) Then
                                Dim idx As Integer
                                For idx = 0 To sDeviceList.Length - 1
                                    If ((sDeviceList(idx).Length > 0) AndAlso
                                        (deviceName.ToUpper.Contains(sDeviceList(idx).ToUpper) = True)) Then
                                        Exit For
                                    End If
                                Next
                                ' If none of the IDs were found then continue looking
                                If (idx >= sDeviceList.Length) Then Continue For
                            End If
                            '
                            If ((IsNothing(sDeviceIDList) = False) AndAlso (sDeviceIDList.Length > 0)) Then
                                Dim idx As Integer
                                For idx = 0 To sDeviceIDList.Length - 1
                                    If ((sDeviceIDList(idx).Length > 0) AndAlso
                                        (deviceID.ToUpper.Contains(sDeviceIDList(idx).ToUpper) = True)) Then
                                        Exit For
                                    End If
                                Next
                                ' If none of the IDs were found then continue looking
                                If (idx >= sDeviceIDList.Length) Then Continue For
                            End If
                            '
                            If ((IsNothing(sHardwareIDList) = False) AndAlso (sHardwareIDList.Length > 0)) Then
                                Dim idx As Integer
                                For idx = 0 To sHardwareIDList.Length - 1
                                    If ((sHardwareIDList(idx).Length > 0) AndAlso
                                        (deviceHardwareIDs.ToUpper.Contains(sHardwareIDList(idx).ToUpper) = True)) Then
                                        Exit For
                                    End If
                                Next
                                ' If none of the IDs were found then continue looking
                                If (idx >= sHardwareIDList.Length) Then Continue For
                            End If
                            '
                            If (DeviceNames.Contains(deviceName) = False) Then
                                DeviceNames.Add(deviceName)
                                DeviceInfoNow.DeviceName = deviceName
                                If (devicePNPID.Length > 0) Then
                                    DeviceInfoNow.DeviceID = devicePNPID
                                Else
                                    DeviceInfoNow.DeviceID = deviceID
                                End If
                                DeviceInfoNow.DeviceCaption = deviceCaption
                                DeviceInfoNow.DeviceDescription = deviceDescription
                                DeviceInfoNow.DevicePortName = devicePortName
                                DeviceInfoNow.DeviceDriverName = deviceDriverName
                                DeviceInfoNow.DeviceManufacturer = deviceManufacturer
                                DeviceInfoNow.DeviceHardwareIDs = deviceHardwareIDs
                                DeviceInfoNow.UserChoice = userchoice
                                DeviceInfoList.Add(DeviceInfoNow)
                                iReturn = iReturn + 1
                            End If
                        Next
                    End If
                Catch ex As Exception
                    Applog("Err:CheckForDevice |ex1: " & ex.Message)
                End Try
            Else
                Applog("CheckForDevice | No_search_results")
            End If
        Catch ex As Exception
            Applog("CheckForDevice | ex2: " & ex.Message)
        End Try
        Try
            If (IsNothing(info) = False) Then
                info.Dispose()
                info = Nothing
            End If
        Catch ex As Exception
            Applog("CheckForDevice | ex3: " & ex.Message)
        End Try
        Try
            If (IsNothing(searchinfo) = False) Then
                searchinfo.Dispose()
                searchinfo = Nothing
            End If
        Catch ex As Exception
            Applog("Err:CheckForDevice | ex4 " & ex.Message)
        End Try
        Try
            If (IsNothing(search) = False) Then
                search.Dispose()
                search = Nothing
            End If
        Catch ex As Exception
            Applog("CheckForDevice | ex5:'" & ex.Message & "'")
        End Try
        Try
            If ((IsNothing(DeviceNames) = False) AndAlso (DeviceNames.Count > 0) AndAlso (iReturn > 0) AndAlso
            (IsNothing(DeviceInfoList) = False) AndAlso (DeviceInfoList.Count > 0)) Then
                sDeviceInfo = DeviceInfoList.ToArray

            End If
        Catch ex As Exception
            iReturn = 0
            Applog("Err:CheckForDevice | Ending-ex:" & ex.Message)
        End Try

        Applog("... CheckForDevice |" & iReturn.ToString)
        Return iReturn
    End Function


    Public Function lFindUSBDevices() As String

        Dim lStatus As String = Nothing

        Dim sReturn() As String = Nothing
        Dim sDevices() As DeviceInfoStruct = Nothing
        Dim iDeviceCount As Integer = 0
        Try
            'sReturn = CheckForDevice("IMclean", "USB")
            iDeviceCount = CheckForDevice("IMclean", "USB", "", sDevices, "")
            If ((iDeviceCount > 0) AndAlso (IsNothing(sDevices) = False) AndAlso (sDevices.Length > 0)) Then
                Dim DeviceNames As List(Of String) = New List(Of String)
                For Each sDeviceInfo As DeviceInfoStruct In sDevices
                    If (DeviceNames.Contains(sDeviceInfo.DeviceName) = False) Then
                        DeviceNames.Add(sDeviceInfo.DeviceName)
                    End If
                Next
                sReturn = DeviceNames.ToArray
            End If

        Catch ex As Exception
            lStatus = "Err:FindUSBDevices | " & ex.Message
        End Try

        If (IsNothing(sReturn) = False) Then

            If sReturn.Count > 0 Then

                lStatus = "Pass:FindUSBDevices."

            End If
        Else

            lStatus = "Err:FindUSBDevices | Dispositivo IMClean no detectado. "

        End If

        Applog(lStatus)
        Return lStatus

    End Function

End Module
