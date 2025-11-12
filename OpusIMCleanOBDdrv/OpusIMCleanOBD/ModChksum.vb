Imports System

Module ModChksum

    Public Function chkSumCADD(Optional ByVal pCadd As String = "000") As String
        '-------------------------------------------------------------------------------
        '-- MJA
        '-- "0c26.23" 31.10.23 
        '-- Funcion para calcular la clave  PU
        '-------------------------------------------------------------------------------
        Dim linCADD As String = Nothing
        Dim lchkSumCADD As String = Nothing

        Dim dt(9) As String
        Dim fxA(9) As Int64

        Try

            Dim i As Integer

            Dim lFecha As String = Nothing

            If pCadd = "000" Then
                lFecha = Format(Now, "ddMMyy") '-- Chksum nuevo
            Else
                '-- Cadena esperada = xxxxxxxxxxxxyyddmm
                If Len(pCadd) > 3 Then
                    lFecha = DecodeDD(Mid(pCadd, 15, 2)) & DecodeDD(Mid(pCadd, 17, 2)) & DecodeDD(Mid(pCadd, 13, 2))  '-- Extraemos y decodificamos Fecha yyddmm -> ddmmyy

                End If

            End If

            Dim lEquipo As String = getMacAddress(0)

            Dim zEq As Integer = Len(lEquipo)
            If zEq > 6 Then
                lEquipo = Mid$(lEquipo, (zEq - 5), 6)
            End If

            Dim lDevice As String = strDeviceInfo.DeviceID

            If Len(lDevice) > 6 Then
                lDevice = Mid$(lDevice, (Len(lDevice) - 5), 6)
            End If

            linCADD = lEquipo & lDevice
            Dim zCADD As Integer = Len(linCADD)

            ReDim DT(zCADD)

            fxA(1) = 726528 + Val(Mid(lFecha, 1, 4))
            fxA(2) = 118088 + Val(Mid(lFecha, 1, 4))
            fxA(3) = 726528 + Val(Mid(lFecha, 1, 4))
            fxA(4) = 156738 + Val(Mid(lFecha, 1, 4))
            fxA(5) = 123538 + Val(Mid(lFecha, 1, 4))
            fxA(6) = 110000 + Val(Mid(lFecha, 1, 4))
            fxA(7) = 201882 + Val(Mid(lFecha, 1, 4))
            fxA(8) = 233211 + Val(Mid(lFecha, 1, 4))
            fxA(9) = 761422 + Val(Mid(lFecha, 1, 4))

            Dim i0 As Integer = 0
            Dim cc As Int64 = 0

            For i = 1 To zCADD
                DT(i) = Mid(linCADD, i, 1)

                For i0 = 1 To 9
                    cc = cc + (Asc(DT(i)) * fxA(i0))
                Next

                lchkSumCADD &= Chr((cc - (26 * Int(cc / 26))) + 65)
                Select Case i
                    Case 4, 8, 12, 16, 20, 24, 28, 32, 36, 40
                        lchkSumCADD &= " "
                End Select

            Next

            Dim dd As Int64 = 0
            Dim lCadFH As String = Nothing

            If pCadd = "000" Then

                lCadFH = CodeDD(Val(Mid(lFecha, 5, 2)) + 1) & CodeDD(Mid(lFecha, 1, 2)) & CodeDD(Mid(lFecha, 3, 2)) '-- yyddmm | yy + 1
                linCADD = lchkSumCADD & lCadFH

            Else

                lCadFH = CodeDD(Mid(lFecha, 5, 2)) & CodeDD(Mid(lFecha, 1, 2)) & CodeDD(Mid(lFecha, 3, 2)) '-- yyddmm

                If Mid(pCadd, 1, 12) = zipESP(lchkSumCADD) Then '-- Validamos Chksum

                    Dim laFecha As Date = CDate(DecodeDD(Mid(lCadFH, 3, 2)) & "/" & DecodeDD(Mid(lCadFH, 5, 2)) & "/20" & DecodeDD(Mid(lCadFH, 1, 2))) '--> dd/mm/yyyy
                    Dim lHoy As Date = CDate(Format(Now, "dd/MM/yyyy"))

                    If lHoy <= laFecha Then '-- Validamos vigencia
                        linCADD = "Pass:Opus IMClean OBD Licencia valida."
                    Else
                        linCADD = "Err:Opus IMClean OBD licencia expirada."
                    End If

                Else

                    linCADD = "Err:Opus IMClean OBD licencia no valida."

                End If

            End If

        Catch ex As Exception

            linCADD = "Err:chkSumCADD | " & ex.Message
            Applog(linCADD)
        End Try

        'MsgBox("chkSumCADD | " & pCadd & " | " & linCADD)
        Return linCADD

    End Function



    Private Function CodeDD(ByVal pDtato As String) As String

        Dim nDato As String

        Select Case Val(pDtato)
            Case 1 : nDato = "QW"
            Case 2 : nDato = "ER"
            Case 3 : nDato = "TY"
            Case 4 : nDato = "UI"
            Case 5 : nDato = "OP"
            Case 6 : nDato = "AS"
            Case 7 : nDato = "DF"
            Case 8 : nDato = "GH"
            Case 9 : nDato = "JK"
            Case 10 : nDato = "LZ"
            Case 11 : nDato = "XC"
            Case 12 : nDato = "VB"
            Case 13 : nDato = "NM"
            Case 14 : nDato = "ZA"
            Case 15 : nDato = "XS"
            Case 16 : nDato = "CD"
            Case 17 : nDato = "VF"
            Case 18 : nDato = "BG"
            Case 19 : nDato = "NH"
            Case 20 : nDato = "MJ"
            Case 21 : nDato = "AQ"
            Case 22 : nDato = "DE"
            Case 23 : nDato = "FR"
            Case 24 : nDato = "GT"
            Case 25 : nDato = "HY"
            Case 26 : nDato = "JU"
            Case 27 : nDato = "KI"
            Case 28 : nDato = "LO"
            Case 29 : nDato = "VE"
            Case 30 : nDato = "NT"
            Case 31 : nDato = "MY"
            Case 32 : nDato = "QE"
            Case 33 : nDato = "WR"
            Case 34 : nDato = "ET"
            Case 35 : nDato = "RY"
            Case 36 : nDato = "TU"
            Case 37 : nDato = "YI"
            Case 38 : nDato = "UO"
            Case 39 : nDato = "IP"
            Case 40 : nDato = "OA"
            Case 41 : nDato = "SF"
            Case 42 : nDato = "DG"
            Case 43 : nDato = "FH"
            Case 44 : nDato = "GJ"
            Case 45 : nDato = "HK"
            Case 46 : nDato = "JL"
            Case 47 : nDato = "KX"
            Case 48 : nDato = "ZC"
            Case 49 : nDato = "VN"
            Case 50 : nDato = "BM"
            Case Else
                nDato = "ZZ"
        End Select

        Return nDato

    End Function

    Private Function DecodeDD(ByVal pDtato As String) As String

        Dim nDato As String

        Select Case pDtato
            Case "QW" : nDato = "01"
            Case "ER" : nDato = "02"
            Case "TY" : nDato = "03"
            Case "UI" : nDato = "04"
            Case "OP" : nDato = "05"
            Case "AS" : nDato = "06"
            Case "DF" : nDato = "07"
            Case "GH" : nDato = "08"
            Case "JK" : nDato = "09"
            Case "LZ" : nDato = "10"
            Case "XC" : nDato = "11"
            Case "VB" : nDato = "12"
            Case "NM" : nDato = "13"
            Case "ZA" : nDato = "14"
            Case "XS" : nDato = "15"
            Case "CD" : nDato = "16"
            Case "VF" : nDato = "17"
            Case "BG" : nDato = "18"
            Case "NH" : nDato = "19"
            Case "MJ" : nDato = "20"
            Case "AQ" : nDato = "21"
            Case "DE" : nDato = "22"
            Case "FR" : nDato = "23"
            Case "GT" : nDato = "24"
            Case "HY" : nDato = "25"
            Case "JU" : nDato = "26"
            Case "KI" : nDato = "27"
            Case "LO" : nDato = "28"
            Case "VE" : nDato = "29"
            Case "NT" : nDato = "30"
            Case "MY" : nDato = "31"
            Case "QE" : nDato = "32"
            Case "WR" : nDato = "33"
            Case "ET" : nDato = "34"
            Case "RY" : nDato = "35"
            Case "TU" : nDato = "36"
            Case "YI" : nDato = "37"
            Case "UO" : nDato = "38"
            Case "IP" : nDato = "39"
            Case "OA" : nDato = "40"
            Case "SF" : nDato = "41"
            Case "DG" : nDato = "42"
            Case "FH" : nDato = "43"
            Case "GJ" : nDato = "44"
            Case "HK" : nDato = "45"
            Case "JL" : nDato = "46"
            Case "KX" : nDato = "47"
            Case "ZC" : nDato = "48"
            Case "VN" : nDato = "49"
            Case "BM" : nDato = "50"
            Case Else
                nDato = "99"
        End Select

        Return nDato

    End Function



    Public Function getOpusKeyFile() As String

        Dim lStatus As String = Nothing

        Dim Apunt As Integer = 0
        Dim Ftx As String = Nothing

        Try

            If System.IO.File.Exists(xLocalKEYfile) Then

                Apunt = FileSystem.FreeFile()
                FileSystem.FileOpen(Apunt, xLocalKEYfile, OpenMode.Input, OpenAccess.Read)
                Do While Not EOF(Apunt)
                    lStatus = FileSystem.LineInput(Apunt)

                Loop
                FileSystem.FileClose(Apunt)

                lStatus = CryptoData(lStatus, 0) '-- Desencriptamos chksum
                lStatus = chkSumCADD(lStatus) '-- Validamos chksum

            Else
                lStatus = "Err:Opus IMClean OBD licencia no registrada."
            End If

        Catch ex As Exception
            lStatus = "Err:getOpusKeyFile | " & ex.Message
        End Try

        Return lStatus

    End Function

    Public Function lset_OpusKeyDevice(ByVal pOpusKey As String)

        Dim lStatus As String = Nothing
        Dim lOpusKey As String = Nothing

        Try

            lOpusKey = zipESP(pOpusKey)
            If Len(lOpusKey) > 18 Then lOpusKey = Mid(lOpusKey, 1, 18)
            'MsgBox(lOpusKey)
            'MsgBox("set_OpusKeyDevice: " & chkSumCADD(lOpusKey))

            If Mid(chkSumCADD(lOpusKey), 1, 4) = "Pass" Then

                Dim lDatoKey As String = CryptoData(lOpusKey, 1)

                If System.IO.File.Exists(xLocalKEYfile) Then System.IO.File.Delete(xLocalKEYfile)

                Dim sw As New System.IO.StreamWriter(xLocalKEYfile, True)
                sw.WriteLine(lDatoKey)
                sw.Close()

                lStatus = "Pass:Opus IMClean OBD registrado exitosamente."

            Else

                lStatus = "Err:Opus IMClean OBD licencia no valida o expirada."

            End If

        Catch ex As Exception
            lStatus = "Err:set_OpusKeyDevice | " & ex.Message

        End Try

        Return lStatus

    End Function

    Public Function calcCRC(ByVal pCampo As String) As String

        pCampo = Trim(pCampo)

        Dim nTXTASC As Int32
        'facNS = calcCRCNS(txtIMCleanNs.Text)
        'Dim facFH As Int32 = calcCRCNS(Trim(txtFecha.Text))
        Dim zTXT As Integer = Len(pCampo)
        Dim nI0, nI1, nTXTASCTot As Integer
        Dim xTXTASCTot As String = Nothing
        Dim xTXTASCSal As String = Nothing

        Try

            nTXTASCTot = 0
            If zTXT > 0 Then
                nI1 = 0
                For nI0 = 1 To zTXT
                    nI1 += 1
                    nTXTASC = Asc(Mid(pCampo, nI0, 1)) * ((zTXT + 1) - nI1)
                    nTXTASCTot += nTXTASC
                    If nI1 = 5 Then nI1 = 0

                Next

            End If

            nTXTASCTot = nTXTASCTot + facNS + facFH '-- agregamos el factor del numero de serie el instrumneto

            xTXTASCTot = Format(nTXTASCTot, "000000")

            xTXTASCSal = Nothing
            zTXT = Len(xTXTASCTot)
            For nI0 = 1 To zTXT
                Select Case Mid(xTXTASCTot, nI0, 1)
                    Case "0" : xTXTASCSal &= "Z"
                    Case "1" : xTXTASCSal &= "S"
                    Case "2" : xTXTASCSal &= "F"
                    Case "3" : xTXTASCSal &= "X"
                    Case "4" : xTXTASCSal &= "C"
                    Case "5" : xTXTASCSal &= "T"
                    Case "6" : xTXTASCSal &= "Y"
                    Case "7" : xTXTASCSal &= "J"
                    Case "8" : xTXTASCSal &= "M"
                    Case "9" : xTXTASCSal &= "K"

                End Select
            Next

            If Len(xTXTASCSal) > 6 Then xTXTASCSal = Mid(xTXTASCSal, 1, 6) '-- limitamos la cadena a 6 caracteres

        Catch ex As Exception
            Applog("Err:calcCRC | " & ex.Message)
        End Try

        Return xTXTASCSal '-- xTXTASCTot & " | " & xTXTASCSal

    End Function

    Public Function calcCRCNS(ByVal pCampo As String) As Int32

        pCampo = Trim(pCampo)

        Dim nTXTASC As Integer
        Dim zTXT As Integer = Len(pCampo)
        Dim nI0, nI1, nTXTASCTot As Int32

        Try
            nTXTASCTot = 0
            If zTXT > 0 Then

                nI1 = 0
                For nI0 = 1 To zTXT
                    nI1 += 1
                    nTXTASC = Asc(Mid(pCampo, nI0, 1)) * ((zTXT + 1) - nI1)
                    nTXTASCTot += nTXTASC
                    If nI1 = 5 Then nI1 = 0

                Next

            End If

        Catch ex As Exception
            Applog("Err:calcCRCNS | " & ex.Message)
        End Try

        Return nTXTASCTot

    End Function



End Module
