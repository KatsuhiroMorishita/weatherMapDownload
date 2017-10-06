Public Class Form1
    ''' <summary>
    ''' タイマーが起動するごとに、気象庁のHPを確認して天気図をダウンロードする
    ''' 
    ''' [開発履歴]
    ''' 2011/7/16 夜中から作っている。台風が来るらしいので大忙し。
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub clockCheckTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles clockCheckTimer.Tick
        Dim t As DateTime = Date.Now
        Dim t2 As DateTime
        Dim wc As New System.Net.WebClient()
        Dim path As String = "http://www.jma.go.jp/jp/g3/images/observe/"
        Dim yy, mm, dd, hh As String
        Dim fname, savePath As String
        Dim hour As Integer
        Const releaseTime As Double = 7830.0            ' ある時刻分の天気図が公開されるまでの時間差（2時間10分後とされている）

        If System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() Then  ' ネットに接続しているかを確認
            Me.ICCondition.Text = "Internet Connection Condition: Connect"      ' 接続できていることを示す。
            ' ダウンロードファイル名生成の準備
            t = t.AddSeconds(-releaseTime)                                      ' 既にリリースされている分がほしいので、リリースまでのディレイを引いて計算する
            hour = Math.Floor(t.Hour / 3) * 3
            If hour = 24 Or hour = 0 Then
                hour = 21
                t = t.AddDays(-1)                                               ' 前日に時間を戻す
            End If
            yy = (t.Year Mod 100).ToString("d2")
            mm = (t.Month).ToString("d2")
            dd = t.Day.ToString("d2")
            hh = hour.ToString("d2")
            fname = yy + mm + dd + hh + ".png"
            savePath = System.IO.Directory.GetCurrentDirectory() + "\" + fname
            t2 = New DateTime(t.Year, t.Month, t.Day, hour, 0, 0).AddSeconds(releaseTime)   ' ダウンロードしたいファイルのリリース時刻を少し余裕をもって計算する
            ' ファイルが未ダウンロード
            If System.IO.File.Exists(savePath) = False Then
                If t2 < Date.Now Then                                                       ' 2時間10分経過していればダウンロードを実行する
                    path += fname
                    Try
                        wc.DownloadFile(path, savePath)                                     ' ダウンロード
                        Me.BackgroundImage = System.Drawing.Image.FromFile(savePath)        ' 読み込めたら、表示する
                        Me.ErrorInfo.Text = "ダウンロードしました。"
                        If hour = 21 Then t2 = t2.AddHours(6) Else t2 = t2.AddHours(3) ' 次のダウンロード時刻を計算する
                    Catch ex As Exception
                        Me.ErrorInfo.Text = "何らかのエラーが発生しました。orz"
                    End Try
                End If
            Else
                If hour = 21 Then t2 = t2.AddHours(6) Else t2 = t2.AddHours(3) ' 既にダウンロード済みであった場合、次のダウンロード時刻を計算する
            End If
            Me.NextDwload.Text = "Download clock:" + t2.ToString                            ' ダウンロード時刻を表示する
            Me.NextDwload.Text += "   Last:" + CInt(Date.Now.Subtract(t2).TotalSeconds).ToString() + "[s]"
        Else
            Me.ICCondition.Text = "Internet Connection Condition: Disconnect"
        End If
        wc.Dispose()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() Then  ' ネットに接続しているかを確認
            Me.ICCondition.Text = "Internet Connection Condition: Connect"
        Else
            Me.ICCondition.Text = "Internet Connection Condition: Disconnect"
        End If
    End Sub
End Class
