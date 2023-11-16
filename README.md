# Interlocking BatterySaver by Wi-Fi
## 「Interlocking BatterySaver by Wi-Fi」は、接続しているWi-Fiアクセスポイント毎にバッテリー節約機能のオンオフが出来る、Windows11/10用ソフトウェアです。

**窓の杜様でこのソフトが紹介されました。**

> 「Interlocking BatterySaver by Wi-Fi」は、接続しているWi-Fiアクセスポイントに応じてPCの「バッテリー節約機能」をON/OFFできるツール。Windows 10/11に対応するフリーソフトで、「Microsoft Store」から無償でダウンロードできる。
> 
> 本ソフトを利用すると、接続するアクセスポイントに応じて「バッテリー節約機能」が有効になるバッテリー残量を変更可能。常時「バッテリー節約機能」をONにしたり、全く利用しないようにすることもできる。OS標準では行えない、きめ細かい運用が可能だ。
> 
> [接続中のWi-Fiに応じてPCの省電力機能を制御できる賢いフリーソフト  
> OS標準では行えない、きめ細かい運用が可能な「Interlocking BatterySaver by Wi-Fi」 – 窓の杜](https://forest.watch.impress.co.jp/docs/review/1496517.html)



<a href='https://apps.microsoft.com/store/detail/interlocking-batterysaver-by-wifi/9NZV3DKCLW2P?launch=true&mode=mini'><img width="200px" alt='Microsoftから入手' src='https://get.microsoft.com/images/ja-JP%20dark.svg'/></a>

**詳しい使い方と最新情報は[こちら](https://taksas.net/interlocking-batterysaver-by-wi-fi)**

![](https://taksas.net/wp-content/uploads/2023/08/Interlocking-BatterySaver-by-Wi-Fi-v2.0.20-2023_08_29-10_23_41-1.png.webp)


接続先毎に、バッテリー残量が何パーセント以下になったら節約機能をオンにするかを設定することが可能です。

設定範囲は、Windows標準では出来ない60%～90%への設定も可能です。

また、Wi-Fiに接続していないとき、リスト一覧に無いWi-Fiに接続しているときの処理も設定可能です。

アクセスポイントが切り替えられたことをOSから伝えられるまで殆どの処理を行わず、負荷の高いGUIは自動起動時読み込まれないようにすることで、低負荷を実現しています。

## 使い方

![](https://taksas.net/wp-content/uploads/2023/08/Screenshot-2023_08_29-11_11_57.png)

初回起動時に、次回以降の自動起動（PCの起動時に一緒に起動する）設定が行われます。OKを押してください。

![](https://taksas.net/wp-content/uploads/2023/04/leaf_20579.png)

右下にある葉っぱのアイコンをクリックしてください。

![](https://taksas.net/wp-content/uploads/2023/08/Interlocking-BatterySaver-by-Wi-Fi-v2.0.20-2023_08_29-10_42_48.png)

現在の設定内容が一覧表示されます。

右下の「AP（アクセスポイント）を追加」をクリックすることで、設定を新たに追加することが出来ます。

![](https://taksas.net/wp-content/uploads/2023/08/Wi-Fi%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E3%83%9D%E3%82%A4%E3%83%B3%E3%83%88-AP-%E3%82%92%E8%BF%BD%E5%8A%A0-2023_08_29-10_42_55.png)

APの名前には、設定を追加したいWi-Fiアクセスポイント名（Buffalo～やAirStation～、aterm～から始まる「Wi-Fiの名前」です）を入力してください。

現在接続しているWi-Fiを設定したい場合は、「現在のAP」ボタンをクリックすると名前が自動で入力されます。

2段目の「常時」の部分をクリックすることで、「何パーセントになったらバッテリー節約機能をオンにするか」を設定できます。デフォルトでは「常時」、つまりそのWi-Fiに接続している間は常にバッテリー節約機能がオンになります。

入力が完了したら、「追加」ボタンをクリックしてください。設定完了です。

## よくある質問

準備中。

不明点の質問、バグの報告などは[作者のTwitter](https://twitter.com/taksasDESUYO)からDMでお願いします。

## 免責事項

このソフトを使用したことによって生じたあらゆる損害について、ソフトウェアの作者は責任を負いません。

## ライセンス

このソフトウェアに含まれている『[WPF UI](https://wpfui.lepo.co/)』は、以下のライセンスで提供されています。

[The MIT License](https://licenses.opensource.jp/MIT/MIT.html)


## 開発者向け情報

### APリストの構造
v2.3以降、APリストは.NETのSettings(Settings.settings)内に"APList"という名前でjson形式で保存されています（実態はstring型で、適宜シリアライズデシリアライズを行って読み書きをします）。

v2.3よりも前のバージョンではユーザーのRoamingフォルダ配下のIBSbW\\data.txt内に保存されています。以降のバージョンでは互換性維持の為に起動時にこのファイルが存在するか確認され、存在した場合は変換の上で読み込み後削除されます（この仕様を利用することで、アプリを再インストールせずにAPリストだけを初期化できます）。


#### 旧バージョン（v2.3よりも前）のdata.txtの仕様
1行につき一つのAPを設定可能です。各ルールの特定にAP名を使用するため同時に同じ名前のAPをリストに登録することは出来ません。

##### フォーマット
```
<AP名>,<バッテリー%>
```

##### 例
```
Buffalo-5G,20
KAGAWA-UNIV,80
```

バッテリー%はこの数値がそのままCMDに渡されることになります。

#### 新バージョンの Settings.settings > APList の仕様
主な変更点は以下になります。
- json形式が採用されており、データのネストをサポートします。
- UUID（UUIDバージョン4）でルールを管理することで、同じAP名の複数のルールが同時に存在できるようになりました。
- バッテリー%指定の新仕様として、-1（設定を無視する）を追加しました。挙動としては、ルールに合致する場合に以降のAPルールのマッチング処理が行われず、結果として直前のバッテリー%が維持されます。
- 曜日指定、時間指定が可能になりました。 

##### フォーマット

```
{
	"metadata": {
		"Format_Version": "<フォーマットバージョン>",
		"Modified_App_Ver": "最終更新時のアプリバージョン",
		"Last_Modified": "<最終更新時間>"
	}
	"AP": {
		"List": {
			"<付与されたUUID>": {
				"Priority": "<優先度、ユニーク、0~>",
				"Regex": "<AP名に正規表現を利用するかどうか。1: Yes 又は 0: No>",
				"APName": "<アクセスポイント名>",
				"Battery": "<バッテリー%, 特殊用途として-1（設定を無視する）を利用可能>",
				"DayOfWeek": {
					"Sun": "<1: Yes 又は 0: No>",
					"Mon": "<1: Yes 又は 0: No>",
					"Tue": "<1: Yes 又は 0: No>",
					"Wed": "<1: Yes 又は 0: No>",
					"Thu": "<1: Yes 又は 0: No>",
					"Fri": "<1: Yes 又は 0: No>",
					"Sat": "<1: Yes 又は 0: No>"
				},
				"Time": {
					"Start": "<HHMMの形式、24時間表記>",
					"End": "<HHMMの形式、24時間表記>"
				}
			}
		}
	}
}
```