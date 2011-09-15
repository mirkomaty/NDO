Imports NDO
Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO


<NDOPersistent()> _
Public Class Picture
    Inherits PictureHeader

    ' Methods
    Public Sub New()
        _rawbytes = Nothing
        _image = Nothing
    End Sub


    ' Properties
    Public Property Image() As Image
        Get
            If (_rawbytes Is Nothing) Then
                Return Nothing
            End If
            If (_image Is Nothing) Then
                Dim stream1 As New MemoryStream(_rawbytes)
                _image = System.Drawing.Image.FromStream(stream1)
            End If
            Return _image
        End Get
        Set(ByVal value As Image)
            _image = value
            If (_image Is Nothing) Then
                _rawbytes = Nothing
            Else
                ' align horizontal size to 4 bytes 
                Dim horsize As Integer = _image.Size.Width * 3
                Dim remainder As Integer = horsize Mod 4
                If (remainder > 0) Then
                    remainder = 1
                End If
                horsize = horsize \ 4
                horsize = (horsize + remainder) * 4
                Dim vertsize As Integer = _image.Size.Height
                _rawbytes = New Byte(horsize * vertsize + 56) {}
                Dim stream1 As New MemoryStream(_rawbytes)
                _image.Save(stream1, ImageFormat.Bmp)
            End If
        End Set
    End Property


    ' Fields
    <NDOTransient()> _
    Private _image As Image
    Private _rawbytes As Byte()
End Class



