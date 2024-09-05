# 如何贡献

## 打开问题
- 打开问题之前，请搜索关键字，查看您遇到的问题是否已被报告。 :pray:

- 问题应提供足够的信息来重现问题，以便我们的开发人员可以帮助您验证和修复它

**信息不足的问题将直接关闭，因为我们无法重现它们**

下一部分适用于那些想要帮助开发 7DaysToDie-ServerKit 的人。

## 开发环境
对于后端开发，您应该拥有：
- Visual Studio 2022

对于前端开发，您应该拥有：
- Visual Studio Code
- NodeJS >= 18
- Git > v2.20

## 克隆存储库
1. 单击此页面右上角的 fork 按钮。
2. 将项目下载到您的机器。
3. 运行以下命令来引导前端项目。

```shell
cd $THE_PROJECT_PATH_YOU_DOWNLOADED
# 如果您尚未安装
npm install
```

## 开始开发
啊，现在您已经正确安装了代码，您可以选择：

- 编译源代码
- 修改和更新翻译。

## 拉取请求
完成编码部分后，请注意：

1. 更新测试以涵盖所有情况
2. 如果您对 API 进行更改，请更新相应的文档
3. 编写全面的提交消息
4. 将本地更改推送到远程，然后将请求拉取到上游。
5. 在描述部分，您可以添加有关更改的更多信息，以帮助审阅者更好地理解此处的上下文。
6. **带有未格式化提交消息的 PR 将不被接受**
   
一个好的 Git 提交信息能够使我们：
- 了解贡献者试图做什么
- 自动生成更改日志

请参阅 [通用惯例](https://www.conventionalcommits.org)&emsp;[保持 git 提交历史简洁](https://about.gitlab.com/blog/2018/06/07/keeping-git-commit-history-clean)

# 翻译
## 我怎么才能帮忙贡献翻译的内容？
1. 在 [Crowdin](https://crowdin.com) 上创建一个账号，推荐使用你的 GitHub 账号第三方登录。
2. 进入 [7DaysToDie-ServerKit](https://crowdin.com/project/7daystodie-serverkit) 项目。
3. 选择你想要贡献翻译的语言。
4. 选择你想要贡献翻译的具体文件。
5. 开始翻译。
   
这个过程很简单，Crowin 的用户界面也非常直观，还可以选择中文模式，你应该很容易就可以学会如何使用它。当你提交完你的翻译之后，它会在翻译审核通过之后上线。

## 我怎么样能帮助翻译审核？
你可以通过在 Crowdin [发起 issue](https://crowdin.com/project/7daystodie-serverkit/discussions) 的方式通知我们，issue 内请告知你希望成为哪个语言的校对官。

就这样，感谢您的贡献！🤩
