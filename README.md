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
> Enums -> EUpperCamelCase
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
> Fields (not private) -> lowerCamelCase
>
> Instance fields (private) -> _lowerCamelCase
>
> Static field (private) -> _lowerCamelCase
>
> Constant fields (not private) -> ALL_UPPER
>
> Constant fields (private) -> ALL_UPPER
>
> Static readonly fields (not private) -> UpperCamelCase
>
> Static readonly fields (private) -> UpperCamelCase
>
> Enum members -> UpperCamelCase
>
> Local functions -> UpperCamelCase
>
> All other entities -> UpperCamelCase

### その他
- `namespace` はディレクトリ構造と統一させる

### Code Style
[Astyle](http://astyle.sourceforge.net)と[git-hooks](https://git-scm.com/docs/githooks)を用いて自動フォーマットしている．
クローン後に，以下のコマンドを実行してください

```
$ git config core.hooksPath .githooks
```

フォーマット形式は，[Artistic Style](http://astyle.sourceforge.net/astyle.html#_Options)を元に`.astylerc`に記述されている
