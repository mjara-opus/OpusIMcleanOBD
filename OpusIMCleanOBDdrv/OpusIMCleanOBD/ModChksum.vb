Imports System

Module ModChksum

    Public Function chkSumCADD(Optional ByVal pCadd As String = "000") As String
        '-------------------------------------------------------------------------------
        '-- MJA
        '-- "0c26.23" 31.10.23 
        '-- Funcion para calcular la clave  PU
        '-------------------------------------------------------------------------------
        Dim lStatus As String = Nothing
        Dim lCadFH As String = Nothing
        Dim lchkSumCADD As String = Nothing

        'MsgBox("chkSumCADD(" & pCadd & ")")

        Try

            Dim lFecha As String = Nothing

            If Len(pCadd) > 3 Then '-- Cadena esperada = xxxxxxxxxxxxxyyddmm
                lFecha = DecodeDD(Mid(pCadd, 16, 2)) & DecodeDD(Mid(pCadd, 18, 2)) & DecodeDD(Mid(pCadd, 14, 2))  '-- Extraemos y decodificamos Fecha yyddmm -> ddmmyy
            Else
                lFecha = Format(Now, "ddMMyy") '-- Chksum nuevo
            End If
            lCadFH = CodeDD(Mid(lFecha, 5, 2)) & CodeDD(Mid(lFecha, 1, 2)) & CodeDD(Mid(lFecha, 3, 2)) '-- yyddmm 

            Dim lDevice As String = strDeviceInfo.DeviceID

            lchkSumCADD = fncCHKSUM_Station(lFecha, lDevice) & lCadFH '--creamos licnecia pFecha: ddmmyy, pDevice: 999999, MacAdd: AAAAAAAAAAAA | 24    

            'MsgBox("chkSumCADD: " & lchkSumCADD)

            If pCadd = "000" Then

                lchkSumCADD = CryptoData(lchkSumCADD, 1) '-- Encriptamos licencia

                If System.IO.File.Exists(xLocalKEYfile) Then System.IO.File.Delete(xLocalKEYfile)

                Dim sw As New System.IO.StreamWriter(xLocalKEYfile, True)
                sw.WriteLine(lchkSumCADD)
                sw.Close()

                lStatus = "Pass:Opus IMClean OBD registrado exitosamente."

            Else

                If Mid(pCadd, 1, 13) = Mid(lchkSumCADD, 1, 13) Then '-- Validamos Chksum | zipESP(lchkSumCADD)

                    Dim lFechaVence As Date = CDate(Mid(lFecha, 1, 2) & "/" & Mid(lFecha, 3, 2) & "/20" & Val(Mid(lFecha, 5, 2)) + 1) '--> dd/mm/yyyy
                    Dim lHoy As Date = CDate(Format(Now, "dd/MM/yyyy"))

                    If lHoy <= lFechaVence Then '-- vigencia valida
                        lStatus = "Pass:Opus IMClean OBD Licencia valida."
                    Else
                        lStatus = "Err:Opus IMClean OBD licencia expirada."
                    End If

                Else

                    lStatus = "Err:Opus IMClean OBD licencia no valida."

                End If

            End If

        Catch ex As Exception

            lStatus = "Err:chkSumCADD | " & ex.Message

        End Try

        'Applog(lStatus)
        'MsgBox("chkSumCADD | " & pCadd & " | " & lStatus)
        Return lStatus

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

        Applog("getOpusKeyFile: " & lStatus)
        Return lStatus

    End Function

    Public Function lset_OpusKeyDevice(ByVal pOpusKey As String)

        Dim lStatus As String = Nothing
        Dim lOpusKey As String = Nothing

        Try

            lOpusKey = zipESP(pOpusKey)

            'If Len(lOpusKey) > 18 Then lOpusKey = Mid(lOpusKey, 1, 18)

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

    Public Function fncCHKSUM_Station(ByVal pFecha As String, ByVal pDevice As String) As String
        '-- Genera CHKSUM de la Microbanca usando el numero de serie del EePromNS
        '-- Data In: pFecha: 010190, pDevice: IEXFM0000207393E, MacAdd: FC5CEE1157CC 

        Dim xStatus As String = ""
        Dim dt(0 To 24)
        Dim a(0 To 24), b(0 To 24), c(0 To 24), d(0 To 24), e(0 To 24), f(0 To 24), g(0 To 24), h(0 To 24), j(0 To 24), k(0 To 24), l(0 To 24), m(0 To 24), n(0 To 24) As Integer
        Dim i As Integer
        Dim cc1, cc2, cc3, cc4, cc5, cc6, cc7, cc8, cc9, cc10, cc11, cc12, cc13
        Dim Tcc1, Tcc2, Tcc3, Tcc4, Tcc5, Tcc6, Tcc7, Tcc8, Tcc9, Tcc10, Tcc11, Tcc12, Tcc13
        Dim xCadena As String

        Dim pMacAdd As String = getMacAddress(0)

        If Len(pDevice) >= 6 Then
            pDevice = Mid(pDevice, Len(pDevice) - 5, 6) '-- Se toman los 6 ultimos digitos del Numero de serie del Instrumento
        Else
            pDevice = Mid(pDevice & "654321", 1, 6) '-- En caso de que Numero de serie sea menos a 6 digitos se completan.
        End If

        'MsgBox("Fecha: " & pFecha)
        'MsgBox("Device: " & pDevice)
        'MsgBox("MacAdd: " & pMacAdd)

        xCadena = pFecha & pDevice & pMacAdd '-- pFecha: ddmmyy, pDevice: 999999, MacAdd: AAAAAAAAAAAA | 24

        'MsgBox("0..Chksum: " & xCadena)

        '---------------
        a(1) = 784
        a(2) = 929
        a(3) = 501
        a(4) = 417
        a(5) = 610
        a(6) = 399
        a(7) = 375
        a(8) = 1076
        a(9) = 1123
        a(10) = 872
        a(11) = 350
        a(12) = 297
        a(13) = 1148
        a(14) = 812
        a(15) = 742
        a(16) = 788
        a(17) = 1083
        a(18) = 1148
        a(19) = 812
        a(20) = 742
        a(21) = 788
        a(22) = 1083
        a(23) = 501
        a(24) = 417

        b(1) = 1136
        b(2) = 660
        b(3) = 668
        b(4) = 741
        b(5) = 422
        b(6) = 1103
        b(7) = 596
        b(8) = 516
        b(9) = 628
        b(10) = 1128
        b(11) = 904
        b(12) = 757
        b(13) = 767
        b(14) = 427
        b(15) = 1035
        b(16) = 379
        b(17) = 1086
        b(18) = 767
        b(19) = 427
        b(20) = 1035
        b(21) = 379
        b(22) = 1086
        b(23) = 668
        b(24) = 741

        c(1) = 1144
        c(2) = 266
        c(3) = 848
        c(4) = 328
        c(5) = 708
        c(6) = 733
        c(7) = 1198
        c(8) = 424
        c(9) = 310
        c(10) = 467
        c(11) = 488
        c(12) = 1191
        c(13) = 305
        c(14) = 1028
        c(15) = 502
        c(16) = 414
        c(17) = 996
        c(18) = 305
        c(19) = 1028
        c(20) = 502
        c(21) = 414
        c(22) = 996
        c(23) = 848
        c(24) = 328

        d(1) = 1056
        d(2) = 516
        d(3) = 330
        d(4) = 603
        d(5) = 322
        d(6) = 515
        d(7) = 1050
        d(8) = 656
        d(9) = 960
        d(10) = 568
        d(11) = 376
        d(12) = 377
        d(13) = 754
        d(14) = 463
        d(15) = 1061
        d(16) = 540
        d(17) = 1041
        d(18) = 754
        d(19) = 463
        d(20) = 1061
        d(21) = 540
        d(22) = 1041
        d(23) = 330
        d(24) = 603

        e(1) = 616
        e(2) = 897
        e(3) = 406
        e(4) = 504
        e(5) = 1193
        e(6) = 475
        e(7) = 1080
        e(8) = 513
        e(9) = 457
        e(10) = 967
        e(11) = 906
        e(12) = 459
        e(13) = 554
        e(14) = 1127
        e(15) = 356
        e(16) = 528
        e(17) = 1247
        e(18) = 554
        e(19) = 1127
        e(20) = 356
        e(21) = 528
        e(22) = 1247
        e(23) = 406
        e(24) = 504

        f(1) = 616
        f(2) = 897
        f(3) = 406
        f(4) = 504
        f(5) = 1193
        f(6) = 475
        f(7) = 1080
        f(8) = 513
        f(9) = 457
        f(10) = 568
        f(11) = 376
        f(12) = 377
        f(13) = 754
        f(14) = 463
        f(15) = 1061
        f(16) = 540
        f(17) = 1041
        f(18) = 754
        f(19) = 463
        f(20) = 1061
        f(21) = 540
        f(22) = 1041
        f(23) = 406
        f(24) = 504

        g(1) = 784
        g(2) = 929
        g(3) = 501
        g(4) = 417
        g(5) = 610
        g(6) = 399
        g(7) = 375
        g(8) = 1076
        g(9) = 1123
        g(10) = 1128
        g(11) = 904
        g(12) = 757
        g(13) = 767
        g(14) = 427
        g(15) = 1035
        g(16) = 379
        g(17) = 1086
        g(18) = 767
        g(19) = 427
        g(20) = 1035
        g(21) = 379
        g(22) = 1086
        g(23) = 501
        g(24) = 417

        h(1) = 1136
        h(2) = 660
        h(3) = 668
        h(4) = 741
        h(5) = 422
        h(6) = 1103
        h(7) = 596
        h(8) = 516
        h(9) = 628
        h(10) = 872
        h(11) = 350
        h(12) = 297
        h(13) = 1148
        h(14) = 812
        h(15) = 742
        h(16) = 788
        h(17) = 1083
        h(18) = 1148
        h(19) = 812
        h(20) = 742
        h(21) = 788
        h(22) = 1083
        h(23) = 668
        h(24) = 741

        j(1) = 1144
        j(2) = 266
        j(3) = 848
        j(4) = 328
        j(5) = 708
        j(6) = 733
        j(7) = 1198
        j(8) = 424
        j(9) = 310
        j(10) = 568
        j(11) = 376
        j(12) = 377
        j(13) = 754
        j(14) = 463
        j(15) = 1061
        j(16) = 540
        j(17) = 1041
        j(18) = 754
        j(19) = 463
        j(20) = 1061
        j(21) = 540
        j(22) = 1041
        j(23) = 266
        j(24) = 848

        k(1) = 923
        k(2) = 314
        k(3) = 823
        k(4) = 336
        k(5) = 1026
        k(6) = 1002
        k(7) = 1279
        k(8) = 277
        k(9) = 401
        k(10) = 408
        k(11) = 469
        k(12) = 490
        k(13) = 521
        k(14) = 1110
        k(15) = 1279
        k(16) = 277
        k(17) = 401
        k(18) = 408
        k(19) = 469
        k(20) = 490
        k(21) = 521
        k(22) = 1110
        k(23) = 823
        k(24) = 336

        l(1) = 1136
        l(2) = 660
        l(3) = 668
        l(4) = 741
        l(5) = 422
        l(6) = 1103
        l(7) = 596
        l(8) = 516
        l(9) = 628
        l(10) = 1128
        l(11) = 904
        l(12) = 757
        l(13) = 767
        l(14) = 812
        l(15) = 742
        l(16) = 788
        l(17) = 1083
        l(18) = 1148
        l(19) = 812
        l(20) = 742
        l(21) = 788
        l(22) = 1083
        l(23) = 668
        l(24) = 741

        m(1) = 1219
        m(2) = 1145
        m(3) = 927
        m(4) = 1280
        m(5) = 719
        m(6) = 814
        m(7) = 1128
        m(8) = 632
        m(9) = 1231
        m(10) = 755
        m(11) = 1228
        m(12) = 818
        m(13) = 494
        m(14) = 685
        m(15) = 1128
        m(16) = 632
        m(17) = 1231
        m(18) = 755
        m(19) = 1228
        m(20) = 818
        m(21) = 494
        m(22) = 685
        m(23) = 927
        m(24) = 1280

        n(1) = 1136
        n(2) = 660
        n(3) = 668
        n(4) = 741
        n(5) = 422
        n(6) = 1103
        n(7) = 596
        n(8) = 516
        n(9) = 784
        n(10) = 929
        n(11) = 501
        n(12) = 417
        n(13) = 610
        n(14) = 399
        n(15) = 375
        n(16) = 1076
        n(17) = 1083
        n(18) = 1148
        n(19) = 812
        n(20) = 742
        n(21) = 788
        n(22) = 1083
        n(23) = 668
        n(24) = 741

        For i = 1 To 24

            dt(i) = Mid$(xCadena, i, 1)

        Next

        cc1 = 0
        cc2 = 0
        cc3 = 0
        cc4 = 0
        cc5 = 0
        cc6 = 0
        cc7 = 0
        cc8 = 0
        cc9 = 0
        cc10 = 0
        cc11 = 0
        cc12 = 0
        cc13 = 0

        For i = 1 To 24
            cc1 = cc1 + (Asc(dt(i)) * a(i))
            cc2 = cc2 + (Asc(dt(i)) * b(i))
            cc3 = cc3 + (Asc(dt(i)) * c(i))
            cc4 = cc4 + (Asc(dt(i)) * d(i))
            cc5 = cc5 + (Asc(dt(i)) * e(i))
            cc6 = cc6 + (Asc(dt(i)) * f(i))
            cc7 = cc7 + (Asc(dt(i)) * g(i))
            cc8 = cc8 + (Asc(dt(i)) * h(i))
            cc9 = cc9 + (Asc(dt(i)) * j(i))
            cc10 = cc10 + (Asc(dt(i)) * k(i))
            cc11 = cc11 + (Asc(dt(i)) * l(i))
            cc12 = cc12 + (Asc(dt(i)) * m(i))
            cc13 = cc13 + (Asc(dt(i)) * n(i))

        Next

        Tcc1 = Chr((cc1 - (26 * Int(cc1 / 26))) + 65)
        Tcc2 = Chr((cc2 - (26 * Int(cc2 / 26))) + 65)
        Tcc3 = Chr((cc3 - (26 * Int(cc3 / 26))) + 65)
        Tcc4 = Chr((cc4 - (26 * Int(cc4 / 26))) + 65)
        Tcc5 = Chr((cc5 - (26 * Int(cc5 / 26))) + 65)
        Tcc6 = Chr((cc6 - (26 * Int(cc6 / 26))) + 65)
        Tcc7 = Chr((cc7 - (26 * Int(cc7 / 26))) + 65)
        Tcc8 = Chr((cc8 - (26 * Int(cc8 / 26))) + 65)
        Tcc9 = Chr((cc9 - (26 * Int(cc9 / 26))) + 65)
        Tcc10 = Chr((cc10 - (26 * Int(cc10 / 26))) + 65)
        Tcc11 = Chr((cc11 - (26 * Int(cc11 / 26))) + 65)
        Tcc12 = Chr((cc12 - (26 * Int(cc12 / 26))) + 65)
        Tcc13 = Chr((cc13 - (26 * Int(cc13 / 26))) + 65)

        xStatus = Tcc1 & Tcc2 & Tcc3 & Tcc4 & Tcc5 & Tcc6 & Tcc7 & Tcc8 & Tcc9 & Tcc10 & Tcc11 & Tcc12 & Tcc13

        Return xStatus

    End Function

End Module
