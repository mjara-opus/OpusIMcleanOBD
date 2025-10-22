Imports System.Xml

Module ModTOOLS

    Public Function ByteArrayToHexString(ByVal InArray As Byte(),
                                          Optional ByVal InStart As Integer = 0,
                                          Optional ByVal strSeperator As String = " ") As String
        Applog("... ByteArrayToHexString")

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
        Return (sReturn)
    End Function

    Public Function DT_DataToString(ByVal InData As DrewTech.IIMClean.DT_IECUData) As String
        Applog("... DT_DataToString")

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
        Return (sReturn)
    End Function


    Public Function GetObjectArrayAsString(ByRef objInput As Object()) As String
        Applog("... GetObjectArrayAsString")

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
        Return (sReturn)
    End Function



    Public Function ByteArrayToString(ByVal InArray As Byte(),
                                       ByVal InStart As Integer,
                                       ByVal strNonPrint As String,
                                       Optional ByVal bIgnoreNull As Boolean = False,
                                       Optional ByRef iPrintableLength As Integer = Nothing) As String
        Applog("... ByteArrayToString")

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
            Debug.WriteLine("ex:'" & ex.Message & "'")
        End Try
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

        Applog("... CountBits: " & iReturn)
        Return (iReturn)
    End Function


    Public Sub WrXML(ByVal pProfile As String, ByVal pCampo As String, ByVal pWCampo As String)
        Dim xI0 As Integer = 0
        Do
            Try

                Dim xmlDoc As Xml.XmlDocument = New XmlDocument()
                xmlDoc.Load(pProfile)
                xmlDoc.SelectSingleNode(pCampo).InnerText = pWCampo
                xmlDoc.Save(pProfile)
                Exit Do

            Catch ex As Exception
                xI0 += 1
                If xI0 > 5 Then
                    Applog("Err:WrXml(" & pProfile & ")[" & pCampo & "].. " & pWCampo & " | " & ex.Message)
                    Exit Do
                End If
                System.Threading.Thread.Sleep(50)
            End Try
        Loop

    End Sub


    Public Function LogBitArray(ByVal iLogLevel As Integer, ByVal sTag As String, ByVal sLogLead As String,
                                 ByVal bInArray As BitArray, ByVal iStartIdx As Integer,
                                 ByVal iStartIdxLog As Integer) As Integer
        Applog("... LogBitArray")

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
        Return (iReturn)
    End Function

    Function GetCallingProcedure(Optional ByVal CallDepth As Integer = 2,
                                     Optional ByVal NameOnly As Boolean = False) As String
        Applog("... GetCallingProcedure")

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
            Applog("Err:GetCallingProcedure | " & ex.Message)
        End Try
        StackFrame = Nothing
        StackTrace = Nothing
        Return (sReturn)
    End Function



    Public Function CReplace(ByVal original As String,
                              ByVal pattern As String,
                              ByVal replacement As String,
                              Optional ByVal comparisonType As System.StringComparison = StringComparison.OrdinalIgnoreCase,
                              Optional ByVal stringBuilderInitialSize As Integer = -1) As String
        'Applog("... CReplace")

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
        Return (sReturn)
    End Function


    Public Function GetPropertyValueAsString(ByRef info As System.Management.ManagementObject,
                                              ByVal ValueName As String) As String
        'Applog("... GetPropertyValueAsString")

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


    Public Function StringArrayToString(ByVal InArray As String()) As String
        'Applog("... StringArrayToString")

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


    Public Function FunctionSeconds(ByVal StartTicks As Int32) As String
        'Applog("... FunctionSeconds")

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



    Public Function GetDAD(Optional ByVal sLocation As String = "") As Boolean
        'Applog("... GetDAD")

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

        Applog("... GetDAD | " & sMessage & " | Cnt:" & lCount.ToString("00000") & "," & MyDADAccessCount.ToString("00") &
                                  " Opn:" & Microsoft.VisualBasic.Left(MyDadOpen.ToString, 1) &
                                  " (" & sLocation & ", " & MyDADLastReleaseLocation & ")")

        GetDAD = bReturn
    End Function


    Public Function MyDADStatus() As String
        'Applog("... MyDADStatus")

        Dim RetValue As String = "DAD:Nothing"
        Try
            If (IsNothing(MyDad) = False) Then
                RetValue = "LCR:" & MyLastCommResult.ToString & "," & MyLastExtendedStatus.ToString
            End If
        Catch ex As Exception

        End Try

        Applog("... MyDADStatus: " & RetValue)

        Return (RetValue)

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


    Public Function ValidateFirmwareVersion(ByVal sVersion As String) As Boolean
        'Applog("... ValidateFirmwareVersion")

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


    Public Function GetMyLogLevel() As Integer
        'Applog("... GetMyLogLevel")
        GetMyLogLevel = MyLogLevel
    End Function

    Public Function RdXML(ByVal pProfile As String, ByVal pCampo As String) As String
        Dim xI0 As Integer = 0
        Dim lDato As String = ""

        Try

            Dim xmlDoc As Xml.XmlDocument = New XmlDocument()
            xmlDoc.Load(pProfile)
            lDato = xmlDoc.SelectSingleNode(pCampo).InnerText

        Catch ex As Exception
            Applog("Err:RdXml(" & pProfile & ")[" & pCampo & "] " & ex.Message)
        End Try

        Return lDato

    End Function


    Public Sub Applog(ByVal lParametro As String)

        Try
            Dim lDato As String

            lDato = Format(Now, "hh:mm:ss| ") & lParametro

            Form1.ListBox1.Items.Add(lDato)

            Dim sw As New System.IO.StreamWriter(logFile, True)
            sw.WriteLine(lDato)
            sw.Close()

        Catch ex As Exception
            'MsgBox("AppLog:" + ex.Message)
        End Try

    End Sub

End Module
