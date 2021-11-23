import sys
import time
import os
import pandas as pd
import ctypes as c

class Test():
    var1 = 1

    var1_c = c.c_float(1)

    arr=[1,2]
    arr_c=(c.c_float * 2)(*[1,2])

    def __init__(self):
        pass
        #Test.var1 = _var1

    

if __name__=="__main__":
    
    args_list=[]
    with open("args.txt", "r") as file1:
        args_list = file1.read().split(" ")
    print("length of args list")
    print(len(args_list))
    for i in args_list:
        print(i)
   