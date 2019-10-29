package entity

import (
	"encoding/json"
	"os"
	"log"
	"errors"
	"regexp"
	"fmt"
	"io"
)

//用户结构体
type User struct {
	Name	string
	Password string
	Email string
	Telphone string
}

var users []User
var curUser User
var usersFile = "entity/data/Users.json"
var curUserFile = "entity/data/curUser.json"

func Init() {
	//读取注册用户数据
	getUsersData()
	//读取登录用户数据
	getCurUsersData()
}

func getUsersData() {
	//打开文件
	filePtr, err := os.Open(usersFile)
	if err!=nil{
		log.Fatal(err)
	}
    defer filePtr.Close()
 
    //创建该文件的json解码器
	dec := json.NewDecoder(filePtr)
    for {
        var user User
        if err := dec.Decode(&user); err == io.EOF {
            break
        } else if err != nil {
            log.Fatal(err)
		}
		users = append(users, user)
    }
}

func getCurUsersData() {
	open, err := os.Open(curUserFile)
	if err!=nil{
		log.Fatal(err)
	}
	//代码执行最后将文件关闭
	defer open.Close()
 
	//生成文件解码器
	decoder := json.NewDecoder(open)
	err1:= decoder.Decode(&curUser)
	if err1!=nil && err1 != io.EOF{
		log.Fatal(err1)
	}
}

func GetCurUser() string {
	return curUser.Name
}

func RegisterUser(name string, password string, email string, phone string) error {
	if GetCurUser() != "" {
		return errors.New("Please log out first.")
	}
    if name == "" {
            return errors.New("Empty user name.")
    }
	if CheckUser(name) {
		return errors.New("User name already exists.")
	}
	if len(password) < 6 {
		return errors.New("Password length cannot be less than 6.")
	}
	if !VerifyEmailFormat(email) {
		return errors.New("Email is invalid.")
	}
	if !VerifyMobileFormat(phone) {
		return errors.New("Telehone is invalid.")
	}
	//persist register information
	users = append(users, User{name, password, email, phone})

	file, err := os.OpenFile(usersFile, os.O_WRONLY|os.O_TRUNC|os.O_CREATE, 0666)
	if err != nil {
		fmt.Println(err)
	}
	defer file.Close()

	for _, v := range users {
		json, err := json.Marshal(v)
		if err != nil {
			fmt.Println(err)
		}
		file.WriteString(string(json))
		file.WriteString("\n")
	}
	return nil
}

func Login(name string, password string) error {
	if name == "" {
		return errors.New("Empty user name.")
	}
	if !CheckUser(name) {
		return errors.New("User name don't exists.")
	} else {
		if  !CheckPassword(name, password) {
			return errors.New("The password is wrong!")
		} else {
			temp := getUser(name)
			open, err := os.OpenFile(curUserFile, os.O_APPEND|os.O_WRONLY, os.ModeAppend)
			if err!=nil{
				log.Fatal(err)
			}
			//代码执行最后将文件关闭
			defer open.Close()
		
			//生成文件器
			encoder := json.NewEncoder(open)
			err1:= encoder.Encode(temp)
			if err1 != nil {
				log.Fatal(err1)
			}
		} 
	}
	return nil
}

func getUser(name string) User {
	for _, i := range users {
		if i.Name == name {
			return i
		}
	}
	return User{}
}

func CheckUser(name string) bool {
	for _, i := range users {
		if i.Name == name {
			return true
		}
	}
	return false
}

func CheckPassword(name string, password string) bool {
	for _, i := range users {
		if i.Name == name {
			if i.Password == password {
				return true
			} else {
				return false
			}
		}
	}
	return false
}

//email verify
func VerifyEmailFormat(email string) bool {
	//pattern := `\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*` //匹配电子邮箱
	pattern := `\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*`

	reg := regexp.MustCompile(pattern)
	return reg.MatchString(email)
}
//mobile verify
func VerifyMobileFormat(mobileNum string) bool {
	regular := `^(1[3|4|5|8][0-9]\d{4,8})$`

	reg := regexp.MustCompile(regular)
	return reg.MatchString(mobileNum)
}