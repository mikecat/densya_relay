##############
#電車の橋渡し#
##############

電車でGo！コントローラー変換器 (以下、変換器)
https://autotraintas.hariko.com/
の「Mapped(Trial)」で入出力されるメモリマップドファイルの情報、
および出力されるキー操作の情報を、UDP で別のコンピュータに転送します。

これを用いることで、
変換器が動かないコンピュータでゲームをしたり、ツールの開発をしたりする際、
変換器が動く他のコンピュータから情報を受け取って操作できるようになります。

また、メモリマップドファイルの情報を直接表示・編集できるため、
変換器が動作する環境や対応コントローラを用意しなくてもツールの開発ができる可能性があります。

変換器の作者とは関係ない、非公式のツールです。

## 使い方

「変換器を動かすコンピュータ」(以下、送信マシン) と、
「ツールなどでメモリマップドファイルの情報を受け取るコンピュータ」(以下、受信マシン)
のそれぞれで「電車の橋渡し」を起動します。

「ネットワーク」の設定を行います。

送信マシン・受信マシンで、「ポート」を同じ値に設定します。
送信マシンの「通信相手アドレス」を受信マシンのIPアドレスに、
受信マシンの「通信相手アドレス」を送信マシンのIPアドレスに、それぞれ設定します。

「通信相手アドレス」と「ポート」を設定したら、
送信マシンでは「送信 (変換器側)」を、
受信マシンでは「受信 (ゲーム側)」を選択します。
これらを選択すると、アドレスとポートの設定をロックし、通信を開始します。

「送信 (変換器側)」を選択すると、「キー送信ウィンドウを開く」ボタンが使えるようになります。
このボタンを押すと、キー操作を受信マシンに転送するためのウィンドウが開きます。
このウィンドウで行われたキー操作が受信マシンに転送されます。
変換器によるキー操作を転送するため、このウィンドウをアクティブにしておきます。

## 送信情報

メモリマップドファイル上の情報のうち、変換器から対応ツールに渡す情報の状態を表します。

### ブレーキ

ブレーキのノッチの位置を表します。

### マスコン

マスコン (力行) のノッチの位置を表します。

### コントローラ

使用しているコントローラの種類、またはマスコンとブレーキの段数を表します。

### 拡張ブレーキ

逆山陽や旅情など、通常のブレーキより段数が多いブレーキの情報を表します。

## 受信情報

メモリマップドファイル上の情報のうち、対応ツールから変換器に渡す情報の状態を表します。

### 戸じめ

戸じめ灯が点灯しているかを表します。

### 振動左

コントローラの左側を振動させるかを表します。

### 振動右

コントローラの右側を振動させるかを表します。

### LED

新幹線コントローラのLEDが10個並んだ部分のうち、何個を点灯させるかを表します。

### ATC

ATCの表示速度 (km/h) を表します。

### 速度

現在走行中の速度 (km/h) を表します。

## 高度な設定

「電車の橋渡し」を普通に使う分には変更しなくてよい設定を行います。
主に実験用です。

### ローカルポート

UDP 通信を受信するポートの番号を設定します。
他のマシンと通信する場合は、「送信先ポートと同じ」でOKです。
1台のマシンで「電車の橋渡し」を複数起動して実験を行う際、
この設定を用いて送信ポートと受信ポートを分けることができます。

### 送信情報サイズ

変換器から対応ツールに渡す情報の、メモリマップドファイル上のバイト数を設定します。
将来仕様が拡張され、データサイズが増えた際、この設定を変更することで転送できる可能性があります。

### 受信情報サイズ

対応ツールから変換器に渡す情報の、メモリマップドファイル上のバイト数を設定します。
将来仕様が拡張され、データサイズが増えた際、この設定を変更することで転送できる可能性があります。

### メモリマップドファイル名

「電車の橋渡し」が読み書きするメモリマップドファイルの名前を設定します。
1台のマシンで「電車の橋渡し」を複数起動して実験を行う際、
この設定を用いて参照するメモリマップドファイルを分けることができます。

### ミューテックスを作成する

チェックを入れると、「電車の橋渡し」の起動中に指定した名前のミューテックスを作成します。
TSXBIN https://www.net3-tv.net/~m-tsuchy/tsuchy/dlpage.htm
のメモリーマップドファイルモードでメモリマップドファイルの内容を確認する際、
ミューテックスが無いと内容を表示してくれないようなので、ミューテックスを用意できるようにしました。

### 読み書き時ミューテックスをロック

「ミューテックスを作成する」およびこの項目にチェックを入れると、
メモリマップドファイルを読み書きする際に作成したミューテックスをロックします。

### ミューテックス名

「ミューテックスを作成する」で作成するミューテックスの名前を設定します。

## 配布元

https://github.com/mikecat/densya_relay
