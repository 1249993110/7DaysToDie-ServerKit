# How To Contribute Code

## Opening issues
- Before opening issue please search for the keyword see whether the issue you encountered has been reported already. :pray:

- Issues should provide sufficient information to reproduce the issue, so that our developers can help you verify and fix it

**Insufficient information issue WILL BE CLOSED directly because we cannot reproduce them**

Next section is for those who wants to help develop 7DaysToDie-ServerKit.

## Pre-request
For back end develop you should be having:
- Visual Studio 2022

For front end develop you should be having:
- Visual Studio Code
- NodeJS >= 18
- Git > v2.20

## Clone the repository
1. Click the fork button on the top right of this page.
2. Download the project to your machine.
3. Run commands below to bootstrap the front end project.

```shell
cd $THE_PROJECT_PATH_YOU_DOWNLOADED
# If you haven't installed yet
npm install
```

## Getting Started developing
Ah, now you have installed the code correctly, you can choose:


- Compile the source code
- Modify and update the translation.

## Pulling request
After you done your coding section, please note:

1. Update the tests to cover all cases
2. Update co-responding documentation if you are making changes against API
3. Write a comprehensive commit message
4. Push your local changes to your remote, and then pull request to the upstream.
5. In the description section, you can add more information about your changes, to help the reviewers for better understanding the context here.
6. **PRs with unformatted commit message WILL NOT BE ACCEPTED**

A good commit message enables us:
- To understand what the contributor is trying to do
- Automatically generates change log

Please refer to [Conventional Commits](https://www.conventionalcommits.org)&emsp;[Keeping git commit history clean](https://about.gitlab.com/blog/2018/06/07/keeping-git-commit-history-clean) for more information.

## How can I help contribute translated content?
1. Create an account on [Crowdin](https://crowdin.com), it is recommended that you use your GitHub account to authorize Crowdin.
2. Go to [7DaysToDie-ServerKit](https://crowdin.com/project/7daystodie-serverkit) project.
3. Choose the language you want to contribute to.
4. Find the file you want to translate.
5. Do the translation.
   
That simple, and the UI is very intuitive to use, you should have no trouble using it. After you submit your work, it would be online once the translation is approved by proofreader.

## How can I become a proofreader?
You can [raise an issue](https://crowdin.com/project/7daystodie-serverkit/discussions) on Crowdin to us for becoming a proofreader of the language you wish to be.

That's all, thanks for your contribution! 🤩
