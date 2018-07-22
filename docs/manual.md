# マニュアル

## 開発環境の構築

1. [Unity](https://unity3d.com/jp) (Version 2018.1.6f1) と
[Rider](https://www.jetbrains.com/rider/) (Version 2018.1.3) をインストールする．

2. リポジトリをクローンする．
```
$ git clone https://github.com/Team-OW/NumberBullet2
```

3. 必要バージョンにcheckoutし，プロジェクトをUnityで開く

  - Unity -> Preferences -> External Tools -> External Script EditorでJetBrains Riderを選ぶ．


4. RiderでNumberBullet2.slnを開く．

## Code Style

### フォーマット

- Riderにおいて，Assets/Scriptsを右クリック -> Code Cleanup -> Built-in Reformat Codeを選択しOKを押すと，自動的にフォーマットされる．(`Alt+Command+L`でも可能)

- コミットする前に，毎回フォーマットを行い体裁を整える．

- .editorconfigがフォーマット方法を管理している．

### 型宣言

- built-in typesのみvarを用いる．他は，明示的な型宣言をする．

- Rider -> Preferences -> Editor -> Code Style -> C# -> Code Styleにおいて
> For built-in types -> Use 'var'
>
> For simple types -> Use explicit types
>
> Elsewhere -> Use explicit types

  以上のように設定することで，Riderが修正を下波線で促してくれる．修正に関しては，下波線部分にカーソルを合わせ`Alt+Enter`を押すことで半自動化できる．

### 命名規則
- 以下の規則に従う．
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

- また，Rider -> Preferences -> Editor -> Code Style -> C# -> Namingにおいて，
以上のような設定をすると，型宣言同様にRiderで半自動化可能．

### その他

- namespaceはディレクトリ構造と統一させる．

- アクセス修飾子は極力意識する．デフォルトで型宣言同様にRiderで半自動化可能．
