# マニュアル

## 開発環境の構築

1. [Unity](https://unity3d.com/jp) (`2018.1.6f1`) と
[Rider](https://www.jetbrains.com/rider/) (`2018.1.3`) をインストールする．

2. リポジトリをクローンする．
```
$ git clone https://github.com/Team-OW/NumberBullet2
```

3. 必要バージョンにcheckoutし，プロジェクトをUnityで開く

  - `Unity -> Preferences -> External Tools -> External Script Editor`でJetBrains Riderを選ぶ．


4. RiderでNumberBullet2.slnを開く．

## Code Style

### フォーマット

- Riderにおいて，`Assets/Project/Scripts`を右クリックし，`Code Cleanup`で`Built-in: Reformat Code`を選択しOKを押すと，自動的にフォーマットされる．

- プルリクを出す前にフォーマットを行い，コードの体裁を整える．

- `.editorconfig`がフォーマット方法を管理している．

### 型宣言

- 原則`var`を用いる．

- Rider -> Preferences -> Editor -> Code Style -> C# -> Code Styleにおいて

> For built-in types -> Use 'var'
>
> For simple types -> Use 'var'
>
> Elsewhere -> Use explicit types

  以上のように設定することで，Riderが修正を下波線で促してくれる．修正に関しては，下波線部分で`Alt+Enter`を押すことで半自動化できる．

### 命名規則
- Rider -> Preferences -> Editor -> Code Style -> C# -> Namingにおいて

> Type and namespace -> UpperCamelCase
>
> Interfaces -> IUpperCamelCase
>
> Type parameters -> TUpperCamelCase
>
> Methods, properties and events -> UpperCamelCase
>
> Local variables -> lowerCamelCase
>
> Local constants -> lowerCamelCase
>
> Parameters -> lowerCamelCase
>
> Fields -> lowerCamelCase
>
> Constant fields -> ALL_UPPER
>
> Static readonly fields -> UpperCamelCase
>
> Enum members -> UpperCamelCase
>
> Local functions -> UpperCamelCase
>
> All other entities -> UpperCamelCase

以上のように設定することで，型宣言同様にRiderで半自動化可能．

### その他

- `namespace`はディレクトリ構造と統一させる．

- アクセス修飾子は極力意識する．デフォルトで型宣言同様にRiderで半自動化可能．
