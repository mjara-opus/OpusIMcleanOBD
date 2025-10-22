Imports System.Net.NetworkInformation
Imports System.Reflection.Emit
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Module ModMJA

    Public Sub OpenDevice()
        '--mja

        Dim rResTT As DrewTech.IIMClean.DT_IGenericResponse(Of DrewTech.IIMClean.DT_SelfTestRes) = Nothing
        'Dim cres As DrewTech.IIMClean.DT_Com_Result = DrewTech.IIMClean.DT_Com_Result.InvalidData
        Dim rRes As DrewTech.IIMClean.DT_Com_Result = DrewTech.IIMClean.DT_Com_Result.InvalidData ' Nothing
        Dim rStr As DrewTech.IIMClean.DT_IGenericResponse(Of String) = Nothing
        Dim rInt As DrewTech.IIMClean.DT_IGenericResponse(Of Integer) = Nothing
        Dim rDec As DrewTech.IIMClean.DT_IGenericResponse(Of Decimal) = Nothing

        Try

            Form1.TextBox1.Text = "Comunicando dispositivo IMClean OBD, espere un momento..."
            MyDad = New DT.DAD.IMClean

            If (IsNothing(MyDad) = False) Then

                rRes = MyDad.Open()
                Applog(rRes.ToString())
                Form1.TextBox1.Text = "DAD: " & rRes.ToString()

                rStr = MyDad.GetFirmwareVersion()
                Applog(rStr.Data)
                Form1.TextBox5.Text = "FirmwareVersion: " & rStr.Data

                rInt = MyDad.GetDeviceType
                Applog(rInt.Data)
                Form1.TextBox3.Text = "DeviceType: " & rInt.Data

                rDec = MyDad.GetVoltage
                Applog(rDec.Data)
                Form1.TextBox4.Text = "Volt: " & rDec.Data

                rDec = MyDad.GetVoltageDLC
                Applog(rDec.Data)
                Form1.TextBox4.Text = Trim(Form1.TextBox4.Text) & " | DLC: " & rDec.Data

                'rResTT = MyDad.SelfTest
                'Applog(rResTT.Data)
                'Form1.TextBox10.Text = rResTT.Data()

                'Form1.TextBox10.Text = GetDrewDeviceConnection()

                MyDad.ClearCommandLog()

            Else

                Form1.TextBox3.Text = "OBD no detectado."

            End If

        Catch ex As Exception
            Applog("Err:OpenDevice | " & ex.Message)
            MsgBox(ex.Message)
        End Try

    End Sub


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



    '-------------------------------------------

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

    Public Sub InitializeStringArrayData()
        InitStringArray(ECUArray)
        InitStringArray(CVNArray)
        InitStringArray(CALIDArray)
    End Sub

    Private Sub InitStringArray(ByRef aString(,) As String)
        Try
            If ((IsNothing(aString) = False) AndAlso
                (aString.GetLength(0) > 0) AndAlso
                (aString.GetLength(1) > 0)) Then
                For idx0 As Integer = 0 To aString.GetLength(0) - 1
                    For idx1 As Integer = 0 To aString.GetLength(1) - 1
                        aString(idx0, idx1) = ""
                    Next
                Next
            End If
        Catch ex As Exception
            Applog("ISA-ex: " & ex.Message)
        End Try
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


    Public Function LocalDecrementActiveCount(ByVal sLogInfo As String) As Integer
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

        Applog("... Connect")

        Try

            Dim StartTicks As Int32 = System.Environment.TickCount
            Dim CallingProcedure As String = LocalGetCallingProcedure()
            LocalIncrementActiveCount(CallingProcedure)
            MyDAD_ConnectCount = MyDAD_ConnectCount + 1
            If (MyDAD_ConnectCount > 9999) Then MyDAD_ConnectCount = 9
            Applog("... Connect | ---------------- Connect ---------------" &
                    " VIN:'" & sVIN & "'" &
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
                        '
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
                                Form1.TextBox4.Text = "Dad.Initialize > Success"
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
                                Form1.TextBox4.Text = "Dad.Initialize > NOT Success"
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
            End If

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
                Applog("Err:Connect | " & ex.Message)
            End Try

            Applog("Connect | Rslt:" & Connect_Succeeded.ToString &
                               " EECUID:" & Connect_MyEngineId.ToString("X02") &
                               " ECUs:" & Mode1ECUCount.ToString("0") & "-(" & sTemp & ")" &
                               " Prot:'" & Connect_MyObdProtocol & "', '" & Connect_LinkProtocol & "'" &
                               " PC:" & Connect_TotalPIDCountDCount.ToString("0") &
                               "  (" & MyLastResult() & ", " & FunctionSeconds(StartTicks) & ")" & " (" & CallingProcedure & ")")
            LocalDecrementActiveCount(CallingProcedure)

            Form1.TextBox1.Text = "Connect: " & Connect_Succeeded.ToString
            Form1.TextBox2.Text = "Prot: " & Connect_MyObdProtocol
            Form1.TextBox3.Text = "LastResult: " & MyLastResult()

            '===================================

            Applog("... Get VIN >>>")

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
                        Applog("GetVIN | ECU:" & EcuId.ToString("X02") & "  VIN: " & OBDdata_VIN)
                    End If

                Next

                Call DECODE_Bus(MyDad.CommandLog)

                Form1.TextBox3.Text = "VIN Hx: " & OBDdata_VIN
                Form1.TextBox4.Text = "VIN: " & OBDdata_VINtxt

            Else
                Form1.TextBox3.Text = "VIN: NULL"
            End If

            Applog("... Get MON >>>")

            sr = Nothing
            MyDad.ClearLogs()
            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
            DoDad(Sub() sr = MyDad.GetModePID(1, 1), "GetModePID(01, 01) - Readiness, DTC count, MIL", "NoEOL")
            If (IsNothing(sr) = False) Then
                lStatus = "sr | Get MON: " & sr.CommResult & " | " & sr.Data.Count

                Call DECODE_Bus(MyDad.CommandLog)
                Form1.TextBox5.Text = "MON Hx: " & OBDdata_MIL
                Call DECODE_MIL(OBDdata_MIL)
                Form1.TextBox6.Text = "MON: " & OBDdata_MILtxt

            Else
                lStatus = "sr | Get MON: NULL"
                Form1.TextBox5.Text = lStatus
            End If
            Applog(lStatus)


            Applog("... Get DTC >>>")

            sr = Nothing
            MyDad.ClearLogs()
            SetupMyLastCommResult(DrewTech.IIMClean.DT_Com_Result.ConditionsNotCorrect, False)
            DoDad(Sub() sr = MyDad.GetModePID(3, 0), "GetModePID(03, 00) - DTC count, MIL", "NoEOL")
            If (IsNothing(sr) = False) Then
                lStatus = "sr | Get DTC: " & sr.CommResult & " | " & sr.Data.Count

                Call DECODE_Bus(MyDad.CommandLog)
                Form1.TextBox7.Text = "DTC Hx: " & OBDdata_DTC
                Call DECODE_DTC(OBDdata_DTC)
                Form1.TextBox8.Text = "DTC: " & OBDdata_DTCtxt

            Else
                lStatus = "sr | Get DTC: NULL"
                Form1.TextBox7.Text = lStatus
            End If
            Applog(lStatus)

            Applog("VIN Hx: " & OBDdata_VIN)
            Applog("VIN: " & OBDdata_VINtxt)

            Applog("MON Hx: " & OBDdata_MIL)
            Applog("MON: " & OBDdata_MILtxt)

            Applog("DTC Hx: " & OBDdata_DTC)
            Applog("DTC: " & OBDdata_DTCtxt)

            Applog("*** Connect ... EOF()")

        Catch ex As Exception

            lStatus = "Err:Connect | " & ex.Message
            Applog(lStatus)
            MsgBox(lStatus)

        End Try

    End Sub


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



    Public Function SetupMyLastCommResult(ByVal CommResult As DrewTech.IIMClean.DT_Com_Result,
                                           ByVal VehicleCommFlag As Boolean,
                                           Optional ByVal NoDeviceCountCheck As Boolean = False) As DrewTech.IIMClean.DT_Com_Result
        'Applog("... SetupMyLastCommResult")

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

        'MyLastExtendedStatus = GetExtendedCommStatus(MyLastCommResult)
        'MyLastExtendedStatusDateTime = DateTime.Now
        'OBDLinkInfo.LastDeviceExtendedStatus = MyLastExtendedStatus.ToString
        'OBDLinkInfo.LastDeviceExtendedStatusDateTime = MyLastExtendedStatusDateTime
        'If ((NoDeviceCountCheck = False) AndAlso (GetDrewDeviceCount("NoLastCommResult") <= 0)) Then
        'OBDLinkInfo.LastDeviceExtendedStatus = "NO_OBD_DEVICES"
        'OBDLinkInfo.LastDeviceExtendedStatusDateTime = DateTime.Now
        'End If

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

        If InStr(pDataTxt, "41 01") > 0 Then '-- OBDdata_MIL 

            If Ix0 > 0 Then lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)

            OBDdata_MIL = Trim(lBustxt)
            'Call DECODE_MIL(OBDdata_MIL)

        Else

            If InStr(pDataTxt, "49 02") > 0 Then '-- OBDdata_MIL 

                If Ix0 > 0 Then lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)

                OBDdata_VIN = Trim(lBustxt)

            Else

                If InStr(pDataTxt, "43") > 0 Then '-- OBDdata_DTC 

                    If Ix0 > 0 Then lBustxt = Mid(pDataTxt, Ix0 + 1, zDB)

                    OBDdata_DTC = Trim(lBustxt)
                    'Call DECODE_DTC(OBDdata_DTC)

                End If

            End If

        End If

    End Sub


    Private Sub DECODE_MIL(ByVal pBufferDAT As String)

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
                Applog("A: " & Mid(xDato, 1, 2) & " | " & CInt("&H" & Mid(xDato, 1, 2)) & " | " & xA)
                Applog("B: " & Mid(xDato, 3, 2) & " | " & CInt("&H" & Mid(xDato, 3, 2)) & " | " & xB)
                Applog("C: " & Mid(xDato, 5, 2) & " | " & CInt("&H" & Mid(xDato, 5, 2)) & " | " & xC)
                Applog("D: " & Mid(xDato, 7, 2) & " | " & CInt("&H" & Mid(xDato, 7, 2)) & " | " & xD)

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
                    LrdOBD_MSI = "000" '-- No disponible se considera aprobatorio PVVO 2025

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
                    LrdOBD_CCM = "000" '-- No disponible se considera aprobatorio PVVO 2025

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
                    LrdOBD_CMB = "000" '-- No disponible se considera aprobatorio PVVO 2025

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
                    LrdOBD_O2S = "000" '-- No disponible se considera aprobatorio PVVO 2025

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
                    LrdOBD_CAT = "000" '-- No disponible se considera aprobatorio PVVO 2025

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
                    LrdOBD_CCC = "100" '-- No disponible se considera aprobatorio PVVO 2025
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
                    LrdOBD_EVS = "000" '-- No disponible se considera aprobatorio PVVO 2025
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
                    LrdOBD_SAS = "000" '-- No disponible se considera aprobatorio PVVO 2025
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
                    LrdOBD_FAA = "000" '-- No disponible se considera aprobatorio PVVO 2025
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
                    LrdOBD_O2C = "000" '-- No disponible se considera aprobatorio PVVO 2025
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
            output = Format(Val(output), "00000000")
            Return output
        End If

    End Function



    Private Sub DECODE_DTC(ByVal pBufferDAT As String)

        '0343 | = ejemplo de salida 1
        '0343 | 0401 8301 9320 0420 0500 0000 = ejemplo de salida 1
        '0343040183019320042005000000 = ejemplo de salida 1

        '03#4300#008#0:430301830193#1:20150000000000##

        'pBufferDAT = "034303400016030043045204490000" '--"034301280300030143030200000000"
        'pBufferDAT = "03430000C0:4305018301931:03042004200500"

        LrdOBD_STATUS_Cadd = zipESP(pBufferDAT) 'lfncLimpCadd(pBufferDAT)
        Dim lPidx As String = Nothing
        Dim Ixd As Integer = 0
        Dim zBufferDAT As Integer = Len(LrdOBD_STATUS_Cadd)
        Applog("DECODE_DTC: " & pBufferDAT & " | " & LrdOBD_STATUS_Cadd)

        Select Case Mid(LrdOBD_STATUS_Cadd, 1, 2)
            Case "03"

                Ixd = Strings.InStr(LrdOBD_STATUS_Cadd, "43") '-- Se localiza lectura del ECU
                Dim Isz As Integer = Len(LrdOBD_STATUS_Cadd)
                Dim xDato As String = Mid(LrdOBD_STATUS_Cadd, Ixd + 2, Isz) '-- Tomamos bus restante

            Case "07"
                Ixd = Strings.InStr(LrdOBD_STATUS_Cadd, "47") '-- Se localiza lectura del ECU
                Dim Isz As Integer = Len(LrdOBD_STATUS_Cadd)
                Dim xDato As String = Mid(LrdOBD_STATUS_Cadd, Ixd + 2, Isz) '-- Tomamos bus restante

        End Select

        OBDdata_DTCtxt = ""

        Try

            Dim Ixa As Integer = 0

            Select Case Mid(LrdOBD_STATUS_Cadd, 1, 2)

                Case "03"

                    Ixa = Strings.InStr(LrdOBD_STATUS_Cadd, "43") '-- Buscamos ":43##"
                    If Ixa > 0 Then

                        Ixa += 4 '-- ":43##?"
                        Do
                            lPidx = Mid(LrdOBD_STATUS_Cadd, Ixa, 4)
                            OBDdata_DTCtxt &= "P" & lPidx & ", "
                            Call DECODE_DTC_CODIGOS(lPidx)
                            Ixa += 4
                            If Len(LrdOBD_STATUS_Cadd) <= Ixa Then Exit Do
                        Loop

                    End If

                Case "07"

                    Ixa = Strings.InStr(LrdOBD_STATUS_Cadd, "47") '-- Buscamos ":47##"
                    If Ixa > 0 Then

                        Ixa += 4 '-- ":47##?"
                        Do
                            lPidx = Mid(LrdOBD_STATUS_Cadd, Ixa, 4)
                            OBDdata_DTCtxt &= "P" & lPidx & ", "
                            Call DECODE_DTC_CODIGOS(lPidx)
                            Ixa += 4
                            If Len(LrdOBD_STATUS_Cadd) <= Ixa Then Exit Do
                        Loop

                    End If

            End Select

        Catch ex As Exception
            lrdError = pBufferDAT & "|" & ex.Message
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

    Public Function zipESP(ByVal pTexto As String) As String
        Dim lTexto As String = Nothing
        Dim lCar As String = Nothing
        Dim Ix0 As Integer = 0

        For Ix0 = 1 To Len(pTexto)
            lCar = Mid(pTexto, Ix0, 1)

            If lCar <> " " Then
                lTexto &= lCar
            End If

        Next

        Applog("zipESP: " & pTexto & " | " & lTexto)

        Return lTexto

    End Function



End Module
