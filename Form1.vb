Imports System.Net
Imports System.Web
Imports System.IO
Imports Newtonsoft.Json.Linq

Public Class Form1

    ' 全局 文件数据
    Public url As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' 真 初始化 释放DLL
        Dim b() As Byte = My.Resources.Resource1.dll
        Dim s As IO.Stream = File.Create(Application.StartupPath + "/Newtonsoft.Json.dll")
        s.Write(b, 0, b.Length)
        s.Close()

        ' 初始化
        TextBox2.ReadOnly = True
        SaveFileDialog1.Filter = "便携式网络图形 |*.png"
        SaveFileDialog1.Title = "选择皮肤文件保存位置..."

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        ' 检测玩家名是否输入
        If (TextBox1.Text = "") Then
            MsgBox("您没有输入用户名",, "信息")
            Exit Sub
        Else
            MsgBox("将获取较长时间，请耐心等待",, "信息")
        End If

        ' 第一接收
        Dim res1str As String
        Dim res1json As JObject

        ' 第二接收
        Dim res2str As String
        Dim res2json As JObject

        ' uuid
        Dim uuid As String

        ' url s
        Dim urlEncode As String
        Dim urlDecodeStr As String
        Dim urlJson As JObject

        ' 定义一个HttpWebRequest类实体
        Dim webRequest As HttpWebRequest
        Dim responseReader As StreamReader
        Dim responseData As String

        ' GET uuid
        webRequest = CType(Net.WebRequest.Create("https://api.mojang.com/users/profiles/minecraft/" + TextBox1.Text), HttpWebRequest)
        responseReader = New StreamReader(webRequest.GetResponse().GetResponseStream())
        responseData = responseReader.ReadToEnd()
        res1str = responseData

        ' str2json
        res1json = JObject.Parse(res1str)

        ' 提取uuid
        uuid = res1json.SelectToken("id")

        ' GET texture (Base64 Encode)
        webRequest = CType(Net.WebRequest.Create("https://sessionserver.mojang.com/session/minecraft/profile/" + uuid), HttpWebRequest)
        responseReader = New StreamReader(webRequest.GetResponse().GetResponseStream())
        responseData = responseReader.ReadToEnd()
        res2str = responseData.Replace("[", "").Replace("]", "")

        ' str2json
        res2json = JObject.Parse(res2str)

        ' 提取url (JSON) (Base64Encode)
        urlEncode = res2json.SelectToken("properties").SelectToken("value")

        ' Base64解码
        urlDecodeStr = System.Text.Encoding.GetEncoding("utf-8").GetString(Convert.FromBase64String(urlEncode))

        ' str2json
        urlJson = JObject.Parse(urlDecodeStr)

        ' skin url
        url = urlJson.SelectToken("textures").SelectToken("SKIN").SelectToken("url")

        ' url 文本框 显示
        TextBox2.Text = url

        ' 请求图片 (For PictureBox)
        webRequest = CType(Net.WebRequest.Create(url), HttpWebRequest)
        Dim res As WebResponse = webRequest.GetResponse
        Dim bmp As New Bitmap(res.GetResponseStream)
        PictureBox1.Image = bmp


        ' 设置保存文件 文件名
        SaveFileDialog1.FileName = TextBox1.Text

        ' 允许点击 另存为
        Button2.Enabled = True

        responseReader.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        ' 保存路径
        Dim savePath As String

        ' 浏览目录
        SaveFileDialog1.ShowDialog()
        savePath = SaveFileDialog1.FileName

        Dim x As System.Net.WebClient = New System.Net.WebClient()
        x.DownloadFile(url, savePath)

        ' Dim file As New System.IO.StreamWriter(savePath)
        ' File.Write(fileData)


    End Sub
End Class
