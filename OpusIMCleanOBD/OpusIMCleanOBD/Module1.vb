Imports System.IO
Imports System.Xml

Imports DT

Module Module1

    Public logFile As String = "Log_OpusIMCleanOBD.txt"

    Public lrdError As String = Nothing
    'Public pathDevProfile As String = "C:\PRY_NT\tmp\OpusIMCleanOBD_files\OpusConfigOBD.xml"

    Public MyDad As DT.DAD.IMClean = Nothing

    Public MyObdProtocolCAN As Boolean = False

    '//--

    Public LrdOBD_STATUS_Cadd As String
    Public OBDdataBus As String()
    Public OBDdata_VIN As String
    Public OBDdata_VINtxt As String
    Public OBDdata_MIL As String
    Public OBDdata_MILtxt As String
    Public OBDdata_DTC As String
    Public OBDdata_DTCtxt As String

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

    '||--


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

    Public MyDAD_ConnectCount As Integer = 0
    Public MyEngineId As Byte = 0

    Public MyLastConnectVIN As String = ""
    Public MyLastConnectSpecialControl As String = ""
    Public MyLastConnectCallingProcedure As String = ""

    Public Connect_Succeeded As Boolean
    Public Connect_MyEngineId As Byte
    Public Connect_MyObdProtocol As String
    Public Connect_LinkProtocol As String
    Public Connect_ConnectModuleHashtable As Hashtable
    Public Connect_TotalPIDCountDCount As Integer
    Public Connect_ConnectVIN As String = ""

    Public Class ConnectResult '------------------------------------------------------
        'Inherits Result
        Public MyExtendedStatus As OBDLinkExtendedStatus
        Public MyEngineModuleId As Byte
        Public MyModuleHashtable As Hashtable
        Public MyConnectProtocol As String
        Public MyLinkProtocol As String
        Public MyTotalPIDCount As Integer
        Public MyDataDateTime As DateTime
        Public MyComLogData As String
        Public MyConnectVIN As String = ""

        Public Sub New(ByVal Succeeded As Boolean,
                       ByVal EngineModuleID As Byte,
                       ByVal ConnectProtocol As String,
                       ByVal LinkProtocol As String,
                       ByVal ConnectHashTable As Hashtable,
                       ByVal TotalPIDCount As Integer)
            'MyBase.New("Connect", Succeeded, EngineModuleID, ConnectHashTable)

            MyConnectProtocol = ConnectProtocol
            MyLinkProtocol = LinkProtocol
            MyTotalPIDCount = TotalPIDCount
            MyConnectVIN = ""
        End Sub

        Public ReadOnly Property ConnectProtocol() As String
            Get
                ConnectProtocol = MyConnectProtocol
            End Get
        End Property

        Public ReadOnly Property LinkProtocol() As String
            Get
                LinkProtocol = MyLinkProtocol
            End Get
        End Property

        Public ReadOnly Property ConnectTotalPIDCount() As Integer
            Get
                ConnectTotalPIDCount = MyTotalPIDCount
            End Get
        End Property

        Public ReadOnly Property ConnectModulePIDCount() As Integer
            Get
                ConnectModulePIDCount = ConnectModulePIDCount(Me.MyEngineModuleId)
            End Get
        End Property

        '<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
        Public ReadOnly Property ConnectModulePIDCount(ByVal ModuleID As Byte) As Integer
            Get
                ConnectModulePIDCount = 0
                Try
                    If (IsNothing(MyModuleHashtable) = False) Then
                        If (MyModuleHashtable.ContainsKey(ModuleID)) Then
                            ConnectModulePIDCount = CInt(MyModuleHashtable.Item(ModuleID))
                        End If
                        For Each Entry As DictionaryEntry In MyModuleHashtable
                            If (CInt(Entry.Key) = ModuleID) Then
                                ConnectModulePIDCount = CInt(Entry.Value)
                            End If
                        Next
                    End If
                Catch ex As Exception
                End Try
            End Get
        End Property

        Public Property ConnectVIN As String
            Get
                ConnectVIN = MyConnectVIN
            End Get
            Set(value As String)
                MyConnectVIN = value
            End Set
        End Property

    End Class '------------------------------------------------------


    '-------------

    Public Const LOGLEVEL_CONFIG_SECTION As String = "ObdLink/Default"
    Public Const LOGLEVEL_CONFIG_KEY As String = "LogLevel"
    Public Const LOGLEVEL_CONFIG_DISPLAY_ERROR As Boolean = False
    Public Const LOGLEVEL_DEFAULT As Integer = 1
    Public Const LOGFILE_PREFIX As String = "OBDDrewDAD"
    Public Const LOGLEVEL_USE_GLOBAL As Boolean = False

    Const OBDLINK_TYPE_CONFIG As String = "ObdLink"
    Const OBDLINK_TYPE_KEY As String = "OBDLinkType"

    Const OBDLINK_FIRMWARE_CONFIG As String = "ObdLink"
    Const OBDLINK_FIRMWARE_KEY_BASE As String = "Firmware"

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
    'Public Const OBDLINK_NAME_SIMULATOR As String = "Simulator"

    Public OBDLinkType As Integer

    Public MyOBDLinkActiveCount As Integer = 0

    Public ts As TraceSource

    'Public OBDLinkInfo As OBDLinkInformation

    Public MyLastExtendedStatus As OBDLinkExtendedStatus = OBDLinkExtendedStatus.Unknown

    Public ECUArray(100, 2) As String  ' Only for mode 1
    Public CVNArray(100, 2) As String
    Public CALIDArray(100, 3) As String
    'Dim L_SortedL_PIDCountDictionary As Dictionary(Of Byte, Integer)

    Public MyLogLevel As Integer = LOGLEVEL_DEFAULT

    Public IsMode1PidSupported_LastPID As Byte = 0
    Public IsMode1PidSupported_LastResult As Boolean = False

    Public ConnectModuleHashtable As Hashtable = Nothing

    Public MyLastCommResult As DrewTech.IIMClean.DT_Com_Result = DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect

    Public PidSupportDictionary As New Dictionary(Of Byte, BitArray)     ' Mode 1 PID support per ECU
    Public PIDCountDictionary As New Dictionary(Of Byte, Integer)        ' Mode 1 PID support count per ECU
    Public SortedPIDCountDictionary As New Dictionary(Of Byte, Integer)  ' Mode 1 PID support count per ECU sorted by count
    Public PidSupportOverall As BitArray = Nothing                       ' Mode 1 PID support overall (all ECUs)
    Public Mode1ECUCount As Integer = 0                                  ' Mode 6 count of responding ECUs
    Public TotalPIDCount As Integer = 0                                  ' Mode 1 total PID count

    Public MyDadOpenTime As DateTime = DateTime.MinValue
    Public MyDAD_SpecialControl As String = ""
    Public MyDadOpen As Boolean = False
    Public MyDADPort As String = ""
    Public MyDoDADSemaphore As System.Threading.Semaphore = Nothing
    Public MyDAD_AllowUpdateRequiredContinue As Boolean = False
    Public MyDAD_FullOpenClose As Boolean = True
    Public MyDAD_DevicesTicks As Int32 = System.Environment.TickCount
    Public MyDADSemaphore As System.Threading.Semaphore = Nothing
    Public MyDAD_InitializeStatusDateTime As DateTime = DateTime.MinValue
    Public MyIMCleanDriverVersion As Integer = 0
    Public MyDAD_SupportedProducts() As String = Nothing
    Public MyDAD_DeviceConnection As String = Nothing
    Public MyDADLastGetLocation As String = ""
    Public MyDADAccessCount As Long = 0
    Public MyDADLastReleaseLocation As String = ""
    Public MyDeviceType As Integer = 0
    Public MyDAD_InitialMaxWaitTime As Integer = 600

    Public MyDAD_InitializeStatus As String = ""

    Public InitializeInterfaceAbort As Boolean = False

    Public PidCountIncludePID0 As Boolean = False
    Public PidCountIncludePIDx20 As Boolean = True
    Public OBDRPMFromEngineOnly As Boolean = False

    Public LogLowLevelData As String = ""

    Public MyDAD_Devices() As String = Nothing
    Public MyDAD_DevicesCount As Integer = 0

    Public MySerialDataLogMaxBytes As Double = 160000
    Public MyVendorDataLogMaxBytes As Double = 120000

    Public MyIMCleanDriverDLLVersion As String = ""

    Public MyObdProtocol As String

    Public Save_VehicleIsLinked As String = ""

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
    Public Mode9TotalPIDCount As Integer = 0                                  ' Mode 9 total PID count

    Public MyLastFunctionSeconds As Double = -1
    Public MyCurrentSerialDataLog As String = ""
    Public MyCurrentSerialDataLogSize As Long = 0

    Public MyDeviceLogClear As Boolean = True
    Public MyDeviceLogClearStatus As String = "?"
    Public MyDeviceLogStatusDateFormat As String = "yyyy-MM-dd_HH:mm:ss"
    Public MyLastClearLogs As DateTime = DateTime.MinValue
    Public MyDeviceLogSave As Boolean = False

    Public MyLastExtendedStatusDateTime As DateTime = DateTime.MinValue

    Public MyCurrentSerialDataLogIsCommandLog As Boolean = False

    Public MyCurrentVendorDataLog As String = ""
    Public MyCurrentVendorDataLogSize As Long = 0

    Public MyLastConnect As DateTime = DateTime.MinValue
    Public MyLastReConnect As DateTime = DateTime.MinValue
    Public MyDeviceLogSaveStatus As String = "?"

    Public lvalue As Integer
    '----------------------------------------------

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

    Public Structure DeviceInfoStruct
        Public DeviceName As String
        Public DeviceCaption As String
        Public DeviceDescription As String
        Public DeviceID As String
        Public DevicePortName As String
        Public DeviceDriverName As String
        Public DeviceManufacturer As String
        Public DeviceHardwareIDs As String
        Public UserChoice() As String
    End Structure
    Public strDeviceInfo As DeviceInfoStruct

    Public Structure OBDWorkParams
        Dim sCommand As String
        Dim sLocation As String
        Dim oInParameters() As Object
        Dim oOutParameters() As Object
    End Structure

    Public Class OBDLinkInformation
        Public Port As String
        Public PortWired As String
        Public PortWireless As String
        Public InitializationStatus As String
        Public InitializationStatusDateTime As DateTime
        Public DLLVersion As String
        Public DriverVersion As String
        Public FirmwareVersion As String
        Public SerialNumber As String
        Public LatestFirmwareVersion As String
        Public LastDeviceExtendedStatus As String
        Public LastDeviceExtendedStatusDateTime As DateTime
        Public VehicleIsLinked As String
        '
        Public BatteryInformationAvailable As Boolean
        Public BatteryInformationUpdateDateTime As DateTime
        Public BatteryCycleCount As Single
        Public BatteryStateOfCharge As Single
        Public BatteryHealth As Single
        Public BatteryTemperature As Single
        Public BatteryCurrent As Single
        '
        Public DeviceInformationAvailable As Boolean
        Public DeviceInformationUpdateDateTime As DateTime
        Public DeviceSupportList() As String
        Public DeviceCurrentList() As String
        Public DeviceConnection As String

        Public Sub New()
            Me.Port = ""
            Me.PortWired = ""
            Me.PortWireless = ""
            Me.InitializationStatus = "?"
            Me.InitializationStatusDateTime = DateTime.MinValue
            Me.DLLVersion = "?"
            Me.DriverVersion = "?"
            Me.FirmwareVersion = "?"
            Me.SerialNumber = "?"
            Me.LatestFirmwareVersion = "?"
            Me.LastDeviceExtendedStatus = "?"
            Me.LastDeviceExtendedStatusDateTime = DateTime.MinValue
            Me.VehicleIsLinked = "?"
            '
            Me.BatteryInformationAvailable = False
            Me.BatteryInformationUpdateDateTime = DateTime.Now.AddHours(-10)
            Me.BatteryCycleCount = 0.0
            Me.BatteryStateOfCharge = 0.0
            Me.BatteryHealth = 0.0
            Me.BatteryTemperature = 0.0
            Me.BatteryCurrent = 0.0
            '
            Me.DeviceInformationAvailable = False
            Me.DeviceInformationUpdateDateTime = DateTime.Now.AddHours(-10)
            Me.DeviceSupportList = Nothing
            Me.DeviceCurrentList = Nothing
            Me.DeviceConnection = ""
        End Sub
    End Class

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



    Public Function CheckForDevice(ByVal sDevice As String, ByVal sDeviceID As String, ByVal sHardwareID As String,
                                ByRef sDeviceInfo() As DeviceInfoStruct, Optional ByVal sControl As String = "") As Integer
        Applog("... CheckForDevice")
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
                            strDeviceInfo.DeviceID = deviceID
                            deviceHardwareIDs = GetPropertyValueAsString(info, "HardwareID")
                            strDeviceInfo.DeviceHardwareIDs = deviceHardwareIDs
                            devicePortName = GetPropertyValueAsString(info, "PortName")
                            strDeviceInfo.DevicePortName = devicePortName
                            deviceManufacturer = GetPropertyValueAsString(info, "Manufacturer")
                            strDeviceInfo.DeviceManufacturer = deviceManufacturer
                            devicePNPID = GetPropertyValueAsString(info, "PNPDeviceID")
                            devicePNPClass = GetPropertyValueAsString(info, "PNPClass")
                            deviceCaption = GetPropertyValueAsString(info, "Caption")
                            deviceName = GetPropertyValueAsString(info, "Name")
                            deviceClass = GetPropertyValueAsString(info, "CreationClassName")
                            deviceDescription = GetPropertyValueAsString(info, "Description")
                            strDeviceInfo.DeviceDescription = deviceDescription
                            deviceDriverName = GetPropertyValueAsString(info, "DriverName")
                            strDeviceInfo.DeviceDriverName = deviceDriverName
                            If ((IsNothing(UC_Property) = False) AndAlso (UC_Property.Length > 0)) Then
                                For idx As Integer = 0 To UC_Property.Length - 1
                                    userchoice(idx) = GetPropertyValueAsString(info, UC_Property(idx))
                                Next
                            End If
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
                    Applog("Err:CFD |ex1: " & ex.Message)
                End Try
            Else
                Applog("CFD | No_search_results")
            End If
        Catch ex As Exception
            Applog("CFD | ex2: " & ex.Message)
        End Try
        Try
            If (IsNothing(info) = False) Then
                info.Dispose()
                info = Nothing
            End If
        Catch ex As Exception
            Applog("CFD | ex3: " & ex.Message)
        End Try
        Try
            If (IsNothing(searchinfo) = False) Then
                searchinfo.Dispose()
                searchinfo = Nothing
            End If
        Catch ex As Exception
            Applog("Err:CFD | ex4 " & ex.Message)
        End Try
        Try
            If (IsNothing(search) = False) Then
                search.Dispose()
                search = Nothing
            End If
        Catch ex As Exception
            Applog("CFD | ex5:'" & ex.Message & "'")
        End Try
        Try
            If ((IsNothing(DeviceNames) = False) AndAlso (DeviceNames.Count > 0) AndAlso (iReturn > 0) AndAlso
            (IsNothing(DeviceInfoList) = False) AndAlso (DeviceInfoList.Count > 0)) Then
                sDeviceInfo = DeviceInfoList.ToArray

            End If
        Catch ex As Exception
            iReturn = 0
            Applog("Err:CFD | Ending-ex:" & ex.Message)
        End Try
        Applog("CFD | Ending ... Cnt:" & iReturn.ToString)
        Return (iReturn)
    End Function


    Public Function IsMode1PidSupported(ByVal Pid As Byte, Optional ByVal ECUList As Byte() = Nothing) As Boolean
        Applog("... IsMode1PidSupported")

        IsMode1PidSupported = False
        If ((IsNothing(PidSupportDictionary) = True) OrElse (PidSupportDictionary.Count <= 0)) Then
            GetSupportedPIDs(1)
        End If
        Dim PidEcuList As Byte()
        PidEcuList = GetECUPIDSupportList(Pid)
        If ((IsNothing(PidEcuList) = False) AndAlso (PidEcuList.Length > 0)) Then
            If ((IsNothing(ECUList) = True) OrElse (ECUList.Length <= 0)) Then
                IsMode1PidSupported = True
            Else
                Dim idx1 As Integer = 0
                Do
                    For idx2 As Integer = 0 To ECUList.Length - 1
                        If (ECUList(idx2) = PidEcuList(idx1)) Then
                            IsMode1PidSupported = True
                            Exit Do
                        End If
                    Next
                    idx1 = idx1 + 1
                Loop While (idx1 < PidEcuList.Length)
            End If
        End If
        IsMode1PidSupported_LastPID = Pid
        IsMode1PidSupported_LastResult = IsMode1PidSupported
        Applog("IsM1PS | PID:" & IsMode1PidSupported_LastPID.ToString("X02") & ":" & IsMode1PidSupported_LastResult.ToString)
    End Function


    Public Sub GetSupportedPIDs(ByVal Mode As Byte)
        Applog("... GetSupportedPIDs")

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
        '
        'PidSupportDictionary = Nothing
        'PIDCountDictionary = Nothing
        'SortedPIDCountDictionary = Nothing
        'PidSupportOverall = Nothing
        ReDim L_ECUArray(ECUArray.GetUpperBound(0), ECUArray.GetUpperBound(1))
        '
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

    Public Function IncludePIDInPIDCount(ByVal ParameterID As Integer) As Boolean
        'Applog("... IncludePIDInPIDCount")
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
        Return (bReturn)
    End Function


    Public Function IncludePID0InPIDCount_borra() As Boolean
        'Applog("... IncludePID0InPIDCount")

        Dim PIDCountIncludePID0 As Boolean = False
        Dim PIDCountIncludeUpdated As Boolean = False
        If (PIDCountIncludeUpdated = False) Then

            Try
                PIDCountIncludePID0 = True
                PIDCountIncludeUpdated = True
            Catch ex As Exception
                Applog("Err:IncludePID0InPIDCount | " & ex.Message)
            End Try

        End If

        Return (PIDCountIncludePID0)

    End Function


    Public Function IncludePIDx20InPIDCount_borra() As Boolean
        'Applog("... IncludePIDx20InPIDCount")

        Static PIDCountIncludePIDx20 As Boolean = True
        Static PIDCountIncludeUpdated As Boolean = False
        If (PIDCountIncludeUpdated = False) Then

            Try
                'If RdXML(pathDevProfile, "/Profile/ObdLink/Default/PidCountIncludePIDx20") = "True" Then
                'PIDCountIncludePIDx20 = True
                'Else
                'PIDCountIncludePIDx20 = False
                'End If
                PIDCountIncludePIDx20 = True
                PIDCountIncludeUpdated = True
            Catch ex As Exception
                Applog("Err:IncludePIDx20InPIDCount | " & ex.Message)
            End Try

        End If
        Return (PIDCountIncludePIDx20)
    End Function

    Public Sub SetSupportedBitArrayFromObdBytes(ByVal ObdBytes() As Byte, ByRef SupportedBitArray As BitArray,
                                                 ByVal StartIndex As Integer, ByVal sDebug As String)
        Applog("... SetSupportedBitArrayFromObdBytes")

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
        Applog("SSBAFOB | BitAryLen:" & SupportedBitArray.Length.ToString("000") &
                               "  BitSetCnt:" & iTemp.ToString("000") &
                               "  BAI:" & sTemp)
    End Sub


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
        If ((bLogFlag = True) OrElse (MyDadOpen = False) OrElse (GetMyLogLevel() > 4)) Then
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

        If (MyDoDADSemaphore.WaitOne(2000) = True) Then
            Try
                Dim StartDateTime As DateTime = DateTime.Now
                If ((MyDadOpen = False) AndAlso (sControl.ToUpper.Contains("SupressAutoOpen".ToUpper) = False)) Then
                    Applog("DoDad-MyDoDADSemaphore-" & iCount.ToString("00") & " | " & Tag & " ... Attempting Open")
                    OpenDAD_If_Needed("DoDad-" & Tag & " (" & CallingProcedure & ")")
                End If
                '
                MyCurrentSerialDataLog = ""
                MyCurrentSerialDataLogSize = 0
                '
                If (Tag.ToUpper.Contains("Close".ToUpper) = True) Then
                    Applog("Doing close ...")
                End If
                '
                If (sControl.ToUpper.Contains("SupressConnectCheck".ToUpper) = False) Then
                    If ((bIncludeVoltage = True) AndAlso
                        (MyDadOpen = True) AndAlso
                        (MyLastCommResult <> DrewTech.IIMClean.DT_Com_Result.Success)) Then
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
                        Catch ex As Exception
                            Applog("Err:DoDad-" & iCount.ToString("00") & " | " & Tag & "-CnctChk-ex:'" & ex.Message)
                        End Try
                    End If
                End If
                '
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
                Try
                    If ((bConnected = True) AndAlso (IsNothing(MyDad) = False)) Then
                        Applog("DoDad-" & iCount.ToString("00") & "  ClearCommandLog ...")
                        MyDad.ClearCommandLog()
                    End If
                Catch ex As Exception
                    Applog("Err:DoDad-" & iCount.ToString("00") & " | " & Tag & "-exCC: " & ex.Message)
                End Try
                '
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

                Const LogEntryPrefix As String = vbCrLf & "                    -  "
                MyCurrentSerialDataLog = ""
                MyCurrentSerialDataLogSize = 0
                If (IsNothing(a) = False) Then
                    Try

                        MyCurrentSerialDataLogIsCommandLog = True
                        Applog("DoDad-" & iCount.ToString("00") & "  CommandLog.Length ...")
                        MyCurrentSerialDataLogSize = MyDad.CommandLog.Length

                        Applog("DoDad-" & iCount.ToString("00") & "  CommandLog ... (" & MyCurrentVendorDataLogSize.ToString("0") & ")")

                        MyCurrentSerialDataLog = Microsoft.VisualBasic.Left(MyDad.CommandLog, CInt(MySerialDataLogMaxBytes))

                        'Applog("MyDad.CommandLog: " & MyDad.CommandLog)

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
                    'Applog("DoDad-" & iCount.ToString("00") & "  SerialDataLog.Length ...")
                    MyCurrentSerialDataLogSize = MyDad.SerialDataLog.Length
                    Applog("DoDad-" & iCount.ToString("00") & "  SerialData.Length ... (" & MyCurrentSerialDataLogSize.ToString("0") & ")")
                    MyCurrentSerialDataLog = MyDad.SerialDataLog  ' Get all bytes (for now)
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

                MyCurrentSerialDataLog = Microsoft.VisualBasic.Right(MyCurrentSerialDataLog,
                                                                     CInt(MySerialDataLogMaxBytes)).Trim
                ' Do some cleanup to make the log pretty
                If (MyCurrentSerialDataLog.Length >= 2) Then
                    If (Right(MyCurrentSerialDataLog, 2) = vbCrLf) Then
                        MyCurrentSerialDataLog = Left(MyCurrentSerialDataLog, MyCurrentSerialDataLog.Length - 2)
                    End If
                End If
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

                If (Tag.ToUpper.Contains("CALID".ToUpper) = True) Then
                    Applog("Tag = CALID")
                End If
                If (Tag.ToUpper.Contains("BATTERY".ToUpper) = True) Then
                    Applog("Tag = BATTERY")
                End If

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
                        ElapsedSeconds = DeltaTimeTicks(StartTicks)
                        Applog("VendorLogTime3: " & ElapsedSeconds.ToString("0.000"))
                        If (GetMyLogLevel() > 0) Then
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

                            ElapsedSeconds = DeltaTimeTicks(StartTicks)

                            ElapsedSeconds = DeltaTimeTicks(StartTicks)

                        End If
                    Catch ex As Exception
                        MyDeviceLogSaveStatus = "SaveError:'" & ex.Message & "'"
                        Applog("Err:DoDad" & " | " & ex.Message)
                    End Try
                End If
            Catch ex As Exception
                LogText = "ex:'" & ex.Message & "'"
            End Try
            MyDoDADSemaphore.Release(1)
        Else
            LogText = Tag & "-" & "Semaphore_Timeout"
        End If
        Applog("DoDad-" & iCount.ToString("00") & " | " & Tag & "-" & LogText & " | " & SpecialLogControl)
        'DbgWrite("DoDad-" & iCount.ToString("00") & "  " & Tag & "-" & LogText.Replace(vbCrLf, " .. "))
        iCount = iCount - 1
        If (iCount < 0) Then iCount = 0

        '------------------------------

        'Applog("TraceLog:" & MyDad.TraceLog)
        'Applog("CommandLog:" & MyDad.CommandLog)
        'Applog("OBDProtocol:" & MyDad.OBDProtocol)
        'Applog("VendorLog:" & MyDad.VendorLog)
        'Applog("SerialDataLog:" & MyDad.SerialDataLog)
        'Applog("Port:" & MyDad.Port)

    End Sub

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



    Public Function GetDrewDeviceCount(Optional ByVal sControl As String = "") As Integer
        Applog("... GetDrewDeviceCount")

        Dim CheckSeconds As Double = 4.0    ' No need to check devices too frequently / 6.0

        If (DeltaTimeTicks(MyDAD_DevicesTicks) > CheckSeconds) Then
            GetDrewDevices(sControl)
        End If
        OBDLinkInfo_DeviceCurrentList = MyDAD_Devices
        Return (MyDAD_DevicesCount)
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




    Public Function GetECUPIDSupportList(ByVal ParameterID As Byte) As Byte()
        Applog("... GetECUPIDSupportList")

        Dim bReturn(-1) As Byte
        Dim iReturnCount As Integer = 0
        Try
            If (ParameterID = 1) Then
                Debug.WriteLine("GetECUPIDSupportList-PID:" & ParameterID.ToString("X02"))
            End If
            For Each spdEntry As KeyValuePair(Of Byte, Integer) In SortedPIDCountDictionary
                For Each e As KeyValuePair(Of Byte, BitArray) In PidSupportDictionary
                    'If (e.Key <> 0) Then
                    If (e.Key = spdEntry.Key) Then
                        Dim ba As BitArray = e.Value
                        If (ba(ParameterID) = True) Then
                            If (iReturnCount >= 0) Then
                                ReDim Preserve bReturn(iReturnCount)
                            End If
                            bReturn(iReturnCount) = e.Key
                            iReturnCount = iReturnCount + 1
                        End If
                    End If
                Next
            Next
        Catch ex As Exception
            Applog("GetECUPIDSupportList | " & ex.Message)
        End Try
        '
        Dim sTemp As String = ""
        Try
            If ((IsNothing(bReturn) = False) AndAlso (bReturn.Count > 0)) Then
                For Each bByte As Byte In bReturn
                    If (sTemp.Length > 0) Then sTemp = sTemp & ", "
                    sTemp = sTemp & bByte.ToString("X02")
                Next
            End If
        Catch ex As Exception
            Applog("Err:GetECUPIDSupportList | " & ex.Message)
        End Try
        Applog("GetECUPIDSupportList | PID:" & ParameterID.ToString("X02") & " Ret-" & iReturnCount.ToString("000") & ": " & sTemp)
        Return (bReturn)
    End Function



    Public Function GetECUArray() As String(,)   'Only used in NY
        'Applog("... GetECUArray")

        Dim sReturn(,) As String = {{"0", "0", "0"}}
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


    Public Function GetCallingProcedure(Optional ByVal CallDepth As Integer = 2,
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
                'StackFrame = New Diagnostics.StackFrame(CallDepth)
                ' sreturn = StackFrame.GetMethod.DeclaringType.FullName
                sReturn = StackFrame.GetMethod.Name.ToString()
                If (NameOnly = False) Then
                    sReturn = StackFrame.GetMethod.DeclaringType.FullName.ToString() & ":" & sReturn
                End If
            End If
        Catch ex As Exception
            Debug.WriteLine("GetCallingProcedure-ex:'" & ex.Message & "'")
        End Try
        StackFrame = Nothing
        StackTrace = Nothing

        'Applog("... GetCallingProcedure | " & sReturn)
        Return (sReturn)
    End Function



    Public Function GetIMCleanDriverVersion() As String
        Applog("... GetIMCleanDriverVersion")

        Dim sReturn As String = "?"
        Try
            MyIMCleanDriverVersion = MyDad.IMCleanDriverVersion
            sReturn = MyIMCleanDriverVersion.ToString("0000")
        Catch ex As Exception
            Applog("Err:GetIMCleanDriverVersion | " & ex.Message)
        End Try
        Applog("Err:GetIMCleanDriverVersion | Ret: " & sReturn)
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
        '
        Try
            If (sInterfaceDLLName.Length < 3) Then
                sInterfaceDLLName = "C:\PRY_NT\OpusIMclean_OBD\OpusIMCleanOBD\OpusIMCleanOBD\IMCleanDriver.dll" 'System.IO.Path.Combine(Sti.RunTime.Context.RunPath, "IMCleanDriver.dll")
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

    Public Function MyLastResultSuccess() As Boolean
        Applog("... MyLastResultSuccess")

        Dim bReturn As Boolean = False
        If (MyLastResult().ToUpper.Contains("Success".ToUpper) = True) Then
            bReturn = True
        End If
        Return (bReturn)
    End Function

    Private Function SetupOBDDeviceType(ByVal GenericResponse As DrewTech.IIMClean.DT_IGenericResponse(Of Integer)) As Boolean '-- borra
        Applog("... SetupOBDDeviceType")

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
                    MsgBox("0002 iNewType: " & iNewType)
                End If
            End If
        End If
        Applog("SetupOBDDeviceType |  Ret:" & bReturn.ToString &
                                          " USBCnt:" & iWiredCount.ToString("0") &
                                          " ComSuc:" & bCommSuccess.ToString &
                                          " CrntTyp:" & iCurrentType.ToString("0") &
                                          " Typ:" & MyDeviceType.ToString("0")) '&
        '"  LnkTyp:" & Me.OBDLinkTypeAsString)
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
                        'Applog("*** FirmwareVersion: " & sres.Data)
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


    Public Function GetVoltageInternal(ByVal bDLC As Boolean, ByVal sControl As String) As Single
        Applog("... GetVoltageInternal")

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
        Applog("GetVoltageInternal | Ret:" & rValue.ToString("0.00") & " Cntrl:'" & sControl & "'" & "  (" & MyLastResult() & ", " & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
        Return (rValue)
    End Function


    Public Function GetDeviceType(ByVal sDebug As String) As String
        Applog("... GetDeviceType")

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
        Return sReturn
    End Function



    Private Function GetDADPort() As String
        Applog("... GetDADPort")

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
        Applog("GetDADPort | " & sReturn)
        Return sReturn
    End Function

    Public Function IncrementActiveCount() As Integer
        Applog("... IncrementActiveCount")

        MyOBDLinkActiveCount = MyOBDLinkActiveCount + 1
        If (MyOBDLinkActiveCount > 9999) Then MyOBDLinkActiveCount = 999
        Return (MyOBDLinkActiveCount)
    End Function


    Public Function LocalIncrementActiveCount(ByVal sLogInfo As String) As Integer
        Applog("... LocalIncrementActiveCount")

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
        Return (iReturn)
    End Function


    Public Function DecrementActiveCount() As Integer
        Applog("... DecrementActiveCount")
        MyOBDLinkActiveCount = MyOBDLinkActiveCount - 1
        If (MyOBDLinkActiveCount < -999) Then MyOBDLinkActiveCount = -99
        Return (MyOBDLinkActiveCount)
    End Function


    Private Function LocalDecrementActiveCount(ByVal sLogInfo As String) As Integer
        Applog("... LocalDecrementActiveCount")

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
        Applog("... GetFirmwareVersion")

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
        Return fv
    End Function


    Private Function GetSerialNumber() As String
        Applog("... GetSerialNumber")

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
        Applog("GetSerialNumber | Sts:" & MyLastResult() & "  SN:'" & fv & "'")
        Return fv
    End Function

    Public Function OBDLinkTypeAsString(Optional ByVal iOBDLinkType As Integer = OBDLINK_TYPE_UNKNOWN) As String
        Applog("... OBDLinkTypeAsString")

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

        MsgBox(sTemp) '-- mja

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
                    System.Windows.Forms.Application.DoEvents()
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
                        System.Windows.Forms.Application.DoEvents()
                    End While
                    'If (MyThread.IsAlive) Then MyThread.Abort()
                    ' Wait a bit for the thread to end
                    WaitStart = System.Environment.TickCount
                    While ((MyThread.IsAlive = True) AndAlso
                           (DeltaTimeTicks(WaitStart, "MS") < 15000))
                        System.Threading.Thread.Sleep(100)
                        System.Windows.Forms.Application.DoEvents()
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


    Public Sub OnCreate()
        Applog("... OnCreate")

        'MyBase.OnCreate()
        Dim StartTicks As Int32 = System.Environment.TickCount
        Dim sTemp As String = ""
        'OBDLinkInfo = New OBDLinkInformation

        'InitLog("OnCreate")

        MyDADSemaphore = New System.Threading.Semaphore(1, 1)
        MyDoDADSemaphore = New System.Threading.Semaphore(1, 1)
        'ts = New TraceSource("ObdLink")
        'ts.TraceInformation("OnCreate")

        Try
            '-t- LogLowLevelData = MyConfig.ReadText(ConfigSection, "LogLowLevelData", LogLowLevelData)
            '-t- MyDAD_SpecialControl = MyConfig.ReadText(ConfigSection, "DADSpecialControl", MyDAD_SpecialControl)
            '-t- PidCountIncludePID0 = Sti.Peripherals.ObdLinks.ObdLink.IncludePID0InPIDCount
            '-t- PidCountIncludePIDx20 = Sti.Peripherals.ObdLinks.ObdLink.IncludePIDx20InPIDCount
            '-t- OBDRPMFromEngineOnly = MyConfig.ReadBoolean(ConfigSection, "OBDRPMFromEngineOnly", OBDRPMFromEngineOnly)
            '-t- MySerialDataLogMaxBytes = MyConfig.ReadDouble(ConfigSection, "SerialDataLogMaxBytes", MySerialDataLogMaxBytes)
            If (MySerialDataLogMaxBytes > 500000) Then MySerialDataLogMaxBytes = 500000
            If (MySerialDataLogMaxBytes < 500) Then MySerialDataLogMaxBytes = 500
            '-t- MyVendorDataLogMaxBytes = MyConfig.ReadDouble(ConfigSection, "VendorDataLogMaxBytes", MyVendorDataLogMaxBytes)
            If (MyVendorDataLogMaxBytes > 500000) Then MyVendorDataLogMaxBytes = 500000
            If (MyVendorDataLogMaxBytes < 500) Then MyVendorDataLogMaxBytes = 500

            '-t- If (MyDAD_SpecialControl.ToUpper.Contains("ALLOWUPDATEREQ".ToUpper) = True) Then
            '-t- MyDAD_AllowUpdateRequiredContinue = True
            '-t- End If
            MyDAD_AllowUpdateRequiredContinue = True

            Applog("OnCreate | PCInclPID0:" & PidCountIncludePID0.ToString &
                                    " PCInclPIDx20:" & PidCountIncludePIDx20.ToString &
                                    " RPMFromEngineOnly:" & OBDRPMFromEngineOnly.ToString &
                                    " SC:'" & MyDAD_SpecialControl & "'")
            '                        " IncVendorLog:" & OBDIncludeVendorLog.ToString)
        Catch ex As Exception
            Applog("Err:OnCreate | " & ex.Message)
            'ts.TraceEvent(TraceEventType.Critical, 0, "OnCreate error", ex.Message)
            Throw New ApplicationException(String.Format("ObdLinkDrewDAD::OnCreate exception {0}", ex.Message))
        End Try

        sTemp = ""
        Try
            InitializeInterfaceAbort = False
            Dim oOutParams(0) As Object
            oOutParams(0) = ""
            sTemp = DoOBDWithThread("Initialize", "OnCreate", Nothing, oOutParams, 140000, InitializeInterfaceAbort)
            If (sTemp.Length > 0) Then
                MyDAD_InitializeStatus = sTemp
                MyDAD_InitializeStatusDateTime = DateTime.Now
            End If

            Try
                If ((IsNothing(oOutParams(0)) = False) AndAlso (MyDAD_InitializeStatus.Length <= 0)) Then
                    MyDAD_InitializeStatus = CStr(oOutParams(0))
                    MyDAD_InitializeStatusDateTime = DateTime.Now
                End If
            Catch ex As Exception
                Applog("Err:OnCreate: " & ex.Message)
            End Try

            If ((sTemp.Length <= 0) AndAlso (MyDAD_InitializeStatus.Length <= 0)) Then
                'MyDAD_InitializeStatus = "Success"
                MyDAD_InitializeStatus = "Unknown"
                MyDAD_InitializeStatusDateTime = DateTime.Now
            End If

        Catch ex As Exception
            MyDAD_InitializeStatus = "EXCEPTION:'" & ex.Message & "'"
            MyDAD_InitializeStatusDateTime = DateTime.Now
            Applog("Err:OnCreate: " & ex.Message)
            'ts.TraceEvent(TraceEventType.Critical, 0, "OnCreate error", ex.Message)
            Throw New ApplicationException(String.Format("ObdLinkDrewDAD::OnCreate exception {0}", ex.Message))
        End Try

        OBDLinkInfo_InitializationStatus = MyDAD_InitializeStatus
        OBDLinkInfo_InitializationStatusDateTime = MyDAD_InitializeStatusDateTime

        Applog("OnCreate | MyOBDOpen:" & MyDadOpen.ToString &
                                "  InitSts:'" & MyDAD_InitializeStatus & "'" &
                                "  (" & FunctionSeconds(StartTicks) & ")")
    End Sub


End Module
