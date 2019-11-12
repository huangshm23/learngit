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
