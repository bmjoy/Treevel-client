# NumberBullet2

## Version
- `Unity`: `2019.1.9f1`  
- `.NET`: `4.x`

## Code Style
- `.editorconfig` が最低限のフォーマット方法を管理
- プルリクを出す前にはフォーマットを行い，コードの体裁を整える

### 命名規則
- 以下に従ってください

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

### その他
- `namespace` はディレクトリ構造と統一させる．

### 自動フォーマット
本プロジェクトは[Astyle](http://astyle.sourceforge.net)と[git-hooks](https://git-scm.com/docs/githooks)を用いて自動フォーマットを行っています。
クローンした直後に以下のコマンドを実行してください。
```
$ git config core.hooksPath .githooks
```
