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


    Public Sub Applog(ByVal lParametro As String)

        Dim logFile As String = "OpusIMCleanOBD.log"

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

End Module
