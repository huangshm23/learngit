## 1. 框架选择：Martini
Martini是快速构建模块化web应用与服务的开发框架。其特点是模块化，方便与其他框架结合，同时其路由设计十分简洁实用，而灵活的中间件则给我们更多的自由发挥空间。
服务器代码：

```go
package main

import (
	"github.com/go-martini/martini"
	flag "github.com/spf13/pflag"
	"fmt"
)

func main() {
  m := martini.Classic()
  port := flag.String("port", "", "port:default is 3000") //传入端口号
  flag.Parse()
  m.Get("/", func() string {
    return "Hello world!"
  })
  m.Get("/hello/(?P<name>[a-zA-Z]+)", func(params martini.Params) string {
	return fmt.Sprintf ("Hello %s\n", params["name"])
  })
  if *port == "" {
	m.Run()
  } else {
	m.RunOnAddr(":" + *port)
  }
}
```

## 2. curl 测试
1）运行服务器：
命令：`go run server.go --port 9090`
结果：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191112111741110.png)
2）浏览器访问测试：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191112111805168.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
3）curl命令测试：
命令：`curl -v http://localhost:9090/hello/testuser`
结果：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191112111915580.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
## 3. ab 测试
1）测试环境：
Ubuntu
2）测试命令：
`ab -n 1000 -c 100 http://localhost:9090/hello/your`
3）测试结果：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191112112026934.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
4）重要参数解释：
|字段	|含义 |
|--------|--------|
|Server Software |服务器软件名|
|Server Hostname	|服务器主机名|
|Server Port	|服务器端口 |
|Document Path	|文件相对路径|
|Document Length	|文件大小|
|Concurrency Level	|并发等级|
| Time taken for tests| 测试一共花的时间|
|Complete requests | 完成的测试数 |
| Failed requests | 失败的测试数 |
|Total transferred| 传输的总的字节大小|
|HTML transferred| 传输的HTML文件大小|
|Requst per second	|平均每秒的请求个数|
|Time per request| 平均每个请求花的时间|
|Transfer rate|传输速度|
|Connection Times	|表内描述了所有的过程中所消耗的最小、中位、最长时间。|
|Percentage of the requests served within a certain time	|每个百分段的请求完成所需的时间|
