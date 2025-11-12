<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.lblHeader = New System.Windows.Forms.Label()
        Me.PicESPLogo = New System.Windows.Forms.PictureBox()
        Me.lblIDmj = New System.Windows.Forms.Label()
        Me.picLogoESP = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtMySQLConnectionString = New System.Windows.Forms.TextBox()
        Me.btnIniSQL = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.lblFirmWare = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblVoltajeDLC = New System.Windows.Forms.Label()
        Me.lblIdDevice = New System.Windows.Forms.Label()
        Me.lblDTC = New System.Windows.Forms.TextBox()
        Me.LblOBD_CAT_C = New System.Windows.Forms.Label()
        Me.LblOBD_O2S_C = New System.Windows.Forms.Label()
        Me.LblOBD_CMB_C = New System.Windows.Forms.Label()
        Me.LblOBD_CCM_C = New System.Windows.Forms.Label()
        Me.LblOBD_MSI_C = New System.Windows.Forms.Label()
        Me.LblOBD_CAT_D = New System.Windows.Forms.Label()
        Me.LblOBD_O2S_D = New System.Windows.Forms.Label()
        Me.LblOBD_CMB_D = New System.Windows.Forms.Label()
        Me.LblOBD_CCM_D = New System.Windows.Forms.Label()
        Me.LblOBD_MSI_D = New System.Windows.Forms.Label()
        Me.LblOBD_mil = New System.Windows.Forms.Label()
        Me.lblProtocolo = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LblOBD_CAT = New System.Windows.Forms.Label()
        Me.LblOBD_O2S = New System.Windows.Forms.Label()
        Me.LblOBD_CMB = New System.Windows.Forms.Label()
        Me.LblOBD_CCM = New System.Windows.Forms.Label()
        Me.LblOBD_MSI = New System.Windows.Forms.Label()
        Me.BtnInitDevice = New System.Windows.Forms.Button()
        Me.PicOBD = New System.Windows.Forms.PictureBox()
        Me.lblVoltaje = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.LblVIN = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblDeviceDescription = New System.Windows.Forms.Label()
        Me.BtnOBDtest = New System.Windows.Forms.Button()
        Me.lblFechaHora = New System.Windows.Forms.Label()
        Me.PanelMsgUsu = New System.Windows.Forms.GroupBox()
        Me.lblTerminalDatos = New System.Windows.Forms.Label()
        Me.lblMensajeUsuario = New System.Windows.Forms.Label()
        Me.BtnContinuar = New System.Windows.Forms.Button()
        Me.PicOpusLogo = New System.Windows.Forms.PictureBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.UsrBallTimer = New usrBallTimer.usrBallTimer()
        CType(Me.PicESPLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picLogoESP, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.PicOBD, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelMsgUsu.SuspendLayout()
        CType(Me.PicOpusLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblHeader
        '
        Me.lblHeader.BackColor = System.Drawing.Color.Transparent
        Me.lblHeader.Font = New System.Drawing.Font("Arial", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader.ForeColor = System.Drawing.Color.White
        Me.lblHeader.Location = New System.Drawing.Point(231, 22)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(920, 38)
        Me.lblHeader.TabIndex = 281
        Me.lblHeader.Text = "Opus IMCleanOBD Vehicular Inspection"
        Me.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PicESPLogo
        '
        Me.PicESPLogo.Image = CType(resources.GetObject("PicESPLogo.Image"), System.Drawing.Image)
        Me.PicESPLogo.Location = New System.Drawing.Point(14, 12)
        Me.PicESPLogo.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.PicESPLogo.Name = "PicESPLogo"
        Me.PicESPLogo.Size = New System.Drawing.Size(97, 52)
        Me.PicESPLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicESPLogo.TabIndex = 280
        Me.PicESPLogo.TabStop = False
        '
        'lblIDmj
        '
        Me.lblIDmj.BackColor = System.Drawing.Color.Transparent
        Me.lblIDmj.Font = New System.Drawing.Font("Arial Unicode MS", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblIDmj.ForeColor = System.Drawing.Color.White
        Me.lblIDmj.Location = New System.Drawing.Point(1172, 14)
        Me.lblIDmj.Name = "lblIDmj"
        Me.lblIDmj.Size = New System.Drawing.Size(70, 14)
        Me.lblIDmj.TabIndex = 279
        Me.lblIDmj.Text = "fw11.11.22"
        Me.lblIDmj.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'picLogoESP
        '
        Me.picLogoESP.Image = CType(resources.GetObject("picLogoESP.Image"), System.Drawing.Image)
        Me.picLogoESP.Location = New System.Drawing.Point(1244, 12)
        Me.picLogoESP.Name = "picLogoESP"
        Me.picLogoESP.Size = New System.Drawing.Size(88, 52)
        Me.picLogoESP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picLogoESP.TabIndex = 278
        Me.picLogoESP.TabStop = False
        '
        'Panel1
        '
        Me.Panel1.BackgroundImage = CType(resources.GetObject("Panel1.BackgroundImage"), System.Drawing.Image)
        Me.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Panel1.Controls.Add(Me.UsrBallTimer)
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.txtMySQLConnectionString)
        Me.Panel1.Controls.Add(Me.btnIniSQL)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.ListBox1)
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Controls.Add(Me.lblFirmWare)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.lblVoltajeDLC)
        Me.Panel1.Controls.Add(Me.lblIdDevice)
        Me.Panel1.Controls.Add(Me.lblDTC)
        Me.Panel1.Controls.Add(Me.LblOBD_CAT_C)
        Me.Panel1.Controls.Add(Me.LblOBD_O2S_C)
        Me.Panel1.Controls.Add(Me.LblOBD_CMB_C)
        Me.Panel1.Controls.Add(Me.LblOBD_CCM_C)
        Me.Panel1.Controls.Add(Me.LblOBD_MSI_C)
        Me.Panel1.Controls.Add(Me.LblOBD_CAT_D)
        Me.Panel1.Controls.Add(Me.LblOBD_O2S_D)
        Me.Panel1.Controls.Add(Me.LblOBD_CMB_D)
        Me.Panel1.Controls.Add(Me.LblOBD_CCM_D)
        Me.Panel1.Controls.Add(Me.LblOBD_MSI_D)
        Me.Panel1.Controls.Add(Me.LblOBD_mil)
        Me.Panel1.Controls.Add(Me.lblProtocolo)
        Me.Panel1.Controls.Add(Me.Label4)
        Me.Panel1.Controls.Add(Me.LblOBD_CAT)
        Me.Panel1.Controls.Add(Me.LblOBD_O2S)
        Me.Panel1.Controls.Add(Me.LblOBD_CMB)
        Me.Panel1.Controls.Add(Me.LblOBD_CCM)
        Me.Panel1.Controls.Add(Me.LblOBD_MSI)
        Me.Panel1.Controls.Add(Me.BtnInitDevice)
        Me.Panel1.Controls.Add(Me.PicOBD)
        Me.Panel1.Controls.Add(Me.lblVoltaje)
        Me.Panel1.Controls.Add(Me.Label7)
        Me.Panel1.Controls.Add(Me.LblVIN)
        Me.Panel1.Controls.Add(Me.Label5)
        Me.Panel1.Controls.Add(Me.lblDeviceDescription)
        Me.Panel1.Controls.Add(Me.BtnOBDtest)
        Me.Panel1.Controls.Add(Me.lblFechaHora)
        Me.Panel1.Controls.Add(Me.PanelMsgUsu)
        Me.Panel1.Location = New System.Drawing.Point(12, 70)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1320, 610)
        Me.Panel1.TabIndex = 282
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Transparent
        Me.Label6.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Location = New System.Drawing.Point(14, 581)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(212, 18)
        Me.Label6.TabIndex = 312
        Me.Label6.Text = "Cadena de conexión MySQL:"
        '
        'txtMySQLConnectionString
        '
        Me.txtMySQLConnectionString.Location = New System.Drawing.Point(231, 577)
        Me.txtMySQLConnectionString.Name = "txtMySQLConnectionString"
        Me.txtMySQLConnectionString.Size = New System.Drawing.Size(850, 26)
        Me.txtMySQLConnectionString.TabIndex = 311
        Me.txtMySQLConnectionString.Text = "server=localhost;uid=opus1234;pwd=1234opus;database=OpusOBDtest;Integrated Securi" &
    "ty=True"
        '
        'btnIniSQL
        '
        Me.btnIniSQL.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnIniSQL.ForeColor = System.Drawing.Color.Black
        Me.btnIniSQL.Location = New System.Drawing.Point(9, 17)
        Me.btnIniSQL.Name = "btnIniSQL"
        Me.btnIniSQL.Size = New System.Drawing.Size(218, 33)
        Me.btnIniSQL.TabIndex = 310
        Me.btnIniSQL.Text = "Conectar SQL"
        Me.btnIniSQL.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(182, 390)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(50, 22)
        Me.Label2.TabIndex = 309
        Me.Label2.Text = "DTC"
        '
        'ListBox1
        '
        Me.ListBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(188, Byte), Integer), CType(CType(183, Byte), Integer), CType(CType(152, Byte), Integer))
        Me.ListBox1.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.ItemHeight = 15
        Me.ListBox1.Location = New System.Drawing.Point(639, 242)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(668, 259)
        Me.ListBox1.TabIndex = 308
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ForeColor = System.Drawing.Color.Black
        Me.Button1.Location = New System.Drawing.Point(9, 184)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(215, 33)
        Me.Button1.TabIndex = 307
        Me.Button1.Text = "test"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'lblFirmWare
        '
        Me.lblFirmWare.BackColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lblFirmWare.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFirmWare.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFirmWare.ForeColor = System.Drawing.Color.White
        Me.lblFirmWare.Location = New System.Drawing.Point(235, 259)
        Me.lblFirmWare.Name = "lblFirmWare"
        Me.lblFirmWare.Size = New System.Drawing.Size(391, 30)
        Me.lblFirmWare.TabIndex = 306
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(138, 263)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(93, 22)
        Me.Label3.TabIndex = 305
        Me.Label3.Text = "FirmWare"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(415, 207)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 22)
        Me.Label1.TabIndex = 304
        Me.Label1.Text = "DLC"
        '
        'lblVoltajeDLC
        '
        Me.lblVoltajeDLC.BackColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lblVoltajeDLC.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblVoltajeDLC.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVoltajeDLC.ForeColor = System.Drawing.Color.White
        Me.lblVoltajeDLC.Location = New System.Drawing.Point(469, 202)
        Me.lblVoltajeDLC.Name = "lblVoltajeDLC"
        Me.lblVoltajeDLC.Size = New System.Drawing.Size(157, 30)
        Me.lblVoltajeDLC.TabIndex = 303
        '
        'lblIdDevice
        '
        Me.lblIdDevice.BackColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lblIdDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblIdDevice.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblIdDevice.ForeColor = System.Drawing.Color.White
        Me.lblIdDevice.Location = New System.Drawing.Point(235, 118)
        Me.lblIdDevice.Name = "lblIdDevice"
        Me.lblIdDevice.Size = New System.Drawing.Size(391, 30)
        Me.lblIdDevice.TabIndex = 301
        '
        'lblDTC
        '
        Me.lblDTC.BackColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lblDTC.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDTC.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblDTC.Location = New System.Drawing.Point(235, 388)
        Me.lblDTC.Multiline = True
        Me.lblDTC.Name = "lblDTC"
        Me.lblDTC.ReadOnly = True
        Me.lblDTC.Size = New System.Drawing.Size(391, 30)
        Me.lblDTC.TabIndex = 300
        '
        'LblOBD_CAT_C
        '
        Me.LblOBD_CAT_C.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_CAT_C.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_CAT_C.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_CAT_C.ForeColor = System.Drawing.Color.White
        Me.LblOBD_CAT_C.Location = New System.Drawing.Point(1178, 203)
        Me.LblOBD_CAT_C.Name = "LblOBD_CAT_C"
        Me.LblOBD_CAT_C.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_CAT_C.TabIndex = 297
        Me.LblOBD_CAT_C.Text = "Completado"
        Me.LblOBD_CAT_C.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_O2S_C
        '
        Me.LblOBD_O2S_C.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_O2S_C.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_O2S_C.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_O2S_C.ForeColor = System.Drawing.Color.White
        Me.LblOBD_O2S_C.Location = New System.Drawing.Point(1178, 166)
        Me.LblOBD_O2S_C.Name = "LblOBD_O2S_C"
        Me.LblOBD_O2S_C.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_O2S_C.TabIndex = 296
        Me.LblOBD_O2S_C.Text = "Completado"
        Me.LblOBD_O2S_C.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_CMB_C
        '
        Me.LblOBD_CMB_C.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_CMB_C.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_CMB_C.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_CMB_C.ForeColor = System.Drawing.Color.White
        Me.LblOBD_CMB_C.Location = New System.Drawing.Point(1178, 129)
        Me.LblOBD_CMB_C.Name = "LblOBD_CMB_C"
        Me.LblOBD_CMB_C.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_CMB_C.TabIndex = 295
        Me.LblOBD_CMB_C.Text = "Completado"
        Me.LblOBD_CMB_C.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_CCM_C
        '
        Me.LblOBD_CCM_C.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_CCM_C.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_CCM_C.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_CCM_C.ForeColor = System.Drawing.Color.White
        Me.LblOBD_CCM_C.Location = New System.Drawing.Point(1178, 92)
        Me.LblOBD_CCM_C.Name = "LblOBD_CCM_C"
        Me.LblOBD_CCM_C.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_CCM_C.TabIndex = 294
        Me.LblOBD_CCM_C.Text = "Completado"
        Me.LblOBD_CCM_C.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_MSI_C
        '
        Me.LblOBD_MSI_C.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_MSI_C.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_MSI_C.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_MSI_C.ForeColor = System.Drawing.Color.White
        Me.LblOBD_MSI_C.Location = New System.Drawing.Point(1178, 55)
        Me.LblOBD_MSI_C.Name = "LblOBD_MSI_C"
        Me.LblOBD_MSI_C.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_MSI_C.TabIndex = 293
        Me.LblOBD_MSI_C.Text = "Completado"
        Me.LblOBD_MSI_C.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_CAT_D
        '
        Me.LblOBD_CAT_D.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_CAT_D.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_CAT_D.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_CAT_D.ForeColor = System.Drawing.Color.White
        Me.LblOBD_CAT_D.Location = New System.Drawing.Point(1042, 203)
        Me.LblOBD_CAT_D.Name = "LblOBD_CAT_D"
        Me.LblOBD_CAT_D.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_CAT_D.TabIndex = 292
        Me.LblOBD_CAT_D.Text = "Disponible"
        Me.LblOBD_CAT_D.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_O2S_D
        '
        Me.LblOBD_O2S_D.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_O2S_D.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_O2S_D.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_O2S_D.ForeColor = System.Drawing.Color.White
        Me.LblOBD_O2S_D.Location = New System.Drawing.Point(1042, 166)
        Me.LblOBD_O2S_D.Name = "LblOBD_O2S_D"
        Me.LblOBD_O2S_D.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_O2S_D.TabIndex = 291
        Me.LblOBD_O2S_D.Text = "Disponible"
        Me.LblOBD_O2S_D.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_CMB_D
        '
        Me.LblOBD_CMB_D.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_CMB_D.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_CMB_D.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_CMB_D.ForeColor = System.Drawing.Color.White
        Me.LblOBD_CMB_D.Location = New System.Drawing.Point(1042, 129)
        Me.LblOBD_CMB_D.Name = "LblOBD_CMB_D"
        Me.LblOBD_CMB_D.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_CMB_D.TabIndex = 290
        Me.LblOBD_CMB_D.Text = "Disponible"
        Me.LblOBD_CMB_D.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_CCM_D
        '
        Me.LblOBD_CCM_D.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_CCM_D.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_CCM_D.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_CCM_D.ForeColor = System.Drawing.Color.White
        Me.LblOBD_CCM_D.Location = New System.Drawing.Point(1042, 92)
        Me.LblOBD_CCM_D.Name = "LblOBD_CCM_D"
        Me.LblOBD_CCM_D.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_CCM_D.TabIndex = 289
        Me.LblOBD_CCM_D.Text = "Disponible"
        Me.LblOBD_CCM_D.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_MSI_D
        '
        Me.LblOBD_MSI_D.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_MSI_D.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_MSI_D.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_MSI_D.ForeColor = System.Drawing.Color.White
        Me.LblOBD_MSI_D.Location = New System.Drawing.Point(1042, 55)
        Me.LblOBD_MSI_D.Name = "LblOBD_MSI_D"
        Me.LblOBD_MSI_D.Size = New System.Drawing.Size(129, 30)
        Me.LblOBD_MSI_D.TabIndex = 288
        Me.LblOBD_MSI_D.Text = "Disponible"
        Me.LblOBD_MSI_D.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_mil
        '
        Me.LblOBD_mil.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_mil.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_mil.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_mil.ForeColor = System.Drawing.Color.White
        Me.LblOBD_mil.Location = New System.Drawing.Point(639, 17)
        Me.LblOBD_mil.Name = "LblOBD_mil"
        Me.LblOBD_mil.Size = New System.Drawing.Size(668, 30)
        Me.LblOBD_mil.TabIndex = 286
        Me.LblOBD_mil.Text = "Luz M.I.L."
        Me.LblOBD_mil.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblProtocolo
        '
        Me.lblProtocolo.BackColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lblProtocolo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblProtocolo.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProtocolo.ForeColor = System.Drawing.Color.White
        Me.lblProtocolo.Location = New System.Drawing.Point(235, 303)
        Me.lblProtocolo.Name = "lblProtocolo"
        Me.lblProtocolo.Size = New System.Drawing.Size(391, 30)
        Me.lblProtocolo.TabIndex = 275
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Arial Unicode MS", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(130, 304)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(94, 25)
        Me.Label4.TabIndex = 274
        Me.Label4.Text = "Protocolo"
        '
        'LblOBD_CAT
        '
        Me.LblOBD_CAT.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_CAT.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_CAT.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_CAT.ForeColor = System.Drawing.Color.White
        Me.LblOBD_CAT.Location = New System.Drawing.Point(639, 202)
        Me.LblOBD_CAT.Name = "LblOBD_CAT"
        Me.LblOBD_CAT.Size = New System.Drawing.Size(397, 30)
        Me.LblOBD_CAT.TabIndex = 267
        Me.LblOBD_CAT.Text = "Eficiencia del convertidor Catalitico "
        Me.LblOBD_CAT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_O2S
        '
        Me.LblOBD_O2S.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_O2S.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_O2S.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_O2S.ForeColor = System.Drawing.Color.White
        Me.LblOBD_O2S.Location = New System.Drawing.Point(639, 165)
        Me.LblOBD_O2S.Name = "LblOBD_O2S"
        Me.LblOBD_O2S.Size = New System.Drawing.Size(397, 30)
        Me.LblOBD_O2S.TabIndex = 266
        Me.LblOBD_O2S.Text = "Sensores de Oxígeno"
        Me.LblOBD_O2S.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_CMB
        '
        Me.LblOBD_CMB.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_CMB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_CMB.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_CMB.ForeColor = System.Drawing.Color.White
        Me.LblOBD_CMB.Location = New System.Drawing.Point(639, 128)
        Me.LblOBD_CMB.Name = "LblOBD_CMB"
        Me.LblOBD_CMB.Size = New System.Drawing.Size(397, 30)
        Me.LblOBD_CMB.TabIndex = 265
        Me.LblOBD_CMB.Text = "Sistema de Combustible"
        Me.LblOBD_CMB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_CCM
        '
        Me.LblOBD_CCM.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_CCM.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_CCM.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_CCM.ForeColor = System.Drawing.Color.White
        Me.LblOBD_CCM.Location = New System.Drawing.Point(639, 91)
        Me.LblOBD_CCM.Name = "LblOBD_CCM"
        Me.LblOBD_CCM.Size = New System.Drawing.Size(397, 30)
        Me.LblOBD_CCM.TabIndex = 264
        Me.LblOBD_CCM.Text = "Componentes Integrales"
        Me.LblOBD_CCM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblOBD_MSI
        '
        Me.LblOBD_MSI.BackColor = System.Drawing.Color.Gray
        Me.LblOBD_MSI.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblOBD_MSI.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblOBD_MSI.ForeColor = System.Drawing.Color.White
        Me.LblOBD_MSI.Location = New System.Drawing.Point(639, 54)
        Me.LblOBD_MSI.Name = "LblOBD_MSI"
        Me.LblOBD_MSI.Size = New System.Drawing.Size(397, 30)
        Me.LblOBD_MSI.TabIndex = 263
        Me.LblOBD_MSI.Text = "Detección condiciones inadecuadas Ingnición cilindros "
        Me.LblOBD_MSI.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BtnInitDevice
        '
        Me.BtnInitDevice.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnInitDevice.ForeColor = System.Drawing.Color.Black
        Me.BtnInitDevice.Location = New System.Drawing.Point(9, 59)
        Me.BtnInitDevice.Name = "BtnInitDevice"
        Me.BtnInitDevice.Size = New System.Drawing.Size(218, 33)
        Me.BtnInitDevice.TabIndex = 260
        Me.BtnInitDevice.Text = "Inicializar OBD"
        Me.BtnInitDevice.UseVisualStyleBackColor = True
        '
        'PicOBD
        '
        Me.PicOBD.Image = CType(resources.GetObject("PicOBD.Image"), System.Drawing.Image)
        Me.PicOBD.Location = New System.Drawing.Point(235, 17)
        Me.PicOBD.Name = "PicOBD"
        Me.PicOBD.Size = New System.Drawing.Size(143, 98)
        Me.PicOBD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicOBD.TabIndex = 258
        Me.PicOBD.TabStop = False
        Me.PicOBD.Visible = False
        '
        'lblVoltaje
        '
        Me.lblVoltaje.BackColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lblVoltaje.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblVoltaje.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVoltaje.ForeColor = System.Drawing.Color.White
        Me.lblVoltaje.Location = New System.Drawing.Point(469, 161)
        Me.lblVoltaje.Name = "lblVoltaje"
        Me.lblVoltaje.Size = New System.Drawing.Size(157, 30)
        Me.lblVoltaje.TabIndex = 252
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.BackColor = System.Drawing.Color.Transparent
        Me.Label7.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.White
        Me.Label7.Location = New System.Drawing.Point(393, 165)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(67, 22)
        Me.Label7.TabIndex = 251
        Me.Label7.Text = "Voltaje"
        '
        'LblVIN
        '
        Me.LblVIN.BackColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.LblVIN.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LblVIN.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblVIN.ForeColor = System.Drawing.Color.White
        Me.LblVIN.Location = New System.Drawing.Point(235, 344)
        Me.LblVIN.Name = "LblVIN"
        Me.LblVIN.Size = New System.Drawing.Size(391, 30)
        Me.LblVIN.TabIndex = 250
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(185, 348)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(42, 22)
        Me.Label5.TabIndex = 249
        Me.Label5.Text = "VIN"
        '
        'lblDeviceDescription
        '
        Me.lblDeviceDescription.BackColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(51, Byte), Integer))
        Me.lblDeviceDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDeviceDescription.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDeviceDescription.ForeColor = System.Drawing.Color.White
        Me.lblDeviceDescription.Location = New System.Drawing.Point(384, 17)
        Me.lblDeviceDescription.Name = "lblDeviceDescription"
        Me.lblDeviceDescription.Size = New System.Drawing.Size(242, 30)
        Me.lblDeviceDescription.TabIndex = 248
        '
        'BtnOBDtest
        '
        Me.BtnOBDtest.Enabled = False
        Me.BtnOBDtest.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnOBDtest.ForeColor = System.Drawing.Color.Black
        Me.BtnOBDtest.Location = New System.Drawing.Point(9, 102)
        Me.BtnOBDtest.Name = "BtnOBDtest"
        Me.BtnOBDtest.Size = New System.Drawing.Size(218, 33)
        Me.BtnOBDtest.TabIndex = 143
        Me.BtnOBDtest.Text = "Vehiculo Inspección"
        Me.BtnOBDtest.UseVisualStyleBackColor = True
        '
        'lblFechaHora
        '
        Me.lblFechaHora.BackColor = System.Drawing.Color.Transparent
        Me.lblFechaHora.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFechaHora.ForeColor = System.Drawing.Color.White
        Me.lblFechaHora.Location = New System.Drawing.Point(1087, 576)
        Me.lblFechaHora.Name = "lblFechaHora"
        Me.lblFechaHora.Size = New System.Drawing.Size(220, 24)
        Me.lblFechaHora.TabIndex = 105
        Me.lblFechaHora.Text = "00/00/0000 - 00:00:00"
        Me.lblFechaHora.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblFechaHora.UseCompatibleTextRendering = True
        '
        'PanelMsgUsu
        '
        Me.PanelMsgUsu.BackColor = System.Drawing.Color.FromArgb(CType(CType(10, Byte), Integer), CType(CType(58, Byte), Integer), CType(CType(102, Byte), Integer))
        Me.PanelMsgUsu.Controls.Add(Me.lblTerminalDatos)
        Me.PanelMsgUsu.Controls.Add(Me.lblMensajeUsuario)
        Me.PanelMsgUsu.Location = New System.Drawing.Point(9, 506)
        Me.PanelMsgUsu.Name = "PanelMsgUsu"
        Me.PanelMsgUsu.Size = New System.Drawing.Size(1298, 67)
        Me.PanelMsgUsu.TabIndex = 42
        Me.PanelMsgUsu.TabStop = False
        '
        'lblTerminalDatos
        '
        Me.lblTerminalDatos.BackColor = System.Drawing.Color.Gray
        Me.lblTerminalDatos.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTerminalDatos.Font = New System.Drawing.Font("Arial", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTerminalDatos.ForeColor = System.Drawing.Color.White
        Me.lblTerminalDatos.Location = New System.Drawing.Point(1191, 21)
        Me.lblTerminalDatos.Name = "lblTerminalDatos"
        Me.lblTerminalDatos.Size = New System.Drawing.Size(100, 31)
        Me.lblTerminalDatos.TabIndex = 283
        Me.lblTerminalDatos.Text = "MySQL"
        Me.lblTerminalDatos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblTerminalDatos.UseCompatibleTextRendering = True
        '
        'lblMensajeUsuario
        '
        Me.lblMensajeUsuario.BackColor = System.Drawing.Color.Transparent
        Me.lblMensajeUsuario.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMensajeUsuario.ForeColor = System.Drawing.Color.White
        Me.lblMensajeUsuario.Location = New System.Drawing.Point(15, 19)
        Me.lblMensajeUsuario.Name = "lblMensajeUsuario"
        Me.lblMensajeUsuario.Size = New System.Drawing.Size(1169, 31)
        Me.lblMensajeUsuario.TabIndex = 31
        Me.lblMensajeUsuario.Text = "Mensajes para el usuario"
        Me.lblMensajeUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BtnContinuar
        '
        Me.BtnContinuar.Font = New System.Drawing.Font("Arial", 13.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnContinuar.ForeColor = System.Drawing.Color.Black
        Me.BtnContinuar.Location = New System.Drawing.Point(1152, 686)
        Me.BtnContinuar.Name = "BtnContinuar"
        Me.BtnContinuar.Size = New System.Drawing.Size(180, 40)
        Me.BtnContinuar.TabIndex = 284
        Me.BtnContinuar.Text = "Terminar"
        Me.BtnContinuar.UseVisualStyleBackColor = True
        '
        'PicOpusLogo
        '
        Me.PicOpusLogo.Image = CType(resources.GetObject("PicOpusLogo.Image"), System.Drawing.Image)
        Me.PicOpusLogo.Location = New System.Drawing.Point(11, 686)
        Me.PicOpusLogo.Name = "PicOpusLogo"
        Me.PicOpusLogo.Size = New System.Drawing.Size(260, 45)
        Me.PicOpusLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicOpusLogo.TabIndex = 283
        Me.PicOpusLogo.TabStop = False
        '
        'Timer1
        '
        Me.Timer1.Interval = 900
        '
        'UsrBallTimer
        '
        Me.UsrBallTimer.BackColor = System.Drawing.Color.White
        Me.UsrBallTimer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.UsrBallTimer.Location = New System.Drawing.Point(9, 390)
        Me.UsrBallTimer.Margin = New System.Windows.Forms.Padding(6, 6, 6, 6)
        Me.UsrBallTimer.Name = "UsrBallTimer"
        Me.UsrBallTimer.Size = New System.Drawing.Size(115, 101)
        Me.UsrBallTimer.TabIndex = 313
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Teal
        Me.ClientSize = New System.Drawing.Size(1354, 734)
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnContinuar)
        Me.Controls.Add(Me.PicOpusLogo)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.lblHeader)
        Me.Controls.Add(Me.PicESPLogo)
        Me.Controls.Add(Me.lblIDmj)
        Me.Controls.Add(Me.picLogoESP)
        Me.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Form1"
        CType(Me.PicESPLogo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picLogoESP, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PicOBD, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelMsgUsu.ResumeLayout(False)
        CType(Me.PicOpusLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblHeader As Label
    Friend WithEvents PicESPLogo As PictureBox
    Friend WithEvents lblIDmj As Label
    Friend WithEvents picLogoESP As PictureBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblDTC As TextBox
    Friend WithEvents LblOBD_CAT_C As Label
    Friend WithEvents LblOBD_O2S_C As Label
    Friend WithEvents LblOBD_CMB_C As Label
    Friend WithEvents LblOBD_CCM_C As Label
    Friend WithEvents LblOBD_MSI_C As Label
    Friend WithEvents LblOBD_CAT_D As Label
    Friend WithEvents LblOBD_O2S_D As Label
    Friend WithEvents LblOBD_CMB_D As Label
    Friend WithEvents LblOBD_CCM_D As Label
    Friend WithEvents LblOBD_MSI_D As Label
    Friend WithEvents LblOBD_mil As Label
    Friend WithEvents lblProtocolo As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents LblOBD_CAT As Label
    Friend WithEvents LblOBD_O2S As Label
    Friend WithEvents LblOBD_CMB As Label
    Friend WithEvents LblOBD_CCM As Label
    Friend WithEvents LblOBD_MSI As Label
    Friend WithEvents BtnInitDevice As Button
    Friend WithEvents PicOBD As PictureBox
    Friend WithEvents lblVoltaje As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents LblVIN As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents lblDeviceDescription As Label
    Friend WithEvents BtnOBDtest As Button
    Friend WithEvents lblFechaHora As Label
    Friend WithEvents PanelMsgUsu As GroupBox
    Friend WithEvents lblTerminalDatos As Label
    Friend WithEvents lblMensajeUsuario As Label
    Friend WithEvents BtnContinuar As Button
    Friend WithEvents PicOpusLogo As PictureBox
    Friend WithEvents Timer1 As Timer
    Friend WithEvents lblIdDevice As Label
    Friend WithEvents lblVoltajeDLC As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents lblFirmWare As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents ListBox1 As ListBox
    Friend WithEvents Label2 As Label
    Friend WithEvents btnIniSQL As Button
    Friend WithEvents Label6 As Label
    Friend WithEvents txtMySQLConnectionString As TextBox
    Friend WithEvents UsrBallTimer As usrBallTimer.usrBallTimer
End Class
