

## 准备开始

	1.你需要安装对应的IDE,比如VS2022，才能打开此项目，然后配置ExampleForm.HttpApi.Host为启动项目
	2.查看ExampleForm.EntityFrameworkCore是否有Migrations
	3.如果2没有，则需要切换到程序包管理器控制台，然后选择项目ExampleForm.EntityFrameworkCore
	4.然后输入add-migration database_init -Context SqliteDbContext
	5.4中的命令你可以基于自己的实际调整，执行完成后，看看有没有报错
	6.然后你就可以点击最上方的开始执行了
	7.启动后，默认控制台会输出一大堆东西，最后会告诉你监听在2222端口
	8.然后你就可以使用浏览器打开http://localhost:22222/page/index.html
	9.如果需要你登陆，则直接使用默认的账号密码登陆即可！
	10.以下是一些归纳的特性的案例测试地址:



## select lselect selects

```
http://localhost:22222/page/pasteform/view.html?path=select
```

## text textarea html markdown

```
http://localhost:22222/page/pasteform/view.html?path=text

```

## date daterange datetime

```
http://localhost:22222/page/pasteform/view.html?path=date

```

更多案例详见/page/index.html和对应得dto


### 待思考
	1.如果一个对象，array 或者 list 要以组的方式显示，则标注说明即可???
	2.然后按照xxx1 xxx2进行分组！
	3.注意这个字段本身是否有分组特性，如果有则直接继承！！！


## 简介
	荣誉 优点  特点
## 问题
	案例问题
## 如何处理

## 进阶用法

## 不足之处

## 请喝咖啡

