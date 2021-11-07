import sys
import time
import os
import pandas as pd
import ctypes as c
if __name__=="__main__":
    
    
    #a = sys.argv[1]
    #b = sys.argv[2]
    
    a="12"
    b="14"
    
    column_names = [a, b]
    return_dict = []
    for i in range(5000):        
        return_dict.append([float(a)*i,float(b)+i-0.5])
        print(f"succeeded ")
    
    pandas_container = pd.DataFrame(list(return_dict), columns=column_names)
    print(pandas_container)
    #time.sleep(5)
    
    # if file does not exist write header
    # else it exists so append without writing the header
    if not os.path.isfile('test.csv'):
        pandas_container.to_csv('test.csv', header=column_names, index=False, sep=";")
    else:
        not_written = True
        while not_written:
            try:
                pandas_container.to_csv('test.csv', mode='a', header=False, index=False, sep=";")
                not_written = False
            except PermissionError:
    
                input("not written, close and try again: ")
    




   