package service

import (
	"fmt"  
    "github.com/github-user/agenda/entity"
)

var Login_flag bool 

func init() {
	entity.Init()
	var temp string
	temp = entity.GetCurUser()
	if temp == "" {
		Login_flag = false
	} else {
		Login_flag = true
	}
}

func RegisterUser(name string, password string, email string, phone string) {
	err := entity.RegisterUser(name, password, email, phone)
	if err != nil {
		fmt.Println(err)
	} else {
		fmt.Println("Register sucessfully!")
	}
}

func Login(name string, password string) {
	err := entity.Login(name, password)
	if err != nil {
		fmt.Println(err)
	} else {
		fmt.Println("Login sucessfully!")
	}
}

func GetFlag() bool {
	return Login_flag
}