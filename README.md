# OpenWeight

## Fit8sと通信して体重を自動取得します

![image](https://user-images.githubusercontent.com/51302983/230729551-a1a3d8a1-cab8-463c-857e-5709ee2f2aae.png)
![image](https://user-images.githubusercontent.com/51302983/230729561-e2092a33-fbc5-4271-8376-ddeaa2890d9f.png)

## 現状
常時通信して体重を自動取得することができます
```
dotnet OpenWeight.Cli.dll daemon AA:89:5E:18:DA:91 hci0
```

## TODO
### Discordの名前を自動で編集する
```Aijkl@70キロ```にしたいです
### DBに保存してビューワーを作る
グラフとかサボった率とか色々解析


## 通信について
BLE (Bluetooth Low Energy)のアドバタイズパケットとして**ブロードキャスト**で体重が飛んできます
特に認証とか暗号化とかはありません、体重なので重要ではないとの認識でブロードキャストしてる見たいです

アドレスが10-12までです、リトルエンディアンで送られてくるのでビックエンディアンに変換してあげる必要があります
![image](https://user-images.githubusercontent.com/51302983/230730173-fdd0a5df-9a27-46e2-83fa-996afe4f77b8.png)
