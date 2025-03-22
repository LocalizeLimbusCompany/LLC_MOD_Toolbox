# 关于 `零协会 API` 的二三事
## 怎样使用零协会 API
零协会的 API 只包含获取版本信息和下载两种功能，使用方法如下：

### 获取版本信息

仅为示例，实际请求可能会改变

请求地址：

```url
GET https://api.zeroasso.top/v2/get_api/get/{repo}
```

可选参数：`repos/LocalizeLimbusCompany/LLC_Mod_Toolbox/releases/latest`、`repos/LocalizeLimbusCompany/LocalizeLimbusCompany/releases/latest`、`repos/LocalizeLimbusCompany/BepInEx_For_LLC/releases/latest`、`repos/LocalizeLimbusCompany/LLC_ChineseFontAsset/releases/latest`和`repos/LocalizeLimbusCompany/LLC_Release/releases/latest`

返回数据：JSON 格式，包含版本号、发布时间、下载地址等信息

当然，以上的 JSON 文件标签大多都可以无视，只需要关注 `body` 即可。



```url
GET https://api.zeroasso.top/v2/resource/get_version
```

返回数据：

```json
{
  "notice":"更新提示",
  "resource_version":"用于检查的版本",
  "version":"用于声明的版本"
}
```



```url
GET https://api.zeroasso.top/v2/grey_test/get_token?code={token}
```

参数：零协会私域提供

返回数据：

- 失败

```json
{
  "message": "Token not found"
}
```

- 成功

```json
{
  "file_name": "在客户端无效",
  "note": "更新提示",
  "status": "当前状态，只存在 test 一种情况？"
}
```



### 下载文件

仅展示 `Api.Custom` 的下载方法，不再展示 GitHub 以及其镜像站的下载方法。

请求地址：

```url
GET https://api.zeroasso.top/v2/download/files?file_name={fileName}
```

可选参数：`BepInEx-IL2CPP-x64.7z`、`LimbusLocalize_BIE.7z`、`tmpchinesefont_BIE.7z`和`Resource/LimbusLocalize_Resource_latest.7z`

返回数据：文件流，直接下载即可



```url
GET https://api.zeroasso.top/v2/grey_test/get_file?code={token}
```

参数：与上述同名一致

返回数据：同上

> [!IMPORTANT]
>
> 零协会已声明暂时不会处理 GitHub 的下载，暂时不用管别的

> [!NOTE]
>
> 灰度测试其实写错了，暂时不管好了