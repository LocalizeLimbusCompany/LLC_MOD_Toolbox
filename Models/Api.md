# 关于 `零协会 API` 的二三事
## 怎样使用零协会 API
零协会的 API 只包含获取版本信息和下载两种功能，使用方法如下：

### 获取版本信息

仅为示例，实际请求可能会改变

请求地址：

```url
GET https://api.zeroasso.top/v2/get_api/get/{repo}
```

可选参数：`repos/LocalizeLimbusCompany/LLC_Mod_Toolbox/releases/latest`、`repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/latest`、`repos/LocalizeLimbusCompany/BepInEx_For_LLC/releases/latest`和`repos/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/latest`

返回数据：JSON 格式，包含版本号、发布时间、下载地址等信息

当然，以上的 JSON 文件标签大多都可以无视，只需要关注 `body` 即可。



```url
GET https://api.zeroasso.top/v2/resource/get_version
```

返回数据：

```json
{
  "notice":"更新提示",
  "resource_version":"用于区分文件的版本",
  "version":"用于检查的版本"
}
```

### 下载文件

仅展示 `Api.Custom` 的下载方法，不再展示 GitHub 以及其镜像站的下载方法。

请求地址：

```url
GET https://api.zeroasso.top/v2/download/files?file_name={fileName}}
```

可选参数：`BepInEx-IL2CPP-x64.7z`、`LimbusLocalize_BIE.7z`、`tmpchinesefont_BIE.7z`和`Resource/LimbusLocalize_Resource_latest.7z`

返回数据：文件流，直接下载即可

> [!IMPORTANT]
>
> 零协会已声明暂时不会处理 GitHub 的下载，暂时不用管别的