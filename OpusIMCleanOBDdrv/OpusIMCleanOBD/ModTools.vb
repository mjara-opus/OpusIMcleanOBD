Imports System.Net.WebRequestMethods
Imports MySql.Data.MySqlClient

Module ModTools

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


    Public Sub InitStringArray(ByRef aString(,) As String)
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
            Applog("Err:InitStringArray | " & ex.Message)
        End Try
    End Sub

    Public Function lCadTxtToCadHex(ByVal pCadena As String, Optional ByVal pFormato As Integer = 0) As String

        '-- pFormato = 0 => A1 B2 C3 E4 F5
        '-- pFormato = 1 => A1B2C3E4F5
        '-- pFormato = 2 => F5 E4 C3 B2 A1
        '-- pFormato = 3 => F5E4C3B2A1

        Dim xCar As String
        Dim xSize As Integer = Len(pCadena)
        Dim Ix0 As Integer = 0
        Dim xHexCar As String = "--"
        Dim xHexCadena As String = ""

        For Ix0 = 1 To xSize
            xCar = Mid(pCadena, Ix0, 1)

            Select Case xCar
                Case "A" : xHexCar = "41"
                Case "B" : xHexCar = "42"
                Case "C" : xHexCar = "43"
                Case "D" : xHexCar = "44"
                Case "E" : xHexCar = "45"
                Case "F" : xHexCar = "46"
                Case "G" : xHexCar = "47"
                Case "H" : xHexCar = "48"
                Case "I" : xHexCar = "49"
                Case "J" : xHexCar = "4a"
                Case "K" : xHexCar = "4b"
                Case "L" : xHexCar = "4c"
                Case "M" : xHexCar = "4d"
                Case "N" : xHexCar = "4e"
                Case "O" : xHexCar = "4f"
                Case "P" : xHexCar = "50"
                Case "Q" : xHexCar = "51"
                Case "R" : xHexCar = "52"
                Case "S" : xHexCar = "53"
                Case "T" : xHexCar = "54"
                Case "U" : xHexCar = "55"
                Case "V" : xHexCar = "56"
                Case "W" : xHexCar = "57"
                Case "X" : xHexCar = "58"
                Case "Y" : xHexCar = "59"
                Case "Z" : xHexCar = "5a"

                Case "0" : xHexCar = "30"
                Case "1" : xHexCar = "31"
                Case "2" : xHexCar = "32"
                Case "3" : xHexCar = "33"
                Case "4" : xHexCar = "34"
                Case "5" : xHexCar = "35"
                Case "6" : xHexCar = "36"
                Case "7" : xHexCar = "37"
                Case "8" : xHexCar = "38"
                Case "9" : xHexCar = "39"

                Case Else : xHexCar = "--"

            End Select

            Select Case pFormato
                Case 0
                    xHexCadena &= " " & xHexCar
                Case 1
                    xHexCadena &= xHexCar
                Case 2
                    xHexCadena = xHexCar & " " & xHexCadena
                Case 3
                    xHexCadena = xHexCar & xHexCadena
            End Select

        Next

        Return Trim(xHexCadena)

    End Function


    Public Function getMacAddress(ByVal pIDmac As Integer) '-- pIDmac = 0, primera tarjeta de red por default 
        Try
            Dim nics() As Net.NetworkInformation.NetworkInterface = Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
            Return nics(pIDmac).GetPhysicalAddress.ToString
            Exit Function

        Catch ex As Exception
            Return "NULL"
        End Try

    End Function

    Public Function CryptoData(ByVal pDato As String, ByVal pCmd As Integer) As String
        '-- v.24.04.25
        '-- pDato = String a encriptar / pCmd (0= Desencripta, 1= encripta, 2=muestra encripta/desencripta

        Dim Ix0 As Integer = 0
        Dim lNewChar As String = ""
        Dim lIdCvv As Integer = 25 'Val(Mid(Format(xIdCentro, "0000"), 3, 2))
        Dim lDato As String = Trim(pDato)
        Dim lnewDato As String = ""
        Dim lnewDato1 As String = ""
        Dim lnewDato0 As String = ""

        Dim ziseDato As Integer = Len(lDato)

        Select Case pCmd
            Case 0 '-- Decodifica
                For Ix0 = 1 To ziseDato Step 3
                    lNewChar = Chr(Val(Mid(pDato, Ix0, 3)) - lIdCvv)
                    lnewDato0 &= lNewChar

                Next
                lnewDato = lnewDato0

            Case 1 '-- Codifica
                For Ix0 = 1 To ziseDato
                    lNewChar = Format(Asc(Mid(pDato, Ix0, 1)) + lIdCvv, "000")
                    lnewDato1 &= lNewChar

                Next
                lnewDato = lnewDato1

            Case 2 '-- '-- Codifica / Decodifica
                For Ix0 = 1 To ziseDato
                    lNewChar = Format(Asc(Mid(pDato, Ix0, 1)) + lIdCvv, "000")
                    lnewDato1 &= lNewChar

                Next

                ziseDato = Len(lnewDato1)

                For Ix0 = 1 To ziseDato Step 3
                    lNewChar = Chr(Val(Mid(lnewDato1, Ix0, 3)) - lIdCvv)
                    lnewDato0 &= lNewChar

                Next
                lnewDato = "CryptoDataASCII: " & pDato & " |z: " & ziseDato & " |c: " & lnewDato1 & " |d: " & lnewDato0

        End Select

        Return lnewDato

    End Function


    Public Sub Applog(ByVal lParametro As String)

        Dim lMes As String = Format(Now, "ddMMyyyy")
        Dim logFile As String = "OpusIMCleanOBD_" & lMes & ".log"

        Try
            Dim lDato As String

            lDato = Format(Now, "hh:mm:ss| ") & lParametro

            Dim sw As New System.IO.StreamWriter(logFile, True)
            sw.WriteLine(lDato)
            sw.Close()

        Catch ex As Exception
            'MsgBox("AppLog:" + ex.Message)
        End Try

    End Sub

    Public Function lConectaSQL() As String

        Dim lStatus As String = Nothing

        Try

            conn.ConnectionString = xVIDB_ConnectionString
            conn.Open()
            lStatus = "Pass: Servicios NET en línea."
            xVIDBLinkOnline = True

        Catch ex As Exception
            xVIDBLinkOnline = False
            lStatus = "Err:ConectaSQL | " & ex.Message
            Applog(lStatus)

        End Try

        Return lStatus

    End Function

    Public Function Set_DataMySQL() As String

        Dim lStatus As String = Nothing

        Try

            Dim str_CADSql As String = "insert into InspectionData 
                                (IdInspectionData, 
                                IdStation,
                                FECHA,
                                DeviceID,
                                VINhx,
                                MILhx,
                                DTChx,
                                VIN,
                                MIL,
                                MSI,
                                CCM,
                                CMB,
                                O2S,
                                CAT,
                                CCC,
                                EVS,
                                SAS,
                                FAA,
                                O2C,
                                DTC)
                              values
                                (@IdInspectionData, 
                                @IdStation,
                                @FECHA,
                                @DeviceID,
                                @VINhx,
                                @MILhx,
                                @DTChx,
                                @VIN,
                                @MIL,
                                @MSI,
                                @CCM,
                                @CMB,
                                @O2S,
                                @CAT,
                                @CCC,
                                @EVS,
                                @SAS,
                                @FAA,
                                @O2C,
                                @DTC)"

            Dim lIdInspectionData As Integer = getIdFolioTablaSQL("InspectionData", "IdInspectionData")

            If conn.State = 0 Then conn.Open()

            Dim sqlCommand As New MySqlCommand
            sqlCommand.Connection = conn
            sqlCommand.CommandText = str_CADSql

            sqlCommand.Parameters.AddWithValue("@IdInspectionData", lIdInspectionData)
            sqlCommand.Parameters.AddWithValue("@IdStation", xMacAddress)
            sqlCommand.Parameters.AddWithValue("@FECHA", IMCleanOBD.lInspectionData.Fecha_Test)
            sqlCommand.Parameters.AddWithValue("@DeviceID", IMCleanOBD.lDeviceData.DeviceID)
            sqlCommand.Parameters.AddWithValue("@VINhx", IMCleanOBD.lInspectionData.OBDdata_VINhx)
            sqlCommand.Parameters.AddWithValue("@MILhx", IMCleanOBD.lInspectionData.OBDdata_MILhx)
            sqlCommand.Parameters.AddWithValue("@DTChx", IMCleanOBD.lInspectionData.OBDdata_DTChx)
            sqlCommand.Parameters.AddWithValue("@VIN", IMCleanOBD.lInspectionData.OBDdata_VIN)
            sqlCommand.Parameters.AddWithValue("@MIL", IIf(IMCleanOBD.lInspectionData.OBDdata_MIL = "1", True, False))
            sqlCommand.Parameters.AddWithValue("@MSI", IMCleanOBD.lInspectionData.OBD_MSI)
            sqlCommand.Parameters.AddWithValue("@CCM", IMCleanOBD.lInspectionData.OBD_CCM)
            sqlCommand.Parameters.AddWithValue("@CMB", IMCleanOBD.lInspectionData.OBD_CMB)
            sqlCommand.Parameters.AddWithValue("@O2S", IMCleanOBD.lInspectionData.OBD_O2S)
            sqlCommand.Parameters.AddWithValue("@CAT", IMCleanOBD.lInspectionData.OBD_CAT)
            sqlCommand.Parameters.AddWithValue("@CCC", IMCleanOBD.lInspectionData.OBD_CCC)
            sqlCommand.Parameters.AddWithValue("@EVS", IMCleanOBD.lInspectionData.OBD_EVS)
            sqlCommand.Parameters.AddWithValue("@SAS", IMCleanOBD.lInspectionData.OBD_SAS)
            sqlCommand.Parameters.AddWithValue("@FAA", IMCleanOBD.lInspectionData.OBD_FAA)
            sqlCommand.Parameters.AddWithValue("@O2C", IMCleanOBD.lInspectionData.OBD_O2C)
            sqlCommand.Parameters.AddWithValue("@DTC", IMCleanOBD.lInspectionData.OBDdata_DTC)

            sqlCommand.ExecuteNonQuery()
            conn.Close()

            lStatus = "Pass:Set_DataMySQL "

        Catch ex As Exception

            lStatus = "Err:Set_DataMySQL | " & ex.Message
            Applog(lStatus)

        End Try


        Return lStatus

    End Function

    Public Function getIdFolioTablaSQL(ByVal pTablaSQL As String, ByVal pCampoTablaSQL As String) As Long

        Dim lIdFolio As Long = 0
        Dim resultado As MySqlDataReader
        Dim sqlCommand As New MySqlCommand

        Dim str_CADSql = " SELECT max(" & pCampoTablaSQL & ") FROM " & pTablaSQL

        Try

            If conn.State = 0 Then conn.Open()

            sqlCommand.Connection = conn
            sqlCommand.CommandText = str_CADSql

            resultado = sqlCommand.ExecuteReader

            If resultado.Read Then

                If resultado.IsDBNull(0) Then
                    lIdFolio = 1
                Else
                    lIdFolio = resultado(0) + 1
                End If
                conn.Close()

            Else

                lIdFolio = 999999
                conn.Close()

            End If

        Catch ex As Exception
            Applog("Err:getIdFolioTablaSQL|" & ex.Message)
            lIdFolio = 999999

        End Try

        Return lIdFolio

    End Function


End Module
