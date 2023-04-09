# OpenWeight

## Fit8sと通信して体重を自動取得します

![image](https://user-images.githubusercontent.com/51302983/230729551-a1a3d8a1-cab8-463c-857e-5709ee2f2aae.png)
![image](https://user-images.githubusercontent.com/51302983/230779046-4635b56a-21f4-480b-855d-6748d6186817.png)


常時通信して体重を自動取得することができます

## --connection-string
このオプションを有効にするとDBに体重を記録しまう
```
dotnet OpenWeight.Cli.dll daemon AA:89:5E:18:DA:91 hci0 --connection-string "Data Source=weight.db" --discord-settings-path ./discord.json
```

## --discord-settings-path
このオプションを有効にすると設定ファイルに基づいてユーザーのニックネームを変更します  
ニックネームの変更にはBOTにニックネームの変更権限が与えられている必要があります
```
{
  "token": "",
  "guildUsers": [
    {
      "guildId": 836823009527332884,
      "userId": 536802665498411013,
      "scribanPattern": "みかん@{{event_data.data.weight}}キロ"
    }
  ],
  "intervalMs": 10000
}
```

## 

## 通信について
BLE (Bluetooth Low Energy)のアドバタイズパケットとして**ブロードキャスト**で体重が飛んできます
特に認証とか暗号化とかはありません、体重なので重要ではないとの認識でブロードキャストしてる見たいです

アドレスが10-12までです、リトルエンディアンで送られてくるのでビックエンディアンに変換してあげる必要があります
![image](https://user-images.githubusercontent.com/51302983/230730173-fdd0a5df-9a27-46e2-83fa-996afe4f77b8.png)
