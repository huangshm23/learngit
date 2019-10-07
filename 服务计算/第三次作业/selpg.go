package main

import (
	"bufio"
	"flag"
	"fmt"
	"io"
	"os"
	"os/exec"
)

type selpgargs struct {
	start_page  int         //开始的页编号
	end_page    int         //结束的页编号
	in_filename  string
	print_dest string
	page_len    int     /* default value, can be overriden by "-l number" on command line */
	page_type bool  /* 'l' for lines-delimited, 'f' for form-feed-delimited */
}

var progname string     /* program name, for error messages */

//show the usage of the command selpg
func usage() {
	fmt.Printf("Usage of %s:\n\n", progname)
	fmt.Printf("%s is a tool to select pages from what you want.\n\n", progname)
	fmt.Printf("Usage:\n\n")
	fmt.Printf("\tselpg -s=Number -e=Number [options] [filename]\n\n")
	fmt.Printf("The arguments are:\n\n")
	fmt.Printf("\t-s=Number\tStart from Page <number>.\n")
	fmt.Printf("\t-e=Number\tEnd to Page <number>.\n")
	fmt.Printf("\t-l=Number\t[options]Specify the number of line per page.Default is 72.\n")
	fmt.Printf("\t-f\t\t[options]Specify that the pages are sperated by \\f.\n")
	fmt.Printf("\t[filename]\t[options]Read input from the file.\n\n")
	fmt.Printf("If no file specified, %s will read input from stdin. Control-D to end.\n\n", progname)
}

//initial flags
func FlagInit(args *selpgargs) {
	flag.Usage = usage
	flag.IntVar(&args.start_page, "s", -1, "Start page.")
	flag.IntVar(&args.end_page, "e", -1, "End page.")
	flag.IntVar(&args.page_len, "l", 72, "page_len.")
	flag.BoolVar(&args.page_type, "f", false, "page_type")
	flag.StringVar(&args.print_dest, "d", "", "print_dest")
	flag.Parse()
}

func process_args(args *selpgargs) {
	if args == nil {
        fmt.Fprintf(os.Stderr, "\n[Error]The args is nil!Please check your program!\n\n")
        os.Exit(1)
    }
    if args.start_page == -1 || args.end_page == -1 {
		fmt.Fprintf(os.Stderr, "\n[Error]:%s, not enough arguments\n\n", progname)
		flag.Usage()
		os.Exit(2)
	}

	if os.Args[1][0] != '-' || os.Args[1][1] != 's' {
		fmt.Fprintf(os.Stderr, "\n[Error]:%s, 2st arg should be -sstart_page\n\n", progname)
		flag.Usage()
		os.Exit(3)
	}

	end_index := 2
	if len(os.Args[1]) == 2 {
		end_index = 3
	}

	if os.Args[end_index][0] != '-' || os.Args[end_index][1] != 'e' {
		fmt.Fprintf(os.Stderr, "\n[Error]:%s, 3st arg should be -eend_page\n\n", progname)
		flag.Usage()
		os.Exit(4)
	}

	if args.start_page > args.end_page || args.start_page < 0 ||
		args.end_page < 0 {
		fmt.Fprintln(os.Stderr, "\n[Error]:Invalid arguments")
		flag.Usage()
		os.Exit(5)
	}

}

func process_input(args *selpgargs) {
	var stdin io.WriteCloser
	var err error
	var cmd *exec.Cmd

	if args.print_dest != "" {
		cmd = exec.Command("cat", "-n")
		stdin, err = cmd.StdinPipe()
		if err != nil {
			fmt.Println(err)
		}
	} else {
		stdin = nil
	}

	if flag.NArg() > 0 {
		args.in_filename = flag.Arg(0)
		output, err := os.Open(args.in_filename)
		if err != nil {
			fmt.Println(err)
			os.Exit(1)
		}
		reader := bufio.NewReader(output)
		if args.page_type {
			for pageNum := 0; pageNum <= args.end_page; pageNum++ {
				line, err := reader.ReadString('\f')
				if err != io.EOF && err != nil {
					fmt.Println(err)
					os.Exit(1)
				}
				if err == io.EOF {
					break
				}
				printOrWrite(args, string(line), stdin)
			}
		} else {
			count := 0
			for {
				line, _, err := reader.ReadLine()
				if err != io.EOF && err != nil {
					fmt.Println(err)
					os.Exit(1)
				}
				if err == io.EOF {
					break
				}
				if count/args.page_len >= args.start_page {
					if count/args.page_len > args.end_page {
						break
					} else {
						printOrWrite(args, string(line), stdin)
					}
				}
				count++
			}

		}
	} else {
		scanner := bufio.NewScanner(os.Stdin)
		count := 0
		target := ""
		for scanner.Scan() {
			line := scanner.Text()
			line += "\n"
			if count/args.page_len >= args.start_page {
				if count/args.page_len <= args.end_page {
					target += line
				}
			}
			count++
		}
		printOrWrite(args, string(target), stdin)
	}

	if args.print_dest != "" {
		stdin.Close()
		cmd.Stdout = os.Stdout
		cmd.Run()
	}
}

func printOrWrite(args *selpgargs, line string, stdin io.WriteCloser) {
	if args.print_dest != "" {
		stdin.Write([]byte(line + "\n"))
	} else {
		fmt.Println(line)
	}
}

func main() {
	progname = os.Args[0]    /* save name by which program is invoked, for error messages */
	var args selpgargs  
	FlagInit(&args)         //初始化参数
	process_args(&args)      //处理参数
	process_input(&args)     //根据用户输入的各个参数进行相应的操作
}
